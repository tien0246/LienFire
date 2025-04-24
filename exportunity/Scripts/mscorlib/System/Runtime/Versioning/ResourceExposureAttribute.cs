using System.Diagnostics;

namespace System.Runtime.Versioning;

[Conditional("RESOURCE_ANNOTATION_WORK")]
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ResourceExposureAttribute : Attribute
{
	private ResourceScope _resourceExposureLevel;

	public ResourceScope ResourceExposureLevel => _resourceExposureLevel;

	public ResourceExposureAttribute(ResourceScope exposureLevel)
	{
		_resourceExposureLevel = exposureLevel;
	}
}
