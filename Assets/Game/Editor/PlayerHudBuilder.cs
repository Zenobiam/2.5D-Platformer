using Game.Scripts.UI;
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
