using System.Collections;
using ResourcesManagerFeature.Runtime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneHarvest : Zone
    {
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
            if (_totalResources <= 0) return false;

            int count = _resourcesCollectOverTime;
            int newTotalResources = _totalResources - _resourcesCollectOverTime;
            
            if (_totalResources < _resourcesCollectOverTime) { count = _totalResources; newTotalResources = 0; }
            ResourcesManagerOne.Instance.AddResources(count);
            _totalResources = newTotalResources;
            
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        [SerializeField] private float _timeBetweenCollect;
        [SerializeField] private int _resourcesCollectOverTime;
        [SerializeField] private int _totalResources;

        private bool _isCollecting;
    }
}