using Game.Scripts.Data;
using Game.Scripts.Services.PersistentProgress.Interfaces;

namespace Game.Scripts.Services.PersistentProgress
{
    public class PersistentProgressService : IPersistentProgressService
    {
        public PlayerProgress Progress { get; set; }
    }
}