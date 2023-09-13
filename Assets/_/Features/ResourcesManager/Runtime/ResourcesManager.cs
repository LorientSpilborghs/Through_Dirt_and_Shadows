using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        public int CurrentResources
        {
            get => _currentResources;
            set => _currentResources = value;
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
            return true;
        }

        public void AddResources(int quantity = 1)
        {
            CurrentResources += quantity;
        }

        [SerializeField] private int _baseResources = 500;
        
        private int _currentResources;
    }
}
