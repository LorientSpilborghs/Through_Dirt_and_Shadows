using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class NuclearCrateEmissionModifier : MonoBehaviour
    {
        private void Start()
        {
            foreach (var zoneHarvest in _zoneHarvests)
            {
                _totalBaseResources += zoneHarvest.BaseResources;
                _totalCurrentResources += zoneHarvest.CurrentResources;
            }
            _color = _nuclearSticksMeshRenderer[0].material.GetColor("_EmissionColor");
        }
        
        public void ModifyEmissionBasedOnResources(int resources)
        {
            _totalCurrentResources -= resources;
            
            foreach (var meshRenderer in _nuclearSticksMeshRenderer)
            {
                meshRenderer.material.SetColor("_EmissionColor", _color * _totalCurrentResources/ _totalBaseResources);
            }
            if (!(_totalCurrentResources <= 0)) return;
            
            foreach (var meshRenderer in _nuclearSticksMeshRenderer)
            {
                meshRenderer.material = _emptyStickMaterial;
            }
        }
        
        [SerializeField] private MeshRenderer[] _nuclearSticksMeshRenderer;
        [SerializeField] private ZoneHarvest[] _zoneHarvests;
        [SerializeField] private Material _emptyStickMaterial;

        private Color _color;
        private float _totalCurrentResources;
        private float _totalBaseResources;
    }
}
