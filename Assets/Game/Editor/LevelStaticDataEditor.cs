using System.Linq;
using Game.Scripts.BaseLogic;
using Game.Scripts.BaseLogic.EnemySpawners;
using Game.Scripts.StaticData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    /// <summary>
    /// Кнопка Collect в Inspector LevelStaticData: собирает SpawnMarker открытой сцены (id из UniqueId, тип, позиция)
    /// и записывает имя сцены в LevelKey. Сцена уровня должна быть открыта.
    /// </summary>
    [CustomEditor(typeof(LevelStaticData))]
    public class LevelStaticDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var levelData = (LevelStaticData)target;

            if (GUILayout.Button("Collect"))
            {
                // FindObjectsByType вместо FindObjectsOfType курса: в Unity 6 старый метод устарел (course-deviations.md).
                levelData.EnemySpawners = FindObjectsByType<SpawnMarker>(FindObjectsSortMode.None)
                    .Select(x => new EnemySpawnerStaticData(x.GetComponent<UniqueId>().Id, x.EnemyTypeId, x.transform.position))
                    .ToList();

                levelData.LevelKey = SceneManager.GetActiveScene().name;
            }

            EditorUtility.SetDirty(target);
        }
    }
}
