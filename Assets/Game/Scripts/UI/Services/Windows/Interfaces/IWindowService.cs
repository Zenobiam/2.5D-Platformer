using Game.Scripts.Infrastructure;

namespace Game.Scripts.UI.Services.Windows.Interfaces
{
    public interface IWindowService : IService
    {
        void Open(WindowId windowId);
    }
}
