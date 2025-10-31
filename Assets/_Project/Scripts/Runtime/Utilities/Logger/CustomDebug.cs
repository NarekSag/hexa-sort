using UnityEngine;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace _Project.Scripts.Runtime.Utilities.Logging
{
    public static class CustomDebug
    {
        

        private static string GetPrefixColor<T>(T feature) where T : Enum
        {
            return feature switch
            {
                LogCategory.Loading => LogColors.System,
                _ => LogColors.Feature
            };
        }

        private static void LogInternal<T>(T feature, string message, LogLevel level, Exception exception = null, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum
        {
            var className = GetClassNameFromFilePath(callerFilePath);
            var methodName = callerMemberName;
            var fullMessage = exception != null ? $"{message}\nException: {exception}" : message;
            
            // Use a format that Unity console can parse properly
            // The key is to put the main message first, then the context
            var prefixColor = GetPrefixColor(feature);
            var structuredMessage = $"{fullMessage}\n<color={prefixColor}>[{feature}]</color> " +
                                  $"<color={LogColors.ClassName}>[{className}]</color> " +
                                  $"<color={LogColors.MethodName}>[{methodName}]</color>";

            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(structuredMessage);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(structuredMessage);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(structuredMessage);
                    break;
            }
        }

        public static void Log<T>(T feature, string message, LogLevel level = LogLevel.Info, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum => 
            LogInternal(feature, message, level, null, callerMemberName, callerFilePath, callerLineNumber);

        public static void LogDebug<T>(T feature, string message, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum => 
            LogInternal(feature, message, LogLevel.Debug, null, callerMemberName, callerFilePath, callerLineNumber);

        public static void LogInfo<T>(T feature, string message, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum => 
            LogInternal(feature, message, LogLevel.Info, null, callerMemberName, callerFilePath, callerLineNumber);

        public static void LogWarning<T>(T feature, string message, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum => 
            LogInternal(feature, message, LogLevel.Warning, null, callerMemberName, callerFilePath, callerLineNumber);

        public static void LogError<T>(T feature, string message, Exception exception = null, 
            [CallerMemberName] string callerMemberName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0) where T : Enum => 
            LogInternal(feature, message, LogLevel.Error, exception, callerMemberName, callerFilePath, callerLineNumber);

        private static string GetColorForLevel(LogLevel level) => level switch
        {
            LogLevel.Debug => LogColors.Debug,
            LogLevel.Info => LogColors.Info,
            LogLevel.Warning => LogColors.Warning,
            LogLevel.Error => LogColors.Error,
            _ => LogColors.Info
        };

        /// <summary>
        /// Gets the caller context information (class name and method name)
        /// </summary>
        /// <param name="callerMemberName">The caller method name</param>
        /// <param name="callerFilePath">The caller file path</param>
        /// <returns>Formatted context string with colored class and method names</returns>
        private static string GetCallerContext(string callerMemberName, string callerFilePath)
        {
            var className = GetClassNameFromFilePath(callerFilePath);
            var methodName = callerMemberName;
            
            return $"<color={LogColors.ClassName}>[{className}]</color> <color={LogColors.MethodName}>[{methodName}]</color>";
        }

        /// <summary>
        /// Extracts class name from file path
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>The class name</returns>
        private static string GetClassNameFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "UnknownClass";

            var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return fileName ?? "UnknownClass";
        }
    }
}