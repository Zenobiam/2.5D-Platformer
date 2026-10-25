using Game.Scripts.Infrastructure;
using Game.Scripts.StaticData;

namespace Game.Scripts.Services.StaticData.Interfaces
{
    public interface IStaticDataService : IService
    {
        void LoadEnemies();
        EnemyStaticData ForEnemy(EnemyTypeId typeId);
    }
}