using UnityEngine;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    public class HexStackBoard : MonoBehaviour {
        [Header("Stack Positions")]
        [SerializeField] private Transform _leftPosition;
        [SerializeField] private Transform _middlePosition;
        [SerializeField] private Transform _rightPosition;
        
        [Inject] private readonly HexStackFactory _stackFactory;
        
        private void Start() {
            CreateStacks();
        }
        
        private void CreateStacks() {
            if (_stackFactory == null) {
                Debug.LogError("HexStackFactory is not initialized!");
                return;
            }
            
            if (!ValidateTransforms()) {
                return;
            }
            
            // Create stack at left position
            _stackFactory.CreateRandomStack(_leftPosition, _leftPosition.position);
            
            // Create stack at middle position
            _stackFactory.CreateRandomStack(_middlePosition, _middlePosition.position);
            
            // Create stack at right position
            _stackFactory.CreateRandomStack(_rightPosition, _rightPosition.position);
        }
        
        /// <summary>
        /// Validates that all required transforms are assigned.
        /// </summary>
        /// <returns>True if all transforms are valid, false otherwise.</returns>
        private bool ValidateTransforms() {
            if (_leftPosition == null) {
                Debug.LogError("Left position transform is not assigned!");
                return false;
            }
            
            if (_middlePosition == null) {
                Debug.LogError("Middle position transform is not assigned!");
                return false;
            }
            
            if (_rightPosition == null) {
                Debug.LogError("Right position transform is not assigned!");
                return false;
            }
            
            return true;
        }
    }
}

