using AutoMapper;
using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;

namespace InventoryApp.API.Profiles;

public class TokenProfile : Profile
{
    public TokenProfile()
    {
        CreateMap<AccessToken,CreateTokenDTO>().ReverseMap();
        CreateMap<AccessToken,UpdateTokenDTO>().ReverseMap();
        CreateMap<AccessToken,TokenDTO>().ReverseMap();
    }
}