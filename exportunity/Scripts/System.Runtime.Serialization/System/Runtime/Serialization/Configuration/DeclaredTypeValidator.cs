using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

internal class DeclaredTypeValidator : ConfigurationValidatorBase
{
	public override bool CanValidate(Type type)
	{
		return typeof(string) == type;
	}

	public override void Validate(object value)
	{
		string text = (string)value;
		if (text.StartsWith(Globals.TypeOfObject.FullName, StringComparison.Ordinal))
		{
			Type type = Type.GetType(text, throwOnError: false);
			if (type != null && Globals.TypeOfObject.Equals(type))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("Known type configuration specifies System.Object."));
			}
		}
	}
}
