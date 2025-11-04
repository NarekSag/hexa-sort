
using UnityEngine;
using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class ConfigInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private HexGridConfig _hexGridConfig;
        [SerializeField] private HexStackConfig _hexStackConfig;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(_hexGridConfig);
            builder.RegisterComponent(_hexStackConfig);
        }
    }
}
