using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Config;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Pooling;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories
{
    public class HexStackFactory
    {
        private readonly CellPool _cellPool;
        private readonly StackPool _stackPool;

        public HexStackFactory(CellPool cellPool, StackPool stackPool)
        {
            _cellPool = cellPool;
            _stackPool = stackPool;
        }

        public IStack CreateRandomStack(Transform parent = null, Vector3 position = default, LevelData levelData = null)
        {
            IStack stack = _stackPool.Get();
            stack.Transform.SetParent(parent);
            stack.Transform.position = position;

            // Use level data if provided, otherwise use defaults
            int minHeight = levelData?.MinStackHeight ?? 2;
            int maxHeight = levelData?.MaxStackHeight ?? 6;
            ColorType[] availableColors = levelData?.AvailableColors ?? new[] { ColorType.Red, ColorType.Blue };

            // Ensure minimum height is at least 2 to match Range constraint and prevent invalid stacks
            minHeight = Mathf.Max(2, minHeight);
            maxHeight = Mathf.Max(minHeight, maxHeight);

            int cellCount = Random.Range(minHeight, maxHeight + 1);

            for (int i = 0; i < cellCount; i++)
            {
                ICell cell = CreateRandomCell(stack.Transform, i, availableColors);
                stack.Cells.Add(cell);
            }

            stack.Initialize();

            return stack;
        }

        private ICell CreateRandomCell(Transform parent, int index, ColorType[] availableColors)
        {
            ICell cell = _cellPool.Get();
            cell.Transform.SetParent(parent);

            ColorType randomColor = availableColors[Random.Range(0, availableColors.Length)];

            cell.Initialize(randomColor, index);

            return cell;
        }
    }
}

