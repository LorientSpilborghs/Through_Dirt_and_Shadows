using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class NuclearCrateEmissionModifierV2 : MonoBehaviour
    {
        private void Start()
        {
            _color = _nuclearSticksMeshRenderer[0].material.GetColor("_EmissionColor");
        }
        
        public void ModifyEmissionBasedOnResources()
        {
            foreach (var meshRenderer in _nuclearSticksMeshRenderer)
            {
                meshRenderer.material.SetColor("_EmissionColor", _color * _zoneHarvests.CurrentResources/ _zoneHarvests.BaseResources);
            }
            if (!(_zoneHarvests.CurrentResources <= 0)) return;
            
            foreach (var meshRenderer in _nuclearSticksMeshRenderer)
            {
                meshRenderer.material = _emptyStickMaterial;
            }
        }
        
        [SerializeField] private MeshRenderer[] _nuclearSticksMeshRenderer;
        [SerializeField] private ZoneHarvestV2 _zoneHarvests;
        [SerializeField] private Material _emptyStickMaterial;

        private Color _color;
    }
}
