using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace RootFeature.Runtime
{
    public class RootV2 : MonoBehaviour
    {
        public SplineContainer Container
        {
            get => _splineContainer;
            set => _splineContainer = value;
        }

        public float DistancePerSeconds
        {
            get => _distancePerSeconds;
            set => _distancePerSeconds = value;
        }

        public float DistanceBetweenKnots
        {
            get => _distanceBetweenKnots;
            set => _distanceBetweenKnots = value;
        }

        private void Awake()
        {
            _splineExtrude = GetComponent<SplineExtrude>();
            _collisionForSpeedModifier = GetComponentInChildren<CollisionForSpeedModifier>();
        }

        private void Start()
        {
            _maxDistancePerSeconds = _distancePerSeconds;
            _collisionForSpeedModifier.m_onEnterSlowZone += LowerSpeed;
            _collisionForSpeedModifier.m_onExitSlowZone += ResetSpeed;
        }

        public void Grow(RootV2 root, Vector3 positionToGo)
        {
            Vector3 modifiedPositionToGo = new Vector3(positionToGo.x, _heightOfTheRoot, positionToGo.z);
            _normalizedDistancePerSeconds = DistancePerSeconds / Vector3.Distance(root.Container.Spline.ToArray()[^1].Position, modifiedPositionToGo);
            _normalizedTargetKnotPosition = 0f;
            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = root.Container.Spline.Knots.ToArray()[root.Container.Spline.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(lastKnot.Position, modifiedPositionToGo, _normalizedTargetKnotPosition);
            
            AddKnotWhileInterpolating(root);
            root.Container.Spline.SetKnot(root.Container.Spline.Knots.Count() - 1, lastKnot);
            _splineExtrude.Rebuild();
            UpdateFrontColliderPosition();
        }

        public void DeleteIfTooClose(RootV2 root)
        {
            if (Vector3.Distance(root.Container.Spline.ToArray()[^1].Position, root.Container.Spline.ToArray()[^2].Position) < 1 
                && root.Container.Spline.Count() > 2)
            {
                root.Container.Spline.Remove(root.Container.Spline.ToArray()[^1]);
            }
        }
        
        private void AddKnotWhileInterpolating(RootV2 root)
        {
            Vector3 pos1 = root.Container.Spline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = root.Container.Spline.Knots.ToArray()[^1].Position;
            if (Vector3.Distance(pos1 , pos2) < DistanceBetweenKnots) return;
            
            root.Container.Spline.Add(new BezierKnot(pos2), TangentMode.AutoSmooth);
        }

        private void UpdateFrontColliderPosition()
        {
            _frontCollider.transform.position = Container.Spline[^1].Position;
        }

        private void LowerSpeed()
        {
            print("slow");
            StartCoroutine(ChangeSpeed(_distancePerSeconds, _minimumDistancePerSeconds, 0.1f));
        }

        private void ResetSpeed()
        {
            StartCoroutine(ChangeSpeed(_distancePerSeconds, _maxDistancePerSeconds, 0.1f));
        }

        private IEnumerator ChangeSpeed(float v_start, float v_end, float duration)
        {
            {
                float elapsed = 0.0f;
                while (elapsed < duration )
                {
                    _distancePerSeconds = Mathf.Lerp( v_start, v_end, elapsed / duration );
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                _distancePerSeconds = v_end;
            }
        }

        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private Collider _frontCollider;
        [Space]
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] private float _minimumDistancePerSeconds = 1f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        [SerializeField] private float _heightOfTheRoot = 0.5f;
        
        private SplineExtrude _splineExtrude;
        private CollisionForSpeedModifier _collisionForSpeedModifier;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
        private float _maxDistancePerSeconds;
    }
}
