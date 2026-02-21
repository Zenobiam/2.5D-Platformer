using Game.Scripts.Constants;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Providers.Interfaces;
using UnityEngine;

namespace Game.Scripts.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        
        public GameFactory(IAssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
        }

        public GameObject CreatePlayer(GameObject InitialPoint) => 
            _assetsProvider.Instantiate(AssetPath.PlayerPath, InitialPoint.transform.position);

        public void CreateHud() => 
            _assetsProvider.Instantiate(AssetPath.HudPath);
    }
}