using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

public class XsdDataContractImporter
{
	private ImportOptions options;

	private CodeCompileUnit codeCompileUnit;

	private DataContractSet dataContractSet;

	private static readonly XmlQualifiedName[] emptyTypeNameArray = new XmlQualifiedName[0];

	private static readonly XmlSchemaElement[] emptyElementArray = new XmlSchemaElement[0];

	private XmlQualifiedName[] singleTypeNameArray;

	private XmlSchemaElement[] singleElementArray;

	public ImportOptions Options
	{
		get
		{
			return options;
		}
		set
		{
			options = value;
		}
	}

	public CodeCompileUnit CodeCompileUnit => GetCodeCompileUnit();

	private DataContractSet DataContractSet
	{
		get
		{
			if (dataContractSet == null)
			{
				dataContractSet = ((Options == null) ? new DataContractSet(null, null, null) : new DataContractSet(Options.DataContractSurrogate, Options.ReferencedTypes, Options.ReferencedCollectionTypes));
			}
			return dataContractSet;
		}
	}

	private XmlQualifiedName[] SingleTypeNameArray
	{
		get
		{
			if (singleTypeNameArray == null)
			{
				singleTypeNameArray = new XmlQualifiedName[1];
			}
			return singleTypeNameArray;
		}
	}

	private XmlSchemaElement[] SingleElementArray
	{
		get
		{
			if (singleElementArray == null)
			{
				singleElementArray = new XmlSchemaElement[1];
			}
			return singleElementArray;
		}
	}

	private bool ImportXmlDataType
	{
		get
		{
			if (Options != null)
			{
				return Options.ImportXmlType;
			}
			return false;
		}
	}

	public XsdDataContractImporter()
	{
	}

	public XsdDataContractImporter(CodeCompileUnit codeCompileUnit)
	{
		this.codeCompileUnit = codeCompileUnit;
	}

	private CodeCompileUnit GetCodeCompileUnit()
	{
		if (codeCompileUnit == null)
		{
			codeCompileUnit = new CodeCompileUnit();
		}
		return codeCompileUnit;
	}

	public void Import(XmlSchemaSet schemas)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		InternalImport(schemas, null, null, null);
	}

	public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (typeNames == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
		}
		InternalImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
	}

	public void Import(XmlSchemaSet schemas, XmlQualifiedName typeName)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (typeName == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
		}
		SingleTypeNameArray[0] = typeName;
		InternalImport(schemas, SingleTypeNameArray, emptyElementArray, emptyTypeNameArray);
	}

	public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
		}
		SingleTypeNameArray[0] = null;
		SingleElementArray[0] = element;
		InternalImport(schemas, emptyTypeNameArray, SingleElementArray, SingleTypeNameArray);
		return SingleTypeNameArray[0];
	}

	public bool CanImport(XmlSchemaSet schemas)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		return InternalCanImport(schemas, null, null, null);
	}

	public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (typeNames == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
		}
		return InternalCanImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
	}

	public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName typeName)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (typeName == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
		}
		return InternalCanImport(schemas, new XmlQualifiedName[1] { typeName }, emptyElementArray, emptyTypeNameArray);
	}

	public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
	{
		if (schemas == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
		}
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
		}
		SingleTypeNameArray[0] = null;
		SingleElementArray[0] = element;
		return InternalCanImport(schemas, emptyTypeNameArray, SingleElementArray, SingleTypeNameArray);
	}

	public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
	{
		DataContract dataContract = FindDataContract(typeName);
		return new CodeExporter(DataContractSet, Options, GetCodeCompileUnit()).GetCodeTypeReference(dataContract);
	}

	public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
		}
		if (typeName == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
		}
		DataContract dataContract = FindDataContract(typeName);
		return new CodeExporter(DataContractSet, Options, GetCodeCompileUnit()).GetElementTypeReference(dataContract, element.IsNillable);
	}

	internal DataContract FindDataContract(XmlQualifiedName typeName)
	{
		if (typeName == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
		}
		DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
		if (dataContract == null)
		{
			dataContract = DataContractSet[typeName];
			if (dataContract == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("Type '{0}' in '{1}' namespace has not been imported.", typeName.Name, typeName.Namespace)));
			}
		}
		return dataContract;
	}

	public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
	{
		if (typeName == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
		}
		DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
		if (dataContract == null)
		{
			dataContract = DataContractSet[typeName];
			if (dataContract == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("Type '{0}' in '{1}' namespace has not been imported.", typeName.Name, typeName.Namespace)));
			}
		}
		return new CodeExporter(DataContractSet, Options, GetCodeCompileUnit()).GetKnownTypeReferences(dataContract);
	}

	[SecuritySafeCritical]
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	private void InternalImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
	{
		if (DiagnosticUtility.ShouldTraceInformation)
		{
			TraceUtility.Trace(TraceEventType.Information, 196618, SR.GetString("XSD import begins"));
		}
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			new SchemaImporter(schemas, typeNames, elements, elementTypeNames, DataContractSet, ImportXmlDataType).Import();
			new CodeExporter(DataContractSet, Options, GetCodeCompileUnit()).Export();
		}
		catch (Exception exception)
		{
			if (Fx.IsFatal(exception))
			{
				throw;
			}
			this.dataContractSet = dataContractSet;
			TraceImportError(exception);
			throw;
		}
		if (DiagnosticUtility.ShouldTraceInformation)
		{
			TraceUtility.Trace(TraceEventType.Information, 196619, SR.GetString("XSD import ends"));
		}
	}

	private void TraceImportError(Exception exception)
	{
		if (DiagnosticUtility.ShouldTraceError)
		{
			TraceUtility.Trace(TraceEventType.Error, 196621, SR.GetString("XSD import error"), null, exception);
		}
	}

	private bool InternalCanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
	{
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			new SchemaImporter(schemas, typeNames, elements, elementTypeNames, DataContractSet, ImportXmlDataType).Import();
			return true;
		}
		catch (InvalidDataContractException)
		{
			this.dataContractSet = dataContractSet;
			return false;
		}
		catch (Exception exception)
		{
			if (Fx.IsFatal(exception))
			{
				throw;
			}
			this.dataContractSet = dataContractSet;
			TraceImportError(exception);
			throw;
		}
	}
}
