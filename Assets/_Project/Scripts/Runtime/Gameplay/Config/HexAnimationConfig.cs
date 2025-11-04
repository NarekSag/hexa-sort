using UnityEngine;
using DG.Tweening;

namespace _Project.Scripts.Runtime.Gameplay.Config 
{
    [CreateAssetMenu(fileName = "HexAnimationConfig", menuName = "Gameplay/Grid/Hex Animation Config")]
    public class HexAnimationConfig : ScriptableObject {
        [SerializeField] private float _jumpPower = 1f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _rotationDuration = 0.5f;
        [SerializeField] private Ease _jumpEase = Ease.Linear;
        [SerializeField] private float _baseDelay = 0f;
        [SerializeField] private float _staggerDelay = 0.15f;

        public float JumpPower => _jumpPower;
        public float JumpDuration => _jumpDuration;
        public float RotationDuration => _rotationDuration;
        public Ease JumpEase => _jumpEase;
        public float BaseDelay => _baseDelay;
        public float StaggerDelay => _staggerDelay;
    }
}
