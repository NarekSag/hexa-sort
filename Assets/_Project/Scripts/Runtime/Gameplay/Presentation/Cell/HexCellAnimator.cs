using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Config;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Cell {
    /// <summary>
    /// Handles animation for a single hex cell.
    /// Each cell has its own animator component that manages its animations.
    /// </summary>
    public class HexCellAnimator : MonoBehaviour {
        private HexAnimationConfig _config;
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes the animator with animation configuration.
        /// Should be called when the cell is created.
        /// </summary>
        public void Initialize(HexAnimationConfig config) {
            if (config == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "HexAnimationConfig is null in HexCellAnimator.Initialize");
                return;
            }
            
            _config = config;
            _isInitialized = true;
        }

        /// <summary>
        /// Gets the base delay for animations.
        /// </summary>
        public float BaseDelay => _config?.BaseDelay ?? 0f;

        /// <summary>
        /// Gets the stagger delay between multiple animations.
        /// </summary>
        public float StaggerDelay => _config?.StaggerDelay ?? 0.15f;

        /// <summary>
        /// Animates this cell's merge from source position to destination position.
        /// </summary>
        public async UniTask AnimateMerge(Vector3 sourcePosition, Vector3 destinationPosition, Vector3 flipAxis, float delay = 0f) {
            if (!_isInitialized || _config == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "HexCellAnimator not initialized or config is null");
                return;
            }

            if (transform == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Transform is null in AnimateMerge");
                return;
            }

            // Store original position and rotation
            transform.position = sourcePosition;
            Quaternion originalRotation = transform.rotation;
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
            transform.DORotate(targetEuler, _config.RotationDuration, RotateMode.FastBeyond360)
                .SetDelay(delay)
                .SetEase(_config.JumpEase)
                .OnComplete(() => {
                    // Ensure rotation returns to original
                    transform.rotation = originalRotation;
                });

            // Create jump animation (this is the main animation we wait for)
            var jumpTween = transform.DOJump(
                destinationPosition,
                _config.JumpPower,
                0, // Number of jumps (0 = smooth arc)
                _config.JumpDuration
            ).SetDelay(delay)
             .SetEase(_config.JumpEase)
             .OnComplete(() => {
                 // Ensure final position and rotation are exactly correct
                 transform.position = destinationPosition;
                 transform.rotation = originalRotation;
             });

            // Wait for jump animation to complete (it's the main movement animation)
            await UniTask.WaitUntil(() => !jumpTween.IsActive());
        }
    }
}

