using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization;

internal static class Globals
{
	internal const BindingFlags ScanAllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	[SecurityCritical]
	private static XmlQualifiedName idQualifiedName;

	[SecurityCritical]
	private static XmlQualifiedName refQualifiedName;

	[SecurityCritical]
	private static Type typeOfObject;

	[SecurityCritical]
	private static Type typeOfValueType;

	[SecurityCritical]
	private static Type typeOfArray;

	[SecurityCritical]
	private static Type typeOfString;

	[SecurityCritical]
	private static Type typeOfInt;

	[SecurityCritical]
	private static Type typeOfULong;

	[SecurityCritical]
	private static Type typeOfVoid;

	[SecurityCritical]
	private static Type typeOfByteArray;

	[SecurityCritical]
	private static Type typeOfTimeSpan;

	[SecurityCritical]
	private static Type typeOfGuid;

	[SecurityCritical]
	private static Type typeOfDateTimeOffset;

	[SecurityCritical]
	private static Type typeOfDateTimeOffsetAdapter;

	[SecurityCritical]
	private static Type typeOfUri;

	[SecurityCritical]
	private static Type typeOfTypeEnumerable;

	[SecurityCritical]
	private static Type typeOfStreamingContext;

	[SecurityCritical]
	private static Type typeOfISerializable;

	[SecurityCritical]
	private static Type typeOfIDeserializationCallback;

	[SecurityCritical]
	private static Type typeOfIObjectReference;

	[SecurityCritical]
	private static Type typeOfXmlFormatClassWriterDelegate;

	[SecurityCritical]
	private static Type typeOfXmlFormatCollectionWriterDelegate;

	[SecurityCritical]
	private static Type typeOfXmlFormatClassReaderDelegate;

	[SecurityCritical]
	private static Type typeOfXmlFormatCollectionReaderDelegate;

	[SecurityCritical]
	private static Type typeOfXmlFormatGetOnlyCollectionReaderDelegate;

	[SecurityCritical]
	private static Type typeOfKnownTypeAttribute;

	[SecurityCritical]
	private static Type typeOfDataContractAttribute;

	[SecurityCritical]
	private static Type typeOfContractNamespaceAttribute;

	[SecurityCritical]
	private static Type typeOfDataMemberAttribute;

	[SecurityCritical]
	private static Type typeOfEnumMemberAttribute;

	[SecurityCritical]
	private static Type typeOfCollectionDataContractAttribute;

	[SecurityCritical]
	private static Type typeOfOptionalFieldAttribute;

	[SecurityCritical]
	private static Type typeOfObjectArray;

	[SecurityCritical]
	private static Type typeOfOnSerializingAttribute;

	[SecurityCritical]
	private static Type typeOfOnSerializedAttribute;

	[SecurityCritical]
	private static Type typeOfOnDeserializingAttribute;

	[SecurityCritical]
	private static Type typeOfOnDeserializedAttribute;

	[SecurityCritical]
	private static Type typeOfFlagsAttribute;

	[SecurityCritical]
	private static Type typeOfSerializableAttribute;

	[SecurityCritical]
	private static Type typeOfNonSerializedAttribute;

	[SecurityCritical]
	private static Type typeOfSerializationInfo;

	[SecurityCritical]
	private static Type typeOfSerializationInfoEnumerator;

	[SecurityCritical]
	private static Type typeOfSerializationEntry;

	[SecurityCritical]
	private static Type typeOfIXmlSerializable;

	[SecurityCritical]
	private static Type typeOfXmlSchemaProviderAttribute;

	[SecurityCritical]
	private static Type typeOfXmlRootAttribute;

	[SecurityCritical]
	private static Type typeOfXmlQualifiedName;

	[SecurityCritical]
	private static Type typeOfXmlSchemaType;

	[SecurityCritical]
	private static Type typeOfXmlSerializableServices;

	[SecurityCritical]
	private static Type typeOfXmlNodeArray;

	[SecurityCritical]
	private static Type typeOfXmlSchemaSet;

