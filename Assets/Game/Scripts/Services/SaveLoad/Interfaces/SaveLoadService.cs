using Game.Scripts.Data;
using Game.Scripts.Data.Extensions;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;

namespace Game.Scripts.Services.SaveLoad.Interfaces
{
    public class SaveLoadService : ISaveLoadService
    {
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private const string Progress = "Progress";
        
        public SaveLoadService(IPersistentProgressService progressService, IGameFactory gameFactory)
        {
            _progressService = progressService;
            _gameFactory = gameFactory;
        }

        public void SaveProgress()
        {
            foreach (ISaveProgress progressWriter in _gameFactory.ProgressWriters)
                progressWriter.UpdateProgress(_progressService.Progress);
            
            PlayerPrefs.SetString(Progress, _progressService.Progress.ToJson());
        }

        public PlayerProgress LoadProgress() =>
            PlayerPrefs.GetString(Progress)?
                .ToDeserializes<PlayerProgress>();
    }
}