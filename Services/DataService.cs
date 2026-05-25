using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LibrarySystem.Services;

public class DataService(LibraryDbContext db)
{
    // ── Auth ──

    public static string HashPassword(string password)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }

    public Librarian? Authenticate(string username, string password)
    {
#pragma warning disable CA1862 
        return db.Librarians.FirstOrDefault(l =>
            l.Username.ToLower() == username.ToLower() &&
            l.PasswordHash == HashPassword(password));
#pragma warning restore CA1862
    }

    // ── Books ──

    public List<Book> GetBooks(bool includeDeleted = false)
        => [.. db.Books
              .Where(b => includeDeleted || !b.IsDeleted)
              .OrderBy(b => b.Title)];

    public Book? GetBook(int id)
        => db.Books.FirstOrDefault(b => b.Id == id);

    public Book AddBook(Book book)
    {
        var existing = db.Books.FirstOrDefault(b => b.ISBN == book.ISBN && !b.IsDeleted);
        if (existing != null)
        {
            existing.CopyCount += book.CopyCount;
            db.SaveChanges();
            return existing;
        }
        db.Books.Add(book);
        db.SaveChanges();
        return book;
    }

    public void UpdateBook(Book book)
    {
        db.Books.Update(book);
        db.SaveChanges();
    }

    public (bool success, string message) DeleteBookCopy(int bookId, bool deleteAll)
    {
        var book = GetBook(bookId);
        if (book == null) return (false, "A könyv nem található.");

        int loanedCount = db.Loans.Count(l => l.BookId == bookId && l.ReturnDate == null);

        if (deleteAll)
        {
            if (loanedCount > 0)
                return (false, $"Nem törölhető: {loanedCount} példány ki van kölcsönözve.");
            book.IsDeleted = true;
            book.CopyCount = 0;
        }
        else
        {
            int available = book.CopyCount - loanedCount;
            if (available <= 0)
                return (false, "Nincs szabad példány. Minden ki van kölcsönözve.");
            book.CopyCount--;
            if (book.CopyCount == 0) book.IsDeleted = true;
        }

        db.SaveChanges();
        return (true, "Sikeres törlés.");
    }

    public List<Book> SearchBooks(string field, string text)
    {
        return [.. db.Books
            .Where(b => !b.IsDeleted)
            .AsEnumerable()
            .Where(b => field switch
            {
                "Author" => b.Author.Contains(text, StringComparison.OrdinalIgnoreCase),
                "ISBN"   => b.ISBN.Contains(text, StringComparison.OrdinalIgnoreCase),
                "Id"     => b.Id.ToString().Contains(text),
                _        => b.Title.Contains(text, StringComparison.OrdinalIgnoreCase)
            })];
    }

    public int GetAvailableCopies(int bookId)
    {
        var book = GetBook(bookId);
        if (book == null) return 0;
        int loaned = db.Loans.Count(l => l.BookId == bookId && l.ReturnDate == null);
        return Math.Max(0, book.CopyCount - loaned);
    }

    // ── Members ──

    public List<LibraryMember> GetMembers(bool includeDeleted = false)
        => [.. db.Members
              .Where(m => includeDeleted || !m.IsDeleted)
              .OrderBy(m => m.Name)];

    public LibraryMember? GetMember(int id)
        => db.Members.FirstOrDefault(m => m.Id == id);

    public LibraryMember AddMember(LibraryMember member)
    {
        db.Members.Add(member);
        db.SaveChanges();
        return member;
    }

    public void UpdateMember(LibraryMember member)
    {
        db.Members.Update(member);
        db.SaveChanges();
    }

    public (bool success, string message) DeleteMember(int id)
    {
        var member = GetMember(id);
        if (member == null) return (false, "A tag nem található.");

        int activeLoansCount = db.Loans.Count(l => l.MemberId == id && l.ReturnDate == null);
        if (activeLoansCount > 0)
            return (false, $"A tag nem törölhető, mert {activeLoansCount} aktív kölcsönzése van.");

        member.IsDeleted = true;
        db.SaveChanges();
        return (true, "Tag sikeresen törölve.");
    }

    public List<LibraryMember> SearchMembers(string field, string text)
    {
        return [.. db.Members
            .Where(m => !m.IsDeleted)
            .AsEnumerable()
            .Where(m => field switch
            {
                "Address" => m.Address.Contains(text, StringComparison.OrdinalIgnoreCase),
                "Contact" => m.Contact.Contains(text, StringComparison.OrdinalIgnoreCase),
                "Id"      => m.Id.ToString().Contains(text),
                _         => m.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            })];
    }

    // ── Loans ──

    public List<Loan> GetLoans()
        => [.. db.Loans
              .Include(l => l.Book)
              .Include(l => l.Member)
              .OrderByDescending(l => l.LoanDate)];

    public Loan? GetLoan(int id)
        => db.Loans.FirstOrDefault(l => l.Id == id);

    public (bool success, string message, Loan? loan) CreateLoan(int bookId, int memberId, DateTime loanDate)
    {
        var book = GetBook(bookId);
        if (book == null) return (false, "A könyv nem található.", null);
        if (!book.IsLoanable) return (false, "Ez a könyv nem kölcsönözhető.", null);
        if (GetAvailableCopies(bookId) <= 0)
            return (false, "Nincs szabad példány.", null);

        var member = GetMember(memberId);
        if (member == null) return (false, "A tag nem található.", null);

        int currentLoans = db.Loans.Count(l => l.MemberId == memberId && l.ReturnDate == null);
        if (member.MaxBooks != int.MaxValue && currentLoans >= member.MaxBooks)
            return (false, $"A tag elérte a kölcsönzési limitet ({member.MaxBooks} könyv).", null);
        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(member.LoanDays)
        };

        db.Loans.Add(loan);
        db.SaveChanges();
        return (true, "Sikeres kölcsönzés.", loan);
    }

    public (bool success, string message, bool overdue, int overdueDays) ReturnBook(int loanId)
    {
        var loan = GetLoan(loanId);
        if (loan == null) return (false, "A kölcsönzés nem található.", false, 0);
        if (loan.ReturnDate.HasValue) return (false, "Ez a könyv már vissza lett hozva.", false, 0);

        bool overdue = DateTime.Today > loan.DueDate;
        int days = overdue ? (DateTime.Today - loan.DueDate).Days : 0;

        loan.ReturnDate = DateTime.Today;
        if (overdue)
        {
            loan.FineAmount = days * 50; // 50 HUF library fine per day
        }

        db.SaveChanges();

        string msg = overdue
            ? $"Könyv visszavéve. Késes: {days} nap. Büntetés: {loan.FineAmount} Ft!"
            : "Könyv sikeresen visszavéve.";

        return (true, msg, overdue, days);
    }

    public List<Loan> GetMemberLoans(int memberId, bool? returned = null)
        => [.. db.Loans
              .Include(l => l.Book)
              .Where(l => l.MemberId == memberId &&
                  (returned == null ||
                   (returned == true  ? l.ReturnDate != null : l.ReturnDate == null)))
              .OrderByDescending(l => l.LoanDate)];

    public (int count, decimal totalFine) ReturnAllForMember(int memberId)
    {
        var activeLoans = db.Loans.Where(l => l.MemberId == memberId && l.ReturnDate == null).ToList();
        decimal fine = 0;
        foreach (var loan in activeLoans)
        {
            var (_, _, _, _) = ReturnBook(loan.Id);
            fine += loan.FineAmount;
        }
        return (activeLoans.Count, fine);
    }

    public (int count, decimal totalFine) ReturnAllForBook(int bookId)
    {
        var activeLoans = db.Loans.Where(l => l.BookId == bookId && l.ReturnDate == null).ToList();
        decimal fine = 0;
        foreach (var loan in activeLoans)
        {
            var (_, _, _, _) = ReturnBook(loan.Id);
            fine += loan.FineAmount;
        }
        return (activeLoans.Count, fine);
    }

    // ── Dashboard ──

    public DashboardViewModel GetDashboard()
    {
        List<Loan> activeLoans = [.. db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)];

        List<Loan> recent = [.. db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .Take(5)];

        var topBooks = db.Books
            .Where(b => !b.IsDeleted && b.Loans.Count > 0)
            .OrderByDescending(b => b.Loans.Count)
            .Take(5)
            .ToList();

        return new DashboardViewModel
        {
            TotalBooks = db.Books.Count(b => !b.IsDeleted),
            TotalMembers = db.Members.Count(m => !m.IsDeleted),
            ActiveLoans = activeLoans.Count,
            OverdueLoans = activeLoans.Count(l => l.IsOverdue),
            TopBooks = topBooks,
            RecentLoans = [.. recent.Select(l => new LoanViewModel
            {
                Loan   = l,
                Book   = l.Book,
                Member = l.Member
            })]
        };
    }
}