using System;

namespace UnityEngine.UIElements;

internal struct StyleDataRef<T> : IEquatable<StyleDataRef<T>> where T : struct, IEquatable<T>, IStyleDataGroup<T>
{
	private class RefCounted
	{
		private static uint m_NextId = 1u;

		private int m_RefCount;

		private readonly uint m_Id;

		public T value;

		public int refCount => m_RefCount;

		public uint id => m_Id;

		public RefCounted()
		{
			m_RefCount = 1;
			m_Id = ++m_NextId;
		}

		public void Acquire()
		{
			m_RefCount++;
		}

		public void Release()
		{
			m_RefCount--;
		}

		public RefCounted Copy()
		{
			return new RefCounted
			{
				value = value.Copy()
			};
		}
	}

	private RefCounted m_Ref;

	public int refCount => m_Ref?.refCount ?? 0;

	public uint id => m_Ref?.id ?? 0;

	public StyleDataRef<T> Acquire()
	{
		m_Ref.Acquire();
		return this;
	}

	public void Release()
	{
		m_Ref.Release();
		m_Ref = null;
	}

	public void CopyFrom(StyleDataRef<T> other)
	{
		if (m_Ref.refCount == 1)
		{
			m_Ref.value.CopyFrom(ref other.m_Ref.value);
			return;
		}
		m_Ref.Release();
		m_Ref = other.m_Ref;
		m_Ref.Acquire();
	}

	public ref readonly T Read()
	{
		return ref m_Ref.value;
	}

	public ref T Write()
	{
		if (m_Ref.refCount == 1)
		{
			return ref m_Ref.value;
		}
		RefCounted refCounted = m_Ref;
		m_Ref = m_Ref.Copy();
		refCounted.Release();
		return ref m_Ref.value;
	}

	public static StyleDataRef<T> Create()
	{
		return new StyleDataRef<T>
		{
			m_Ref = new RefCounted()
		};
	}

	public override int GetHashCode()
	{
		return (m_Ref != null) ? m_Ref.value.GetHashCode() : 0;
	}

	public static bool operator ==(StyleDataRef<T> lhs, StyleDataRef<T> rhs)
	{
		return lhs.m_Ref == rhs.m_Ref || lhs.m_Ref.value.Equals(rhs.m_Ref.value);
	}

	public static bool operator !=(StyleDataRef<T> lhs, StyleDataRef<T> rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(StyleDataRef<T> other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is StyleDataRef<T> other && Equals(other);
	}
}
