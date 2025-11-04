using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services {
    public class StackPositionService {
        private readonly StackColliderService _colliderService;

        public StackPositionService(StackColliderService colliderService) {
            _colliderService = colliderService;
        }

        public void RepositionAllCells(IList<ICell> cells, int? excludeFromIndex = null) {
            if (cells == null) {
                return;
            }

            for (int i = 0; i < cells.Count; i++) {
                if (cells[i] != null) {
                    // Skip repositioning if this index should be excluded (being animated)
                    if (excludeFromIndex.HasValue && i >= excludeFromIndex.Value) {
                        continue;
                    }
                    
                    float yOffset = _colliderService.CalculateYOffset(i);
                    cells[i].LocalPosition = new Vector3(0f, yOffset, 0f);
                }
            }
        }

        // Legacy method - kept for backward compatibility during transition
        public void RepositionAllHexagons(IList<ICell> cells, int? excludeFromIndex = null) {
            if (cells == null) {
                return;
            }

            foreach (var hex in cells) {
                if (hex != null) {
                    cells.Add(hex);
                }
            }
            
            RepositionAllCells(cells, excludeFromIndex);
        }
    }
}
