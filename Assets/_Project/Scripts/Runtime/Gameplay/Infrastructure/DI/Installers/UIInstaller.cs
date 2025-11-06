using UnityEngine;
using VContainer.Unity;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.UI.Game;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class UIInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameView _gameView;

        public void Install(IContainerBuilder builder)
        {
            if (_gameView != null)
            {
                builder.RegisterComponent(_gameView);
            }

            builder.Register<GameViewModel>(Lifetime.Scoped);
        }
    }
}

