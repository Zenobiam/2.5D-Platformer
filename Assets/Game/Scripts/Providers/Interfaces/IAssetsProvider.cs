using System.Threading.Tasks;
using Game.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Providers.Interfaces
{
    public interface IAssetsProvider : IService
    {
        void Initialize();
        Task<GameObject> Instantiate(string address);
        Task<GameObject> Instantiate(string address, Vector3 at);
        Task<T> Load<T>(AssetReference assetReference) where T : class;
        Task<T> Load<T>(string address) where T : class;
        void CleanUp();
    }
}
