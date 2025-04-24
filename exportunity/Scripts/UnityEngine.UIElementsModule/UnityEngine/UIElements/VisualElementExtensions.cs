using System;

namespace UnityEngine.UIElements;

public static class VisualElementExtensions
{
	public static Vector2 WorldToLocal(this VisualElement ele, Vector2 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.MultiplyMatrix44Point2(ref ele.worldTransformInverse, p);
	}

	public static Vector2 LocalToWorld(this VisualElement ele, Vector2 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.MultiplyMatrix44Point2(ref ele.worldTransformRef, p);
	}

	public static Rect WorldToLocal(this VisualElement ele, Rect r)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.CalculateConservativeRect(ref ele.worldTransformInverse, r);
	}

	public static Rect LocalToWorld(this VisualElement ele, Rect r)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.CalculateConservativeRect(ref ele.worldTransformRef, r);
	}

	public static Vector2 ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Vector2 point)
	{
		return dest.WorldToLocal(src.LocalToWorld(point));
	}

	public static Rect ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Rect rect)
	{
		return dest.WorldToLocal(src.LocalToWorld(rect));
	}

	public static void StretchToParentSize(this VisualElement elem)
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		IStyle style = elem.style;
		style.position = Position.Absolute;
		style.left = 0f;
		style.top = 0f;
		style.right = 0f;
		style.bottom = 0f;
	}

	public static void StretchToParentWidth(this VisualElement elem)
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		IStyle style = elem.style;
		style.position = Position.Absolute;
		style.left = 0f;
		style.right = 0f;
	}

	public static void AddManipulator(this VisualElement ele, IManipulator manipulator)
	{
		if (manipulator != null)
		{
			manipulator.target = ele;
		}
	}

	public static void RemoveManipulator(this VisualElement ele, IManipulator manipulator)
	{
		if (manipulator != null)
		{
			manipulator.target = null;
		}
	}
}
