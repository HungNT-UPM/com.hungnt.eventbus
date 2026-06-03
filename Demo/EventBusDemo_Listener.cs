using UnityEngine;

namespace HungNT.EventBus.Demo
{
    /// <summary>
    /// Demo: Listener đăng ký / hủy đăng ký events.
    /// Gán vào GameObject khác với <see cref="EventBusDemo_Dispatcher"/>,
    /// rồi quan sát Inspector của EventBus để thấy listener live.
    /// </summary>
    public class EventBusDemo_Listener : MonoBehaviour
    {
        // ── Cách 1: Register/Unregister thủ công qua singleton ───────────────

        private void OnEnable()
        {
            EventBus.Instance.Register<OnGameStart>(OnGameStart);
            EventBus.Instance.Register<OnGameWin>(OnGameWin);
            EventBus.Instance.Register<OnCoinChanged>(OnCoinChanged);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unregister<OnGameStart>(OnGameStart);
            EventBus.Instance.Unregister<OnGameWin>(OnGameWin);
            EventBus.Instance.Unregister<OnCoinChanged>(OnCoinChanged);
        }

        // ── Cách 2: Register/Unregister qua Extension Methods ────────────────

        private void Start()
        {
            this.Register<OnPlayerJump>(OnPlayerJump);
        }

        private void OnDestroy()
        {
            this.Unregister<OnPlayerJump>(OnPlayerJump);
        }

        // ── Handlers ─────────────────────────────────────────────────────────

        private void OnGameStart(OnGameStart _)
            => Debug.Log($"[{name}] Game Started!");

        private void OnGameWin(OnGameWin _)
            => Debug.Log($"[{name}] Game Won!");

        private void OnCoinChanged(OnCoinChanged e)
            => Debug.Log($"[{name}] Coin: {e.OldValue} → {e.NewValue} (Δ{e.Delta})");

        private void OnPlayerJump(OnPlayerJump e)
            => Debug.Log($"[{name}] Player jumped {e.JumpHeight}m");
    }
}
