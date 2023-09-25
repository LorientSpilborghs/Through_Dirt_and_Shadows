using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class GlobalPurification : MonoBehaviour
    {
        public static GlobalPurification Instance { get; private set; }
        
        public List<ZonePurification> ZonePurifications
        {
            get => _zonePurifications;
            set => _zonePurifications = value;
        }

        public int CurrentPercentage
        {
            get => _currentPercentage;
            set => _currentPercentage = value;
        }

        public Action m_onValueChange;
        public Action m_onZonePurified;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            m_onZonePurified += UpdateGlobalPurification;
            foreach (var gameObject in GameObject.FindGameObjectsWithTag("Purification"))
            {
                ZonePurifications.Add(gameObject.GetComponent<ZonePurification>());
            }
        }

        private void UpdateGlobalPurification()
        {
            foreach (var zonePurification in _zonePurifications)
            {
                if (!zonePurification.IsPurified) continue;
                CurrentPercentage += zonePurification.GlobalPercentageOnPurified;
            }
            
            m_onValueChange?.Invoke();
            if (CurrentPercentage < 100) return;
            
            //TODO ENDGAME
        }

        private List<ZonePurification> _zonePurifications = new List<ZonePurification>();
        private int _currentPercentage;
    }
}
