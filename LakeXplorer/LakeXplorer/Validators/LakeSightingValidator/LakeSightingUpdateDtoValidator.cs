using FluentValidation;
using LakeXplorer.Models.Dto.LakeSightingDtos;

namespace LakeXplorer.Validators.LakeSightingValidator
{
    public class LakeSightingUpdateDtoValidator : AbstractValidator<LakeSightingUpdateDto>
    {
        public LakeSightingUpdateDtoValidator()
        {
            RuleFor(lakeSighting => lakeSighting.FormFile)
                .NotEmpty().WithMessage("Image is required")
                .NotNull().WithMessage("Image is required")
                .Must(IsImage).WithMessage("The file should be image");
        }

        private bool IsImage(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return false;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };

            var fileExtension = Path.GetExtension(formFile.FileName).ToLower();
            var contentType = formFile.ContentType.ToLower();

            return allowedExtensions.Contains(fileExtension) &&
                   allowedMimeTypes.Contains(contentType);
        }
    }
}
