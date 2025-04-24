namespace System.Drawing.Printing;

[Serializable]
public class PaperSource
{
	private string _name;

	private PaperSourceKind _kind;

	public PaperSourceKind Kind
	{
		get
		{
			if (_kind >= (PaperSourceKind)256)
			{
				return PaperSourceKind.Custom;
			}
			return _kind;
		}
	}

	public int RawKind
	{
		get
		{
			return (int)_kind;
		}
		set
		{
			_kind = (PaperSourceKind)value;
		}
	}

	public string SourceName
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public PaperSource()
	{
		_kind = PaperSourceKind.Custom;
		_name = string.Empty;
	}

	internal PaperSource(PaperSourceKind kind, string name)
	{
		_kind = kind;
		_name = name;
	}

	public override string ToString()
	{
		return "[PaperSource " + SourceName + " Kind=" + Kind.ToString() + "]";
	}
}
