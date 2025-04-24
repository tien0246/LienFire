using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

public class XsdDataContractExporter
{
	private ExportOptions options;

	private XmlSchemaSet schemas;

	private DataContractSet dataContractSet;

	public ExportOptions Options
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

	public XmlSchemaSet Schemas
	{
		get
		{
			XmlSchemaSet schemaSet = GetSchemaSet();
			SchemaImporter.CompileSchemaSet(schemaSet);
			return schemaSet;
		}
	}

	private DataContractSet DataContractSet
	{
		get
		{
			if (dataContractSet == null)
			{
				dataContractSet = new DataContractSet((Options == null) ? null : Options.GetSurrogate());
			}
			return dataContractSet;
		}
	}

	public XsdDataContractExporter()
	{
	}

	public XsdDataContractExporter(XmlSchemaSet schemas)
	{
		this.schemas = schemas;
	}

	private XmlSchemaSet GetSchemaSet()
	{
		if (schemas == null)
		{
			schemas = new XmlSchemaSet();
			schemas.XmlResolver = null;
		}
		return schemas;
	}

	private void TraceExportBegin()
	{
		if (DiagnosticUtility.ShouldTraceInformation)
		{
			TraceUtility.Trace(TraceEventType.Information, 196616, SR.GetString("XSD export begins"));
		}
	}

	private void TraceExportEnd()
	{
		if (DiagnosticUtility.ShouldTraceInformation)
		{
			TraceUtility.Trace(TraceEventType.Information, 196617, SR.GetString("XSD export ends"));
		}
	}

	private void TraceExportError(Exception exception)
	{
		if (DiagnosticUtility.ShouldTraceError)
		{
			TraceUtility.Trace(TraceEventType.Error, 196620, SR.GetString("XSD export error"), null, exception);
		}
	}

	public void Export(ICollection<Assembly> assemblies)
	{
		if (assemblies == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
		}
		TraceExportBegin();
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			foreach (Assembly assembly in assemblies)
			{
				if (assembly == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null assembly.", "assemblies")));
				}
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					CheckAndAddType(types[i]);
				}
			}
			Export();
		}
		catch (Exception exception)
		{
			if (Fx.IsFatal(exception))
			{
				throw;
			}
			this.dataContractSet = dataContractSet;
			TraceExportError(exception);
			throw;
		}
		TraceExportEnd();
	}

	public void Export(ICollection<Type> types)
	{
		if (types == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
		}
		TraceExportBegin();
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			foreach (Type type in types)
			{
				if (type == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null type.", "types")));
				}
				AddType(type);
			}
			Export();
		}
		catch (Exception exception)
		{
			if (Fx.IsFatal(exception))
			{
				throw;
			}
			this.dataContractSet = dataContractSet;
			TraceExportError(exception);
			throw;
		}
		TraceExportEnd();
	}

	public void Export(Type type)
	{
		if (type == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
		}
		TraceExportBegin();
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			AddType(type);
			Export();
		}
		catch (Exception exception)
		{
			if (Fx.IsFatal(exception))
			{
				throw;
			}
			this.dataContractSet = dataContractSet;
			TraceExportError(exception);
			throw;
		}
		TraceExportEnd();
	}

	public XmlQualifiedName GetSchemaTypeName(Type type)
	{
		if (type == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
		}
		type = GetSurrogatedType(type);
		DataContract dataContract = DataContract.GetDataContract(type);
		DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
		if (dataContract is XmlDataContract { IsAnonymous: not false })
		{
			return XmlQualifiedName.Empty;
		}
		return dataContract.StableName;
	}

	public XmlSchemaType GetSchemaType(Type type)
	{
		if (type == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
		}
		type = GetSurrogatedType(type);
		DataContract dataContract = DataContract.GetDataContract(type);
		DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
		if (dataContract is XmlDataContract { IsAnonymous: not false } xmlDataContract)
		{
			return xmlDataContract.XsdType;
		}
		return null;
	}

	public XmlQualifiedName GetRootElementName(Type type)
	{
		if (type == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
		}
		type = GetSurrogatedType(type);
		DataContract dataContract = DataContract.GetDataContract(type);
		DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
		if (dataContract.HasRoot)
		{
			return new XmlQualifiedName(dataContract.TopLevelElementName.Value, dataContract.TopLevelElementNamespace.Value);
		}
		return null;
	}

	private Type GetSurrogatedType(Type type)
	{
		IDataContractSurrogate surrogate;
		if (options != null && (surrogate = Options.GetSurrogate()) != null)
		{
			type = DataContractSurrogateCaller.GetDataContractType(surrogate, type);
		}
		return type;
	}

	private void CheckAndAddType(Type type)
	{
		type = GetSurrogatedType(type);
		if (!type.ContainsGenericParameters && DataContract.IsTypeSerializable(type))
		{
			AddType(type);
		}
	}

	private void AddType(Type type)
	{
		DataContractSet.Add(type);
	}

	private void Export()
	{
		AddKnownTypes();
		new SchemaExporter(GetSchemaSet(), DataContractSet).Export();
	}

	private void AddKnownTypes()
	{
		if (Options == null)
		{
			return;
		}
		Collection<Type> knownTypes = Options.KnownTypes;
		if (knownTypes == null)
		{
			return;
		}
		for (int i = 0; i < knownTypes.Count; i++)
		{
			Type type = knownTypes[i];
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null known type.")));
			}
			AddType(type);
		}
	}

	public bool CanExport(ICollection<Assembly> assemblies)
	{
		if (assemblies == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
		}
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			foreach (Assembly assembly in assemblies)
			{
				if (assembly == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null assembly.", "assemblies")));
				}
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					CheckAndAddType(types[i]);
				}
			}
			AddKnownTypes();
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
			TraceExportError(exception);
			throw;
		}
	}

	public bool CanExport(ICollection<Type> types)
	{
		if (types == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
		}
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			foreach (Type type in types)
			{
				if (type == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null type.", "types")));
				}
				AddType(type);
			}
			AddKnownTypes();
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
			TraceExportError(exception);
			throw;
		}
	}

	public bool CanExport(Type type)
	{
		if (type == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
		}
		DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
		try
		{
			AddType(type);
			AddKnownTypes();
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
			TraceExportError(exception);
			throw;
		}
	}
}
