using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack {
    public class HexStackBoard : MonoBehaviour {
        [Header("Spawn Points")]
        [Tooltip("Assign StackSpawnPoint components from child GameObjects, or leave empty to auto-detect.")]
        [SerializeField] private StackSpawnPoint[] _spawnPoints;
        
        [Inject] private HexStackFactory _stackFactory;
        
        private void Awake() {
            // Auto-detect spawn points if not assigned
            if (_spawnPoints == null || _spawnPoints.Length == 0) {
                _spawnPoints = GetComponentsInChildren<StackSpawnPoint>();
            }
        }
        
        private void Start() {
            InitializeSpawnPoints();
        }
        
        private void InitializeSpawnPoints() {
            if (_spawnPoints == null || _spawnPoints.Length == 0) {
                return;
            }
            
            foreach (var spawnPoint in _spawnPoints) {
                if (spawnPoint != null) {
                    spawnPoint.Initialize(_stackFactory);
                }
            }
        }
    }
}

