using System;
using System.Collections.Generic;
using System.Linq;
using InputManagerFeature.Runtime;
using ResourcesManagerFeature.Runtime;
using RootFeature.Runtime;
using UnityEngine;
using UnityEngine.Splines;

namespace PlayerRuntime
{
    public class PlayerV2 : MonoBehaviour
    {
        #region Public Members
        
        public static PlayerV2 Instance { get; private set; }

        public Action<Vector3> m_onCameraBlendingStart;
        public Action<Vector3> m_onInterpolate;
        public Action m_onResetCameraPos;
        public Action m_onCameraBlendingStop;
        public Action m_onNewKnotInstantiate;
        public Action<bool> m_onNewKnotSelected;
        public Func<bool> m_isCameraBlendingOver;
        public Func<bool> m_isInThirdPerson;
        
        public Vector3 PointerPosition
        {
            get => _pointerPosition;
            set => _pointerPosition = value;
        }

        public BezierKnot CurrentClosestKnot
        {
            get => _currentClosestKnot;
            set => _currentClosestKnot = value;
        }

        public bool IsInterpolating
        {
            get => _isInterpolating;
            set => _isInterpolating = value;
        }

        public RootV2 RootToModify
        {
            get => _rootToModify;
            set => _rootToModify = value;
        }

        public Spline CurrentClosestSpline
        {
            get => _currentClosestSpline;
            set => _currentClosestSpline = value;
        }

        public RootV2 CurrentClosestRoot
        {
            get => _currentClosestRoot;
            set => _currentClosestRoot = value;
        }

        #endregion
        
        #region Unity API

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            InputManager.Instance.m_onMouseMove += OnMouseMoveEventHandler;
            InputManager.Instance.m_onLeftMouseDown += OnLeftMouseDownEventHandler;
            InputManager.Instance.m_onRightMouseDown += OnRightMouseDownEventHandler;
            InputManager.Instance.m_onMouseHold += OnMouseHoldEventHandler;
            InputManager.Instance.m_onMouseUp += OnMouseUpEventHandler;
            InputManager.Instance.m_onSpaceBarDown += OnSpaceBarDownEventHandler;
            
            AddNewRoot(Vector3.zero + Vector3.up * _heightOfTheRootAtStart);
        }

        private void OnDestroy()
        {
            InputManager.Instance.m_onMouseMove -= OnMouseMoveEventHandler;
            InputManager.Instance.m_onLeftMouseDown -= OnLeftMouseDownEventHandler;
            InputManager.Instance.m_onRightMouseDown -= OnRightMouseDownEventHandler;
            InputManager.Instance.m_onMouseHold -= OnMouseHoldEventHandler;
            InputManager.Instance.m_onMouseUp -= OnMouseUpEventHandler;
            InputManager.Instance.m_onSpaceBarDown -= OnSpaceBarDownEventHandler;
        }
        
        #endregion

        #region My Methods
        
        private void OnMouseMoveEventHandler(Vector3 pos)
        {
            if (m_isCameraBlendingOver?.Invoke() is false) return;
            PointerPosition = pos;
            GetTheRightRoot(true);
        }
        
        private void OnLeftMouseDownEventHandler()
        {
            if (m_isCameraBlendingOver?.Invoke() is false) return;
            m_onCameraBlendingStart?.Invoke(CurrentClosestKnot.Position);
        }

        private void OnRightMouseDownEventHandler()
        {
            m_onCameraBlendingStop?.Invoke();
        }
        
        private void OnMouseHoldEventHandler()
        {
            if (m_isInThirdPerson?.Invoke()is false) return;
            if (!_isInterpolating)
            {
                RootToModify = GetTheRightRoot() ?? RootToModify;
            }
            if (_frontColliderBehaviour.IsBlocked) return;
            if (!UseResourcesWhileGrowing(((RootToModify.Container.Spline.Count - 1) 
                                           * RootToModify.Container.Spline.Count) 
                                          / ResourcesManager.Instance.ResourcesCostDivider)) return;
            RootToModify.Grow(RootToModify, PointerPosition);
            m_onInterpolate?.Invoke((Vector3)RootToModify.Container.Spline[^1].Position);
            IsInterpolating = true;
            if (IsMaxDistanceBetweenKnots()) m_onNewKnotInstantiate?.Invoke();
        }
        
