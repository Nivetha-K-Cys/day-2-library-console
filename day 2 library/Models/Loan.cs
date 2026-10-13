
namespace LibraryManagementSystem.Models;

public abstract class Loan
{
    public int LoanId { get; }
    public Book Book { get; }
    public Member Member { get; }
    public DateTime IssueDate { get; }
    public DateTime DueDate { get; }
    public DateTime? ReturnDate { get; private set; }
    public decimal Fine { get; private set; }
    public bool FinePaid { get; private set; }

    public bool IsReturned => ReturnDate.HasValue;

    protected Loan(int id, Book book, Member member, int days)
    {
        LoanId = id;
        Book = book;
        Member = member;
        IssueDate = DateTime.Today;
        DueDate = IssueDate.AddDays(days);
    }

    public virtual decimal CalculateFine(int overdueDays)
    {
        return Math.Max(0, overdueDays) * 5m;
    }

    // Method overloading
    public decimal CalculateFine(DateTime returnDate)
    {
        int days = Math.Max(0,
            (returnDate.Date - DueDate.Date).Days);

        return CalculateFine(days);
    }

    public void ReturnBook(int overdueDays)
    {
        if (IsReturned)
            throw new InvalidOperationException("Already returned.");

        if (overdueDays < 0)
            throw new ArgumentException("Invalid overdue days.");

        Fine = CalculateFine(overdueDays);

        // Simulated return date for testing
        ReturnDate = DueDate.AddDays(overdueDays);
    }

    public void MarkFinePaid()
    {
        if (Fine > 0)
            FinePaid = true;
    }

    public virtual void Display()
    {
        string status = IsReturned ? "Returned" : "Issued";

        Console.WriteLine(
            $"Loan ID: {LoanId} | Book: {Book.Title} | " +
            $"Member: {Member.Name} | Status: {status} | Fine: Rs.{Fine}");
    }
}
