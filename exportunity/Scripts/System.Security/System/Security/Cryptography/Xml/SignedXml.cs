using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class SignedXml
{
	private class ReferenceLevelSortOrder : IComparer
	{
		private ArrayList _references;

		public ArrayList References
		{
			get
			{
				return _references;
			}
			set
			{
				_references = value;
			}
		}

		public int Compare(object a, object b)
		{
			Reference reference = a as Reference;
			Reference reference2 = b as Reference;
			int index = 0;
			int index2 = 0;
			int num = 0;
			foreach (Reference reference3 in References)
			{
				if (reference3 == reference)
				{
					index = num;
				}
				if (reference3 == reference2)
				{
					index2 = num;
				}
				num++;
			}
			int referenceLevel = reference.SignedXml.GetReferenceLevel(index, References);
			int referenceLevel2 = reference2.SignedXml.GetReferenceLevel(index2, References);
			return referenceLevel.CompareTo(referenceLevel2);
		}
	}

	protected Signature m_signature;

	protected string m_strSigningKeyName;

	private AsymmetricAlgorithm _signingKey;

	private XmlDocument _containingDocument;

	private IEnumerator _keyInfoEnum;

	private X509Certificate2Collection _x509Collection;

	private IEnumerator _x509Enum;

	private bool[] _refProcessed;

	private int[] _refLevelCache;

	internal XmlResolver _xmlResolver;

	internal XmlElement _context;

	private bool _bResolverSet;

	private Func<SignedXml, bool> _signatureFormatValidator = DefaultSignatureFormatValidator;

	private Collection<string> _safeCanonicalizationMethods;

	private static IList<string> s_knownCanonicalizationMethods;

	private static IList<string> s_defaultSafeTransformMethods;

	private const string XmlDsigMoreHMACMD5Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";

	private const string XmlDsigMoreHMACSHA256Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";

	private const string XmlDsigMoreHMACSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";

	private const string XmlDsigMoreHMACSHA512Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

	private const string XmlDsigMoreHMACRIPEMD160Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";

	private EncryptedXml _exml;

	public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

	public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";

	public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

	public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

	public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";

	public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

	public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

	public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

	public const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

	public const string XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

	public const string XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";

	public const string XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

	public const string XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

	public const string XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

	public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

	public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

	public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";

	public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

	public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";

	public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";

	public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";

	public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

	public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";

	public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

	private bool _bCacheValid;

	private byte[] _digestedSignedInfo;

	public string SigningKeyName
	{
		get
		{
			return m_strSigningKeyName;
		}
		set
		{
			m_strSigningKeyName = value;
		}
	}

	public XmlResolver Resolver
	{
		set
		{
			_xmlResolver = value;
			_bResolverSet = true;
		}
	}

	internal bool ResolverSet => _bResolverSet;

	public Func<SignedXml, bool> SignatureFormatValidator
	{
		get
		{
			return _signatureFormatValidator;
		}
		set
		{
			_signatureFormatValidator = value;
		}
	}

	public Collection<string> SafeCanonicalizationMethods => _safeCanonicalizationMethods;

	public AsymmetricAlgorithm SigningKey
	{
		get
		{
			return _signingKey;
		}
		set
		{
			_signingKey = value;
		}
	}

	public EncryptedXml EncryptedXml
	{
		get
		{
			if (_exml == null)
			{
				_exml = new EncryptedXml(_containingDocument);
			}
			return _exml;
		}
		set
		{
			_exml = value;
		}
	}

	public Signature Signature => m_signature;

	public SignedInfo SignedInfo => m_signature.SignedInfo;

	public string SignatureMethod => m_signature.SignedInfo.SignatureMethod;

	public string SignatureLength => m_signature.SignedInfo.SignatureLength;

	public byte[] SignatureValue => m_signature.SignatureValue;

	public KeyInfo KeyInfo
	{
		get
		{
			return m_signature.KeyInfo;
		}
		set
		{
			m_signature.KeyInfo = value;
		}
	}

	private static IList<string> KnownCanonicalizationMethods
	{
		get
		{
			if (s_knownCanonicalizationMethods == null)
			{
				s_knownCanonicalizationMethods = new List<string> { "http://www.w3.org/TR/2001/REC-xml-c14n-20010315", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments", "http://www.w3.org/2001/10/xml-exc-c14n#", "http://www.w3.org/2001/10/xml-exc-c14n#WithComments" };
			}
			return s_knownCanonicalizationMethods;
		}
	}

	private static IList<string> DefaultSafeTransformMethods
	{
		get
		{
			if (s_defaultSafeTransformMethods == null)
			{
				s_defaultSafeTransformMethods = new List<string> { "http://www.w3.org/2000/09/xmldsig#enveloped-signature", "http://www.w3.org/2000/09/xmldsig#base64", "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform", "http://www.w3.org/2002/07/decrypt#XML" };
			}
			return s_defaultSafeTransformMethods;
		}
	}

	public SignedXml()
	{
		Initialize(null);
	}

	public SignedXml(XmlDocument document)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		Initialize(document.DocumentElement);
	}

	public SignedXml(XmlElement elem)
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		Initialize(elem);
	}

	private void Initialize(XmlElement element)
	{
		_containingDocument = element?.OwnerDocument;
		_context = element;
		m_signature = new Signature();
		m_signature.SignedXml = this;
		m_signature.SignedInfo = new SignedInfo();
		_signingKey = null;
		_safeCanonicalizationMethods = new Collection<string>(KnownCanonicalizationMethods);
	}

	public XmlElement GetXml()
	{
		if (_containingDocument != null)
		{
			return m_signature.GetXml(_containingDocument);
		}
		return m_signature.GetXml();
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		m_signature.LoadXml(value);
		if (_context == null)
		{
			_context = value;
		}
		_bCacheValid = false;
	}

	public void AddReference(Reference reference)
	{
		m_signature.SignedInfo.AddReference(reference);
	}

	public void AddObject(DataObject dataObject)
	{
		m_signature.AddObject(dataObject);
	}

	public bool CheckSignature()
	{
		AsymmetricAlgorithm signingKey;
		return CheckSignatureReturningKey(out signingKey);
	}

	public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
	{
		SignedXmlDebugLog.LogBeginSignatureVerification(this, _context);
		signingKey = null;
		bool flag = false;
		AsymmetricAlgorithm asymmetricAlgorithm = null;
		if (!CheckSignatureFormat())
		{
			return false;
		}
		do
		{
			asymmetricAlgorithm = GetPublicKey();
			if (asymmetricAlgorithm != null)
			{
				flag = CheckSignature(asymmetricAlgorithm);
				SignedXmlDebugLog.LogVerificationResult(this, asymmetricAlgorithm, flag);
			}
		}
		while (asymmetricAlgorithm != null && !flag);
		signingKey = asymmetricAlgorithm;
		return flag;
	}

	public bool CheckSignature(AsymmetricAlgorithm key)
	{
		if (!CheckSignatureFormat())
		{
			return false;
		}
		if (!CheckSignedInfo(key))
		{
			SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
			return false;
		}
		if (!CheckDigestedReferences())
		{
			SignedXmlDebugLog.LogVerificationFailure(this, "references");
			return false;
		}
		SignedXmlDebugLog.LogVerificationResult(this, key, verified: true);
		return true;
	}

	public bool CheckSignature(KeyedHashAlgorithm macAlg)
	{
		if (!CheckSignatureFormat())
		{
			return false;
		}
		if (!CheckSignedInfo(macAlg))
		{
			SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
			return false;
		}
		if (!CheckDigestedReferences())
		{
			SignedXmlDebugLog.LogVerificationFailure(this, "references");
			return false;
		}
		SignedXmlDebugLog.LogVerificationResult(this, macAlg, verified: true);
		return true;
	}

	public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
	{
		if (!verifySignatureOnly)
		{
			X509ExtensionEnumerator enumerator = certificate.Extensions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Extension current = enumerator.Current;
				if (string.Compare(current.Oid.Value, "2.5.29.15", StringComparison.OrdinalIgnoreCase) == 0)
				{
					X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension();
					x509KeyUsageExtension.CopyFrom(current);
					SignedXmlDebugLog.LogVerifyKeyUsage(this, certificate, x509KeyUsageExtension);
					if ((x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.DigitalSignature) != X509KeyUsageFlags.None || (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.NonRepudiation) != X509KeyUsageFlags.None)
					{
						break;
					}
					SignedXmlDebugLog.LogVerificationFailure(this, "X509 key usage verification");
					return false;
				}
			}
			X509Chain x509Chain = new X509Chain();
			x509Chain.ChainPolicy.ExtraStore.AddRange(BuildBagOfCerts());
			bool num = x509Chain.Build(certificate);
			SignedXmlDebugLog.LogVerifyX509Chain(this, x509Chain, certificate);
			if (!num)
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "X509 chain verification");
				return false;
			}
		}
		using (AsymmetricAlgorithm key = Utils.GetAnyPublicKey(certificate))
		{
			if (!CheckSignature(key))
			{
				return false;
			}
		}
		SignedXmlDebugLog.LogVerificationResult(this, certificate, verified: true);
		return true;
	}

	public void ComputeSignature()
	{
		SignedXmlDebugLog.LogBeginSignatureComputation(this, _context);
		BuildDigestedReferences();
		AsymmetricAlgorithm signingKey = SigningKey;
		if (signingKey == null)
		{
			throw new CryptographicException("Signing key is not loaded.");
		}
		if (SignedInfo.SignatureMethod == null)
		{
			if (signingKey is DSA)
			{
				SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
			}
			else
			{
				if (!(signingKey is RSA))
				{
					throw new CryptographicException("Failed to create signing key.");
				}
				if (SignedInfo.SignatureMethod == null)
				{
					SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
				}
			}
		}
		SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(SignedInfo.SignatureMethod);
		if (signatureDescription == null)
		{
			throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
		}
		HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
		if (hashAlgorithm == null)
		{
			throw new CryptographicException("Could not create hash algorithm object.");
		}
		GetC14NDigest(hashAlgorithm);
		AsymmetricSignatureFormatter asymmetricSignatureFormatter = signatureDescription.CreateFormatter(signingKey);
		SignedXmlDebugLog.LogSigning(this, signingKey, signatureDescription, hashAlgorithm, asymmetricSignatureFormatter);
		m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(hashAlgorithm);
	}

	public void ComputeSignature(KeyedHashAlgorithm macAlg)
	{
		if (macAlg == null)
		{
			throw new ArgumentNullException("macAlg");
		}
		if (!(macAlg is HMAC hMAC))
		{
			throw new CryptographicException("The key does not fit the SignatureMethod.");
		}
		int num = ((m_signature.SignedInfo.SignatureLength != null) ? Convert.ToInt32(m_signature.SignedInfo.SignatureLength, null) : hMAC.HashSize);
		if (num < 0 || num > hMAC.HashSize)
		{
			throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
		}
		if (num % 8 != 0)
		{
			throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
		}
		BuildDigestedReferences();
		switch (hMAC.HashName)
		{
		case "SHA1":
			SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
			break;
		case "SHA256":
			SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
			break;
		case "SHA384":
			SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
			break;
		case "SHA512":
			SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
			break;
		case "MD5":
			SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
			break;
		case "RIPEMD160":
			SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
			break;
		default:
			throw new CryptographicException("The key does not fit the SignatureMethod.");
		}
		byte[] c14NDigest = GetC14NDigest(hMAC);
		SignedXmlDebugLog.LogSigning(this, hMAC);
		m_signature.SignatureValue = new byte[num / 8];
		Buffer.BlockCopy(c14NDigest, 0, m_signature.SignatureValue, 0, num / 8);
	}

	protected virtual AsymmetricAlgorithm GetPublicKey()
	{
		if (KeyInfo == null)
		{
			throw new CryptographicException("A KeyInfo element is required to check the signature.");
		}
		if (_x509Enum != null)
		{
			AsymmetricAlgorithm nextCertificatePublicKey = GetNextCertificatePublicKey();
			if (nextCertificatePublicKey != null)
			{
				return nextCertificatePublicKey;
			}
		}
		if (_keyInfoEnum == null)
		{
			_keyInfoEnum = KeyInfo.GetEnumerator();
		}
		while (_keyInfoEnum.MoveNext())
		{
			if (_keyInfoEnum.Current is RSAKeyValue rSAKeyValue)
			{
				return rSAKeyValue.Key;
			}
			if (_keyInfoEnum.Current is DSAKeyValue dSAKeyValue)
			{
				return dSAKeyValue.Key;
			}
			if (!(_keyInfoEnum.Current is KeyInfoX509Data keyInfoX509Data))
			{
				continue;
			}
			_x509Collection = Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification);
			if (_x509Collection.Count > 0)
			{
				_x509Enum = _x509Collection.GetEnumerator();
				AsymmetricAlgorithm nextCertificatePublicKey2 = GetNextCertificatePublicKey();
				if (nextCertificatePublicKey2 != null)
				{
					return nextCertificatePublicKey2;
				}
			}
		}
		return null;
	}

	private X509Certificate2Collection BuildBagOfCerts()
	{
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		if (KeyInfo != null)
		{
			foreach (KeyInfoClause item in KeyInfo)
			{
				if (item is KeyInfoX509Data keyInfoX509Data)
				{
					x509Certificate2Collection.AddRange(Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification));
				}
			}
		}
		return x509Certificate2Collection;
	}

	private AsymmetricAlgorithm GetNextCertificatePublicKey()
	{
		while (_x509Enum.MoveNext())
		{
			X509Certificate2 x509Certificate = (X509Certificate2)_x509Enum.Current;
			if (x509Certificate != null)
			{
				return Utils.GetAnyPublicKey(x509Certificate);
			}
		}
		return null;
	}

	public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
	{
		return DefaultGetIdElement(document, idValue);
	}

	internal static XmlElement DefaultGetIdElement(XmlDocument document, string idValue)
	{
		if (document == null)
		{
			return null;
		}
		try
		{
			XmlConvert.VerifyNCName(idValue);
		}
		catch (XmlException)
		{
			return null;
		}
		XmlElement elementById = document.GetElementById(idValue);
		if (elementById != null)
		{
			XmlDocument xmlDocument = (XmlDocument)document.CloneNode(deep: true);
			XmlElement elementById2 = xmlDocument.GetElementById(idValue);
			if (elementById2 != null)
			{
				elementById2.Attributes.RemoveAll();
				if (xmlDocument.GetElementById(idValue) != null)
				{
					throw new CryptographicException("Malformed reference element.");
				}
			}
			return elementById;
		}
		elementById = GetSingleReferenceTarget(document, "Id", idValue);
		if (elementById != null)
		{
			return elementById;
		}
		elementById = GetSingleReferenceTarget(document, "id", idValue);
		if (elementById != null)
		{
			return elementById;
		}
		return GetSingleReferenceTarget(document, "ID", idValue);
	}

	private static bool DefaultSignatureFormatValidator(SignedXml signedXml)
	{
		if (signedXml.DoesSignatureUseTruncatedHmac())
		{
			return false;
		}
		if (!signedXml.DoesSignatureUseSafeCanonicalizationMethod())
		{
			return false;
		}
		return true;
	}

	private bool DoesSignatureUseTruncatedHmac()
	{
		if (SignedInfo.SignatureLength == null)
		{
			return false;
		}
		HMAC hMAC = CryptoHelpers.CreateFromName<HMAC>(SignatureMethod);
		if (hMAC == null)
		{
			return false;
		}
		int result = 0;
		if (!int.TryParse(SignedInfo.SignatureLength, out result))
		{
			return true;
		}
		return result != hMAC.HashSize;
	}

	private bool DoesSignatureUseSafeCanonicalizationMethod()
	{
		foreach (string safeCanonicalizationMethod in SafeCanonicalizationMethods)
		{
			if (string.Equals(safeCanonicalizationMethod, SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		SignedXmlDebugLog.LogUnsafeCanonicalizationMethod(this, SignedInfo.CanonicalizationMethod, SafeCanonicalizationMethods);
		return false;
	}

	private bool ReferenceUsesSafeTransformMethods(Reference reference)
	{
		TransformChain transformChain = reference.TransformChain;
		int count = transformChain.Count;
		for (int i = 0; i < count; i++)
		{
			Transform transform = transformChain[i];
			if (!IsSafeTransform(transform.Algorithm))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsSafeTransform(string transformAlgorithm)
	{
		foreach (string safeCanonicalizationMethod in SafeCanonicalizationMethods)
		{
			if (string.Equals(safeCanonicalizationMethod, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		foreach (string defaultSafeTransformMethod in DefaultSafeTransformMethods)
		{
			if (string.Equals(defaultSafeTransformMethod, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		SignedXmlDebugLog.LogUnsafeTransformMethod(this, transformAlgorithm, SafeCanonicalizationMethods, DefaultSafeTransformMethods);
		return false;
	}

	private byte[] GetC14NDigest(HashAlgorithm hash)
	{
		bool flag = hash is KeyedHashAlgorithm;
		if (flag || !_bCacheValid || !SignedInfo.CacheValid)
		{
			string text = ((_containingDocument == null) ? null : _containingDocument.BaseURI);
			XmlResolver xmlResolver = (_bResolverSet ? _xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
			XmlDocument xmlDocument = Utils.PreProcessElementInput(SignedInfo.GetXml(), xmlResolver, text);
			CanonicalXmlNodeList namespaces = ((_context == null) ? null : Utils.GetPropagatedAttributes(_context));
			SignedXmlDebugLog.LogNamespacePropagation(this, namespaces);
			Utils.AddNamespaces(xmlDocument.DocumentElement, namespaces);
			Transform canonicalizationMethodObject = SignedInfo.CanonicalizationMethodObject;
			canonicalizationMethodObject.Resolver = xmlResolver;
			canonicalizationMethodObject.BaseURI = text;
			SignedXmlDebugLog.LogBeginCanonicalization(this, canonicalizationMethodObject);
			canonicalizationMethodObject.LoadInput(xmlDocument);
			SignedXmlDebugLog.LogCanonicalizedOutput(this, canonicalizationMethodObject);
			_digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
			_bCacheValid = !flag;
		}
		return _digestedSignedInfo;
	}

	private int GetReferenceLevel(int index, ArrayList references)
	{
		if (_refProcessed[index])
		{
			return _refLevelCache[index];
		}
		_refProcessed[index] = true;
		Reference reference = (Reference)references[index];
		if (reference.Uri == null || reference.Uri.Length == 0 || (reference.Uri.Length > 0 && reference.Uri[0] != '#'))
		{
			_refLevelCache[index] = 0;
			return 0;
		}
		if (reference.Uri.Length > 0 && reference.Uri[0] == '#')
		{
			string text = Utils.ExtractIdFromLocalUri(reference.Uri);
			if (text == "xpointer(/)")
			{
				_refLevelCache[index] = 0;
				return 0;
			}
			for (int i = 0; i < references.Count; i++)
			{
				if (((Reference)references[i]).Id == text)
				{
					_refLevelCache[index] = GetReferenceLevel(i, references) + 1;
					return _refLevelCache[index];
				}
			}
			_refLevelCache[index] = 0;
			return 0;
		}
		throw new CryptographicException("Malformed reference element.");
	}

	private void BuildDigestedReferences()
	{
		ArrayList references = SignedInfo.References;
		_refProcessed = new bool[references.Count];
		_refLevelCache = new int[references.Count];
		ReferenceLevelSortOrder referenceLevelSortOrder = new ReferenceLevelSortOrder();
		referenceLevelSortOrder.References = references;
		ArrayList arrayList = new ArrayList();
		foreach (Reference item in references)
		{
			arrayList.Add(item);
		}
		arrayList.Sort(referenceLevelSortOrder);
		CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
		foreach (DataObject @object in m_signature.ObjectList)
		{
			canonicalXmlNodeList.Add(@object.GetXml());
		}
		foreach (Reference item2 in arrayList)
		{
			if (item2.DigestMethod == null)
			{
				item2.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
			}
			SignedXmlDebugLog.LogSigningReference(this, item2);
			item2.UpdateHashValue(_containingDocument, canonicalXmlNodeList);
			if (item2.Id != null)
			{
				canonicalXmlNodeList.Add(item2.GetXml());
			}
		}
	}

	private bool CheckDigestedReferences()
	{
		ArrayList references = m_signature.SignedInfo.References;
		for (int i = 0; i < references.Count; i++)
		{
			Reference reference = (Reference)references[i];
			if (!ReferenceUsesSafeTransformMethods(reference))
			{
				return false;
			}
			SignedXmlDebugLog.LogVerifyReference(this, reference);
			byte[] array = null;
			try
			{
				array = reference.CalculateHashValue(_containingDocument, m_signature.ReferencedItems);
			}
			catch (CryptoSignedXmlRecursionException)
			{
				SignedXmlDebugLog.LogSignedXmlRecursionLimit(this, reference);
				return false;
			}
			SignedXmlDebugLog.LogVerifyReferenceHash(this, reference, array, reference.DigestValue);
			if (!CryptographicEquals(array, reference.DigestValue))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private static bool CryptographicEquals(byte[] a, byte[] b)
	{
		int num = 0;
		if (a.Length != b.Length)
		{
			return false;
		}
		int num2 = a.Length;
		for (int i = 0; i < num2; i++)
		{
			num |= a[i] - b[i];
		}
		return num == 0;
	}

	private bool CheckSignatureFormat()
	{
		if (_signatureFormatValidator == null)
		{
			return true;
		}
		SignedXmlDebugLog.LogBeginCheckSignatureFormat(this, _signatureFormatValidator);
		bool result = _signatureFormatValidator(this);
		SignedXmlDebugLog.LogFormatValidationResult(this, result);
		return result;
	}

	private bool CheckSignedInfo(AsymmetricAlgorithm key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		SignedXmlDebugLog.LogBeginCheckSignedInfo(this, m_signature.SignedInfo);
		SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(SignatureMethod);
		if (signatureDescription == null)
		{
			throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
		}
		Type type = Type.GetType(signatureDescription.KeyAlgorithm);
		if (!IsKeyTheCorrectAlgorithm(key, type))
		{
			return false;
		}
		HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
		if (hashAlgorithm == null)
		{
			throw new CryptographicException("Could not create hash algorithm object.");
		}
		byte[] c14NDigest = GetC14NDigest(hashAlgorithm);
		AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(key);
		SignedXmlDebugLog.LogVerifySignedInfo(this, key, signatureDescription, hashAlgorithm, asymmetricSignatureDeformatter, c14NDigest, m_signature.SignatureValue);
		return asymmetricSignatureDeformatter.VerifySignature(c14NDigest, m_signature.SignatureValue);
	}

	private bool CheckSignedInfo(KeyedHashAlgorithm macAlg)
	{
		if (macAlg == null)
		{
			throw new ArgumentNullException("macAlg");
		}
		SignedXmlDebugLog.LogBeginCheckSignedInfo(this, m_signature.SignedInfo);
		int num = ((m_signature.SignedInfo.SignatureLength != null) ? Convert.ToInt32(m_signature.SignedInfo.SignatureLength, null) : macAlg.HashSize);
		if (num < 0 || num > macAlg.HashSize)
		{
			throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
		}
		if (num % 8 != 0)
		{
			throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
		}
		if (m_signature.SignatureValue == null)
		{
			throw new CryptographicException("Signature requires a SignatureValue.");
		}
		if (m_signature.SignatureValue.Length != num / 8)
		{
			throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
		}
		byte[] c14NDigest = GetC14NDigest(macAlg);
		SignedXmlDebugLog.LogVerifySignedInfo(this, macAlg, c14NDigest, m_signature.SignatureValue);
		for (int i = 0; i < m_signature.SignatureValue.Length; i++)
		{
			if (m_signature.SignatureValue[i] != c14NDigest[i])
			{
				return false;
			}
		}
		return true;
	}

	private static XmlElement GetSingleReferenceTarget(XmlDocument document, string idAttributeName, string idValue)
	{
		string xpath = "//*[@" + idAttributeName + "=\"" + idValue + "\"]";
		XmlNodeList xmlNodeList = document.SelectNodes(xpath);
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return null;
		}
		if (xmlNodeList.Count == 1)
		{
			return xmlNodeList[0] as XmlElement;
		}
		throw new CryptographicException("Malformed reference element.");
	}

	private static bool IsKeyTheCorrectAlgorithm(AsymmetricAlgorithm key, Type expectedType)
	{
		Type type = key.GetType();
		if (type == expectedType)
		{
			return true;
		}
		if (expectedType.IsSubclassOf(type))
		{
			return true;
		}
		while (expectedType != null && expectedType.BaseType != typeof(AsymmetricAlgorithm))
		{
			expectedType = expectedType.BaseType;
		}
		if (expectedType == null)
		{
			return false;
		}
		if (type.IsSubclassOf(expectedType))
		{
			return true;
		}
		return false;
	}
}
