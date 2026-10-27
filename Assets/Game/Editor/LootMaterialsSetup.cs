using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Editor
{
    /// <summary>
    /// Материалы лута из курса (урок 10) написаны под built-in: частицы Cartoon FX на legacy-шейдерах
    /// Particles/Additive и Particles/Additive (Soft), череп — Mobile/Diffuse. В URP они розовые,
    /// а штатный конвертер Render Pipeline Converter legacy-частицы не переводит.
    /// Скрипт ставит URP-шейдеры и переносит текстуру и цвет. Повторный запуск безопасен.
    /// </summary>
    public static class LootMaterialsSetup
    {
        private const string ParticlesShader = "Universal Render Pipeline/Particles/Unlit";
        private const string LitShader = "Universal Render Pipeline/Simple Lit";

        private const string CfxMaterials = "Assets/Resources/Effects/JMO Assets/Cartoon FX/Materials/";

        /// <summary>
        /// Particles/Additive: цвет = 2 * _TintColor * текстура — поэтому tint удваивается.
        /// </summary>
        private static readonly string[] AdditiveMaterials =
        {
            CfxMaterials + "Light/CFX3_AuraRunic ADD.mat",
        };

        /// <summary>
        /// Particles/Additive (Soft): без tint, цвет даёт только текстура и цвет частицы.
        /// </summary>
        private static readonly string[] AdditiveSoftMaterials =
        {
            CfxMaterials + "Light/CFX3_CircleSoft ADDSOFT.mat",
            CfxMaterials + "Light/CFX3_SmallRay ADDSOFT.mat",
            CfxMaterials + "Symbols/CFX3_Skull ADDSOFT.mat",
        };

        private const string SkullMaterial = "Assets/Resources/Loot/Skull/Materials/cemetery_halloween_atlas.mat";

        [MenuItem("Tools/Setup/Loot Materials to URP")]
        public static void Setup()
        {
            Shader particles = Shader.Find(ParticlesShader);
            Shader lit = Shader.Find(LitShader);
            if (particles == null || lit == null)
            {
                Debug.LogError("Loot materials: не найдены URP-шейдеры, проверьте пакет URP.");
                return;
            }

            foreach (string path in AdditiveMaterials)
                ConvertParticle(path, particles, doubleTint: true);

            foreach (string path in AdditiveSoftMaterials)
                ConvertParticle(path, particles, doubleTint: false);

            ConvertLit(SkullMaterial, lit);

            AssetDatabase.SaveAssets();
            Debug.Log("Loot materials: готово.");
        }

        private static void ConvertParticle(string path, Shader shader, bool doubleTint)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Debug.LogError($"Loot materials: нет материала {path}");
                return;
            }

            // Свойства читаются до смены шейдера: после неё _TintColor у материала недоступен.
            Texture texture = material.GetTexture("_MainTex");
            Color color = doubleTint && material.HasProperty("_TintColor")
                ? material.GetColor("_TintColor") * 2f
                : Color.white;

            material.shader = shader;
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", color);

            // Transparent + Additive — так же, как выставляет Inspector URP Particles.
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 2f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.One);
            material.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            material.SetFloat("_DstBlendAlpha", (float)BlendMode.One);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;

            EditorUtility.SetDirty(material);
            Debug.Log($"Loot materials: {material.name} → {shader.name}");
        }

        private static void ConvertLit(string path, Shader shader)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Debug.LogError($"Loot materials: нет материала {path}");
                return;
            }

            Texture texture = material.GetTexture("_MainTex");

            // Mobile/Diffuse не использует _Color, поэтому цвет — белый, как было в built-in.
            material.shader = shader;
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);

            EditorUtility.SetDirty(material);
            Debug.Log($"Loot materials: {material.name} → {shader.name}");
        }
    }
}
