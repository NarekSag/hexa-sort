using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private HexStackBoard _hexStackBoard;
        [SerializeField] private InputInstaller _inputInstaller;
        [SerializeField] private ConfigInstaller _configInstaller;
        [SerializeField] private UIInstaller _uiInstaller;

        protected override void Configure(IContainerBuilder builder)
        {
            try
            {
                RegisterInstallers(builder);
                RegisterComponents(builder);
                RegisterFactories(builder);
                RegisterManagers(builder);
                RegisterStateAndBoosters(builder);
                RegisterInputServices(builder);

                builder.RegisterEntryPoint<GameplayFlow>();
            }
            catch (System.Exception ex)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayScope Configuration Failed: {ex.Message}");
            }
        }

        private void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<HexGridFactory>(Lifetime.Scoped);
            builder.Register<HexStackFactory>(Lifetime.Scoped);
        }
        
        private void RegisterManagers(IContainerBuilder builder)
        {
            builder.Register<LevelManager>(Lifetime.Singleton);
        }
        
        private void RegisterStateAndBoosters(IContainerBuilder builder)
        {
            // Register state manager
            builder.Register<GameplayStateManager>(Lifetime.Singleton);
            
            // Register booster manager
            builder.Register<BoosterManager>(Lifetime.Singleton).AsSelf();
            
            // Register hammer booster
            builder.Register<HammerBooster>(Lifetime.Singleton).AsSelf();
            
            // Configure booster manager to register hammer booster
            builder.RegisterBuildCallback(resolver =>
            {
                var boosterManager = resolver.Resolve<BoosterManager>();
                var hammerBooster = resolver.Resolve<HammerBooster>();
                boosterManager.RegisterBooster(hammerBooster);
            });
        }
        
        private void RegisterInputServices(IContainerBuilder builder)
        {
            builder.Register<BoosterInputService>(Lifetime.Scoped);
        }

        private void RegisterComponents(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexStackBoard);
        }

        private void RegisterInstallers(IContainerBuilder builder)
        {
            _inputInstaller.Install(builder);
            _configInstaller.Install(builder);
            _uiInstaller.Install(builder);
        }
    }
}

