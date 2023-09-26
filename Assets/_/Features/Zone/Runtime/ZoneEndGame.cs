using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneEndGame : Zone
    {
        protected override void OnEnterZone()
        {
            Debug.Log("chouette");
        }

        protected override void OnExitZone(){}
    }
}
