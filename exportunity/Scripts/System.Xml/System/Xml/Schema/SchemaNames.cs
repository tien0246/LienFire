namespace System.Xml.Schema;

internal sealed class SchemaNames
{
	public enum Token
	{
		Empty = 0,
		SchemaName = 1,
		SchemaType = 2,
		SchemaMaxOccurs = 3,
		SchemaMinOccurs = 4,
		SchemaInfinite = 5,
		SchemaModel = 6,
		SchemaOpen = 7,
		SchemaClosed = 8,
		SchemaContent = 9,
		SchemaMixed = 10,
		SchemaEmpty = 11,
		SchemaElementOnly = 12,
		SchemaTextOnly = 13,
		SchemaOrder = 14,
		SchemaSeq = 15,
		SchemaOne = 16,
		SchemaMany = 17,
		SchemaRequired = 18,
		SchemaYes = 19,
		SchemaNo = 20,
		SchemaString = 21,
		SchemaId = 22,
		SchemaIdref = 23,
		SchemaIdrefs = 24,
		SchemaEntity = 25,
		SchemaEntities = 26,
		SchemaNmtoken = 27,
		SchemaNmtokens = 28,
		SchemaEnumeration = 29,
		SchemaDefault = 30,
		XdrRoot = 31,
		XdrElementType = 32,
		XdrElement = 33,
		XdrGroup = 34,
		XdrAttributeType = 35,
		XdrAttribute = 36,
		XdrDatatype = 37,
		XdrDescription = 38,
		XdrExtends = 39,
		SchemaXdrRootAlias = 40,
		SchemaDtType = 41,
		SchemaDtValues = 42,
		SchemaDtMaxLength = 43,
		SchemaDtMinLength = 44,
		SchemaDtMax = 45,
		SchemaDtMin = 46,
		SchemaDtMinExclusive = 47,
		SchemaDtMaxExclusive = 48,
		SchemaTargetNamespace = 49,
		SchemaVersion = 50,
		SchemaFinalDefault = 51,
		SchemaBlockDefault = 52,
		SchemaFixed = 53,
		SchemaAbstract = 54,
		SchemaBlock = 55,
		SchemaSubstitutionGroup = 56,
		SchemaFinal = 57,
		SchemaNillable = 58,
		SchemaRef = 59,
		SchemaBase = 60,
		SchemaDerivedBy = 61,
		SchemaNamespace = 62,
		SchemaProcessContents = 63,
		SchemaRefer = 64,
		SchemaPublic = 65,
		SchemaSystem = 66,
		SchemaSchemaLocation = 67,
		SchemaValue = 68,
		SchemaSource = 69,
		SchemaAttributeFormDefault = 70,
		SchemaElementFormDefault = 71,
		SchemaUse = 72,
		SchemaForm = 73,
		XsdSchema = 74,
		XsdAnnotation = 75,
		XsdInclude = 76,
		XsdImport = 77,
		XsdElement = 78,
		XsdAttribute = 79,
		xsdAttributeGroup = 80,
		XsdAnyAttribute = 81,
		XsdGroup = 82,
		XsdAll = 83,
		XsdChoice = 84,
		XsdSequence = 85,
		XsdAny = 86,
		XsdNotation = 87,
		XsdSimpleType = 88,
		XsdComplexType = 89,
		XsdUnique = 90,
		XsdKey = 91,
		XsdKeyref = 92,
		XsdSelector = 93,
		XsdField = 94,
		XsdMinExclusive = 95,
		XsdMinInclusive = 96,
		XsdMaxExclusive = 97,
		XsdMaxInclusive = 98,
		XsdTotalDigits = 99,
		XsdFractionDigits = 100,
		XsdLength = 101,
		XsdMinLength = 102,
		XsdMaxLength = 103,
		XsdEnumeration = 104,
		XsdPattern = 105,
		XsdDocumentation = 106,
		XsdAppInfo = 107,
		XsdComplexContent = 108,
		XsdComplexContentExtension = 109,
		XsdComplexContentRestriction = 110,
		XsdSimpleContent = 111,
		XsdSimpleContentExtension = 112,
		XsdSimpleContentRestriction = 113,
		XsdSimpleTypeList = 114,
		XsdSimpleTypeRestriction = 115,
		XsdSimpleTypeUnion = 116,
		XsdWhitespace = 117,
		XsdRedefine = 118,
		SchemaItemType = 119,
		SchemaMemberTypes = 120,
		SchemaXPath = 121,
		XmlLang = 122
	}

