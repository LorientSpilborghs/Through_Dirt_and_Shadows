using UnityEngine;
using UnityEngine.UI;

namespace UIFeature.Runtime
{
    public class HealthUI : MonoBehaviour
    {
        public Slider m_greenHealthSlider;
        public Slider m_upcomingHealthSlider;
        
        public void SetHealth(float currentResources, float maxResources)
        {
            m_greenHealthSlider.maxValue = maxResources;
            m_greenHealthSlider.value = currentResources;
            m_upcomingHealthSlider.maxValue = maxResources;
            m_upcomingHealthSlider.value = currentResources;
            // _text.text = $"{currentResources}";
        }

        public void UpdateHealth(float currentResources, float totalUpcomingResources)
        {
            m_greenHealthSlider.value = currentResources;
            m_upcomingHealthSlider.value = totalUpcomingResources;
            // _text.text = $"{currentResources}";
        }
        
        
        // private TextMeshProUGUI _text;
    }
}
