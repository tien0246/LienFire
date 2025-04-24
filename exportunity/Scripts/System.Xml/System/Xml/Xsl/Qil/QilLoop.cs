namespace System.Xml.Xsl.Qil;

internal class QilLoop : QilBinary
{
	public QilIterator Variable
	{
		get
		{
			return (QilIterator)base.Left;
		}
		set
		{
			base.Left = value;
		}
	}

	public QilNode Body
	{
		get
		{
			return base.Right;
		}
		set
		{
			base.Right = value;
		}
	}

	public QilLoop(QilNodeType nodeType, QilNode variable, QilNode body)
		: base(nodeType, variable, body)
	{
	}
}
