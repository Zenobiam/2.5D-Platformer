using Game.Scripts.UI.Elements;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class PlayerHudBuilder
    {
        private const string HudPrefabPath = "Assets/Resources/Prefabs/Player/HUD.prefab";
        private const string HpBarName = "HpBar";
        private const string MobileName = "Mobile";
        private const string JoystickAreaName = "JoyStickArea";
        private const string AttackButtonName = "AttackButton";
        private const string LootCounterName = "LootCounter";
        private const string LootIconPath = "Assets/Resources/Prefabs/Player/CFX3_T_Skull.png";
        private const string ShopButtonName = "Shop_button";
        private const string ShopButtonSprite = "Assets/ThirdParty/Simple Fantasy GUI/PNG/Login Panel/LoginButton_Hover.png";
        // Та же кнопка, что привязана к действию Attack в PlayerControls (группа Gamepad).
        private const string AttackControlPath = "<Gamepad>/buttonWest";

        [MenuItem("Tools/UI/Build Player HUD")]
        public static void Build()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(HudPrefabPath);
            try
            {
                root.SetActive(true);
                root.transform.localScale = Vector3.one;

                Transform hpBarRoot = FindOrCreateChild(root.transform, HpBarName);
                Image fill = BuildHpBar(hpBarRoot);

                HpBar hpBar = hpBarRoot.GetComponent<HpBar>() ?? hpBarRoot.gameObject.AddComponent<HpBar>();
                hpBar.ImageCurrent = fill;

                ActorUI actorUI = root.GetComponent<ActorUI>() ?? root.AddComponent<ActorUI>();
                actorUI.HpBar = hpBar;

                BuildLootCounter(root.transform);
                BuildShopButton(root.transform);
                BuildMobileControls(root.transform);

                PrefabUtility.SaveAsPrefabAsset(root, HudPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Player HUD built: {HudPrefabPath}");
        }

        private static Image BuildHpBar(Transform hpBarRoot)
        {
            var rect = (RectTransform)hpBarRoot;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(30f, -40f);
            rect.sizeDelta = new Vector2(300f, 40f);

            Sprite barSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            Image background = FindOrCreateImage(hpBarRoot, "Background");
            StretchFull(background.rectTransform);
            background.sprite = barSprite;
            background.color = new Color(0f, 0f, 0f, 0.5f);

            Image fill = FindOrCreateImage(hpBarRoot, "Fill");
            StretchFull(fill.rectTransform);
            fill.sprite = barSprite;
            fill.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 1f;

            return fill;
        }

        /// <summary>
        /// Счётчик лута справа сверху, напротив HpBar: иконка черепа + число (урок 10, как Hud/Loot.prefab курса,
        /// но дочерним объектом HUD, а не вложенным префабом).
        /// Раскладка и перенос задаются при каждом запуске; цвет, размер шрифта и текст — только при создании,
        /// чтобы правки в Inspector не затирались.
        /// </summary>
        private static void BuildLootCounter(Transform root)
        {
            Transform lootRoot = FindOrCreateChild(root, LootCounterName);
            var rect = (RectTransform)lootRoot;
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-30f, -40f);
            rect.sizeDelta = new Vector2(400f, 100f);

            // Ширину детей задаёт группа по их preferred size: длинное число растёт влево и сдвигает иконку,
            // а не переносится на вторую строку.
            if (!lootRoot.TryGetComponent(out HorizontalLayoutGroup layout))
                layout = lootRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            bool iconCreated = lootRoot.Find("Icon") == null;
            Image icon = FindOrCreateImage(lootRoot, "Icon");
            icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(LootIconPath);
            icon.preserveAspect = true;
            if (iconCreated)
                icon.color = new Color(0.83f, 0.43f, 0.71f, 1f);
            if (icon.sprite == null)
                Debug.LogError($"Player HUD: нет спрайта {LootIconPath}");

            if (!icon.TryGetComponent(out LayoutElement iconSize))
                iconSize = icon.gameObject.AddComponent<LayoutElement>();
            iconSize.preferredWidth = 100f;
            iconSize.preferredHeight = 100f;

            Transform counterTransform = FindOrCreateChild(lootRoot, "Counter");
            bool counterCreated = !counterTransform.TryGetComponent(out TextMeshProUGUI counter);
            if (counterCreated)
            {
                counter = counterTransform.gameObject.AddComponent<TextMeshProUGUI>();
                counter.fontSize = 76f;
                counter.color = Color.white;
                counter.text = "0";
            }

            if (counter.font == null)
                counter.font = TMP_Settings.defaultFontAsset;
            counter.alignment = TextAlignmentOptions.MidlineRight;
            counter.textWrappingMode = TextWrappingModes.NoWrap;
            counter.overflowMode = TextOverflowModes.Overflow;

            // Иконка слева от числа: порядок детей задаёт порядок в HorizontalLayoutGroup.
            icon.transform.SetSiblingIndex(0);
            counterTransform.SetSiblingIndex(1);

            if (!lootRoot.TryGetComponent(out LootCounter lootCounter))
                lootCounter = lootRoot.gameObject.AddComponent<LootCounter>();
            lootCounter.Counter = counter;
        }

        /// <summary>
        /// Кнопка Shop (урок 13): Button + OpenWindowButton (WindowId.Shop), правый низ над кнопкой атаки.
        /// Вне группы Mobile — нужна и на ПК. WindowService ей передаёт GameFactory.CreateHud (OpenWindowButton.Init).
        /// Спрайт и текст — только при создании.
        /// </summary>
        private static void BuildShopButton(Transform root)
        {
            bool created = root.Find(ShopButtonName) == null;
            Transform shop = FindOrCreateChild(root, ShopButtonName);
            var rect = (RectTransform)shop;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-187f, 369f);
            rect.sizeDelta = new Vector2(160f, 78f);

            if (!shop.TryGetComponent(out Image image))
                image = shop.gameObject.AddComponent<Image>();
            if (created)
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopButtonSprite);
            image.raycastTarget = true;

            if (!shop.TryGetComponent(out Button button))
                button = shop.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            if (!shop.TryGetComponent(out OpenWindowButton openWindow))
                openWindow = shop.gameObject.AddComponent<OpenWindowButton>();
            openWindow.Button = button;
            openWindow.WindowId = Game.Scripts.UI.Services.Windows.WindowId.Shop;

            Transform textTransform = FindOrCreateChild(shop, "Text (TMP)");
            StretchFull((RectTransform)textTransform);
            bool textCreated = !textTransform.TryGetComponent(out TextMeshProUGUI text);
            if (textCreated)
            {
                text = textTransform.gameObject.AddComponent<TextMeshProUGUI>();
                text.text = "Shop";
                text.fontSize = 36f;
                text.color = Color.white;
            }
            if (text.font == null)
                text.font = TMP_Settings.defaultFontAsset;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
        }

        // Mobile — только экранное управление; общие элементы (HpBar) остаются на корне HUD.
        private static void BuildMobileControls(Transform root)
        {
            Transform mobile = FindOrCreateChild(root, MobileName);
            StretchFull((RectTransform)mobile);
            if (mobile.GetComponent<MobileOnly>() == null)
                mobile.gameObject.AddComponent<MobileOnly>();

            Transform joystickArea = root.Find(JoystickAreaName);
            if (joystickArea != null)
                joystickArea.SetParent(mobile, false);

            Image attackButton = FindOrCreateImage(mobile, AttackButtonName);
            var rect = attackButton.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-80f, 80f);
            rect.sizeDelta = new Vector2(200f, 200f);
            attackButton.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            attackButton.color = new Color(1f, 1f, 1f, 0.5f);

            if (!attackButton.TryGetComponent(out OnScreenButton onScreenButton))
                onScreenButton = attackButton.gameObject.AddComponent<OnScreenButton>();
            onScreenButton.controlPath = AttackControlPath;
        }

        private static Image FindOrCreateImage(Transform parent, string name)
        {
            Transform child = FindOrCreateChild(parent, name);
            return child.GetComponent<Image>() ?? child.gameObject.AddComponent<Image>();
        }

        private static Transform FindOrCreateChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (child != null)
                return child;

            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
