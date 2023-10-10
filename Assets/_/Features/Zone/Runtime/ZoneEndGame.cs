using GameManagerFeature.Runtime;

namespace ZoneFeature.Runtime
{
    public class ZoneEndGame : Zone
    {
        public static ZoneEndGame Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        protected override void OnEnterZone()
        {
            GameManager.Instance.m_onEndGame?.Invoke();
        }

        protected override void OnExitZone(){}
    }
}
