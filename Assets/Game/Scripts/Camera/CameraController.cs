using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _followTarget;
        [SerializeField] private float _aheadDistance;
        [SerializeField] private float _cameraSpeed;
        private float _lookAhead;

        private PlayerFacing _playerFacing;

        private void LateUpdate()
        {
            if (!_followTarget) return;

            transform.position = new Vector3(_followTarget.position.x + _lookAhead, transform.position.y, transform.position.z);

            // Корень игрока не поворачивается (поворачивается модель), поэтому направление берём из PlayerFacing.
            int facing = _playerFacing != null ? _playerFacing.Direction : 1;

            _lookAhead = Mathf.Lerp(_lookAhead, (_aheadDistance * facing), Time.deltaTime * _cameraSpeed);
        }

        public void Follow(GameObject follow)
        {
            _followTarget = follow.transform;
            _playerFacing = follow.GetComponent<PlayerFacing>();
        }
    }
}
