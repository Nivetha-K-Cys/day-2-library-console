
namespace LibraryManagementSystem.Models;

public readonly struct LoanPeriod
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public LoanPeriod(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public int TotalDays => (End - Start).Days;
}

public readonly struct FineSummary
{
    public int OverdueDays { get; }
    public decimal Amount { get; }

    public FineSummary(int days, decimal amount)
    {
        OverdueDays = days;
        Amount = amount;
    }
}
