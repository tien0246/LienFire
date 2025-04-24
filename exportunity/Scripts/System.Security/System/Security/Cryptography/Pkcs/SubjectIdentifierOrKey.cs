using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class SubjectIdentifierOrKey
{
	public SubjectIdentifierOrKeyType Type { get; }

	public object Value { get; }

	internal SubjectIdentifierOrKey(SubjectIdentifierOrKeyType type, object value)
	{
		Type = type;
		Value = value;
	}

	internal SubjectIdentifierOrKey()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
