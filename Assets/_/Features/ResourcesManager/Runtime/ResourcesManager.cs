using System;
using System.Collections.Generic;
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

        public List<float> UpcomingHarvest
        {
            get => _upcomingHarvest;
            set => _upcomingHarvest = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CurrentResources = _baseResources;
        }

        public bool UseResources(float quantity = 1)
        {
            if (quantity > CurrentResources) return false;

            CurrentResources -= quantity;
            m_onResourcesChange?.Invoke();
            return true;
        }

        public void AddResources(float quantity = 1)
        {
            CurrentResources += quantity;
            if (CurrentResources > MaxResources)
            {
                CurrentResources = MaxResources;
            }
            
            m_onResourcesChange?.Invoke();
        }

        [SerializeField] private float _baseResources = 500;
        [SerializeField] private float _maxResources = 1000;
        [SerializeField] private int _resourcesCostDivider = 1;

        private List<float> _upcomingHarvest = new List<float>();
        private float _currentResources;
    }
}
