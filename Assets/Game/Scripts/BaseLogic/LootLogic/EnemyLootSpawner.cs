using Game.Scripts.BaseLogic.Enemy;
using Game.Scripts.Data;
using Game.Scripts.Enemy;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Services.Randomizer.Interfaces;
using UnityEngine;

namespace Game.Scripts.BaseLogic.LootLogic
{
    public class EnemyLootSpawner : MonoBehaviour
    {
        public EnemyDeath EnemyDeath;
        private IGameFactory _factory;
        private IRandomService _random;
        private int _lootMin;
        private int _lootMax;

        public void Construct(IGameFactory factory, IRandomService random)
        {
            _factory = factory;
            _random = random;
        }

        private void Start()
        {
            EnemyDeath.Died += OnDeathSpawnLoot;
        }

        private void OnDeathSpawnLoot()
        {
            LootPiece loot = _factory.CreateLoot();
            loot.transform.position = transform.position;
            loot.GetComponent<UniqueId>().GenerateId();
            var lootItem = GenerateLoot();
            
            loot.Initialize(lootItem);
        }

        private Loot GenerateLoot()
        {
            return new Loot()
            {
                Value = _random.Next(_lootMin, _lootMax),
            };
        }

        public void SetLoot(int min, int max)
        {
            _lootMin = min;
            _lootMax = max;
        }
    }
}