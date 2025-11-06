using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces
{
    public interface IStack
    {
        float Height { get; }
        IList<ICell> Cells { get; }
        Transform Transform { get; }
        event System.Action<IStack> OnPlaced;
        void Initialize();
        void UpdateColliderSize();
        float CalculateYOffset(int index);
        void RepositionAllCells(int? excludeFromIndex = null);
        void SetParent(Transform parent);
        void NotifyPlaced();
        void SetDraggable(bool draggable);
    }
}

