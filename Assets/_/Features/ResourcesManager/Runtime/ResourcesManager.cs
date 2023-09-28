using System;
using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        public Action m_onResourcesChange;
        
        public int CurrentResources
        {
            get => _currentResources;
            set => _currentResources = value;
        }

        public int ResourcesCostDivider
        {
            get => _resourcesCostDivider;
            set => _resourcesCostDivider = value;
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

        public bool UseResources(int quantity = 1)
        {
            if (quantity > CurrentResources) return false;

            CurrentResources -= quantity;
            m_onResourcesChange?.Invoke();
            return true;
        }

        public void AddResources(int quantity = 1)
        {
            CurrentResources += quantity;
            if (CurrentResources > _maxResources)
            {
                CurrentResources = _maxResources;
            }
            
            m_onResourcesChange?.Invoke();
        }

        [SerializeField] private int _baseResources = 500;
        [SerializeField] private int _maxResources = 1000;
        [SerializeField] private int _resourcesCostDivider = 1;
        
        private int _currentResources;
    }
}
