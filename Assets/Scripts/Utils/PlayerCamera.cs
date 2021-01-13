using UnityEngine;
using Zenject;

namespace Defong
{
    public sealed class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteBounds;
        [SerializeField] private Camera _cam = default;
        [SerializeField] private float speed = 3;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private float directionOffset;
        [SerializeField] private float maxOrthographicSize = 5f;

        private float leftBound;
        private float rightBound;
        private float bottomBound;
        private float topBound;
        private Vector3 targetPosition;

        private void OnValidate()
        {
            _cam = GetComponent<Camera>();
        }

        private void Awake()
        {
            ProjectContext.Instance.Container.BindInstances(this);
        }

        private void Update()
        {
            if (_spriteBounds == null)
            {
                return;
            }

            UpdatePosition();
        }

        public void SetTarget(SpriteRenderer target)
        {
            _spriteBounds = target;

            CalculateZoom();
            CalculateBorders();
            UpdatePosition(true);
        }

        private void CalculateZoom()
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = _spriteBounds.bounds.size.x / _spriteBounds.bounds.size.y;

            if (screenRatio < targetRatio)
            {
                _cam.orthographicSize = Mathf.Clamp(_spriteBounds.bounds.size.y / 2, 0, maxOrthographicSize);
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                _cam.orthographicSize = Mathf.Clamp(_spriteBounds.bounds.size.y / 2 * differenceInSize, 0, maxOrthographicSize);
            }
        }

        private void UpdatePosition(bool immediate = false)
        {
            targetPosition = _spriteBounds.transform.position;

            if (transform.position == targetPosition) return;

            targetPosition = CalculatePosition(targetPosition + offset + _spriteBounds.transform.up * directionOffset);
            transform.position = CalculatePosition(immediate ? targetPosition : Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime));
        }

        private void CalculateBorders()
        {
            Bounds bounds = _spriteBounds.sprite.bounds;
            float camVertExtent = _cam.orthographicSize;
            float camHorzExtent = _cam.aspect * camVertExtent;

            leftBound = bounds.min.x * _spriteBounds.transform.lossyScale.x + camHorzExtent + _spriteBounds.transform.position.x;
            rightBound = bounds.max.x * _spriteBounds.transform.lossyScale.x - camHorzExtent + _spriteBounds.transform.position.x;
            bottomBound = bounds.min.y * _spriteBounds.transform.lossyScale.y + camVertExtent + _spriteBounds.transform.position.y;
            topBound = bounds.max.y * _spriteBounds.transform.lossyScale.y - camVertExtent + _spriteBounds.transform.position.y;

            targetPosition = CalculatePosition(targetPosition);
        }

        private Vector3 CalculatePosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, leftBound, rightBound);
            position.y = Mathf.Clamp(position.y, bottomBound, topBound);
            position.z = -10f;

            return position;
        }
    }
}
