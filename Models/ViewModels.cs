using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models;

public class LoginViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int FailedAttempts { get; set; }
    public string? ErrorMessage { get; set; }
}

public class BookSearchViewModel
{
    public string SearchField { get; set; } = "Title";
    public string SearchText { get; set; } = "";
    public List<Book> Results { get; set; } = [];
    public Dictionary<int, int> AvailableCopies { get; set; } = [];
}

public class MemberSearchViewModel
{
    public string SearchField { get; set; } = "Name";
    public string SearchText { get; set; } = "";

    public List<LibraryMember> Results { get; set; } = [];
}

public class LoanViewModel
{
    public Loan Loan { get; set; } = new();
    public Book? Book { get; set; }
    public LibraryMember? Member { get; set; }
}

public class MemberLoansViewModel
{
    public LibraryMember Member { get; set; } = null!;
    public List<LoanViewModel> ActiveLoans { get; set; } = [];
    public List<LoanViewModel> ReturnedLoans { get; set; } = [];
}

public class CreateLoanViewModel
{
    public List<int> BookIds { get; set; } = [];
    
    [Required(ErrorMessage = "A tag kiválasztása kötelező!")]
    [Range(1, int.MaxValue, ErrorMessage = "A tag kiválasztása kötelező!")]
    public int MemberId { get; set; }
    public DateTime LoanDate { get; set; } = DateTime.Today;
    public string? ErrorMessage { get; set; }
    public List<Book> AvailableBooks { get; set; } = [];
    public List<LibraryMember> Members { get; set; } = [];
}

public class CreateMemberViewModel
{
    [Required(ErrorMessage = "A név megadása kötelező!")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A névnek legalább 3 és legfeljebb 100 karakternek kell lennie!")]
    [RegularExpression(@"^[a-zA-ZáéíóöőúüűÁÉÍÓÖŐÚÜŰ\s\.]+$", ErrorMessage = "A név csak betűket, szóközt és pontot tartalmazhat!")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "A lakcím megadása kötelező!")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "A lakcímnek legalább 10 karakternek kell lennie!")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Az elérhetőség megadása kötelező!")]
    [EmailAddress(ErrorMessage = "Érvénytelen e-mail formátum!")]
    public string Contact { get; set; } = "";

    [Required(ErrorMessage = "A típus kiválasztása kötelező!")]
    public MemberType MemberType { get; set; } = MemberType.Student;
}

public class EditMemberViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A név megadása kötelező!")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A névnek legalább 3 és legfeljebb 100 karakternek kell lennie!")]
    [RegularExpression(@"^[a-zA-ZáéíóöőúüűÁÉÍÓÖŐÚÜŰ\s\.]+$", ErrorMessage = "A név csak betűket, szóközt és pontot tartalmazhat!")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "A lakcím megadása kötelező!")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "A lakcímnek legalább 10 karakternek kell lennie!")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Az elérhetőség megadása kötelező!")]
    [EmailAddress(ErrorMessage = "Érvénytelen e-mail formátum!")]
    public string Contact { get; set; } = "";
    
    public string? MemberTypeDisplay { get; set; }
}

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public List<Book> TopBooks { get; set; } = [];
    public List<LoanViewModel> RecentLoans { get; set; } = [];
}