namespace HungNT.EventBus
{
    /// <summary>
    /// Marker interface for all typed game events.
    /// Define events as structs for zero-allocation dispatch:
    /// <code>
    /// public struct OnCoinChanged : IEvent { public int NewValue; }
    /// </code>
    /// </summary>
    public interface IEvent
    {
    }
}