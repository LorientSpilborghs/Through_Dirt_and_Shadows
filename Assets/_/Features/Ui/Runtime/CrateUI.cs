using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class CrateUI : MonoBehaviour
    {
        public Slider m_slider;
   
        private void Start()
        {
            _zoneHarvest = GetComponentInChildren<ZoneHarvestV2>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _zoneHarvest.m_onValueChange += OnValueChangeEventHandler;
            m_slider.maxValue = _zoneHarvest.BaseResources;
            m_slider.value = _zoneHarvest.BaseResources;
            _text.text = $"{_zoneHarvest.BaseResources} / {_zoneHarvest.BaseResources}";
        }

        private void OnDestroy()
        {
            _zoneHarvest.m_onValueChange -= OnValueChangeEventHandler;
        }

        private void OnValueChangeEventHandler()
        {
            m_slider.value = _zoneHarvest.CurrentResources;
            _text.text = $"{_zoneHarvest.CurrentResources} / {_zoneHarvest.BaseResources}";
        }

        private ZoneHarvestV2 _zoneHarvest;
        private TextMeshProUGUI _text;
    }
}
