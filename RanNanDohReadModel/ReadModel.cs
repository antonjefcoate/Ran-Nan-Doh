using System;
using System.Linq;
using System.Collections.Generic;
using RanNanDoh.DataViews;
using SimpleCQRS;

namespace SimpleCQRS
{
    public static class BullShitDatabase 
    {
        public static Dictionary<Guid, PlayerDto> Users = new Dictionary<Guid, PlayerDto>();
        public static Dictionary<Guid, UserRoundResultListDto> UserRoundList = new Dictionary<Guid, UserRoundResultListDto>();
    }
}

namespace RanNanDoh.ReadModels
{
    public interface IPlayerReadModel
    {
        PlayerDto GetUser(Guid id);
        List<UserRoundResultDto> GetPlayerSummaryGameHistory(Guid id);

        PlayerListDto GetAllPlayers();
        FacebookPlayerDto GetFacebookPlayer(long fbUid);
    }

    public class PlayerReadModel : IPlayerReadModel
    {
        private readonly IViewModelReader<UserRoundResultListDto> _modelReader;

        public PlayerReadModel(IViewModelReader<UserRoundResultListDto> modelReader)
        {
            _modelReader = modelReader;
        }

        public PlayerDto GetUser(Guid id)
        {
            return BullShitDatabase.Users[id];
        }

        public List<UserRoundResultDto> GetPlayerSummaryGameHistory(Guid id)
        {
            return _modelReader.Get(id).Items;
        }

        public PlayerListDto GetAllPlayers()
        {
            return new PlayerListDto() { 
                Names = BullShitDatabase.Users.Select(x => x.Value.Name).ToList(), 
                TotalCount = BullShitDatabase.Users .Count};
        }

        public FacebookPlayerDto GetFacebookPlayer(long fbUid)
        {
            var playerPair = BullShitDatabase.Users
                .Where(x => x.Value is FacebookPlayerDto)
                .FirstOrDefault(x => ((FacebookPlayerDto)x.Value).FacebookUid == fbUid);
            return (FacebookPlayerDto)playerPair.Value;
        }
    }
}
