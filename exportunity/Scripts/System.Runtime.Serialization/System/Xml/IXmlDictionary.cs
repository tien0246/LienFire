namespace System.Xml;

public interface IXmlDictionary
{
	bool TryLookup(string value, out XmlDictionaryString result);

	bool TryLookup(int key, out XmlDictionaryString result);

	bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result);
}
