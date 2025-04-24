namespace System.Data.SqlClient;

internal enum ParsingErrorState
{
	Undefined = 0,
	FedAuthInfoLengthTooShortForCountOfInfoIds = 1,
	FedAuthInfoLengthTooShortForData = 2,
	FedAuthInfoFailedToReadCountOfInfoIds = 3,
	FedAuthInfoFailedToReadTokenStream = 4,
	FedAuthInfoInvalidOffset = 5,
	FedAuthInfoFailedToReadData = 6,
	FedAuthInfoDataNotUnicode = 7,
	FedAuthInfoDoesNotContainStsurlAndSpn = 8,
	FedAuthInfoNotReceived = 9,
	FedAuthNotAcknowledged = 10,
	FedAuthFeatureAckContainsExtraData = 11,
	FedAuthFeatureAckUnknownLibraryType = 12,
	UnrequestedFeatureAckReceived = 13,
	UnknownFeatureAck = 14,
	InvalidTdsTokenReceived = 15,
	SessionStateLengthTooShort = 16,
	SessionStateInvalidStatus = 17,
	CorruptedTdsStream = 18,
	ProcessSniPacketFailed = 19,
	FedAuthRequiredPreLoginResponseInvalidValue = 20
}
