using UnityEngine;

namespace ZoneFeature.Runtime
{
    [RequireComponent(typeof(Collider))]
    public abstract class Zone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            OnEnterZone();
        }

        private void OnTriggerExit(Collider other)
        {
            OnExitZone();
        }

        protected abstract void OnEnterZone();
        protected abstract void OnExitZone();
    }
}
