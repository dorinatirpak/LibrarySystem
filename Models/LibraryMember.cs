using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models;

public enum MemberType
{
    Student,
    Professor,
    ExternalUniversity,
    Other
}

public abstract class LibraryMember
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

    public bool IsDeleted { get; set; } = false;

    public MemberType MemberType { get; set; }

    [NotMapped]
    public abstract int MaxBooks { get; }
    [NotMapped]
    public abstract int LoanDays { get; }
    [NotMapped]
    public abstract string MemberTypeDisplay { get; }

    public ICollection<Loan> Loans { get; set; } = [];
}

public class StudentMember : LibraryMember
{
    public StudentMember()
    {
        MemberType = MemberType.Student;
    }

    public override int MaxBooks => 5;
    public override int LoanDays => 60;
    public override string MemberTypeDisplay => "Egyetemi hallgató";
}

public class ProfessorMember : LibraryMember
{
    public ProfessorMember()
    {
        MemberType = MemberType.Professor;
    }

    public override int MaxBooks => int.MaxValue;
    public override int LoanDays => 365;
    public override string MemberTypeDisplay => "Egyetemi oktató";
}

public class ExternalMember : LibraryMember
{
    public ExternalMember()
    {
        MemberType = MemberType.ExternalUniversity;
    }

    public override int MaxBooks => 4;
    public override int LoanDays => 30;
    public override string MemberTypeDisplay => "Más egyetem polgára/oktatója";
}

public class OtherMember : LibraryMember
{
    public OtherMember()
    {
        MemberType = MemberType.Other;
    }

    public override int MaxBooks => 2;
    public override int LoanDays => 14;
    public override string MemberTypeDisplay => "Egyéb";
}