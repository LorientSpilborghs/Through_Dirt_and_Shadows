using System.Collections;
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
            // _resourcesManager.m_onResourcesChange += UpdateVignetteBasedOnHealth;
            _isFadeOut = true;
        }

        private void Update()
        {
            UpdateVignetteBasedOnHealth();
        }

        private void UpdateVignetteBasedOnHealth()
        {
            float currentResourcesNormalized = _resourcesManager.CurrentResources/_resourcesManager.MaxResources;
            _globalVolume.profile.TryGet(out Vignette vignette);
                
            if (currentResourcesNormalized > _highHealthPercentage)
            {
                vignette.color.Override(_vignetteColorAtHighHealth.value);
                
                if (_vignetteLerpFadeIn is not null || _vignetteLerpFadeOut is not null) return;
                
                if (_vignetteLerpFadeIn is null && _isFadeOut)
                {
                    _isFadeOut = false;
                    _vignetteLerpFadeIn = StartCoroutine(VignetteLerpFadeIn(vignette, _vignetteMaxIntensityHigh));
                }
                if (_vignetteLerpFadeOut is null && _isFadeIn)
                {
                    _isFadeIn = false;
                    _vignetteLerpFadeOut = StartCoroutine(VignetteLerpFadeOut(vignette));
                }
            }
            else if (currentResourcesNormalized < _lowHealthPercentage)
            {
                vignette.color.Override(_vignetteColorAtLowHealth.value);
                
                if (_vignetteLerpFadeIn is not null || _vignetteLerpFadeOut is not null) return;
                
                if (_vignetteLerpFadeIn is null && _isFadeOut)
                {
                    _isFadeOut = false;
                    _vignetteLerpFadeIn = StartCoroutine(VignetteLerpFadeIn(vignette, _vignetteMaxIntensityLow));
                }
                if (_vignetteLerpFadeOut is null && _isFadeIn)
                {
                    _isFadeIn = false;
                    _vignetteLerpFadeOut = StartCoroutine(VignetteLerpFadeOut(vignette));
                }
            }
            else
            {
                vignette.color.Override(Color.white);
            }
        }

        private IEnumerator VignetteLerpFadeIn(Vignette vignette, float vignetteMaxIntensity)
        {
            _timer = 0;
            while (_timer < _lerpDurationFadeIn)
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignetteMaxIntensity, _timer/_lerpDurationFadeIn);
                _timer += Time.deltaTime;
                yield return null;
            }

            _isFadeIn = true;
            _vignetteLerpFadeIn = null;
        }
        
        private IEnumerator VignetteLerpFadeOut(Vignette vignette)
        {
            _timer = 0;
            while (_timer < _lerpDurationFadeOut)
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, _timer/_lerpDurationFadeOut);
                _timer += Time.deltaTime;
                yield return null;
            }

            _isFadeOut = true;
            _vignetteLerpFadeOut = null;
        }

        [SerializeField] private Volume _globalVolume;
        [Space] [Header("Vignette Effect")]
        [SerializeField] [Range(0,1)] private float _vignetteMaxIntensityLow = 0.2f;
        [SerializeField] [Range(0,1)] private float _vignetteMaxIntensityHigh = 0.2f;
        [SerializeField] private float _lerpDurationFadeIn = 1;
        [SerializeField] private float _lerpDurationFadeOut = 1;
        [SerializeField] [Range(0,1)] private float _lowHealthPercentage = 0.2f;
        [SerializeField] [Range(0,1)] private float _highHealthPercentage = 0.8f;
        [SerializeField] private ColorParameter _vignetteColorAtLowHealth;
        [SerializeField] private ColorParameter _vignetteColorAtHighHealth;

        private ResourcesManager _resourcesManager;
        private Coroutine _vignetteLerpFadeIn;
        private Coroutine _vignetteLerpFadeOut;
        private float _timer;
        private bool _isFadeIn;
        private bool _isFadeOut;
    }
}
