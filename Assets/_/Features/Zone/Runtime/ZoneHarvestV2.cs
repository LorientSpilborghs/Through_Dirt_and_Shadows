using System;
using System.Collections;
using ResourcesManagerFeature.Runtime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneHarvestV2 : Zone
    {
        public Action m_onValueChange;
        
        public int BaseResources
        {
            get => _baseResources;
            set => _baseResources = value;
        }

        public float CurrentResources
        {
            get => _currentResources;
            set => _currentResources = value;
        }

        private void Awake()
        {
            _nuclearCrateEmissionModifier = GetComponentInParent<NuclearCrateEmissionModifierV2>();
            CurrentResources = BaseResources;
        }

        private void Start()
        {
            _resourcesManager = ResourcesManager.Instance;
        }

        protected override void OnEnterZone()
        {
            if (_isCollecting) return;
            
            _resourcesManager.TotalUpcomingResources += _baseResources;
            
            if (_zoneParticle.Length != 0)
            {
                foreach (var particleSystem in _zoneParticle)
                {
                    particleSystem.gameObject.SetActive(true);
                }
            }
            
            StartCoroutine(Collecting());
            _isCollecting = true;
        }

        protected override void OnExitZone() {} 
        
        // Collect System Over Time
        // private IEnumerator Collecting(float timeBetweenCollect)
        // {
        //     float zoneBoostDivider = 0;
        //     foreach (var zoneBoost in _zoneBoosts)
        //     {
        //         if (!zoneBoost.IsActive) continue;
        //         zoneBoostDivider++;
        //     }
        //
        //     if (zoneBoostDivider > 0)
        //     {
        //         timeBetweenCollect = _baseTimeBetweenCollect / (zoneBoostDivider * _timeReducingEfficiencyPercentage / 100);
        //     }
        //     
        //     if (!Collect()) yield break;
        //     
        //     yield return new WaitForSeconds(timeBetweenCollect);
        //     StartCoroutine(Collecting(timeBetweenCollect));
        // }     
        
        private IEnumerator Collecting()
        {
            float boostedCollectPerSeconds;
            do
            {
                boostedCollectPerSeconds = _resourcesCollectPerSeconds;
                float zoneBoostDivider = 0;

                foreach (var zoneBoost in _zoneBoosts)
                {
                    if (!zoneBoost.IsActive) continue;
                    zoneBoostDivider++;
                }

                if (zoneBoostDivider > 0)
                {
                    boostedCollectPerSeconds += boostedCollectPerSeconds * (zoneBoostDivider * _timeReducingEfficiencyPercentage / 100);
                }

                yield return null;
            } while (Collect(boostedCollectPerSeconds));
        }
        
        private bool Collect(float boostedCollectPerSeconds)
        {
            if (CurrentResources <= 0)
            {
                if (!_isImpactingGlobalPurification) return false;
                GlobalPurification.Instance.m_onZonePurified?.Invoke(_globalPercentageOnPurified);

                if (_zoneParticle.Length == 0) return false;
                
                foreach (var particleSystem in _zoneParticle)
                {
                    particleSystem.gameObject.SetActive(false);
                }

                return false;
            }

            float gainedResources = boostedCollectPerSeconds * Time.deltaTime;
            float newTotalResources;

            if (CurrentResources >= gainedResources)
            {
                newTotalResources = CurrentResources - gainedResources;
            }
            else
            {
                gainedResources = CurrentResources;
                newTotalResources = 0;
            }
            
            ResourcesManager.Instance.AddResources(gainedResources, false);
            CurrentResources = newTotalResources;
            m_onValueChange?.Invoke();
            _nuclearCrateEmissionModifier?.ModifyEmissionBasedOnResources();
            
            return true;
        }
        

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        [SerializeField] private int _resourcesCollectPerSeconds;
        [SerializeField] private int _baseResources;
        [SerializeField] private int _timeReducingEfficiencyPercentage = 100;
        [SerializeField] private bool _isImpactingGlobalPurification;
        [SerializeField] private float _globalPercentageOnPurified;
        [Space] [SerializeField] private ParticleSystem[] _zoneParticle;
        [Space] [SerializeField] private ZoneBoost[] _zoneBoosts;

        private NuclearCrateEmissionModifierV2 _nuclearCrateEmissionModifier;
        private ResourcesManager _resourcesManager;
        private bool _isCollecting;
        private int _completionPercentage;
        private float _currentResources;
        private int _currentKnotInTheZone;
    }
}
