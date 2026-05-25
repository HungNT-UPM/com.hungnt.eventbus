using System;
using System.Collections.Generic;

namespace HungNT.EventDispatcher
{
    /// <summary>
    /// Type-safe event dispatcher (singleton).
    /// Tự bỏ qua listener bị destroy khi Dispatch, không crash.
    /// Nên Unregister trong OnDestroy để giữ list gọn.
    /// </summary>
    public class EventDispatcher : MonoSingleton<EventDispatcher>
    {
        // Key = event Type, Value = multicast Delegate (Action<TEvent>)
        private readonly Dictionary<Type, Delegate> _handlers = new();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _handlers.Clear();
        }

        // ── Register / Unregister ────────────────────────────────────────────

        /// <summary>Đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public void Register<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            if (listener == null)
            {
                this.LogWarning($"Register<{typeof(TEvent).Name}> — listener null, skipped.");
                return;
            }

            var type = typeof(TEvent);
            _handlers[type] = _handlers.TryGetValue(type, out var existing)
                ? Delegate.Combine(existing, listener)
                : listener;
        }

        /// <summary>Hủy đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public void Unregister<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            if (listener == null) return;

            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out var existing)) return;

            var updated = Delegate.Remove(existing, listener);
            if (updated == null) _handlers.Remove(type);
            else _handlers[type] = updated;
        }

        // ── Dispatch ─────────────────────────────────────────────────────────

        /// <summary>Gửi event có data tới tất cả listener đã đăng ký.</summary>
        public void Dispatch<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var del)) return;
            SafeInvoke(del, evt);
        }

        /// <summary>Gửi signal event không có data.</summary>
        public void Dispatch<TEvent>() where TEvent : struct, IEvent
            => Dispatch(default(TEvent));

        // ── Utilities ────────────────────────────────────────────────────────

        /// <summary>Xóa tất cả listener của một event type.</summary>
        public void ClearEvent<TEvent>() where TEvent : IEvent
            => _handlers.Remove(typeof(TEvent));

        /// <summary>Xóa toàn bộ listener.</summary>
        public void ClearAll()
        {
            _handlers.Clear();
            this.Log("All listeners cleared.".Color("orange"));
        }

        // ── Debug / Editor ───────────────────────────────────────────────────

        /// <summary>
        /// Snapshot tất cả listener đang đăng ký — dùng bởi EventDispatcherEditor.
        /// </summary>
        public List<ListenerDebugEntry> GetDebugSnapshot()
        {
            var result = new List<ListenerDebugEntry>();

            foreach (var kvp in _handlers)
            {
                if (kvp.Value == null) continue;

                foreach (var d in kvp.Value.GetInvocationList())
                {
                    bool isDestroyed = d.Target is UnityEngine.Object uObj && uObj == null;

                    // Lấy reference thực của object register (nếu là UnityEngine.Object)
                    UnityEngine.Object registeredObj = null;
                    if (!isDestroyed && d.Target is UnityEngine.Object obj)
                        registeredObj = obj;

                    result.Add(new ListenerDebugEntry
                    {
                        EventName = kvp.Key.Name,
                        TargetName = d.Target != null ? d.Target.GetType().Name : "static",
                        MethodName = d.Method.Name,
                        IsDestroyed = isDestroyed,
                        RegisteredObject = registeredObj,
                    });
                }
            }

            return result;
        }

        /// <summary>Listener entry — chỉ dùng để hiển thị trong Editor.</summary>
        public struct ListenerDebugEntry
        {
            public string EventName;
            public string TargetName;
            public string MethodName;
            public bool IsDestroyed;

            /// <summary>Reference tới UnityEngine.Object đã đăng ký (null nếu static hoặc bị destroy).</summary>
            public UnityEngine.Object RegisteredObject;

            public override string ToString() =>
                IsDestroyed
                    ? $"[DESTROYED] {EventName} ← {TargetName}.{MethodName}"
                    : $"{EventName} ← {TargetName}.{MethodName}";
        }

        // ── Safe invoke ──────────────────────────────────────────────────────

        private static void SafeInvoke<TEvent>(Delegate del, TEvent evt)
        {
            foreach (var d in del.GetInvocationList())
            {
                if (d.Target is UnityEngine.Object unityObj && unityObj == null)
                    continue;

                try
                {
                    ((Action<TEvent>)d).Invoke(evt);
                }
                catch (Exception ex)
                {
                    DebugEx.LogError($"[{nameof(EventDispatcher)}] Exception in {d.Target?.GetType().Name}.{d.Method.Name}: {ex}");
                }
            }
        }
    }
}