
namespace LibraryManagementSystem.Models;

public enum MembershipType
{
    Standard = 1,
    Premium = 2
}

public class Member
{
    public int Id { get; }
    public string Name { get; }
    public MembershipType Membership { get; }
    public int BorrowingLimit { get; }
    public int ActiveLoans { get; private set; }

    public Member(int id, string name, MembershipType membership)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid member ID.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid member name.");

        Id = id;
        Name = name.Trim();
        Membership = membership;

        BorrowingLimit = membership switch
        {
            MembershipType.Standard => 2,
            MembershipType.Premium => 5,
            _ => throw new ArgumentException("Invalid membership type.")
        };
    }

    public bool CanBorrow()
    {
        return ActiveLoans < BorrowingLimit;
    }

    private static void UpdateCount(ref int count, int change)
    {
        count = Math.Max(0, count + change);
    }

    public void IncreaseLoans()
    {
        int count = ActiveLoans;
        UpdateCount(ref count, 1);
        ActiveLoans = count;
    }

    public void DecreaseLoans()
    {
        int count = ActiveLoans;
        UpdateCount(ref count, -1);
        ActiveLoans = count;
    }
}
