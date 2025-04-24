using System;

namespace UnityEngine.UIElements;

internal static class MeshGenerationContextUtils
{
	public struct BorderParams
	{
		public Rect rect;

		public Color playmodeTintColor;

		public Color leftColor;

		public Color topColor;

		public Color rightColor;

		public Color bottomColor;

		public float leftWidth;

		public float topWidth;

		public float rightWidth;

		public float bottomWidth;

		public Vector2 topLeftRadius;

		public Vector2 topRightRadius;

		public Vector2 bottomRightRadius;

		public Vector2 bottomLeftRadius;

		public Material material;

		internal ColorPage leftColorPage;

		internal ColorPage topColorPage;

		internal ColorPage rightColorPage;

		internal ColorPage bottomColorPage;
	}

	public struct RectangleParams
	{
		public Rect rect;

		public Rect uv;

		public Color color;

		public Texture texture;

		public Sprite sprite;

		public VectorImage vectorImage;

		public Material material;

		public ScaleMode scaleMode;

		public Color playmodeTintColor;

		public Vector2 topLeftRadius;

		public Vector2 topRightRadius;

		public Vector2 bottomRightRadius;

		public Vector2 bottomLeftRadius;

		public int leftSlice;

		public int topSlice;

		public int rightSlice;

		public int bottomSlice;

		public float sliceScale;

		internal Rect spriteGeomRect;

		internal ColorPage colorPage;

		internal MeshGenerationContext.MeshFlags meshFlags;

		public static RectangleParams MakeSolid(Rect rect, Color color, ContextType panelContext)
		{
			Color color2 = ((panelContext == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			return new RectangleParams
			{
				rect = rect,
				color = color,
				uv = new Rect(0f, 0f, 1f, 1f),
				playmodeTintColor = color2
			};
		}

		private static void AdjustUVsForScaleMode(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
		{
			float num = Mathf.Abs((float)texture.width * uv.width / ((float)texture.height * uv.height));
			float num2 = rect.width / rect.height;
			switch (scaleMode)
			{
			case ScaleMode.ScaleAndCrop:
				if (num2 > num)
				{
					float num5 = uv.height * (num / num2);
					float num6 = (uv.height - num5) * 0.5f;
					uv = new Rect(uv.x, uv.y + num6, uv.width, num5);
				}
				else
				{
					float num7 = uv.width * (num2 / num);
					float num8 = (uv.width - num7) * 0.5f;
					uv = new Rect(uv.x + num8, uv.y, num7, uv.height);
				}
				break;
			case ScaleMode.ScaleToFit:
				if (num2 > num)
				{
					float num3 = num / num2;
					rect = new Rect(rect.xMin + rect.width * (1f - num3) * 0.5f, rect.yMin, num3 * rect.width, rect.height);
				}
				else
				{
					float num4 = num2 / num;
					rect = new Rect(rect.xMin, rect.yMin + rect.height * (1f - num4) * 0.5f, rect.width, num4 * rect.height);
				}
				break;
			default:
				throw new NotImplementedException();
			case ScaleMode.StretchToFill:
				break;
			}
			rectOut = rect;
			uvOut = uv;
		}

		private static void AdjustSpriteUVsForScaleMode(Rect containerRect, Rect srcRect, Rect spriteGeomRect, Sprite sprite, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
		{
			float num = sprite.rect.width / sprite.rect.height;
			float num2 = containerRect.width / containerRect.height;
			Rect rect = spriteGeomRect;
			rect.position -= (Vector2)sprite.bounds.min;
			rect.position /= (Vector2)sprite.bounds.size;
			rect.size /= (Vector2)sprite.bounds.size;
			Vector2 position = rect.position;
			position.y = 1f - rect.size.y - position.y;
			rect.position = position;
			switch (scaleMode)
			{
			case ScaleMode.StretchToFill:
			{
				Vector2 size2 = containerRect.size;
				containerRect.position = rect.position * size2;
				containerRect.size = rect.size * size2;
				break;
			}
			case ScaleMode.ScaleAndCrop:
			{
				Rect b = containerRect;
				if (num2 > num)
				{
					b.height = b.width / num;
					b.position = new Vector2(b.position.x, (0f - (b.height - containerRect.height)) / 2f);
				}
				else
				{
					b.width = b.height * num;
					b.position = new Vector2((0f - (b.width - containerRect.width)) / 2f, b.position.y);
				}
				Vector2 size = b.size;
				b.position += rect.position * size;
				b.size = rect.size * size;
				Rect rect2 = RectIntersection(containerRect, b);
				if (rect2.width < 1E-30f || rect2.height < 1E-30f)
				{
					rect2 = Rect.zero;
				}
				else
				{
					Rect rect3 = rect2;
					rect3.position -= b.position;
					rect3.position /= b.size;
					rect3.size /= b.size;
					Vector2 position2 = rect3.position;
					position2.y = 1f - rect3.size.y - position2.y;
					rect3.position = position2;
					srcRect.position += rect3.position * srcRect.size;
					srcRect.size *= rect3.size;
				}
				containerRect = rect2;
				break;
			}
			case ScaleMode.ScaleToFit:
				if (num2 > num)
				{
					float num3 = num / num2;
					containerRect = new Rect(containerRect.xMin + containerRect.width * (1f - num3) * 0.5f, containerRect.yMin, num3 * containerRect.width, containerRect.height);
				}
				else
				{
					float num4 = num2 / num;
					containerRect = new Rect(containerRect.xMin, containerRect.yMin + containerRect.height * (1f - num4) * 0.5f, containerRect.width, num4 * containerRect.height);
				}
				containerRect.position += rect.position * containerRect.size;
				containerRect.size *= rect.size;
				break;
			default:
				throw new NotImplementedException();
			}
			rectOut = containerRect;
			uvOut = srcRect;
		}

		private static Rect RectIntersection(Rect a, Rect b)
		{
			Rect zero = Rect.zero;
			zero.min = Vector2.Max(a.min, b.min);
			zero.max = Vector2.Min(a.max, b.max);
			zero.size = Vector2.Max(zero.size, Vector2.zero);
			return zero;
		}

		private static Rect ComputeGeomRect(Sprite sprite)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			Vector2[] vertices = sprite.vertices;
			foreach (Vector2 rhs in vertices)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector, vector2 - vector);
		}

		private static Rect ComputeUVRect(Sprite sprite)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			Vector2[] array = sprite.uv;
			foreach (Vector2 rhs in array)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector, vector2 - vector);
		}

