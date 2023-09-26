using System;
using RootFeature.Runtime;
using UnityEngine;
using UnityEngine.VFX;

namespace ParticleFeature.Runtime
{
    public class RootParticle : MonoBehaviour
    {
        private void Awake()
        {
            _root = GetComponent<RootV2>();
        }

        private void Start()
        {
            _root.m_onStartGrow += OnStartGrowEventHandler;
            _root.m_onGrow += OnGrowEventHandler;
            _root.m_onEndGrow += OnEndGrowEventHandler;
            
            _visualEffect.SetFloat("Rate", 0);
        }

        private void OnStartGrowEventHandler()
        {
            _visualEffect.SetFloat("Rate", 32);
        }
        
        private void OnGrowEventHandler(Vector3 pos, bool isInterpolating)
        {
            _visualEffect.transform.position = _root.Container.Spline[^1].Position;
            Vector3 newDirection = new Vector3(pos.x, _visualEffect.transform.position.y, pos.z);
            _visualEffect.transform.LookAt(newDirection,Vector3.up);
        }
        
        private void OnEndGrowEventHandler()
        {
            _visualEffect.SetFloat("Rate", 0);
        }
        
        [SerializeField] private VisualEffect _visualEffect;
        private RootV2 _root;
        private bool _isPlayingParticle;
    }
}