	private XmlNameTable nameTable;

	public string NsDataType;

	public string NsDataTypeAlias;

	public string NsDataTypeOld;

	public string NsXml;

	public string NsXmlNs;

	public string NsXdr;

	public string NsXdrAlias;

	public string NsXs;

	public string NsXsi;

	public string XsiType;

	public string XsiNil;

	public string XsiSchemaLocation;

	public string XsiNoNamespaceSchemaLocation;

	public string XsdSchema;

	public string XdrSchema;

	public XmlQualifiedName QnPCData;

	public XmlQualifiedName QnXml;

	public XmlQualifiedName QnXmlNs;

	public XmlQualifiedName QnDtDt;

	public XmlQualifiedName QnXmlLang;

	public XmlQualifiedName QnName;

	public XmlQualifiedName QnType;

	public XmlQualifiedName QnMaxOccurs;

	public XmlQualifiedName QnMinOccurs;

	public XmlQualifiedName QnInfinite;

	public XmlQualifiedName QnModel;

	public XmlQualifiedName QnOpen;

	public XmlQualifiedName QnClosed;

	public XmlQualifiedName QnContent;

	public XmlQualifiedName QnMixed;

	public XmlQualifiedName QnEmpty;

	public XmlQualifiedName QnEltOnly;

	public XmlQualifiedName QnTextOnly;

	public XmlQualifiedName QnOrder;

	public XmlQualifiedName QnSeq;

	public XmlQualifiedName QnOne;

	public XmlQualifiedName QnMany;

	public XmlQualifiedName QnRequired;

	public XmlQualifiedName QnYes;

	public XmlQualifiedName QnNo;

	public XmlQualifiedName QnString;

	public XmlQualifiedName QnID;

	public XmlQualifiedName QnIDRef;

	public XmlQualifiedName QnIDRefs;

	public XmlQualifiedName QnEntity;

	public XmlQualifiedName QnEntities;

	public XmlQualifiedName QnNmToken;

	public XmlQualifiedName QnNmTokens;

	public XmlQualifiedName QnEnumeration;

	public XmlQualifiedName QnDefault;

	public XmlQualifiedName QnXdrSchema;

	public XmlQualifiedName QnXdrElementType;

	public XmlQualifiedName QnXdrElement;

	public XmlQualifiedName QnXdrGroup;

	public XmlQualifiedName QnXdrAttributeType;

	public XmlQualifiedName QnXdrAttribute;

	public XmlQualifiedName QnXdrDataType;

	public XmlQualifiedName QnXdrDescription;

	public XmlQualifiedName QnXdrExtends;

	public XmlQualifiedName QnXdrAliasSchema;

	public XmlQualifiedName QnDtType;

	public XmlQualifiedName QnDtValues;

	public XmlQualifiedName QnDtMaxLength;

	public XmlQualifiedName QnDtMinLength;

	public XmlQualifiedName QnDtMax;

	public XmlQualifiedName QnDtMin;

	public XmlQualifiedName QnDtMinExclusive;

	public XmlQualifiedName QnDtMaxExclusive;

	public XmlQualifiedName QnTargetNamespace;

	public XmlQualifiedName QnVersion;

	public XmlQualifiedName QnFinalDefault;

	public XmlQualifiedName QnBlockDefault;

	public XmlQualifiedName QnFixed;

	public XmlQualifiedName QnAbstract;

