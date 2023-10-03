using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class GlobalPurificationUIV2 : MonoBehaviour
    {
        private void Start()
        {
            _globalPurification = GlobalPurification.Instance;
            _globalPurification.m_onValueChange += OnValueChangeUpdateEventHandler;
        }

        private void OnValueChangeUpdateEventHandler(float globalPurificationPercentage)
        {
            if (!_firstGaugeDone)
            {
                if (!FillGauge(_gauges[0], globalPurificationPercentage)) return;
                _radioactiveLight[0].gameObject.SetActive(false);
                _firstGaugeDone = true;
            }
            if (!_secondGaugeDone)
            {
                if (!FillGauge(_gauges[1], globalPurificationPercentage)) return;
                _radioactiveLight[1].gameObject.SetActive(false);
                _secondGaugeDone = true;
            }

            FillGauge(_gauges[2], globalPurificationPercentage);
            if (_gauges[^1].fillAmount <= 0)
            {
                _radioactiveLight[2].gameObject.SetActive(false);
            }
        }

        private bool FillGauge(Image image, float globalPurificationPercentage)
        {
            if (_savedAmount < 0)
            {
                image.fillAmount += _savedAmount;
                _savedAmount = 0;
                return false;
            }
            
            float newAmount = (globalPurificationPercentage / 100) * _gauges.Length;
            
            if (image.fillAmount - newAmount < 0)
            {
                _savedAmount = image.fillAmount - newAmount;
                image.fillAmount = 0;
                return true;
            }
            else
            {
                image.fillAmount -= newAmount;
                return false;
            }
        }
        
        
        [SerializeField] private Image[] _gauges;
        [SerializeField] private Image[] _radioactiveLight;
        
        private GlobalPurification _globalPurification;
        private bool _firstGaugeDone;
        private bool _secondGaugeDone;
        private bool _thirdGaugeDone;
        private float _savedAmount;
    }
}
