
namespace LibraryManagementSystem.Payments;

public class DigitalPayment : IPaymentProcessor
{
    bool IPaymentProcessor.ProcessPayment(decimal amount)
    {
        if (amount <= 0)
            return false;

        Console.WriteLine("Digital payment successful.");
        return true;
    }
}
