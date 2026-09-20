using System.Collections;
using UnityEngine;

namespace Game.Scripts.Enemy
{
    public class Aggro : MonoBehaviour
    {
        public TriggerObserver TriggerObserver;
        public Follow Follow;

        public float Cooldown;

        private WaitForSeconds _switchFollowOffAfterCooldown;
        private Coroutine _aggroCoroutine;
        private bool _hasAggroTarget;

        private void Start()
        {
            _switchFollowOffAfterCooldown = new WaitForSeconds(Cooldown);

            TriggerObserver.TriggerEnter += TriggerEnter;
            TriggerObserver.TriggerExit += TriggerExit;

            SwitchFollowOff();
        }

        private void OnDestroy()
        {
            TriggerObserver.TriggerEnter -= TriggerEnter;
            TriggerObserver.TriggerExit -= TriggerExit;
        }

        private void TriggerEnter(Collider ignore)
        {
            _hasAggroTarget = true;

            StopAggroCoroutine();
            SwitchFollowOn();
        }

        private void TriggerExit(Collider ignore)
        {
            if (!_hasAggroTarget)
                return;

            _hasAggroTarget = false;
            _aggroCoroutine = StartCoroutine(SwitchFollowOffAfterCooldown());
        }

        private IEnumerator SwitchFollowOffAfterCooldown()
        {
            yield return _switchFollowOffAfterCooldown;

            if (_hasAggroTarget)
                SwitchFollowOn();
            else
                SwitchFollowOff();
        }

        private void StopAggroCoroutine()
        {
            if (_aggroCoroutine == null)
                return;

            StopCoroutine(_aggroCoroutine);
            _aggroCoroutine = null;
        }

        private void SwitchFollowOn() =>
            Follow.enabled = true;

        private void SwitchFollowOff() =>
            Follow.enabled = false;
    }
}
