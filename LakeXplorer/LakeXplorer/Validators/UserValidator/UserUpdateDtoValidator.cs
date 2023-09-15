using FluentValidation;
using LakeXplorer.Models.Dto.UserDtos;

namespace LakeXplorer.Validators.UserValidator
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("You must set an username")
                .NotNull().WithMessage("You must set an username")
                .MaximumLength(15).WithMessage("Ssername is to long");
        }
    }
}
