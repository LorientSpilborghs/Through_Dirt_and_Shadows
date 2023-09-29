using System;
using System.Collections;
using System.Linq;
using ResourcesManagerFeature.Runtime;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RootFeature.Runtime
{
    public class RootV2 : MonoBehaviour
    {
        public Action m_onStartGrow;
        public Action<Vector3, bool> m_onGrow;
        public Action m_onEndGrow;
        
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

        public int MinimumNumberOfKnotsForCostReduction
        {
            get => _minimumNumberOfKnotsForCostReduction;
            set => _minimumNumberOfKnotsForCostReduction = value;
        }

        public int SupplementalCostForNewRoot
        {
            get => _supplementalCostForNewRoot;
            set => _supplementalCostForNewRoot = value;
        }

        public int CostReduction
        {
            get => _costReduction;
            set => _costReduction = value;
        }

        public bool IsGrowing
        {
            get => _isGrowing;
            set => _isGrowing = value;
        }

        public CanvasGroup RootWarningUI
        {
            get => _rootWarningUI;
            set => _rootWarningUI = value;
        }

        public CanvasGroup EnvironmentWarningUI
        {
            get => _environmentWarningUI;
            set => _environmentWarningUI = value;
        }

        private void Awake()
        {
            _splineExtrude = GetComponent<SplineExtrude>();
            _collisionForSpeedModifier = GetComponentInChildren<CollisionForSpeedModifier>();
            RootWarningUI = GetComponentsInChildren<CanvasGroup>()[0];
            EnvironmentWarningUI = GetComponentsInChildren<CanvasGroup>()[1];
        }

        private void Start()
        {
            _collisionForSpeedModifier.m_onEnterSlowZone += LowerSpeed;
            _collisionForSpeedModifier.m_onExitSlowZone += ResetSpeed;
            _maxDistancePerSeconds = _distancePerSeconds;
        }

        private void Update()
        {
            MoveTheHeadInTheGround();
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
            m_onGrow?.Invoke(positionToGo, IsGrowing);
            UpdateHeadOfTheRootTransform(positionToGo);
        }

        public void StartGrowing()
        {
            m_onStartGrow?.Invoke();
        }
        
        public void EndGrowing()
        {
            m_onEndGrow?.Invoke();
        }

        public void DeleteIfTooClose(RootV2 root)
        {
            if (!(Vector3.Distance(root.Container.Spline.ToArray()[^1].Position,
                    root.Container.Spline.ToArray()[^2].Position) < _minimumDistanceBetweenKnots)
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
            if (root.Container.Spline.Count >= _numberOfKnotAtWhichSlowStart)
            {
                SpeedPercentage -= (1f / (_maximumNumberOfKnot - _numberOfKnotAtWhichSlowStart)) * _speedPercentageReducerMultiplier;
                if (SpeedPercentage > 0) return;
                _baseRootHeadRotation = _rootHeadPrefab.transform.rotation;
                _isStopped = true;
            }

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
        
        private void MoveTheHeadInTheGround()
        {
            if (!_isStopped) return;

            _resetPosDelta += Time.deltaTime / _timeToReachGround;

            _rootHeadPrefab.transform.rotation = Quaternion.Lerp(_baseRootHeadRotation,_groundedRootHeadRotation, _resetPosDelta);
            Debug.Log("a");
            if (_resetPosDelta >= 1) _isStopped = false;
        }

        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private Collider _frontCollider;
        [SerializeField] private GameObject _rootHeadPrefab;
        [Space]
        [SerializeField] private int _maximumNumberOfKnot = 50;
        [SerializeField] private int _supplementalCostForNewRoot;
        [SerializeField] private int _minimumNumberOfKnotsForCostReduction;
        [SerializeField] private int _costReduction;
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] private int _numberOfKnotAtWhichSlowStart;
        [SerializeField] private float _speedPercentageReducerMultiplier = 1;
        [Space]
        [Header("ZoneSlow")]
        [SerializeField] private float _minimumDistancePerSeconds = 1f;
        [SerializeField] private float _timeBeforeReachingMinimumSpeed = 0.1f;
        [SerializeField] private float _timeBeforeRecoveringBaseSpeed = 0.5f;
        [Space]
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        [SerializeField] private float _minimumDistanceBetweenKnots = 0.1f;
        [SerializeField] private float _heightOfTheRoot = 0.5f;
        [SerializeField] private Quaternion _groundedRootHeadRotation;
        [SerializeField] private float _timeToReachGround = 1;
        [Space]
        [SerializeField] private Ivy[] _ivyPreset;
        
        private SplineExtrude _splineExtrude;
        private CollisionForSpeedModifier _collisionForSpeedModifier;
        private Quaternion _baseRootHeadRotation;
        private CanvasGroup _rootWarningUI;
        private CanvasGroup _environmentWarningUI;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
        private float _maxDistancePerSeconds;
        private bool _isSlow;
        private bool _isGrowing;
        private bool _isStopped;
        private int _initialGrowCost;
        private float _speedPercentage = 1;
        private float _resetPosDelta;
    }
}
