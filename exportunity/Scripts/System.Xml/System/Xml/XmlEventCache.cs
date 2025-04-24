using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml.Xsl.Runtime;

namespace System.Xml;

internal sealed class XmlEventCache : XmlRawWriter
{
	private enum XmlEventType
	{
		Unknown = 0,
		DocType = 1,
		StartElem = 2,
		StartAttr = 3,
		EndAttr = 4,
		CData = 5,
		Comment = 6,
		PI = 7,
		Whitespace = 8,
		String = 9,
		Raw = 10,
		EntRef = 11,
		CharEnt = 12,
		SurrCharEnt = 13,
		Base64 = 14,
		BinHex = 15,
		XmlDecl1 = 16,
		XmlDecl2 = 17,
		StartContent = 18,
		EndElem = 19,
		FullEndElem = 20,
		Nmsp = 21,
		EndBase64 = 22,
		Close = 23,
		Flush = 24,
		Dispose = 25
	}

	private struct XmlEvent
	{
		private XmlEventType eventType;

		private string s1;

		private string s2;

		private string s3;

		private object o;

		public XmlEventType EventType => eventType;

		public string String1 => s1;

		public string String2 => s2;

		public string String3 => s3;

		public object Object => o;

		public void InitEvent(XmlEventType eventType)
		{
			this.eventType = eventType;
		}

		public void InitEvent(XmlEventType eventType, string s1)
		{
			this.eventType = eventType;
			this.s1 = s1;
		}

		public void InitEvent(XmlEventType eventType, string s1, string s2)
		{
			this.eventType = eventType;
			this.s1 = s1;
			this.s2 = s2;
		}

		public void InitEvent(XmlEventType eventType, string s1, string s2, string s3)
		{
			this.eventType = eventType;
			this.s1 = s1;
			this.s2 = s2;
			this.s3 = s3;
		}

		public void InitEvent(XmlEventType eventType, string s1, string s2, string s3, object o)
		{
			this.eventType = eventType;
			this.s1 = s1;
			this.s2 = s2;
			this.s3 = s3;
			this.o = o;
		}

		public void InitEvent(XmlEventType eventType, object o)
		{
			this.eventType = eventType;
			this.o = o;
		}
	}

	private List<XmlEvent[]> pages;

	private XmlEvent[] pageCurr;

	private int pageSize;

	private bool hasRootNode;

	private StringConcat singleText;

	private string baseUri;

	private const int InitialPageSize = 32;

	public string BaseUri => baseUri;

	public bool HasRootNode => hasRootNode;

	public override XmlWriterSettings Settings => null;

	public XmlEventCache(string baseUri, bool hasRootNode)
	{
		this.baseUri = baseUri;
		this.hasRootNode = hasRootNode;
	}

	public void EndEvents()
	{
		if (singleText.Count == 0)
		{
			AddEvent(XmlEventType.Unknown);
		}
	}

