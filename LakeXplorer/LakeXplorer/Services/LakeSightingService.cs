using AutoMapper;
using FluentValidation;
using LakeXplorer.Helpers;
using LakeXplorer.Models;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Repository.IRepository;
using LakeXplorer.Services.ISerices;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;

namespace LakeXplorer.Services
{
    public class LakeSightingService : ILakeSightingService
    {
        private readonly ILakeSightingRepository _lakeSightingRepository;
        private readonly IValidator<LakeSightingCreateDto> _lakeSightningCreateValidator;
        private readonly IValidator<LakeSightingUpdateDto> _lakeSightningUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILikeService _likeService;

        public LakeSightingService(ILakeSightingRepository lakeSightingRepository, IValidator<LakeSightingCreateDto> lakeSightningCreateValidator, IValidator<LakeSightingUpdateDto> lakeSightningUpdateValidator, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILikeService likeService)
        {
            _lakeSightingRepository = lakeSightingRepository;
            _lakeSightningCreateValidator = lakeSightningCreateValidator;
            _lakeSightningUpdateValidator = lakeSightningUpdateValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _likeService = likeService;
        }

        public async Task<List<LakeSightingViewDto>> GetAll()
        {
            var lakesSighting = await _lakeSightingRepository.GetAll();
            return _mapper.Map<List<LakeSightingViewDto>>(lakesSighting);
        }

        public async Task<LakeSightingViewDto> GetById(int id)
        {
            Expression<Func<LakeSighting, bool>> condition = x => x.Id == id;
            var lakeSighting = await _lakeSightingRepository.GetByCondition(condition);
            var lakeSightingViewDto = _mapper.Map<LakeSightingViewDto>(lakeSighting);

            lakeSightingViewDto.Image = await File.ReadAllBytesAsync(lakeSighting.ImageUrl);

            return lakeSightingViewDto;
        }

        public async Task Post(int lakeId, LakeSightingCreateDto lakeSightingCreateDto)
        {
            var validator = await _lakeSightningCreateValidator.ValidateAsync(lakeSightingCreateDto);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var path = "Images/LakeSightingImages/";

            var lakeSighting = _mapper.Map<LakeSighting>(lakeSightingCreateDto);
            lakeSighting.LakeId = lakeId;
            lakeSighting.UserId = GetUserId();
            lakeSighting.ImageUrl = path + lakeSightingCreateDto.FormFile.FileName;
            lakeSighting.FunFact = GetFunFact();

            await _lakeSightingRepository.Post(lakeSighting);
            SaveImage(path, lakeSightingCreateDto.FormFile);
        }

        private string GetFunFact()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "X-RapidAPI-Key", "ba6e02a674msh46b836ce4ca183ap1094c0jsn70100b336662" },
                    { "X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com" },
                },
            };
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var body = response.Content.ReadAsStringAsync().Result;
            var funFact = JsonConvert.DeserializeObject<FunFact>(body)
                ?? throw new Exception("FunFact is not fetch");
            return funFact.Value;
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

        public async Task DailyFunFact()
        {
            var lakeSightings = await _lakeSightingRepository.GetAll();

            foreach (var lakeSighting in lakeSightings)
            {
                lakeSighting.FunFact = GetFunFact();
                await _lakeSightingRepository.Update(lakeSighting);
            }
        }

        public async Task Update(int id, LakeSightingUpdateDto lakeSightingUpdateDto)
        {
            var validator = await _lakeSightningUpdateValidator.ValidateAsync(lakeSightingUpdateDto);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var lakeSighting = await GetLakeSightingById(id);
            File.Delete(lakeSighting.ImageUrl);

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (!userId.Equals(lakeSighting.UserId) && !userRole.Equals("Admin"))
            {
                throw new Exception("This is not your LakeSighting, you can not update");
            }

            var path = "Images/LakeSightingImages/";


            lakeSighting.ImageUrl = path + lakeSightingUpdateDto.FormFile.FileName;

            await _lakeSightingRepository.Update(lakeSighting);
            
            SaveImage(path, lakeSightingUpdateDto.FormFile);
        }

        private async Task<LakeSighting> GetLakeSightingById(int id)
        {
            Expression<Func<LakeSighting, bool>> condition = x => x.Id == id;
            return await _lakeSightingRepository.GetByCondition(condition);
        }

        private string GetUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                         x.Type == ClaimTypes.Role)?.Value
                         ?? throw new Exception("You must login first");
        }

        public async Task Like(int id)
        {
            var userId = GetUserId();

            await _likeService.Post(id, userId);
        }

        public async Task Delete(int id)
        {
            var lakeSighting = await GetLakeSightingById(id);
            var userId = GetUserId();
            var userRole = GetUserRole();

            if (!userId.Equals(lakeSighting.UserId) && !userRole.Equals("Admin"))
            {
                throw new Exception("This is not your LakeSighting, you can not delete");
            }

            await _lakeSightingRepository.Delete(lakeSighting);
        }

        public async Task Delete(LakeSighting lakeSighting)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();

            if (!userId.Equals(lakeSighting.UserId) && !userRole.Equals("Admin"))
            {
                throw new Exception("This is not your LakeSighting, you can not delete");
            }

            await _lakeSightingRepository.Delete(lakeSighting);
        }

        public async Task Dislike(int id)
        {
            var userId = GetUserId();

            await _likeService.Delet(id, userId);
        }
    }
}
