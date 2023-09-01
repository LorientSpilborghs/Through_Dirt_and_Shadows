using System;
using System.Linq;
using InputManagerFeature.Runtime;
using RootFeature.Runtime;
using UnityEngine;
using UnityEngine.Splines;

namespace PlayerRuntime
{
    public class Player : MonoBehaviour
    {
        #region Public Members
        
        public static Player Instance { get; private set; }

        public Action m_onInterpolateStart;
        public Action<Vector3> m_onInterpolate;
        public Action m_onInterpolateEnd;
        public Action m_onMouseMove;
        public Action m_onResetCameraPos;

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

        public Spline SplineToModify
        {
            get => _splineToModify;
            set => _splineToModify = value;
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
            InputManager.Instance.m_onMouseDown += OnMouseDownEventHandler;
            InputManager.Instance.m_onMouseHold += OnMouseHoldEventHandler;
            InputManager.Instance.m_onMouseUp += OnMouseUpEventHandler;
            InputManager.Instance.m_onSpaceBarDown += OnSpaceBarDownEventHandler;
        }

        private void OnDestroy()
        {
            InputManager.Instance.m_onMouseMove -= OnMouseMoveEventHandler;
        }
        
        #endregion

        #region My Methods
        
        private void OnMouseMoveEventHandler(Vector3 pos)
        {
            PointerPosition = pos;
            GetTheRightSpline(true);
            m_onMouseMove.Invoke();
        }
        
        private void OnMouseDownEventHandler()
        {
            SplineToModify = GetTheRightSpline();
            _rootOrigin.CanRebuild(true);
            IsInterpolating = true;
            
            m_onInterpolateStart?.Invoke();
        }
        
        private void OnMouseHoldEventHandler()
        {
            //Vector3 splineDirection = m_previousKnotPosition - (Vector3)_rootToModify.MySpline.Knots.ToArray()[_rootToModify.MySpline.Knots.Count() - 2].Position;
            //float angle = Vector3.SignedAngle(Vector3.forward, splineDirection, Vector3.up);
            
            _rootOrigin.Grow(SplineToModify, PointerPosition);
            m_onInterpolate?.Invoke(SplineToModify[^1].Position);
        }
        
        private void OnMouseUpEventHandler()
        {
            _rootOrigin.DeleteIfTooClose(SplineToModify);
            _rootOrigin.CanRebuild(false);
            IsInterpolating = false;
            
            m_onInterpolateEnd?.Invoke();
        }
        
        private void OnSpaceBarDownEventHandler()
        {
            m_onResetCameraPos?.Invoke();
        }
        
        private Spline GetTheRightSpline(bool onlySetKnot = false)
        {
            Spline spline = _rootOrigin.SplinesList[0];
            BezierKnot closestKnot = spline.Knots.ToArray()[^1];
        
            for (int i = 0; i < _rootOrigin.SplinesList.Count; i++)
            {
                for (int j = 0; j < _rootOrigin.SplinesList[i].Knots.Count(); j++)
                {
                    float dist1 = Vector3.Distance(_rootOrigin.SplinesList[i].Knots.ToArray()[j].Position, PointerPosition);
                    float dist2 = Vector3.Distance(closestKnot.Position, PointerPosition);
                    if (dist1 > dist2) continue;
        
                    closestKnot = _rootOrigin.SplinesList[i].Knots.ToArray()[j];
                    spline = _rootOrigin.SplinesList[i];
                }
            }
        
            CurrentClosestKnot = closestKnot;
            if (onlySetKnot) return null;
            
            return IsLastKnotFromSpline(closestKnot, spline) ? spline : _rootOrigin.AddSpline(closestKnot.Position);
        }
        
        #endregion

        
        #region Utils
        
        private bool IsLastKnotFromSpline(BezierKnot knot, Spline spline)
        {
            return spline.ToArray()[^1].Equals(knot);
        }
        
        #endregion
        
        
        #region Private and Protected Members
        
        [SerializeField] private Root _rootOrigin;
        
        private Vector3 _pointerPosition;
        private Spline _splineToModify;
        private BezierKnot _currentClosestKnot;
        private bool _isInterpolating;

        #endregion
    }
}
