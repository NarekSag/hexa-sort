using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers {
    public interface IHexGridMapper {
        HexGridData GridData { get; }
        bool IsValidCoordinate(HexCoordinates coordinates);
        HexCoordinates GetCoordinateFromOffset(int x, int z);
        Vector3 GetWorldPositionFromOffset(int offsetX, int offsetZ);
        Vector3 GetWorldPosition(HexCoordinates coordinates);
        HexCoordinates GetCoordinateFromWorldPosition(Vector3 worldPosition);
        Vector3 GetCenteringOffset();
    }
}

