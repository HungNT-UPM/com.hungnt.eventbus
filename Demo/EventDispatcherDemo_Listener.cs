using UnityEngine;

namespace HungNT.EventDispatcher.Demo
{
    /// <summary>
    /// Demo: Listener đăng ký / hủy đăng ký events.
    /// Gán vào GameObject khác với <see cref="EventDispatcherDemo_Dispatcher"/>,
    /// rồi quan sát Inspector của EventDispatcher để thấy listener live.
    /// </summary>
    public class EventDispatcherDemo_Listener : MonoBehaviour
    {
        // ── Cách 1: Register/Unregister thủ công qua singleton ───────────────

        private void OnEnable()
        {
            EventDispatcher.Instance.Register<OnGameStart>(OnGameStart);
            EventDispatcher.Instance.Register<OnGameWin>(OnGameWin);
            EventDispatcher.Instance.Register<OnCoinChanged>(OnCoinChanged);
        }

        private void OnDisable()
        {
            EventDispatcher.Instance.Unregister<OnGameStart>(OnGameStart);
            EventDispatcher.Instance.Unregister<OnGameWin>(OnGameWin);
            EventDispatcher.Instance.Unregister<OnCoinChanged>(OnCoinChanged);
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
