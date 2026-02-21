using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.Providers.Interfaces
{
    public interface IAssetsProvider : IService
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at);
    }
}