		private static Rect ApplyPackingRotation(Rect uv, SpritePackingRotation rotation)
		{
			switch (rotation)
			{
			case SpritePackingRotation.FlipHorizontal:
			{
				uv.position += new Vector2(uv.size.x, 0f);
				Vector2 size2 = uv.size;
				size2.x = 0f - size2.x;
				uv.size = size2;
				break;
			}
			case SpritePackingRotation.FlipVertical:
			{
				uv.position += new Vector2(0f, uv.size.y);
				Vector2 size = uv.size;
				size.y = 0f - size.y;
				uv.size = size;
				break;
			}
			case SpritePackingRotation.Rotate180:
				uv.position += uv.size;
				uv.size = -uv.size;
				break;
			}
			return uv;
		}

		public static RectangleParams MakeTextured(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, ContextType panelContext)
		{
			Color color = ((panelContext == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			AdjustUVsForScaleMode(rect, uv, texture, scaleMode, out rect, out uv);
			return new RectangleParams
			{
				rect = rect,
				uv = uv,
				color = Color.white,
				texture = texture,
				scaleMode = scaleMode,
				playmodeTintColor = color
			};
		}

		public static RectangleParams MakeSprite(Rect containerRect, Rect subRect, Sprite sprite, ScaleMode scaleMode, ContextType panelContext, bool hasRadius, ref Vector4 slices)
		{
			if (sprite.texture == null)
			{
				Debug.LogWarning("Ignoring textureless sprite named \"" + sprite.name + "\", please import as a VectorImage instead");
				return default(RectangleParams);
			}
			Color color = ((panelContext == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			Rect rect = ComputeGeomRect(sprite);
			Rect rect2 = ComputeUVRect(sprite);
			Vector4 border = sprite.border;
			bool flag = border != Vector4.zero || slices != Vector4.zero;
			bool flag2 = subRect != new Rect(0f, 0f, 1f, 1f);
			bool flag3 = scaleMode == ScaleMode.ScaleAndCrop || flag || hasRadius || flag2;
			if (flag3 && sprite.packed && sprite.packingRotation != SpritePackingRotation.None)
			{
				rect2 = ApplyPackingRotation(rect2, sprite.packingRotation);
			}
			Rect srcRect;
			if (flag2)
			{
				srcRect = subRect;
				srcRect.position *= rect2.size;
				srcRect.position += rect2.position;
				srcRect.size *= rect2.size;
			}
			else
			{
				srcRect = rect2;
			}
			AdjustSpriteUVsForScaleMode(containerRect, srcRect, rect, sprite, scaleMode, out var rectOut, out var uvOut);
			RectangleParams result = new RectangleParams
			{
				rect = rectOut,
				uv = uvOut,
				color = Color.white,
				texture = (flag3 ? sprite.texture : null),
				sprite = (flag3 ? null : sprite),
				spriteGeomRect = rect,
				scaleMode = scaleMode,
				playmodeTintColor = color,
				meshFlags = (sprite.packed ? MeshGenerationContext.MeshFlags.SkipDynamicAtlas : MeshGenerationContext.MeshFlags.None)
			};
			Vector4 vector = new Vector4(border.x, border.w, border.z, border.y);
			if (slices != Vector4.zero && vector != Vector4.zero && vector != slices)
			{
				Debug.LogWarning($"Sprite \"{sprite.name}\" borders {vector} are overridden by style slices {slices}");
			}
			else if (slices == Vector4.zero)
			{
				slices = vector;
			}
			return result;
		}

		public static RectangleParams MakeVectorTextured(Rect rect, Rect uv, VectorImage vectorImage, ScaleMode scaleMode, ContextType panelContext)
		{
			Color color = ((panelContext == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			return new RectangleParams
			{
				rect = rect,
				uv = uv,
				color = Color.white,
				vectorImage = vectorImage,
				scaleMode = scaleMode,
				playmodeTintColor = color
			};
		}

		internal bool HasRadius(float epsilon)
		{
			return (topLeftRadius.x > epsilon && topLeftRadius.y > epsilon) || (topRightRadius.x > epsilon && topRightRadius.y > epsilon) || (bottomRightRadius.x > epsilon && bottomRightRadius.y > epsilon) || (bottomLeftRadius.x > epsilon && bottomLeftRadius.y > epsilon);
		}
	}

	public struct TextParams
	{
		public Rect rect;

		public string text;

		public Font font;

		public FontDefinition fontDefinition;

		public int fontSize;

		public Length letterSpacing;

		public Length wordSpacing;

		public Length paragraphSpacing;

		public FontStyle fontStyle;

		public Color fontColor;

		public TextAnchor anchor;

		public bool wordWrap;

		public float wordWrapWidth;

		public bool richText;

		public Color playmodeTintColor;

		public TextOverflow textOverflow;

		public TextOverflowPosition textOverflowPosition;

		public OverflowInternal overflow;

		public IPanel panel;

		public override int GetHashCode()
		{
			int hashCode = rect.GetHashCode();
			hashCode = (hashCode * 397) ^ ((text != null) ? text.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ ((font != null) ? font.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ fontDefinition.GetHashCode();
			hashCode = (hashCode * 397) ^ fontSize;
			hashCode = (hashCode * 397) ^ (int)fontStyle;
			hashCode = (hashCode * 397) ^ fontColor.GetHashCode();
			hashCode = (hashCode * 397) ^ (int)anchor;
			hashCode = (hashCode * 397) ^ wordWrap.GetHashCode();
			hashCode = (hashCode * 397) ^ wordWrapWidth.GetHashCode();
			hashCode = (hashCode * 397) ^ richText.GetHashCode();
			hashCode = (hashCode * 397) ^ playmodeTintColor.GetHashCode();
			hashCode = (hashCode * 397) ^ textOverflow.GetHashCode();
			hashCode = (hashCode * 397) ^ textOverflowPosition.GetHashCode();
			hashCode = (hashCode * 397) ^ overflow.GetHashCode();
			hashCode = (hashCode * 397) ^ letterSpacing.GetHashCode();
			hashCode = (hashCode * 397) ^ wordSpacing.GetHashCode();
			return (hashCode * 397) ^ paragraphSpacing.GetHashCode();
		}

		internal static TextParams MakeStyleBased(VisualElement ve, string text)
		{
			ComputedStyle computedStyle = ve.computedStyle;
			TextElement textElement = ve as TextElement;
			bool flag = textElement == null;
			TextParams result = new TextParams
			{
				rect = ve.contentRect,
				text = text,
				fontDefinition = computedStyle.unityFontDefinition,
				font = TextUtilities.GetFont(ve),
				fontSize = (int)computedStyle.fontSize.value,
				fontStyle = computedStyle.unityFontStyleAndWeight,
				fontColor = computedStyle.color,
				anchor = computedStyle.unityTextAlign,
				wordWrap = (computedStyle.whiteSpace == WhiteSpace.Normal),
				wordWrapWidth = ((computedStyle.whiteSpace == WhiteSpace.Normal) ? ve.contentRect.width : 0f),
				richText = (textElement?.enableRichText ?? false)
			};
			IPanel obj = ve.panel;
			result.playmodeTintColor = ((obj != null && obj.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			result.textOverflow = computedStyle.textOverflow;
			result.textOverflowPosition = computedStyle.unityTextOverflowPosition;
			result.overflow = computedStyle.overflow;
			result.letterSpacing = (flag ? ((Length)0f) : computedStyle.letterSpacing);
			result.wordSpacing = (flag ? ((Length)0f) : computedStyle.wordSpacing);
			result.paragraphSpacing = (flag ? ((Length)0f) : computedStyle.unityParagraphSpacing);
			result.panel = ve.panel;
			return result;
		}

		internal static TextNativeSettings GetTextNativeSettings(TextParams textParams, float scaling)
		{
			return new TextNativeSettings
			{
				text = textParams.text,
				font = TextUtilities.GetFont(textParams),
				size = textParams.fontSize,
				scaling = scaling,
				style = textParams.fontStyle,
				color = textParams.fontColor,
				anchor = textParams.anchor,
				wordWrap = textParams.wordWrap,
				wordWrapWidth = textParams.wordWrapWidth,
				richText = textParams.richText
			};
		}
	}

	public static void Rectangle(this MeshGenerationContext mgc, RectangleParams rectParams)
	{
		mgc.painter.DrawRectangle(rectParams);
	}

	public static void Border(this MeshGenerationContext mgc, BorderParams borderParams)
	{
		mgc.painter.DrawBorder(borderParams);
	}

	public static void Text(this MeshGenerationContext mgc, TextParams textParams, ITextHandle handle, float pixelsPerPoint)
	{
		if (TextUtilities.IsFontAssigned(textParams))
		{
			mgc.painter.DrawText(textParams, handle, pixelsPerPoint);
		}
	}

	private static Vector2 ConvertBorderRadiusPercentToPoints(Vector2 borderRectSize, Length length)
	{
		float a = length.value;
		float a2 = length.value;
		if (length.unit == LengthUnit.Percent)
		{
			a = borderRectSize.x * length.value / 100f;
			a2 = borderRectSize.y * length.value / 100f;
		}
		a = Mathf.Max(a, 0f);
		a2 = Mathf.Max(a2, 0f);
		return new Vector2(a, a2);
	}

	public static void GetVisualElementRadii(VisualElement ve, out Vector2 topLeft, out Vector2 bottomLeft, out Vector2 topRight, out Vector2 bottomRight)
	{
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		Vector2 borderRectSize = new Vector2(resolvedStyle.width, resolvedStyle.height);
		ComputedStyle computedStyle = ve.computedStyle;
		topLeft = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderTopLeftRadius);
		bottomLeft = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderBottomLeftRadius);
		topRight = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderTopRightRadius);
		bottomRight = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderBottomRightRadius);
	}

	public static void AdjustBackgroundSizeForBorders(VisualElement visualElement, ref Rect rect)
	{
		IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
		if (resolvedStyle.borderLeftWidth >= 1f && resolvedStyle.borderLeftColor.a >= 1f)
		{
			rect.x += 0.5f;
			rect.width -= 0.5f;
		}
		if (resolvedStyle.borderTopWidth >= 1f && resolvedStyle.borderTopColor.a >= 1f)
		{
			rect.y += 0.5f;
			rect.height -= 0.5f;
		}
		if (resolvedStyle.borderRightWidth >= 1f && resolvedStyle.borderRightColor.a >= 1f)
		{
			rect.width -= 0.5f;
		}
		if (resolvedStyle.borderBottomWidth >= 1f && resolvedStyle.borderBottomColor.a >= 1f)
		{
			rect.height -= 0.5f;
		}
	}
}
