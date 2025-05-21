using AutoMapper;
using CookBook_Api.DTOs;
using CookBook_Api.Models;

namespace CookBook_Api.Mappings
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Recipe, RecipeDTO>().ReverseMap();
        }
    }
}
