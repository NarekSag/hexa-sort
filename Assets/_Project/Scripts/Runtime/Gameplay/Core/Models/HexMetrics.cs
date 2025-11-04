using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Core.Models {
    public static class HexMetrics {

        public const float OuterRadius = 0.5f; // The scale of the object

        public const float InnerRadius = OuterRadius * 0.866025404f;

        public static readonly Vector3[] Corners = {
            new Vector3(0f, 0f, OuterRadius),
            new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
            new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(0f, 0f, -OuterRadius),
            new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
            new Vector3(0f, 0f, OuterRadius)
	    };
    }
}