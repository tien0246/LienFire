namespace System.ComponentModel.Composition;

internal enum CompositionErrorId
{
	Unknown = 0,
	InvalidExportMetadata = 1,
	ImportNotSetOnPart = 2,
	ImportEngine_ComposeTookTooManyIterations = 3,
	ImportEngine_ImportCardinalityMismatch = 4,
	ImportEngine_PartCycle = 5,
	ImportEngine_PartCannotSetImport = 6,
	ImportEngine_PartCannotGetExportedValue = 7,
	ImportEngine_PartCannotActivate = 8,
	ImportEngine_PreventedByExistingImport = 9,
	ImportEngine_InvalidStateForRecomposition = 10,
	ReflectionModel_ImportThrewException = 11,
	ReflectionModel_ImportNotAssignableFromExport = 12,
	ReflectionModel_ImportCollectionNull = 13,
	ReflectionModel_ImportCollectionNotWritable = 14,
	ReflectionModel_ImportCollectionConstructionThrewException = 15,
	ReflectionModel_ImportCollectionGetThrewException = 16,
	ReflectionModel_ImportCollectionIsReadOnlyThrewException = 17,
	ReflectionModel_ImportCollectionClearThrewException = 18,
	ReflectionModel_ImportCollectionAddThrewException = 19,
	ReflectionModel_ImportManyOnParameterCanOnlyBeAssigned = 20
}
