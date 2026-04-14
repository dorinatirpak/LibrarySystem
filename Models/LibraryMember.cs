using System.Collections.Generic;
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
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
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