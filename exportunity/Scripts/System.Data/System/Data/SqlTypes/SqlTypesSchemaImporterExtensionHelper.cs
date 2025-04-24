using System.CodeDom;
using System.CodeDom.Compiler;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace System.Data.SqlTypes;

public class SqlTypesSchemaImporterExtensionHelper : SchemaImporterExtension
{
	private string m_name;

	private string m_targetNamespace;

	private string[] m_references;

	private CodeNamespaceImport[] m_namespaceImports;

	private string m_destinationType;

	private bool m_direct;

	protected static readonly string SqlTypesNamespace = "http://schemas.microsoft.com/sqlserver/2004/sqltypes";

	public SqlTypesSchemaImporterExtensionHelper(string name, string targetNamespace, string[] references, CodeNamespaceImport[] namespaceImports, string destinationType, bool direct)
	{
		Init(name, targetNamespace, references, namespaceImports, destinationType, direct);
	}

	public SqlTypesSchemaImporterExtensionHelper(string name, string destinationType)
	{
		Init(name, SqlTypesNamespace, null, null, destinationType, direct: true);
	}

	public SqlTypesSchemaImporterExtensionHelper(string name, string destinationType, bool direct)
	{
		Init(name, SqlTypesNamespace, null, null, destinationType, direct);
	}

	private void Init(string name, string targetNamespace, string[] references, CodeNamespaceImport[] namespaceImports, string destinationType, bool direct)
	{
		m_name = name;
		m_targetNamespace = targetNamespace;
		if (references == null)
		{
			m_references = new string[1];
			m_references[0] = "System.Data.dll";
		}
		else
		{
			m_references = references;
		}
		if (namespaceImports == null)
		{
			m_namespaceImports = new CodeNamespaceImport[2];
			m_namespaceImports[0] = new CodeNamespaceImport("System.Data");
			m_namespaceImports[1] = new CodeNamespaceImport("System.Data.SqlTypes");
		}
		else
		{
			m_namespaceImports = namespaceImports;
		}
		m_destinationType = destinationType;
		m_direct = direct;
	}

	public override string ImportSchemaType(string name, string xmlNamespace, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
	{
		if (m_direct && context is XmlSchemaElement && string.CompareOrdinal(m_name, name) == 0 && string.CompareOrdinal(m_targetNamespace, xmlNamespace) == 0)
		{
			compileUnit.ReferencedAssemblies.AddRange(m_references);
			mainNamespace.Imports.AddRange(m_namespaceImports);
			return m_destinationType;
		}
		return null;
	}

	public override string ImportSchemaType(XmlSchemaType type, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
	{
		if (!m_direct && type is XmlSchemaSimpleType && context is XmlSchemaElement)
		{
			XmlQualifiedName qualifiedName = ((XmlSchemaSimpleType)type).BaseXmlSchemaType.QualifiedName;
			if (string.CompareOrdinal(m_name, qualifiedName.Name) == 0 && string.CompareOrdinal(m_targetNamespace, qualifiedName.Namespace) == 0)
			{
				compileUnit.ReferencedAssemblies.AddRange(m_references);
				mainNamespace.Imports.AddRange(m_namespaceImports);
				return m_destinationType;
			}
		}
		return null;
	}
}
