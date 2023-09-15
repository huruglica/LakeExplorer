using FluentValidation;
using LakeXplorer.Models.Dto.UserDtos;
using LakeXplorer.Validators.UserValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.UserHelpers
{
    public class UserCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public UserCreateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
