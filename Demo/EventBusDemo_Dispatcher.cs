using UnityEngine;

namespace HungNT.EventBus.Demo
{
    /// <summary>
    /// Demo: Dispatch events qua EventBus.
    /// Gán vào một GameObject rồi nhấn các nút trong Inspector (Play Mode).
    /// </summary>
    public class EventBusDemo_Dispatcher : MonoBehaviour
    {
        // ── Cách 1: Dùng trực tiếp qua singleton ─────────────────────────────

        [ContextMenu("Dispatch OnGameStart (singleton)")]
        public void DispatchGameStart()
            => EventBus.Instance.Dispatch<OnGameStart>();

        [ContextMenu("Dispatch OnGameWin (singleton)")]
        public void DispatchGameWin()
            => EventBus.Instance.Dispatch<OnGameWin>();

        [ContextMenu("Dispatch OnCoinChanged (singleton)")]
        public void DispatchCoinChanged()
            => EventBus.Instance.Dispatch(new OnCoinChanged { OldValue = 50, NewValue = 150 });

        // ── Cách 2: Dùng Extension Methods (this.Dispatch) ────────────────────

        [ContextMenu("Dispatch OnPlayerJump (extension, với data)")]
        public void DispatchPlayerJump()
            => this.Dispatch(new OnPlayerJump { JumpHeight = 3.5f });
    }
}