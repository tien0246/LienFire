using System.Collections.Generic;
using System.ComponentModel;

namespace System.Security.Claims;

public static class DynamicRoleClaimProvider
{
	[Obsolete("Use ClaimsAuthenticationManager to add claims to a ClaimsIdentity", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void AddDynamicRoleClaims(ClaimsIdentity claimsIdentity, IEnumerable<Claim> claims)
	{
		claimsIdentity.ExternalClaims.Add(claims);
	}
}
