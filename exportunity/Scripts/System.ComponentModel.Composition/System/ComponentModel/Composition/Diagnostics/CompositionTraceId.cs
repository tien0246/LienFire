namespace System.ComponentModel.Composition.Diagnostics;

internal enum CompositionTraceId : ushort
{
	Rejection_DefinitionRejected = 1,
	Rejection_DefinitionResurrected = 2,
	Discovery_AssemblyLoadFailed = 3,
	Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute = 4,
	Discovery_DefinitionMismatchedExportArity = 5,
	Discovery_DefinitionContainsNoExports = 6,
	Discovery_MemberMarkedWithMultipleImportAndImportMany = 7
}
