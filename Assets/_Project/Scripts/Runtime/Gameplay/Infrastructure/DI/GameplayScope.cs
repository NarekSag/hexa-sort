using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Config;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private HexGridConfig _hexGridConfig;
        [SerializeField] private HexAnimationConfig _hexagonAnimationConfig;
        [SerializeField] private HexStackConfig _hexStackConfig;
        [SerializeField] private HexStackBoard _hexStackBoard;
        [SerializeField] private InputInstaller _inputInstaller;

        protected override void Configure(IContainerBuilder builder)
        {
            try
            {
                RegisterComponents(builder);
                RegisterServices(builder);
                RegisterInstallers(builder);

                builder.RegisterEntryPoint<GameplayFlow>();
            }
            catch (System.Exception ex)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayScope Configuration Failed: {ex.Message}");
            }
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<HexGridFactory>(Lifetime.Scoped);
            builder.Register<HexStackFactory>(Lifetime.Scoped);
        }

        private void RegisterComponents(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexGridConfig);
            builder.RegisterComponent(_hexagonAnimationConfig);
            builder.RegisterComponent(_hexStackConfig);
            builder.RegisterComponent(_hexStackBoard);
        }

        private void RegisterInstallers(IContainerBuilder builder)
        {
            _inputInstaller.Install(builder);
        }
    }
}

