using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb;

[Serializable]
public sealed class OleDbPermission : DBDataPermission
{
	private string[] _providerRestriction;

	private string _providers;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
	public string Provider
	{
		get
		{
			string text = _providers;
			if (text == null)
			{
				string[] providerRestriction = _providerRestriction;
				if (providerRestriction != null && providerRestriction.Length != 0)
				{
					text = providerRestriction[0];
					for (int i = 1; i < providerRestriction.Length; i++)
					{
						text = text + ";" + providerRestriction[i];
					}
				}
			}
			if (text == null)
			{
				return ADP.StrEmpty;
			}
			return text;
		}
		set
		{
			string[] providerRestriction = null;
			if (!ADP.IsEmpty(value))
			{
				providerRestriction = value.Split(new char[1] { ';' });
				providerRestriction = DBConnectionString.RemoveDuplicates(providerRestriction);
			}
			_providerRestriction = providerRestriction;
			_providers = value;
		}
	}

	[Obsolete("OleDbPermission() has been deprecated.  Use the OleDbPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
	public OleDbPermission()
		: this(PermissionState.None)
	{
	}

	public OleDbPermission(PermissionState state)
		: base(state)
	{
	}

	[Obsolete("OleDbPermission(PermissionState state, Boolean allowBlankPassword) has been deprecated.  Use the OleDbPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
	public OleDbPermission(PermissionState state, bool allowBlankPassword)
		: this(state)
	{
		base.AllowBlankPassword = allowBlankPassword;
	}

	private OleDbPermission(OleDbPermission permission)
		: base(permission)
	{
	}

	internal OleDbPermission(OleDbPermissionAttribute permissionAttribute)
		: base(permissionAttribute)
	{
	}

	internal OleDbPermission(OleDbConnectionString constr)
		: base(constr)
	{
		if (constr == null || constr.IsEmpty)
		{
			base.Add(ADP.StrEmpty, ADP.StrEmpty, KeyRestrictionBehavior.AllowOnly);
		}
	}

	public override IPermission Copy()
	{
		return new OleDbPermission(this);
	}
}
