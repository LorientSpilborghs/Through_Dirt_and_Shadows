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
            // m_upcomingHealthSlider.value = totalUpcomingResources;

            if (m_greenHealthSlider.value > m_redHealthSlider.value)
            {
                m_redHealthSlider.value = m_greenHealthSlider.value;
            }
            
            if (_delayUpcomingHealthSlider is not null)
            {
                StopCoroutine(_delayUpcomingHealthSlider);
            }
            _delayUpcomingHealthSlider = StartCoroutine(DelayUpcomingHealthSlider(totalUpcomingResources));
            
            if (_delayRedHealthSlider is not null)
            {
                StopCoroutine(_delayRedHealthSlider);
            }
            _delayRedHealthSlider = StartCoroutine(DelayRedHealthSlider());
            // _text.text = $"{currentResources}";
        }
        
        private IEnumerator DelayUpcomingHealthSlider(float totalUpcomingResources)
        {
            _upcomingTimer = 0;
            while (m_upcomingHealthSlider.value < totalUpcomingResources)
            {
                m_upcomingHealthSlider.value = Mathf.Lerp(m_upcomingHealthSlider.value, totalUpcomingResources, _upcomingTimer/_upcomingHealthDelayDuration);
                _upcomingTimer += Time.deltaTime;
                yield return null;
            }

            _delayUpcomingHealthSlider = null;
        }

        private IEnumerator DelayRedHealthSlider()
        {
            _redTimer = 0;
            while (m_redHealthSlider.value > m_greenHealthSlider.value)
            {
                m_redHealthSlider.value = Mathf.Lerp(m_redHealthSlider.value, m_greenHealthSlider.value, _redTimer/_redHealthDelayDuration);
                _redTimer += Time.deltaTime;
                yield return null;
            }

            _delayRedHealthSlider = null;
        }

        
        
        [SerializeField] private float _redHealthDelayDuration = 10;
        [SerializeField] private float _upcomingHealthDelayDuration = 1;
        
        private Coroutine _delayRedHealthSlider;
        private Coroutine _delayUpcomingHealthSlider;
        private float _upcomingTimer;
        private float _redTimer;
        // private TextMeshProUGUI _text;
    }
}
