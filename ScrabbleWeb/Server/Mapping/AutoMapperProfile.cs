using AutoMapper;
using ScrabbleData;
using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Server.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Game, GameData>()
                .ForMember(gd => gd.GameId, options => options.Ignore())
                .ReverseMap()
                .ForMember(g => g.GameId, options => options.MapFrom(gd => gd.GameId));
            CreateMap<Game, GamePlayers>().ReverseMap();
        }
    }
}
