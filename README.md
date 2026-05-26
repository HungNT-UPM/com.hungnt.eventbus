# HungNT Event Dispatcher (`com.hungnt.eventdispatcher`)

Event bus type-safe cho Unity: struct implement **`IEvent`**, đăng ký/lắng nghe qua singleton **`EventDispatcher`**, có extension trên `MonoBehaviour` và inspector Odin xem listener live.

## Tính năng

- **`EventDispatcher`** — `MonoSingleton<EventDispatcher>`
- **`Register` / `Unregister` / `Dispatch`** — generic theo kiểu event
- **Signal events** — struct rỗng (`Dispatch<OnGameStart>()`)
- **Events có data** — struct chứa field (`Dispatch(new OnCoinChanged { ... })`)
- **`EventDispatcherExtensions`** — `this.Register`, `this.Unregister`, `this.Dispatch` trên `MonoBehaviour`
- Editor: inspector liệt kê listener đang active (Play Mode)

## Cài đặt

### OpenUPM

```json
"dependencies": {
  "com.hungnt.eventdispatcher": "1.0.2"
}
```

(Cần scoped registry OpenUPM với scope `com.hungnt` — xem README của **core**.)

### GitHub

```json
"com.hungnt.eventdispatcher": "https://github.com/HungNT-UPM/com.hungnt.eventdispatcher.git#v1.0.2"
```

**Phụ thuộc:** `com.hungnt.core` (+ Odin qua core).

## Demo

Assembly **`HungNT.EventDispatcher.Demo`** trong folder `Demo/`:

| Script | Vai trò |
|--------|---------|
| `EventDispatcherDemo_Dispatcher` | Gửi event (Context Menu / gọi từ code) |
| `EventDispatcherDemo_Listener` | Đăng ký handler trong `OnEnable` / `OnDisable` |
| `DemoEvents.cs` | Định nghĩa `OnGameStart`, `OnCoinChanged`, … |

### 1. Định nghĩa event

```csharp
public struct OnCoinChanged : IEvent
{
    public int OldValue;
    public int NewValue;
}
```

### 2. Listener

```csharp
private void OnEnable()
{
    EventDispatcher.Instance.Register<OnCoinChanged>(OnCoinChanged);
    // hoặc: this.Register<OnCoinChanged>(OnCoinChanged);
}

private void OnDisable()
{
    EventDispatcher.Instance.Unregister<OnCoinChanged>(OnCoinChanged);
}

private void OnCoinChanged(OnCoinChanged e)
    => Debug.Log($"Coin: {e.OldValue} → {e.NewValue}");
```

### 3. Dispatch

```csharp
// Signal (không data)
EventDispatcher.Instance.Dispatch<OnGameStart>();
this.Dispatch<OnGameStart>();

// Có data
EventDispatcher.Instance.Dispatch(new OnCoinChanged { OldValue = 10, NewValue = 20 });
this.Dispatch(new OnPlayerJump { JumpHeight = 3.5f });
```

**Thử nhanh:** tạo 2 GameObject, gắn `_Dispatcher` và `_Listener`, vào Play Mode → trên `_Dispatcher` dùng Context Menu *Dispatch …* hoặc mở inspector **`EventDispatcher`** singleton để xem listener.
