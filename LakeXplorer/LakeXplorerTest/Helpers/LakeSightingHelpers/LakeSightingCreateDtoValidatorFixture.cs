using FluentValidation;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Validators.LakeSightingValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.LakeSightingHelpers
{
    public class LakeSightingCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public LakeSightingCreateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<LakeSightingCreateDto>, LakeSightingCreateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
