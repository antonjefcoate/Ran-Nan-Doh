using RanNanDoh.DataViews;

namespace RanNanDohUi.Models
{
    public interface IPlayerSession
    {
        PlayerDto Get();
        void Set(PlayerDto player);
    }
}