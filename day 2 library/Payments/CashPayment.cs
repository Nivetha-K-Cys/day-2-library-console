
namespace LibraryManagementSystem.Payments;

public class CashPayment : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        if (amount <= 0)
            return false;

        Console.WriteLine("Cash payment successful.");
        return true;
    }
}
