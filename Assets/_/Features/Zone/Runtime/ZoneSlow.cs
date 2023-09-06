using System;
using RootFeature.Runtime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneSlow : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            float? distancePerSeconds = other?.GetComponent<RootV2>().DistancePerSeconds;
        }
        
        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}
