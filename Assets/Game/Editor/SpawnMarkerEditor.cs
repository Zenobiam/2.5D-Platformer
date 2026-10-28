using Game.Scripts.BaseLogic.EnemySpawners;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Красная сфера 0.5 на SpawnMarker в Scene view (в Game view не видна).
    /// </summary>
    [CustomEditor(typeof(SpawnMarker))]
    public class SpawnMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(SpawnMarker spawner, GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawner.transform.position, 0.5f);
        }
    }
}
