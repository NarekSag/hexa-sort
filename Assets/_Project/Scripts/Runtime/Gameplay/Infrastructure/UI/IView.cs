namespace _Project.Scripts.Runtime.Gameplay.UI
{
    public interface IView<TViewModel>
    {
        void Initialize(TViewModel viewModel);
    }
}

