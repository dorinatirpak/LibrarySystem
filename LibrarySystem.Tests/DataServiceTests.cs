using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Tests;

public class DataServiceTests
{
    private static LibraryDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public void CreateLoan_CalculatesCorrectDueDate_ForStudent()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        var book = new Book { Title = "Test Book", ISBN = "123", CopyCount = 1 };
        db.Books.Add(book);

        var member = new StudentMember { Name = "John Doe" }; // LoanDays = 60
        db.Members.Add(member);
        db.SaveChanges();

        var loanDate = new DateTime(2026, 5, 1);

        // Act
        var result = service.CreateLoan(book.Id, member.Id, loanDate);

        // Assert
        Assert.True(result.success);
        Assert.NotNull(result.loan);
        Assert.Equal(loanDate.AddDays(60), result.loan.DueDate);
    }

    [Fact]
    public void CreateLoan_Fails_WhenMemberLimitReached()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        var member = new OtherMember { Name = "Limited Member" }; // MaxBooks = 2
        db.Members.Add(member);

        var book1 = new Book { Title = "B1", ISBN = "ISBN1", CopyCount = 1 };
        var book2 = new Book { Title = "B2", ISBN = "ISBN2", CopyCount = 1 };
        var book3 = new Book { Title = "B3", ISBN = "ISBN3", CopyCount = 1 };
        db.Books.AddRange(book1, book2, book3);
        db.SaveChanges();

        service.CreateLoan(book1.Id, member.Id, DateTime.Today);
        service.CreateLoan(book2.Id, member.Id, DateTime.Today);

        // Act
        var result = service.CreateLoan(book3.Id, member.Id, DateTime.Today);

        // Assert
        Assert.False(result.success);
        Assert.Contains("limit", result.message);
    }

    [Fact]
    public void ReturnBook_CalculatesFine_WhenOverdue()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        var book = new Book { Title = "B1", ISBN = "I1", CopyCount = 1 };
        var member = new OtherMember { Name = "M1" };
        db.Books.Add(book);
        db.Members.Add(member);
        db.SaveChanges();

        // Create a loan that is already overdue
        var loanDate = DateTime.Today.AddDays(-20);
        var res = service.CreateLoan(book.Id, member.Id, loanDate);
        var loan = res.loan!;

        // OtherMember has 14 days. If we loaned 20 days ago, it's 6 days overdue.
        // Act
        var returnRes = service.ReturnBook(loan.Id);

        // Assert
        Assert.True(returnRes.overdue);
        Assert.Equal(6, returnRes.overdueDays);
        Assert.Equal(300, loan.FineAmount); // 6 * 50 = 300
    }

    [Fact]
    public void AddBook_IncreasesCopyCount_IfISBNExists()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        var book1 = new Book { Title = "T1", Author = "A1", ISBN = "999", CopyCount = 2 };
        service.AddBook(book1);

        var book2 = new Book { Title = "T1 Updated", Author = "A1", ISBN = "999", CopyCount = 3 };

        // Act
        var result = service.AddBook(book2);

        // Assert
        Assert.Equal(5, result.CopyCount);
        Assert.Equal(1, db.Books.Count());
    }

    [Fact]
    public void SearchBooks_FiltersCorrectly_ByTitle()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        db.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "1" },
            new Book { Title = "Design Patterns", Author = "GoF", ISBN = "2" }
        );
        db.SaveChanges();

        // Act
        var results = service.SearchBooks("Title", "Clean");

        // Assert
        Assert.Single(results);
        Assert.Equal("Clean Code", results[0].Title);
    }

    [Fact]
    public void SearchMembers_FiltersCorrectly_ByName()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        db.Members.AddRange(
            new StudentMember { Name = "Alice Smith", Address = "A1" },
            new StudentMember { Name = "Bob Jones", Address = "A2" }
        );
        db.SaveChanges();

        // Act
        var results = service.SearchMembers("Name", "Alice");

        // Assert
        Assert.Single(results);
        Assert.Equal("Alice Smith", results[0].Name);
    }

    [Fact]
    public void DeleteMember_Fails_WhenHasActiveLoans()
    {
        // Arrange
        using var db = GetDbContext();
        var service = new DataService(db);

        var book = new Book { Title = "B1", ISBN = "I1", CopyCount = 1 };
        var member = new StudentMember { Name = "M1" };
        db.Books.Add(book);
        db.Members.Add(member);
        db.SaveChanges();

        service.CreateLoan(book.Id, member.Id, DateTime.Today);

        // Act
        var result = service.DeleteMember(member.Id);

        // Assert
        Assert.False(result.success);
        Assert.Contains("aktív kölcsönzése", result.message);
    }
}
