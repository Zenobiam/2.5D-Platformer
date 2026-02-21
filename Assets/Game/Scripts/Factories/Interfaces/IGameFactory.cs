using UnityEngine;

namespace Game.Scripts.Factories.Interfaces
{
    public interface IGameFactory
    {
        GameObject CreatePlayer(GameObject InitialPoint);
        void CreateHud();
    }
}
