namespace UnityEngine.UIElements;

internal class NavigationTabEvent : NavigationEventBase<NavigationTabEvent>
{
	public enum Direction
	{
		None = 0,
		Next = 1,
		Previous = 2
	}

	public Direction direction { get; private set; }

	internal static Direction DetermineMoveDirection(int moveValue)
	{
		return (moveValue > 0) ? Direction.Next : ((moveValue < 0) ? Direction.Previous : Direction.None);
	}

	public static NavigationTabEvent GetPooled(int moveValue)
	{
		NavigationTabEvent navigationTabEvent = EventBase<NavigationTabEvent>.GetPooled();
		navigationTabEvent.direction = DetermineMoveDirection(moveValue);
		return navigationTabEvent;
	}

	protected override void Init()
	{
		base.Init();
		direction = Direction.None;
	}

	public NavigationTabEvent()
	{
		Init();
	}
}
