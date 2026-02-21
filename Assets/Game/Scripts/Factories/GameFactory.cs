using System.Collections.Generic;
using Game.Scripts.Constants;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;

namespace Game.Scripts.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        
        public List<ISaveProgressReader> ProgressReaders { get; } = new List<ISaveProgressReader>();
        public List<ISaveProgress> ProgressWriters { get; } = new List<ISaveProgress>();
        
        public GameFactory(IAssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
        }

        public GameObject CreatePlayer(GameObject InitialPoint) => 
            InstantiateAndRegister(AssetPath.PlayerPath, InitialPoint.transform.position);

        public void CreateHud() => 
            InstantiateAndRegister(AssetPath.HudPath);

        public void CleanUp()
        {
            ProgressWriters.Clear();
            ProgressReaders.Clear();
        }

        private GameObject InstantiateAndRegister(string prefabPath, Vector3 position)
        {
            GameObject gameObject = _assetsProvider.Instantiate(prefabPath, at: position);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }
        
        private GameObject InstantiateAndRegister(string prefabPath)
        {
            GameObject gameObject = _assetsProvider.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISaveProgressReader progressReader in gameObject.GetComponentsInChildren<ISaveProgressReader>())
                Register(progressReader);
        }

        private void Register(ISaveProgressReader progressReader)
        {
            if (progressReader is ISaveProgress progressWriter)
                ProgressWriters.Add(progressWriter);
            
            ProgressReaders.Add(progressReader);
        }
    }
}