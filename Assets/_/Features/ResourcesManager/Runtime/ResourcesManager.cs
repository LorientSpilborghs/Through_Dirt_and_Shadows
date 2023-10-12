using System;
using GameManagerFeature.Runtime;
using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        public Action m_onResourcesChange;
        public Action<int, float> m_onChangeMaxHealthTier;
        
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
            _gameManager = GameManager.Instance;
        }

        public bool UseResources(float quantity)
        {
            if (quantity > CurrentResources)
            {
                CheckIfGameIsOver(CurrentResources, TotalUpcomingResources);
                return false;
            }

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

        public void ChangePlayerMaxResources(float currentPurificationPercentage)
        {
            if (currentPurificationPercentage >= _purificationPercentageForTierTwo && !_isTierTwoUnlocked)
            {
                _maxResources = _maxResourcesAtTierTwo;
                m_onChangeMaxHealthTier?.Invoke(2, _maxResourcesAtTierTwo);
                _isTierTwoUnlocked = true;
                return;
            }
            
            if (currentPurificationPercentage >= _purificationPercentageForTierThree && !_isTierThreeUnlocked)
            {
                _maxResources = _maxResourcesAtTierThree;
                m_onChangeMaxHealthTier?.Invoke(3, _maxResourcesAtTierThree);
                _isTierThreeUnlocked = true;
                return;
            }

            if (!(currentPurificationPercentage >= _purificationPercentageForTierFour) && !_isTierFourUnlocked) return;
            _maxResources = _maxResourcesAtTierFour;
            m_onChangeMaxHealthTier?.Invoke(4, _maxResourcesAtTierFour);
            _isTierFourUnlocked = true;
        }

        private void CheckIfGameIsOver(float currentResources, float totalUpcomingResources)
        {
            if (currentResources < 1 && totalUpcomingResources < 1 && !_gameManager.PlayerLost)
            {
                _gameManager.m_onGameOver?.Invoke();
            }
        }
        

        [SerializeField] private float _baseResources = 250;
        [SerializeField] private float _maxResources = 500;
        [SerializeField] private int _resourcesCostDivider = 100;
        [Space] 
        [Header("Manage Player Max Health Based On Purification Percentage")]
        [SerializeField] private float _purificationPercentageForTierTwo = 20;
        [SerializeField] private float _purificationPercentageForTierThree = 40;
        [SerializeField] private float _purificationPercentageForTierFour = 60;
        [SerializeField] private float _maxResourcesAtTierTwo = 1000;
        [SerializeField] private float _maxResourcesAtTierThree = 1500;
        [SerializeField] private float _maxResourcesAtTierFour = 2000;


        private GameManager _gameManager;
        private float _currentResources;
        private float _totalUpcomingResources;
        private bool _isTierTwoUnlocked;
        private bool _isTierThreeUnlocked;
        private bool _isTierFourUnlocked;
    }
}
