using System;
using System.Collections.Generic;
using System.Linq;
using GameManagerFeature.Runtime;
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
        public Action m_onCameraBlendingStop;
        public Action<Vector3> m_onInterpolate;
        public Action<bool> m_onCameraRotate;
        public Action m_onResetCameraPos;
        public Action m_onNewKnotInstantiate;
        public Action m_onPauseMenu;
        public Action m_onUIShow;
        public Action<bool> m_onNewKnotSelected;
        public Func<bool> m_isCameraBlendingOver;
        public Func<bool> m_isInThirdPerson;
        public Func<bool> m_isInTopView;
        
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

        public int CurrentClosestKnotIndex
        {
            get => _currentClosestKnotIndex;
            set => _currentClosestKnotIndex = value;
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
            _resourcesManager = ResourcesManager.Instance;
            _gameManager = GameManager.Instance;
            m_onCameraBlendingStart += OnCameraBlendingStartEventHandler;
            m_onCameraBlendingStop += OnCameraBlendingStopEventHandler;
            InputManager.Instance.m_onMouseMove += OnMouseMoveEventHandler;
            InputManager.Instance.m_onLeftMouseDown += OnLeftMouseDownEventHandler;
            InputManager.Instance.m_onRightMouseHold += OnRightMouseHoldEventHandler;
            InputManager.Instance.m_onRightMouseUp += OnRightMouseUpEventHandler;
            InputManager.Instance.m_onPositiveScrollDown += OnPositiveScrollDownEventHandler;
            InputManager.Instance.m_onNegativeScrollDown += OnNegativeScrollDownEventHandler;
            InputManager.Instance.m_onSpaceKeyDown += OnSpaceKeyEventHandler;
            InputManager.Instance.m_onLeftMouseHold += OnMouseHoldEventHandler;
            InputManager.Instance.m_onLeftMouseUp += OnMouseUpEventHandler;
            InputManager.Instance.m_onEscapeKeyDown += OnEscapeKeyDownEventHandler;
            InputManager.Instance.m_onTabKeyDown += OnTabKeyDownEventHandler;
            InputManager.Instance.m_onReturnKeyDown += OnReturnKeyDownEventHandler;
            
            RootToModify = AddNewRoot(_playerStartPosition.position + Vector3.up * _heightOfTheRootAtStart);
        }

        private void OnDestroy()
        {
            InputManager.Instance.m_onMouseMove -= OnMouseMoveEventHandler;
            InputManager.Instance.m_onLeftMouseDown -= OnLeftMouseDownEventHandler;
            InputManager.Instance.m_onRightMouseHold -= OnRightMouseHoldEventHandler;
            InputManager.Instance.m_onRightMouseUp -= OnRightMouseUpEventHandler;
            InputManager.Instance.m_onPositiveScrollDown -= OnPositiveScrollDownEventHandler;
            InputManager.Instance.m_onNegativeScrollDown -= OnNegativeScrollDownEventHandler;
            InputManager.Instance.m_onSpaceKeyDown -= OnSpaceKeyEventHandler;
            InputManager.Instance.m_onLeftMouseHold -= OnMouseHoldEventHandler;
            InputManager.Instance.m_onLeftMouseUp -= OnMouseUpEventHandler;
            InputManager.Instance.m_onEscapeKeyDown -= OnEscapeKeyDownEventHandler;
            InputManager.Instance.m_onTabKeyDown -= OnTabKeyDownEventHandler;
            InputManager.Instance.m_onReturnKeyDown -= OnReturnKeyDownEventHandler;
        }
        
        #endregion

        #region My Methods
        
        private void OnMouseMoveEventHandler(Vector3 pos)
        {
            if (m_isCameraBlendingOver?.Invoke() is false) return;
            PointerPosition = pos;
            if (m_isInThirdPerson?.Invoke() is true)
            {
                _currentClosestKnot = RootToModify.Container.Spline[^1];
                return;
            }
            GetTheRightRoot(true);
        }
        
        private void OnLeftMouseDownEventHandler()
        {
            SelectRoot();
        }
        
        private void OnPositiveScrollDownEventHandler()
        {
            SelectRoot();
        }

        private void SelectRoot()
        {
            if (m_isCameraBlendingOver?.Invoke() is false || m_isInThirdPerson?.Invoke() is true) return;
            RootToModify = GetTheRightRoot() ?? RootToModify;
            m_onCameraBlendingStart?.Invoke(CurrentClosestKnot.Position);
        }
        
        private void CameraRotate()
        {
            m_onCameraRotate?.Invoke(m_isInThirdPerson?.Invoke() is true);
        }
        
        
        private void OnRightMouseHoldEventHandler()
        {
            CameraRotate();
        }

        private void CameraRotateAndDragEnd()
        {
            if (Cursor.visible || Cursor.lockState is CursorLockMode.None) return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        private void OnRightMouseUpEventHandler()
        {
            CameraRotateAndDragEnd();
        }

        private void OnNegativeScrollDownEventHandler()
        {
            m_onCameraBlendingStop?.Invoke();
        }
        
        private void OnMouseHoldEventHandler()
        {
            if (m_isInThirdPerson?.Invoke()is false) return;
            
            if (_frontColliderBehaviour.IsBlocked) return;
            if (RootToModify.SpeedPercentage <= 0) return;
            if (!UseResourcesWhileGrowing(((RootToModify.Container.Spline.Count - 1 + RootToModify.InitialGrowCost) 
                                     * (RootToModify.Container.Spline.Count + RootToModify.InitialGrowCost)) 
                                     / _resourcesManager.ResourcesCostDivider)) return;
            if (!IsInterpolating) RootToModify.StartGrowing();
            RootToModify.Grow(RootToModify, PointerPosition);
            m_onInterpolate?.Invoke((Vector3)RootToModify.Container.Spline[^1].Position);
            IsInterpolating = true;
            RootToModify.IsGrowing = true;
        }
        
        private void OnMouseUpEventHandler()
        {
            RootToModify.EndGrowing();
            IsInterpolating = false;
            RootToModify.IsGrowing = false;
            RootToModify?.DeleteIfTooClose(RootToModify);
        }
        
        private void OnSpaceKeyEventHandler()
        {
            if (m_isInTopView?.Invoke() is false) return;
            m_onResetCameraPos?.Invoke();
        }

        private void OnEscapeKeyDownEventHandler()
        {
            m_onPauseMenu?.Invoke();
        }

        private void OnTabKeyDownEventHandler()
        {
            m_onUIShow?.Invoke();
        }

        private void OnReturnKeyDownEventHandler()
        {
            if (_gameManager.IsGameEnd) return;
            _gameManager.m_onShowEnd?.Invoke();
        }
        
        private RootV2 GetTheRightRoot(bool onlySetKnot = false)
        {
            RootV2 root = _rootsList[0];
            BezierKnot closestKnot = root.Container.Spline.ToArray()[^1];
            int closestKnotIndex = 0;
        
            for (int i = 0; i < _rootsList.Count; i++)
            {
                for (int j = 0; j < _rootsList[i].Container.Spline.Count(); j++)
                {
                    float dist1 = Vector3.Distance(_rootsList[i].Container.Spline.ToArray()[j].Position, PointerPosition);
                    float dist2 = Vector3.Distance(closestKnot.Position, PointerPosition);
                    if (dist1 > dist2) continue;
        
                    closestKnot = _rootsList[i].Container.Spline.ToArray()[j];
                    closestKnotIndex = j;
                    root = _rootsList[i];
                }
            }
            CurrentClosestKnot = closestKnot;
            CurrentClosestKnotIndex = closestKnotIndex;
            CurrentClosestSpline = root.Container.Spline;
            CurrentClosestRoot = root;
            
            m_onNewKnotSelected?.Invoke(IsLastKnotFromSpline(closestKnot, root));
            
            if (onlySetKnot) return null;

            if (IsLastKnotFromSpline(closestKnot, root))
            {
                return root;
            }
            else
            {
                return _resourcesManager.UseResources(IsGettingCostReduction(CurrentClosestRoot, CurrentClosestKnotIndex)) 
                    ? AddNewRoot((Vector3)CurrentClosestKnot.Position) : null;
            }
        }
        
        private RootV2 AddNewRoot(Vector3 position)
        {
            RootV2 newRoot = Instantiate(_rootPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<RootV2>();
            if (RootToModify is not null)
            {
                newRoot.InitialGrowCost = CurrentClosestKnotIndex + CurrentClosestRoot.InitialGrowCost;
            }
            
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
            if (IsMaxDistanceBetweenKnots())
            {
                if (_resourcesManager.UseResources(resourcesUsage))
                {
                    m_onNewKnotInstantiate?.Invoke();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void OnCameraBlendingStartEventHandler(Vector3 pos)
        {
            RootToModify.EnableDisableWarningUI(true);
        }
        
        private void OnCameraBlendingStopEventHandler()
        {
            RootToModify.EnableDisableWarningUI(false);
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

            if (Vector3.Distance(pos1, pos2) < RootToModify.DistanceBetweenKnots) return false;
            return true;
        }

        private int IsGettingCostReduction(RootV2 root, int closestKnotIndex)
        {
            if (closestKnotIndex < root.MinimumNumberOfKnotsForCostReduction)
            {
                return ((closestKnotIndex - 1 + root.InitialGrowCost) 
                       * (closestKnotIndex + root.InitialGrowCost)
                       + root.SupplementalCostForNewRoot) / _resourcesManager.ResourcesCostDivider;
            }
            else
            {
                return (((closestKnotIndex - 1 + root.InitialGrowCost) 
                       * (closestKnotIndex + root.InitialGrowCost)
                       + root.SupplementalCostForNewRoot) / _resourcesManager.ResourcesCostDivider) - root.CostReduction;
            }
        }
        
        #endregion
        
        
        #region Private and Protected Members
        
        [SerializeField] private GameObject _rootPrefab;
        [SerializeField] private FrontColliderBehaviour _frontColliderBehaviour;
        [Space] 
        [SerializeField] private Transform _playerStartPosition;
        [SerializeField] private float _heightOfTheRootAtStart = 0.5f;

        private ResourcesManager _resourcesManager;
        private GameManager _gameManager;
        private List<RootV2> _rootsList = new();
        private Vector3 _pointerPosition;
        private RootV2 _rootToModify;
        private RootV2 _currentClosestRoot;
        private BezierKnot _currentClosestKnot;
        private Spline _currentClosestSpline;
        private int _currentClosestKnotIndex;
        private bool _isInterpolating;

        #endregion
    }
}