	[SecurityCritical]
	private static object[] emptyObjectArray;

	[SecurityCritical]
	private static Type[] emptyTypeArray;

	[SecurityCritical]
	private static Type typeOfIPropertyChange;

	[SecurityCritical]
	private static Type typeOfIExtensibleDataObject;

	[SecurityCritical]
	private static Type typeOfExtensionDataObject;

	[SecurityCritical]
	private static Type typeOfISerializableDataNode;

	[SecurityCritical]
	private static Type typeOfClassDataNode;

	[SecurityCritical]
	private static Type typeOfCollectionDataNode;

	[SecurityCritical]
	private static Type typeOfXmlDataNode;

	[SecurityCritical]
	private static Type typeOfNullable;

	[SecurityCritical]
	private static Type typeOfReflectionPointer;

	[SecurityCritical]
	private static Type typeOfIDictionaryGeneric;

	[SecurityCritical]
	private static Type typeOfIDictionary;

	[SecurityCritical]
	private static Type typeOfIListGeneric;

	[SecurityCritical]
	private static Type typeOfIList;

	[SecurityCritical]
	private static Type typeOfICollectionGeneric;

	[SecurityCritical]
	private static Type typeOfICollection;

	[SecurityCritical]
	private static Type typeOfIEnumerableGeneric;

	[SecurityCritical]
	private static Type typeOfIEnumerable;

	[SecurityCritical]
	private static Type typeOfIEnumeratorGeneric;

	[SecurityCritical]
	private static Type typeOfIEnumerator;

	[SecurityCritical]
	private static Type typeOfKeyValuePair;

	[SecurityCritical]
	private static Type typeOfKeyValue;

	[SecurityCritical]
	private static Type typeOfIDictionaryEnumerator;

	[SecurityCritical]
	private static Type typeOfDictionaryEnumerator;

	[SecurityCritical]
	private static Type typeOfGenericDictionaryEnumerator;

	[SecurityCritical]
	private static Type typeOfDictionaryGeneric;

	[SecurityCritical]
	private static Type typeOfHashtable;

	[SecurityCritical]
	private static Type typeOfListGeneric;

	[SecurityCritical]
	private static Type typeOfXmlElement;

	[SecurityCritical]
	private static Type typeOfDBNull;

	[SecurityCritical]
	private static Uri dataContractXsdBaseNamespaceUri;

	public const bool DefaultIsRequired = false;

	public const bool DefaultEmitDefaultValue = true;

	public const int DefaultOrder = 0;

	public const bool DefaultIsReference = false;

	public static readonly string NewObjectId = string.Empty;

	public const string SimpleSRSInternalsVisiblePattern = "^[\\s]*System\\.Runtime\\.Serialization[\\s]*$";

	public const string FullSRSInternalsVisiblePattern = "^[\\s]*System\\.Runtime\\.Serialization[\\s]*,[\\s]*PublicKey[\\s]*=[\\s]*(?i:00000000000000000400000000000000)[\\s]*$";

	public const string NullObjectId = null;

	public const string Space = " ";

	public const string OpenBracket = "[";

	public const string CloseBracket = "]";

	public const string Comma = ",";

	public const string XsiPrefix = "i";

	public const string XsdPrefix = "x";

	public const string SerPrefix = "z";

	public const string SerPrefixForSchema = "ser";

	public const string ElementPrefix = "q";

	public const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";

	public const string DataContractXmlNamespace = "http://schemas.datacontract.org/2004/07/System.Xml";

	public const string SchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

	public const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";

	public const string XsiNilLocalName = "nil";

	public const string XsiTypeLocalName = "type";

	public const string TnsPrefix = "tns";

	public const string OccursUnbounded = "unbounded";

	public const string AnyTypeLocalName = "anyType";

	public const string StringLocalName = "string";

	public const string IntLocalName = "int";

	public const string True = "true";

	public const string False = "false";

	public const string ArrayPrefix = "ArrayOf";

	public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

