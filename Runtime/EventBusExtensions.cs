using System;
using UnityEngine;

namespace HungNT.EventBus
{
    /// <summary>
    /// Extension methods trên MonoBehaviour để gọi EventBus ngắn gọn hơn.
    /// An toàn khi app đang quit: <see cref="EventBus"/> Instance trả về null lúc teardown — các call trở thành no-op
    /// (Unregister trong OnDestroy không tạo lại EventBus giữa lúc thoát game).
    /// </summary>
    public static class EventBusExtensions
    {
        /// <summary>Đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public static void Register<TEvent>(this MonoBehaviour _, Action<TEvent> listener) where TEvent : IEvent
        {
            var bus = EventBus.Instance;
            if (bus != null)
                bus.Register(listener);
        }

        /// <summary>Hủy đăng ký lắng nghe <typeparamref name="TEvent"/>. Không tạo mới EventBus nếu chưa/không còn tồn tại.</summary>
        public static void Unregister<TEvent>(this MonoBehaviour _, Action<TEvent> listener) where TEvent : IEvent
        {
            if (!EventBus.HasInstance)
                return;

            EventBus.Instance.Unregister(listener);
        }

        /// <summary>Gửi event có data.</summary>
        public static void Dispatch<TEvent>(this MonoBehaviour _, TEvent evt) where TEvent : IEvent
        {
            var bus = EventBus.Instance;
            if (bus != null)
                bus.Dispatch(evt);
        }

        /// <summary>Gửi signal event (không có data). Vd: <c>this.Dispatch&lt;OnGameStart&gt;()</c></summary>
        public static void Dispatch<TEvent>(this MonoBehaviour _) where TEvent : struct, IEvent
        {
            var bus = EventBus.Instance;
            if (bus != null)
                bus.Dispatch<TEvent>();
        }
    }
}
