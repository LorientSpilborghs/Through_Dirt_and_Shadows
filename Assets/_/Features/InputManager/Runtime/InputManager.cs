using System;
using System.Collections;
using System.Drawing;
using GameManagerFeature.Runtime;
using UnityEngine;

namespace InputManagerFeature.Runtime
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        public Action<Vector3> m_onMouseMove;
        
        public Action m_onLeftMouseDown;
        public Action m_onLeftMouseHold;
        public Action m_onLeftMouseUp;
        public Action m_onRightMouseHold;
        public Action m_onRightMouseUp;
        public Action m_onMiddleMouseDown;
        public Action m_onEscapeKeyDown;
        public Action m_onTabKeyDown;

        public int FpsCount;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            _camera = Camera.main;
            StartCoroutine(HandleFpsCounter(1));
        }

        private void Update()
        {
            OnEscapeKeyDown();
            if (GameManager.Instance.IsGamePause) return;
            if (GameManager.Instance.IsCutScenePlaying) return;
            MouseWorldPosition();
            
            OnLeftMouseDown();
            OnRightMouseHold();
            OnRightMouseUp();
            OnMiddleMouseDown();
            OnLeftMouseHold();
            OnleftMouseUp();
            OnTabKeyDown();
        }

        private IEnumerator HandleFpsCounter(float secToWait)
        {
            yield return new WaitForSeconds(secToWait);
            FpsCount = (int)(1f / Time.unscaledDeltaTime);
            StartCoroutine(HandleFpsCounter(0.5f));
        }

        private void OnGUI()
        {
            GUILayout.Label($"<color=red>FPS {FpsCount}</color>");
        }

        private void MouseWorldPosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100, _layerMask)) return;
            if (hit.point == _previousMousePos) return;
            
            m_onMouseMove?.Invoke(hit.point);
            _previousMousePos = hit.point;
        }

        private void OnLeftMouseDown()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            m_onLeftMouseDown?.Invoke();
        }

        private void OnRightMouseHold()
        {
            if (!Input.GetKey(KeyCode.Mouse1)) return;
            m_onRightMouseHold?.Invoke();
        }

        private void OnRightMouseUp()
        {
            if (!Input.GetKeyUp(KeyCode.Mouse1)) return;
            m_onRightMouseUp?.Invoke();
        }

        private void OnMiddleMouseDown()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse2)) return;
            m_onMiddleMouseDown?.Invoke();
        }

        private void OnLeftMouseHold()
        {
            if (!Input.GetKey(KeyCode.Mouse0)) return; 
            m_onLeftMouseHold?.Invoke();
        }

        private void OnleftMouseUp()
        {
            if (!Input.GetKeyUp(KeyCode.Mouse0)) return; 
            m_onLeftMouseUp?.Invoke();
        }

        private void OnEscapeKeyDown()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            m_onEscapeKeyDown?.Invoke();
        }

        private void OnTabKeyDown()
        {
            if (!Input.GetKeyDown(KeyCode.Tab)) return;
            m_onTabKeyDown?.Invoke();
        }
        
        
        [SerializeField] private LayerMask _layerMask;
        private Camera _camera;
        private Vector3 _previousMousePos;
    }
}