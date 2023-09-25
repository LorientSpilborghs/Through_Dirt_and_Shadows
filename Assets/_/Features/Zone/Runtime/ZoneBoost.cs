namespace ZoneFeature.Runtime
{
    public class ZoneBoost : Zone
    {
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        
        protected override void OnEnterZone()
        {
            IsActive = true;
        }
        protected override void OnExitZone() {}

        private bool _isActive;
    }
}
