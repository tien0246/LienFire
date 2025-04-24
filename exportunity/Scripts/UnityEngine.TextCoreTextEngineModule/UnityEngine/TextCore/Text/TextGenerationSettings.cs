using System;

namespace UnityEngine.TextCore.Text;

internal class TextGenerationSettings : IEquatable<TextGenerationSettings>
{
	public string text;

	public Rect screenRect;

	public Vector4 margins;

	public float scale = 1f;

	public FontAsset fontAsset;

	public Material material;

	public SpriteAsset spriteAsset;

	public TextStyleSheet styleSheet;

	public FontStyles fontStyle = FontStyles.Normal;

	public TextSettings textSettings;

	public TextAlignment textAlignment = TextAlignment.TopLeft;

	public TextOverflowMode overflowMode = TextOverflowMode.Overflow;

	public bool wordWrap = false;

	public float wordWrappingRatio;

	public Color color = Color.white;

	public TextColorGradient fontColorGradient;

	public bool tintSprites;

	public bool overrideRichTextColors;

	public float fontSize = 18f;

	public bool autoSize;

	public float fontSizeMin;

	public float fontSizeMax;

	public bool enableKerning = true;

	public bool richText;

	public bool isRightToLeft;

	public bool extraPadding;

	public bool parseControlCharacters = true;

	public float characterSpacing;

	public float wordSpacing;

	public float lineSpacing;

	public float paragraphSpacing;

	public float lineSpacingMax;

	public int maxVisibleCharacters = 99999;

	public int maxVisibleWords = 99999;

	public int maxVisibleLines = 99999;

	public int firstVisibleCharacter = 0;

	public bool useMaxVisibleDescender;

	public TextFontWeight fontWeight = TextFontWeight.Regular;

	public int pageToDisplay = 1;

	public TextureMapping horizontalMapping = TextureMapping.Character;

	public TextureMapping verticalMapping = TextureMapping.Character;

	public float uvLineOffset;

	public VertexSortingOrder geometrySortingOrder = VertexSortingOrder.Normal;

	public bool inverseYAxis;

	public float charWidthMaxAdj;

