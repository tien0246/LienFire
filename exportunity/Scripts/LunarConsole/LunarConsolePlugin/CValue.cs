namespace LunarConsolePlugin;

internal struct CValue
{
	public string stringValue;

	public int intValue;

	public float floatValue;

	public bool Equals(ref CValue other)
	{
		if (other.intValue == intValue && other.floatValue == floatValue)
		{
			return other.stringValue == stringValue;
		}
		return false;
	}
}
