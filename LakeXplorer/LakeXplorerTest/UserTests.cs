using AutoMapper;
using FluentValidation;
using LakeXplorer.Controllers;
using LakeXplorer.Models.Dto.UserDtos;
using LakeXplorer.Repository;
using LakeXplorer.Services;
using LakeXplorerTest.Helpers.UserHelpers;
using LakeXplorerTest.Helpers;
using Microsoft.AspNetCore.Http;
using LakeXplorer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LakeXplorer.Models;
using Microsoft.AspNetCore.Mvc;

namespace LakeXplorerTest
{
    public class UserTests : IClassFixture<UserCreateDtoValidatorFixture>,
                             IClassFixture<UserUpdateDtoValidatorFixture>,
                             IClassFixture<AutoMapperFixture>
    {
        private readonly UserController _userController;
        private readonly UserService _userService;

        private readonly UserCreateDtoValidatorFixture _userCreateDtoValidatorFixture;
        private readonly UserUpdateDtoValidatorFixture _userUpdateDtoValidatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly LakeXplorerDbContext _lakeXplorerDbContext;
        private readonly UserRepository _userRepository;

        private readonly IValidator<UserCreateDto> _userCreateValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserTests(UserCreateDtoValidatorFixture userCreateDtoValidatorFixture, UserUpdateDtoValidatorFixture userUpdateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            DbContextOptionsBuilder<LakeXplorerDbContext> dbOptions = new DbContextOptionsBuilder<LakeXplorerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _lakeXplorerDbContext = new LakeXplorerDbContext(dbOptions.Options);
            _userRepository = new UserRepository(_lakeXplorerDbContext);

            _userCreateDtoValidatorFixture = userCreateDtoValidatorFixture;
            _userUpdateDtoValidatorFixture = userUpdateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _httpContextAccessorHelper = new HttpContextAccessorHelper();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _userCreateValidator = _userCreateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<UserCreateDto>>();
            _userUpdateValidator = _userUpdateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<UserUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _userService = new UserService(_userRepository, _userCreateValidator, _userUpdateValidator, _mapper, _httpContextAccessor);
            _userController = new UserController(_userService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var userOne = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            var userTwo = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(userOne);
            await _userRepository.Post(userTwo);

            var result = await _userController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var users = await _userRepository.GetAll();
            Assert.Equal(2, users.Count);
        }

        [Theory]
        [InlineData("64f1bd63826610e30d527560", "64f1bd63826610e30d527561")]
        public async Task Test_GetById(string okId, string notOkId)
        {
            var user = new User
            {
                Id = okId,
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var result = await _userController.GetById(okId);
            var notOkResult = await _userController.GetById(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Fact]
        public async Task Test_GetMyDataOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var result = await _userController.GetMyData();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_GetMyDataNotOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460990",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var result = await _userController.GetMyData();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("Username", "email@email.ee", "Password")]
        public async Task Test_PostOk(string username, string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Username = username,
                Email = email,
                Password = password
            };

            var result = await _userController.Post(userCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("UsernameUsername", "email@email.ee", "Password")]
        [InlineData("", "email@email.ee", "Password")]
        [InlineData(null, "email@email.ee", "Password")]
        [InlineData("Username", "", "Password")]
        [InlineData("Username", null, "Password")]
        [InlineData("Username", "email@email", "Password")]
        [InlineData("Username", "email@.ee", "Password")]
        [InlineData("Username", "email@.email.ee", "Password")]
        [InlineData("Username", "@email.ee", "Password")]
        [InlineData("Username", "email@email.ee", "")]
        [InlineData("Username", "email@email.ee", null)]
        [InlineData("Username", "email@email.ee", "Pass")]
        [InlineData("Username", "email@email.ee", "PasswordPassword")]
        public async Task Test_PostNotOk(string username, string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Username = username,
                Email = email,
                Password = password
            };

            var result = await _userController.Post(userCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        public async Task Test_SetRoleOk(string role)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var result = await _userController.SetRole(user.Id, role);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Supervisor")]
        [InlineData("Regular")]
        [InlineData("Manager")]
        public async Task Test_SetRoleNotOk(string role)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var result = await _userController.SetRole(user.Id, role);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("UpdatedUsername")]
        public async Task Test_UpdateOk(string username)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var userUpdateDto = new UserUpdateDto
            {
                Username = username
            };

            var result = await _userController.Update(user.Id, userUpdateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("UsernameUsername")]
        public async Task Test_UpdateNotOk(string username)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "Creator"
            };

            await _userRepository.Post(user);

            var userUpdateDto = new UserUpdateDto
            {
                Username = username
            };

            var result = await _userController.Update(user.Id, userUpdateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("64f1bd63826610e30d527560", "64f1bd63826610e30d527561")]
        public async Task Test_Delete(string okId, string notOkId)
        {
            var user = new User
            {
                Id = okId,
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "Creator"
            };

            await _userRepository.Post(user);

            var result = await _userController.Delete(okId);
            var notOkResult = await _userController.Delete(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }
    }
}
