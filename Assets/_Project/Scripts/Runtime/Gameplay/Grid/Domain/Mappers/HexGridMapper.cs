using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers {
    public class HexGridMapper : IHexGridMapper {
        public HexGridData GridData { get; private set; }

        public HexGridMapper(int width, int height) {
            GridData = new HexGridData(width, height);
        }

        public bool IsValidCoordinate(HexCoordinates coordinates) {
            return GridData.IsValidCoordinate(coordinates);
        }

        public HexCoordinates GetCoordinateFromOffset(int x, int z) {
            return HexCoordinates.FromOffsetCoordinates(x, z);
        }

        public Vector3 GetCenteringOffset() {
            int width = GridData.Width;
            int height = GridData.Height;
            
            // Calculate grid bounds
            // Max X: rightmost cell position (accounting for odd row offset)
            float maxX = ((width - 1) + (height - 1) * 0.5f - (height - 1) / 2) * (HexMetrics.InnerRadius * 2f);
            float minX = 0f;
            
            // Max Z: topmost cell position
            float maxZ = (height - 1) * (HexMetrics.OuterRadius * 1.5f);
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

