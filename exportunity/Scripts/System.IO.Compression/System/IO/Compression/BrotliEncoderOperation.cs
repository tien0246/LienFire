namespace System.IO.Compression;

internal enum BrotliEncoderOperation
{
	Process = 0,
	Flush = 1,
	Finish = 2,
	EmitMetadata = 3
}
