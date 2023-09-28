using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    // public class GlobalPurificationUI : MonoBehaviour
    // {
    //     private void Start()
    //     {
    //         StartCoroutine(WaitForEndOfFrame());
    //         _globalPurification = GlobalPurification.Instance;
    //         _slider = GetComponent<Slider>();
    //         _canvasGroup = GetComponentInChildren<CanvasGroup>();
    //         _text = GetComponentInChildren<TextMeshProUGUI>();
    //         _globalPurification.m_onValueChange += OnValueChangeUpdateEventHandler;
    //         _slider.maxValue = 100;
    //         _slider.value = _globalPurification.CurrentPercentage;
    //     }
    //
    //     private void OnValueChangeUpdateEventHandler()
    //     {
    //         if (!_isEnable)
    //         {
    //             _canvasGroup.alpha = 1;
    //             _isEnable = true;
    //         }
    //         _slider.value = _globalPurification.CurrentPercentage;
    //         _text.text = $"{_globalPurification.CurrentPercentage} / {100}";
    //     }
    //
    //     private IEnumerator WaitForEndOfFrame()
    //     {
    //         yield return new WaitForEndOfFrame();
    //         Start();
    //     }
    //     
    //     private GlobalPurification _globalPurification;
    //     private Slider _slider;
    //     private TextMeshProUGUI _text;
    //     private CanvasGroup _canvasGroup;
    //     private bool _isEnable;
    // }
}
