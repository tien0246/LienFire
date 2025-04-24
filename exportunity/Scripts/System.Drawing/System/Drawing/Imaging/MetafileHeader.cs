using System.Runtime.InteropServices;
using Unity;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
[System.MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
public sealed class MetafileHeader
{
	private MonoMetafileHeader header;

	public Rectangle Bounds => new Rectangle(header.x, header.y, header.width, header.height);

	public float DpiX => header.dpi_x;

	public float DpiY => header.dpi_y;

	public int EmfPlusHeaderSize => header.emfplus_header_size;

	public int LogicalDpiX => header.logical_dpi_x;

	public int LogicalDpiY => header.logical_dpi_y;

	public int MetafileSize => header.size;

	public MetafileType Type => header.type;

	public int Version => header.version;

	public MetaHeader WmfHeader
	{
		get
		{
			if (IsWmf())
			{
				return new MetaHeader(header.wmf_header);
			}
			throw new ArgumentException("WmfHeader only available on WMF files.");
		}
	}

	internal MetafileHeader(IntPtr henhmetafile)
	{
		Marshal.PtrToStructure(henhmetafile, this);
	}

	[System.MonoTODO("always returns false")]
	public bool IsDisplay()
	{
		return false;
	}

	public bool IsEmf()
	{
		return Type == MetafileType.Emf;
	}

	public bool IsEmfOrEmfPlus()
	{
		return Type >= MetafileType.Emf;
	}

	public bool IsEmfPlus()
	{
		return Type >= MetafileType.EmfPlusOnly;
	}

	public bool IsEmfPlusDual()
	{
		return Type == MetafileType.EmfPlusDual;
	}

	public bool IsEmfPlusOnly()
	{
		return Type == MetafileType.EmfPlusOnly;
	}

	public bool IsWmf()
	{
		return Type <= MetafileType.WmfPlaceable;
	}

	public bool IsWmfPlaceable()
	{
		return Type == MetafileType.WmfPlaceable;
	}

	internal MetafileHeader()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
