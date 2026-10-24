using UnityEngine;

namespace Game.Scripts.UI
{
    // Поворачивает объект (полоску HP врага) лицом к камере, как в курсе.
    // UnityEngine.Camera полностью: внутри Game.Scripts имя Camera занято namespace Game.Scripts.Camera.
    public class HpCamera : MonoBehaviour
    {
        private UnityEngine.Camera _mainCamera;

        private void Start() =>
            _mainCamera = UnityEngine.Camera.main;

        private void Update()
        {
            Quaternion rotation = _mainCamera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.up);
        }
    }
}
