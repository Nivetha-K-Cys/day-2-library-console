
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Payments;

namespace LibraryManagementSystem.Services;

public class LibraryService
{
    private readonly Book[] books =
    {
        new Book(1, "C Programming", "Dennis Ritchie", 3),
        new Book(2, "Java Programming", "James Gosling", 2),
        new Book(3, "Python Basics", "Mark Lutz", 4),
        new Book(4, "Clean Code", "Robert Martin", 1),
        new Book(5, "Database Systems", "Ramez Elmasri", 2)
    };

    private readonly List<Member> members = new();
    private readonly List<Loan> loans = new();
    private int nextLoanId = 1;

    private static bool ReadPositiveId(string input, out int id)
    {
        return int.TryParse(input, out id) && id > 0;
    }

    public void DisplayBooks()
    {
        Console.WriteLine("\n--- BOOK LIST ---");
        Console.WriteLine("ID   Title                    Author                 Copies");
        Console.WriteLine(new string('-', 65));

        Book[] sorted = books.OrderBy(b => b.Title).ToArray();

        foreach (Book book in sorted)
        {
            book.Display();
        }
    }

    public void SearchBooks()
    {
        Console.Write("Enter title or author: ");
        string keyword = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Invalid search keyword.");
            return;
        }

        bool found = false;

