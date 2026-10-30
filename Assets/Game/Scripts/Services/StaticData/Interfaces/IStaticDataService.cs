using Game.Scripts.Infrastructure;
using Game.Scripts.StaticData;
using Game.Scripts.StaticData.Windows;
using Game.Scripts.UI.Services.Windows;

namespace Game.Scripts.Services.StaticData.Interfaces
{
    public interface IStaticDataService : IService
    {
        void Load();
        EnemyStaticData ForEnemy(EnemyTypeId typeId);
        LevelStaticData ForLevel(string sceneKey);
        WindowConfig ForWindow(WindowId windowId);
    }
}