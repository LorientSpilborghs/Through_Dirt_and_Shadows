using System;
using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public bool UseResources(int quantity = 1)
        {
            if (quantity > _currentResources) return false;

            _currentResources -= quantity;
            return true;
        }

        public void AddResources(int quantity = 1)
        {
            _currentResources += quantity;
        }
        
        private int _currentResources;
    }
}
