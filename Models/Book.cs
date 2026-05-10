namespace LibrarySystem.Models;

public class Book
{
    public int Id { get; set; }
    public string Author { get; set; } = "";
    public string Title { get; set; } = "";
    public string Publisher { get; set; } = "";
    public int Year { get; set; }
    public string Edition { get; set; } = "";
    public string ISBN { get; set; } = "";
    public bool IsLoanable { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int CopyCount { get; set; } = 1;

    public ICollection<Loan> Loans { get; set; } = [];
}