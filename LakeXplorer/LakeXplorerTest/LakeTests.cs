using AutoMapper;
using FluentValidation;
using LakeXplorer.Controllers;
using LakeXplorer.Data;
using LakeXplorer.Repository;
using LakeXplorer.Services;
using LakeXplorerTest.Helpers;
using Microsoft.AspNetCore.Http;
using LakeXplorerTest.Helpers.LakeHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Services.ISerices;
using FakeItEasy;
using LakeXplorer.Models;
using Microsoft.AspNetCore.Mvc;
using LakeXplorer.Models.Dto.LakeSightingDtos;

namespace LakeXplorerTest
{
    public class LakeTests : IClassFixture<LakeCreateDtoValidatorFixture>,
                             IClassFixture<LakeUpdateDtoValidatorFixture>,
                             IClassFixture<AutoMapperFixture>
    {
        private readonly LakeController _lakeController;
        private readonly LakeService _lakeService;

        private readonly LakeCreateDtoValidatorFixture _lakeCreateDtoValidatorFixture;
        private readonly LakeUpdateDtoValidatorFixture _lakeUpdateDtoValidatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly LakeXplorerDbContext _lakeXplorerDbContext;
        private readonly LakeRepository _lakeRepository;

        private readonly IValidator<LakeCreateDto> _lakeCreateValidator;
        private readonly IValidator<LakeUpdateDto> _lakeUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILakeSightingService _lakeSightingService;

        public LakeTests(LakeCreateDtoValidatorFixture lakeCreateDtoValidatorFixture, LakeUpdateDtoValidatorFixture lakeUpdateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            DbContextOptionsBuilder<LakeXplorerDbContext> dbOptions = new DbContextOptionsBuilder<LakeXplorerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _lakeXplorerDbContext = new LakeXplorerDbContext(dbOptions.Options);
            _lakeRepository = new LakeRepository(_lakeXplorerDbContext);

            _lakeCreateDtoValidatorFixture = lakeCreateDtoValidatorFixture;
            _lakeUpdateDtoValidatorFixture = lakeUpdateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _httpContextAccessorHelper = new HttpContextAccessorHelper();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _lakeCreateValidator = _lakeCreateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<LakeCreateDto>>();
            _lakeUpdateValidator = _lakeUpdateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<LakeUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _lakeSightingService = A.Fake<ILakeSightingService>();

            _lakeService = new LakeService(_lakeRepository, _lakeCreateValidator, _lakeUpdateValidator, _mapper,
                _httpContextAccessor, _lakeSightingService);
            _lakeController = new LakeController(_lakeService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var lakeOne = new Lake
            {
                Id = 1,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            var lakeTwo = new Lake
            {
                Id = 2,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lakeOne);
            await _lakeRepository.Post(lakeTwo);

            var result = await _lakeController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var lakes = await _lakeRepository.GetAll();
            Assert.Equal(2, lakes.Count);
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Test_GetById(int okId, int notOkId)
        {
            var lake = new Lake
            {
                Id = okId,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Images/TestImage.jpeg",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lake);

            var result = await _lakeController.GetById(okId);
            var notOkResult = await _lakeController.GetById(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Fact]
        public async Task Test_PostOk()
        {
            var fakeFromFile = new FakeFormFile
            {
                FileName = "image.jpeg",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeCreateDto = new LakeCreateDto
            {
                Name = "Name",
                Description = "Description",
                FormFile = fakeFromFile
            };

            var result = await _lakeController.Post(lakeCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("Name", "", "image.jpeg")]
        [InlineData("Name", null, "image.jpeg")]
        [InlineData("", "Description", "image.jpeg")]
        [InlineData(null, "Description", "image.jpeg")]
        [InlineData("NameNameNameName", "Description", "image.jpeg")]
        [InlineData("Name", "Description", "image.docs")]
        public async Task Test_PostNotOk(string name, string description, string fileName)
        {
            var fakeFromFile = new FakeFormFile
            {
                FileName = fileName,
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeCreateDto = new LakeCreateDto
            {
                Name = name,
                Description = description,
                FormFile = fakeFromFile
            };

            var result = await _lakeController.Post(lakeCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_UpdateOk()
        {
            var lake = new Lake
            {
                Id = 1,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lake);

            var fakeFromFile = new FakeFormFile
            {
                FileName = "image.jpeg",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeUpdateDto = new LakeUpdateDto
            {
                FormFile = fakeFromFile
            };

            var result = await _lakeController.Update(lake.Id, lakeUpdateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, "image.docs")]
        [InlineData(2, "image.jpeg")]
        public async Task Test_UpdateNotOk(int id, string fileName)
        {
            var lake = new Lake
            {
                Id = 1,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lake);

            var fakeFromFile = new FakeFormFile
            {
                FileName = fileName,
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeUpdateDto = new LakeUpdateDto
            {
                FormFile = fakeFromFile
            };

            var result = await _lakeController.Update(id, lakeUpdateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_AddLakeSightingOk()
        {
            var lake = new Lake
            {
                Id = 1,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lake);

            var fakeFormFile = new FakeFormFile
            {
                FileName = "image.jpeg",
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingCreateDto = new LakeSightingCreateDto
            {
                Longitude = 12.34,
                Latitude = 56.78,
                FormFile = fakeFormFile
            };

            var result = await _lakeController.AddLakeSighting(lake.Id, lakeSightingCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, "image.docs")]
        [InlineData(2, "image.jpeg")]
        public async Task Test_AddLakeSightingNotOk(int id, string fileName)
        {
            var lake = new Lake
            {
                Id = 1,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lake);

            var fakeFormFile = new FakeFormFile
            {
                FileName = fileName,
                Length = 100,
                ContentType = "image/jpeg"
            };

            var lakeSightingCreateDto = new LakeSightingCreateDto
            {
                Longitude = 12.34,
                Latitude = 56.78,
                FormFile = fakeFormFile
            };

            var result = await _lakeController.AddLakeSighting(id, lakeSightingCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Test_Delete(int okId, int notOkId)
        {
            var lakeOne = new Lake
            {
                Id = okId,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            var lakeTwo = new Lake
            {
                Id = 3,
                Name = "Name",
                Description = "Description",
                ImageUrl = "Image",
                UserId = "UserId"
            };

            await _lakeRepository.Post(lakeOne);
            await _lakeRepository.Post(lakeTwo);

            var result = await _lakeController.Delete(okId);
            var notOkResult = await _lakeController.Delete(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);

            var lakes = await _lakeRepository.GetAll();
            Assert.Single(lakes);
        }
    }
}
