using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize]
public class ReportsController(DataService data) : Controller
{
    public IActionResult Index()
    {
        var allLoans = data.GetLoans();
        var allMembers = data.GetMembers();
        var allBooks = data.GetBooks(includeDeleted: true);

        // Top 10 members by total loans (only those with at least one loan)
        var topMembers = allMembers
            .Select(m => new
            {
                Member = m,
                LoanCount = allLoans.Count(l => l.MemberId == m.Id),
                TotalFines = allLoans.Where(l => l.MemberId == m.Id).Sum(l => l.FineAmount)
            })
            .Where(x => x.LoanCount > 0)
            .OrderByDescending(x => x.LoanCount)
            .Take(10)
            .ToList();

        // Top 10 most borrowed books (only those with at least one loan)
        var topBooks = allBooks
            .Select(b => new
            {
                Book = b,
                LoanCount = allLoans.Count(l => l.BookId == b.Id)
            })
            .Where(x => x.LoanCount > 0)
            .OrderByDescending(x => x.LoanCount)
            .Take(10)
            .ToList();

        // Overdue loans
        var overdueLoans = allLoans.Where(l => l.IsOverdue).OrderByDescending(l => l.OverdueDays).ToList();

        ViewBag.TopMembers = topMembers;
        ViewBag.TopBooks = topBooks;
        ViewBag.OverdueLoans = overdueLoans;

        return View();
    }
}