using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIFeature.Runtime
{
    public class HealthUI : MonoBehaviour
    {
        public Slider m_greenHealthSlider;
        public Slider m_redHealthSlider;
        public Slider m_upcomingHealthSlider;
        
        public void SetHealth(float currentResources, float maxResources)
        {
            m_greenHealthSlider.maxValue = maxResources;
            m_greenHealthSlider.value = currentResources;
            m_upcomingHealthSlider.maxValue = maxResources;
            m_upcomingHealthSlider.value = currentResources;            
            m_redHealthSlider.maxValue = maxResources;
            m_redHealthSlider.value = currentResources;
            // _text.text = $"{currentResources}";
        }

        public void UpdateHealth(float currentResources, float totalUpcomingResources)
        {
            m_greenHealthSlider.value = currentResources;
            m_upcomingHealthSlider.value = totalUpcomingResources;

            if (m_greenHealthSlider.value > m_redHealthSlider.value)
            {
                m_redHealthSlider.value = m_greenHealthSlider.value;
            }
            
            if (_delayRedHealthSlider is not null)
            {
                StopCoroutine(_delayRedHealthSlider);
            }
            _delayRedHealthSlider = StartCoroutine(DelayRedHealthSlider());
            // _text.text = $"{currentResources}";
        }

        private IEnumerator DelayRedHealthSlider()
        {
            _timer = 0;
            while (m_redHealthSlider.value > m_greenHealthSlider.value)
            {
                m_redHealthSlider.value = Mathf.Lerp(m_redHealthSlider.value, m_greenHealthSlider.value, _timer/_redHealthDelayDuration);
                _timer += Time.deltaTime;
                yield return null;
            }

            _delayRedHealthSlider = null;
        }

        
        
        [SerializeField] private float _redHealthDelayDuration = 10;
        
        private Coroutine _delayRedHealthSlider;
        private float _timer;
        // private TextMeshProUGUI _text;
    }
}
