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

        public int MinLoot;
        public int MaxLoot;

        /// <summary>
        /// Насколько вперёд от центра врага вынесен центр сферы удара (мировые единицы).
        /// Удар — Physics.OverlapSphere: центр на EffectiveDistance вперёд (и на 0.5 выше), радиус <see cref="Cleavage"/>.
        /// Достаёт на EffectiveDistance + Cleavage от центра врага — это должно быть не меньше
        /// NavMeshAgent.Stopping Distance + радиус капсулы игрока (0.35), иначе враг бьёт мимо.
        /// </summary>
        [Range(0.5f, 1f)]
        public float EffectiveDistance = 0.6f;

        /// <summary>
        /// Радиус сферы удара (мировые единицы). Охват удара — <see cref="EffectiveDistance"/> + Cleavage.
        /// </summary>
        [Range(0.5f, 1f)]
        public float Cleavage;

        public float MoveSpeed;

        public GameObject Prefab;
    }
}