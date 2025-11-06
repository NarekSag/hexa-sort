using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Boosters
{
    public class BoosterManager
    {
        private readonly Dictionary<int, IBooster> _boosters = new Dictionary<int, IBooster>();
        private IBooster _activeBooster;
        private readonly Subject<Unit> _boostersReset = new Subject<Unit>();
        
        public IBooster ActiveBooster => _activeBooster;
        public IObservable<Unit> OnBoostersReset => _boostersReset;
        
        public void RegisterBooster(IBooster booster)
        {
            if (booster == null)
            {
                return;
            }
            
            _boosters[booster.Id] = booster;
        }
        
        public IBooster GetBooster(int id)
        {
            _boosters.TryGetValue(id, out IBooster booster);
            return booster;
        }
        
        public IEnumerable<IBooster> GetAvailableBoosters()
        {
            return _boosters.Values.Where(b => b.CanUse());
        }
        
        public void ResetUsageForLevel()
        {
            foreach (var booster in _boosters.Values)
            {
                // Reset usage for boosters that support it
                if (booster is HammerBooster hammerBooster)
                {
                    hammerBooster.ResetUsage();
                }
                // Add other booster types here as needed
            }
            
            // Clear any active booster
            ClearActiveBooster();
            
            // Notify that boosters have been reset
            _boostersReset.OnNext(Unit.Default);
        }
        
        public void SetActiveBooster(IBooster booster)
        {
            _activeBooster = booster;
        }
        
        public void ClearActiveBooster()
        {
            if (_activeBooster != null)
            {
                _activeBooster.Cancel();
                _activeBooster = null;
            }
        }
        
        public void MarkBoosterAsUsed(int id)
        {
            var booster = GetBooster(id);
            if (booster == null)
            {
                return;
            }
            
            // Mark as used for boosters that support it
            if (booster is HammerBooster hammerBooster)
            {
                hammerBooster.MarkAsUsed();
            }
            // Add other booster types here as needed
        }
    }
}

