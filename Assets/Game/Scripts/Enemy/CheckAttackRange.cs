using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyAttack))]
    public class CheckAttackRange : MonoBehaviour
    {
        public EnemyAttack EnemyAttack;
        public TriggerObserver TriggerObserver;
        
        private void Start()
        {
            TriggerObserver.TriggerEnter +=  OnTriggerEnter;
            TriggerObserver.TriggerExit +=  OnTriggerExit;
        }

        private void OnTriggerExit(Collider obj)
        {
            EnemyAttack.DisableAttack();
        }

        private void OnTriggerEnter(Collider obj)
        {
            EnemyAttack.EnableAttack();
        }
    }
}