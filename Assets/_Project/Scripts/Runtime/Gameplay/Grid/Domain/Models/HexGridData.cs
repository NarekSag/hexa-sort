using System.Collections.Generic;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models {
    public class HexGridData {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CellCount => Width * Height;

        private readonly Dictionary<HexCoordinates, HexCoordinates> _validCoordinates;

        public HexGridData(int width, int height) {
            Width = width;
            Height = height;
            _validCoordinates = new Dictionary<HexCoordinates, HexCoordinates>();
            InitializeValidCoordinates();
        }

        private void InitializeValidCoordinates() {
            for (int z = 0; z < Height; z++) {
                for (int x = 0; x < Width; x++) {
                    HexCoordinates coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
                    _validCoordinates[coordinates] = coordinates;
                }
            }
        }

        public bool IsValidCoordinate(HexCoordinates coordinates) {
            return _validCoordinates.ContainsKey(coordinates);
        }

        public IEnumerable<HexCoordinates> GetAllCoordinates() {
            return _validCoordinates.Keys;
        }
    }
}

