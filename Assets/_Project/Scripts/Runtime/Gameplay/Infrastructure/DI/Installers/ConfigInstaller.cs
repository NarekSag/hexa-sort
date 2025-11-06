
using UnityEngine;
using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class ConfigInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private HexStackConfig _hexStackConfig;
        [SerializeField] private LevelProgressionConfig _levelProgressionConfig;
        [SerializeField] private BoosterUnlockConfig _boosterUnlockConfig;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexStackConfig);
            builder.RegisterComponent(_levelProgressionConfig);
            builder.RegisterComponent(_levelProgressionConfig.SlotPrefab);
            builder.RegisterComponent(_boosterUnlockConfig);
        }
    }
}
