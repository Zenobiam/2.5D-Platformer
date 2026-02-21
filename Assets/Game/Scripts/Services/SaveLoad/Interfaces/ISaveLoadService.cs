using Game.Scripts.Data;
using Game.Scripts.Infrastructure;

namespace Game.Scripts.Services.SaveLoad.Interfaces
{
    public interface ISaveLoadService : IService
    {
        void SaveProgress();
        PlayerProgress LoadProgress();
    }
}