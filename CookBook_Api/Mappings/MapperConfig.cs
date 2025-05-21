using AutoMapper;

namespace CookBook_Api.Mappings
{
    public class MapperConfig
    {
        public static IMapper InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RecipeProfile>();
            });
            return config.CreateMapper();
        }
    }
}
