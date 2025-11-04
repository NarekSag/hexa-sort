using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain {
    /// <summary>
    /// Simple registry for looking up slots by coordinates.
    /// Replaces the bloated HexGrid class.
    /// </summary>
    public class HexSlotRegistry {
        private readonly Dictionary<HexCoordinates, ISlot> _slots = new Dictionary<HexCoordinates, ISlot>();

        public void Register(HexCoordinates coordinates, ISlot slot) {
            _slots[coordinates] = slot;
        }

        public ISlot GetSlot(HexCoordinates coordinates) {
            _slots.TryGetValue(coordinates, out ISlot slot);
            return slot;
        }

        public void Clear() {
            _slots.Clear();
        }
    }
}

