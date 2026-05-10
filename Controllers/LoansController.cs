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
                MaxBooks = m.MaxBooks == int.MaxValue ? "Korlátlan" : m.MaxBooks.ToString()
            });
        return Json(members);
    }
}