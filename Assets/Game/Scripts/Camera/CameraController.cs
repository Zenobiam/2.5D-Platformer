using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private float _aheadDistance;
    [SerializeField] private float _cameraSpeed;
    private float _lookAhead;

    private float _facing;

    private void LateUpdate()
    {
        if (!_followTarget) return;
        
        transform.position = new Vector3(_followTarget.position.x + _lookAhead, transform.position.y, transform.position.z);

        _facing = _followTarget.rotation.eulerAngles.y > 90 ? -1 : 1;

        _lookAhead = Mathf.Lerp(_lookAhead, (_aheadDistance * _facing), Time.deltaTime * _cameraSpeed);
    }

    public void Follow(GameObject follow) => _followTarget = follow.transform;
}
