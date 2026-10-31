using System.Threading.Tasks;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.Ads.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData.Windows;
using Game.Scripts.UI.Services.Factory.Interfaces;
using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Windows.Shop;
using UnityEngine;

namespace Game.Scripts.UI.Services.Factory
{
    /// <summary>
    /// Фабрика UI (урок 13): UIRoot — общий Canvas окон (Sort Order 1: поверх HUD, под шторкой), окна — из WindowStaticData.
    /// UIRoot создаётся при загрузке уровня до мира (LoadSceneState), окна — по кнопке через WindowService.
    /// </summary>
    public class UIFactory : IUIFactory
    {
        /// <summary>
        /// Адрес Addressables (урок 15; раньше — путь Resources "UI/UIRoot"). Ставит Tools → Setup → Addressables.
        /// </summary>
        private const string UIRootPath = "UIRoot";

        private readonly IAssetsProvider _assets;
        private readonly IStaticDataService _staticData;
        private readonly IPersistentProgressService _progressService;
        private readonly IAdsService _adsService;

        private Transform _uiRoot;

        public UIFactory(IAssetsProvider assets, IStaticDataService staticData, IPersistentProgressService progressService,
            IAdsService adsService)
        {
            _assets = assets;
            _staticData = staticData;
            _progressService = progressService;
            _adsService = adsService;
        }

        public async Task CreateUIRoot()
        {
            GameObject root = await _assets.Instantiate(UIRootPath);
            _uiRoot = root.transform;
        }

        public void CreateShop()
        {
            WindowConfig config = _staticData.ForWindow(WindowId.Shop);
            // Урок 14: окну магазина нужна реклама — общий WindowBase.Construct не хватает, фабрика знает конкретный тип.
            var window = (ShopWindow)Object.Instantiate(config.Template, _uiRoot);
            window.Construct(_adsService, _progressService);
        }
    }
}
