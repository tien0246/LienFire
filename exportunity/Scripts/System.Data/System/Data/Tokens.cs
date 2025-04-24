namespace System.Data;

internal enum Tokens
{
	None = 0,
	Name = 1,
	Numeric = 2,
	Decimal = 3,
	Float = 4,
	BinaryConst = 5,
	StringConst = 6,
	Date = 7,
	ListSeparator = 8,
	LeftParen = 9,
	RightParen = 10,
	ZeroOp = 11,
	UnaryOp = 12,
	BinaryOp = 13,
	Child = 14,
	Parent = 15,
	Dot = 16,
	Unknown = 17,
	EOS = 18
}
