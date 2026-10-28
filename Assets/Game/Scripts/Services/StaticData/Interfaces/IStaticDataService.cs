using Game.Scripts.Infrastructure;
using Game.Scripts.StaticData;

namespace Game.Scripts.Services.StaticData.Interfaces
{
    public interface IStaticDataService : IService
    {
        void Load();
        EnemyStaticData ForEnemy(EnemyTypeId typeId);
        LevelStaticData ForLevel(string sceneKey);
    }
}