using System.Collections;
using ResourcesManagerFeature.Runtime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneHarvest : Zone
    {
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
            _nuclearCrateEmissionModifier = GetComponentInParent<NuclearCrateEmissionModifier>();
            CurrentResources = BaseResources;
        }

        protected override void OnEnterZone()
        {
            if (_isCollecting) return;
            StartCoroutine(Collecting());
            _isCollecting = true;
        }
        protected override void OnExitZone() {} 
        
        private IEnumerator Collecting()
        {
            yield return new WaitForSeconds(_timeBetweenCollect);
            if (!Collect()) yield break;
            StartCoroutine(Collecting());
        }
        
        private bool Collect()
        {
            if (CurrentResources <= 0) return false;

            int count = _resourcesCollectOverTime;
            int newTotalResources = CurrentResources - _resourcesCollectOverTime;
            _nuclearCrateEmissionModifier?.ModifyEmissionBasedOnResources(_resourcesCollectOverTime);

            if (CurrentResources < _resourcesCollectOverTime) { count = CurrentResources; newTotalResources = 0; }
            ResourcesManager.Instance.AddResources(count);
            CurrentResources = newTotalResources;
            
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        [SerializeField] private float _timeBetweenCollect;
        [SerializeField] private int _resourcesCollectOverTime;
        [SerializeField] private int _baseResources;

        private NuclearCrateEmissionModifier _nuclearCrateEmissionModifier;
        private Material _material;
        private bool _isCollecting;
        private int _completionPercentage;
        private int _currentResources;
    }
}