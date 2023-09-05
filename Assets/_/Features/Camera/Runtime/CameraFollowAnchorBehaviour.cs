using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraFollowAnchorBehaviour : MonoBehaviour
    {
        private void Start()
        {
            PlayerRuntime.PlayerV2.Instance.m_onInterpolate += FollowInterpolatingKnot;
        }

        private void FollowInterpolatingKnot(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}
