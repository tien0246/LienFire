using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaSimpleTypeRestriction : XmlSchemaSimpleTypeContent
{
	private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;

	private XmlSchemaSimpleType baseType;

	private XmlSchemaObjectCollection facets = new XmlSchemaObjectCollection();

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

	[XmlElement("length", typeof(XmlSchemaLengthFacet))]
	[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
	[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
	[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
	[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
	[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
	[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
	[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
	[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
	[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
	[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
	[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
	public XmlSchemaObjectCollection Facets => facets;

	internal override XmlSchemaObject Clone()
	{
		XmlSchemaSimpleTypeRestriction obj = (XmlSchemaSimpleTypeRestriction)MemberwiseClone();
		obj.BaseTypeName = baseTypeName.Clone();
		return obj;
	}
}
