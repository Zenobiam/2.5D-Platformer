using Game.Scripts.Data;
using Game.Scripts.Infrastructure;

namespace Game.Scripts.Services.PersistentProgress.Interfaces
{
    public interface IPersistentProgressService : IService
    {
        PlayerProgress Progress { get; set; }
    }
}