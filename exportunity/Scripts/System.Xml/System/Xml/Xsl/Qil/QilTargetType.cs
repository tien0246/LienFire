namespace System.Xml.Xsl.Qil;

internal class QilTargetType : QilBinary
{
	public QilNode Source
	{
		get
		{
			return base.Left;
		}
		set
		{
			base.Left = value;
		}
	}

	public XmlQueryType TargetType
	{
		get
		{
			return (XmlQueryType)((QilLiteral)base.Right).Value;
		}
		set
		{
			((QilLiteral)base.Right).Value = value;
		}
	}

	public QilTargetType(QilNodeType nodeType, QilNode expr, QilNode targetType)
		: base(nodeType, expr, targetType)
	{
	}
}
