using FluentValidation;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Validators.LakeValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.LakeHelpers
{
    public class LakeUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public LakeUpdateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<LakeUpdateDto>, LakeUpdateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
