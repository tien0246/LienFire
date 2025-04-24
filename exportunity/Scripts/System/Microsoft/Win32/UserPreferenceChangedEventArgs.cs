using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
public class UserPreferenceChangedEventArgs : EventArgs
{
	private UserPreferenceCategory mycategory;

	public UserPreferenceCategory Category => mycategory;

	public UserPreferenceChangedEventArgs(UserPreferenceCategory category)
	{
		mycategory = category;
	}
}
