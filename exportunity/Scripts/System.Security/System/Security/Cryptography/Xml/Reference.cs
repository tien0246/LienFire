using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class Reference
{
	internal const string DefaultDigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";

	private string _id;

	private string _uri;

	private string _type;

	private TransformChain _transformChain;

	private string _digestMethod;

	private byte[] _digestValue;

	private HashAlgorithm _hashAlgorithm;

	private object _refTarget;

	private ReferenceTargetType _refTargetType;

	private XmlElement _cachedXml;

	private SignedXml _signedXml;

	internal CanonicalXmlNodeList _namespaces;

	public string Id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public string Uri
	{
		get
		{
			return _uri;
		}
		set
		{
			_uri = value;
			_cachedXml = null;
		}
	}

	public string Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
			_cachedXml = null;
		}
	}

	public string DigestMethod
	{
		get
		{
			return _digestMethod;
		}
		set
		{
			_digestMethod = value;
			_cachedXml = null;
		}
	}

	public byte[] DigestValue
	{
		get
		{
			return _digestValue;
		}
		set
		{
			_digestValue = value;
			_cachedXml = null;
		}
	}

	public TransformChain TransformChain
	{
		get
		{
			if (_transformChain == null)
			{
				_transformChain = new TransformChain();
			}
			return _transformChain;
		}
		set
		{
			_transformChain = value;
			_cachedXml = null;
		}
	}

	internal bool CacheValid => _cachedXml != null;

	internal SignedXml SignedXml
	{
		get
		{
			return _signedXml;
		}
		set
		{
			_signedXml = value;
		}
	}

	internal ReferenceTargetType ReferenceTargetType => _refTargetType;

	public Reference()
	{
		_transformChain = new TransformChain();
		_refTarget = null;
		_refTargetType = ReferenceTargetType.UriReference;
		_cachedXml = null;
		_digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
	}

	public Reference(Stream stream)
	{
		_transformChain = new TransformChain();
		_refTarget = stream;
		_refTargetType = ReferenceTargetType.Stream;
		_cachedXml = null;
		_digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
	}

	public Reference(string uri)
	{
		_transformChain = new TransformChain();
		_refTarget = uri;
		_uri = uri;
		_refTargetType = ReferenceTargetType.UriReference;
		_cachedXml = null;
		_digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
	}

	internal Reference(XmlElement element)
	{
		_transformChain = new TransformChain();
		_refTarget = element;
		_refTargetType = ReferenceTargetType.XmlElement;
		_cachedXml = null;
		_digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
	}

	public XmlElement GetXml()
	{
		if (CacheValid)
		{
			return _cachedXml;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.PreserveWhitespace = true;
		return GetXml(xmlDocument);
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		XmlElement xmlElement = document.CreateElement("Reference", "http://www.w3.org/2000/09/xmldsig#");
		if (!string.IsNullOrEmpty(_id))
		{
			xmlElement.SetAttribute("Id", _id);
		}
		if (_uri != null)
		{
			xmlElement.SetAttribute("URI", _uri);
		}
		if (!string.IsNullOrEmpty(_type))
		{
			xmlElement.SetAttribute("Type", _type);
		}
		if (TransformChain.Count != 0)
		{
			xmlElement.AppendChild(TransformChain.GetXml(document, "http://www.w3.org/2000/09/xmldsig#"));
		}
		if (string.IsNullOrEmpty(_digestMethod))
		{
			throw new CryptographicException("A DigestMethod must be specified on a Reference prior to generating XML.");
		}
		XmlElement xmlElement2 = document.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement2.SetAttribute("Algorithm", _digestMethod);
		xmlElement.AppendChild(xmlElement2);
		if (DigestValue == null)
		{
			if (_hashAlgorithm.Hash == null)
			{
				throw new CryptographicException("A Reference must contain a DigestValue.");
			}
			DigestValue = _hashAlgorithm.Hash;
		}
		XmlElement xmlElement3 = document.CreateElement("DigestValue", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement3.AppendChild(document.CreateTextNode(Convert.ToBase64String(_digestValue)));
		xmlElement.AppendChild(xmlElement3);
		return xmlElement;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		_id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
		_uri = Utils.GetAttribute(value, "URI", "http://www.w3.org/2000/09/xmldsig#");
		_type = Utils.GetAttribute(value, "Type", "http://www.w3.org/2000/09/xmldsig#");
		if (!Utils.VerifyAttributes(value, new string[3] { "Id", "URI", "Type" }))
		{
			throw new CryptographicException("Malformed element {0}.", "Reference");
		}
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
		xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
		bool flag = false;
		TransformChain = new TransformChain();
		XmlNodeList xmlNodeList = value.SelectNodes("ds:Transforms", xmlNamespaceManager);
		if (xmlNodeList != null && xmlNodeList.Count != 0)
		{
			if (xmlNodeList.Count > 1)
			{
				throw new CryptographicException("Malformed element {0}.", "Reference/Transforms");
			}
			flag = true;
			XmlElement xmlElement = xmlNodeList[0] as XmlElement;
			if (!Utils.VerifyAttributes(xmlElement, (string[])null))
			{
				throw new CryptographicException("Malformed element {0}.", "Reference/Transforms");
			}
			XmlNodeList xmlNodeList2 = xmlElement.SelectNodes("ds:Transform", xmlNamespaceManager);
			if (xmlNodeList2 != null)
			{
				if (xmlNodeList2.Count != xmlElement.SelectNodes("*").Count)
				{
					throw new CryptographicException("Malformed element {0}.", "Reference/Transforms");
				}
				if (xmlNodeList2.Count > 10)
				{
					throw new CryptographicException("Malformed element {0}.", "Reference/Transforms");
				}
				foreach (XmlNode item in xmlNodeList2)
				{
					XmlElement xmlElement2 = item as XmlElement;
					string attribute = Utils.GetAttribute(xmlElement2, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
					if (attribute == null || !Utils.VerifyAttributes(xmlElement2, "Algorithm"))
					{
						throw new CryptographicException("Unknown transform has been encountered.");
					}
					Transform transform = CryptoHelpers.CreateFromName<Transform>(attribute);
					if (transform == null)
					{
						throw new CryptographicException("Unknown transform has been encountered.");
					}
					AddTransform(transform);
					transform.LoadInnerXml(xmlElement2.ChildNodes);
					if (!(transform is XmlDsigEnvelopedSignatureTransform))
					{
						continue;
					}
					XmlNode xmlNode = xmlElement2.SelectSingleNode("ancestor::ds:Signature[1]", xmlNamespaceManager);
					XmlNodeList xmlNodeList3 = xmlElement2.SelectNodes("//ds:Signature", xmlNamespaceManager);
					if (xmlNodeList3 == null)
					{
						continue;
					}
					int num = 0;
					foreach (XmlNode item2 in xmlNodeList3)
					{
						num++;
						if (item2 == xmlNode)
						{
							((XmlDsigEnvelopedSignatureTransform)transform).SignaturePosition = num;
							break;
						}
					}
				}
			}
		}
		XmlNodeList xmlNodeList4 = value.SelectNodes("ds:DigestMethod", xmlNamespaceManager);
		if (xmlNodeList4 == null || xmlNodeList4.Count == 0 || xmlNodeList4.Count > 1)
		{
			throw new CryptographicException("Malformed element {0}.", "Reference/DigestMethod");
		}
		XmlElement element = xmlNodeList4[0] as XmlElement;
		_digestMethod = Utils.GetAttribute(element, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
		if (_digestMethod == null || !Utils.VerifyAttributes(element, "Algorithm"))
		{
			throw new CryptographicException("Malformed element {0}.", "Reference/DigestMethod");
		}
		XmlNodeList xmlNodeList5 = value.SelectNodes("ds:DigestValue", xmlNamespaceManager);
		if (xmlNodeList5 == null || xmlNodeList5.Count == 0 || xmlNodeList5.Count > 1)
		{
			throw new CryptographicException("Malformed element {0}.", "Reference/DigestValue");
		}
		XmlElement xmlElement3 = xmlNodeList5[0] as XmlElement;
		_digestValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlElement3.InnerText));
		if (!Utils.VerifyAttributes(xmlElement3, (string[])null))
		{
			throw new CryptographicException("Malformed element {0}.", "Reference/DigestValue");
		}
		int num2 = (flag ? 3 : 2);
		if (value.SelectNodes("*").Count != num2)
		{
			throw new CryptographicException("Malformed element {0}.", "Reference");
		}
		_cachedXml = value;
	}

	public void AddTransform(Transform transform)
	{
		if (transform == null)
		{
			throw new ArgumentNullException("transform");
		}
		transform.Reference = this;
		TransformChain.Add(transform);
	}

	internal void UpdateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
	{
		DigestValue = CalculateHashValue(document, refList);
	}

	internal byte[] CalculateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
	{
		_hashAlgorithm = CryptoHelpers.CreateFromName<HashAlgorithm>(_digestMethod);
		if (_hashAlgorithm == null)
		{
			throw new CryptographicException("Could not create hash algorithm object.");
		}
		string text = ((document == null) ? (Environment.CurrentDirectory + "\\") : document.BaseURI);
		Stream stream = null;
		WebResponse webResponse = null;
		Stream stream2 = null;
		XmlResolver xmlResolver = null;
		byte[] array = null;
		try
		{
			switch (_refTargetType)
			{
			case ReferenceTargetType.Stream:
				xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
				stream = TransformChain.TransformToOctetStream((Stream)_refTarget, xmlResolver, text);
				break;
			case ReferenceTargetType.UriReference:
				if (_uri == null)
				{
					xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
					stream = TransformChain.TransformToOctetStream((Stream)null, xmlResolver, text);
					break;
				}
				if (_uri.Length == 0)
				{
					if (document == null)
					{
						throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, "An XmlDocument context is required to resolve the Reference Uri {0}.", _uri));
					}
					xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
					XmlDocument document2 = Utils.DiscardComments(Utils.PreProcessDocumentInput(document, xmlResolver, text));
					stream = TransformChain.TransformToOctetStream(document2, xmlResolver, text);
					break;
				}
				if (_uri[0] == '#')
				{
					bool discardComments = true;
					string idFromLocalUri = Utils.GetIdFromLocalUri(_uri, out discardComments);
					if (idFromLocalUri == "xpointer(/)")
					{
						if (document == null)
						{
							throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, "An XmlDocument context is required to resolve the Reference Uri {0}.", _uri));
						}
						xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
						stream = TransformChain.TransformToOctetStream(Utils.PreProcessDocumentInput(document, xmlResolver, text), xmlResolver, text);
						break;
					}
					XmlElement xmlElement = SignedXml.GetIdElement(document, idFromLocalUri);
					if (xmlElement != null)
					{
						_namespaces = Utils.GetPropagatedAttributes(xmlElement.ParentNode as XmlElement);
					}
					if (xmlElement == null && refList != null)
					{
						foreach (XmlNode @ref in refList)
						{
							if (@ref is XmlElement xmlElement2 && Utils.HasAttribute(xmlElement2, "Id", "http://www.w3.org/2000/09/xmldsig#") && Utils.GetAttribute(xmlElement2, "Id", "http://www.w3.org/2000/09/xmldsig#").Equals(idFromLocalUri))
							{
								xmlElement = xmlElement2;
								if (_signedXml._context != null)
								{
									_namespaces = Utils.GetPropagatedAttributes(_signedXml._context);
								}
								break;
							}
						}
					}
					if (xmlElement == null)
					{
						throw new CryptographicException("Malformed reference element.");
					}
					XmlDocument xmlDocument = Utils.PreProcessElementInput(xmlElement, xmlResolver, text);
					Utils.AddNamespaces(xmlDocument.DocumentElement, _namespaces);
					xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
					if (discardComments)
					{
						XmlDocument document3 = Utils.DiscardComments(xmlDocument);
						stream = TransformChain.TransformToOctetStream(document3, xmlResolver, text);
					}
					else
					{
						stream = TransformChain.TransformToOctetStream(xmlDocument, xmlResolver, text);
					}
					break;
				}
				throw new CryptographicException("Unable to resolve Uri {0}.", _uri);
			case ReferenceTargetType.XmlElement:
				xmlResolver = (SignedXml.ResolverSet ? SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
				stream = TransformChain.TransformToOctetStream(Utils.PreProcessElementInput((XmlElement)_refTarget, xmlResolver, text), xmlResolver, text);
				break;
			default:
				throw new CryptographicException("Unable to resolve Uri {0}.", _uri);
			}
			stream = SignedXmlDebugLog.LogReferenceData(this, stream);
			return _hashAlgorithm.ComputeHash(stream);
		}
		finally
		{
			stream?.Close();
			webResponse?.Close();
			stream2?.Close();
		}
	}
}
