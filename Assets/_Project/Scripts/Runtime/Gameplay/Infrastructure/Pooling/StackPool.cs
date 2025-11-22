using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Pooling
{
    public class StackPool
    {
        private readonly Queue<IStack> _availableStacks = new Queue<IStack>();
        private readonly HashSet<IStack> _activeStacks = new HashSet<IStack>();
        private readonly CellPool _cellPool;
        private Transform _poolContainer;
        private readonly int _initialPoolSize;

        public StackPool(CellPool cellPool)
        {
            _cellPool = cellPool;
            _initialPoolSize = 20;

            // Create pool container GameObject
            GameObject containerObject = new GameObject("StackPool");
            _poolContainer = containerObject.transform;
            _poolContainer.SetParent(null);
            Object.DontDestroyOnLoad(containerObject);

            // Warmup the pool
            Warmup(_initialPoolSize);
        }

        public IStack Get()
        {
            // Ensure pool container exists
            if (_poolContainer == null)
            {
                GameObject containerObject = new GameObject("StackPool");
                _poolContainer = containerObject.transform;
                _poolContainer.SetParent(null);
                Object.DontDestroyOnLoad(containerObject);
            }

            IStack stack;

            if (_availableStacks.Count > 0)
            {
                stack = _availableStacks.Dequeue();
            }
            else
            {
                // Pool is empty, create a new stack
                GameObject stackObject = new GameObject("HexStack");
                stackObject.transform.SetParent(_poolContainer);
                stack = stackObject.AddComponent<HexStack>();
            }

            _activeStacks.Add(stack);
            stack.Transform.gameObject.SetActive(true);
            return stack;
        }

        public void Return(IStack stack)
        {
            if (stack == null || stack.Transform == null || stack.Transform.gameObject == null)
            {
                return;
            }

            if (!_activeStacks.Contains(stack))
            {
                // Stack is not from this pool or already returned
                return;
            }

            _activeStacks.Remove(stack);

            // Return all cells in the stack to the cell pool
            if (stack.Cells != null)
            {
                var cellsCopy = new List<ICell>(stack.Cells);
                foreach (ICell cell in cellsCopy)
                {
                    if (cell != null)
                    {
                        _cellPool?.Return(cell);
                    }
                }
                stack.Cells.Clear();
            }

            // Reset stack state
            stack.Transform.gameObject.SetActive(false);
            stack.Transform.SetParent(_poolContainer);
            stack.Transform.localPosition = Vector3.zero;
            stack.Transform.localRotation = Quaternion.identity;
            stack.Transform.localScale = Vector3.one;

            // Reset stack properties
            stack.SetDraggable(true);
            if (stack is HexStack hexStack)
            {
                hexStack.UpdateColliderSize();
            }

            _availableStacks.Enqueue(stack);
        }

        public void Warmup(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject stackObject = new GameObject("HexStack");
                stackObject.transform.SetParent(_poolContainer);
                IStack stack = stackObject.AddComponent<HexStack>();
                stack.Transform.gameObject.SetActive(false);
                _availableStacks.Enqueue(stack);
            }
        }

        public void Clear()
        {
            // Return all cells from active stacks to cell pool, then destroy stacks
            var activeStacksCopy = new List<IStack>(_activeStacks);
            foreach (IStack stack in activeStacksCopy)
            {
                if (stack != null && stack.Cells != null)
                {
                    var cellsCopy = new List<ICell>(stack.Cells);
                    foreach (ICell cell in cellsCopy)
                    {
                        if (cell != null)
                        {
                            _cellPool?.Return(cell);
                        }
                    }
                    stack.Cells.Clear();
                }
            }

            // Destroy all pooled stacks
            while (_availableStacks.Count > 0)
            {
                IStack stack = _availableStacks.Dequeue();
                if (stack != null && stack.Transform != null && stack.Transform.gameObject != null)
                {
                    Object.Destroy(stack.Transform.gameObject);
                }
            }

            // Destroy active stacks
            foreach (IStack stack in _activeStacks)
            {
                if (stack != null && stack.Transform != null && stack.Transform.gameObject != null)
                {
                    Object.Destroy(stack.Transform.gameObject);
                }
            }

            _activeStacks.Clear();

            // Destroy pool container
            if (_poolContainer != null && _poolContainer.gameObject != null)
            {
                Object.Destroy(_poolContainer.gameObject);
            }
        }

        public int GetActiveCount()
        {
            return _activeStacks.Count;
        }

        public int GetAvailableCount()
        {
            return _availableStacks.Count;
        }
    }
}

