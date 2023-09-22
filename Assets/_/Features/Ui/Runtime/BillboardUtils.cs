using UnityEngine;

namespace UIFeature.Runtime
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
