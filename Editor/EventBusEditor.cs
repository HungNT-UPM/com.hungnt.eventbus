using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace HungNT.EventBus.Editor
{
    /// <summary>
    /// Custom Inspector cho EventBus.
    /// Dùng OdinEditor nếu có Odin, fallback về IMGUI thuần nếu không.
    /// Hiển thị live tất cả listener theo event — repaint mỗi 0.5s khi Play.
    /// </summary>
    [CustomEditor(typeof(EventBus))]
    public class EventBusEditor : OdinEditor
    {
        private readonly Dictionary<string, bool> _foldouts = new();
        private double _lastRepaintTime;

        // Styles (lazy-init)
        private GUIStyle _headerStyle;
        private GUIStyle _listenerStyle;
        private GUIStyle _destroyedStyle;
        private GUIStyle _badgeStyle;
        private GUIStyle _sectionBoxStyle;

        // ────────────────────────────────────────────────────────────────────

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(6);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Listener list available in Play Mode only.", MessageType.Info);
                return;
            }

            InitStyles();

            var dispatcher = (EventBus)target;
            var snapshot = dispatcher.GetDebugSnapshot();

            DrawHeader(snapshot.Count);

            if (snapshot.Count == 0)
            {
                EditorGUILayout.LabelField("  (no listeners registered)", EditorStyles.miniLabel);
                return;
            }

            var grouped = GroupByEvent(snapshot);
            foreach (var kvp in grouped)
                DrawEventGroup(kvp.Key, kvp.Value);

            // Auto-repaint mỗi 0.5s
            if (EditorApplication.timeSinceStartup - _lastRepaintTime > 0.5)
            {
                _lastRepaintTime = EditorApplication.timeSinceStartup;
                Repaint();
            }
        }

        // ── Draw helpers ─────────────────────────────────────────────────────

        private void DrawHeader(int totalCount)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("⚡ Event Listeners", _headerStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"  {totalCount} registered  ", _badgeStyle, GUILayout.Width(110));
            EditorGUILayout.EndHorizontal();

            var rect = GUILayoutUtility.GetLastRect();
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
            EditorGUILayout.Space(8);
        }

        private void DrawEventGroup(string eventName, List<EventBus.ListenerDebugEntry> listeners)
        {
            if (!_foldouts.ContainsKey(eventName))
                _foldouts[eventName] = true;

            int destroyedCount = 0;
            foreach (var l in listeners)
                if (l.IsDestroyed)
                    destroyedCount++;

            var label = destroyedCount > 0
                ? $"⚠  {eventName}  ({listeners.Count} listeners, {destroyedCount} destroyed)"
                : $"◆  {eventName}  ({listeners.Count} listener{(listeners.Count > 1 ? "s" : "")})";

            // Box bao bọc từng event group
            EditorGUILayout.BeginVertical(_sectionBoxStyle);

            _foldouts[eventName] = EditorGUILayout.Foldout(_foldouts[eventName], label, true, _headerStyle);

            if (_foldouts[eventName])
            {
                EditorGUI.indentLevel++;
                foreach (var entry in listeners)
                    DrawListenerRow(entry);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawListenerRow(EventBus.ListenerDebugEntry entry)
        {
            var style = entry.IsDestroyed ? _destroyedStyle : _listenerStyle;
            var icon = entry.IsDestroyed ? "⚠ " : "→ ";
            var label = entry.IsDestroyed
                ? $"{icon}[DESTROYED]  {entry.TargetName}.{entry.MethodName}"
                : $"{icon}{entry.TargetName}.{entry.MethodName}";

            EditorGUILayout.BeginHorizontal();

            // Label listener
            EditorGUILayout.LabelField(label, style, GUILayout.MinWidth(160));

            // ObjectField reference (read-only)
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(
                    entry.RegisteredObject,
                    typeof(UnityEngine.Object),
                    allowSceneObjects: true,
                    GUILayout.MinWidth(120), GUILayout.MaxWidth(200));
            }

            EditorGUILayout.EndHorizontal();
        }

        // ── Grouping ─────────────────────────────────────────────────────────

        private static Dictionary<string, List<EventBus.ListenerDebugEntry>> GroupByEvent(
            List<EventBus.ListenerDebugEntry> snapshot)
        {
            var grouped = new Dictionary<string, List<EventBus.ListenerDebugEntry>>();
            foreach (var entry in snapshot)
            {
                if (!grouped.ContainsKey(entry.EventName))
                    grouped[entry.EventName] = new List<EventBus.ListenerDebugEntry>();
                grouped[entry.EventName].Add(entry);
            }
            return grouped;
        }

        // ── Styles ───────────────────────────────────────────────────────────

        private void InitStyles()
        {
            if (_headerStyle != null) return;

            _headerStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
            };
            _headerStyle.normal.textColor = new Color(0.88f, 0.88f, 0.88f);
            _headerStyle.onNormal.textColor = new Color(0.88f, 0.88f, 0.88f);

            _listenerStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
            };
            _listenerStyle.normal.textColor = new Color(0.55f, 0.92f, 0.55f); // green

            _destroyedStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
            };
            _destroyedStyle.normal.textColor = new Color(1f, 0.45f, 0.35f); // red-orange

            _badgeStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
            };

            _sectionBoxStyle = new GUIStyle("box")
            {
                padding = new RectOffset(6, 6, 4, 4),
                margin = new RectOffset(0, 0, 2, 2),
            };
        }
    }
}