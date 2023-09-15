using FluentValidation;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Validators.LakeValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.LakeHelpers
{
    public class LakeCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public LakeCreateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<LakeCreateDto>, LakeCreateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
