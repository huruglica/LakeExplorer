using AutoMapper;
using FluentValidation;
using LakeXplorer.Models;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Repository.IRepository;
using LakeXplorer.Services.ISerices;
using System.Linq.Expressions;
using System.Security.Claims;

namespace LakeXplorer.Services
{
    public class LakeService : ILakeService
    {
        private readonly ILakeRepository _lakeRepository;
        private readonly IValidator<LakeCreateDto> _lakeCreateValidator;
        private readonly IValidator<LakeUpdateDto> _lakeUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILakeSightingService _lakeSightingService;

        public LakeService(ILakeRepository lakeRepository, IValidator<LakeCreateDto> lakeCreateValidator, IValidator<LakeUpdateDto> lakeUpdateValidator, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILakeSightingService lakeSightingService)
        {
            _lakeRepository = lakeRepository;
            _lakeCreateValidator = lakeCreateValidator;
            _lakeUpdateValidator = lakeUpdateValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _lakeSightingService = lakeSightingService;
        }

        public async Task<List<LakeViewDto>> GetAll()
        {
            var lakes = await _lakeRepository.GetAll();
            return _mapper.Map<List<LakeViewDto>>(lakes);
        }

        public async Task<LakeViewDto> GetById(int id)
        {
            Expression<Func<Lake, bool>> condition = x => x.Id == id;
            var lake = await _lakeRepository.GetByCondition(condition);
            var lakeViewDto = _mapper.Map<LakeViewDto>(lake);

            lakeViewDto.Image = await File.ReadAllBytesAsync(lake.ImageUrl);

            return lakeViewDto;
        }

        public async Task Post(LakeCreateDto lakeCreateDto)
        {
            var validator = await _lakeCreateValidator.ValidateAsync(lakeCreateDto);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var path = "Images/LakeImages/";

            var userId = GetUserId();

            var lake = _mapper.Map<Lake>(lakeCreateDto);
            lake.UserId = userId;
            lake.ImageUrl = path + lakeCreateDto.FormFile.FileName;

            await _lakeRepository.Post(lake);
            
            SaveImage(path, lakeCreateDto.FormFile);
        }

        private void SaveImage(string path, IFormFile formFile)
        {
            FileStream fileStream = File.Create(path + formFile.FileName);
            formFile.CopyTo(fileStream);
            fileStream.Flush();
        }

        private string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                         x.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? throw new Exception("You must login first");
        }

        public async Task Update(int id, LakeUpdateDto lakeUpdateDto)
        {
            var validator = await _lakeUpdateValidator.ValidateAsync(lakeUpdateDto);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var path = "Images/LakeImages/";

            var lake = await GetLakeById(id);
            File.Delete(lake.ImageUrl);


            lake.ImageUrl = path + lakeUpdateDto.FormFile.FileName;
            await _lakeRepository.Update(lake);
            
            SaveImage(path, lakeUpdateDto.FormFile);
        }

        private async Task<Lake> GetLakeById(int id)
        {
            Expression<Func<Lake, bool>> condition = x => x.Id == id;
            return await _lakeRepository.GetByCondition(condition);
        }

        public async Task AddLakeSighting(int id, LakeSightingCreateDto lakeSightingCreateDto)
        {
            await _lakeSightingService.Post(id, lakeSightingCreateDto);
        }

        public async Task Delete(int id)
        {
            var lake = await GetLakeById(id);

            await _lakeRepository.Delete(lake);
        }
    }
}
