using AutoMapper;
using ScrabbleWeb.Client.Game;
using ScrabbleWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GameDto, Game.Game>().ReverseMap();
        }
    }
}
