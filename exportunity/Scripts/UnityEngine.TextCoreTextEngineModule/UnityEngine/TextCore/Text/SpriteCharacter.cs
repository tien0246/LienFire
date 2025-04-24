using System;

namespace UnityEngine.TextCore.Text;

[Serializable]
public class SpriteCharacter : TextElement
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private int m_HashCode;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			if (!(value == m_Name))
			{
				m_Name = value;
				m_HashCode = TextUtilities.GetHashCodeCaseSensitive(m_Name);
			}
		}
	}

	public int hashCode => m_HashCode;

	public SpriteCharacter()
	{
		m_ElementType = TextElementType.Sprite;
	}

	public SpriteCharacter(uint unicode, SpriteGlyph glyph)
	{
		m_ElementType = TextElementType.Sprite;
		base.unicode = unicode;
		base.glyphIndex = glyph.index;
		base.glyph = glyph;
		base.scale = 1f;
	}

	public SpriteCharacter(uint unicode, SpriteAsset spriteAsset, SpriteGlyph glyph)
	{
		m_ElementType = TextElementType.Sprite;
		base.unicode = unicode;
		base.textAsset = spriteAsset;
		base.glyph = glyph;
		base.glyphIndex = glyph.index;
		base.scale = 1f;
	}

	internal SpriteCharacter(uint unicode, uint glyphIndex)
	{
		m_ElementType = TextElementType.Sprite;
		base.unicode = unicode;
		base.textAsset = null;
		base.glyph = null;
		base.glyphIndex = glyphIndex;
		base.scale = 1f;
	}
}
