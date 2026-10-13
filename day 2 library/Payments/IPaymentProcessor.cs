
namespace LibraryManagementSystem.Payments;

public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount);
}
