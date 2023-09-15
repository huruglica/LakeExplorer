using AutoMapper;
using LakeXplorer.Models;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Models.Dto.LikeDto;
using LakeXplorer.Models.Dto.UserDtos;

namespace LakeXplorer.Helpers
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<User, UserViewDto>().ReverseMap()
                .ForMember(x => x.Lakes, y => y.MapFrom(z => z.Lakes));

            CreateMap<Lake, LakeCreateDto>().ReverseMap();
            CreateMap<Lake, LakeUpdateDto>().ReverseMap();
            CreateMap<Lake, LakeViewDto>().ReverseMap()
                .ForMember(x => x.LakeSighting, y => y.MapFrom(z => z.LakeSighting));

            CreateMap<LakeSighting, LakeSightingCreateDto>().ReverseMap();
            CreateMap<LakeSighting, LakeSightingUpdateDto>().ReverseMap();
            CreateMap<LakeSighting, LakeSightingViewDto>().ReverseMap()
                .ForMember(x => x.User, y => y.MapFrom(z => z.User))
                .ForMember(x => x.Likes, y => y.MapFrom(z => z.Likes));

            CreateMap<Like, LikeViewDto>().ReverseMap()
                .ForMember(x => x.User, y => y.MapFrom(z => z.User));
        }
    }
}
