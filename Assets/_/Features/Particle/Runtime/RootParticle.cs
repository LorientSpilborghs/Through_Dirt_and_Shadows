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
        }

        private void Update()
        {
            _particle.gameObject.SetActive(_root.IsGrowing);
        }

        private void OnGrowEventHandler(Vector3 pos)
        {
            _particle.transform.position = _root.Container.Spline[^1].Position;
            Vector3 newDirection = new Vector3(pos.x, _particle.transform.position.y, pos.z);
            _particle.transform.LookAt(newDirection,Vector3.up);
        }
        
        [SerializeField] private ParticleSystem _particle;
        private RootV2 _root;
        private bool _isPlayingParticle;
    }
}
