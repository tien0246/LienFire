using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

public sealed class SerializationSectionGroup : ConfigurationSectionGroup
{
	public DataContractSerializerSection DataContractSerializer => (DataContractSerializerSection)base.Sections["dataContractSerializer"];

	public NetDataContractSerializerSection NetDataContractSerializer => (NetDataContractSerializerSection)base.Sections["netDataContractSerializer"];

	public static SerializationSectionGroup GetSectionGroup(System.Configuration.Configuration config)
	{
		if (config == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
		}
		return (SerializationSectionGroup)config.SectionGroups["system.runtime.serialization"];
	}
}
