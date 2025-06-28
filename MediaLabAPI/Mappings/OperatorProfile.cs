using AutoMapper;
using MediaLabAPI.Models;
using MediaLabAPI.DTOs;

namespace MediaLabAPI.Mappings
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {
            CreateMap<C_ANA_Operators, OperatorDto>();
        }
    }
}