	public const string XmlnsPrefix = "xmlns";

	public const string SchemaLocalName = "schema";

	public const string CollectionsNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

	public const string DefaultClrNamespace = "GeneratedNamespace";

	public const string DefaultTypeName = "GeneratedType";

	public const string DefaultGeneratedMember = "GeneratedMember";

	public const string DefaultFieldSuffix = "Field";

	public const string DefaultPropertySuffix = "Property";

	public const string DefaultMemberSuffix = "Member";

	public const string NameProperty = "Name";

	public const string NamespaceProperty = "Namespace";

	public const string OrderProperty = "Order";

	public const string IsReferenceProperty = "IsReference";

	public const string IsRequiredProperty = "IsRequired";

	public const string EmitDefaultValueProperty = "EmitDefaultValue";

	public const string ClrNamespaceProperty = "ClrNamespace";

	public const string ItemNameProperty = "ItemName";

	public const string KeyNameProperty = "KeyName";

	public const string ValueNameProperty = "ValueName";

	public const string SerializationInfoPropertyName = "SerializationInfo";

	public const string SerializationInfoFieldName = "info";

	public const string NodeArrayPropertyName = "Nodes";

	public const string NodeArrayFieldName = "nodesField";

	public const string ExportSchemaMethod = "ExportSchema";

	public const string IsAnyProperty = "IsAny";

	public const string ContextFieldName = "context";

	public const string GetObjectDataMethodName = "GetObjectData";

	public const string GetEnumeratorMethodName = "GetEnumerator";

	public const string MoveNextMethodName = "MoveNext";

	public const string AddValueMethodName = "AddValue";

	public const string CurrentPropertyName = "Current";

	public const string ValueProperty = "Value";

	public const string EnumeratorFieldName = "enumerator";

	public const string SerializationEntryFieldName = "entry";

	public const string ExtensionDataSetMethod = "set_ExtensionData";

	public const string ExtensionDataSetExplicitMethod = "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData";

	public const string ExtensionDataObjectPropertyName = "ExtensionData";

	public const string ExtensionDataObjectFieldName = "extensionDataField";

	public const string AddMethodName = "Add";

	public const string ParseMethodName = "Parse";

	public const string GetCurrentMethodName = "get_Current";

	public const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

	public const string ClrTypeLocalName = "Type";

	public const string ClrAssemblyLocalName = "Assembly";

	public const string IsValueTypeLocalName = "IsValueType";

	public const string EnumerationValueLocalName = "EnumerationValue";

	public const string SurrogateDataLocalName = "Surrogate";

	public const string GenericTypeLocalName = "GenericType";

	public const string GenericParameterLocalName = "GenericParameter";

	public const string GenericNameAttribute = "Name";

	public const string GenericNamespaceAttribute = "Namespace";

	public const string GenericParameterNestedLevelAttribute = "NestedLevel";

	public const string IsDictionaryLocalName = "IsDictionary";

	public const string ActualTypeLocalName = "ActualType";

	public const string ActualTypeNameAttribute = "Name";

	public const string ActualTypeNamespaceAttribute = "Namespace";

	public const string DefaultValueLocalName = "DefaultValue";

	public const string EmitDefaultValueAttribute = "EmitDefaultValue";

	public const string ISerializableFactoryTypeLocalName = "FactoryType";

	public const string IdLocalName = "Id";

	public const string RefLocalName = "Ref";

	public const string ArraySizeLocalName = "Size";

	public const string KeyLocalName = "Key";

	public const string ValueLocalName = "Value";

	public const string MscorlibAssemblyName = "0";

	public const string MscorlibAssemblySimpleName = "mscorlib";

	public const string MscorlibFileName = "mscorlib.dll";

