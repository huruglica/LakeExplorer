using AutoMapper;
using FluentValidation;
using LakeXplorer.Controllers;
using LakeXplorer.Data;
using LakeXplorer.Models;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Repository;
using LakeXplorer.Services;
using LakeXplorer.Services.ISerices;
using LakeXplorerTest.Helpers;
using LakeXplorerTest.Helpers.LakeSightingHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest
{
    public class LakeSightingTests : IClassFixture<LakeSightingCreateDtoValidatorFixture>,
                                     IClassFixture<LakeSightingUpdateDtoValidatorFixture>,
                                     IClassFixture<AutoMapperFixture>
    {
        private readonly LakeSightingController _lakeSightingController;
        private readonly LakeSightingService _lakeSightingService;

        private readonly LakeSightingCreateDtoValidatorFixture _lakeSightingCreateDtoValidatorFixture;
        private readonly LakeSightingUpdateDtoValidatorFixture _lakeSightingUpdateDtoValidatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly LakeXplorerDbContext _lakeXplorerDbContext;
        private readonly LakeSightingRepository _lakeSightingRepository;
        private readonly UserRepository _userRepository;
        private readonly LikeRepository _likeRepository;

        private readonly IValidator<LakeSightingCreateDto> _lakeSightingCreateValidator;
        private readonly IValidator<LakeSightingUpdateDto> _lakeSightingUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILikeService _likeService;

        public LakeSightingTests(LakeSightingCreateDtoValidatorFixture lakeSightingCreateDtoValidatorFixture, LakeSightingUpdateDtoValidatorFixture lakeSightingUpdateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            DbContextOptionsBuilder<LakeXplorerDbContext> dbOptions = new DbContextOptionsBuilder<LakeXplorerDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _lakeXplorerDbContext = new LakeXplorerDbContext(dbOptions.Options);
            _lakeSightingRepository = new LakeSightingRepository(_lakeXplorerDbContext);
            _userRepository = new UserRepository(_lakeXplorerDbContext);
            _likeRepository = new LikeRepository(_lakeXplorerDbContext);

            _lakeSightingCreateDtoValidatorFixture = lakeSightingCreateDtoValidatorFixture;
            _lakeSightingUpdateDtoValidatorFixture = lakeSightingUpdateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _httpContextAccessorHelper = new HttpContextAccessorHelper();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _lakeSightingCreateValidator = _lakeSightingCreateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<LakeSightingCreateDto>>();
            _lakeSightingUpdateValidator = _lakeSightingUpdateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<LakeSightingUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _likeService = new LikeService(_likeRepository);

            _lakeSightingService = new LakeSightingService(_lakeSightingRepository, _lakeSightingCreateValidator,
                                   _lakeSightingUpdateValidator, _mapper, _httpContextAccessor, _likeService);
            _lakeSightingController = new LakeSightingController(_lakeSightingService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var lakeSightingOne = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            var lakeSightingTwo = new LakeSighting
            {
                Id = 2,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSightingOne);
            await _lakeSightingRepository.Post(lakeSightingTwo);

            var result = await _lakeSightingController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var lakeSightings = await _lakeSightingRepository.GetAll();
            Assert.Equal(2, lakeSightings.Count);
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Test_GetById(int okId, int notOkId)
        {
            var user = new User
            {
                Id = "UserId",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var lakeSighting = new LakeSighting
            {
                Id = okId,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Images/TestImage.jpeg",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var result = await _lakeSightingController.GetById(okId);
            var notOkResult = await _lakeSightingController.GetById(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Fact]
        public async Task Test_PostOk()
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

            var fakeFromFile = new FakeFormFile
            {
                FileName = "image.jpeg",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingCreateDto = new LakeSightingCreateDto
            {
                Longitude = 1,
                Latitude = 1,
                FormFile = fakeFromFile
            };

            var result = await _lakeSightingController.Post(1, lakeSightingCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_PostNotOk()
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

            var fakeFromFile = new FakeFormFile
            {
                FileName = "image.docs",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingCreateDto = new LakeSightingCreateDto
            {
                Longitude = 1,
                Latitude = 1,
                FormFile = fakeFromFile
            };

            var result = await _lakeSightingController.Post(1, lakeSightingCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_UpdateOk()
        {
            var user = new User
            {
                Id = "UserId",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var lakeSighting = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var fakeFromFile = new FakeFormFile
            {
                FileName = "image.jpeg",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingUpdateDto = new LakeSightingUpdateDto
            {
                FormFile = fakeFromFile
            };

            var result = await _lakeSightingController.Update(lakeSighting.Id, lakeSightingUpdateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, "image.docs")]
        [InlineData(2, "image.jpeg")]
        public async Task Test_UpdateNotOk(int id, string fileName)
        {
            var user = new User
            {
                Id = "UserId",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var lakeSighting = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var fakeFromFile = new FakeFormFile
            {
                FileName = fileName,
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingUpdateDto = new LakeSightingUpdateDto
            {
                FormFile = fakeFromFile
            };

            var result = await _lakeSightingController.Update(id, lakeSightingUpdateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_Like()
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

            var lakeSighting = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var result = await _lakeSightingController.Like(lakeSighting.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Test_Delete(int okId, int notOkId)
        {
            var user = new User
            {
                Id = "UserId",
                Username = "Username",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User"
            };

            await _userRepository.Post(user);

            var lakeSighting = new LakeSighting
            {
                Id = okId,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var result = await _lakeSightingController.Delete(okId);
            var notOkResult = await _lakeSightingController.Delete(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Fact]
        public async Task Test_DislikeOk()
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

            var lakeSighting = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var like = new Like
            {
                UserId = user.Id,
                LakeSightingId = lakeSighting.Id,
            };

            await _likeRepository.Post(like);

            var result = await _lakeSightingController.Dislike(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, "UserId")]
        [InlineData(2, "64dcd34fe55c1e2ee8460991")]
        public async Task Test_DislikeNotOk(int id, string userId)
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

            var lakeSighting = new LakeSighting
            {
                Id = 1,
                Longitude = 1,
                Latitude = 1,
                ImageUrl = "Image",
                UserId = "UserId",
                LakeId = 1,
                FunFact = "FunFact"
            };

            await _lakeSightingRepository.Post(lakeSighting);

            var like = new Like
            {
                UserId = userId,
                LakeSightingId = id
            };

            await _likeRepository.Post(like);

            var result = await _lakeSightingController.Dislike(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
