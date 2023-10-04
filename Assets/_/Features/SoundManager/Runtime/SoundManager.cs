using FMODUnity;
using ResourcesManagerFeature.Runtime;
using UnityEngine;

namespace SoundManagerFeature.Runtime
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            ResourcesManager.Instance.m_onChangeMaxHealthTier += OnChangeMaxHealthTierEventHandler;
        }

        private void OnChangeMaxHealthTierEventHandler(int arg1, float arg2)
        {
            RuntimeManager.PlayOneShot(_soundOnTierChange);
        }


        [SerializeField] private EventReference _soundOnTierChange;
    }
}
