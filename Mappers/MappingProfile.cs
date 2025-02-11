using AutoMapper;
using WeatherApi.Models;
using WeatherApi.DTOs;
using sp_backend.DTO;

namespace WeatherApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Equipment Model -> DTO
            CreateMap<Equipment, EquipmentDto>()
                .ForMember(dest => dest.SubEquipments, opt => opt.MapFrom(src => src.SubEquipments));
            CreateMap<EquipmentDto, Equipment>();

            // SubEquipment Model -> DTO
            CreateMap<SubEquipment, SubEquipmentDto>();
            CreateMap<SubEquipmentDto, SubEquipment>();
            CreateMap<Account, AccountResponseDTO>();
            CreateMap<Account, AccountDTO>();

            // Map AccountDTO -> Account
            CreateMap<AccountDTO, Account>();
        }
    }
}
