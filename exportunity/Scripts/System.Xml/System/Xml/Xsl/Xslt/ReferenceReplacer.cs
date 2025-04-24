using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt;

internal class ReferenceReplacer : QilReplaceVisitor
{
	private QilReference lookFor;

	private QilReference replaceBy;

	public ReferenceReplacer(QilFactory f)
		: base(f)
	{
	}

	public QilNode Replace(QilNode expr, QilReference lookFor, QilReference replaceBy)
	{
		QilDepthChecker.Check(expr);
		this.lookFor = lookFor;
		this.replaceBy = replaceBy;
		return VisitAssumeReference(expr);
	}

	protected override QilNode VisitReference(QilNode n)
	{
		if (n != lookFor)
		{
			return n;
		}
		return replaceBy;
	}
}
