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
    /// Урок 15: позиция игрока — объект с тегом InitialPoint, позиция перехода — LevelTransferInitialPoint
    /// (без этих объектов на сцене Collect падает с NullReferenceException, как в курсе). TransferTo вписывается руками.
    /// </summary>
    [CustomEditor(typeof(LevelStaticData))]
    public class LevelStaticDataEditor : UnityEditor.Editor
    {
        private const string InitialPointTag = "InitialPoint";
        private const string LevelTransferInitialPointTag = "LevelTransferInitialPoint";

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

                levelData.InitialPlayerPosition = GameObject.FindWithTag(InitialPointTag).transform.position;

                levelData.LevelTransfer.Position = GameObject.FindWithTag(LevelTransferInitialPointTag).transform.position;
            }

            EditorUtility.SetDirty(target);
        }
    }
}
