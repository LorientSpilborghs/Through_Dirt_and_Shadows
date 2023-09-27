using System;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneEndGame : Zone
    {
        public static ZoneEndGame Instance { get; private set; }

        public Action m_onEnterZoneEndGame;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        protected override void OnEnterZone()
        {
            Debug.Log("chouette");
            m_onEnterZoneEndGame?.Invoke();
        }

        protected override void OnExitZone(){}

        [SerializeField] private Animator _uiVisualFade;
    }
}
