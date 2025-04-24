using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class ColorPalette
{
	private int _flags;

	private Color[] _entries;

	public int Flags => _flags;

	public Color[] Entries => _entries;

	internal ColorPalette(int count)
	{
		_entries = new Color[count];
	}

	internal ColorPalette()
	{
		_entries = new Color[1];
	}

	internal void ConvertFromMemory(IntPtr memory)
	{
		_flags = Marshal.ReadInt32(memory);
		int num = Marshal.ReadInt32((IntPtr)((long)memory + 4));
		_entries = new Color[num];
		for (int i = 0; i < num; i++)
		{
			int argb = Marshal.ReadInt32((IntPtr)((long)memory + 8 + i * 4));
			_entries[i] = Color.FromArgb(argb);
		}
	}

	internal IntPtr ConvertToMemory()
	{
		int num = _entries.Length;
		IntPtr intPtr;
		checked
		{
			intPtr = Marshal.AllocHGlobal(4 * (2 + num));
			Marshal.WriteInt32(intPtr, 0, _flags);
			Marshal.WriteInt32((IntPtr)((long)intPtr + 4), 0, num);
		}
		for (int i = 0; i < num; i++)
		{
			Marshal.WriteInt32((IntPtr)((long)intPtr + 4 * (i + 2)), 0, _entries[i].ToArgb());
		}
		return intPtr;
	}
}
