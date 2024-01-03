using UnityEngine;

namespace BlobFighters.RayMarching
{
    public sealed class BasicCamera : MonoBehaviour
    {
        [SerializeField]
        private float verticalSpeed;

        private CameraTarget _cameraTarget;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _cameraTarget = FindObjectOfType<CameraTarget>(); // i hate this one btw
        }

        private void FixedUpdate()
        {
            Vector3 p = transform.position;
            Vector3 target = _cameraTarget.Position;

            p.x = _cameraTarget.Position.x;
            p.y = Mathf.MoveTowards(p.y, target.y, verticalSpeed * Time.deltaTime);
            _transform.position = p;
        }

        private void LateUpdate()
        {
        }
    }
}