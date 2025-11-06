using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces
{
    public interface IDraggable
    {
        void SetPosition(Vector3 position);
        Vector3 GetPosition();
        bool CanBeDragged();
    }
}

