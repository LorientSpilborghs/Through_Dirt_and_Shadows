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
        }
        
        public void ModifyEmissionBasedOnResources(int resources)
        {
            _totalCurrentResources -= resources;
            foreach (var meshRenderer in _nuclearSticksMeshRenderer)
            {
                Color colour = meshRenderer.material.GetColor("_EmissionColor");
                meshRenderer.material.SetColor("_EmissionColor", colour * _totalCurrentResources/ _totalBaseResources);
            }
        }
        
        [SerializeField] private MeshRenderer[] _nuclearSticksMeshRenderer;
        [SerializeField] private ZoneHarvest[] _zoneHarvests;
        
        private float _totalCurrentResources;
        private float _totalBaseResources;
    }
}
