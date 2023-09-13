using UnityEngine;

namespace UiFeature.Runtime
{
    public class BillboardUtils : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main == null) return;
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}
