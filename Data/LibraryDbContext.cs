using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<LibraryMember> Members { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Librarian> Librarians { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryMember>()
            .HasDiscriminator<MemberType>("MemberType") 
            .HasValue<StudentMember>(MemberType.Student) 
            .HasValue<ProfessorMember>(MemberType.Professor)
            .HasValue<ExternalMember>(MemberType.ExternalUniversity)
            .HasValue<OtherMember>(MemberType.Other);

        modelBuilder.Entity<LibraryMember>()
            .Ignore(m => m.MaxBooks)
            .Ignore(m => m.LoanDays)
            .Ignore(m => m.MemberTypeDisplay);

        modelBuilder.Entity<Loan>()
            .Ignore(l => l.IsReturned)
            .Ignore(l => l.IsOverdue)
            .Ignore(l => l.OverdueDays);

        // ── Kapcsolatok ──
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Oszlop-korlátok ──
        modelBuilder.Entity<Book>(b =>
        {
            b.Property(x => x.Author).HasMaxLength(200).IsRequired();
            b.Property(x => x.Title).HasMaxLength(300).IsRequired();
            b.Property(x => x.Publisher).HasMaxLength(200).IsRequired();
            b.Property(x => x.Edition).HasMaxLength(50).IsRequired();
            b.Property(x => x.ISBN).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<LibraryMember>(m =>
        {
            m.Property(x => x.Name).HasMaxLength(200).IsRequired();
            m.Property(x => x.Address).HasMaxLength(300).IsRequired();
            m.Property(x => x.Contact).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Librarian>(l =>
        {
            l.Property(x => x.Username).HasMaxLength(100).IsRequired();
            l.Property(x => x.PasswordHash).IsRequired();
            l.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            l.HasIndex(x => x.Username).IsUnique();
        });

        // ── Seed adatok ──
        // Jelszó: admin123  →  SHA-256 hex
        modelBuilder.Entity<Librarian>().HasData(new Librarian
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9",
            FullName = "Könyvtáros"
        });

        modelBuilder.Entity<StudentMember>().HasData(
            new StudentMember
            {
                Id = 1,
                Name = "Kovacs Anna",
                Address = "Budapest, Fo u. 1.",
                Contact = "kovacs.anna@edu.hu"
            });

        modelBuilder.Entity<ProfessorMember>().HasData(
            new ProfessorMember
            {
                Id = 2,
                Name = "Dr. Nagy Peter",
                Address = "Debrecen, Kossuth u. 5.",
                Contact = "nagy.peter@univ.hu"
            });

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Author = "Knuth, Donald E.",
                Title = "The Art of Computer Programming",
                Publisher = "Addison-Wesley",
                Year = 1968,
                Edition = "3rd",
                ISBN = "978-0201038040",
                IsLoanable = true,
                CopyCount = 3
            },
            new Book
            {
                Id = 2,
                Author = "Martin, Robert C.",
                Title = "Clean Code",
                Publisher = "Prentice Hall",
                Year = 2008,
                Edition = "1st",
                ISBN = "978-0132350884",
                IsLoanable = true,
                CopyCount = 2
            },
            new Book
            {
                Id = 3,
                Author = "Gamma et al.",
                Title = "Design Patterns",
                Publisher = "Addison-Wesley",
                Year = 1994,
                Edition = "1st",
                ISBN = "978-0201633610",
                IsLoanable = true,
                CopyCount = 1
            }
        );
    }
}