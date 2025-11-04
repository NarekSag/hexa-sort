using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack {
    public class StackSpawnPoint : MonoBehaviour {
        private HexStackFactory _stackFactory;
        private IStack _currentStack;
        
        public void Initialize(HexStackFactory stackFactory) {
            _stackFactory = stackFactory;
            CreateStack();
        }
        
        private void OnDestroy() {
            UnsubscribeFromStack();
        }
        
        public void CreateStack() {
            if (_stackFactory == null) {
                Debug.LogError($"StackFactory is not initialized for spawn point at {transform.name}!");
                return;
            }
            
            UnsubscribeFromStack();
            
            _currentStack = _stackFactory.CreateRandomStack(transform, transform.position);
            if (_currentStack != null) {
                _currentStack.OnPlaced += OnStackPlaced;
            }
        }
        
        private void OnStackPlaced(IStack placedStack) {
            if (placedStack == _currentStack) {
                UnsubscribeFromStack();
                CreateStack();
            }
        }
        
        private void UnsubscribeFromStack() {
            if (_currentStack != null) {
                _currentStack.OnPlaced -= OnStackPlaced;
                _currentStack = null;
            }
        }
    }
}

