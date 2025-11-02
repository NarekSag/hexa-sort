using UnityEngine;
using System;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models {
    [System.Serializable]
    public struct HexCoordinates : IEquatable<HexCoordinates>
    {
        [SerializeField]
        private int _x;
        
        [SerializeField]
        private int _z;

        public int X => _x;

        public int Z => _z;

        public HexCoordinates(int x, int z) {
            _x = x;
            _z = z;
        }

        public int Y => -X - Z;

        public static HexCoordinates FromOffsetCoordinates(int x, int z) {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString() {
            return $"({X}, {Z})";
        }

        public bool Equals(HexCoordinates other) {
            return _x == other._x && _z == other._z;
        }

        public override bool Equals(object obj) {
            return obj is HexCoordinates other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_x, _z);
        }

        public static bool operator ==(HexCoordinates left, HexCoordinates right) {
            return left.Equals(right);
        }

        public static bool operator !=(HexCoordinates left, HexCoordinates right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets all 6 neighbor coordinates in a hexagonal grid.
        /// </summary>
        public HexCoordinates[] GetNeighbors() {
            return new HexCoordinates[] {
                new HexCoordinates(X + 1, Z),      // East
                new HexCoordinates(X + 1, Z - 1), // Northeast
                new HexCoordinates(X, Z - 1),      // Northwest
                new HexCoordinates(X - 1, Z),      // West
                new HexCoordinates(X - 1, Z + 1),  // Southwest
                new HexCoordinates(X, Z + 1)       // Southeast
            };
        }
    }
}