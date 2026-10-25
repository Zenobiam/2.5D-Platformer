using System;
using System.Collections.Generic;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;

namespace Game.Scripts.Factories.Interfaces
{
    public interface IGameFactory : IService
    {
        GameObject CreatePlayer(GameObject InitialPoint);
        GameObject PlayerGameObject { get; }
        event Action PlayerCreated;
        GameObject CreateHud();
        List<ISaveProgressReader> ProgressReaders { get; }
        List<ISaveProgress> ProgressWriters { get; }
        void CleanUp();
        void Register(ISaveProgressReader progressReader);
    }
}