	public XmlQualifiedName QnBlock;

	public XmlQualifiedName QnSubstitutionGroup;

	public XmlQualifiedName QnFinal;

	public XmlQualifiedName QnNillable;

	public XmlQualifiedName QnRef;

	public XmlQualifiedName QnBase;

	public XmlQualifiedName QnDerivedBy;

	public XmlQualifiedName QnNamespace;

	public XmlQualifiedName QnProcessContents;

	public XmlQualifiedName QnRefer;

	public XmlQualifiedName QnPublic;

	public XmlQualifiedName QnSystem;

	public XmlQualifiedName QnSchemaLocation;

	public XmlQualifiedName QnValue;

	public XmlQualifiedName QnUse;

	public XmlQualifiedName QnForm;

	public XmlQualifiedName QnElementFormDefault;

	public XmlQualifiedName QnAttributeFormDefault;

	public XmlQualifiedName QnItemType;

	public XmlQualifiedName QnMemberTypes;

	public XmlQualifiedName QnXPath;

	public XmlQualifiedName QnXsdSchema;

	public XmlQualifiedName QnXsdAnnotation;

	public XmlQualifiedName QnXsdInclude;

	public XmlQualifiedName QnXsdImport;

	public XmlQualifiedName QnXsdElement;

	public XmlQualifiedName QnXsdAttribute;

	public XmlQualifiedName QnXsdAttributeGroup;

	public XmlQualifiedName QnXsdAnyAttribute;

	public XmlQualifiedName QnXsdGroup;

	public XmlQualifiedName QnXsdAll;

	public XmlQualifiedName QnXsdChoice;

	public XmlQualifiedName QnXsdSequence;

	public XmlQualifiedName QnXsdAny;

	public XmlQualifiedName QnXsdNotation;

	public XmlQualifiedName QnXsdSimpleType;

	public XmlQualifiedName QnXsdComplexType;

	public XmlQualifiedName QnXsdUnique;

	public XmlQualifiedName QnXsdKey;

	public XmlQualifiedName QnXsdKeyRef;

	public XmlQualifiedName QnXsdSelector;

	public XmlQualifiedName QnXsdField;

	public XmlQualifiedName QnXsdMinExclusive;

	public XmlQualifiedName QnXsdMinInclusive;

	public XmlQualifiedName QnXsdMaxInclusive;

	public XmlQualifiedName QnXsdMaxExclusive;

	public XmlQualifiedName QnXsdTotalDigits;

	public XmlQualifiedName QnXsdFractionDigits;

	public XmlQualifiedName QnXsdLength;

	public XmlQualifiedName QnXsdMinLength;

	public XmlQualifiedName QnXsdMaxLength;

	public XmlQualifiedName QnXsdEnumeration;

	public XmlQualifiedName QnXsdPattern;

	public XmlQualifiedName QnXsdDocumentation;

	public XmlQualifiedName QnXsdAppinfo;

	public XmlQualifiedName QnSource;

	public XmlQualifiedName QnXsdComplexContent;

	public XmlQualifiedName QnXsdSimpleContent;

	public XmlQualifiedName QnXsdRestriction;

	public XmlQualifiedName QnXsdExtension;

	public XmlQualifiedName QnXsdUnion;

	public XmlQualifiedName QnXsdList;

	public XmlQualifiedName QnXsdWhiteSpace;

	public XmlQualifiedName QnXsdRedefine;

	public XmlQualifiedName QnXsdAnyType;

	internal XmlQualifiedName[] TokenToQName = new XmlQualifiedName[123];

	public XmlNameTable NameTable => nameTable;

