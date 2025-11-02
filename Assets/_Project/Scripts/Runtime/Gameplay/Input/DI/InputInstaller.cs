using _Project.Scripts.Runtime.Gameplay.Input.Drag;
using UnityEngine;
using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation;
using _Project.Scripts.Runtime.Gameplay.Input.Raycast;

namespace _Project.Scripts.Runtime.Gameplay.Input.DI
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
