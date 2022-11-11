using AutoMapper;

namespace NZWalks.API.Profiles
{
    public class WalkDifficultyProfile: Profile
    {
        public WalkDifficultyProfile()
        {
            CreateMap<Models.Domain.WalkDifficulty, Models.DTO.WalkDifficulty>().ReverseMap();
            CreateMap<Models.Domain.WalkDifficulty, Models.DTO.UpdateWalkDifficultyRequest>().ReverseMap();
            CreateMap<Models.Domain.WalkDifficulty, Models.DTO.AddWalkDifficultyRequest>().ReverseMap();

        }
    }
}
