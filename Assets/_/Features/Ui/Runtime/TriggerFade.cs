using UnityEngine;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class TriggerFade : MonoBehaviour
    {
        private void Start()
        {
            _zoneEndGame = ZoneEndGame.Instance;
            _zoneEndGame.m_onEnterZoneEndGame += Fade;
        }

        private void Fade() {}

        private ZoneEndGame _zoneEndGame;
    }
}
