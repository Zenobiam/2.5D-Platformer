using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.Factories.Interfaces
{
    public interface IGameFactory : IService
    {
        GameObject CreatePlayer(GameObject InitialPoint);
        void CreateHud();
    }
}
