namespace HungNT.EventBus.Demo
{
    // ── Custom event chỉ dùng trong Demo ─────────────────────────────────────

    /// <summary>Signal: game bắt đầu.</summary>
    public struct OnGameStart : IEvent
    {
    }

    /// <summary>Signal: người chơi thắng.</summary>
    public struct OnGameWin : IEvent
    {
    }

    public struct OnCoinChanged : IEvent
    {
        public int OldValue;
        public int NewValue;
        public int Delta => NewValue - OldValue;
    }

    /// <summary>Event có data: player nhảy.</summary>
    public struct OnPlayerJump : IEvent
    {
        public float JumpHeight;
    }
}
