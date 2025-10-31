using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Scripts.Runtime.Utilities.Loading;

namespace _Project.Scripts.Runtime.Bootstrap.Units
{
    public class ApplicationConfigurationLoadUnit : ILoadUnit
    {
        public UniTask Load()
        {
            // Load application configuration
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            SetScreenOrientation();
            
            return UniTask.CompletedTask;
        }

        private void SetScreenOrientation()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }
    }
}
