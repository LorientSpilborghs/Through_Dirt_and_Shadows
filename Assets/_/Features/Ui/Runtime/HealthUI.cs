using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFeature.Runtime
{
    public class HealthUI : MonoBehaviour
    {
        private void Start()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _slider = GetComponent<Slider>();
        }

        public void SetHealth(float currentResources, float maxResources)
        {
            _slider.maxValue = maxResources;
            _slider.value = currentResources;
            _text.text = $"{currentResources}";
        }

        public void UpdateHealth(float currentResources)
        {
            _slider.value = currentResources;
            _text.text = $"{currentResources}";
        }
        
        
        private Slider _slider;
        private TextMeshProUGUI _text;
    }
}
