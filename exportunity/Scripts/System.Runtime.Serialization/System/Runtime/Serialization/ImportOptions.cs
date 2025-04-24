using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace System.Runtime.Serialization;

public class ImportOptions
{
	private bool generateSerializable;

	private bool generateInternal;

	private bool enableDataBinding;

	private CodeDomProvider codeProvider;

	private ICollection<Type> referencedTypes;

	private ICollection<Type> referencedCollectionTypes;

	private IDictionary<string, string> namespaces;

	private bool importXmlType;

	private IDataContractSurrogate dataContractSurrogate;

	public bool GenerateSerializable
	{
		get
		{
			return generateSerializable;
		}
		set
		{
			generateSerializable = value;
		}
	}

	public bool GenerateInternal
	{
		get
		{
			return generateInternal;
		}
		set
		{
			generateInternal = value;
		}
	}

	public bool EnableDataBinding
	{
		get
		{
			return enableDataBinding;
		}
		set
		{
			enableDataBinding = value;
		}
	}

	public CodeDomProvider CodeProvider
	{
		get
		{
			return codeProvider;
		}
		set
		{
			codeProvider = value;
		}
	}

	public ICollection<Type> ReferencedTypes
	{
		get
		{
			if (referencedTypes == null)
			{
				referencedTypes = new List<Type>();
			}
			return referencedTypes;
		}
	}

	public ICollection<Type> ReferencedCollectionTypes
	{
		get
		{
			if (referencedCollectionTypes == null)
			{
				referencedCollectionTypes = new List<Type>();
			}
			return referencedCollectionTypes;
		}
	}

	public IDictionary<string, string> Namespaces
	{
		get
		{
			if (namespaces == null)
			{
				namespaces = new Dictionary<string, string>();
			}
			return namespaces;
		}
	}

	public bool ImportXmlType
	{
		get
		{
			return importXmlType;
		}
		set
		{
			importXmlType = value;
		}
	}

	public IDataContractSurrogate DataContractSurrogate
	{
		get
		{
			return dataContractSurrogate;
		}
		set
		{
			dataContractSurrogate = value;
		}
	}
}
