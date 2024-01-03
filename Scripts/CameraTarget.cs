using UnityEngine;

namespace BlobFighters.RayMarching
{
    public sealed class CameraTarget : MonoBehaviour
    {
        private Transform _transform;

        public Vector3 Position
        {
            get { return _transform.position; }
        }

        private void Awake()
        {
            _transform = transform;
        }
    }
}