using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Animation {
    /// <summary>
    /// Service for animating hexagon movements during stack merging.
    /// Follows Dependency Inversion Principle (SOLID).
    /// </summary>
    public interface IHexagonAnimationService {
        /// <summary>
        /// Animates a hexagon from source position to destination position with jump and rotation.
        /// </summary>
        /// <param name="hexagon">The hexagon transform to animate</param>
        /// <param name="sourcePosition">Starting world position</param>
        /// <param name="destinationPosition">Target world position</param>
        /// <param name="flipAxis">Axis to flip around (Vector3.right for X axis, Vector3.forward for Z axis)</param>
        /// <param name="delay">Delay before starting the animation</param>
        void AnimateHexagonMerge(Transform hexagon, Vector3 sourcePosition, Vector3 destinationPosition, Vector3 flipAxis, float delay);

        /// <summary>
        /// Animates multiple hexagons from source stack to destination stack.
        /// </summary>
        /// <param name="hexagons">List of hexagon transforms to animate</param>
        /// <param name="sourceStackTransform">Transform of the source stack</param>
        /// <param name="destinationStackTransform">Transform of the destination stack</param>
                /// <param name="targetLocalPositions">List of target local positions for each hexagon</param>
                /// <param name="baseDelay">Base delay before starting animations (staggered per hexagon). If null, uses config default.</param>
                /// <param name="staggerDelay">Delay between each hexagon animation. If null, uses config default.</param>
                void AnimateHexagonStackMerge(
                    List<Transform> hexagons,
                    Transform sourceStackTransform,
                    Transform destinationStackTransform,
                    List<Vector3> targetLocalPositions
                );
    }
}

