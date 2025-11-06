using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot
{
    public class HexSlotRegistry
    {
        private readonly Dictionary<HexCoordinates, ISlot> _slots = new Dictionary<HexCoordinates, ISlot>();

        public void Register(HexCoordinates coordinates, ISlot slot)
        {
            _slots[coordinates] = slot;
        }

        public ISlot GetSlot(HexCoordinates coordinates)
        {
            _slots.TryGetValue(coordinates, out ISlot slot);
            return slot;
        }

        public IEnumerable<ISlot> GetAllSlots()
        {
            return _slots.Values;
        }

        public void Clear()
        {
            _slots.Clear();
        }
    }
}

