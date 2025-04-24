using System.ComponentModel;

namespace System.Xml.Xsl.Runtime;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum IteratorResult
{
	NoMoreNodes = 0,
	NeedInputNode = 1,
	HaveCurrentNode = 2
}
