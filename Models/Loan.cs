namespace LibrarySystem.Models;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue  => !IsReturned && DateTime.Today > DueDate;
    public int  OverdueDays => IsOverdue ? (DateTime.Today - DueDate).Days : 0;

    // EF navigation
    public Book? Book { get; set; }
    public LibraryMember? Member { get; set; }
}
