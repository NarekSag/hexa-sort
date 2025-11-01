using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;
using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.DI
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private HexGridConfig _hexGridConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            try
            {
                RegisterConfigs(builder);
                RegisterServices(builder);
                
                builder.RegisterEntryPoint<GameplayFlow>();
            }
            catch (System.Exception ex)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayScope Configuration Failed: {ex.Message}");
            }
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexGridConfig);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<HexGridFactory>(Lifetime.Scoped);
        }
    }
}

