using FluentValidation;
using LakeXplorer.Models.Dto.LakeDtos;

namespace LakeXplorer.Validators.LakeValidator
{
    public class LakeCreateDtoValidator : AbstractValidator<LakeCreateDto>
    {
        public LakeCreateDtoValidator()
        {
            RuleFor(lake => lake.Name)
                .NotEmpty().WithMessage("Name is required")
                .NotNull().WithMessage("Name is required")
                .MaximumLength(15).WithMessage("Name is to long");

            RuleFor(lake => lake.Description)
                .NotEmpty().WithMessage("Description is required")
                .NotNull().WithMessage("Description is required");

            RuleFor(lake => lake.FormFile)
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
