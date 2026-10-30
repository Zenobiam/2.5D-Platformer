using Game.Scripts.UI.Services.Factory.Interfaces;
using Game.Scripts.UI.Services.Windows.Interfaces;

namespace Game.Scripts.UI.Services.Windows
{
    /// <summary>
    /// Открывает окно по WindowId. Кнопка знает только id, сервис — как его открыть (через UIFactory).
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;

        public WindowService(IUIFactory uiFactory) =>
            _uiFactory = uiFactory;

        public void Open(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.None:
                    break;
                case WindowId.Shop:
                    _uiFactory.CreateShop();
                    break;
            }
        }
    }
}
