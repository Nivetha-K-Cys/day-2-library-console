
namespace LibraryManagementSystem.Models;

public class Book
{
    public int Id { get; }
    public string Title { get; }
    public string Author { get; }
    public int TotalCopies { get; }
    public int AvailableCopies { get; private set; }

    public Book(int id, string title, string author, int copies)
    {
        Id = id;
        Title = title;
        Author = author;
        TotalCopies = copies;
        AvailableCopies = copies;
    }

    public bool IssueCopy()
    {
        if (AvailableCopies <= 0)
            return false;

        AvailableCopies--;
        return true;
    }

    public void ReturnCopy()
    {
        if (AvailableCopies < TotalCopies)
            AvailableCopies++;
    }

    public void Display()
    {
        Console.WriteLine(
            $"{Id,-5}{Title,-25}{Author,-22}{AvailableCopies}/{TotalCopies}");
    }
}
