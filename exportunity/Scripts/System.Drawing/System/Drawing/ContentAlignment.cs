using System.ComponentModel;
using System.Drawing.Design;

namespace System.Drawing;

[Editor("System.Drawing.Design.ContentAlignmentEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public enum ContentAlignment
{
	TopLeft = 1,
	TopCenter = 2,
	TopRight = 4,
	MiddleLeft = 0x10,
	MiddleCenter = 0x20,
	MiddleRight = 0x40,
	BottomLeft = 0x100,
	BottomCenter = 0x200,
	BottomRight = 0x400
}
