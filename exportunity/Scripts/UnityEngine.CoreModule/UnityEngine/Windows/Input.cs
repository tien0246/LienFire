using System;

namespace UnityEngine.Windows;

public static class Input
{
	public unsafe static void ForwardRawInput(IntPtr rawInputHeaderIndices, IntPtr rawInputDataIndices, uint indicesCount, IntPtr rawInputData, uint rawInputDataSize)
	{
		ForwardRawInput((uint*)(void*)rawInputHeaderIndices, (uint*)(void*)rawInputDataIndices, indicesCount, (byte*)(void*)rawInputData, rawInputDataSize);
	}

	public unsafe static void ForwardRawInput(uint* rawInputHeaderIndices, uint* rawInputDataIndices, uint indicesCount, byte* rawInputData, uint rawInputDataSize)
	{
		throw new NotSupportedException();
	}
}
