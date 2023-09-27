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

        public int CurrentResources
        {
            get => _currentResources;
            set => _currentResources = value;
        }

        private void Awake()
        {
            _nuclearCrateEmissionModifier = GetComponentInParent<NuclearCrateEmissionModifierV2>();
            CurrentResources = BaseResources;
        }

        protected override void OnEnterZone()
        {
            if (_isCollecting) return;
            float timeBetweenCollect = _baseTimeBetweenCollect;
            StartCoroutine(Collecting(timeBetweenCollect));
            _isCollecting = true;
        }

        protected override void OnExitZone() {} 
        
        private IEnumerator Collecting(float timeBetweenCollect)
        {
            float zoneBoostDivider = 0;
            foreach (var zoneBoost in _zoneBoosts)
            {
                if (!zoneBoost.IsActive) continue;
                zoneBoostDivider++;
            }

            if (zoneBoostDivider > 0)
            {
                timeBetweenCollect = _baseTimeBetweenCollect / (zoneBoostDivider * _timeReducingEfficiencyPercentage / 100);
            }
            
            if (!Collect()) yield break;
            
            yield return new WaitForSeconds(timeBetweenCollect);
            StartCoroutine(Collecting(timeBetweenCollect));
        }
        
        private bool Collect()
        {
            if (CurrentResources <= 0)
            {
                if (!_isImpactingGlobalPurification) return false;
                GlobalPurification.Instance.m_onZonePurified?.Invoke(_globalPercentageOnPurified);
                return false;
            }

            int count = _resourcesCollectOverTime;
            int newTotalResources = CurrentResources - _resourcesCollectOverTime;
            if (CurrentResources < _resourcesCollectOverTime) { count = CurrentResources; newTotalResources = 0; }
            
            ResourcesManager.Instance.AddResources(count);
            CurrentResources = newTotalResources;
            m_onValueChange?.Invoke();
            _nuclearCrateEmissionModifier?.ModifyEmissionBasedOnResources();
            
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        [SerializeField] private float _baseTimeBetweenCollect;
        [SerializeField] private int _resourcesCollectOverTime;
        [SerializeField] private int _baseResources;
        [SerializeField] private int _timeReducingEfficiencyPercentage = 100;
        [SerializeField] private bool _isImpactingGlobalPurification;
        [SerializeField] private int _globalPercentageOnPurified;
        [Space] [SerializeField] private ZoneBoost[] _zoneBoosts;

        private NuclearCrateEmissionModifierV2 _nuclearCrateEmissionModifier;
        private bool _isCollecting;
        private int _completionPercentage;
        private int _currentResources;
        private int _currentKnotInTheZone;
    }
}
