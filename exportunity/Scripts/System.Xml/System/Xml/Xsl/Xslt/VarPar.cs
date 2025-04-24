using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt;

internal class VarPar : XslNode
{
	public XslFlags DefValueFlags;

	public QilNode Value;

	public VarPar(XslNodeType nt, QilName name, string select, XslVersion xslVer)
		: base(nt, name, select, xslVer)
	{
	}
}
