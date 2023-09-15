using LakeXplorer.Helpers;
using LakeXplorer.Models.Dto.UserDtos;
using LakeXplorer.Models;

namespace LakeXplorer.Services.ISerices
{
    public interface IUserService
    {
        Task<List<UserViewDto>> GetAll();
        Task<UserViewDto> GetById(string id);
        Task<User> GetUserById(string id);
        Task<UserViewDto> GetMyData();
        Task<UserViewDto> GetByEmail(string email);
        Task<string> Login(UserLogin userLogin);
        Task Post(UserCreateDto userCreateDto);
        Task Update(string id, UserUpdateDto userUpdateDto);
        Task SetRole(string id, string role);
        Task Delete(string id);
    }
}
