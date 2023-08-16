using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Runtime
{
    public class SpawnRoot : MonoBehaviour
    {
        private void Update()
        {
            if (RootController.m_instance.m_isControllingRoot) return;

            if (!Input.GetMouseButtonDown(0)) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _raycastHit, 10))
            {
                if (_raycastHit.transform.CompareTag("Heart"))
                {
                    Instantiate(_rootPrefab, _raycastHit.transform);
                }

                else if (_raycastHit.transform.CompareTag("Root"))
                {
                    Instantiate(_rootPrefab, _raycastHit.transform);
                }
            }
        }

        [SerializeField] private GameObject _rootPrefab;

        private RaycastHit _raycastHit;
    }
}
