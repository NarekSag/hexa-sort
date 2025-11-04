using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers {
    /// <summary>
    /// Interface for mapping between grid coordinates and world positions.
    /// </summary>
    public interface IHexGridMapper {
        bool IsValidCoordinate(int x, int z);
        HexCoordinates GetCoordinateFromOffset(int x, int z);
        Vector3 GetWorldPositionFromOffset(int offsetX, int offsetZ);
        Vector3 GetWorldPosition(HexCoordinates coordinates);
        HexCoordinates GetCoordinateFromWorldPosition(Vector3 worldPosition);
        Vector3 GetCenteringOffset();
    }
}

