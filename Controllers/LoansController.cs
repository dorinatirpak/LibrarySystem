using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize]
public class LoansController(DataService data) : Controller
{
    public IActionResult Create(int? bookId, int? memberId)
    {
        if (memberId.HasValue && memberId.Value > 0)
        {
            var member = data.GetMember(memberId.Value);
            if (member != null)
            {
                int currentLoans = data.GetMemberLoans(member.Id, returned: false).Count;
                if (member.MaxBooks != int.MaxValue && currentLoans >= member.MaxBooks)
                {
                    TempData["Error"] = $"A tag ({member.Name}) már elérte a maximális kölcsönzési limitet ({member.MaxBooks} könyv).";
                    return RedirectToAction("Loans", "Members", new { id = member.Id });
                }
            }
        }

        var vm = new CreateLoanViewModel
        {
            BookIds = bookId.HasValue ? [bookId.Value] : [],
            LoanDate = DateTime.Today,
            MemberId = memberId ?? 0
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(CreateLoanViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (vm.BookIds == null || vm.BookIds.Count == 0)
        {
            vm.ErrorMessage = "Legalább egy könyvet ki kell választani!";
            return View(vm);
        }

        int successCount = 0;
        string lastError = "";

        foreach (var id in vm.BookIds)
        {
            var (success, message, _) = data.CreateLoan(id, vm.MemberId, vm.LoanDate);
            if (success) successCount++;
            else lastError = message;
        }

        if (successCount > 0)
        {
            TempData["Success"] = $"{successCount} könyv sikeresen kikölcsönözve!";
            return RedirectToAction("Loans", "Members", new { id = vm.MemberId });
        }

        vm.ErrorMessage = lastError;
        return View(vm);
    }

    [HttpPost]
    public IActionResult Return(int id)
    {
        var loan = data.GetLoan(id);
        var (success, message, overdue, _) = data.ReturnBook(id);

        if (success)
        {
            if (overdue)
                TempData["Warning"] = message;
            else
                TempData["Success"] = message;
        }
        else
        {
            TempData["Error"] = message;
        }

        int memberId = loan?.MemberId ?? 0;
        return RedirectToAction("Loans", "Members", new { id = memberId });
    }

    [HttpPost]
    public IActionResult ReturnAllForMember(int memberId)
    {
        var (count, totalFine) = data.ReturnAllForMember(memberId);
        if (count > 0)
        {
            string msg = $"{count} db könyv sikeresen visszavéve.";
            if (totalFine > 0) msg += $" Összesített büntetés: {totalFine} Ft.";
            TempData["Success"] = msg;
        }
        else
        {
            TempData["Info"] = "Nincsenek aktív kölcsönzések ennél a tagnál.";
        }
        return RedirectToAction("Loans", "Members", new { id = memberId });
    }

    [HttpPost]
    public IActionResult ReturnAllForBook(int bookId)
    {
        var (count, totalFine) = data.ReturnAllForBook(bookId);
        if (count > 0)
        {
            string msg = $"{count} db példány sikeresen visszavéve.";
            if (totalFine > 0) msg += $" Összesített büntetés: {totalFine} Ft.";
            TempData["Success"] = msg;
        }
        else
        {
            TempData["Info"] = "Nincsenek kint lévő példányok ebből a könyvből.";
        }
        return RedirectToAction("Index", "Books");
    }

    public IActionResult All()
    {
        List<LoanViewModel> loans = [.. data.GetLoans()
            .OrderByDescending(l => l.LoanDate)
            .Select(l => new LoanViewModel
            {
                Loan   = l,
                Book   = data.GetBook(l.BookId),
                Member = data.GetMember(l.MemberId)
            })];

        return View(loans);
    }

    [HttpGet]
    public IActionResult GetMemberDetails(int id)
    {
        var member = data.GetMember(id);
        if (member == null) return NotFound();

        return Json(new
        {
            member.Id,
            member.Name,
            member.MemberTypeDisplay,
            member.Contact,
            member.Address,
            member.LoanDays,
            MaxBooks = member.MaxBooks,
            ActiveLoanCount = data.GetMemberLoans(member.Id, returned: false).Count
        });
    }

    [HttpGet]
    public IActionResult GetBookDetails(int id)
    {
        var book = data.GetBook(id);
        if (book == null) return NotFound();

        return Json(new
        {
            book.Id,
            book.Title,
            book.Author,
            book.ISBN,
            book.Year,
            Available = data.GetAvailableCopies(book.Id)
        });
    }

    [HttpGet]
    public IActionResult SearchBooks(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return Json(new List<object>());

        var byTitle = data.SearchBooks("Title", term);
        var byAuthor = data.SearchBooks("Author", term);
        var byISBN = data.SearchBooks("ISBN", term);

        var books = byTitle.Concat(byAuthor).Concat(byISBN)
            .Where(b => b.IsLoanable && data.GetAvailableCopies(b.Id) > 0)
            .DistinctBy(b => b.Id)
            .Take(50)
            .Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.ISBN,
                b.Year,
                Available = data.GetAvailableCopies(b.Id)
            });
        return Json(books);
    }

    [HttpGet]
    public IActionResult SearchMembers(string term)
    {
        var members = data.SearchMembers("Name", term ?? "")
            .Concat(data.SearchMembers("Address", term ?? ""))
            .DistinctBy(m => m.Id)
            .Take(15)
            .Select(m => new
            {
                m.Id,
                m.Name,
                m.MemberTypeDisplay,
                m.Contact,
                m.Address,
                m.LoanDays,
                MaxBooks = m.MaxBooks,
                ActiveLoanCount = data.GetMemberLoans(m.Id, returned: false).Count
            });
        return Json(members);
    }
}