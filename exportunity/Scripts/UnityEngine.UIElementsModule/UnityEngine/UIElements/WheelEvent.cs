namespace UnityEngine.UIElements;

public class WheelEvent : MouseEventBase<WheelEvent>
{
	public Vector3 delta { get; private set; }

	public new static WheelEvent GetPooled(Event systemEvent)
	{
		WheelEvent wheelEvent = MouseEventBase<WheelEvent>.GetPooled(systemEvent);
		wheelEvent.imguiEvent = systemEvent;
		if (systemEvent != null)
		{
			wheelEvent.delta = systemEvent.delta;
		}
		return wheelEvent;
	}

	internal static WheelEvent GetPooled(Vector3 delta, Vector3 mousePosition)
	{
		WheelEvent wheelEvent = EventBase<WheelEvent>.GetPooled();
		wheelEvent.delta = delta;
		wheelEvent.mousePosition = mousePosition;
		return wheelEvent;
	}

	internal static WheelEvent GetPooled(Vector3 delta, IPointerEvent pointerEvent)
	{
		WheelEvent wheelEvent = MouseEventBase<WheelEvent>.GetPooled(pointerEvent);
		wheelEvent.delta = delta;
		return wheelEvent;
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.Bubbles | EventPropagation.TricklesDown | EventPropagation.Cancellable | EventPropagation.SkipDisabledElements;
		delta = Vector3.zero;
	}

	public WheelEvent()
	{
		LocalInit();
	}
}
