using System;
using System.Collections.Generic;
using System.Linq;
using InputManagerFeature.Runtime;
using RootFeature.Runtime;
using UnityEngine;
using UnityEngine.Splines;

namespace PlayerRuntime
{
    public class PlayerV2 : MonoBehaviour
    {
        #region Public Members
        
        public static PlayerV2 Instance { get; private set; }

        public Action m_onInterpolateStart;
        public Action<Vector3> m_onInterpolate;
        public Action m_onInterpolateEnd;
        public Action m_onResetCameraPos;
        public Action m_onInterpolationStop;

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

            AddNewRoot(Vector3.zero);
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
            PointerPosition = pos;
            GetTheRightRoot(true);
        }
        
        private void OnLeftMouseDownEventHandler()
        {
            RootToModify = GetTheRightRoot();
            IsInterpolating = true;
            m_onInterpolateStart?.Invoke();
        }

        private void OnRightMouseDownEventHandler()
        {
            m_onInterpolationStop?.Invoke();
        }
        
        private void OnMouseHoldEventHandler()
        {
            if (_frontColliderBehaviour.IsBlocked) return;
            
            RootToModify.Grow(RootToModify, PointerPosition);
            m_onInterpolate?.Invoke((Vector3)RootToModify.Container.Spline[^1].Position);
        }
        
        private void OnMouseUpEventHandler()
        {
            RootToModify.DeleteIfTooClose(RootToModify);
            IsInterpolating = false;
            
            m_onInterpolateEnd?.Invoke();
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
            if (onlySetKnot) return null;
            
            return IsLastKnotFromSpline(closestKnot, root) ? root : AddNewRoot((Vector3)CurrentClosestKnot.Position);
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
        
        #endregion

        
        #region Utils
        
        private bool IsLastKnotFromSpline(BezierKnot knot, RootV2 root)
        {
            return root.Container.Spline.ToArray()[^1].Equals(knot);
        }
        
        #endregion
        
        
        #region Private and Protected Members
        
        [SerializeField] private GameObject _rootPrefab;
        [SerializeField] private FrontColliderBehaviour _frontColliderBehaviour;
        [Space]
        private List<RootV2> _rootsList = new();
        private Vector3 _pointerPosition;
        private RootV2 _rootToModify;
        private BezierKnot _currentClosestKnot;
        private bool _isInterpolating;

        #endregion
    }
}
