using UnityEngine;

namespace ParticleFeature.Runtime
{
    [CreateAssetMenu(fileName = "New Particle", menuName = "ParticlePreset")]
    public class ParticlePreset : ScriptableObject
    {
        public ParticleSystem m_particleSystem;
        public float m_height;
    }
}
