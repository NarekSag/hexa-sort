using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Models;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces
{
    public interface ICell
    {
        Transform Transform { get; }
        Vector3 LocalPosition { get; set; }
        ColorType ColorType { get; }
        void Initialize(ColorType colorType, int index);
    }
}

