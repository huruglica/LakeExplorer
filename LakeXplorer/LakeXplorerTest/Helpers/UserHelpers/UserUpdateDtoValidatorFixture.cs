using FluentValidation;
using LakeXplorer.Models.Dto.UserDtos;
using LakeXplorer.Validators.UserValidator;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers.UserHelpers
{
    public class UserUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public UserUpdateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
