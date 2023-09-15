using FluentValidation;
using FluentValidation.Validators;
using LakeXplorer.Models.Dto.UserDtos;

namespace LakeXplorer.Validators.UserValidator
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required")
                .NotNull().WithMessage("Username is required")
                .MaximumLength(15).WithMessage("Username is to long");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .NotNull().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password is to short")
                .MaximumLength(15).WithMessage("Password is to long");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .NotNull().WithMessage("Email is required")
                .EmailAddress(EmailValidationMode.Net4xRegex)
                .MaximumLength(120).WithMessage("Email address is to long");
        }
    }
}
