using UnityEngine.Scripting;

namespace UnityEngineInternal.Video;

[UsedByNativeCode]
internal enum VideoError
{
	NoErr = 0,
	OutOfMemoryErr = 1,
	CantReadFile = 2,
	CantWriteFile = 3,
	BadParams = 4,
	NoData = 5,
	BadPermissions = 6,
	DeviceNotAvailable = 7,
	ResourceNotAvailable = 8,
	NetworkErr = 9
}
