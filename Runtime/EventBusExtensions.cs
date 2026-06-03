using System;
using UnityEngine;

namespace HungNT.EventBus
{
    /// <summary>
    /// Extension methods trên MonoBehaviour để gọi EventBus ngắn gọn hơn.
    /// </summary>
    public static class EventBusExtensions
    {
        /// <summary>Đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public static void Register<TEvent>(this MonoBehaviour _, Action<TEvent> listener) where TEvent : IEvent
        {
            EventBus.Instance.Register(listener);
        }

        /// <summary>Hủy đăng ký lắng nghe <typeparamref name="TEvent"/>.</summary>
        public static void Unregister<TEvent>(this MonoBehaviour _, Action<TEvent> listener) where TEvent : IEvent
        {
            EventBus.Instance.Unregister(listener);
        }

        /// <summary>Gửi event có data.</summary>
        public static void Dispatch<TEvent>(this MonoBehaviour _, TEvent evt) where TEvent : IEvent
        {
            EventBus.Instance.Dispatch(evt);
        }

        /// <summary>Gửi signal event (không có data). Vd: <c>this.Dispatch&lt;OnGameStart&gt;()</c></summary>
        public static void Dispatch<TEvent>(this MonoBehaviour _) where TEvent : struct, IEvent
        {
            EventBus.Instance.Dispatch<TEvent>();
        }
    }
}