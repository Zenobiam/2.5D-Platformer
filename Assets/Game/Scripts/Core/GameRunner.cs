using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core
{
    public class GameRunner : MonoBehaviour
    {
        public GameBootstrapper BootstrapperPrefab;

        private void Awake()
        {
            var bootstrapper = FindObjectsByType<GameBootstrapper>(FindObjectsSortMode.None);
            
            if (bootstrapper == null || bootstrapper.Length == 0)
                Instantiate(BootstrapperPrefab);
        }
    }
}