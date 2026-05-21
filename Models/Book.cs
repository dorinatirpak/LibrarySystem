using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models;

public class Book
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "A szerző megadása kötelező!")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "A szerző neve legalább 2 és legfeljebb 100 karakter!")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "A cím megadása kötelező!")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "A cím legalább 1 és legfeljebb 200 karakter!")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "A kiadó megadása kötelező!")]
    public string Publisher { get; set; } = "";

    [Required(ErrorMessage = "A kiadási év megadása kötelező!")]
    [Range(1800, 2026, ErrorMessage = "A kiadási évnek 1800 és 2026 között kell lennie!")]
    public int Year { get; set; }

    public string Edition { get; set; } = "";

    [Required(ErrorMessage = "Az ISBN megadása kötelező!")]
    [RegularExpression(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$", ErrorMessage = "Érvénytelen ISBN formátum (csak számok és kötőjel, 10 vagy 13 számjegy)!")]
    public string ISBN { get; set; } = "";

    public bool IsLoanable { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [Range(0, 1000, ErrorMessage = "A példányszám nem lehet negatív!")]
    public int CopyCount { get; set; } = 1;

    public ICollection<Loan> Loans { get; set; } = [];
}
