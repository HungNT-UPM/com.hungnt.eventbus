# com.hungnt.eventbus

Event bus type-safe cho Unity: struct implement **`IEvent`**, đăng ký/lắng nghe qua singleton **`EventBus`**, có extension trên `MonoBehaviour` và inspector Odin xem listener live.

## Tính năng

- **`EventBus`** — `MonoSingleton<EventBus>`
- **`Register` / `Unregister` / `Dispatch`** — generic theo kiểu event
- **Signal events** — struct rỗng (`Dispatch<OnGameStart>()`)
- **Events có data** — struct chứa field (`Dispatch(new OnCoinChanged { ... })`)
- **`EventBusExtensions`** — `this.Register`, `this.Unregister`, `this.Dispatch` trên `MonoBehaviour`
- Editor: inspector liệt kê listener đang active (Play Mode)

## Yêu cầu

- Unity 2022.3+
- `com.hungnt.core` ≥ 1.0.6
- Odin Inspector (optional — editor inspector gracefully disabled nếu không có)

## Cài đặt

Thêm vào `manifest.json`:

```json
{
  "dependencies": {
    "com.hungnt.eventbus": "1.0.7"
  },
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": ["com.hungnt"]
    }
  ]
}
```

## Sử dụng

### 1. Định nghĩa event

```csharp
// Signal event (không có data)
public struct OnGameStart : IEvent { }

// Event có data
public struct OnCoinChanged : IEvent
{
    public int OldValue;
    public int NewValue;
}
```

### 2. Listener (MonoBehaviour)

```csharp
private void OnEnable()
{
    this.Register<OnCoinChanged>(OnCoinChanged);
}

private void OnDisable()
{
    this.Unregister<OnCoinChanged>(OnCoinChanged);
}

private void OnCoinChanged(OnCoinChanged e)
    => Debug.Log($"Coin: {e.OldValue} → {e.NewValue}");
```

### 3. Dispatch

```csharp
// Signal
this.Dispatch<OnGameStart>();

// Event có data
this.Dispatch(new OnCoinChanged { OldValue = 10, NewValue = 20 });

// Direct (non-MonoBehaviour context)
EventBus.Instance.Dispatch(new OnCoinChanged { OldValue = 0, NewValue = 5 });
```

## Demo

Assembly **`HungNT.EventBus.Demo`** trong folder `Demo/`:

| Script | Vai trò |
|--------|---------|
| `EventBusDemo_Dispatcher` | Gửi event (Context Menu / gọi từ code) |
| `EventBusDemo_Listener` | Đăng ký handler trong `OnEnable` / `OnDisable` |
| `DemoEvents.cs` | Định nghĩa `OnGameStart`, `OnCoinChanged`, … |

**Thử nhanh:** tạo 2 GameObject, gắn `_Dispatcher` và `_Listener`, vào Play Mode → Context Menu *Dispatch …* hoặc mở inspector **`EventBus`** singleton.

## Changelog

### v1.0.7
- **Rename:** `com.hungnt.eventdispatcher` → `com.hungnt.eventbus`
- **Rename:** class `EventDispatcher` → `EventBus`, namespace `HungNT.EventDispatcher` → `HungNT.EventBus`
- **Rename:** `EventDispatcherExtensions` → `EventBusExtensions`
- **Rename:** assembly `HungNT.EventDispatcher` → `HungNT.EventBus`

### v1.0.6
- Loại bỏ `GetInvocationList()` alloc — dùng `List<Delegate>` per type
- `SafeInvoke` bỏ qua destroyed Unity objects

### v1.0.5
- Thêm Odin Editor inspector (live listener list trong Play Mode)
