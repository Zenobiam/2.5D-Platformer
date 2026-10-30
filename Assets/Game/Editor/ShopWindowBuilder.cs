using System.Collections.Generic;
using System.Linq;
using Game.Scripts.StaticData.Windows;
using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Windows.Shop;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Editor
{
    /// <summary>
    /// Урок 12 (UI part 1): окно магазина как в итоге курса — префабы Window, ShopItem, Shop в Resources/UI.
    /// Шаг 1 — импорт спрайтов: границы 9-slice (значения курса) и без Physics Shape.
    /// Шаг 2 — префабы. Повторный запуск безопасен: объекты находятся по имени; раскладка, компоненты и Raycast Target
    /// задаются каждый раз, а спрайт, цвет, текст и размер шрифта — только при создании объекта (правки в Inspector сохраняются).
    /// Что и зачем по каждому компоненту — Ai.Tasks/2026-10-30-ui-shop-layout.md § «Компоненты: что и зачем».
    /// </summary>
    public static class ShopWindowBuilder
    {
        private const string UiFolder = "Assets/Resources/UI";
        private const string WindowPath = UiFolder + "/Window.prefab";
        private const string ShopItemPath = UiFolder + "/ShopItem.prefab";
        private const string ShopPath = UiFolder + "/Shop.prefab";
        private const string UIRootPath = UiFolder + "/UIRoot.prefab";

        private const string WindowDataFolder = "Assets/Resources/StaticData/UI";
        private const string WindowDataPath = WindowDataFolder + "/WindowStaticData.asset";

        private const string Fantasy = "Assets/ThirdParty/Simple Fantasy GUI/PNG/";
        private const string InventoryBox = Fantasy + "Inventory/Inventory_Box.png";
        private const string TitleBox = Fantasy + "Inventory/Title_Box.png";
        private const string CloseIcon = Fantasy + "Inventory/Close_Icon.png";
        private const string InventorySlot = Fantasy + "Inventory/Inventory_Slot.png";
        private const string ButtonShadow = Fantasy + "Menu Panel/ButtonShadow.png";
        private const string ButtonSprite = Fantasy + "Menu Panel/Button.png";
        private const string RedRibbon = Fantasy + "Character Panel/Red_Ribbon.png";
        private const string PlayerFrame = Fantasy + "Character Bar/Player_Frame.png";
        private const string LoginButtonHover = Fantasy + "Login Panel/LoginButton_Hover.png";
        private const string MoneyBar = Fantasy + "Inventory/Money_Bar.png";

        private const string Icons = "Assets/ThirdParty/RPG icons/512X512/";
        private const string AdsIcon = Icons + "Elixir_1.png";
        private static readonly string[] ShopItemIcons =
            { Icons + "Sword_1.png", Icons + "Helmet_1.png", Icons + "Shield.png", Icons + "Armor_1.png" };

        private const string CasualSheet = "Assets/ThirdParty/2D Casual UI/Sprite/GUI.png";
        private const string PlaySpriteName = "GUI_33";
        private const string PlayCircleSpriteName = "GUI_7";

        private const string SkullIcon = "Assets/Resources/Prefabs/Player/CFX3_T_Skull.png";
        private const string BlankSprite = "Assets/Game/Visuals/UI/blank.png";

        /// <summary>
        /// Границы 9-slice из курса (Sprite Editor → Border): x = Left, y = Bottom, z = Right, w = Top, пиксели текстуры.
        /// Нужны для Image Type = Sliced: углы не растягиваются, тянутся края и середина. 0 по оси — по ней тянется целиком.
        /// </summary>
        private static readonly (string Path, Vector4 Border)[] SlicedSprites =
        {
            (InventoryBox, new Vector4(154, 154, 154, 154)),
            (TitleBox, new Vector4(232, 0, 232, 0)),
            (InventorySlot, new Vector4(21, 21, 21, 21)),
            (ButtonSprite, new Vector4(20, 20, 20, 20)),
            (ButtonShadow, new Vector4(55, 0, 55, 0)),
            (RedRibbon, new Vector4(28, 0, 22, 0)),
        };

        [MenuItem("Tools/UI/Build Shop Window")]
        public static void Build()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Shop window: выйдите из Play Mode.");
                return;
            }

            SetupSpriteImport();

            if (!AssetDatabase.IsValidFolder(UiFolder))
                AssetDatabase.CreateFolder("Assets/Resources", "UI");

            EditPrefab(WindowPath, BuildWindow);
            EditPrefab(ShopItemPath, BuildShopItem);
            EditPrefab(ShopPath, BuildShop);
            EditPrefab(UIRootPath, BuildUIRoot);
            BuildWindowStaticData();

            AssetDatabase.SaveAssets();
            Debug.Log($"Shop window built: {WindowPath}, {ShopItemPath}, {ShopPath}, {UIRootPath}, {WindowDataPath}");
        }

        #region Import

        /// <summary>
        /// Generate Physics Shape выкл у всех спрайтов UI: контур нужен только 2D-физике (PolygonCollider2D), Image его не использует.
        /// В курсе снят у 8 из 9 спрайтов Simple Fantasy GUI; у нас — у всех спрайтов окна (единое правило).
        /// </summary>
        private static void SetupSpriteImport()
        {
            string[] uiSprites =
            {
                InventoryBox, TitleBox, CloseIcon, InventorySlot, ButtonShadow, ButtonSprite, RedRibbon, PlayerFrame,
                LoginButtonHover, MoneyBar, AdsIcon, CasualSheet, SkullIcon, BlankSprite,
            };

            foreach (string path in uiSprites.Concat(ShopItemIcons))
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    Debug.LogError($"Shop window: нет текстуры {path}");
                    continue;
                }

                bool changed = false;

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                if (settings.spriteGenerateFallbackPhysicsShape)
                {
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    changed = true;
                }

                foreach ((string slicedPath, Vector4 border) in SlicedSprites)
                {
                    if (slicedPath == path && importer.spriteBorder != border)
                    {
                        importer.spriteBorder = border;
                        changed = true;
                    }
                }

                if (changed)
                    importer.SaveAndReimport();
            }
        }

        #endregion

        #region Prefabs

        /// <summary>
        /// Базовое окно: фон на весь родитель, плашка заголовка сверху, кнопка закрытия в правом верхнем углу.
        /// Close_button — в уроке 13 станет полем WindowBase.CloseButton.
        /// </summary>
        private static void BuildWindow(GameObject root)
        {
            Stretch(Rect(root.transform));

            (Image background, bool bgNew) = ImageChild(root.transform, "Background");
            Stretch(background.rectTransform);
            Sliced(background, InventoryBox, bgNew);

            (Image title, bool titleNew) = ImageChild(root.transform, "Title_back");
            SetRect(title.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, -50), new Vector2(-96, 100));
            Sliced(title, TitleBox, titleNew);

            TextChild(title.transform, "Text (TMP)", "PERFECT SHOP, SO MANY ITEMS", 46);

            (Image close, bool closeNew) = ImageChild(root.transform, "Close_button");
            SetRect(close.rectTransform, Vector2.one, Vector2.one, new Vector2(-30, -30), new Vector2(70, 70));
            if (closeNew)
                close.sprite = LoadSprite(CloseIcon);
            close.raycastTarget = true;
            MakeButton(close.gameObject, close);
        }

        /// <summary>
        /// Товар: слот (Sliced) с иконкой, количество, кнопка цены (череп + число).
        /// LayoutElement на корне — размер в HorizontalLayoutGroup прокрутки магазина.
        /// </summary>
        private static void BuildShopItem(GameObject root)
        {
            var rect = Rect(root.transform);
            rect.sizeDelta = new Vector2(450, 550);

            Image slot = GetOrAdd<Image>(root);
            Sliced(slot, InventorySlot, slot.sprite == null);
            slot.raycastTarget = false;

            LayoutElement size = GetOrAdd<LayoutElement>(root);
            size.preferredWidth = 450;
            size.preferredHeight = 550;

            (Image item, bool itemNew) = ImageChild(root.transform, "Item");
            SetRect(item.rectTransform, Center, Center, new Vector2(0, 50), new Vector2(300, 300));
            Icon(item, ShopItemIcons[0], itemNew);

            (Image button, bool buttonNew) = ImageChild(root.transform, "Button");
            SetRect(button.rectTransform, Vector2.zero, new Vector2(1, 0), new Vector2(0, 20), new Vector2(-48, 100));
            button.rectTransform.pivot = new Vector2(0.5f, 0);
            Sliced(button, ButtonSprite, buttonNew);
            button.raycastTarget = true;
            MakeButton(button.gameObject, button);

            Transform price = Child(button.transform, "Price_container").Transform;
            Stretch(Rect(price));
            HorizontalLayoutGroup priceLayout = GetOrAdd<HorizontalLayoutGroup>(price.gameObject);
            priceLayout.childAlignment = TextAnchor.MiddleCenter;
            priceLayout.spacing = 10;
            priceLayout.childControlWidth = true;
            priceLayout.childControlHeight = true;
            priceLayout.childForceExpandWidth = false;
            priceLayout.childForceExpandHeight = false;

            (Image skull, bool skullNew) = ImageChild(price, "Price_icon");
            Icon(skull, SkullIcon, skullNew);
            LayoutElement skullSize = GetOrAdd<LayoutElement>(skull.gameObject);
            skullSize.preferredWidth = 80;
            skullSize.preferredHeight = 80;

            TextMeshProUGUI priceText = TextChild(price, "Price_text", "999", 70);
            priceText.textWrappingMode = TextWrappingModes.NoWrap;

            TextMeshProUGUI count = TextChild(root.transform, "Count_text", "100", 70, CountTextColor);
            SetRect(count.rectTransform, new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300, 0));
        }

        /// <summary>
        /// Магазин: вложенный Window; Content делится долями — реклама слева 30%, прокрутка товаров справа 70%, подпись внизу 10%.
        /// У прокрутки свой Canvas: при скролле перестраивается только она, а не всё окно.
        /// </summary>
        private static void BuildShop(GameObject root)
        {
            var rect = Rect(root.transform);
            Stretch(rect);
            rect.sizeDelta = new Vector2(-72, -40);

            NestedPrefab(root.transform, WindowPath, "Window").transform.SetSiblingIndex(0);

            Transform content = Child(root.transform, "Content").Transform;
            SetRect(Rect(content), Vector2.zero, Vector2.one, new Vector2(2, -48), new Vector2(-116, -216));

            RewardedAdItem adItem = BuildAdsItem(content);
            BuildInApps(content);
            TextMeshProUGUI skullText = BuildMoneyBar(content);

            BindShopWindow(root, skullText, adItem);

            Transform bottom = Child(content, "Bottom_container").Transform;
            SetRect(Rect(bottom), Vector2.zero, new Vector2(1, 0.1f), Vector2.zero, new Vector2(-28, 0));
            TextMeshProUGUI label = TextChild(bottom, "Label_text", "Check shop often to get free items", 44);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 5), new Vector2(0, -10));
        }

        /// <summary>
        /// Рекламный товар (урок 14 — Rewarded ads): слот с иконкой и лентой FREE, кнопка WATCH AD.
        /// Кнопка без своего Image, как в курсе: нажатие ловит лента (Raycast Target) и поднимается к Button на родителе.
        /// Play — белый треугольник 2D Casual UI на зелёном круге (в курсе — готовая кнопка GUI CARTOON, у нас её нет).
        /// </summary>
        private static RewardedAdItem BuildAdsItem(Transform content)
        {
            Transform container = Child(content, "AdsItem_container").Transform;
            SetRect(Rect(container), new Vector2(0, 0.1f), new Vector2(0.3f, 0.9f), Vector2.zero, Vector2.zero);

            (Image item, bool itemNew) = ImageChild(container, "Item");
            SetRect(item.rectTransform, Center, Center, new Vector2(0, 50), new Vector2(400, 500));
            Sliced(item, InventorySlot, itemNew);

            (Image icon, bool iconNew) = ImageChild(item.transform, "Icon");
            SetRect(icon.rectTransform, Center, Center, Vector2.zero, new Vector2(300, 300));
            Icon(icon, AdsIcon, iconNew);

            (Image freeBack, bool freeNew) = ImageChild(item.transform, "FreeLabel_background");
            SetRect(freeBack.rectTransform, new Vector2(0, 1), Vector2.one, new Vector2(0, -50), new Vector2(60, 100));
            Sliced(freeBack, ButtonShadow, freeNew);
            TextChild(freeBack.transform, "Free_text", "FREE", 46);

            Transform watch = Child(container, "WatchAd_button").Transform;
            SetRect(Rect(watch), Center, Center, new Vector2(0, -220), new Vector2(440, 100));

            (Image ribbon, bool ribbonNew) = ImageChild(watch, "Label_background");
            SetRect(ribbon.rectTransform, Center, Center, new Vector2(58, 0), new Vector2(323, 100));
            Sliced(ribbon, RedRibbon, ribbonNew);
            ribbon.raycastTarget = true;
            TextMeshProUGUI watchText = TextChild(ribbon.transform, "Watch_text", "WATCH AD", 40);
            watchText.rectTransform.sizeDelta = new Vector2(-44, 0);

            (Image circle, bool circleNew) = ImageChild(watch, "Icon_background");
            SetRect(circle.rectTransform, Center, Center, new Vector2(-162, 0), new Vector2(140, 140));
            if (circleNew)
                circle.sprite = LoadSheetSprite(PlayCircleSpriteName);
            circle.preserveAspect = true;

            (Image play, bool playNew) = ImageChild(watch, "Icon");
            SetRect(play.rectTransform, Center, Center, new Vector2(-156, 0), new Vector2(60, 90));
            if (playNew)
                play.sprite = LoadSheetSprite(PlaySpriteName);
            play.preserveAspect = true;

            (Image frame, bool frameNew) = ImageChild(watch, "Frame");
            SetRect(frame.rectTransform, Center, Center, new Vector2(-162, 0), new Vector2(150, 150));
            if (frameNew)
                frame.sprite = LoadSprite(PlayerFrame);

            MakeButton(watch.gameObject, ribbon);

            TextMeshProUGUI notAvailable = TextChild(item.transform, "AdNotAvailable_text", "Ad not available", 40);
            SetRect(notAvailable.rectTransform, new Vector2(0, 0.5f), new Vector2(1, 0.5f), Vector2.zero, new Vector2(-80, 200));

            return BuildRewardedAdItem(container.gameObject, watch.gameObject, icon.gameObject, notAvailable.gameObject);
        }

        /// <summary>
        /// Урок 14: RewardedAdItem на AdsItem_container — кнопка показа рекламы (WatchAd_button), что видно, когда
        /// реклама готова (кнопка и иконка товара), и когда нет («Ad not available»). Видимость переключает код
        /// по IAdsService.IsRewardedVideoReady.
        /// </summary>
        private static RewardedAdItem BuildRewardedAdItem(GameObject container, GameObject watchButton, GameObject icon, GameObject notAvailable)
        {
            RewardedAdItem adItem = GetOrAdd<RewardedAdItem>(container);
            adItem.ShowAdButton = watchButton.GetComponent<UnityEngine.UI.Button>();
            adItem.AdActiveObjects = new[] { watchButton, icon };
            adItem.AdInactiveObjects = new[] { notAvailable };
            return adItem;
        }

        /// <summary>
        /// Прокрутка товаров: свой Canvas + GraphicRaycaster (без него кнопки внутри вложенного Canvas не нажимаются),
        /// ScrollRect → Viewport (Image + Mask режет то, что за границей) → Content (HorizontalLayoutGroup раскладывает,
        /// ContentSizeFitter растягивает по числу товаров — иначе ScrollRect не знает, сколько крутить).
        /// Content прижат к левому краю (anchors x = 0, pivot x = 0) — растёт вправо; в курсе растянут на Viewport.
        /// </summary>
        private static void BuildInApps(Transform content)
        {
            Transform container = Child(content, "InApps_container").Transform;
            SetRect(Rect(container), new Vector2(0.3f, 0.1f), new Vector2(1, 0.9f), Vector2.zero, Vector2.zero);
            GetOrAdd<Canvas>(container.gameObject);
            GetOrAdd<GraphicRaycaster>(container.gameObject);

            (Image scroll, bool scrollNew) = ImageChild(container, "Scroll");
            Stretch(scroll.rectTransform);
            scroll.rectTransform.sizeDelta = new Vector2(-50, -50);
            if (scrollNew)
            {
                scroll.sprite = LoadSprite(BlankSprite);
                scroll.color = new Color(1, 1, 1, 0);
            }
            scroll.raycastTarget = true;

            (Image viewport, bool viewportNew) = ImageChild(scroll.transform, "Viewport");
            Stretch(viewport.rectTransform);
            viewport.raycastTarget = true;
            Mask mask = GetOrAdd<Mask>(viewport.gameObject);
            mask.showMaskGraphic = false;

            Transform items = Child(viewport.transform, "Content").Transform;
            var itemsRect = Rect(items);
            SetRect(itemsRect, Vector2.zero, new Vector2(0, 1), Vector2.zero, new Vector2(0, -20));
            itemsRect.pivot = new Vector2(0, 0.5f);

            HorizontalLayoutGroup layout = GetOrAdd<HorizontalLayoutGroup>(items.gameObject);
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.spacing = 30;
            layout.padding = new RectOffset(20, 20, 0, 0);
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(items.gameObject);
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            for (int i = 0; i < ShopItemIcons.Length; i++)
            {
                string name = $"ShopItem_{i + 1}";
                bool isNew = items.Find(name) == null;
                GameObject shopItem = NestedPrefab(items, ShopItemPath, name);
                if (isNew)
                    shopItem.transform.Find("Item").GetComponent<Image>().sprite = LoadSprite(ShopItemIcons[i]);
            }

            ScrollRect scrollRect = GetOrAdd<ScrollRect>(scroll.gameObject);
            scrollRect.content = itemsRect;
            scrollRect.viewport = viewport.rectTransform;
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.scrollSensitivity = WheelScrollSensitivity;
        }

        /// <summary>
        /// Урок 13: плашка с числом черепов справа сверху (Money_container — верхние 10% Content, поэтому реклама
        /// и прокрутка опущены до 0.9). Skulls_container — Money_Bar + HorizontalLayoutGroup: череп и число в ряд,
        /// ширина числа меняется с суммой. Skull_text — поле ShopWindow.SkullText.
        /// </summary>
        private static TextMeshProUGUI BuildMoneyBar(Transform content)
        {
            Transform money = Child(content, "Money_container").Transform;
            SetRect(Rect(money), new Vector2(1, 0.9f), Vector2.one, new Vector2(0, 4), new Vector2(500, -8));
            Rect(money).pivot = new Vector2(1, 0.5f);

            (Image bar, bool barNew) = ImageChild(money, "Skulls_container");
            SetRect(bar.rectTransform, new Vector2(1, 0), Vector2.one, new Vector2(-30, 0), new Vector2(500, 0));
            bar.rectTransform.pivot = new Vector2(1, 0.5f);
            if (barNew)
                bar.sprite = LoadSprite(MoneyBar);

            HorizontalLayoutGroup layout = GetOrAdd<HorizontalLayoutGroup>(bar.gameObject);
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 30, 0, 0);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            (Image skull, bool skullNew) = ImageChild(bar.transform, "Skull_icon");
            Icon(skull, SkullIcon, skullNew);
            LayoutElement skullSize = GetOrAdd<LayoutElement>(skull.gameObject);
            skullSize.preferredWidth = 60;
            skullSize.preferredHeight = 60;

            TextMeshProUGUI text = TextChild(bar.transform, "Skull_text", "300", 70);
            text.textWrappingMode = TextWrappingModes.NoWrap;
            return text;
        }

        /// <summary>
        /// ShopWindow на корне Shop: CloseButton — крестик вложенного Window (private SerializeField — через SerializedObject),
        /// SkullText — число черепов. Без компонента UIFactory не возьмёт префаб как WindowBase.
        /// </summary>
        private static void BindShopWindow(GameObject root, TextMeshProUGUI skullText, RewardedAdItem adItem)
        {
            ShopWindow shopWindow = GetOrAdd<ShopWindow>(root);
            shopWindow.SkullText = skullText;
            shopWindow.AdItem = adItem;

            var closeButton = root.transform.Find("Window/Close_button").GetComponent<UnityEngine.UI.Button>();
            var serialized = new SerializedObject(shopWindow);
            serialized.FindProperty("CloseButton").objectReferenceValue = closeButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// UIRoot (урок 13): Canvas окон. Overlay, Sort Order 1 — поверх HUD (0), под шторкой (100);
        /// CanvasScaler 1920×1080 Match 0 — ширина 1920 на любом экране, фиксированные элементы окна не налезают;
        /// GraphicRaycaster — без него окна не нажимаются.
        /// </summary>
        private static void BuildUIRoot(GameObject root)
        {
            Canvas canvas = GetOrAdd<Canvas>(root);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;

            CanvasScaler scaler = GetOrAdd<CanvasScaler>(root);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0;

            GetOrAdd<GraphicRaycaster>(root);
        }

        /// <summary>
        /// WindowStaticData (урок 13): Shop → Shop.prefab. Грузит StaticDataService.Load при старте —
        /// без ассета NRE в BootstrapState.
        /// </summary>
        private static void BuildWindowStaticData()
        {
            if (!AssetDatabase.IsValidFolder(WindowDataFolder))
                AssetDatabase.CreateFolder("Assets/Resources/StaticData", "UI");

            var data = AssetDatabase.LoadAssetAtPath<WindowStaticData>(WindowDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<WindowStaticData>();
                AssetDatabase.CreateAsset(data, WindowDataPath);
            }

            if (data.Configs == null)
                data.Configs = new List<WindowConfig>();

            var shopTemplate = AssetDatabase.LoadAssetAtPath<GameObject>(ShopPath).GetComponent<ShopWindow>();
            WindowConfig shop = data.Configs.FirstOrDefault(c => c.WindowId == WindowId.Shop);
            if (shop == null)
            {
                shop = new WindowConfig { WindowId = WindowId.Shop };
                data.Configs.Add(shop);
            }
            shop.Template = shopTemplate;

            EditorUtility.SetDirty(data);
        }

        #endregion

        #region Helpers

        private static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Цвет количества товара из префаба курса.
        /// </summary>
        private static readonly Color CountTextColor = new Color(0.829f, 0.809f, 0.684f, 1f);

        /// <summary>
        /// Скорость колеса мыши для ПК (решение 2026-10-30, пока ПК — основная платформа): InputSystemUIInputModule
        /// отдаёт 6 единиц на щелчок (scrollDeltaPerTick), ScrollRect умножает на scrollSensitivity (по умолчанию 1) —
        /// ~130 щелчков на всю ленту. С 50 — 3–4 щелчка. На перетаскивание пальцем/мышью не влияет. В курсе — 1 (мобилка).
        /// </summary>
        private const float WheelScrollSensitivity = 50f;

        /// <summary>
        /// Новый префаб сначала сохраняется пустым из preview-сцены (открытая сцена не помечается изменённой),
        /// дальше и новый, и существующий собираются через LoadPrefabContents.
        /// </summary>
        private static void EditPrefab(string path, System.Action<GameObject> build)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                Scene previewScene = EditorSceneManager.NewPreviewScene();
                try
                {
                    var empty = new GameObject(System.IO.Path.GetFileNameWithoutExtension(path), typeof(RectTransform));
                    empty.layer = LayerMask.NameToLayer("UI");
                    SceneManager.MoveGameObjectToScene(empty, previewScene);
                    PrefabUtility.SaveAsPrefabAsset(empty, path);
                }
                finally
                {
                    EditorSceneManager.ClosePreviewScene(previewScene);
                }
            }

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                build(root);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static GameObject NestedPrefab(Transform parent, string prefabPath, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
                return existing.gameObject;

            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, parent);
            instance.name = name;
            return instance;
        }

        private static (Transform Transform, bool Created) Child(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (child != null)
                return (child, false);

            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent, false);
            return (go.transform, true);
        }

        /// <summary>
        /// Image без Raycast Target: декоративные картинки не участвуют в поиске касания (видео Optimization).
        /// Кнопкам Raycast Target включается отдельно.
        /// </summary>
        private static (Image Image, bool Created) ImageChild(Transform parent, string name)
        {
            (Transform child, bool created) = Child(parent, name);
            Image image = GetOrAdd<Image>(child.gameObject);
            if (created)
                image.raycastTarget = false;
            return (image, created);
        }

        /// <summary>
        /// Текст окна как в курсе: Bold (задаётся каждый запуск — часть вида окна курса), цвет — при создании.
        /// Цвет не белый (Count_text) переводится один раз и для уже созданного текста, если он остался белым по умолчанию.
        /// </summary>
        private static TextMeshProUGUI TextChild(Transform parent, string name, string text, float fontSize, Color? color = null)
        {
            (Transform child, bool created) = Child(parent, name);
            TextMeshProUGUI tmp = GetOrAdd<TextMeshProUGUI>(child.gameObject);
            Stretch(tmp.rectTransform);
            Color textColor = color ?? Color.white;
            if (created)
            {
                tmp.text = text;
                tmp.fontSize = fontSize;
                tmp.color = textColor;
            }
            else if (color.HasValue && tmp.color == Color.white)
            {
                tmp.color = textColor;
            }
            if (tmp.font == null)
                tmp.font = TMP_Settings.defaultFontAsset;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static void Sliced(Image image, string spritePath, bool setSprite)
        {
            if (setSprite)
                image.sprite = LoadSprite(spritePath);
            image.type = Image.Type.Sliced;
        }

        private static void Icon(Image image, string spritePath, bool setSprite)
        {
            if (setSprite)
                image.sprite = LoadSprite(spritePath);
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        private static void MakeButton(GameObject go, Graphic target)
        {
            UnityEngine.UI.Button button = GetOrAdd<UnityEngine.UI.Button>(go);
            button.targetGraphic = target;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T component))
                component = go.AddComponent<T>();
            return component;
        }

        private static RectTransform Rect(Transform transform) =>
            (RectTransform)transform;

        private static void Stretch(RectTransform rect) =>
            SetRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                Debug.LogError($"Shop window: нет спрайта {path}");
            return sprite;
        }

        private static Sprite LoadSheetSprite(string spriteName)
        {
            Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(CasualSheet).OfType<Sprite>().FirstOrDefault(s => s.name == spriteName);
            if (sprite == null)
                Debug.LogError($"Shop window: нет спрайта {spriteName} в {CasualSheet}");
            return sprite;
        }

        #endregion
    }
}
