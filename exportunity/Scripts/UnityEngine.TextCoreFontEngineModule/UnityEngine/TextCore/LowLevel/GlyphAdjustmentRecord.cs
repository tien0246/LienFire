using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel;

[Serializable]
[UsedByNativeCode]
public struct GlyphAdjustmentRecord : IEquatable<GlyphAdjustmentRecord>
{
	[SerializeField]
	[NativeName("glyphIndex")]
	private uint m_GlyphIndex;

	[SerializeField]
	[NativeName("glyphValueRecord")]
	private GlyphValueRecord m_GlyphValueRecord;

	public uint glyphIndex
	{
		get
		{
			return m_GlyphIndex;
		}
		set
		{
			m_GlyphIndex = value;
		}
	}

	public GlyphValueRecord glyphValueRecord
	{
		get
		{
			return m_GlyphValueRecord;
		}
		set
		{
			m_GlyphValueRecord = value;
		}
	}

	public GlyphAdjustmentRecord(uint glyphIndex, GlyphValueRecord glyphValueRecord)
	{
		m_GlyphIndex = glyphIndex;
		m_GlyphValueRecord = glyphValueRecord;
	}

	[ExcludeFromDocs]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	[ExcludeFromDocs]
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	[ExcludeFromDocs]
	public bool Equals(GlyphAdjustmentRecord other)
	{
		return base.Equals((object)other);
	}

	[ExcludeFromDocs]
	public static bool operator ==(GlyphAdjustmentRecord lhs, GlyphAdjustmentRecord rhs)
	{
		return lhs.m_GlyphIndex == rhs.m_GlyphIndex && lhs.m_GlyphValueRecord == rhs.m_GlyphValueRecord;
	}

	[ExcludeFromDocs]
	public static bool operator !=(GlyphAdjustmentRecord lhs, GlyphAdjustmentRecord rhs)
	{
		return !(lhs == rhs);
	}
}
