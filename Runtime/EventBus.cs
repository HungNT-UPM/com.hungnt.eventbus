using System;
using System.Collections.Generic;

namespace HungNT.EventBus
{
    /// <summary>
    /// Global event bus (singleton): type-safe pub/sub dùng struct events.
    /// Tự bỏ qua listener bị destroy khi Dispatch, không crash.
    /// Nên Unregister trong OnDestroy để giữ list gọn.
    /// </summary>
    public class EventBus : MonoSingleton<EventBus>
    {
        // Dùng List<Delegate> thay multicast delegate để tránh alloc trên dispatch path (không gọi GetInvocationList)
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _handlers.Clear();
        }

        #region === REGISTER / UNREGISTER ===

        /// <summary>Đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public void Register<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            if (listener == null)
            {
                this.LogWarning($"Register<{typeof(TEvent).Name}> — listener null, skipped.");
                return;
            }

            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }

            list.Add(listener);
        }

        /// <summary>Hủy đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public void Unregister<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            if (listener == null) return;

            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out var list)) return;

            list.Remove(listener);
            if (list.Count == 0)
                _handlers.Remove(type);
        }

        #endregion

        #region === DISPATCH ===

        /// <summary>Gửi event có data tới tất cả listener đã đăng ký.</summary>
        public void Dispatch<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list)) return;
            SafeInvoke(list, evt);
        }

        /// <summary>Gửi signal event không có data.</summary>
        public void Dispatch<TEvent>() where TEvent : struct, IEvent
            => Dispatch(default(TEvent));

        #endregion

        #region === UTILITIES ===

        /// <summary>Xóa tất cả listener của một event type.</summary>
        public void ClearEvent<TEvent>() where TEvent : IEvent
            => _handlers.Remove(typeof(TEvent));

        /// <summary>Xóa toàn bộ listener.</summary>
        public void ClearAll()
        {
            _handlers.Clear();
            this.Log("All listeners cleared.".Color("orange"));
        }

        #endregion

        #region === DEBUG / EDITOR ===

        /// <summary>Snapshot tất cả listener đang đăng ký — dùng bởi EventBusEditor.</summary>
        public List<ListenerDebugEntry> GetDebugSnapshot()
        {
            var result = new List<ListenerDebugEntry>();

            foreach (var kvp in _handlers)
            {
                if (kvp.Value == null) continue;

                foreach (var d in kvp.Value)
                {
                    bool isDestroyed = d.Target is UnityEngine.Object uObj && uObj == null;

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

        #endregion

        private static void SafeInvoke<TEvent>(List<Delegate> list, TEvent evt)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];
                if (d.Target is UnityEngine.Object unityObj && unityObj == null)
                    continue;

                try
                {
                    ((Action<TEvent>)d).Invoke(evt);
                }
                catch (Exception ex)
                {
                    DebugEx.LogError($"[{nameof(EventBus)}] Exception in {d.Target?.GetType().Name}.{d.Method.Name}: {ex}");
                }
            }
        }
    }
}
