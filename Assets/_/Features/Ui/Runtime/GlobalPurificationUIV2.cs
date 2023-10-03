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
                if (!FillGauge(_images[0], globalPurificationPercentage)) return;
                _firstGaugeDone = true;
            }
            if (!_secondGaugeDone)
            {
                if (!FillGauge(_images[1], globalPurificationPercentage)) return;
                _secondGaugeDone = true;
            }
            
            FillGauge(_images[2], globalPurificationPercentage);
        }

        private bool FillGauge(Image image, float globalPurificationPercentage)
        {
            if (_savedAmount < 0)
            {
                image.fillAmount += _savedAmount;
                _savedAmount = 0;
                return false;
            }
            
            float newAmount = (globalPurificationPercentage / 100) * _images.Length;
            
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
        
        
        
        [SerializeField] private Image[] _images;
        
        private GlobalPurification _globalPurification;
        private bool _firstGaugeDone;
        private bool _secondGaugeDone;
        private float _savedAmount;
    }
}
