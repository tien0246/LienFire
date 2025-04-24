using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.StyleSheets;

[StructLayout(LayoutKind.Explicit)]
[DebuggerDisplay("id = {id}, keyword = {keyword}, number = {number}, boolean = {boolean}, color = {color}, object = {resource}")]
internal struct StyleValue
{
	[FieldOffset(0)]
	public StylePropertyId id;

	[FieldOffset(4)]
	public StyleKeyword keyword;

	[FieldOffset(8)]
	public float number;

	[FieldOffset(8)]
	public Length length;

	[FieldOffset(8)]
	public Color color;

	[FieldOffset(8)]
	public GCHandle resource;
}