	public const string SerializationSchema = "<?xml version='1.0' encoding='utf-8'?>\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\n  <xs:element name='byte' nillable='true' type='xs:byte' />\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\n  <xs:element name='double' nillable='true' type='xs:double' />\n  <xs:element name='float' nillable='true' type='xs:float' />\n  <xs:element name='int' nillable='true' type='xs:int' />\n  <xs:element name='long' nillable='true' type='xs:long' />\n  <xs:element name='QName' nillable='true' type='xs:QName' />\n  <xs:element name='short' nillable='true' type='xs:short' />\n  <xs:element name='string' nillable='true' type='xs:string' />\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\n  <xs:element name='char' nillable='true' type='tns:char' />\n  <xs:simpleType name='char'>\n    <xs:restriction base='xs:int'/>\n  </xs:simpleType>  \n  <xs:element name='duration' nillable='true' type='tns:duration' />\n  <xs:simpleType name='duration'>\n    <xs:restriction base='xs:duration'>\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:element name='guid' nillable='true' type='tns:guid' />\n  <xs:simpleType name='guid'>\n    <xs:restriction base='xs:string'>\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:attribute name='FactoryType' type='xs:QName' />\n  <xs:attribute name='Id' type='xs:ID' />\n  <xs:attribute name='Ref' type='xs:IDREF' />\n</xs:schema>\n";

	internal static XmlQualifiedName IdQualifiedName
	{
		[SecuritySafeCritical]
		get
		{
			if (idQualifiedName == null)
			{
				idQualifiedName = new XmlQualifiedName("Id", "http://schemas.microsoft.com/2003/10/Serialization/");
			}
			return idQualifiedName;
		}
	}

	internal static XmlQualifiedName RefQualifiedName
	{
		[SecuritySafeCritical]
		get
		{
			if (refQualifiedName == null)
			{
				refQualifiedName = new XmlQualifiedName("Ref", "http://schemas.microsoft.com/2003/10/Serialization/");
			}
			return refQualifiedName;
		}
	}

