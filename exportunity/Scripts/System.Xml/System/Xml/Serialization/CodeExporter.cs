using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;
using Microsoft.CSharp;
using Unity;

namespace System.Xml.Serialization;

[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
public abstract class CodeExporter
{
	private Hashtable exportedMappings;

	private Hashtable exportedClasses;

	private CodeNamespace codeNamespace;

	private CodeCompileUnit codeCompileUnit;

	private bool rootExported;

	private TypeScope scope;

	private CodeAttributeDeclarationCollection includeMetadata;

	private CodeGenerationOptions options;

	private CodeDomProvider codeProvider;

	private CodeAttributeDeclaration generatedCodeAttribute;

	internal CodeCompileUnit CodeCompileUnit => codeCompileUnit;

	internal CodeNamespace CodeNamespace
	{
		get
		{
			if (codeNamespace == null)
			{
				codeNamespace = new CodeNamespace();
			}
			return codeNamespace;
		}
	}

	internal CodeDomProvider CodeProvider
	{
		get
		{
			if (codeProvider == null)
			{
				codeProvider = new CSharpCodeProvider();
			}
			return codeProvider;
		}
	}

	internal Hashtable ExportedClasses
	{
		get
		{
			if (exportedClasses == null)
			{
				exportedClasses = new Hashtable();
			}
			return exportedClasses;
		}
	}

	internal Hashtable ExportedMappings
	{
		get
		{
			if (exportedMappings == null)
			{
				exportedMappings = new Hashtable();
			}
			return exportedMappings;
		}
	}

	internal bool GenerateProperties => (options & CodeGenerationOptions.GenerateProperties) != 0;

	internal CodeAttributeDeclaration GeneratedCodeAttribute
	{
		get
		{
			if (generatedCodeAttribute == null)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(typeof(GeneratedCodeAttribute).FullName);
				Assembly assembly = Assembly.GetEntryAssembly();
				if (assembly == null)
				{
					assembly = Assembly.GetExecutingAssembly();
					if (assembly == null)
					{
						assembly = typeof(CodeExporter).Assembly;
					}
				}
				AssemblyName name = assembly.GetName();
				codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(name.Name)));
				string productVersion = GetProductVersion(assembly);
				codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression((productVersion == null) ? name.Version.ToString() : productVersion)));
				generatedCodeAttribute = codeAttributeDeclaration;
			}
			return generatedCodeAttribute;
		}
	}

	public CodeAttributeDeclarationCollection IncludeMetadata => includeMetadata;

	internal TypeScope Scope => scope;

	private bool EnableDataBinding => (options & CodeGenerationOptions.EnableDataBinding) != 0;

	internal static CodeMemberMethod RaisePropertyChangedEventMethod
	{
		get
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "RaisePropertyChanged";
			codeMemberMethod.Attributes = (MemberAttributes)12290;
			CodeArgumentReferenceExpression codeArgumentReferenceExpression = new CodeArgumentReferenceExpression("propertyName");
			codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), codeArgumentReferenceExpression.ParameterName));
			CodeVariableReferenceExpression codeVariableReferenceExpression = new CodeVariableReferenceExpression("propertyChanged");
			codeMemberMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(PropertyChangedEventHandler), codeVariableReferenceExpression.VariableName, new CodeEventReferenceExpression(new CodeThisReferenceExpression(), PropertyChangedEvent.Name)));
			CodeConditionStatement codeConditionStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(codeVariableReferenceExpression, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)));
			codeMemberMethod.Statements.Add(codeConditionStatement);
			codeConditionStatement.TrueStatements.Add(new CodeDelegateInvokeExpression(codeVariableReferenceExpression, new CodeThisReferenceExpression(), new CodeObjectCreateExpression(typeof(PropertyChangedEventArgs), codeArgumentReferenceExpression)));
			return codeMemberMethod;
		}
	}

	internal static CodeMemberEvent PropertyChangedEvent => new CodeMemberEvent
	{
		Attributes = MemberAttributes.Public,
		Name = "PropertyChanged",
		Type = new CodeTypeReference(typeof(PropertyChangedEventHandler)),
		ImplementationTypes = { typeof(INotifyPropertyChanged) }
	};

	internal CodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable exportedMappings)
	{
		includeMetadata = new CodeAttributeDeclarationCollection();
		base._002Ector();
		if (codeNamespace != null)
		{
			CodeGenerator.ValidateIdentifiers(codeNamespace);
		}
		this.codeNamespace = codeNamespace;
		if (codeCompileUnit != null)
		{
			if (!codeCompileUnit.ReferencedAssemblies.Contains("System.dll"))
			{
				codeCompileUnit.ReferencedAssemblies.Add("System.dll");
			}
			if (!codeCompileUnit.ReferencedAssemblies.Contains("System.Xml.dll"))
			{
				codeCompileUnit.ReferencedAssemblies.Add("System.Xml.dll");
			}
		}
		this.codeCompileUnit = codeCompileUnit;
		this.options = options;
		this.exportedMappings = exportedMappings;
		this.codeProvider = codeProvider;
	}

	internal static CodeAttributeDeclaration FindAttributeDeclaration(Type type, CodeAttributeDeclarationCollection metadata)
	{
		foreach (CodeAttributeDeclaration metadatum in metadata)
		{
			if (metadatum.Name == type.FullName || metadatum.Name == type.Name)
			{
				return metadatum;
			}
		}
		return null;
	}

	private static string GetProductVersion(Assembly assembly)
	{
		object[] customAttributes = assembly.GetCustomAttributes(inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			if (customAttributes[i] is AssemblyInformationalVersionAttribute)
			{
				return ((AssemblyInformationalVersionAttribute)customAttributes[i]).InformationalVersion;
			}
		}
		return null;
	}

	internal void CheckScope(TypeScope scope)
	{
		if (this.scope == null)
		{
			this.scope = scope;
		}
		else if (this.scope != scope)
		{
			throw new InvalidOperationException(Res.GetString("Exported mappings must come from the same importer."));
		}
	}

	internal abstract void ExportDerivedStructs(StructMapping mapping);

	internal abstract void EnsureTypesExported(Accessor[] accessors, string ns);

	internal static void AddWarningComment(CodeCommentStatementCollection comments, string text)
	{
		comments.Add(new CodeCommentStatement(Res.GetString("CODEGEN Warning: {0}", text), docComment: false));
	}

	internal void ExportRoot(StructMapping mapping, Type includeType)
	{
		if (rootExported)
		{
			return;
		}
		rootExported = true;
		ExportDerivedStructs(mapping);
		for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
		{
			if (!structMapping.ReferencedByElement && structMapping.IncludeInSchema && !structMapping.IsAnonymousType)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(includeType.FullName);
				codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(structMapping.TypeDesc.FullName)));
				includeMetadata.Add(codeAttributeDeclaration);
			}
		}
		Hashtable hashtable = new Hashtable();
		foreach (TypeMapping typeMapping in Scope.TypeMappings)
		{
			if (typeMapping is ArrayMapping)
			{
				ArrayMapping arrayMapping = (ArrayMapping)typeMapping;
				if (ShouldInclude(arrayMapping) && !hashtable.Contains(arrayMapping.TypeDesc.FullName))
				{
					CodeAttributeDeclaration codeAttributeDeclaration2 = new CodeAttributeDeclaration(includeType.FullName);
					codeAttributeDeclaration2.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(arrayMapping.TypeDesc.FullName)));
					includeMetadata.Add(codeAttributeDeclaration2);
					hashtable.Add(arrayMapping.TypeDesc.FullName, string.Empty);
					Accessor[] elements = arrayMapping.Elements;
					EnsureTypesExported(elements, arrayMapping.Namespace);
				}
			}
		}
	}

	private static bool ShouldInclude(ArrayMapping arrayMapping)
	{
		if (arrayMapping.ReferencedByElement)
		{
			return false;
		}
		if (arrayMapping.Next != null)
		{
			return false;
		}
		if (arrayMapping.Elements.Length == 1 && arrayMapping.Elements[0].Mapping.TypeDesc.Kind == TypeKind.Node)
		{
			return false;
		}
		for (int i = 0; i < arrayMapping.Elements.Length; i++)
		{
			if (arrayMapping.Elements[i].Name != arrayMapping.Elements[i].Mapping.DefaultElementName)
			{
				return false;
			}
		}
		return true;
	}

	internal CodeTypeDeclaration ExportEnum(EnumMapping mapping, Type type)
	{
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(mapping.TypeDesc.Name);
		codeTypeDeclaration.Comments.Add(new CodeCommentStatement(Res.GetString("<remarks/>"), docComment: true));
		codeTypeDeclaration.IsEnum = true;
		if (mapping.IsFlags && mapping.Constants.Length > 31)
		{
			codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(typeof(long)));
		}
		codeTypeDeclaration.TypeAttributes |= TypeAttributes.Public;
		CodeNamespace.Types.Add(codeTypeDeclaration);
		for (int i = 0; i < mapping.Constants.Length; i++)
		{
			ExportConstant(codeTypeDeclaration, mapping.Constants[i], type, mapping.IsFlags, 1L << i);
		}
		if (mapping.IsFlags)
		{
			CodeAttributeDeclaration value = new CodeAttributeDeclaration(typeof(FlagsAttribute).FullName);
			codeTypeDeclaration.CustomAttributes.Add(value);
		}
		CodeGenerator.ValidateIdentifiers(codeTypeDeclaration);
		return codeTypeDeclaration;
	}

	internal void AddTypeMetadata(CodeAttributeDeclarationCollection metadata, Type type, string defaultName, string name, string ns, bool includeInSchema)
	{
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(type.FullName);
		if (name == null || name.Length == 0)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument("AnonymousType", new CodePrimitiveExpression(true)));
		}
		else if (defaultName != name)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument("TypeName", new CodePrimitiveExpression(name)));
		}
		if (ns != null && ns.Length != 0)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(ns)));
		}
		if (!includeInSchema)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument("IncludeInSchema", new CodePrimitiveExpression(false)));
		}
		if (codeAttributeDeclaration.Arguments.Count > 0)
		{
			metadata.Add(codeAttributeDeclaration);
		}
	}

	internal static void AddIncludeMetadata(CodeAttributeDeclarationCollection metadata, StructMapping mapping, Type type)
	{
		if (!mapping.IsAnonymousType)
		{
			for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(type.FullName);
				codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(structMapping.TypeDesc.FullName)));
				metadata.Add(codeAttributeDeclaration);
				AddIncludeMetadata(metadata, structMapping, type);
			}
		}
	}

	internal static void ExportConstant(CodeTypeDeclaration codeClass, ConstantMapping constant, Type type, bool init, long enumValue)
	{
		CodeMemberField codeMemberField = new CodeMemberField(typeof(int).FullName, constant.Name);
		codeMemberField.Comments.Add(new CodeCommentStatement(Res.GetString("<remarks/>"), docComment: true));
		if (init)
		{
			codeMemberField.InitExpression = new CodePrimitiveExpression(enumValue);
		}
		codeClass.Members.Add(codeMemberField);
		if (constant.XmlName != constant.Name)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(type.FullName);
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(constant.XmlName)));
			codeMemberField.CustomAttributes.Add(codeAttributeDeclaration);
		}
	}

	internal static object PromoteType(Type type, object value)
	{
		if (type == typeof(sbyte))
		{
			return ((IConvertible)value).ToInt16(null);
		}
		if (type == typeof(ushort))
		{
			return ((IConvertible)value).ToInt32(null);
		}
		if (type == typeof(uint))
		{
			return ((IConvertible)value).ToInt64(null);
		}
		if (type == typeof(ulong))
		{
			return ((IConvertible)value).ToDecimal(null);
		}
		return value;
	}

	internal CodeMemberProperty CreatePropertyDeclaration(CodeMemberField field, string name, string typeName)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Type = new CodeTypeReference(typeName);
		codeMemberProperty.Name = name;
		codeMemberProperty.Attributes = (codeMemberProperty.Attributes & (MemberAttributes)(-61441)) | MemberAttributes.Public;
		CodeMethodReturnStatement codeMethodReturnStatement = new CodeMethodReturnStatement();
		codeMethodReturnStatement.Expression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name);
		codeMemberProperty.GetStatements.Add(codeMethodReturnStatement);
		CodeAssignStatement codeAssignStatement = new CodeAssignStatement();
		CodeExpression left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name);
		CodeExpression right = new CodePropertySetValueReferenceExpression();
		codeAssignStatement.Left = left;
		codeAssignStatement.Right = right;
		if (EnableDataBinding)
		{
			codeMemberProperty.SetStatements.Add(codeAssignStatement);
			codeMemberProperty.SetStatements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), RaisePropertyChangedEventMethod.Name, new CodePrimitiveExpression(name)));
		}
		else
		{
			codeMemberProperty.SetStatements.Add(codeAssignStatement);
		}
		return codeMemberProperty;
	}

	internal static string MakeFieldName(string name)
	{
		return CodeIdentifier.MakeCamel(name) + "Field";
	}

	internal void AddPropertyChangedNotifier(CodeTypeDeclaration codeClass)
	{
		if (EnableDataBinding && codeClass != null)
		{
			if (codeClass.BaseTypes.Count == 0)
			{
				codeClass.BaseTypes.Add(typeof(object));
			}
			codeClass.BaseTypes.Add(new CodeTypeReference(typeof(INotifyPropertyChanged)));
			codeClass.Members.Add(PropertyChangedEvent);
			codeClass.Members.Add(RaisePropertyChangedEventMethod);
		}
	}

	internal CodeExporter()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
