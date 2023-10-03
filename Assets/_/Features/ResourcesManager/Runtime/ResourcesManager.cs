using System;
using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        public Action m_onResourcesChange;
        
        public float CurrentResources
        {
            get => _currentResources;
            set => _currentResources = value;
        }

        public int ResourcesCostDivider
        {
            get => _resourcesCostDivider;
            set => _resourcesCostDivider = value;
        }

        public float MaxResources
        {
            get => _maxResources;
            set => _maxResources = value;
        }

        public float TotalUpcomingResources
        {
            get => _totalUpcomingResources;
            set => _totalUpcomingResources = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CurrentResources = _baseResources;
            TotalUpcomingResources = CurrentResources;
        }

        public bool UseResources(float quantity)
        {
            if (quantity > CurrentResources) return false;

            CurrentResources -= quantity;
            TotalUpcomingResources -= quantity;
            m_onResourcesChange?.Invoke();
            return true;
        }

        public void AddResources(float quantity, bool isDeletingKnot)
        {
            CurrentResources += quantity;

            if (isDeletingKnot)
            {
                TotalUpcomingResources += quantity;
            }
            
            if (CurrentResources > MaxResources)
            {
                CurrentResources = MaxResources;
                TotalUpcomingResources -= quantity;
            }
            
            m_onResourcesChange?.Invoke();
        }

        [SerializeField] private float _baseResources = 500;
        [SerializeField] private float _maxResources = 1000;
        [SerializeField] private int _resourcesCostDivider = 1;
        
        private float _currentResources;
        private float _totalUpcomingResources;
    }
}
