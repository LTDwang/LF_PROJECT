using UnityEngine.Playables;

#if UNITY_EDITOR
using System;
using System.Reflection;
#endif

namespace NoWireAnim
{
    internal static class PlayableGraphVisualizerBridge
    {
#if UNITY_EDITOR
        private static Type _clientType;
        private static MethodInfo _showMethod;
        private static MethodInfo _hideMethod;
        private static bool _initialized;

        private static void EnsureInit()
        {
            if (_initialized) return;
            _initialized = true;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                _clientType = assembly.GetType("GraphVisualizerClient");
                if (_clientType != null)
                    break;
            }

            if (_clientType == null)
                return;

            _showMethod = _clientType.GetMethod(
                "Show",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(PlayableGraph) },
                null);

            _hideMethod = _clientType.GetMethod(
                "Hide",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(PlayableGraph) },
                null);
        }

        public static bool IsAvailable
        {
            get
            {
                EnsureInit();
                return _showMethod != null;
            }
        }

        public static void Show(PlayableGraph graph)
        {
            EnsureInit();

            if (!graph.IsValid() || _showMethod == null)
                return;

            _showMethod.Invoke(null, new object[] { graph });
        }

        public static void Hide(PlayableGraph graph)
        {
            EnsureInit();

            if (!graph.IsValid() || _hideMethod == null)
                return;

            _hideMethod.Invoke(null, new object[] { graph });
        }
#else
        public static bool IsAvailable => false;
        public static void Show(PlayableGraph graph) { }
        public static void Hide(PlayableGraph graph) { }
#endif
    }
}