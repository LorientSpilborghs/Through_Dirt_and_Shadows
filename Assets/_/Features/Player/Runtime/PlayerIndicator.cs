using System;
using UnityEngine;

namespace Player.Runtime
{
    public class PlayerIndicator : MonoBehaviour
    {
        public SplineRootControllerV2 m_splineRootControllerV2;
        
        private void Start()
        {
            m_splineRootControllerV2.onRootCreation += OnRootCreationEventHandler;
        }

        private void OnRootCreationEventHandler(object sender, EventArgs e)
        {
            Instantiate(_arrow);
        }

        [SerializeField] private GameObject _arrow;
    }
}
