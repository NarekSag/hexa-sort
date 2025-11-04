using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Animation {
    public class HexAnimationService {
        private readonly HexAnimationConfig _config;

        public HexAnimationService(HexAnimationConfig config) {
            _config = config;
        }

        public async UniTask AnimateHexagonMerge(Transform hexagon, Vector3 sourcePosition, Vector3 destinationPosition, Vector3 flipAxis, float delay) {
            if (hexagon == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Hexagon transform is null in AnimateHexagonMerge");
                return;
            }

            // Store original position and rotation
            hexagon.position = sourcePosition;
            Quaternion originalRotation = hexagon.rotation;
            Vector3 originalEuler = originalRotation.eulerAngles;

            // Calculate target Euler angles by adding 180 degrees to the flip axis
            Vector3 targetEuler = originalEuler;
            if (Mathf.Approximately(flipAxis.x, 1f)) {
                // Flip on X axis (rotate around X)
                targetEuler.x += 180f;
            } else if (Mathf.Approximately(flipAxis.z, 1f)) {
                // Flip on Z axis (rotate around Z)
                targetEuler.z += 180f;
            }

            // Create rotation animation that flips 360 degrees around the axis and returns to original
            hexagon.transform.DORotate(targetEuler, _config.RotationDuration, RotateMode.FastBeyond360)
                .SetDelay(delay)
                .SetEase(_config.JumpEase)
                .OnComplete(() => {
                    // Ensure rotation returns to original
                    hexagon.rotation = originalRotation;
                });

            // Create jump animation (this is the main animation we wait for)
            var jumpTween = hexagon.transform.DOJump(
                destinationPosition,
                _config.JumpPower,
                0, // Number of jumps (0 = smooth arc)
                _config.JumpDuration
            ).SetDelay(delay)
             .SetEase(_config.JumpEase)
             .OnComplete(() => {
                 // Ensure final position and rotation are exactly correct
                 hexagon.position = destinationPosition;
                 hexagon.rotation = originalRotation;
             });

            // Wait for jump animation to complete (it's the main movement animation)
            await UniTask.WaitUntil(() => !jumpTween.IsActive());
        }

        public async UniTask AnimateHexagonStackMerge(
            List<Transform> hexagons,
            Transform sourceStackTransform,
            Transform destinationStackTransform,
            List<Vector3> targetLocalPositions
        ) {
            if (hexagons == null || hexagons.Count == 0) {
                CustomDebug.LogError(LogCategory.Gameplay, "Hexagons list is null or empty in AnimateHexagonStackMerge");
                return;
            }

            if (sourceStackTransform == null || destinationStackTransform == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Source or destination stack transform is null in AnimateHexagonStackMerge");
                return;
            }

            if (targetLocalPositions == null || targetLocalPositions.Count != hexagons.Count) {
                CustomDebug.LogError(LogCategory.Gameplay, "Target positions count doesn't match hexagons count in AnimateHexagonStackMerge");
                return;
            }

            // Create list of tasks for all animations
            var animationTasks = new List<UniTask>();

            for (int i = 0; i < hexagons.Count; i++) {
                Transform hexagon = hexagons[i];
                if (hexagon == null) continue;

                // Capture source world position and original rotation BEFORE changing parent
                Vector3 sourceWorldPosition = hexagon.position;
                Quaternion originalRotation = hexagon.rotation;
                
                Vector3 targetLocalPosition = targetLocalPositions[i];
                Vector3 destinationWorldPosition = destinationStackTransform.TransformPoint(targetLocalPosition);

                // Calculate direction from source to destination
                Vector3 direction = (destinationWorldPosition - sourceWorldPosition).normalized;
                
                // Determine flip axis based on direction (X or Z axis)
                // If movement is more along X axis, flip on Z axis (forward/backward flip)
                // If movement is more along Z axis, flip on X axis (left/right flip)
                bool flipOnZAxis = Mathf.Abs(direction.x) > Mathf.Abs(direction.z);
                Vector3 flipAxis = flipOnZAxis ? Vector3.forward : Vector3.right;

                // Calculate delay with stagger
                float delay = _config.BaseDelay + (i * _config.StaggerDelay);

                // Set parent immediately so local position calculations are correct
                hexagon.SetParent(destinationStackTransform);

                // Restore original rotation before animating (parent change might have affected it)
                hexagon.rotation = originalRotation;

                // Animate to the destination and add to task list
                animationTasks.Add(AnimateHexagonMerge(hexagon, sourceWorldPosition, destinationWorldPosition, flipAxis, delay));
            }

            // Wait for all animations to complete
            await UniTask.WhenAll(animationTasks);
        }
    }
}

