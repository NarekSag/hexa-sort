using UnityEngine;
using System.Collections.Generic;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    public class HexStackBoard : MonoBehaviour {
        [Header("Stack Positions")]
        [SerializeField] private Transform _leftPosition;
        [SerializeField] private Transform _middlePosition;
        [SerializeField] private Transform _rightPosition;
        
        [Inject] private readonly HexStackFactory _stackFactory;
        
        private readonly Dictionary<HexStack, Transform> _trackedStacks = new Dictionary<HexStack, Transform>();
        
        private void Start() {
            CreateInitialStacks();
        }
        
        private void OnDestroy() {
            UnsubscribeFromAllStacks();
        }
        
        private void CreateInitialStacks() {
            if (_stackFactory == null) {
                Debug.LogError("HexStackFactory is not initialized!");
                return;
            }
            
            if (!ValidateTransforms()) {
                return;
            }
            
            CreateStackAt(_leftPosition);
            CreateStackAt(_middlePosition);
            CreateStackAt(_rightPosition);
        }
        
        private void CreateStackAt(Transform position) {
            HexStack stack = _stackFactory.CreateRandomStack(position, position.position);
            TrackStack(stack, position);
        }
        
        private void TrackStack(HexStack stack, Transform position) {
            _trackedStacks[stack] = position;
            stack.OnPlaced += OnStackPlaced;
        }
        
        private void OnStackPlaced(HexStack placedStack) {
            if (_trackedStacks.TryGetValue(placedStack, out Transform position)) {
                placedStack.OnPlaced -= OnStackPlaced;
                _trackedStacks.Remove(placedStack);
                
                CreateStackAt(position);
            }
        }
        
        private void UnsubscribeFromAllStacks() {
            foreach (var stack in _trackedStacks.Keys) {
                if (stack != null) {
                    stack.OnPlaced -= OnStackPlaced;
                }
            }
            _trackedStacks.Clear();
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

