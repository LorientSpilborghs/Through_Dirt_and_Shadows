using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraFollowAnchorBehaviour : MonoBehaviour
    {
        private void Start()
        {
            PlayerRuntime.Player.Instance.m_onInterpolate += FollowInterpolatingKnot;
        }

        private void FollowInterpolatingKnot(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}
