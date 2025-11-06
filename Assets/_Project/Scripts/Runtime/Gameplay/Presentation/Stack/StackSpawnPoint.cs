using System;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack
{
    public class StackSpawnPoint : MonoBehaviour
    {
        private HexStackFactory _stackFactory;
        private IStack _currentStack;
        private LevelData _currentLevelData;

        public event Action<StackSpawnPoint> OnStackPlaced;

        public bool HasStack => _currentStack != null;
        public bool IsEmpty => _currentStack == null;

        public void Initialize(HexStackFactory stackFactory, LevelData levelData = null)
        {
            _stackFactory = stackFactory;
            _currentLevelData = levelData;

            CreateStack();
        }

        public void SetLevelData(LevelData levelData)
        {
            _currentLevelData = levelData;
        }

        public void CreateStack()
        {
            if (_stackFactory == null)
            {
                CustomDebug.LogError(LogCategory.Gameplay,
                    $"StackSpawnPoint ({gameObject.name}): StackFactory is NULL!");
                return;
            }

            // Don't create if we already have a stack
            if (_currentStack != null)
            {
                CustomDebug.Log(LogCategory.Gameplay,
                    $"StackSpawnPoint ({gameObject.name}): Already has a stack, skipping creation");
                return;
            }

            _currentStack = _stackFactory.CreateRandomStack(transform, transform.position, _currentLevelData);

            if (_currentStack != null)
            {
                _currentStack.OnPlaced += OnCurrentStackPlaced;
            }
            else
            {
                CustomDebug.LogError(LogCategory.Gameplay,
                    $"StackSpawnPoint ({gameObject.name}): Factory returned NULL stack!");
            }
        }

        public void ClearStack()
        {
            if (_currentStack != null)
            {
                // Store reference before unsubscribing (which sets _currentStack to null)
                var stackToDestroy = _currentStack;
                UnsubscribeFromStack();

                if (stackToDestroy != null && stackToDestroy.Transform.gameObject != null)
                {
                    Destroy(stackToDestroy.Transform.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromStack();
        }

        private void OnCurrentStackPlaced(IStack placedStack)
        {
            if (placedStack == _currentStack)
            {
                UnsubscribeFromStack();

                // Notify board that this spawn point is now empty
                // Board will decide when to respawn across all points
                OnStackPlaced?.Invoke(this);
            }
        }

        private void UnsubscribeFromStack()
        {
            if (_currentStack != null)
            {
                _currentStack.OnPlaced -= OnCurrentStackPlaced;
                _currentStack = null;
            }
        }
    }
}