        private void OnMouseUpEventHandler()
        {
            IsInterpolating = false;
            RootToModify?.DeleteIfTooClose(RootToModify);
        }
        
        private void OnSpaceBarDownEventHandler()
        {
            m_onResetCameraPos?.Invoke();
        }
        
        private RootV2 GetTheRightRoot(bool onlySetKnot = false)
        {
            RootV2 root = _rootsList[0];
            BezierKnot closestKnot = root.Container.Spline.ToArray()[^1];
        
            for (int i = 0; i < _rootsList.Count; i++)
            {
                for (int j = 0; j < _rootsList[i].Container.Spline.Count(); j++)
                {
                    float dist1 = Vector3.Distance(_rootsList[i].Container.Spline.ToArray()[j].Position, PointerPosition);
                    float dist2 = Vector3.Distance(closestKnot.Position, PointerPosition);
                    if (dist1 > dist2) continue;
        
                    closestKnot = _rootsList[i].Container.Spline.ToArray()[j];
                    root = _rootsList[i];
                }
            }
            CurrentClosestKnot = closestKnot;
            CurrentClosestSpline = root.Container.Spline;
            
            m_onNewKnotSelected?.Invoke(IsLastKnotFromSpline(closestKnot, root));
            
            if (onlySetKnot) return null;

            if (IsLastKnotFromSpline(closestKnot, root))
            {
                return root;
            }
            else
            {
                return UseResourcesWhileGrowing(RootToModify.Container.Spline.Count * ResourcesManager.Instance.ResourcesCostDivider) 
                    ? AddNewRoot((Vector3)CurrentClosestKnot.Position) : null;
            }
        }
        
        private RootV2 AddNewRoot(Vector3 position)
        {
            RootV2 newRoot = Instantiate(_rootPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<RootV2>();
            Mesh mesh = new Mesh();
            mesh.isReadable.Equals(true);
            newRoot.GetComponent<MeshFilter>().mesh = mesh;
            
            newRoot.Container.Spline.Add(new BezierKnot(position), TangentMode.AutoSmooth);
            newRoot.Container.Spline.Add(new BezierKnot(position), TangentMode.AutoSmooth);
            
            _rootsList.Add(newRoot);
            return newRoot;
        }

        private bool UseResourcesWhileGrowing(int resourcesUsage)
        {
            return !(IsMaxDistanceBetweenKnots()) 
                   || ResourcesManager.Instance.UseResources(resourcesUsage);
        }
        
        #endregion

        
        #region Utils
        
        private static bool IsLastKnotFromSpline(BezierKnot knot, RootV2 root)
        {
            return root.Container.Spline.ToArray()[^1].Equals(knot);
        }

        private bool IsMaxDistanceBetweenKnots()
        {
            Vector3 pos1 = RootToModify.Container.Spline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = RootToModify.Container.Spline.Knots.ToArray()[^1].Position;

            return Vector3.Distance(pos1, pos2) > RootToModify.DistanceBetweenKnots;
        }
        
        #endregion
        
        
        #region Private and Protected Members
        
        [SerializeField] private GameObject _rootPrefab;
        [SerializeField] private FrontColliderBehaviour _frontColliderBehaviour;
        [SerializeField] private float _heightOfTheRootAtStart = 0.5f;
        [Space]
        private List<RootV2> _rootsList = new();
        private Vector3 _pointerPosition;
        private RootV2 _rootToModify;
        private RootV2 _currentClosestRoot;
        private BezierKnot _currentClosestKnot;
        private Spline _currentClosestSpline;
        private bool _isInterpolating;

        #endregion
    }
}
