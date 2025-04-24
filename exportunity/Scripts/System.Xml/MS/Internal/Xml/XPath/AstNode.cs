using System.Xml.XPath;

namespace MS.Internal.Xml.XPath;

internal abstract class AstNode
{
	public enum AstType
	{
		Axis = 0,
		Operator = 1,
		Filter = 2,
		ConstantOperand = 3,
		Function = 4,
		Group = 5,
		Root = 6,
		Variable = 7,
		Error = 8
	}

	public abstract AstType Type { get; }

	public abstract XPathResultType ReturnType { get; }
}
