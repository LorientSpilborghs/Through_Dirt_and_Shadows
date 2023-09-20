using System.Collections;
using System.Linq;
using ResourcesManagerFeature.Runtime;
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
        
        public float DistanceBetweenKnots
        {
            get => _distanceBetweenKnots;
            set => _distanceBetweenKnots = value;
        }

        public int InitialGrowCost
        {
            get => _initialGrowCost;
            set => _initialGrowCost = value;
        }

        public float SpeedPercentage
        {
            get => _speedPercentage;
            set => _speedPercentage = value;
        }

        private void Awake()
        {
            _splineExtrude = GetComponent<SplineExtrude>();
            _collisionForSpeedModifier = GetComponentInChildren<CollisionForSpeedModifier>();
        }

        private void Start()
        {
            _collisionForSpeedModifier.m_onEnterSlowZone += LowerSpeed;
            _collisionForSpeedModifier.m_onExitSlowZone += ResetSpeed;
            _maxDistancePerSeconds = _distancePerSeconds;
        }

        private void OnDestroy()
        {
            _collisionForSpeedModifier.m_onEnterSlowZone -= LowerSpeed;
            _collisionForSpeedModifier.m_onExitSlowZone -= ResetSpeed;        
        }

        public void Grow(RootV2 root, Vector3 positionToGo)
        {
            Vector3 modifiedPositionToGo = new Vector3(positionToGo.x, _heightOfTheRoot, positionToGo.z);
            _normalizedDistancePerSeconds = (_distancePerSeconds * SpeedPercentage) / Vector3.Distance(root.Container.Spline.ToArray()[^1].Position, modifiedPositionToGo);
            _normalizedTargetKnotPosition = 0f;
            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = root.Container.Spline.Knots.ToArray()[root.Container.Spline.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(lastKnot.Position, modifiedPositionToGo, _normalizedTargetKnotPosition);
            
            AddKnotWhileInterpolating(root);
            root.Container.Spline.SetKnot(root.Container.Spline.Knots.Count() - 1, lastKnot);
            _splineExtrude.Rebuild();
            _frontCollider.transform.position = Container.Spline[^1].Position;
            UpdateHeadOfTheRootTransform(positionToGo);
        }

        public void DeleteIfTooClose(RootV2 root)
        {
            if (!(Vector3.Distance(root.Container.Spline.ToArray()[^1].Position,
                    root.Container.Spline.ToArray()[^2].Position) < 0.1)
                || root.Container.Spline.Count() <= 2) return;
            
            root.Container.Spline.Remove(root.Container.Spline.ToArray()[^1]);
            ResourcesManager.Instance.AddResources((root.Container.Spline.Count - 1) 
                * root.Container.Spline.Count / ResourcesManager.Instance.ResourcesCostDivider);
            _splineExtrude.Rebuild();
            _frontCollider.transform.position = Container.Spline[^1].Position;
            _rootHeadPrefab.transform.position = Container.Spline[^1].Position;
        }
        
        private void AddKnotWhileInterpolating(RootV2 root)
        {
            Vector3 pos1 = root.Container.Spline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = root.Container.Spline.Knots.ToArray()[^1].Position;
            if (Vector3.Distance(pos1 , pos2) < DistanceBetweenKnots) return;
            
            root.Container.Spline.Add(new BezierKnot(pos2), TangentMode.AutoSmooth);
            SpeedPercentage -= 1f / _maximumNumberOfKnot;

            foreach (var ivy in _ivyPreset)
            {
                if (Random.Range(ivy._randomInBetweenXY.x, ivy._randomInBetweenXY.y) == ivy._randomInBetweenXY.x)
                {
                    InstantiateIvy(pos2, ivy);
                }
            }
        }
        
        private void UpdateHeadOfTheRootTransform(Vector3 direction)
        {
            _rootHeadPrefab.transform.position = Container.Spline[^1].Position;
            Vector3 newDirection = new Vector3(direction.x, _rootHeadPrefab.transform.position.y, direction.z);
            _rootHeadPrefab.transform.LookAt(newDirection,Vector3.up);
        }

        private void LowerSpeed()
        {
            _isSlow = true;
            StartCoroutine(ChangeSpeed(_distancePerSeconds, _minimumDistancePerSeconds, _timeBeforeReachingMinimumSpeed));
        }

        private void ResetSpeed()
        {
            _isSlow = false;
            StartCoroutine(DelayBeforeSpeedChange());
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
        
        private IEnumerator DelayBeforeSpeedChange()
        {
            yield return new WaitForSeconds(_timeBeforeRecoveringBaseSpeed);
            if (_isSlow) yield break;
            StartCoroutine(ChangeSpeed(_distancePerSeconds, _maxDistancePerSeconds, _timeBeforeReachingMinimumSpeed));
        }

        private void InstantiateIvy(Vector3 pos, Ivy ivyPreset)
        {
            // int knots = _splineContainer.Spline.Count % 10;
            // if(knots % 5 == 0)
            // {
            //     var index = Random.Range(0, _ivyPrefab.Length - 1);
            //     Instantiate(_ivyPrefab[index], pos + new Vector3(0,_heightOfTheIvy,0), Quaternion.identity);
            // }
            
            Instantiate(ivyPreset._ivyPrefab, pos + new Vector3(0,ivyPreset._height,0), Quaternion.identity);
        }

        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private Collider _frontCollider;
        [SerializeField] private GameObject _rootHeadPrefab;
        [Space]
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] private float _minimumDistancePerSeconds = 1f;
        [SerializeField] private float _timeBeforeReachingMinimumSpeed = 0.1f;
        [SerializeField] private float _timeBeforeRecoveringBaseSpeed = 0.5f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        [SerializeField] private float _heightOfTheRoot = 0.5f;
        [SerializeField] private int _maximumNumberOfKnot = 50;
        [Space]
        [SerializeField] private Ivy[] _ivyPreset;
        
        private SplineExtrude _splineExtrude;
        private CollisionForSpeedModifier _collisionForSpeedModifier;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
        private float _maxDistancePerSeconds;
        private bool _isSlow;
        private int _initialGrowCost;
        private float _speedPercentage = 1;
    }
}
