using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float _lookAhead;

    private float _facing;

    private void Update()
    {
        transform.position = new Vector3(player.position.x + _lookAhead, transform.position.y, transform.position.z);

        // TODO если будет баговать, привязать к facing из PlayerVisual скрипта
        _facing = player.rotation.eulerAngles.y > 90 ? -1 : 1;

        Debug.Log($"facing: {_facing}");

        _lookAhead = Mathf.Lerp(_lookAhead, (aheadDistance * _facing), Time.deltaTime * cameraSpeed);
    }
}
