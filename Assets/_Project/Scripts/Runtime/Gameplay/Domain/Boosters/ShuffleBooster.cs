using System;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Boosters
{
    public class ShuffleBooster : IBooster
    {
        private int _usageRemaining = 1;

        public int Id => (int)BoosterType.Shuffle;
        public BoosterType Type => BoosterType.Shuffle;
        public string Name => "Shuffle";
        public string Description => "Shuffle all stacks and get 3 new ones";

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

