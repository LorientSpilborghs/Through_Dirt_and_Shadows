using UnityEngine;

namespace ResourcesManagerFeature.Runtime
{
    public class Resources : MonoBehaviour
    {
        public enum ResourcesTypes
        {
            A,
            B,
            C
        }
        
        [SerializeField] private ResourcesTypes _resourcesTypes;
    }
}
