using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData.Windows;
using Game.Scripts.UI.Services.Factory.Interfaces;
using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Windows;
using UnityEngine;

namespace Game.Scripts.UI.Services.Factory
{
    /// <summary>
    /// Фабрика UI (урок 13): UIRoot — общий Canvas окон (Sort Order 1: поверх HUD, под шторкой), окна — из WindowStaticData.
    /// UIRoot создаётся при загрузке уровня до мира (LoadSceneState), окна — по кнопке через WindowService.
    /// </summary>
    public class UIFactory : IUIFactory
    {
        private const string UIRootPath = "UI/UIRoot";

        private readonly IAssetsProvider _assets;
        private readonly IStaticDataService _staticData;
        private readonly IPersistentProgressService _progressService;

        private Transform _uiRoot;

        public UIFactory(IAssetsProvider assets, IStaticDataService staticData, IPersistentProgressService progressService)
        {
            _assets = assets;
            _staticData = staticData;
            _progressService = progressService;
        }

        public void CreateUIRoot() =>
            _uiRoot = _assets.Instantiate(UIRootPath).transform;

        public void CreateShop()
        {
            WindowConfig config = _staticData.ForWindow(WindowId.Shop);
            WindowBase window = Object.Instantiate(config.Template, _uiRoot);
            window.Construct(_progressService);
        }
    }
}
