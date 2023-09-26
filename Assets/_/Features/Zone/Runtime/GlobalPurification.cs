using System;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class GlobalPurification : MonoBehaviour
    {
        public static GlobalPurification Instance { get; private set; }

        public int CurrentPercentage
        {
            get => _currentPercentage;
            set => _currentPercentage = value;
        }

        public Action m_onValueChange;
        public Action<int> m_onZonePurified;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            m_onZonePurified += UpdateGlobalPurification;
        }

        private void UpdateGlobalPurification(int globalPurificationPercentage)
        {
            CurrentPercentage += globalPurificationPercentage;
            
            m_onValueChange?.Invoke();
            if (CurrentPercentage < _neededPercentageToWin) return;
            
            //TODO ENDGAME
        }

        [SerializeField] private int _neededPercentageToWin;

        private int _currentPercentage;
    }
}