        for (int i = 0; i < books.Length; i++)
        {
            Book book = books[i];

            if (book.Title.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase))
            {
                book.Display();
                found = true;
                continue;
            }
        }

        if (!found)
            Console.WriteLine("Book not found.");
    }

    public void RegisterMember()
    {
        Console.Write("Enter Member ID: ");

        if (!ReadPositiveId(Console.ReadLine() ?? "", out int id))
        {
            Console.WriteLine("Invalid Member ID.");
            return;
        }

        if (members.Any(m => m.Id == id))
        {
            Console.WriteLine("Member ID already exists.");
            return;
        }

        Console.Write("Enter Member Name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name) ||
            !name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            Console.WriteLine("Invalid member name.");
            return;
        }

        Console.WriteLine("1. Standard");
        Console.WriteLine("2. Premium");
        Console.Write("Select Membership: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) ||
            (choice != 1 && choice != 2))
        {
            Console.WriteLine("Invalid membership.");
            return;
        }

        MembershipType type = (MembershipType)choice;

        Member member = new Member(id, name, type);
        members.Add(member);

        Console.WriteLine("\nMember registered successfully!");
        Console.WriteLine($"ID: {member.Id}");
        Console.WriteLine($"Name: {member.Name}");
        Console.WriteLine($"Membership: {member.Membership}");
        Console.WriteLine($"Borrowing Limit: {member.BorrowingLimit}");
    }

    public void IssueBook()
    {
        DisplayBooks();

        Console.Write("\nEnter Book ID: ");

        if (!ReadPositiveId(Console.ReadLine() ?? "", out int bookId))
        {
            Console.WriteLine("Invalid Book ID.");
            return;
        }

        Book? book = books.FirstOrDefault(b => b.Id == bookId);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        if (book.AvailableCopies <= 0)
        {
            Console.WriteLine("No copies available.");
            return;
        }

        Console.Write("Enter Member ID: ");

        if (!ReadPositiveId(Console.ReadLine() ?? "", out int memberId))
        {
            Console.WriteLine("Invalid Member ID.");
            return;
        }

        Member? member = members.FirstOrDefault(m => m.Id == memberId);

        if (member == null)
        {
            Console.WriteLine("Member not found. Register first.");
            return;
        }

        if (!member.CanBorrow())
        {
            Console.WriteLine("Borrowing limit reached.");
            return;
        }

        if (!book.IssueCopy())
        {
            Console.WriteLine("No copies available.");
            return;
        }

        Loan loan = member.Membership switch
        {
            MembershipType.Premium =>
                new PremiumLoan(nextLoanId++, book, member),

            _ => new StandardLoan(nextLoanId++, book, member)
        };

        loans.Add(loan);
        member.IncreaseLoans();

        LoanPeriod period = new LoanPeriod(
            loan.IssueDate, loan.DueDate);

        Console.WriteLine("\nBook issued successfully!");
        Console.WriteLine($"Loan ID: {loan.LoanId}");
        Console.WriteLine($"Book: {book.Title}");
        Console.WriteLine($"Member: {member.Name}");
        Console.WriteLine($"Issue Date: {loan.IssueDate:dd-MM-yyyy}");
        Console.WriteLine($"Due Date: {loan.DueDate:dd-MM-yyyy}");
        Console.WriteLine($"Loan Period: {period.TotalDays} days");
        Console.WriteLine($"Available Copies: {book.AvailableCopies}");
    }

    public void ReturnBook()
    {
        Console.Write("Enter Loan ID: ");

        if (!ReadPositiveId(Console.ReadLine() ?? "", out int loanId))
        {
            Console.WriteLine("Invalid Loan ID.");
            return;
        }

        Loan? loan = loans.FirstOrDefault(l => l.LoanId == loanId);

        if (loan == null)
        {
            Console.WriteLine("Loan not found.");
            return;
        }

        if (loan.IsReturned)
        {
            Console.WriteLine("Book already returned.");
            return;
        }

        Console.Write("Enter overdue days (0 for on-time): ");

        if (!int.TryParse(Console.ReadLine(), out int days) ||
            days < 0)
        {
            Console.WriteLine("Invalid overdue days.");
            return;
        }

        loan.ReturnBook(days);
        loan.Book.ReturnCopy();
        loan.Member.DecreaseLoans();

        FineSummary summary = new FineSummary(days, loan.Fine);
        DisplayFine(in summary);

        (int Days, decimal Amount) result = (days, loan.Fine);

        Console.WriteLine("\nBook returned successfully!");
        Console.WriteLine($"Book: {loan.Book.Title}");
        Console.WriteLine($"Overdue Days: {result.Days}");
        Console.WriteLine($"Fine: Rs.{result.Amount}");
    }

    private static void DisplayFine(in FineSummary summary)
    {
        Console.WriteLine("\n--- FINE SUMMARY ---");
        Console.WriteLine($"Overdue Days: {summary.OverdueDays}");
        Console.WriteLine($"Amount: Rs.{summary.Amount}");
    }

    public void DisplayLoans()
    {
        if (loans.Count == 0)
        {
            Console.WriteLine("No loans found.");
            return;
        }

        Console.WriteLine("\n--- LOAN DETAILS ---");

        foreach (Loan loan in loans)
        {
            loan.Display();
        }
    }

    public void PayFine()
    {
        Console.Write("Enter Loan ID: ");

        if (!ReadPositiveId(Console.ReadLine() ?? "", out int id))
        {
            Console.WriteLine("Invalid Loan ID.");
            return;
        }

        Loan? loan = loans.FirstOrDefault(l => l.LoanId == id);

        if (loan == null)
        {
            Console.WriteLine("Loan not found.");
            return;
        }

        if (!loan.IsReturned)
        {
            Console.WriteLine("Return the book first.");
            return;
        }

        if (loan.FinePaid)
        {
            Console.WriteLine("Fine already paid.");
            return;
        }

        if (loan.Fine <= 0)
        {
            Console.WriteLine("No fine to pay.");
            return;
        }

        Console.WriteLine($"Fine Amount: Rs.{loan.Fine}");
        Console.WriteLine("1. Cash");
        Console.WriteLine("2. Digital");
        Console.Write("Select payment method: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid payment method.");
            return;
        }

        IPaymentProcessor processor;

        switch (choice)
        {
            case 1:
                processor = new CashPayment();
                break;

            case 2:
                processor = new DigitalPayment();
                break;

            default:
                Console.WriteLine("Payment failed: Invalid method.");
                return;
        }

        bool success = processor.ProcessPayment(loan.Fine);

        if (!success)
        {
            Console.WriteLine("Payment failed.");
            return;
        }

        loan.MarkFinePaid();

        Console.WriteLine("\n========== PAYMENT RECEIPT ==========");
        Console.WriteLine($"Receipt No: REC-{loan.LoanId:D4}");
        Console.WriteLine($"Member: {loan.Member.Name}");
        Console.WriteLine($"Book: {loan.Book.Title}");
        Console.WriteLine($"Amount: Rs.{loan.Fine}");
        Console.WriteLine($"Method: {(choice == 1 ? "Cash" : "Digital")}");
        Console.WriteLine($"Date: {DateTime.Now:dd-MM-yyyy}");
        Console.WriteLine("Status: PAID (SIMULATED)");
        Console.WriteLine("=====================================");
    }

    public void DemonstrateConcepts()
    {
        Console.WriteLine("\n--- C# CONCEPT DEMONSTRATION ---");

        // Primitive value type copying
        int a = 10;
        int b = a;
        b = 20;

        Console.WriteLine($"Value copy: a={a}, b={b}");

        // Class reference copying
        Book original = books[0];
        Book reference = original;

        Console.WriteLine(
            $"Same object: {ReferenceEquals(original, reference)}");

        // Record equality and with
        BookSummary r1 = new BookSummary(
            1, "C Programming", "Dennis Ritchie");

        BookSummary r2 = r1 with { };

        Console.WriteLine($"Record equality: {r1 == r2}");

        BookSummary r3 = r1 with { Title = "Advanced C" };

        Console.WriteLine($"New record title: {r3.Title}");

        // Method overloading
        Loan sample = new StandardLoan(
            999, books[0],
            new Member(999, "Demo", MembershipType.Standard));

        Console.WriteLine($"Fine for 3 days: Rs.{sample.CalculateFine(3)}");

        // break and continue
        foreach (Book book in books)
        {
            if (book.AvailableCopies == 0)
                continue;

            if (book.Id == 3)
            {
                Console.WriteLine("Book ID 3 found using break.");
                break;
            }
        }
    }
}
