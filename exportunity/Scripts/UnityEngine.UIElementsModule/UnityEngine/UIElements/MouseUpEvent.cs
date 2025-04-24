namespace UnityEngine.UIElements;

public class MouseUpEvent : MouseEventBase<MouseUpEvent>
{
	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.Bubbles | EventPropagation.TricklesDown | EventPropagation.Cancellable | EventPropagation.SkipDisabledElements;
	}

	public MouseUpEvent()
	{
		LocalInit();
	}

	public new static MouseUpEvent GetPooled(Event systemEvent)
	{
		if (systemEvent != null)
		{
			PointerDeviceState.ReleaseButton(PointerId.mousePointerId, systemEvent.button);
		}
		return MouseEventBase<MouseUpEvent>.GetPooled(systemEvent);
	}

	private static MouseUpEvent MakeFromPointerEvent(IPointerEvent pointerEvent)
	{
		if (pointerEvent != null && pointerEvent.button >= 0)
		{
			PointerDeviceState.ReleaseButton(PointerId.mousePointerId, pointerEvent.button);
		}
		return MouseEventBase<MouseUpEvent>.GetPooled(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerUpEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerMoveEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerCancelEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}
}
