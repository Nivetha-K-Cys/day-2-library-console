
namespace LibraryManagementSystem.Models;

public class PremiumLoan : Loan
{
    public PremiumLoan(int id, Book book, Member member)
        : base(id, book, member, 21)
    {
    }

    public override decimal CalculateFine(int overdueDays)
    {
        return Math.Max(0, overdueDays) * 2m;
    }
}
