
using UnityEngine;
using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Drag;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.PositionCalculation;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class InputInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private DragService _dragService;

        public void Install(IContainerBuilder builder)
        {
            // Register InputActionAsset
            builder.RegisterComponent(_dragService).AsSelf();
            
            // Input Services
            builder.Register<IInputService, InputService>(Lifetime.Scoped);
            builder.Register<IRaycastService, RaycastService>(Lifetime.Scoped);
            builder.Register<IPositionCalculationService, PositionCalculationService>(Lifetime.Scoped);
        }
    }
}
