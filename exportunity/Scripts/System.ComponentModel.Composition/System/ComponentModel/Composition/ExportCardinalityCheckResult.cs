namespace System.ComponentModel.Composition;

internal enum ExportCardinalityCheckResult
{
	Match = 0,
	NoExports = 1,
	TooManyExports = 2
}
