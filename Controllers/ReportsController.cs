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

        var overdueLoans = allLoans.Where(l => l.IsOverdue).OrderByDescending(l => l.OverdueDays).ToList();

        ViewBag.TopMembers = topMembers;
        ViewBag.TopBooks = topBooks;
        ViewBag.OverdueLoans = overdueLoans;

        return View();
    }
}