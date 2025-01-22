using FluentValidation;
using LibraryManagementDomain.Models;

namespace LibraryManagementApplication.Validation
{
    public class AuthorValidator : AbstractValidator<AuthorModel>
    {
        public AuthorValidator()
        {
            // Rule for Name: Greater than 2 characters and less than 10 characters
            RuleFor(author => author.Name)
                .NotEmpty().WithMessage("Author name is required.")
                .MinimumLength(3).WithMessage("Author name must be greater than 2 characters.")
                .MaximumLength(9).WithMessage("Author name must be less than 10 characters.");

            // Rule for AuthorType: Between 1 and 4 inclusive
            RuleFor(author => (int)author.AuthorType) // Cast to int
                .InclusiveBetween(1, 4)
                .WithMessage("AuthorType must be between 1 and 4 inclusive.");
        }
    }
}
