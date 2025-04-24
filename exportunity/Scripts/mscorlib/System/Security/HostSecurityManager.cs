using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Unity;

namespace System.Security;

[Serializable]
[ComVisible(true)]
public class HostSecurityManager
{
	public virtual PolicyLevel DomainPolicy => null;

	public virtual HostSecurityManagerOptions Flags => HostSecurityManagerOptions.AllFlags;

	public virtual ApplicationTrust DetermineApplicationTrust(Evidence applicationEvidence, Evidence activatorEvidence, TrustManagerContext context)
	{
		if (applicationEvidence == null)
		{
			throw new ArgumentNullException("applicationEvidence");
		}
		ActivationArguments activationArguments = null;
		foreach (object item in applicationEvidence)
		{
			activationArguments = item as ActivationArguments;
			if (activationArguments != null)
			{
				break;
			}
		}
		if (activationArguments == null)
		{
			throw new ArgumentException(string.Format(Locale.GetText("No {0} found in {1}."), "ActivationArguments", "Evidence"), "applicationEvidence");
		}
		if (activationArguments.ActivationContext == null)
		{
			throw new ArgumentException(string.Format(Locale.GetText("No {0} found in {1}."), "ActivationContext", "ActivationArguments"), "applicationEvidence");
		}
		if (ApplicationSecurityManager.DetermineApplicationTrust(activationArguments.ActivationContext, context))
		{
			if (activationArguments.ApplicationIdentity == null)
			{
				return new ApplicationTrust();
			}
			return new ApplicationTrust(activationArguments.ApplicationIdentity);
		}
		return null;
	}

	public virtual Evidence ProvideAppDomainEvidence(Evidence inputEvidence)
	{
		return inputEvidence;
	}

	public virtual Evidence ProvideAssemblyEvidence(Assembly loadedAssembly, Evidence inputEvidence)
	{
		return inputEvidence;
	}

	public virtual PermissionSet ResolvePolicy(Evidence evidence)
	{
		if (evidence == null)
		{
			throw new NullReferenceException("evidence");
		}
		return SecurityManager.ResolvePolicy(evidence);
	}

	public virtual EvidenceBase GenerateAppDomainEvidence(Type evidenceType)
	{
		ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public virtual EvidenceBase GenerateAssemblyEvidence(Type evidenceType, Assembly assembly)
	{
		ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public virtual Type[] GetHostSuppliedAppDomainEvidenceTypes()
	{
		ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public virtual Type[] GetHostSuppliedAssemblyEvidenceTypes(Assembly assembly)
	{
		ThrowStub.ThrowNotSupportedException();
		return null;
	}
}