	public SchemaNames(XmlNameTable nameTable)
	{
		this.nameTable = nameTable;
		NsDataType = nameTable.Add("urn:schemas-microsoft-com:datatypes");
		NsDataTypeAlias = nameTable.Add("uuid:C2F41010-65B3-11D1-A29F-00AA00C14882");
		NsDataTypeOld = nameTable.Add("urn:uuid:C2F41010-65B3-11D1-A29F-00AA00C14882/");
		NsXml = nameTable.Add("http://www.w3.org/XML/1998/namespace");
		NsXmlNs = nameTable.Add("http://www.w3.org/2000/xmlns/");
		NsXdr = nameTable.Add("urn:schemas-microsoft-com:xml-data");
		NsXdrAlias = nameTable.Add("uuid:BDC6E3F0-6DA3-11D1-A2A3-00AA00C14882");
		NsXs = nameTable.Add("http://www.w3.org/2001/XMLSchema");
		NsXsi = nameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
		XsiType = nameTable.Add("type");
		XsiNil = nameTable.Add("nil");
		XsiSchemaLocation = nameTable.Add("schemaLocation");
		XsiNoNamespaceSchemaLocation = nameTable.Add("noNamespaceSchemaLocation");
		XsdSchema = nameTable.Add("schema");
		XdrSchema = nameTable.Add("Schema");
		QnPCData = new XmlQualifiedName(nameTable.Add("#PCDATA"));
		QnXml = new XmlQualifiedName(nameTable.Add("xml"));
		QnXmlNs = new XmlQualifiedName(nameTable.Add("xmlns"), NsXmlNs);
		QnDtDt = new XmlQualifiedName(nameTable.Add("dt"), NsDataType);
		QnXmlLang = new XmlQualifiedName(nameTable.Add("lang"), NsXml);
		QnName = new XmlQualifiedName(nameTable.Add("name"));
		QnType = new XmlQualifiedName(nameTable.Add("type"));
		QnMaxOccurs = new XmlQualifiedName(nameTable.Add("maxOccurs"));
		QnMinOccurs = new XmlQualifiedName(nameTable.Add("minOccurs"));
		QnInfinite = new XmlQualifiedName(nameTable.Add("*"));
		QnModel = new XmlQualifiedName(nameTable.Add("model"));
		QnOpen = new XmlQualifiedName(nameTable.Add("open"));
		QnClosed = new XmlQualifiedName(nameTable.Add("closed"));
		QnContent = new XmlQualifiedName(nameTable.Add("content"));
		QnMixed = new XmlQualifiedName(nameTable.Add("mixed"));
		QnEmpty = new XmlQualifiedName(nameTable.Add("empty"));
		QnEltOnly = new XmlQualifiedName(nameTable.Add("eltOnly"));
		QnTextOnly = new XmlQualifiedName(nameTable.Add("textOnly"));
		QnOrder = new XmlQualifiedName(nameTable.Add("order"));
		QnSeq = new XmlQualifiedName(nameTable.Add("seq"));
		QnOne = new XmlQualifiedName(nameTable.Add("one"));
		QnMany = new XmlQualifiedName(nameTable.Add("many"));
		QnRequired = new XmlQualifiedName(nameTable.Add("required"));
		QnYes = new XmlQualifiedName(nameTable.Add("yes"));
		QnNo = new XmlQualifiedName(nameTable.Add("no"));
		QnString = new XmlQualifiedName(nameTable.Add("string"));
		QnID = new XmlQualifiedName(nameTable.Add("id"));
		QnIDRef = new XmlQualifiedName(nameTable.Add("idref"));
		QnIDRefs = new XmlQualifiedName(nameTable.Add("idrefs"));
		QnEntity = new XmlQualifiedName(nameTable.Add("entity"));
		QnEntities = new XmlQualifiedName(nameTable.Add("entities"));
		QnNmToken = new XmlQualifiedName(nameTable.Add("nmtoken"));
		QnNmTokens = new XmlQualifiedName(nameTable.Add("nmtokens"));
		QnEnumeration = new XmlQualifiedName(nameTable.Add("enumeration"));
		QnDefault = new XmlQualifiedName(nameTable.Add("default"));
		QnTargetNamespace = new XmlQualifiedName(nameTable.Add("targetNamespace"));
		QnVersion = new XmlQualifiedName(nameTable.Add("version"));
		QnFinalDefault = new XmlQualifiedName(nameTable.Add("finalDefault"));
		QnBlockDefault = new XmlQualifiedName(nameTable.Add("blockDefault"));
		QnFixed = new XmlQualifiedName(nameTable.Add("fixed"));
		QnAbstract = new XmlQualifiedName(nameTable.Add("abstract"));
		QnBlock = new XmlQualifiedName(nameTable.Add("block"));
		QnSubstitutionGroup = new XmlQualifiedName(nameTable.Add("substitutionGroup"));
		QnFinal = new XmlQualifiedName(nameTable.Add("final"));
		QnNillable = new XmlQualifiedName(nameTable.Add("nillable"));
		QnRef = new XmlQualifiedName(nameTable.Add("ref"));
		QnBase = new XmlQualifiedName(nameTable.Add("base"));
		QnDerivedBy = new XmlQualifiedName(nameTable.Add("derivedBy"));
		QnNamespace = new XmlQualifiedName(nameTable.Add("namespace"));
		QnProcessContents = new XmlQualifiedName(nameTable.Add("processContents"));
		QnRefer = new XmlQualifiedName(nameTable.Add("refer"));
		QnPublic = new XmlQualifiedName(nameTable.Add("public"));
		QnSystem = new XmlQualifiedName(nameTable.Add("system"));
		QnSchemaLocation = new XmlQualifiedName(nameTable.Add("schemaLocation"));
		QnValue = new XmlQualifiedName(nameTable.Add("value"));
		QnUse = new XmlQualifiedName(nameTable.Add("use"));
		QnForm = new XmlQualifiedName(nameTable.Add("form"));
		QnAttributeFormDefault = new XmlQualifiedName(nameTable.Add("attributeFormDefault"));
		QnElementFormDefault = new XmlQualifiedName(nameTable.Add("elementFormDefault"));
		QnSource = new XmlQualifiedName(nameTable.Add("source"));
		QnMemberTypes = new XmlQualifiedName(nameTable.Add("memberTypes"));
		QnItemType = new XmlQualifiedName(nameTable.Add("itemType"));
		QnXPath = new XmlQualifiedName(nameTable.Add("xpath"));
		QnXdrSchema = new XmlQualifiedName(XdrSchema, NsXdr);
		QnXdrElementType = new XmlQualifiedName(nameTable.Add("ElementType"), NsXdr);
		QnXdrElement = new XmlQualifiedName(nameTable.Add("element"), NsXdr);
		QnXdrGroup = new XmlQualifiedName(nameTable.Add("group"), NsXdr);
		QnXdrAttributeType = new XmlQualifiedName(nameTable.Add("AttributeType"), NsXdr);
		QnXdrAttribute = new XmlQualifiedName(nameTable.Add("attribute"), NsXdr);
		QnXdrDataType = new XmlQualifiedName(nameTable.Add("datatype"), NsXdr);
		QnXdrDescription = new XmlQualifiedName(nameTable.Add("description"), NsXdr);
		QnXdrExtends = new XmlQualifiedName(nameTable.Add("extends"), NsXdr);
		QnXdrAliasSchema = new XmlQualifiedName(nameTable.Add("Schema"), NsDataTypeAlias);
		QnDtType = new XmlQualifiedName(nameTable.Add("type"), NsDataType);
		QnDtValues = new XmlQualifiedName(nameTable.Add("values"), NsDataType);
		QnDtMaxLength = new XmlQualifiedName(nameTable.Add("maxLength"), NsDataType);
		QnDtMinLength = new XmlQualifiedName(nameTable.Add("minLength"), NsDataType);
		QnDtMax = new XmlQualifiedName(nameTable.Add("max"), NsDataType);
		QnDtMin = new XmlQualifiedName(nameTable.Add("min"), NsDataType);
		QnDtMinExclusive = new XmlQualifiedName(nameTable.Add("minExclusive"), NsDataType);
		QnDtMaxExclusive = new XmlQualifiedName(nameTable.Add("maxExclusive"), NsDataType);
		QnXsdSchema = new XmlQualifiedName(XsdSchema, NsXs);
		QnXsdAnnotation = new XmlQualifiedName(nameTable.Add("annotation"), NsXs);
		QnXsdInclude = new XmlQualifiedName(nameTable.Add("include"), NsXs);
		QnXsdImport = new XmlQualifiedName(nameTable.Add("import"), NsXs);
		QnXsdElement = new XmlQualifiedName(nameTable.Add("element"), NsXs);
		QnXsdAttribute = new XmlQualifiedName(nameTable.Add("attribute"), NsXs);
		QnXsdAttributeGroup = new XmlQualifiedName(nameTable.Add("attributeGroup"), NsXs);
		QnXsdAnyAttribute = new XmlQualifiedName(nameTable.Add("anyAttribute"), NsXs);
		QnXsdGroup = new XmlQualifiedName(nameTable.Add("group"), NsXs);
		QnXsdAll = new XmlQualifiedName(nameTable.Add("all"), NsXs);
		QnXsdChoice = new XmlQualifiedName(nameTable.Add("choice"), NsXs);
		QnXsdSequence = new XmlQualifiedName(nameTable.Add("sequence"), NsXs);
		QnXsdAny = new XmlQualifiedName(nameTable.Add("any"), NsXs);
		QnXsdNotation = new XmlQualifiedName(nameTable.Add("notation"), NsXs);
		QnXsdSimpleType = new XmlQualifiedName(nameTable.Add("simpleType"), NsXs);
		QnXsdComplexType = new XmlQualifiedName(nameTable.Add("complexType"), NsXs);
		QnXsdUnique = new XmlQualifiedName(nameTable.Add("unique"), NsXs);
		QnXsdKey = new XmlQualifiedName(nameTable.Add("key"), NsXs);
		QnXsdKeyRef = new XmlQualifiedName(nameTable.Add("keyref"), NsXs);
		QnXsdSelector = new XmlQualifiedName(nameTable.Add("selector"), NsXs);
		QnXsdField = new XmlQualifiedName(nameTable.Add("field"), NsXs);
		QnXsdMinExclusive = new XmlQualifiedName(nameTable.Add("minExclusive"), NsXs);
		QnXsdMinInclusive = new XmlQualifiedName(nameTable.Add("minInclusive"), NsXs);
		QnXsdMaxInclusive = new XmlQualifiedName(nameTable.Add("maxInclusive"), NsXs);
		QnXsdMaxExclusive = new XmlQualifiedName(nameTable.Add("maxExclusive"), NsXs);
		QnXsdTotalDigits = new XmlQualifiedName(nameTable.Add("totalDigits"), NsXs);
		QnXsdFractionDigits = new XmlQualifiedName(nameTable.Add("fractionDigits"), NsXs);
		QnXsdLength = new XmlQualifiedName(nameTable.Add("length"), NsXs);
		QnXsdMinLength = new XmlQualifiedName(nameTable.Add("minLength"), NsXs);
		QnXsdMaxLength = new XmlQualifiedName(nameTable.Add("maxLength"), NsXs);
		QnXsdEnumeration = new XmlQualifiedName(nameTable.Add("enumeration"), NsXs);
		QnXsdPattern = new XmlQualifiedName(nameTable.Add("pattern"), NsXs);
		QnXsdDocumentation = new XmlQualifiedName(nameTable.Add("documentation"), NsXs);
		QnXsdAppinfo = new XmlQualifiedName(nameTable.Add("appinfo"), NsXs);
		QnXsdComplexContent = new XmlQualifiedName(nameTable.Add("complexContent"), NsXs);
		QnXsdSimpleContent = new XmlQualifiedName(nameTable.Add("simpleContent"), NsXs);
		QnXsdRestriction = new XmlQualifiedName(nameTable.Add("restriction"), NsXs);
		QnXsdExtension = new XmlQualifiedName(nameTable.Add("extension"), NsXs);
		QnXsdUnion = new XmlQualifiedName(nameTable.Add("union"), NsXs);
		QnXsdList = new XmlQualifiedName(nameTable.Add("list"), NsXs);
		QnXsdWhiteSpace = new XmlQualifiedName(nameTable.Add("whiteSpace"), NsXs);
		QnXsdRedefine = new XmlQualifiedName(nameTable.Add("redefine"), NsXs);
		QnXsdAnyType = new XmlQualifiedName(nameTable.Add("anyType"), NsXs);
		CreateTokenToQNameTable();
	}

