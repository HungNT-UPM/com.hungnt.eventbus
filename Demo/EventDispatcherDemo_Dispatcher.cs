using UnityEngine;

namespace HungNT.EventDispatcher.Demo
{
    /// <summary>
    /// Demo: Dispatch events qua EventDispatcher.
    /// Gán vào một GameObject rồi nhấn các nút trong Inspector (Play Mode).
    /// </summary>
    public class EventDispatcherDemo_Dispatcher : MonoBehaviour
    {
        // ── Cách 1: Dùng trực tiếp qua singleton ─────────────────────────────

        [ContextMenu("Dispatch OnGameStart (singleton)")]
        public void DispatchGameStart()
            => EventDispatcher.Instance.Dispatch<OnGameStart>();

        [ContextMenu("Dispatch OnGameWin (singleton)")]
        public void DispatchGameWin()
            => EventDispatcher.Instance.Dispatch<OnGameWin>();

        [ContextMenu("Dispatch OnCoinChanged (singleton)")]
        public void DispatchCoinChanged()
            => EventDispatcher.Instance.Dispatch(new OnCoinChanged { OldValue = 50, NewValue = 150 });

        // ── Cách 2: Dùng Extension Methods (this.Dispatch) ────────────────────

        [ContextMenu("Dispatch OnPlayerJump (extension, với data)")]
        public void DispatchPlayerJump()
            => this.Dispatch(new OnPlayerJump { JumpHeight = 3.5f });
    }
}