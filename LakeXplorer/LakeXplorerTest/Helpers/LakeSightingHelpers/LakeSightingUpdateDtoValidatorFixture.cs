using FluentValidation;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Validators.LakeSightingValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.LakeSightingHelpers
{
    public class LakeSightingUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public LakeSightingUpdateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<LakeSightingUpdateDto>, LakeSightingUpdateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
