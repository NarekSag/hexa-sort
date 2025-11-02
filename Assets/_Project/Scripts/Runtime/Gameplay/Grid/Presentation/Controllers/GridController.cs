using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Stack.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers {
    public class GridController {
        private readonly HexGrid _grid;
        private readonly IHexGridMapper _mapper;
        private readonly StackController _stackController;

        public GridController(HexGrid grid, IHexGridMapper mapper, StackController stackController) {
            _grid = grid;
            _mapper = mapper;
            _stackController = stackController;
        }

        public void CheckNeighborsAndMerge(HexCoordinates slotCoordinates) {
            if (_grid == null) {
                return;
            }

            HexStackSlot slot = _grid.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return;
            }

            HexCoordinates[] neighbors = slotCoordinates.GetNeighbors();
            foreach (HexCoordinates neighborCoords in neighbors) {
                HexStackSlot neighborSlot = _grid.GetSlot(neighborCoords);
                if (neighborSlot != null && !neighborSlot.IsEmpty()) {
                    // Merge neighbor's stacks into this slot
                    _stackController.MergeSlots(slot, neighborSlot);
                }
            }
        }

        public bool IsValidCoordinate(HexCoordinates coordinates) {
            return _mapper != null && _mapper.IsValidCoordinate(coordinates);
        }

        public HexStackSlot GetSlot(HexCoordinates coordinates) {
            return _grid?.GetSlot(coordinates);
        }

        public HexCoordinates[] GetNeighbors(HexCoordinates coordinates) {
            return coordinates.GetNeighbors();
        }
    }
}

