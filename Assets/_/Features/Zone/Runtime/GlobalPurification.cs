using System;
using System.Collections;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class GlobalPurification : MonoBehaviour
    {
        public static GlobalPurification Instance { get; private set; }

        public float CurrentPercentage
        {
            get => _currentPercentage;
            set => _currentPercentage = value;
        }

        public int NeededPercentageToWin
        {
            get => _neededPercentageToWin;
            set => _neededPercentageToWin = value;
        }

        public Action<float> m_onValueChange;
        public Action<float> m_onZonePurified;
        public Action m_onAreaPurified;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            m_onZonePurified += UpdateGlobalPurification;
        }

        private void UpdateGlobalPurification(float globalPurificationPercentage)
        {
            CurrentPercentage += globalPurificationPercentage;
            m_onValueChange?.Invoke(globalPurificationPercentage);
            if (CurrentPercentage < NeededPercentageToWin || _isAreaPurified) return;

            _isAreaPurified = true;
            _fogRevelerPrefab.transform.SetPositionAndRotation(_doorTransform.position, Quaternion.identity);
            m_onAreaPurified?.Invoke();
            StartCoroutine(WaitForCutScene());
        }

        private IEnumerator WaitForCutScene()
        {
            yield return new WaitForSeconds(2f);
            _finalDoorAnimator.Play("OpenDoor");
            _doorLightRenderer.materials[0].renderQueue = 0;
        }

        [SerializeField] private int _neededPercentageToWin;
        [SerializeField] private GameObject _fogRevelerPrefab;
        [Header("Final Door Info")]
        [SerializeField] private Transform _doorTransform;
        [SerializeField] private Animator _finalDoorAnimator;
        [SerializeField] private MeshRenderer _doorLightRenderer;

        private float _currentPercentage;
        private bool _isAreaPurified;
    }
}
