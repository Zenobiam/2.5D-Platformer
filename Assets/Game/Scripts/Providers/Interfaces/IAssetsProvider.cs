using UnityEngine;

namespace Game.Scripts.Providers.Interfaces
{
    public interface IAssetsProvider
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 initPosition);
    }
}