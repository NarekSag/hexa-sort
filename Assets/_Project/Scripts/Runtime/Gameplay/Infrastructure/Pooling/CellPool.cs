using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Pooling
{
    public class CellPool
    {
        private readonly Queue<ICell> _availableCells = new Queue<ICell>();
        private readonly HashSet<ICell> _activeCells = new HashSet<ICell>();
        private readonly HexCell _cellPrefab;
        private Transform _poolContainer;
        private readonly int _initialPoolSize;

        public CellPool(LevelProgressionConfig config)
        {
            if (config == null || config.CellPrefab == null)
            {
                Debug.LogError("CellPool: CellPrefab is not assigned in config!");
                return;
            }

            _cellPrefab = config.CellPrefab;
            _initialPoolSize = 50;

            // Create pool container GameObject
            GameObject containerObject = new GameObject("CellPool");
            _poolContainer = containerObject.transform;
            _poolContainer.SetParent(null);
            Object.DontDestroyOnLoad(containerObject);

            // Warmup the pool
            Warmup(_initialPoolSize);
        }

        public ICell Get()
        {
            // Ensure pool container exists
            if (_poolContainer == null)
            {
                GameObject containerObject = new GameObject("CellPool");
                _poolContainer = containerObject.transform;
                _poolContainer.SetParent(null);
                Object.DontDestroyOnLoad(containerObject);
            }

            ICell cell;

            if (_availableCells.Count > 0)
            {
                cell = _availableCells.Dequeue();
            }
            else
            {
                // Pool is empty, create a new cell
                cell = Object.Instantiate(_cellPrefab, _poolContainer);
            }

            _activeCells.Add(cell);
            cell.Transform.gameObject.SetActive(true);
            cell.Transform.localScale = Vector3.one * 0.5f;
            return cell;
        }

        public void Return(ICell cell)
        {
            if (cell == null || cell.Transform == null || cell.Transform.gameObject == null)
            {
                return;
            }

            if (!_activeCells.Contains(cell))
            {
                // Cell is not from this pool or already returned
                return;
            }

            _activeCells.Remove(cell);

            // Reset cell state
            cell.Transform.gameObject.SetActive(false);
            cell.Transform.SetParent(_poolContainer);
            cell.Transform.localPosition = Vector3.zero;
            cell.Transform.localRotation = Quaternion.identity;
            cell.Transform.localScale = Vector3.one * 0.5f;

            _availableCells.Enqueue(cell);
        }

        public void Warmup(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ICell cell = Object.Instantiate(_cellPrefab, _poolContainer);
                cell.Transform.gameObject.SetActive(false);
                cell.Transform.localScale = Vector3.one * 0.5f;
                _availableCells.Enqueue(cell);
            }
        }

        public void Clear()
        {
            // Return all active cells to pool first
            var activeCellsCopy = new List<ICell>(_activeCells);
            foreach (ICell cell in activeCellsCopy)
            {
                if (cell != null && cell.Transform != null && cell.Transform.gameObject != null)
                {
                    cell.Transform.gameObject.SetActive(false);
                    if (_poolContainer != null)
                    {
                        cell.Transform.SetParent(_poolContainer);
                    }
                    cell.Transform.localPosition = Vector3.zero;
                    cell.Transform.localRotation = Quaternion.identity;
                    cell.Transform.localScale = Vector3.one * 0.5f;
                }
            }

            // Destroy all pooled cells
            while (_availableCells.Count > 0)
            {
                ICell cell = _availableCells.Dequeue();
                if (cell != null && cell.Transform != null && cell.Transform.gameObject != null)
                {
                    Object.Destroy(cell.Transform.gameObject);
                }
            }

            // Destroy active cells
            foreach (ICell cell in _activeCells)
            {
                if (cell != null && cell.Transform != null && cell.Transform.gameObject != null)
                {
                    Object.Destroy(cell.Transform.gameObject);
                }
            }

            _activeCells.Clear();

            // Destroy pool container
            if (_poolContainer != null && _poolContainer.gameObject != null)
            {
                Object.Destroy(_poolContainer.gameObject);
            }
        }

        public int GetActiveCount()
        {
            return _activeCells.Count;
        }

        public int GetAvailableCount()
        {
            return _availableCells.Count;
        }
    }
}

