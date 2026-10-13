
namespace LibraryManagementSystem.Models;

public class StandardLoan : Loan
{
    public StandardLoan(int id, Book book, Member member)
        : base(id, book, member, 14)
    {
    }

    public override decimal CalculateFine(int overdueDays)
    {
        return Math.Max(0, overdueDays) * 5m;
    }
}
