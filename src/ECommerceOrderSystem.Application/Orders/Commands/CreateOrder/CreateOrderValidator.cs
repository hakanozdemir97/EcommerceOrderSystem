using FluentValidation;
using ECommerceOrderSystem.Domain.Enums;

namespace ECommerceOrderSystem.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .Must(BeValidPaymentMethod)
                .WithMessage("Payment method must be either 'CreditCard' or 'BankTransfer'");
        }

        private static bool BeValidPaymentMethod(string paymentMethod)
        {
            return Enum.TryParse<PaymentMethod>(paymentMethod, true, out _);
        }
    }
}