	public void CreateTokenToQNameTable()
	{
		TokenToQName[1] = QnName;
		TokenToQName[2] = QnType;
		TokenToQName[3] = QnMaxOccurs;
		TokenToQName[4] = QnMinOccurs;
		TokenToQName[5] = QnInfinite;
		TokenToQName[6] = QnModel;
		TokenToQName[7] = QnOpen;
		TokenToQName[8] = QnClosed;
		TokenToQName[9] = QnContent;
		TokenToQName[10] = QnMixed;
		TokenToQName[11] = QnEmpty;
		TokenToQName[12] = QnEltOnly;
		TokenToQName[13] = QnTextOnly;
		TokenToQName[14] = QnOrder;
		TokenToQName[15] = QnSeq;
		TokenToQName[16] = QnOne;
		TokenToQName[17] = QnMany;
		TokenToQName[18] = QnRequired;
		TokenToQName[19] = QnYes;
		TokenToQName[20] = QnNo;
		TokenToQName[21] = QnString;
		TokenToQName[22] = QnID;
		TokenToQName[23] = QnIDRef;
		TokenToQName[24] = QnIDRefs;
		TokenToQName[25] = QnEntity;
		TokenToQName[26] = QnEntities;
		TokenToQName[27] = QnNmToken;
		TokenToQName[28] = QnNmTokens;
		TokenToQName[29] = QnEnumeration;
		TokenToQName[30] = QnDefault;
		TokenToQName[31] = QnXdrSchema;
		TokenToQName[32] = QnXdrElementType;
		TokenToQName[33] = QnXdrElement;
		TokenToQName[34] = QnXdrGroup;
		TokenToQName[35] = QnXdrAttributeType;
		TokenToQName[36] = QnXdrAttribute;
		TokenToQName[37] = QnXdrDataType;
		TokenToQName[38] = QnXdrDescription;
		TokenToQName[39] = QnXdrExtends;
		TokenToQName[40] = QnXdrAliasSchema;
		TokenToQName[41] = QnDtType;
		TokenToQName[42] = QnDtValues;
		TokenToQName[43] = QnDtMaxLength;
		TokenToQName[44] = QnDtMinLength;
		TokenToQName[45] = QnDtMax;
		TokenToQName[46] = QnDtMin;
		TokenToQName[47] = QnDtMinExclusive;
		TokenToQName[48] = QnDtMaxExclusive;
		TokenToQName[49] = QnTargetNamespace;
		TokenToQName[50] = QnVersion;
		TokenToQName[51] = QnFinalDefault;
		TokenToQName[52] = QnBlockDefault;
		TokenToQName[53] = QnFixed;
		TokenToQName[54] = QnAbstract;
		TokenToQName[55] = QnBlock;
		TokenToQName[56] = QnSubstitutionGroup;
		TokenToQName[57] = QnFinal;
		TokenToQName[58] = QnNillable;
		TokenToQName[59] = QnRef;
		TokenToQName[60] = QnBase;
		TokenToQName[61] = QnDerivedBy;
		TokenToQName[62] = QnNamespace;
		TokenToQName[63] = QnProcessContents;
		TokenToQName[64] = QnRefer;
		TokenToQName[65] = QnPublic;
		TokenToQName[66] = QnSystem;
		TokenToQName[67] = QnSchemaLocation;
		TokenToQName[68] = QnValue;
		TokenToQName[119] = QnItemType;
		TokenToQName[120] = QnMemberTypes;
		TokenToQName[121] = QnXPath;
		TokenToQName[74] = QnXsdSchema;
		TokenToQName[75] = QnXsdAnnotation;
		TokenToQName[76] = QnXsdInclude;
		TokenToQName[77] = QnXsdImport;
		TokenToQName[78] = QnXsdElement;
		TokenToQName[79] = QnXsdAttribute;
		TokenToQName[80] = QnXsdAttributeGroup;
		TokenToQName[81] = QnXsdAnyAttribute;
		TokenToQName[82] = QnXsdGroup;
		TokenToQName[83] = QnXsdAll;
		TokenToQName[84] = QnXsdChoice;
		TokenToQName[85] = QnXsdSequence;
		TokenToQName[86] = QnXsdAny;
		TokenToQName[87] = QnXsdNotation;
		TokenToQName[88] = QnXsdSimpleType;
		TokenToQName[89] = QnXsdComplexType;
		TokenToQName[90] = QnXsdUnique;
		TokenToQName[91] = QnXsdKey;
		TokenToQName[92] = QnXsdKeyRef;
		TokenToQName[93] = QnXsdSelector;
		TokenToQName[94] = QnXsdField;
		TokenToQName[95] = QnXsdMinExclusive;
		TokenToQName[96] = QnXsdMinInclusive;
		TokenToQName[97] = QnXsdMaxExclusive;
		TokenToQName[98] = QnXsdMaxInclusive;
		TokenToQName[99] = QnXsdTotalDigits;
		TokenToQName[100] = QnXsdFractionDigits;
		TokenToQName[101] = QnXsdLength;
		TokenToQName[102] = QnXsdMinLength;
		TokenToQName[103] = QnXsdMaxLength;
		TokenToQName[104] = QnXsdEnumeration;
		TokenToQName[105] = QnXsdPattern;
		TokenToQName[117] = QnXsdWhiteSpace;
		TokenToQName[106] = QnXsdDocumentation;
		TokenToQName[107] = QnXsdAppinfo;
		TokenToQName[108] = QnXsdComplexContent;
		TokenToQName[110] = QnXsdRestriction;
		TokenToQName[113] = QnXsdRestriction;
		TokenToQName[115] = QnXsdRestriction;
		TokenToQName[109] = QnXsdExtension;
		TokenToQName[112] = QnXsdExtension;
		TokenToQName[111] = QnXsdSimpleContent;
		TokenToQName[116] = QnXsdUnion;
		TokenToQName[114] = QnXsdList;
		TokenToQName[118] = QnXsdRedefine;
		TokenToQName[69] = QnSource;
		TokenToQName[72] = QnUse;
		TokenToQName[73] = QnForm;
		TokenToQName[71] = QnElementFormDefault;
		TokenToQName[70] = QnAttributeFormDefault;
		TokenToQName[122] = QnXmlLang;
		TokenToQName[0] = XmlQualifiedName.Empty;
	}

	public SchemaType SchemaTypeFromRoot(string localName, string ns)
	{
		if (IsXSDRoot(localName, ns))
		{
			return SchemaType.XSD;
		}
		if (IsXDRRoot(localName, XmlSchemaDatatype.XdrCanonizeUri(ns, nameTable, this)))
		{
			return SchemaType.XDR;
		}
		return SchemaType.None;
	}

	public bool IsXSDRoot(string localName, string ns)
	{
		if (localName == XsdSchema)
		{
			return ns == NsXs;
		}
		return false;
	}

	public bool IsXDRRoot(string localName, string ns)
	{
		if (localName == XdrSchema)
		{
			return ns == NsXdr;
		}
		return false;
	}

	public XmlQualifiedName GetName(Token token)
	{
		return TokenToQName[(int)token];
	}
}
