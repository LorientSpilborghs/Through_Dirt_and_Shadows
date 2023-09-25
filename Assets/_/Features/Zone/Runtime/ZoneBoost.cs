namespace ZoneFeature.Runtime
{
    public class ZoneBoost : Zone
    {
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        private void Start()
        {
            _growRoot = GetComponentInChildren<GrowRoot>();
        }

        protected override void OnEnterZone()
        {
            IsActive = true;
            _growRoot.StartGrowRoot();
        }
        protected override void OnExitZone() {}


        private GrowRoot _growRoot;
        private bool _isActive;
    }
}
