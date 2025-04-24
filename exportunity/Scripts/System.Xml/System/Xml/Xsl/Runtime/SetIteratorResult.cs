using System.ComponentModel;

namespace System.Xml.Xsl.Runtime;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum SetIteratorResult
{
	NoMoreNodes = 0,
	InitRightIterator = 1,
	NeedLeftNode = 2,
	NeedRightNode = 3,
	HaveCurrentNode = 4
}
