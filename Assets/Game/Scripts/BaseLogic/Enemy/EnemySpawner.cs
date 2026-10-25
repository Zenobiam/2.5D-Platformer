using System;
using Game.Scripts.Data;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.BaseLogic.Enemy
{
    public class EnemySpawner : MonoBehaviour, ISaveProgress
    {
        public MonsterTypeId MonsterTypeId;
        private string _id;

        public bool isEmpty;

        private void Awake()
        {
            _id = GetComponent<UniqueId>().Id;
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if(progress.KillData.ClearedSpawners.Contains(_id))
                isEmpty = true;
            else
                Spawn();
        }

        private void Spawn()
        {
            
        }

        public void UpdateProgress(PlayerProgress progress)
        {
           if(isEmpty)
               progress.KillData.ClearedSpawners.Add(_id);
        }
    }
}