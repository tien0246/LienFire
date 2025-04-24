using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaSimpleContentRestriction : XmlSchemaContent
{
	private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;

	private XmlSchemaSimpleType baseType;

	private XmlSchemaObjectCollection facets = new XmlSchemaObjectCollection();

	private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();

	private XmlSchemaAnyAttribute anyAttribute;

	[XmlAttribute("base")]
	public XmlQualifiedName BaseTypeName
	{
		get
		{
			return baseTypeName;
		}
		set
		{
			baseTypeName = ((value == null) ? XmlQualifiedName.Empty : value);
		}
	}

	[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
	public XmlSchemaSimpleType BaseType
	{
		get
		{
			return baseType;
		}
		set
		{
			baseType = value;
		}
	}

	[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
	[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
	[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
	[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
	[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
	[XmlElement("length", typeof(XmlSchemaLengthFacet))]
	[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
	[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
	[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
	[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
	[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
	[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
	public XmlSchemaObjectCollection Facets => facets;

	[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
	[XmlElement("attribute", typeof(XmlSchemaAttribute))]
	public XmlSchemaObjectCollection Attributes => attributes;

	[XmlElement("anyAttribute")]
	public XmlSchemaAnyAttribute AnyAttribute
	{
		get
		{
			return anyAttribute;
		}
		set
		{
			anyAttribute = value;
		}
	}

	internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
	{
		attributes = newAttributes;
	}
}
