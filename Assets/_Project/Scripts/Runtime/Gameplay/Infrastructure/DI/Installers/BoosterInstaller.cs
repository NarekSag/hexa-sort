using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class BoosterInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Register state manager
            builder.Register<GameplayStateManager>(Lifetime.Singleton);

            // Register booster manager
            builder.Register<BoosterManager>(Lifetime.Singleton).AsSelf();

            // Register all boosters
            builder.Register<HammerBooster>(Lifetime.Singleton).AsSelf();
            builder.Register<ShuffleBooster>(Lifetime.Singleton).AsSelf();

            // Configure booster manager to register all boosters
            builder.RegisterBuildCallback(resolver =>
            {
                var boosterManager = resolver.Resolve<BoosterManager>();
                var hammerBooster = resolver.Resolve<HammerBooster>();
                var shuffleBooster = resolver.Resolve<ShuffleBooster>();
                boosterManager.RegisterBooster(hammerBooster);
                boosterManager.RegisterBooster(shuffleBooster);
            });
        }
    }
}

