using System.Globalization;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class ToolboxItemAttribute : Attribute
{
	private Type _toolboxItemType;

	private string _toolboxItemTypeName;

	public static readonly ToolboxItemAttribute Default = new ToolboxItemAttribute("System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

	public static readonly ToolboxItemAttribute None = new ToolboxItemAttribute(defaultType: false);

	public Type ToolboxItemType
	{
		get
		{
			if (_toolboxItemType == null && _toolboxItemTypeName != null)
			{
				try
				{
					_toolboxItemType = Type.GetType(_toolboxItemTypeName, throwOnError: true);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(global::SR.Format("Failed to create ToolboxItem of type: {0}", _toolboxItemTypeName), innerException);
				}
			}
			return _toolboxItemType;
		}
	}

	public string ToolboxItemTypeName
	{
		get
		{
			if (_toolboxItemTypeName == null)
			{
				return string.Empty;
			}
			return _toolboxItemTypeName;
		}
	}

	public override bool IsDefaultAttribute()
	{
		return Equals(Default);
	}

	public ToolboxItemAttribute(bool defaultType)
	{
		if (defaultType)
		{
			_toolboxItemTypeName = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		}
	}

	public ToolboxItemAttribute(string toolboxItemTypeName)
	{
		toolboxItemTypeName.ToUpper(CultureInfo.InvariantCulture);
		_toolboxItemTypeName = toolboxItemTypeName;
	}

	public ToolboxItemAttribute(Type toolboxItemType)
	{
		_toolboxItemType = toolboxItemType;
		_toolboxItemTypeName = toolboxItemType.AssemblyQualifiedName;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (obj is ToolboxItemAttribute toolboxItemAttribute)
		{
			return toolboxItemAttribute.ToolboxItemTypeName == ToolboxItemTypeName;
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (_toolboxItemTypeName != null)
		{
			return _toolboxItemTypeName.GetHashCode();
		}
		return base.GetHashCode();
	}
}