	public void EventsToWriter(XmlWriter writer)
	{
		if (singleText.Count != 0)
		{
			writer.WriteString(singleText.GetResult());
			return;
		}
		XmlRawWriter xmlRawWriter = writer as XmlRawWriter;
		for (int i = 0; i < pages.Count; i++)
		{
			XmlEvent[] array = pages[i];
			for (int j = 0; j < array.Length; j++)
			{
				switch (array[j].EventType)
				{
				case XmlEventType.Unknown:
					return;
				case XmlEventType.DocType:
					writer.WriteDocType(array[j].String1, array[j].String2, array[j].String3, (string)array[j].Object);
					break;
				case XmlEventType.StartElem:
					writer.WriteStartElement(array[j].String1, array[j].String2, array[j].String3);
					break;
				case XmlEventType.StartAttr:
					writer.WriteStartAttribute(array[j].String1, array[j].String2, array[j].String3);
					break;
				case XmlEventType.EndAttr:
					writer.WriteEndAttribute();
					break;
				case XmlEventType.CData:
					writer.WriteCData(array[j].String1);
					break;
				case XmlEventType.Comment:
					writer.WriteComment(array[j].String1);
					break;
				case XmlEventType.PI:
					writer.WriteProcessingInstruction(array[j].String1, array[j].String2);
					break;
				case XmlEventType.Whitespace:
					writer.WriteWhitespace(array[j].String1);
					break;
				case XmlEventType.String:
					writer.WriteString(array[j].String1);
					break;
				case XmlEventType.Raw:
					writer.WriteRaw(array[j].String1);
					break;
				case XmlEventType.EntRef:
					writer.WriteEntityRef(array[j].String1);
					break;
				case XmlEventType.CharEnt:
					writer.WriteCharEntity((char)array[j].Object);
					break;
				case XmlEventType.SurrCharEnt:
				{
					char[] array3 = (char[])array[j].Object;
					writer.WriteSurrogateCharEntity(array3[0], array3[1]);
					break;
				}
				case XmlEventType.Base64:
				{
					byte[] array2 = (byte[])array[j].Object;
					writer.WriteBase64(array2, 0, array2.Length);
					break;
				}
				case XmlEventType.BinHex:
				{
					byte[] array2 = (byte[])array[j].Object;
					writer.WriteBinHex(array2, 0, array2.Length);
					break;
				}
				case XmlEventType.XmlDecl1:
					xmlRawWriter?.WriteXmlDeclaration((XmlStandalone)array[j].Object);
					break;
				case XmlEventType.XmlDecl2:
					xmlRawWriter?.WriteXmlDeclaration(array[j].String1);
					break;
				case XmlEventType.StartContent:
					xmlRawWriter?.StartElementContent();
					break;
				case XmlEventType.EndElem:
					if (xmlRawWriter != null)
					{
						xmlRawWriter.WriteEndElement(array[j].String1, array[j].String2, array[j].String3);
					}
					else
					{
						writer.WriteEndElement();
					}
					break;
				case XmlEventType.FullEndElem:
					if (xmlRawWriter != null)
					{
						xmlRawWriter.WriteFullEndElement(array[j].String1, array[j].String2, array[j].String3);
					}
					else
					{
						writer.WriteFullEndElement();
					}
					break;
				case XmlEventType.Nmsp:
					if (xmlRawWriter != null)
					{
						xmlRawWriter.WriteNamespaceDeclaration(array[j].String1, array[j].String2);
					}
					else
					{
						writer.WriteAttributeString("xmlns", array[j].String1, "http://www.w3.org/2000/xmlns/", array[j].String2);
					}
					break;
				case XmlEventType.EndBase64:
					xmlRawWriter?.WriteEndBase64();
					break;
				case XmlEventType.Close:
					writer.Close();
					break;
				case XmlEventType.Flush:
					writer.Flush();
					break;
				case XmlEventType.Dispose:
					((IDisposable)writer).Dispose();
					break;
				}
			}
		}
	}

