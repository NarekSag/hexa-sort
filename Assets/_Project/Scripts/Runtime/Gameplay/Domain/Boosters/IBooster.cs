using System;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Boosters
{
    public interface IBooster
    {
        int Id { get; }
        BoosterType Type { get; }
        string Name { get; }
        string Description { get; }

        bool CanUse();
        void Use();
        void Cancel();

        event Action<IBooster> OnBoosterUsed;
    }
}

