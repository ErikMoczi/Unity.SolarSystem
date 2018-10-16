using System;
using ECS.Utils;
using UnityEngine;

namespace Mono
{
    public sealed class CameraController : MonoBehaviour
    {
        private enum Mode
        {
            isIdle,
            isRotating,
            isZooming
        }

        [SerializeField] private float _xSpeed = 200f;
        [SerializeField] private float _ySpeed = 200f;
        [SerializeField] private float _yMinLimit = -80f;
        [SerializeField] private float _yMaxLimit = 80f;
        [SerializeField] private float _zoomRate = 40f;
        [SerializeField] private float _rotationDampening = 5f;

        private Transform _targetRotation;
        private float _xDeg;
        private float _yDeg;
        private Vector3 _desiredPosition;
        private Vector3 _camPlanePoint;
        private Vector3 _vectorPoint;
        private Ray _ray;
        private Mode _mode = Mode.isIdle;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _targetRotation = new GameObject("Cam targetRotation").transform;

            _xDeg = Vector3.Angle(Vector3.right, transform.right);
            _yDeg = Vector3.Angle(Vector3.up, transform.up);

            LinePlaneIntersect(transform.forward.normalized, transform.position, Vector3.up, Vector2.zero,
                ref _camPlanePoint);

            _targetRotation.position = _camPlanePoint;
            _targetRotation.rotation = transform.rotation;
        }

        private void LateUpdate()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var wheel = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetMouseButton(1))
            {
                _mode = Mode.isRotating;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _yDeg = transform.rotation.eulerAngles.x;
                if (_yDeg > 180)
                {
                    _yDeg -= 360;
                }

                _xDeg = transform.rotation.eulerAngles.y;

                _mode = Mode.isIdle;
            }
            else if (Math.Abs(wheel) > 0.000f)
            {
                _mode = Mode.isZooming;
            }

            switch (_mode)
            {
                case Mode.isIdle:
                {
                    break;
                }
                case Mode.isRotating:
                {
                    _xDeg += Input.GetAxis("Mouse X") * _xSpeed;
                    _yDeg -= Input.GetAxis("Mouse Y") * _ySpeed;

                    _yDeg = ClampAngle(_yDeg, _yMinLimit, _yMaxLimit, 5);

                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.Euler(_yDeg, _xDeg, 0),
                        Time.deltaTime * _rotationDampening / Time.timeScale
                    );
                    _targetRotation.rotation = transform.rotation;

                    var magnitude = (_targetRotation.position - transform.position).magnitude;
                    transform.position = _targetRotation.position - transform.rotation * Vector3.forward * magnitude;
                    _targetRotation.position = _targetRotation.position;

                    break;
                }
                case Mode.isZooming:
                {
                    var s0 = LinePlaneIntersect(transform.forward, transform.position, Vector3.up, Vector2.zero,
                        ref _camPlanePoint);
                    _targetRotation.position = transform.forward * s0 + transform.position;
                    var lineToPlaneLength = LinePlaneIntersect(_ray.direction, transform.position, Vector3.up,
                        Vector2.zero, ref _vectorPoint);

                    if (wheel > 0)
                    {
                        if (lineToPlaneLength > 1.1f)
                        {
                            _desiredPosition = (_vectorPoint - transform.position) / 2 + transform.position;
                        }
                    }
                    else if (wheel < 0)
                    {
                        _desiredPosition = -(_targetRotation.position - transform.position) / 2 + transform.position;
                    }

                    transform.position = Vector3.Lerp(
                        transform.position,
                        _desiredPosition,
                        _zoomRate * Time.deltaTime / Time.timeScale
                    );

                    if (transform.position == _desiredPosition)
                    {
                        _mode = Mode.isIdle;
                    }

                    break;
                }
                default:
                {
                    break;
                }
            }

            transform.position = Vector3.ClampMagnitude(transform.position, Scales.SolarSystemEdge);
        }

        private float LinePlaneIntersect(Vector3 u, Vector3 P0, Vector3 N, Vector3 D, ref Vector3 point)
        {
            var s = Vector3.Dot(N, (D - P0)) / Vector3.Dot(N, u);
            point = P0 + s * u;
            return s;
        }

        private static float ClampAngle(float angle, float minOuter, float maxOuter, float inner)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            angle = Mathf.Clamp(angle, minOuter, maxOuter);
            if (angle < inner && angle > 0)
            {
                angle -= 2 * inner;
            }
            else if (angle > -inner && angle < 0)
            {
                angle += 2 * inner;
            }

            return angle;
        }
    }
}