using System;
using System.Collections;
using UnityEngine;

namespace InputManagerFeature.Runtime
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        public Action<Vector3> m_onMouseMove;
        
        public Action m_onMouseDown;
        public Action m_onMouseHold;
        public Action m_onMouseUp;
        public Action m_onSpaceBarDown;

        public int FpsCount;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            _camera = Camera.main;
            
            StartCoroutine(Wait(1));
        }

        private void Update()
        {
            MouseWorldPosition();
            
            OnMouseDown();
            OnMouseHold();
            OnMouseUp();
            OnSpaceBarDown();
        }

        private IEnumerator Wait(float secToWait)
        {
            yield return new WaitForSeconds(secToWait);
            FpsCount = (int)(1f / Time.unscaledDeltaTime);
            StartCoroutine(Wait(0.5f));
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

        private void OnMouseDown()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return; 
            m_onMouseDown?.Invoke();
        }

        private void OnMouseHold()
        {
            if (!Input.GetKey(KeyCode.Mouse0)) return; 
            m_onMouseHold?.Invoke();
        }

        private void OnMouseUp()
        {
            if (!Input.GetKeyUp(KeyCode.Mouse0)) return; 
            m_onMouseUp?.Invoke();
        }

        private void OnSpaceBarDown()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            m_onSpaceBarDown?.Invoke();
        }


        [SerializeField] private LayerMask _layerMask;
        private bool _isPushing;
        private Camera _camera;
        private Vector3 _previousMousePos;
    }
}