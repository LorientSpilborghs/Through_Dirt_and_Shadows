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
            _greenHealthSlider = GetComponent<Slider>();
        }

        public void SetHealth(float currentResources, float maxResources)
        {
            _greenHealthSlider.maxValue = maxResources;
            _greenHealthSlider.value = currentResources;
            _text.text = $"{currentResources}";
        }

        public void UpdateHealth(float currentResources)
        {
            _greenHealthSlider.value = currentResources;
            _text.text = $"{currentResources}";
        }
        
        
        private Slider _greenHealthSlider;
        private TextMeshProUGUI _text;
        private Coroutine _delayRedSlider;
    }
}
