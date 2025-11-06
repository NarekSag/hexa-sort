using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces
{
    public interface ISlot
    {
        IReadOnlyList<IStack> Stacks { get; }

        HexCoordinates GetCoordinates();

        void Initialize(HexCoordinates coordinates, GridController gridController);

        void SetStack(IStack hexStack, bool checkNeighbors = true);

        void ClearStacks();

        bool IsEmpty();

        bool CanAccept(IDraggable draggable, out Vector3 targetPosition);
    }
}

