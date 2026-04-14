using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class LoansController(DataService data) : Controller
{
    public IActionResult Create(int? bookId, int? memberId)
    {
        List<Book> books = [.. data.GetBooks()
            .Where(b => b.IsLoanable && data.GetAvailableCopies(b.Id) > 0)];

        var vm = new CreateLoanViewModel
        {
            AvailableBooks = books,
            Members = data.GetMembers(),
            LoanDate = DateTime.Today,
            BookId = bookId ?? 0,
            MemberId = memberId ?? 0
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(CreateLoanViewModel vm)
    {
        var (success, message, loan) = data.CreateLoan(vm.BookId, vm.MemberId, vm.LoanDate);
        if (success)
        {
            TempData["Success"] = message;
            return RedirectToAction("Loans", "Members", new { id = vm.MemberId });
        }

        vm.ErrorMessage = message;
        vm.AvailableBooks = [.. data.GetBooks().Where(b => b.IsLoanable && data.GetAvailableCopies(b.Id) > 0)];
        vm.Members = data.GetMembers();
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
}