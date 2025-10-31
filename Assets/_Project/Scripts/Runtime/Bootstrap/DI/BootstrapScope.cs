using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Utilities.Loading;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Bootstrap.DI
{
    public class BootstrapScope : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            try
            {
                RegisterServices(builder);
                
                builder.RegisterEntryPoint<BootstrapFlow>();
            }
            catch (System.Exception ex)
            {
                CustomDebug.LogError(LogCategory.Bootstrap, $"BootstrapScope Configuration Failed: {ex.Message}");
            }
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<LoadingService>(Lifetime.Singleton);
        }
    }
}
