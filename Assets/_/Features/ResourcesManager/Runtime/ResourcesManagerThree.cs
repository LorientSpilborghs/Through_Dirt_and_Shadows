using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManagerThree : MonoBehaviour
    {
        public static ResourcesManagerThree Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _currentResources = _baseResources;
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

        [SerializeField] private int _baseResources = 500;
        
        private int _currentResources;
    }
}
