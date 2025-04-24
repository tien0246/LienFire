using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaChoice : XmlSchemaGroupBase
{
	private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();

	[XmlElement("sequence", typeof(XmlSchemaSequence))]
	[XmlElement("any", typeof(XmlSchemaAny))]
	[XmlElement("group", typeof(XmlSchemaGroupRef))]
	[XmlElement("element", typeof(XmlSchemaElement))]
	[XmlElement("choice", typeof(XmlSchemaChoice))]
	public override XmlSchemaObjectCollection Items => items;

	internal override bool IsEmpty => base.IsEmpty;

	internal override void SetItems(XmlSchemaObjectCollection newItems)
	{
		items = newItems;
	}
}
