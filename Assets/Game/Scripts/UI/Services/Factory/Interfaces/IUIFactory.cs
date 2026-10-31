using System.Threading.Tasks;
using Game.Scripts.Infrastructure;

namespace Game.Scripts.UI.Services.Factory.Interfaces
{
    public interface IUIFactory : IService
    {
        Task CreateUIRoot();
        void CreateShop();
    }
}
