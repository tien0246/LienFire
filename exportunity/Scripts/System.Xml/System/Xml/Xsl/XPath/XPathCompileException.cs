using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System.Xml.Xsl.XPath;

[Serializable]
internal class XPathCompileException : XslLoadException
{
	private enum TrimType
	{
		Left = 0,
		Right = 1,
		Middle = 2
	}

	public string queryString;

	public int startChar;

	public int endChar;

	protected XPathCompileException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		queryString = (string)info.GetValue("QueryString", typeof(string));
		startChar = (int)info.GetValue("StartChar", typeof(int));
		endChar = (int)info.GetValue("EndChar", typeof(int));
	}

	[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("QueryString", queryString);
		info.AddValue("StartChar", startChar);
		info.AddValue("EndChar", endChar);
	}

	internal XPathCompileException(string queryString, int startChar, int endChar, string resId, params string[] args)
		: base(resId, args)
	{
		this.queryString = queryString;
		this.startChar = startChar;
		this.endChar = endChar;
	}

	internal XPathCompileException(string resId, params string[] args)
		: base(resId, args)
	{
	}

	private static void AppendTrimmed(StringBuilder sb, string value, int startIndex, int count, TrimType trimType)
	{
		if (count <= 32)
		{
			sb.Append(value, startIndex, count);
			return;
		}
		switch (trimType)
		{
		case TrimType.Left:
			sb.Append("...");
			sb.Append(value, startIndex + count - 32, 32);
			break;
		case TrimType.Right:
			sb.Append(value, startIndex, 32);
			sb.Append("...");
			break;
		case TrimType.Middle:
			sb.Append(value, startIndex, 16);
			sb.Append("...");
			sb.Append(value, startIndex + count - 16, 16);
			break;
		}
	}

	internal string MarkOutError()
	{
		if (queryString == null || queryString.Trim(' ').Length == 0)
		{
			return null;
		}
		int num = endChar - startChar;
		StringBuilder stringBuilder = new StringBuilder();
		AppendTrimmed(stringBuilder, queryString, 0, startChar, TrimType.Left);
		if (num > 0)
		{
			stringBuilder.Append(" -->");
			AppendTrimmed(stringBuilder, queryString, startChar, num, TrimType.Middle);
		}
		stringBuilder.Append("<-- ");
		AppendTrimmed(stringBuilder, queryString, endChar, queryString.Length - endChar, TrimType.Right);
		return stringBuilder.ToString();
	}

	internal override string FormatDetailedMessage()
	{
		string text = Message;
		string text2 = MarkOutError();
		if (text2 != null && text2.Length > 0)
		{
			if (text.Length > 0)
			{
				text += Environment.NewLine;
			}
			text += text2;
		}
		return text;
	}
}
