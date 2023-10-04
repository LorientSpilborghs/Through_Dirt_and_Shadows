using ResourcesManagerFeature.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessManagerFeature.Runtime
{
    public class PostProcessManager : MonoBehaviour
    {
        public static PostProcessManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            _resourcesManager = ResourcesManager.Instance;
            _resourcesManager.m_onResourcesChange += UpdateVignetteBasedOnHealth;
        }

        private void UpdateVignetteBasedOnHealth()
        {
            float currentResourcesNormalized = _resourcesManager.CurrentResources/_resourcesManager.MaxResources;
            _globalVolume.profile.TryGet(out Vignette vignette);
                
            if (currentResourcesNormalized > _highHealthPercentage)
            {
                vignette.color.Override(_vignetteColorAtHighHealth.value);

                vignette.intensity.value = _vignetteMaxIntensity;
            }
            else if (currentResourcesNormalized < _lowHealthPercentage)
            {
                vignette.color.Override(_vignetteColorAtLowHealth.value);
                
                vignette.intensity.value = _vignetteMaxIntensity;
            }
            else
            {
                vignette.color.Override(Color.white);
            }
        }
        

        [SerializeField] private Volume _globalVolume;
        [Space] [Header("Vignette Effect")]
        [SerializeField] [Range(0,1)] private float _vignetteMaxIntensity;
        [SerializeField] private float _lerpDuration = 1;
        [SerializeField] [Range(0,1)] private float _lowHealthPercentage = 0.2f;
        [SerializeField] [Range(0,1)] private float _highHealthPercentage = 0.8f;
        [SerializeField] private ColorParameter _vignetteColorAtLowHealth;
        [SerializeField] private ColorParameter _vignetteColorAtHighHealth;

        private ResourcesManager _resourcesManager;
        private Coroutine _vignetteLerp;
        private float _timer;
    }
}
