using Game.Scripts.Data;

namespace Game.Scripts.Services.PersistentProgress.Interfaces
{
    public interface ISaveProgressReader
    {
        void LoadProgress(PlayerProgress progress);
    }

    public interface ISaveProgress : ISaveProgressReader
    {
        void UpdateProgress(PlayerProgress progress);
    }
}