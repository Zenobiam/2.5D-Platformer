using Game.Scripts.Infrastructure;

namespace Game.Scripts.UI.Services.Factory.Interfaces
{
    public interface IUIFactory : IService
    {
        void CreateUIRoot();
        void CreateShop();
    }
}
