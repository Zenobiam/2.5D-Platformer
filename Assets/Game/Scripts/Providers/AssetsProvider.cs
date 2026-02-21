using Game.Scripts.Providers.Interfaces;
using UnityEngine;

namespace Game.Scripts.Providers
{
    public class AssetsProvider : IAssetsProvider
    {
        public GameObject Instantiate(string path)
        {
            var playerPrefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(playerPrefab);
        }

        public GameObject Instantiate(string path, Vector3 initPosition)
        {
            var playerPrefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(playerPrefab, initPosition, Quaternion.identity);
        }
    }
}