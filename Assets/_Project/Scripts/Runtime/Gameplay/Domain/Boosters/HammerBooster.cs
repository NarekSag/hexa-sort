using System;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Boosters
{
    public class HammerBooster : IBooster
    {
        private int _usageRemaining = 1;
        
        public int Id => (int)BoosterType.Hammer;
        public BoosterType Type => BoosterType.Hammer;
        public string Name => "Hammer";
        public string Description => "Destroy any stack";
        
        public event Action<IBooster> OnBoosterUsed;
        
        public bool CanUse()
        {
            return _usageRemaining > 0;
        }
        
        public void Use()
        {
            if (!CanUse())
            {
                return;
            }
            
            OnBoosterUsed?.Invoke(this);
        }
        
        public void Cancel()
        {
            // No action needed on cancel
        }
        
        public void ResetUsage()
        {
            _usageRemaining = 1;
        }
        
        public void MarkAsUsed()
        {
            if (_usageRemaining > 0)
            {
                _usageRemaining--;
            }
        }
        
        public int GetUsageRemaining()
        {
            return _usageRemaining;
        }
    }
}