	public string EventsToString()
	{
		if (singleText.Count != 0)
		{
			return singleText.GetResult();
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		for (int i = 0; i < pages.Count; i++)
		{
			XmlEvent[] array = pages[i];
			for (int j = 0; j < array.Length; j++)
			{
				switch (array[j].EventType)
				{
				case XmlEventType.Unknown:
					return stringBuilder.ToString();
				case XmlEventType.CData:
				case XmlEventType.Whitespace:
				case XmlEventType.String:
				case XmlEventType.Raw:
					if (!flag)
					{
						stringBuilder.Append(array[j].String1);
					}
					break;
				case XmlEventType.StartAttr:
					flag = true;
					break;
				case XmlEventType.EndAttr:
					flag = false;
					break;
				}
			}
		}
		return string.Empty;
	}

	public override void WriteDocType(string name, string pubid, string sysid, string subset)
	{
		AddEvent(XmlEventType.DocType, name, pubid, sysid, subset);
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
		AddEvent(XmlEventType.StartElem, prefix, localName, ns);
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
		AddEvent(XmlEventType.StartAttr, prefix, localName, ns);
	}

	public override void WriteEndAttribute()
	{
		AddEvent(XmlEventType.EndAttr);
	}

	public override void WriteCData(string text)
	{
		AddEvent(XmlEventType.CData, text);
	}

	public override void WriteComment(string text)
	{
		AddEvent(XmlEventType.Comment, text);
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
		AddEvent(XmlEventType.PI, name, text);
	}

	public override void WriteWhitespace(string ws)
	{
		AddEvent(XmlEventType.Whitespace, ws);
	}

	public override void WriteString(string text)
	{
		if (pages == null)
		{
			singleText.ConcatNoDelimiter(text);
		}
		else
		{
			AddEvent(XmlEventType.String, text);
		}
	}

	public override void WriteChars(char[] buffer, int index, int count)
	{
		WriteString(new string(buffer, index, count));
	}

	public override void WriteRaw(char[] buffer, int index, int count)
	{
		WriteRaw(new string(buffer, index, count));
	}

	public override void WriteRaw(string data)
	{
		AddEvent(XmlEventType.Raw, data);
	}

	public override void WriteEntityRef(string name)
	{
		AddEvent(XmlEventType.EntRef, name);
	}

	public override void WriteCharEntity(char ch)
	{
		AddEvent(XmlEventType.CharEnt, ch);
	}

	public override void WriteSurrogateCharEntity(char lowChar, char highChar)
	{
		char[] o = new char[2] { lowChar, highChar };
		AddEvent(XmlEventType.SurrCharEnt, o);
	}

	public override void WriteBase64(byte[] buffer, int index, int count)
	{
		AddEvent(XmlEventType.Base64, ToBytes(buffer, index, count));
	}

	public override void WriteBinHex(byte[] buffer, int index, int count)
	{
		AddEvent(XmlEventType.BinHex, ToBytes(buffer, index, count));
	}

	public override void Close()
	{
		AddEvent(XmlEventType.Close);
	}

	public override void Flush()
	{
		AddEvent(XmlEventType.Flush);
	}

	public override void WriteValue(object value)
	{
		WriteString(XmlUntypedConverter.Untyped.ToString(value, resolver));
	}

	public override void WriteValue(string value)
	{
		WriteString(value);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing)
			{
				AddEvent(XmlEventType.Dispose);
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	internal override void WriteXmlDeclaration(XmlStandalone standalone)
	{
		AddEvent(XmlEventType.XmlDecl1, standalone);
	}

	internal override void WriteXmlDeclaration(string xmldecl)
	{
		AddEvent(XmlEventType.XmlDecl2, xmldecl);
	}

	internal override void StartElementContent()
	{
		AddEvent(XmlEventType.StartContent);
	}

	internal override void WriteEndElement(string prefix, string localName, string ns)
	{
		AddEvent(XmlEventType.EndElem, prefix, localName, ns);
	}

	internal override void WriteFullEndElement(string prefix, string localName, string ns)
	{
		AddEvent(XmlEventType.FullEndElem, prefix, localName, ns);
	}

	internal override void WriteNamespaceDeclaration(string prefix, string ns)
	{
		AddEvent(XmlEventType.Nmsp, prefix, ns);
	}

	internal override void WriteEndBase64()
	{
		AddEvent(XmlEventType.EndBase64);
	}

	private void AddEvent(XmlEventType eventType)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType);
	}

	private void AddEvent(XmlEventType eventType, string s1)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType, s1);
	}

	private void AddEvent(XmlEventType eventType, string s1, string s2)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType, s1, s2);
	}

	private void AddEvent(XmlEventType eventType, string s1, string s2, string s3)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType, s1, s2, s3);
	}

	private void AddEvent(XmlEventType eventType, string s1, string s2, string s3, object o)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType, s1, s2, s3, o);
	}

	private void AddEvent(XmlEventType eventType, object o)
	{
		int num = NewEvent();
		pageCurr[num].InitEvent(eventType, o);
	}

	private int NewEvent()
	{
		if (pages == null)
		{
			pages = new List<XmlEvent[]>();
			pageCurr = new XmlEvent[32];
			pages.Add(pageCurr);
			if (singleText.Count != 0)
			{
				pageCurr[0].InitEvent(XmlEventType.String, singleText.GetResult());
				pageSize++;
				singleText.Clear();
			}
		}
		else if (pageSize >= pageCurr.Length)
		{
			pageCurr = new XmlEvent[pageSize * 2];
			pages.Add(pageCurr);
			pageSize = 0;
		}
		return pageSize++;
	}

	private static byte[] ToBytes(byte[] buffer, int index, int count)
	{
		if (index != 0 || count != buffer.Length)
		{
			if (buffer.Length - index > count)
			{
				count = buffer.Length - index;
			}
			byte[] array = new byte[count];
			Array.Copy(buffer, index, array, 0, count);
			return array;
		}
		return buffer;
	}
}
