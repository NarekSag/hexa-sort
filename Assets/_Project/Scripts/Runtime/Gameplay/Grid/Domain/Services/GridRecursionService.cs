using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services {
    public class GridRecursionService {
        private const int MAX_RECURSION_DEPTH = 50;

        public bool ShouldContinueRecursion(HexCoordinates slotCoordinates, HashSet<HexCoordinates> visitedSlots, int depth) {
            if (depth > MAX_RECURSION_DEPTH) {
                return false;
            }

            if (visitedSlots.Contains(slotCoordinates)) {
                return false;
            }

            return true;
        }

        public HashSet<HexCoordinates> CreateVisitedSetForRecursion(
            HashSet<HexCoordinates> currentVisitedSet,
            HexCoordinates slotToRecheck,
            HashSet<HexCoordinates> slotsThatReceivedCells) {
            
            HashSet<HexCoordinates> newVisitedSet = new HashSet<HexCoordinates>(currentVisitedSet);
            
            // Remove the slot we're re-checking
            newVisitedSet.Remove(slotToRecheck);
            
            // Remove all slots that received cells, so they can be re-checked
            foreach (HexCoordinates receivedCoords in slotsThatReceivedCells) {
                newVisitedSet.Remove(receivedCoords);
            }

            return newVisitedSet;
        }
    }
}

