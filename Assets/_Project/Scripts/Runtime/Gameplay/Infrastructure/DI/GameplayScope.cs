using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private HexStackBoard _hexStackBoard;
        [SerializeField] private InputInstaller _inputInstaller;
        [SerializeField] private ConfigInstaller _configInstaller;

        protected override void Configure(IContainerBuilder builder)
        {
            try
            {
                RegisterInstallers(builder);
                RegisterComponents(builder);
                RegisterFactories(builder);

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

        private void RegisterComponents(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexStackBoard);
        }

        private void RegisterInstallers(IContainerBuilder builder)
        {
            _inputInstaller.Install(builder);
            _configInstaller.Install(builder);
        }
    }
}

