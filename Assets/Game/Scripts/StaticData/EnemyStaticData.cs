using UnityEngine;

namespace Game.Scripts.StaticData
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "StaticData/Enemy")]
    public class EnemyStaticData  : ScriptableObject
    {
        public EnemyTypeId EnemyTypeId;
        
        [Range(0, 100)]
        public int Hp;
        
        [Range(1f, 30f)]
        public float Damage;

        // Удар врага — сфера Physics.OverlapSphere: центр на EffectiveDistance вперёд от центра врага (и на 0.5 выше),
        // радиус Cleavage. Достаёт на EffectiveDistance + Cleavage от центра врага — это должно быть не меньше
        // NavMeshAgent.Stopping Distance + радиус капсулы игрока (0.35), иначе враг бьёт мимо.

        // Насколько вперёд от центра врага вынесен центр сферы удара (мировые единицы).
        [Range(0.5f, 1f)]
        public float EffectiveDistance = 0.6f;

        // Радиус сферы удара (мировые единицы).
        [Range(0.5f, 1f)]
        public float Cleavage;

        public float MoveSpeed;

        public GameObject Prefab;
    }
}