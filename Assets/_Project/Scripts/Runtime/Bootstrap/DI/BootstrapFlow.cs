using System;
using VContainer.Unity;
using _Project.Scripts.Runtime.Bootstrap.Units;
using _Project.Scripts.Runtime.Utilities.Loading;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Bootstrap.DI
{
    public class BootstrapFlow : IStartable
    {
        private readonly LoadingService _loadingService;

        public BootstrapFlow(LoadingService loadingService)
        {
            _loadingService = loadingService;
        }
        
        public async void Start()
        {
            try
            {
                await _loadingService.BeginLoading(new ApplicationConfigurationLoadUnit());
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Bootstrap, $"BootstrapFlow Start Failed: {e.Message}");
            }
        }
    }
}
