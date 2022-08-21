using AutoMapper;
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models.Users;
using XIVMarketBoard_Api.Repositories.Models.ResponseDto;

namespace XIVMarketBoard_Api.Helpers
{

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {

            // User -> AuthenticateResponse
            CreateMap<User, AuthenticateResponse>();

            // RegisterRequest -> User
            CreateMap<Ingredient, ResponseIngredient>();

            CreateMap<Item, ResponseItem>();

            CreateMap<Job, ResponseJob>();

            CreateMap<Recipe, ResponseRecipe>();

            CreateMap<UniversalisEntry, ResponseUniversalisEntry>();

            CreateMap<SaleHistory, ResponseUniversalisHistory>();

            CreateMap<MbPost, ResponseUniversalisPost>();

            CreateMap<ItemSearchCategory, ResponseItemCategory>();

            CreateMap<ItemUICategory, ResponseItemCategory>();

            // UpdateRequest -> User
            CreateMap<UpdateRequest, User>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
        }
    }
}
