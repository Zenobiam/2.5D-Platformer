using Game.Scripts.Infrastructure;
using Game.Scripts.Services.SaveLoad.Interfaces;
using UnityEngine;

namespace Game.Scripts.Triggers
{
    public class SaveTrigger : MonoBehaviour // TODO now is for demo
    {
        private ISaveLoadService _saveLoadService;
        
        public BoxCollider Collider;

        private void Awake()
        {
            _saveLoadService = ServiceLocator.Container.Single<ISaveLoadService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _saveLoadService.SaveProgress();
            Debug.Log("Progress saved ");
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (!Collider) return;
            
            Gizmos.color = new Color32(30, 200, 30, 130);
            Gizmos.DrawCube(transform.position + Collider.center, Collider.size);
        }
    }
}