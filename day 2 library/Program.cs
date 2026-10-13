
using LibraryManagementSystem.Services;

LibraryService library = new LibraryService();

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("======================================");
    Console.WriteLine("      LIBRARY MANAGEMENT SYSTEM");
    Console.WriteLine("======================================");
    Console.WriteLine("1. Display All Books");
    Console.WriteLine("2. Search Books");
    Console.WriteLine("3. Register Member");
    Console.WriteLine("4. Issue Book");
    Console.WriteLine("5. Return Book");
    Console.WriteLine("6. Display Loans");
    Console.WriteLine("7. Pay Fine");
    Console.WriteLine("8. Demonstrate C# Concepts");
    Console.WriteLine("9. Exit");
    Console.WriteLine("======================================");

    Console.Write("Enter your choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            library.DisplayBooks();
            break;

        case 2:
            library.SearchBooks();
            break;

        case 3:
            library.RegisterMember();
            break;

        case 4:
            library.IssueBook();
            break;

        case 5:
            library.ReturnBook();
            break;

        case 6:
            library.DisplayLoans();
            break;

        case 7:
            library.PayFine();
            break;

        case 8:
            library.DemonstrateConcepts();
            break;

        case 9:
            Console.WriteLine("Thank you!");
            running = false;
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
