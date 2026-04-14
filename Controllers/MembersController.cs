using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services;

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

    public IActionResult Create() => View(new CreateMemberViewModel());

    [HttpPost]
    public IActionResult Create(CreateMemberViewModel vm)
    {
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
        return View(member);
    }

    [HttpPost]
    public IActionResult Edit(int id, string name, string address, string contact)
    {
        var member = data.GetMember(id);
        if (member == null) return NotFound();
        member.Name = name;
        member.Address = address;
        member.Contact = contact;
        data.UpdateMember(member);
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