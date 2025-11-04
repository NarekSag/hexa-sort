using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Models;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Grid.Models {
    /// <summary>
    /// Maps between grid coordinates, offset coordinates, and world positions.
    /// </summary>
    public class HexGridMapper : IHexGridMapper {
        private readonly int _width;
        private readonly int _height;

        public HexGridMapper(int width, int height) {
            _width = width;
            _height = height;
        }

        public bool IsValidCoordinate(int x, int z) {
            return x >= 0 && x < _width && z >= 0 && z < _height;
        }

        public HexCoordinates GetCoordinateFromOffset(int x, int z) {
            return HexCoordinates.FromOffsetCoordinates(x, z);
        }

        public Vector3 GetCenteringOffset() {
            // Calculate grid bounds
            // Max X: rightmost cell position (accounting for odd row offset)
            float maxX = ((_width - 1) + (_height - 1) * 0.5f - (_height - 1) / 2) * (HexMetrics.InnerRadius * 2f);
            float minX = 0f;
            
            // Max Z: topmost cell position
            float maxZ = (_height - 1) * (HexMetrics.OuterRadius * 1.5f);
            float minZ = 0f;
            
            // Calculate center offset (negative to shift grid to center)
            float centerOffsetX = -(minX + maxX) * 0.5f;
            float centerOffsetZ = -(minZ + maxZ) * 0.5f;
            
            return new Vector3(centerOffsetX, 0f, centerOffsetZ);
        }

        public Vector3 GetWorldPositionFromOffset(int offsetX, int offsetZ) {
            float positionX = (offsetX + offsetZ * 0.5f - offsetZ / 2) * (HexMetrics.InnerRadius * 2f);
            float positionZ = offsetZ * (HexMetrics.OuterRadius * 1.5f);
            
            Vector3 centeringOffset = GetCenteringOffset();
            
            return new Vector3(positionX + centeringOffset.x, 0f, positionZ + centeringOffset.z);
        }

        public Vector3 GetWorldPosition(HexCoordinates coordinates) {
            int x = coordinates.X;
            int z = coordinates.Z;
            
            float offset = (z % 2) * 0.5f;
            float positionX = (x + offset) * (HexMetrics.InnerRadius * 2f);
            float positionZ = z * (HexMetrics.OuterRadius * 1.5f);
            
            return new Vector3(positionX, 0f, positionZ);
        }

        public HexCoordinates GetCoordinateFromWorldPosition(Vector3 worldPosition) {
            float x = worldPosition.x / (HexMetrics.InnerRadius * 2f);
            float z = worldPosition.z / (HexMetrics.OuterRadius * 1.5f);
            
            int q = Mathf.RoundToInt(x);
            int r = Mathf.RoundToInt(-x - z);
            int s = Mathf.RoundToInt(z);
            
            return new HexCoordinates(q, s);
        }
    }
}

