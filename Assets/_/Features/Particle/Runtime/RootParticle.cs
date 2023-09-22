using System.Collections;
using System.Collections.Generic;
using RootFeature.Runtime;
using UnityEngine;

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
            _root.m_onGrow += OnGrowEventHandler;
            foreach (var particlePreset in _particleSystem)
            {
                Instantiate(particlePreset.m_particleSystem, 
                    new Vector3(transform.position.x, particlePreset.m_height, transform.position.z),
                    Quaternion.identity, transform);
            }
        }

        private void OnGrowEventHandler()
        {
            if (_isPlayingParticle) return;
            
            foreach (var particlePreset in _particleSystem)
            {
                particlePreset.m_particleSystem.Play();
                particlePreset.m_particleSystem.transform.position = _root.Container.Spline[^1].Position;
            }
            StartCoroutine(WaitForAnimationToEnd());
        }

        private IEnumerator WaitForAnimationToEnd()
        {
            yield return new WaitForSeconds(_particleSystem[^1].m_particleSystem.main.duration);
            _isPlayingParticle = false;
        } 

        [SerializeField] private ParticlePreset[] _particleSystem;

        private RootV2 _root;
        private bool _isPlayingParticle;
    }
}
