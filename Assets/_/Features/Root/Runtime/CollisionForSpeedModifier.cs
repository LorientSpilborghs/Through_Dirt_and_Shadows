using System;
using UnityEngine;

namespace RootFeature.Runtime
{
    public class CollisionForSpeedModifier : MonoBehaviour
    {
        public Action m_onEnterSlowZone;
        public Action m_onExitSlowZone;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag is not "Slow") return;
            m_onEnterSlowZone?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag is not "Slow") return;
            m_onExitSlowZone?.Invoke();
        }
    }
}