	internal static Type TypeOfObject
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfObject == null)
			{
				typeOfObject = typeof(object);
			}
			return typeOfObject;
		}
	}

	internal static Type TypeOfValueType
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfValueType == null)
			{
				typeOfValueType = typeof(ValueType);
			}
			return typeOfValueType;
		}
	}

	internal static Type TypeOfArray
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfArray == null)
			{
				typeOfArray = typeof(Array);
			}
			return typeOfArray;
		}
	}

	internal static Type TypeOfString
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfString == null)
			{
				typeOfString = typeof(string);
			}
			return typeOfString;
		}
	}

	internal static Type TypeOfInt
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfInt == null)
			{
				typeOfInt = typeof(int);
			}
			return typeOfInt;
		}
	}

	internal static Type TypeOfULong
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfULong == null)
			{
				typeOfULong = typeof(ulong);
			}
			return typeOfULong;
		}
	}

	internal static Type TypeOfVoid
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfVoid == null)
			{
				typeOfVoid = typeof(void);
			}
			return typeOfVoid;
		}
	}

	internal static Type TypeOfByteArray
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfByteArray == null)
			{
				typeOfByteArray = typeof(byte[]);
			}
			return typeOfByteArray;
		}
	}

	internal static Type TypeOfTimeSpan
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfTimeSpan == null)
			{
				typeOfTimeSpan = typeof(TimeSpan);
			}
			return typeOfTimeSpan;
		}
	}

	internal static Type TypeOfGuid
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfGuid == null)
			{
				typeOfGuid = typeof(Guid);
			}
			return typeOfGuid;
		}
	}

	internal static Type TypeOfDateTimeOffset
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDateTimeOffset == null)
			{
				typeOfDateTimeOffset = typeof(DateTimeOffset);
			}
			return typeOfDateTimeOffset;
		}
	}

	internal static Type TypeOfDateTimeOffsetAdapter
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDateTimeOffsetAdapter == null)
			{
				typeOfDateTimeOffsetAdapter = typeof(DateTimeOffsetAdapter);
			}
			return typeOfDateTimeOffsetAdapter;
		}
	}

	internal static Type TypeOfUri
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfUri == null)
			{
				typeOfUri = typeof(Uri);
			}
			return typeOfUri;
		}
	}

	internal static Type TypeOfTypeEnumerable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfTypeEnumerable == null)
			{
				typeOfTypeEnumerable = typeof(IEnumerable<Type>);
			}
			return typeOfTypeEnumerable;
		}
	}

	internal static Type TypeOfStreamingContext
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfStreamingContext == null)
			{
				typeOfStreamingContext = typeof(StreamingContext);
			}
			return typeOfStreamingContext;
		}
	}

	internal static Type TypeOfISerializable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfISerializable == null)
			{
				typeOfISerializable = typeof(ISerializable);
			}
			return typeOfISerializable;
		}
	}

	internal static Type TypeOfIDeserializationCallback
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIDeserializationCallback == null)
			{
				typeOfIDeserializationCallback = typeof(IDeserializationCallback);
			}
			return typeOfIDeserializationCallback;
		}
	}

	internal static Type TypeOfIObjectReference
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIObjectReference == null)
			{
				typeOfIObjectReference = typeof(IObjectReference);
			}
			return typeOfIObjectReference;
		}
	}

	internal static Type TypeOfXmlFormatClassWriterDelegate
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlFormatClassWriterDelegate == null)
			{
				typeOfXmlFormatClassWriterDelegate = typeof(XmlFormatClassWriterDelegate);
			}
			return typeOfXmlFormatClassWriterDelegate;
		}
	}

	internal static Type TypeOfXmlFormatCollectionWriterDelegate
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlFormatCollectionWriterDelegate == null)
			{
				typeOfXmlFormatCollectionWriterDelegate = typeof(XmlFormatCollectionWriterDelegate);
			}
			return typeOfXmlFormatCollectionWriterDelegate;
		}
	}

	internal static Type TypeOfXmlFormatClassReaderDelegate
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlFormatClassReaderDelegate == null)
			{
				typeOfXmlFormatClassReaderDelegate = typeof(XmlFormatClassReaderDelegate);
			}
			return typeOfXmlFormatClassReaderDelegate;
		}
	}

	internal static Type TypeOfXmlFormatCollectionReaderDelegate
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlFormatCollectionReaderDelegate == null)
			{
				typeOfXmlFormatCollectionReaderDelegate = typeof(XmlFormatCollectionReaderDelegate);
			}
			return typeOfXmlFormatCollectionReaderDelegate;
		}
	}

	internal static Type TypeOfXmlFormatGetOnlyCollectionReaderDelegate
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlFormatGetOnlyCollectionReaderDelegate == null)
			{
				typeOfXmlFormatGetOnlyCollectionReaderDelegate = typeof(XmlFormatGetOnlyCollectionReaderDelegate);
			}
			return typeOfXmlFormatGetOnlyCollectionReaderDelegate;
		}
	}

	internal static Type TypeOfKnownTypeAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfKnownTypeAttribute == null)
			{
				typeOfKnownTypeAttribute = typeof(KnownTypeAttribute);
			}
			return typeOfKnownTypeAttribute;
		}
	}

	internal static Type TypeOfDataContractAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDataContractAttribute == null)
			{
				typeOfDataContractAttribute = typeof(DataContractAttribute);
			}
			return typeOfDataContractAttribute;
		}
	}

	internal static Type TypeOfContractNamespaceAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfContractNamespaceAttribute == null)
			{
				typeOfContractNamespaceAttribute = typeof(ContractNamespaceAttribute);
			}
			return typeOfContractNamespaceAttribute;
		}
	}

	internal static Type TypeOfDataMemberAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDataMemberAttribute == null)
			{
				typeOfDataMemberAttribute = typeof(DataMemberAttribute);
			}
			return typeOfDataMemberAttribute;
		}
	}

	internal static Type TypeOfEnumMemberAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfEnumMemberAttribute == null)
			{
				typeOfEnumMemberAttribute = typeof(EnumMemberAttribute);
			}
			return typeOfEnumMemberAttribute;
		}
	}

	internal static Type TypeOfCollectionDataContractAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfCollectionDataContractAttribute == null)
			{
				typeOfCollectionDataContractAttribute = typeof(CollectionDataContractAttribute);
			}
			return typeOfCollectionDataContractAttribute;
		}
	}

	internal static Type TypeOfOptionalFieldAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfOptionalFieldAttribute == null)
			{
				typeOfOptionalFieldAttribute = typeof(OptionalFieldAttribute);
			}
			return typeOfOptionalFieldAttribute;
		}
	}

	internal static Type TypeOfObjectArray
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfObjectArray == null)
			{
				typeOfObjectArray = typeof(object[]);
			}
			return typeOfObjectArray;
		}
	}

	internal static Type TypeOfOnSerializingAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfOnSerializingAttribute == null)
			{
				typeOfOnSerializingAttribute = typeof(OnSerializingAttribute);
			}
			return typeOfOnSerializingAttribute;
		}
	}

	internal static Type TypeOfOnSerializedAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfOnSerializedAttribute == null)
			{
				typeOfOnSerializedAttribute = typeof(OnSerializedAttribute);
			}
			return typeOfOnSerializedAttribute;
		}
	}

	internal static Type TypeOfOnDeserializingAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfOnDeserializingAttribute == null)
			{
				typeOfOnDeserializingAttribute = typeof(OnDeserializingAttribute);
			}
			return typeOfOnDeserializingAttribute;
		}
	}

	internal static Type TypeOfOnDeserializedAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfOnDeserializedAttribute == null)
			{
				typeOfOnDeserializedAttribute = typeof(OnDeserializedAttribute);
			}
			return typeOfOnDeserializedAttribute;
		}
	}

	internal static Type TypeOfFlagsAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfFlagsAttribute == null)
			{
				typeOfFlagsAttribute = typeof(FlagsAttribute);
			}
			return typeOfFlagsAttribute;
		}
	}

	internal static Type TypeOfSerializableAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfSerializableAttribute == null)
			{
				typeOfSerializableAttribute = typeof(SerializableAttribute);
			}
			return typeOfSerializableAttribute;
		}
	}

	internal static Type TypeOfNonSerializedAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfNonSerializedAttribute == null)
			{
				typeOfNonSerializedAttribute = typeof(NonSerializedAttribute);
			}
			return typeOfNonSerializedAttribute;
		}
	}

	internal static Type TypeOfSerializationInfo
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfSerializationInfo == null)
			{
				typeOfSerializationInfo = typeof(SerializationInfo);
			}
			return typeOfSerializationInfo;
		}
	}

	internal static Type TypeOfSerializationInfoEnumerator
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfSerializationInfoEnumerator == null)
			{
				typeOfSerializationInfoEnumerator = typeof(SerializationInfoEnumerator);
			}
			return typeOfSerializationInfoEnumerator;
		}
	}

	internal static Type TypeOfSerializationEntry
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfSerializationEntry == null)
			{
				typeOfSerializationEntry = typeof(SerializationEntry);
			}
			return typeOfSerializationEntry;
		}
	}

	internal static Type TypeOfIXmlSerializable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIXmlSerializable == null)
			{
				typeOfIXmlSerializable = typeof(IXmlSerializable);
			}
			return typeOfIXmlSerializable;
		}
	}

	internal static Type TypeOfXmlSchemaProviderAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlSchemaProviderAttribute == null)
			{
				typeOfXmlSchemaProviderAttribute = typeof(XmlSchemaProviderAttribute);
			}
			return typeOfXmlSchemaProviderAttribute;
		}
	}

	internal static Type TypeOfXmlRootAttribute
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlRootAttribute == null)
			{
				typeOfXmlRootAttribute = typeof(XmlRootAttribute);
			}
			return typeOfXmlRootAttribute;
		}
	}

	internal static Type TypeOfXmlQualifiedName
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlQualifiedName == null)
			{
				typeOfXmlQualifiedName = typeof(XmlQualifiedName);
			}
			return typeOfXmlQualifiedName;
		}
	}

	internal static Type TypeOfXmlSchemaType
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlSchemaType == null)
			{
				typeOfXmlSchemaType = typeof(XmlSchemaType);
			}
			return typeOfXmlSchemaType;
		}
	}

	internal static Type TypeOfXmlSerializableServices
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlSerializableServices == null)
			{
				typeOfXmlSerializableServices = typeof(XmlSerializableServices);
			}
			return typeOfXmlSerializableServices;
		}
	}

	internal static Type TypeOfXmlNodeArray
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlNodeArray == null)
			{
				typeOfXmlNodeArray = typeof(XmlNode[]);
			}
			return typeOfXmlNodeArray;
		}
	}

	internal static Type TypeOfXmlSchemaSet
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlSchemaSet == null)
			{
				typeOfXmlSchemaSet = typeof(XmlSchemaSet);
			}
			return typeOfXmlSchemaSet;
		}
	}

	internal static object[] EmptyObjectArray
	{
		[SecuritySafeCritical]
		get
		{
			if (emptyObjectArray == null)
			{
				emptyObjectArray = new object[0];
			}
			return emptyObjectArray;
		}
	}

	internal static Type[] EmptyTypeArray
	{
		[SecuritySafeCritical]
		get
		{
			if (emptyTypeArray == null)
			{
				emptyTypeArray = new Type[0];
			}
			return emptyTypeArray;
		}
	}

	internal static Type TypeOfIPropertyChange
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIPropertyChange == null)
			{
				typeOfIPropertyChange = typeof(INotifyPropertyChanged);
			}
			return typeOfIPropertyChange;
		}
	}

	internal static Type TypeOfIExtensibleDataObject
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIExtensibleDataObject == null)
			{
				typeOfIExtensibleDataObject = typeof(IExtensibleDataObject);
			}
			return typeOfIExtensibleDataObject;
		}
	}

	internal static Type TypeOfExtensionDataObject
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfExtensionDataObject == null)
			{
				typeOfExtensionDataObject = typeof(ExtensionDataObject);
			}
			return typeOfExtensionDataObject;
		}
	}

	internal static Type TypeOfISerializableDataNode
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfISerializableDataNode == null)
			{
				typeOfISerializableDataNode = typeof(ISerializableDataNode);
			}
			return typeOfISerializableDataNode;
		}
	}

	internal static Type TypeOfClassDataNode
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfClassDataNode == null)
			{
				typeOfClassDataNode = typeof(ClassDataNode);
			}
			return typeOfClassDataNode;
		}
	}

	internal static Type TypeOfCollectionDataNode
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfCollectionDataNode == null)
			{
				typeOfCollectionDataNode = typeof(CollectionDataNode);
			}
			return typeOfCollectionDataNode;
		}
	}

	internal static Type TypeOfXmlDataNode
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlDataNode == null)
			{
				typeOfXmlDataNode = typeof(XmlDataNode);
			}
			return typeOfXmlDataNode;
		}
	}

	internal static Type TypeOfNullable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfNullable == null)
			{
				typeOfNullable = typeof(Nullable<>);
			}
			return typeOfNullable;
		}
	}

	internal static Type TypeOfReflectionPointer
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfReflectionPointer == null)
			{
				typeOfReflectionPointer = typeof(Pointer);
			}
			return typeOfReflectionPointer;
		}
	}

	internal static Type TypeOfIDictionaryGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIDictionaryGeneric == null)
			{
				typeOfIDictionaryGeneric = typeof(IDictionary<, >);
			}
			return typeOfIDictionaryGeneric;
		}
	}

	internal static Type TypeOfIDictionary
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIDictionary == null)
			{
				typeOfIDictionary = typeof(IDictionary);
			}
			return typeOfIDictionary;
		}
	}

	internal static Type TypeOfIListGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIListGeneric == null)
			{
				typeOfIListGeneric = typeof(IList<>);
			}
			return typeOfIListGeneric;
		}
	}

	internal static Type TypeOfIList
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIList == null)
			{
				typeOfIList = typeof(IList);
			}
			return typeOfIList;
		}
	}

	internal static Type TypeOfICollectionGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfICollectionGeneric == null)
			{
				typeOfICollectionGeneric = typeof(ICollection<>);
			}
			return typeOfICollectionGeneric;
		}
	}

	internal static Type TypeOfICollection
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfICollection == null)
			{
				typeOfICollection = typeof(ICollection);
			}
			return typeOfICollection;
		}
	}

	internal static Type TypeOfIEnumerableGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIEnumerableGeneric == null)
			{
				typeOfIEnumerableGeneric = typeof(IEnumerable<>);
			}
			return typeOfIEnumerableGeneric;
		}
	}

	internal static Type TypeOfIEnumerable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIEnumerable == null)
			{
				typeOfIEnumerable = typeof(IEnumerable);
			}
			return typeOfIEnumerable;
		}
	}

	internal static Type TypeOfIEnumeratorGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIEnumeratorGeneric == null)
			{
				typeOfIEnumeratorGeneric = typeof(IEnumerator<>);
			}
			return typeOfIEnumeratorGeneric;
		}
	}

	internal static Type TypeOfIEnumerator
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIEnumerator == null)
			{
				typeOfIEnumerator = typeof(IEnumerator);
			}
			return typeOfIEnumerator;
		}
	}

	internal static Type TypeOfKeyValuePair
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfKeyValuePair == null)
			{
				typeOfKeyValuePair = typeof(KeyValuePair<, >);
			}
			return typeOfKeyValuePair;
		}
	}

	internal static Type TypeOfKeyValue
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfKeyValue == null)
			{
				typeOfKeyValue = typeof(KeyValue<, >);
			}
			return typeOfKeyValue;
		}
	}

	internal static Type TypeOfIDictionaryEnumerator
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfIDictionaryEnumerator == null)
			{
				typeOfIDictionaryEnumerator = typeof(IDictionaryEnumerator);
			}
			return typeOfIDictionaryEnumerator;
		}
	}

	internal static Type TypeOfDictionaryEnumerator
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDictionaryEnumerator == null)
			{
				typeOfDictionaryEnumerator = typeof(CollectionDataContract.DictionaryEnumerator);
			}
			return typeOfDictionaryEnumerator;
		}
	}

	internal static Type TypeOfGenericDictionaryEnumerator
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfGenericDictionaryEnumerator == null)
			{
				typeOfGenericDictionaryEnumerator = typeof(CollectionDataContract.GenericDictionaryEnumerator<, >);
			}
			return typeOfGenericDictionaryEnumerator;
		}
	}

	internal static Type TypeOfDictionaryGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDictionaryGeneric == null)
			{
				typeOfDictionaryGeneric = typeof(Dictionary<, >);
			}
			return typeOfDictionaryGeneric;
		}
	}

	internal static Type TypeOfHashtable
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfHashtable == null)
			{
				typeOfHashtable = typeof(Hashtable);
			}
			return typeOfHashtable;
		}
	}

	internal static Type TypeOfListGeneric
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfListGeneric == null)
			{
				typeOfListGeneric = typeof(List<>);
			}
			return typeOfListGeneric;
		}
	}

	internal static Type TypeOfXmlElement
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfXmlElement == null)
			{
				typeOfXmlElement = typeof(XmlElement);
			}
			return typeOfXmlElement;
		}
	}

	internal static Type TypeOfDBNull
	{
		[SecuritySafeCritical]
		get
		{
			if (typeOfDBNull == null)
			{
				typeOfDBNull = typeof(DBNull);
			}
			return typeOfDBNull;
		}
	}

	internal static Uri DataContractXsdBaseNamespaceUri
	{
		[SecuritySafeCritical]
		get
		{
			if (dataContractXsdBaseNamespaceUri == null)
			{
				dataContractXsdBaseNamespaceUri = new Uri("http://schemas.datacontract.org/2004/07/");
			}
			return dataContractXsdBaseNamespaceUri;
		}
	}
}