	public bool Equals(TextGenerationSettings other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		return text == other.text && screenRect.Equals(other.screenRect) && margins.Equals(other.margins) && scale.Equals(other.scale) && object.Equals(fontAsset, other.fontAsset) && object.Equals(material, other.material) && object.Equals(spriteAsset, other.spriteAsset) && object.Equals(styleSheet, other.styleSheet) && fontStyle == other.fontStyle && object.Equals(textSettings, other.textSettings) && textAlignment == other.textAlignment && overflowMode == other.overflowMode && wordWrap == other.wordWrap && wordWrappingRatio.Equals(other.wordWrappingRatio) && color.Equals(other.color) && object.Equals(fontColorGradient, other.fontColorGradient) && tintSprites == other.tintSprites && overrideRichTextColors == other.overrideRichTextColors && fontSize.Equals(other.fontSize) && autoSize == other.autoSize && fontSizeMin.Equals(other.fontSizeMin) && fontSizeMax.Equals(other.fontSizeMax) && enableKerning == other.enableKerning && richText == other.richText && isRightToLeft == other.isRightToLeft && extraPadding == other.extraPadding && parseControlCharacters == other.parseControlCharacters && characterSpacing.Equals(other.characterSpacing) && wordSpacing.Equals(other.wordSpacing) && lineSpacing.Equals(other.lineSpacing) && paragraphSpacing.Equals(other.paragraphSpacing) && lineSpacingMax.Equals(other.lineSpacingMax) && maxVisibleCharacters == other.maxVisibleCharacters && maxVisibleWords == other.maxVisibleWords && maxVisibleLines == other.maxVisibleLines && firstVisibleCharacter == other.firstVisibleCharacter && useMaxVisibleDescender == other.useMaxVisibleDescender && fontWeight == other.fontWeight && pageToDisplay == other.pageToDisplay && horizontalMapping == other.horizontalMapping && verticalMapping == other.verticalMapping && uvLineOffset.Equals(other.uvLineOffset) && geometrySortingOrder == other.geometrySortingOrder && inverseYAxis == other.inverseYAxis && charWidthMaxAdj.Equals(other.charWidthMaxAdj);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if ((object)obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((TextGenerationSettings)obj);
	}

	public override int GetHashCode()
	{
		int num = ((text != null) ? text.GetHashCode() : 0);
		num = (num * 397) ^ screenRect.GetHashCode();
		num = (num * 397) ^ margins.GetHashCode();
		num = (num * 397) ^ scale.GetHashCode();
		num = (num * 397) ^ ((fontAsset != null) ? fontAsset.GetHashCode() : 0);
		num = (num * 397) ^ ((material != null) ? material.GetHashCode() : 0);
		num = (num * 397) ^ ((spriteAsset != null) ? spriteAsset.GetHashCode() : 0);
		num = (num * 397) ^ (int)fontStyle;
		num = (num * 397) ^ ((textSettings != null) ? textSettings.GetHashCode() : 0);
		num = (num * 397) ^ (int)textAlignment;
		num = (num * 397) ^ (int)overflowMode;
		num = (num * 397) ^ wordWrap.GetHashCode();
		num = (num * 397) ^ wordWrappingRatio.GetHashCode();
		num = (num * 397) ^ color.GetHashCode();
		num = (num * 397) ^ ((fontColorGradient != null) ? fontColorGradient.GetHashCode() : 0);
		num = (num * 397) ^ tintSprites.GetHashCode();
		num = (num * 397) ^ overrideRichTextColors.GetHashCode();
		num = (num * 397) ^ fontSize.GetHashCode();
		num = (num * 397) ^ autoSize.GetHashCode();
		num = (num * 397) ^ fontSizeMin.GetHashCode();
		num = (num * 397) ^ fontSizeMax.GetHashCode();
		num = (num * 397) ^ enableKerning.GetHashCode();
		num = (num * 397) ^ richText.GetHashCode();
		num = (num * 397) ^ isRightToLeft.GetHashCode();
		num = (num * 397) ^ extraPadding.GetHashCode();
		num = (num * 397) ^ parseControlCharacters.GetHashCode();
		num = (num * 397) ^ characterSpacing.GetHashCode();
		num = (num * 397) ^ wordSpacing.GetHashCode();
		num = (num * 397) ^ lineSpacing.GetHashCode();
		num = (num * 397) ^ paragraphSpacing.GetHashCode();
		num = (num * 397) ^ lineSpacingMax.GetHashCode();
		num = (num * 397) ^ maxVisibleCharacters;
		num = (num * 397) ^ maxVisibleWords;
		num = (num * 397) ^ maxVisibleLines;
		num = (num * 397) ^ firstVisibleCharacter;
		num = (num * 397) ^ useMaxVisibleDescender.GetHashCode();
		num = (num * 397) ^ (int)fontWeight;
		num = (num * 397) ^ pageToDisplay;
		num = (num * 397) ^ (int)horizontalMapping;
		num = (num * 397) ^ (int)verticalMapping;
		num = (num * 397) ^ uvLineOffset.GetHashCode();
		num = (num * 397) ^ (int)geometrySortingOrder;
		num = (num * 397) ^ inverseYAxis.GetHashCode();
		return (num * 397) ^ charWidthMaxAdj.GetHashCode();
	}

	public static bool operator ==(TextGenerationSettings left, TextGenerationSettings right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(TextGenerationSettings left, TextGenerationSettings right)
	{
		return !object.Equals(left, right);
	}

	public override string ToString()
	{
		return string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}\n {8}: {9}\n {10}: {11}\n {12}: {13}\n {14}: {15}\n {16}: {17}\n {18}: {19}\n {20}: {21}\n {22}: {23}\n {24}: {25}\n {26}: {27}\n {28}: {29}\n {30}: {31}\n {32}: {33}\n {34}: {35}\n  {36}: {37}\n {38}: {39}\n {40}: {41}\n {42}: {43}\n {44}: {45}\n {46}: {47}\n {48}: {49}\n {50}: {51}\n {52}: {53}\n {54}: {55}\n {56}: {57}\n {58}: {59}\n {60}: {61}\n {62}: {63}\n {64}: {65}\n {66}: {67}\n {68}: {69}\n {70}: {71}\n {72}: {73}\n {74}: {75}\n {76}: {77}\n {78}: {79}\n {80}: {81}\n {82}: {83}\n {84}: {85}\n {86}: {87}\n {88}: {89}", "text", text, "screenRect", screenRect, "margins", margins, "scale", scale, "fontAsset", fontAsset, "material", material, "spriteAsset", spriteAsset, "styleSheet", styleSheet, "fontStyle", fontStyle, "textSettings", textSettings, "textAlignment", textAlignment, "overflowMode", overflowMode, "wordWrap", wordWrap, "wordWrappingRatio", wordWrappingRatio, "color", color, "fontColorGradient", fontColorGradient, "tintSprites", tintSprites, "overrideRichTextColors", overrideRichTextColors, "fontSize", fontSize, "autoSize", autoSize, "fontSizeMin", fontSizeMin, "fontSizeMax", fontSizeMax, "enableKerning", enableKerning, "richText", richText, "isRightToLeft", isRightToLeft, "extraPadding", extraPadding, "parseControlCharacters", parseControlCharacters, "characterSpacing", characterSpacing, "wordSpacing", wordSpacing, "lineSpacing", lineSpacing, "paragraphSpacing", paragraphSpacing, "lineSpacingMax", lineSpacingMax, "maxVisibleCharacters", maxVisibleCharacters, "maxVisibleWords", maxVisibleWords, "maxVisibleLines", maxVisibleLines, "firstVisibleCharacter", firstVisibleCharacter, "useMaxVisibleDescender", useMaxVisibleDescender, "fontWeight", fontWeight, "pageToDisplay", pageToDisplay, "horizontalMapping", horizontalMapping, "verticalMapping", verticalMapping, "uvLineOffset", uvLineOffset, "geometrySortingOrder", geometrySortingOrder, "inverseYAxis", inverseYAxis, "charWidthMaxAdj", charWidthMaxAdj);
	}
}
