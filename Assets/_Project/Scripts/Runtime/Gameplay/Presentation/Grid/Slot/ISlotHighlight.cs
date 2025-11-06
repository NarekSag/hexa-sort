namespace _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot
{
    public interface ISlotHighlight
    {
        void SetHighlighted(bool highlighted);
        void RemoveHighlight();
        void Initialize(UnityEngine.MeshRenderer renderer);
    }
}

