using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class ApplicationDirectory : EvidenceBase, IBuiltInEvidence
{
	private string directory;

	public string Directory => directory;

	public ApplicationDirectory(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length < 1)
		{
			throw new FormatException(Locale.GetText("Empty"));
		}
		directory = name;
	}

	public object Copy()
	{
		return new ApplicationDirectory(Directory);
	}

	public override bool Equals(object o)
	{
		if (o is ApplicationDirectory applicationDirectory)
		{
			ThrowOnInvalid(applicationDirectory.directory);
			return directory == applicationDirectory.directory;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Directory.GetHashCode();
	}

	public override string ToString()
	{
		ThrowOnInvalid(Directory);
		SecurityElement securityElement = new SecurityElement("System.Security.Policy.ApplicationDirectory");
		securityElement.AddAttribute("version", "1");
		securityElement.AddChild(new SecurityElement("Directory", directory));
		return securityElement.ToString();
	}

	int IBuiltInEvidence.GetRequiredSize(bool verbose)
	{
		return ((!verbose) ? 1 : 3) + directory.Length;
	}

	[MonoTODO("IBuiltInEvidence")]
	int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
	{
		return 0;
	}

	[MonoTODO("IBuiltInEvidence")]
	int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
	{
		return 0;
	}

	private void ThrowOnInvalid(string appdir)
	{
		if (appdir.IndexOfAny(Path.InvalidPathChars) != -1)
		{
			throw new ArgumentException(string.Format(Locale.GetText("Invalid character(s) in directory {0}"), appdir), "other");
		}
	}
}
