using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain;

namespace _Project.Scripts.Runtime.Gameplay.Stack.Controllers {
    public class StackController {
        private readonly StackMergeService _mergeService;
        private readonly StackPositionService _positionService;
        private readonly HexAnimationService _animationService;

        public StackController(
            StackMergeService mergeService,
            StackPositionService positionService,
            HexAnimationService animationService) {
            _mergeService = mergeService;
            _positionService = positionService;
            _animationService = animationService;
        }

        public void MergeStacks(IStack targetStack, IStack sourceStack, bool animate = true) {
            if (targetStack == null || sourceStack == null || targetStack == sourceStack) {
                return;
            }

            // Store the starting index for positioning
            int startingIndex = targetStack.Cells.Count;

            // Use the service to perform the merge
            IStack targetStackInterface = targetStack;
            IStack sourceStackInterface = sourceStack;
            
            _mergeService.MergeStacks(targetStackInterface, sourceStackInterface, animate, _animationService);

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

        public void MergeSlots(ISlot targetSlot, ISlot sourceSlot) {
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
                        bool animate = _animationService != null;
                        MergeStacks(targetStack, sourceStack, animate);

                        // Destroy the empty stack (with delay if animating)
                        if (sourceStack.Cells.Count == 0) {
                            if (_animationService != null) {
                                // Delay destruction to allow animation to complete
                                Object.Destroy(sourceStack.Transform.gameObject, 0.3f);
                            } else {
                                Object.Destroy(sourceStack.Transform.gameObject);
                            }
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

