using System.CodeDom.Compiler;
using System.Security.Permissions;
using System.Xml.Schema;

namespace System.Xml.Serialization;

public class SoapSchemaImporter : SchemaImporter
{
	public SoapSchemaImporter(XmlSchemas schemas)
		: base(schemas, CodeGenerationOptions.GenerateProperties, null, new ImportContext())
	{
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers)
		: base(schemas, CodeGenerationOptions.GenerateProperties, null, new ImportContext(typeIdentifiers, shareTypes: false))
	{
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers, CodeGenerationOptions options)
		: base(schemas, options, null, new ImportContext(typeIdentifiers, shareTypes: false))
	{
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, ImportContext context)
		: base(schemas, options, null, context)
	{
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, CodeDomProvider codeProvider, ImportContext context)
		: base(schemas, options, codeProvider, context)
	{
	}

	public XmlTypeMapping ImportDerivedTypeMapping(XmlQualifiedName name, Type baseType, bool baseTypeCanBeIndirect)
	{
		TypeMapping typeMapping = ImportType(name, excludeFromImport: false);
		if (typeMapping is StructMapping)
		{
			MakeDerived((StructMapping)typeMapping, baseType, baseTypeCanBeIndirect);
		}
		else if (baseType != null)
		{
			throw new InvalidOperationException(Res.GetString("Type '{0}' from namespace '{1}' is not a complex type and cannot be used as a {2}.", name.Name, name.Namespace, baseType.FullName));
		}
		ElementAccessor elementAccessor = new ElementAccessor();
		elementAccessor.IsSoap = true;
		elementAccessor.Name = name.Name;
		elementAccessor.Namespace = name.Namespace;
		elementAccessor.Mapping = typeMapping;
		elementAccessor.IsNullable = true;
		elementAccessor.Form = XmlSchemaForm.Qualified;
		return new XmlTypeMapping(base.Scope, elementAccessor);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember member)
	{
		TypeMapping typeMapping = ImportType(member.MemberType, excludeFromImport: true);
		if (!(typeMapping is StructMapping))
		{
			return ImportMembersMapping(name, ns, new SoapSchemaMember[1] { member });
		}
		MembersMapping membersMapping = new MembersMapping();
		membersMapping.TypeDesc = base.Scope.GetTypeDesc(typeof(object[]));
		membersMapping.Members = ((StructMapping)typeMapping).Members;
		membersMapping.HasWrapperElement = true;
		ElementAccessor elementAccessor = new ElementAccessor();
		elementAccessor.IsSoap = true;
		elementAccessor.Name = name;
		elementAccessor.Namespace = ((typeMapping.Namespace != null) ? typeMapping.Namespace : ns);
		elementAccessor.Mapping = membersMapping;
		elementAccessor.IsNullable = false;
		elementAccessor.Form = XmlSchemaForm.Qualified;
		return new XmlMembersMapping(base.Scope, elementAccessor, XmlMappingAccess.Read | XmlMappingAccess.Write);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members)
	{
		return ImportMembersMapping(name, ns, members, hasWrapperElement: true);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement)
	{
		return ImportMembersMapping(name, ns, members, hasWrapperElement, null, baseTypeCanBeIndirect: false);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement, Type baseType, bool baseTypeCanBeIndirect)
	{
		XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
		XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)(xmlSchemaComplexType.Particle = new XmlSchemaSequence());
		foreach (SoapSchemaMember soapSchemaMember in members)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = soapSchemaMember.MemberName;
			xmlSchemaElement.SchemaTypeName = soapSchemaMember.MemberType;
			xmlSchemaSequence.Items.Add(xmlSchemaElement);
		}
		CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
		codeIdentifiers.UseCamelCasing = true;
		MembersMapping membersMapping = new MembersMapping();
		membersMapping.TypeDesc = base.Scope.GetTypeDesc(typeof(object[]));
		membersMapping.Members = ImportTypeMembers(xmlSchemaComplexType, ns, codeIdentifiers);
		membersMapping.HasWrapperElement = hasWrapperElement;
		if (baseType != null)
		{
			for (int j = 0; j < membersMapping.Members.Length; j++)
			{
				MemberMapping memberMapping = membersMapping.Members[j];
				if (memberMapping.Accessor.Mapping is StructMapping)
				{
					MakeDerived((StructMapping)memberMapping.Accessor.Mapping, baseType, baseTypeCanBeIndirect);
				}
			}
		}
		ElementAccessor elementAccessor = new ElementAccessor();
		elementAccessor.IsSoap = true;
		elementAccessor.Name = name;
		elementAccessor.Namespace = ns;
		elementAccessor.Mapping = membersMapping;
		elementAccessor.IsNullable = false;
		elementAccessor.Form = XmlSchemaForm.Qualified;
		return new XmlMembersMapping(base.Scope, elementAccessor, XmlMappingAccess.Read | XmlMappingAccess.Write);
	}

	private ElementAccessor ImportElement(XmlSchemaElement element, string ns)
	{
		if (!element.RefName.IsEmpty)
		{
			throw new InvalidOperationException(Res.GetString("Element reference syntax not supported with encoded SOAP. Replace element reference '{0}' from namespace '{1}' with a local element declaration.", element.RefName.Name, element.RefName.Namespace));
		}
		if (element.Name.Length == 0)
		{
			XmlQualifiedName parentName = XmlSchemas.GetParentName(element);
			throw new InvalidOperationException(Res.GetString("This element has no name. Please review schema type '{0}' from namespace '{1}'.", parentName.Name, parentName.Namespace));
		}
		TypeMapping mapping = ImportElementType(element, ns);
		return new ElementAccessor
		{
			IsSoap = true,
			Name = element.Name,
			Namespace = ns,
			Mapping = mapping,
			IsNullable = element.IsNillable,
			Form = XmlSchemaForm.None
		};
	}

	private TypeMapping ImportElementType(XmlSchemaElement element, string ns)
	{
		TypeMapping typeMapping;
		if (!element.SchemaTypeName.IsEmpty)
		{
			typeMapping = ImportType(element.SchemaTypeName, excludeFromImport: false);
		}
		else
		{
			if (element.SchemaType == null)
			{
				if (!element.SubstitutionGroup.IsEmpty)
				{
					XmlQualifiedName parentName = XmlSchemas.GetParentName(element);
					throw new InvalidOperationException(Res.GetString("Substitution group may not be used with encoded SOAP. Please review type declaration '{0}' from namespace '{1}'.", parentName.Name, parentName.Namespace));
				}
				XmlQualifiedName parentName2 = XmlSchemas.GetParentName(element);
				throw new InvalidOperationException(Res.GetString("Please review type declaration '{0}' from namespace '{1}': element '{2}' does not specify a type.", parentName2.Name, parentName2.Namespace, element.Name));
			}
			XmlQualifiedName parentName3 = XmlSchemas.GetParentName(element);
			if (!(element.SchemaType is XmlSchemaComplexType))
			{
				throw new InvalidOperationException(Res.GetString("Types must be declared at the top level in the schema. Please review schema type '{0}' from namespace '{1}': element '{2}' is using anonymous type declaration, anonymous types are not supported with encoded SOAP.", parentName3.Name, parentName3.Namespace, element.Name));
			}
			typeMapping = ImportType((XmlSchemaComplexType)element.SchemaType, ns, excludeFromImport: false);
			if (!(typeMapping is ArrayMapping))
			{
				throw new InvalidOperationException(Res.GetString("Types must be declared at the top level in the schema. Please review schema type '{0}' from namespace '{1}': element '{2}' is using anonymous type declaration, anonymous types are not supported with encoded SOAP.", parentName3.Name, parentName3.Namespace, element.Name));
			}
		}
		typeMapping.ReferencedByElement = true;
		return typeMapping;
	}

	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	internal override void ImportDerivedTypes(XmlQualifiedName baseName)
	{
		foreach (XmlSchema schema in base.Schemas)
		{
			if (base.Schemas.IsReference(schema) || XmlSchemas.IsDataSet(schema))
			{
				continue;
			}
			XmlSchemas.Preprocess(schema);
			foreach (object value in schema.SchemaTypes.Values)
			{
				if (value is XmlSchemaType)
				{
					XmlSchemaType xmlSchemaType = (XmlSchemaType)value;
					if (xmlSchemaType.DerivedFrom == baseName)
					{
						ImportType(xmlSchemaType.QualifiedName, excludeFromImport: false);
					}
				}
			}
		}
	}

	private TypeMapping ImportType(XmlQualifiedName name, bool excludeFromImport)
	{
		if (name.Name == "anyType" && name.Namespace == "http://www.w3.org/2001/XMLSchema")
		{
			return ImportRootMapping();
		}
		object obj = FindType(name);
		TypeMapping typeMapping = (TypeMapping)base.ImportedMappings[obj];
		if (typeMapping == null)
		{
			if (obj is XmlSchemaComplexType)
			{
				typeMapping = ImportType((XmlSchemaComplexType)obj, name.Namespace, excludeFromImport);
			}
			else
			{
				if (!(obj is XmlSchemaSimpleType))
				{
					throw new InvalidOperationException(Res.GetString("Internal error."));
				}
				typeMapping = ImportDataType((XmlSchemaSimpleType)obj, name.Namespace, name.Name, isList: false);
			}
		}
		if (excludeFromImport)
		{
			typeMapping.IncludeInSchema = false;
		}
		return typeMapping;
	}

	private TypeMapping ImportType(XmlSchemaComplexType type, string typeNs, bool excludeFromImport)
	{
		if (type.Redefined != null)
		{
			throw new NotSupportedException(Res.GetString("Cannot import schema for type '{0}' from namespace '{1}'. Redefine not supported.", type.Name, typeNs));
		}
		TypeMapping typeMapping = ImportAnyType(type, typeNs);
		if (typeMapping == null)
		{
			typeMapping = ImportArrayMapping(type, typeNs);
		}
		if (typeMapping == null)
		{
			typeMapping = ImportStructType(type, typeNs, excludeFromImport);
		}
		return typeMapping;
	}

	private TypeMapping ImportAnyType(XmlSchemaComplexType type, string typeNs)
	{
		if (type.Particle == null)
		{
			return null;
		}
		if (!(type.Particle is XmlSchemaAll) && !(type.Particle is XmlSchemaSequence))
		{
			return null;
		}
		XmlSchemaGroupBase xmlSchemaGroupBase = (XmlSchemaGroupBase)type.Particle;
		if (xmlSchemaGroupBase.Items.Count != 1 || !(xmlSchemaGroupBase.Items[0] is XmlSchemaAny))
		{
			return null;
		}
		return ImportRootMapping();
	}

	private StructMapping ImportStructType(XmlSchemaComplexType type, string typeNs, bool excludeFromImport)
	{
		if (type.Name == null)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)type.Parent;
			XmlQualifiedName parentName = XmlSchemas.GetParentName(xmlSchemaElement);
			throw new InvalidOperationException(Res.GetString("Types must be declared at the top level in the schema. Please review schema type '{0}' from namespace '{1}': element '{2}' is using anonymous type declaration, anonymous types are not supported with encoded SOAP.", parentName.Name, parentName.Namespace, xmlSchemaElement.Name));
		}
		TypeDesc baseTypeDesc = null;
		Mapping mapping = null;
		if (!type.DerivedFrom.IsEmpty)
		{
			mapping = ImportType(type.DerivedFrom, excludeFromImport);
			if (mapping is StructMapping)
			{
				baseTypeDesc = ((StructMapping)mapping).TypeDesc;
			}
			else
			{
				mapping = null;
			}
		}
		if (mapping == null)
		{
			mapping = GetRootMapping();
		}
		Mapping mapping2 = (Mapping)base.ImportedMappings[type];
		if (mapping2 != null)
		{
			return (StructMapping)mapping2;
		}
		string text = GenerateUniqueTypeName(Accessor.UnescapeName(type.Name));
		StructMapping structMapping = new StructMapping();
		structMapping.IsReference = base.Schemas.IsReference(type);
		TypeFlags typeFlags = TypeFlags.Reference;
		if (type.IsAbstract)
		{
			typeFlags |= TypeFlags.Abstract;
		}
		structMapping.TypeDesc = new TypeDesc(text, text, TypeKind.Struct, baseTypeDesc, typeFlags);
		structMapping.Namespace = typeNs;
		structMapping.TypeName = type.Name;
		structMapping.BaseMapping = (StructMapping)mapping;
		base.ImportedMappings.Add(type, structMapping);
		if (excludeFromImport)
		{
			structMapping.IncludeInSchema = false;
		}
		CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
		codeIdentifiers.AddReserved(text);
		AddReservedIdentifiersForDataBinding(codeIdentifiers);
		structMapping.Members = ImportTypeMembers(type, typeNs, codeIdentifiers);
		base.Scope.AddTypeMapping(structMapping);
		ImportDerivedTypes(new XmlQualifiedName(type.Name, typeNs));
		return structMapping;
	}

	private MemberMapping[] ImportTypeMembers(XmlSchemaComplexType type, string typeNs, CodeIdentifiers members)
	{
		if (type.AnyAttribute != null)
		{
			throw new InvalidOperationException(Res.GetString("Any may not be specified. Attributes are not supported with encoded SOAP. Please review schema type '{0}' from namespace '{1}'.", type.Name, type.QualifiedName.Namespace));
		}
		XmlSchemaObjectCollection attributes = type.Attributes;
		for (int i = 0; i < attributes.Count; i++)
		{
			object obj = attributes[i];
			if (obj is XmlSchemaAttributeGroup)
			{
				throw new InvalidOperationException(Res.GetString("Attributes are not supported with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}': use elements (not attributes) for fields/parameters.", type.Name, type.QualifiedName.Namespace));
			}
			if (obj is XmlSchemaAttribute && ((XmlSchemaAttribute)obj).Use != XmlSchemaUse.Prohibited)
			{
				throw new InvalidOperationException(Res.GetString("Attributes are not supported with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}': use elements (not attributes) for fields/parameters.", type.Name, type.QualifiedName.Namespace));
			}
		}
		if (type.Particle != null)
		{
			ImportGroup(type.Particle, members, typeNs);
		}
		else if (type.ContentModel != null && type.ContentModel is XmlSchemaComplexContent)
		{
			XmlSchemaComplexContent xmlSchemaComplexContent = (XmlSchemaComplexContent)type.ContentModel;
			if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
			{
				if (((XmlSchemaComplexContentExtension)xmlSchemaComplexContent.Content).Particle != null)
				{
					ImportGroup(((XmlSchemaComplexContentExtension)xmlSchemaComplexContent.Content).Particle, members, typeNs);
				}
			}
			else if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentRestriction && ((XmlSchemaComplexContentRestriction)xmlSchemaComplexContent.Content).Particle != null)
			{
				ImportGroup(((XmlSchemaComplexContentRestriction)xmlSchemaComplexContent.Content).Particle, members, typeNs);
			}
		}
		return (MemberMapping[])members.ToArray(typeof(MemberMapping));
	}

	private void ImportGroup(XmlSchemaParticle group, CodeIdentifiers members, string ns)
	{
		if (group is XmlSchemaChoice)
		{
			XmlQualifiedName parentName = XmlSchemas.GetParentName(group);
			throw new InvalidOperationException(Res.GetString("Choice is not supported with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}': use all or sequence (not choice) for fields/parameters.", parentName.Name, parentName.Namespace));
		}
		ImportGroupMembers(group, members, ns);
	}

	private void ImportGroupMembers(XmlSchemaParticle particle, CodeIdentifiers members, string ns)
	{
		XmlQualifiedName parentName = XmlSchemas.GetParentName(particle);
		if (particle is XmlSchemaGroupRef)
		{
			throw new InvalidOperationException(Res.GetString("The ref syntax for groups is not supported with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}': replace the group reference with local group declaration.", parentName.Name, parentName.Namespace));
		}
		if (!(particle is XmlSchemaGroupBase))
		{
			return;
		}
		XmlSchemaGroupBase xmlSchemaGroupBase = (XmlSchemaGroupBase)particle;
		if (xmlSchemaGroupBase.IsMultipleOccurrence)
		{
			throw new InvalidOperationException(Res.GetString("Group may not repeat.  Unbounded groups are not supported with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}'.", parentName.Name, parentName.Namespace));
		}
		for (int i = 0; i < xmlSchemaGroupBase.Items.Count; i++)
		{
			object obj = xmlSchemaGroupBase.Items[i];
			if (obj is XmlSchemaGroupBase || obj is XmlSchemaGroupRef)
			{
				throw new InvalidOperationException(Res.GetString("Nested groups may not be used with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}'.", parentName.Name, parentName.Namespace));
			}
			if (obj is XmlSchemaElement)
			{
				ImportElementMember((XmlSchemaElement)obj, members, ns);
			}
			else if (obj is XmlSchemaAny)
			{
				throw new InvalidOperationException(Res.GetString("Any may not be used with encoded SOAP. Please change definition of schema type '{0}' from namespace '{1}'.", parentName.Name, parentName.Namespace));
			}
		}
	}

	private ElementAccessor ImportArray(XmlSchemaElement element, string ns)
	{
		if (element.SchemaType == null)
		{
			return null;
		}
		if (!element.IsMultipleOccurrence)
		{
			return null;
		}
		XmlSchemaType schemaType = element.SchemaType;
		ArrayMapping arrayMapping = ImportArrayMapping(schemaType, ns);
		if (arrayMapping == null)
		{
			return null;
		}
		return new ElementAccessor
		{
			IsSoap = true,
			Name = element.Name,
			Namespace = ns,
			Mapping = arrayMapping,
			IsNullable = false,
			Form = XmlSchemaForm.None
		};
	}

	private ArrayMapping ImportArrayMapping(XmlSchemaType type, string ns)
	{
		ArrayMapping arrayMapping;
		if (type.Name == "Array" && ns == "http://schemas.xmlsoap.org/soap/encoding/")
		{
			arrayMapping = new ArrayMapping();
			TypeMapping rootMapping = GetRootMapping();
			ElementAccessor elementAccessor = new ElementAccessor();
			elementAccessor.IsSoap = true;
			elementAccessor.Name = "anyType";
			elementAccessor.Namespace = ns;
			elementAccessor.Mapping = rootMapping;
			elementAccessor.IsNullable = true;
			elementAccessor.Form = XmlSchemaForm.None;
			arrayMapping.Elements = new ElementAccessor[1] { elementAccessor };
			arrayMapping.TypeDesc = elementAccessor.Mapping.TypeDesc.CreateArrayTypeDesc();
			arrayMapping.TypeName = "ArrayOf" + CodeIdentifier.MakePascal(elementAccessor.Mapping.TypeName);
			return arrayMapping;
		}
		if (!(type.DerivedFrom.Name == "Array") || !(type.DerivedFrom.Namespace == "http://schemas.xmlsoap.org/soap/encoding/"))
		{
			return null;
		}
		XmlSchemaContentModel contentModel = ((XmlSchemaComplexType)type).ContentModel;
		if (!(contentModel.Content is XmlSchemaComplexContentRestriction))
		{
			return null;
		}
		arrayMapping = new ArrayMapping();
		XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)contentModel.Content;
		for (int i = 0; i < xmlSchemaComplexContentRestriction.Attributes.Count; i++)
		{
			if (!(xmlSchemaComplexContentRestriction.Attributes[i] is XmlSchemaAttribute xmlSchemaAttribute) || !(xmlSchemaAttribute.RefName.Name == "arrayType") || !(xmlSchemaAttribute.RefName.Namespace == "http://schemas.xmlsoap.org/soap/encoding/"))
			{
				continue;
			}
			string text = null;
			if (xmlSchemaAttribute.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = xmlSchemaAttribute.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (xmlAttribute.LocalName == "arrayType" && xmlAttribute.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/")
					{
						text = xmlAttribute.Value;
						break;
					}
				}
			}
			if (text != null)
			{
				string dims;
				XmlQualifiedName xmlQualifiedName = TypeScope.ParseWsdlArrayType(text, out dims, xmlSchemaAttribute);
				TypeDesc typeDesc = base.Scope.GetTypeDesc(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
				TypeMapping typeMapping;
				if (typeDesc != null && typeDesc.IsPrimitive)
				{
					typeMapping = new PrimitiveMapping();
					typeMapping.TypeDesc = typeDesc;
					typeMapping.TypeName = typeDesc.DataType.Name;
				}
				else
				{
					typeMapping = ImportType(xmlQualifiedName, excludeFromImport: false);
				}
				ElementAccessor elementAccessor2 = new ElementAccessor();
				elementAccessor2.IsSoap = true;
				elementAccessor2.Name = xmlQualifiedName.Name;
				elementAccessor2.Namespace = ns;
				elementAccessor2.Mapping = typeMapping;
				elementAccessor2.IsNullable = true;
				elementAccessor2.Form = XmlSchemaForm.None;
				arrayMapping.Elements = new ElementAccessor[1] { elementAccessor2 };
				arrayMapping.TypeDesc = elementAccessor2.Mapping.TypeDesc.CreateArrayTypeDesc();
				arrayMapping.TypeName = "ArrayOf" + CodeIdentifier.MakePascal(elementAccessor2.Mapping.TypeName);
				return arrayMapping;
			}
		}
		XmlSchemaParticle particle = xmlSchemaComplexContentRestriction.Particle;
		if (particle is XmlSchemaAll || particle is XmlSchemaSequence)
		{
			XmlSchemaGroupBase xmlSchemaGroupBase = (XmlSchemaGroupBase)particle;
			if (xmlSchemaGroupBase.Items.Count != 1 || !(xmlSchemaGroupBase.Items[0] is XmlSchemaElement))
			{
				return null;
			}
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaGroupBase.Items[0];
			if (!xmlSchemaElement.IsMultipleOccurrence)
			{
				return null;
			}
			ElementAccessor elementAccessor3 = ImportElement(xmlSchemaElement, ns);
			arrayMapping.Elements = new ElementAccessor[1] { elementAccessor3 };
			arrayMapping.TypeDesc = elementAccessor3.Mapping.TypeDesc.CreateArrayTypeDesc();
			return arrayMapping;
		}
		return null;
	}

	private void ImportElementMember(XmlSchemaElement element, CodeIdentifiers members, string ns)
	{
		ElementAccessor elementAccessor;
		if ((elementAccessor = ImportArray(element, ns)) == null)
		{
			elementAccessor = ImportElement(element, ns);
		}
		MemberMapping memberMapping = new MemberMapping();
		memberMapping.Name = CodeIdentifier.MakeValid(Accessor.UnescapeName(elementAccessor.Name));
		memberMapping.Name = members.AddUnique(memberMapping.Name, memberMapping);
		if (memberMapping.Name.EndsWith("Specified", StringComparison.Ordinal))
		{
			string name = memberMapping.Name;
			memberMapping.Name = members.AddUnique(memberMapping.Name, memberMapping);
			members.Remove(name);
		}
		memberMapping.TypeDesc = elementAccessor.Mapping.TypeDesc;
		memberMapping.Elements = new ElementAccessor[1] { elementAccessor };
		if (element.IsMultipleOccurrence)
		{
			memberMapping.TypeDesc = memberMapping.TypeDesc.CreateArrayTypeDesc();
		}
		if (element.MinOccurs == 0m && memberMapping.TypeDesc.IsValueType && !memberMapping.TypeDesc.HasIsEmpty)
		{
			memberMapping.CheckSpecified = SpecifiedAccessor.ReadWrite;
		}
	}

	private TypeMapping ImportDataType(XmlSchemaSimpleType dataType, string typeNs, string identifier, bool isList)
	{
		TypeMapping typeMapping = ImportNonXsdPrimitiveDataType(dataType, typeNs);
		if (typeMapping != null)
		{
			return typeMapping;
		}
		if (dataType.Content is XmlSchemaSimpleTypeRestriction)
		{
			foreach (XmlSchemaObject facet in ((XmlSchemaSimpleTypeRestriction)dataType.Content).Facets)
			{
				if (facet is XmlSchemaEnumerationFacet)
				{
					return ImportEnumeratedDataType(dataType, typeNs, identifier, isList);
				}
			}
		}
		else if (dataType.Content is XmlSchemaSimpleTypeList || dataType.Content is XmlSchemaSimpleTypeUnion)
		{
			if (dataType.Content is XmlSchemaSimpleTypeList)
			{
				XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)dataType.Content;
				if (xmlSchemaSimpleTypeList.ItemType != null)
				{
					typeMapping = ImportDataType(xmlSchemaSimpleTypeList.ItemType, typeNs, identifier, isList: true);
					if (typeMapping != null)
					{
						return typeMapping;
					}
				}
			}
			typeMapping = new PrimitiveMapping();
			typeMapping.TypeDesc = base.Scope.GetTypeDesc(typeof(string));
			typeMapping.TypeName = typeMapping.TypeDesc.DataType.Name;
			return typeMapping;
		}
		return ImportPrimitiveDataType(dataType);
	}

	private TypeMapping ImportEnumeratedDataType(XmlSchemaSimpleType dataType, string typeNs, string identifier, bool isList)
	{
		TypeMapping typeMapping = (TypeMapping)base.ImportedMappings[dataType];
		if (typeMapping != null)
		{
			return typeMapping;
		}
		XmlSchemaSimpleType dataType2 = FindDataType(dataType.DerivedFrom);
		TypeDesc typeDesc = base.Scope.GetTypeDesc(dataType2);
		if (typeDesc != null && typeDesc != base.Scope.GetTypeDesc(typeof(string)))
		{
			return ImportPrimitiveDataType(dataType);
		}
		identifier = Accessor.UnescapeName(identifier);
		string text = GenerateUniqueTypeName(identifier);
		EnumMapping enumMapping = new EnumMapping();
		enumMapping.IsReference = base.Schemas.IsReference(dataType);
		enumMapping.TypeDesc = new TypeDesc(text, text, TypeKind.Enum, null, TypeFlags.None);
		enumMapping.TypeName = identifier;
		enumMapping.Namespace = typeNs;
		enumMapping.IsFlags = isList;
		CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
		if (!(dataType.Content is XmlSchemaSimpleTypeRestriction))
		{
			throw new InvalidOperationException(Res.GetString("Invalid content '{0}' for enumerated data type {1}.", dataType.Content.GetType().Name, identifier));
		}
		XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)dataType.Content;
		for (int i = 0; i < xmlSchemaSimpleTypeRestriction.Facets.Count; i++)
		{
			object obj = xmlSchemaSimpleTypeRestriction.Facets[i];
			if (obj is XmlSchemaEnumerationFacet)
			{
				XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = (XmlSchemaEnumerationFacet)obj;
				ConstantMapping constantMapping = new ConstantMapping();
				string identifier2 = CodeIdentifier.MakeValid(xmlSchemaEnumerationFacet.Value);
				constantMapping.Name = codeIdentifiers.AddUnique(identifier2, constantMapping);
				constantMapping.XmlName = xmlSchemaEnumerationFacet.Value;
				constantMapping.Value = i;
			}
		}
		enumMapping.Constants = (ConstantMapping[])codeIdentifiers.ToArray(typeof(ConstantMapping));
		if (isList && enumMapping.Constants.Length > 63)
		{
			typeMapping = new PrimitiveMapping();
			typeMapping.TypeDesc = base.Scope.GetTypeDesc(typeof(string));
			typeMapping.TypeName = typeMapping.TypeDesc.DataType.Name;
			base.ImportedMappings.Add(dataType, typeMapping);
			return typeMapping;
		}
		base.ImportedMappings.Add(dataType, enumMapping);
		base.Scope.AddTypeMapping(enumMapping);
		return enumMapping;
	}

	private PrimitiveMapping ImportPrimitiveDataType(XmlSchemaSimpleType dataType)
	{
		TypeDesc dataTypeSource = GetDataTypeSource(dataType);
		return new PrimitiveMapping
		{
			TypeDesc = dataTypeSource,
			TypeName = dataTypeSource.DataType.Name
		};
	}

	private PrimitiveMapping ImportNonXsdPrimitiveDataType(XmlSchemaSimpleType dataType, string ns)
	{
		PrimitiveMapping primitiveMapping = null;
		TypeDesc typeDesc = null;
		if (dataType.Name != null && dataType.Name.Length != 0)
		{
			typeDesc = base.Scope.GetTypeDesc(dataType.Name, ns);
			if (typeDesc != null)
			{
				primitiveMapping = new PrimitiveMapping();
				primitiveMapping.TypeDesc = typeDesc;
				primitiveMapping.TypeName = typeDesc.DataType.Name;
			}
		}
		return primitiveMapping;
	}

	private TypeDesc GetDataTypeSource(XmlSchemaSimpleType dataType)
	{
		if (dataType.Name != null && dataType.Name.Length != 0)
		{
			TypeDesc typeDesc = base.Scope.GetTypeDesc(dataType);
			if (typeDesc != null)
			{
				return typeDesc;
			}
		}
		if (!dataType.DerivedFrom.IsEmpty)
		{
			return GetDataTypeSource(FindDataType(dataType.DerivedFrom));
		}
		return base.Scope.GetTypeDesc(typeof(string));
	}

	private XmlSchemaSimpleType FindDataType(XmlQualifiedName name)
	{
		TypeDesc typeDesc = base.Scope.GetTypeDesc(name.Name, name.Namespace);
		if (typeDesc != null && typeDesc.DataType is XmlSchemaSimpleType)
		{
			return (XmlSchemaSimpleType)typeDesc.DataType;
		}
		XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)base.Schemas.Find(name, typeof(XmlSchemaSimpleType));
		if (xmlSchemaSimpleType != null)
		{
			return xmlSchemaSimpleType;
		}
		if (name.Namespace == "http://www.w3.org/2001/XMLSchema")
		{
			return (XmlSchemaSimpleType)base.Scope.GetTypeDesc(typeof(string)).DataType;
		}
		throw new InvalidOperationException(Res.GetString("The datatype '{0}' is missing.", name.ToString()));
	}

	private object FindType(XmlQualifiedName name)
	{
		if (name != null && name.Namespace == "http://schemas.xmlsoap.org/soap/encoding/")
		{
			object obj = base.Schemas.Find(name, typeof(XmlSchemaComplexType));
			if (obj != null)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)obj;
				XmlQualifiedName derivedFrom = xmlSchemaType.DerivedFrom;
				if (!derivedFrom.IsEmpty)
				{
					return FindType(derivedFrom);
				}
				return xmlSchemaType;
			}
			return FindDataType(name);
		}
		object obj2 = base.Schemas.Find(name, typeof(XmlSchemaComplexType));
		if (obj2 != null)
		{
			return obj2;
		}
		return FindDataType(name);
	}
}
