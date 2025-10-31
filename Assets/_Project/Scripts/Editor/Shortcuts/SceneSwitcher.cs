using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.IO;

namespace _Project.Scripts.Editor.Shortcuts
{
    /// <summary>
    /// Provides keyboard shortcuts (Alt+1 through Alt+9) to quickly switch between scenes in build settings.
    /// Supports up to 9 scenes with no menu items - shortcuts only.
    /// </summary>
    public static class SceneSwitcher
    {
        [Shortcut("Scene Switcher/Open Scene 1", KeyCode.Alpha1, ShortcutModifiers.Alt)]
        private static void OpenScene1() => OpenScene(0);

        [Shortcut("Scene Switcher/Open Scene 2", KeyCode.Alpha2, ShortcutModifiers.Alt)]
        private static void OpenScene2() => OpenScene(1);

        [Shortcut("Scene Switcher/Open Scene 3", KeyCode.Alpha3, ShortcutModifiers.Alt)]
        private static void OpenScene3() => OpenScene(2);

        [Shortcut("Scene Switcher/Open Scene 4", KeyCode.Alpha4, ShortcutModifiers.Alt)]
        private static void OpenScene4() => OpenScene(3);

        [Shortcut("Scene Switcher/Open Scene 5", KeyCode.Alpha5, ShortcutModifiers.Alt)]
        private static void OpenScene5() => OpenScene(4);

        [Shortcut("Scene Switcher/Open Scene 6", KeyCode.Alpha6, ShortcutModifiers.Alt)]
        private static void OpenScene6() => OpenScene(5);

        [Shortcut("Scene Switcher/Open Scene 7", KeyCode.Alpha7, ShortcutModifiers.Alt)]
        private static void OpenScene7() => OpenScene(6);

        [Shortcut("Scene Switcher/Open Scene 8", KeyCode.Alpha8, ShortcutModifiers.Alt)]
        private static void OpenScene8() => OpenScene(7);

        [Shortcut("Scene Switcher/Open Scene 9", KeyCode.Alpha9, ShortcutModifiers.Alt)]
        private static void OpenScene9() => OpenScene(8);

        private static void OpenScene(int index)
        {
            var scenes = EditorBuildSettings.scenes;
            if (index < 0 || index >= scenes.Length)
            {
                UnityEngine.Debug.LogWarning($"Scene index {index} is out of range. Only {scenes.Length} scene(s) available.");
                return;
            }

            var scene = scenes[index];
            
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scene.path);
            }
        }

        /// <summary>
        /// Gets the display name for a scene at the given index, formatted as "index.SceneName"
        /// </summary>
        public static string GetSceneDisplayName(int index)
        {
            var scenes = EditorBuildSettings.scenes;
            if (index < 0 || index >= scenes.Length)
                return $"Scene {index + 1}";

            var sceneName = Path.GetFileNameWithoutExtension(scenes[index].path);
            return $"{index}.{sceneName}";
        }
    }
}