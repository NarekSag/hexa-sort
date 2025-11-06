using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack
{
    public class HexStackBoard : MonoBehaviour
    {
        [Header("Spawn Points")]
        [Tooltip("Assign StackSpawnPoint components from child GameObjects, or leave empty to auto-detect.")]
        [SerializeField]
        private StackSpawnPoint[] _spawnPoints;

        [Inject] private HexStackFactory _stackFactory;

        private LevelData _currentLevelData;
        private bool _isInitialized = false;

        private void Awake()
        {
            // Auto-detect spawn points if not assigned
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                _spawnPoints = GetComponentsInChildren<StackSpawnPoint>();
                CustomDebug.Log(LogCategory.Gameplay,
                    $"HexStackBoard: Auto-detected {_spawnPoints.Length} spawn points");
            }
        }

        public void Initialize(LevelData levelData = null)
        {
            _currentLevelData = levelData;

            UnsubscribeFromAllSpawnPoints();
            InitializeSpawnPoints();
            SubscribeToAllSpawnPoints();

            _isInitialized = true;
        }

        public void SetLevelData(LevelData levelData)
        {
            _currentLevelData = levelData;

            if (_spawnPoints == null)
            {
                return;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    spawnPoint.SetLevelData(levelData);
                }
            }
        }

        public void ClearAllStacks()
        {
            if (_spawnPoints == null)
            {
                return;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    spawnPoint.ClearStack();
                }
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromAllSpawnPoints();
        }

        private void InitializeSpawnPoints()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                CustomDebug.LogWarning(LogCategory.Gameplay, "No spawn points found on HexStackBoard!");
                return;
            }

            if (_stackFactory == null)
            {
                CustomDebug.LogError(LogCategory.Gameplay,
                    "HexStackBoard: StackFactory is NULL! Cannot initialize spawn points.");
                return;
            }

            // Initialize all spawn points (creates initial stacks)
            int initializedCount = 0;
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                if (_spawnPoints[i] != null)
                {
                    _spawnPoints[i].Initialize(_stackFactory, _currentLevelData);
                    initializedCount++;
                }
                else
                {
                    CustomDebug.LogWarning(LogCategory.Gameplay, $"Spawn point {i} is NULL!");
                }
            }
        }

        private void SubscribeToAllSpawnPoints()
        {
            if (_spawnPoints == null)
            {
                return;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    spawnPoint.OnStackPlaced += OnAnyStackPlaced;
                }
            }
        }

        private void UnsubscribeFromAllSpawnPoints()
        {
            if (_spawnPoints == null)
            {
                return;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    spawnPoint.OnStackPlaced -= OnAnyStackPlaced;
                }
            }
        }

        private void OnAnyStackPlaced(StackSpawnPoint placedSpawnPoint)
        {
            if (!_isInitialized)
            {
                return;
            }

            // Check if all spawn points are now empty
            if (AreAllSpawnPointsEmpty())
            {
                CustomDebug.Log(LogCategory.Gameplay, "All spawn points empty - spawning new stacks!");
                SpawnStacksAtAllPoints();
            }
        }

        private bool AreAllSpawnPointsEmpty()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                return false;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null && !spawnPoint.IsEmpty)
                {
                    return false; // Found at least one with a stack
                }
            }

            return true; // All are empty
        }

        private void SpawnStacksAtAllPoints()
        {
            if (_spawnPoints == null)
            {
                return;
            }

            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null && spawnPoint.IsEmpty)
                {
                    spawnPoint.CreateStack();
                }
            }
        }

        public void ShuffleStacks()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                CustomDebug.LogWarning(LogCategory.Gameplay, "HexStackBoard: No spawn points available for shuffle!");
                return;
            }

            // Clear all existing stacks
            ClearAllStacks();

            // Spawn new stacks at all empty spawn points
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null && spawnPoint.IsEmpty)
                {
                    spawnPoint.CreateStack();
                }
            }
        }
    }
}
