using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack.Controllers {
    public class StackController {
        private readonly StackMergeService _mergeService;
        private readonly StackPositionService _positionService;

        public StackController(
            StackMergeService mergeService,
            StackPositionService positionService) {
            _mergeService = mergeService;
            _positionService = positionService;
        }

        public async UniTask MergeStacks(IStack targetStack, IStack sourceStack, bool animate = true) {
            if (targetStack == null || sourceStack == null || targetStack == sourceStack) {
                return;
            }

            // Store the starting index for positioning
            int startingIndex = targetStack.Cells.Count;

            // Use the service to perform the merge (now async)
            await _mergeService.MergeStacks(targetStack, sourceStack, animate);

            // Update collider sizes
            targetStack.UpdateColliderSize();
            if (sourceStack != null) {
                sourceStack.UpdateColliderSize();
            }

            // Reposition cells if not animating
            if (!animate) {
                _positionService.RepositionAllHexagons(targetStack.Cells);
            } else {
                _positionService.RepositionAllHexagons(targetStack.Cells, excludeFromIndex: startingIndex);
            }
        }

        public async UniTask MergeSlots(ISlot targetSlot, ISlot sourceSlot) {
            if (targetSlot == null || sourceSlot == null || targetSlot == sourceSlot) {
                return;
            }

            if (sourceSlot.IsEmpty()) {
                return;
            }

            var stacksToMerge = new List<IStack>(sourceSlot.Stacks);

            // If target slot has stacks, merge into the first one
            if (!targetSlot.IsEmpty()) {
                IStack targetStack = targetSlot.Stacks[0];

                foreach (IStack sourceStack in stacksToMerge) {
                    if (sourceStack != null) {
                        await MergeStacks(targetStack, sourceStack, animate: true);

                        // Destroy the empty stack (with delay to allow animation to complete)
                        if (sourceStack.Cells.Count == 0) {
                            Object.Destroy(sourceStack.Transform.gameObject, 0.3f);
                        }
                    }
                }
            } else {
                // If target slot is empty, move the entire stacks to target slot
                foreach (IStack stack in stacksToMerge) {
                    if (stack != null) {
                        // Add stack without triggering neighbor checks (since we're already merging)
                        targetSlot.SetStack(stack, checkNeighbors: false);
                    }
                }
            }

            // Clear the source slot
            sourceSlot.ClearStacks();
        }
    }
}

