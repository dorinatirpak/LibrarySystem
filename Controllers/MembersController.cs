using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize]
public class MembersController(DataService data) : Controller
{
    public IActionResult Index()
    {
        var vm = new MemberSearchViewModel { Results = data.GetMembers() };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Search(MemberSearchViewModel vm)
    {
        vm.Results = data.SearchMembers(vm.SearchField, vm.SearchText);
        return View("Index", vm);
    }

    [HttpGet]
    public IActionResult SearchDynamic(string field, string text)
    {
        var vm = new MemberSearchViewModel
        {
            SearchField = field,
            SearchText = text ?? "",
            Results = data.SearchMembers(field, text ?? "")
        };
        return PartialView("_MemberTablePartial", vm);
    }

    public IActionResult Create() => View(new CreateMemberViewModel());

    [HttpPost]
    public IActionResult Create(CreateMemberViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        LibraryMember member = vm.MemberType switch
        {
            MemberType.Student => new StudentMember(),
            MemberType.Professor => new ProfessorMember(),
            MemberType.ExternalUniversity => new ExternalMember(),
            _ => new OtherMember()
        };
        member.Name = vm.Name;
        member.Address = vm.Address;
        member.Contact = vm.Contact;

        data.AddMember(member);
        TempData["Success"] = "Tag sikeresen felvéve!";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var member = data.GetMember(id);
        if (member == null) return NotFound();
        
        var vm = new EditMemberViewModel
        {
            Id = member.Id,
            Name = member.Name,
            Address = member.Address,
            Contact = member.Contact,
            MemberTypeDisplay = member.MemberTypeDisplay
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(EditMemberViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        
        var existing = data.GetMember(vm.Id);
        if (existing == null) return NotFound();
        
        existing.Name = vm.Name;
        existing.Address = vm.Address;
        existing.Contact = vm.Contact;
        
        data.UpdateMember(existing);
        TempData["Success"] = "Tag sikeresen módosítva!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var (success, message) = data.DeleteMember(id);
        if (success)
            TempData["Success"] = message;
        else
            TempData["Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Loans(int id)
    {
        var member = data.GetMember(id);
        if (member == null) return NotFound();

        var active = data.GetMemberLoans(id, returned: false);
        var returned = data.GetMemberLoans(id, returned: true);

        var vm = new MemberLoansViewModel
        {
            Member = member,
            ActiveLoans = [.. active.Select(l => new LoanViewModel
            {
                Loan = l, Book = data.GetBook(l.BookId), Member = member
            })],
            ReturnedLoans = [.. returned.Select(l => new LoanViewModel
            {
                Loan = l, Book = data.GetBook(l.BookId), Member = member
            })]
        };
        return View(vm);
    }
}