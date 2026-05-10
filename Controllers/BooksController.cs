using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize]
public class BooksController(DataService data) : Controller
{
    public IActionResult Index()
    {
        var books = data.GetBooks();
        var vm = new BookSearchViewModel { Results = books };
        foreach (var b in books)
            vm.AvailableCopies[b.Id] = data.GetAvailableCopies(b.Id);
        return View(vm);
    }

    [HttpPost]
    public IActionResult Search(BookSearchViewModel vm)
    {
        vm.Results = data.SearchBooks(vm.SearchField, vm.SearchText);
        foreach (var b in vm.Results)
            vm.AvailableCopies[b.Id] = data.GetAvailableCopies(b.Id);
        return View("Index", vm);
    }

    [HttpGet]
    public IActionResult SearchDynamic(string field, string text)
    {
        var vm = new BookSearchViewModel
        {
            SearchField = field,
            SearchText = text ?? "",
            Results = data.SearchBooks(field, text ?? "")
        };
        foreach (var b in vm.Results)
            vm.AvailableCopies[b.Id] = data.GetAvailableCopies(b.Id);
        return PartialView("_BookTablePartial", vm);
    }

    public IActionResult Create() => View(new Book());

    [HttpPost]
    public IActionResult Create(Book book)
    {
        if (!ModelState.IsValid) return View(book);
        data.AddBook(book);
        TempData["Success"] = "Könyv sikeresen felvéve!";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var book = data.GetBook(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(Book book)
    {
        if (!ModelState.IsValid) return View(book);
        data.UpdateBook(book);
        TempData["Success"] = "Könyv sikeresen módosítva!";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var book = data.GetBook(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id, bool deleteAll)
    {
        var (success, message) = data.DeleteBookCopy(id, deleteAll);
        if (success)
            TempData["Success"] = message;
        else
            TempData["Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var book = data.GetBook(id);
        if (book == null) return NotFound();
        ViewBag.Available = data.GetAvailableCopies(id);
        return View(book);
    }
}
