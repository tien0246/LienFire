using System;

namespace Mirror.Examples.MultipleMatch;

[Flags]
public enum CellValue : ushort
{
	None = 0,
	A1 = 1,
	B1 = 2,
	C1 = 4,
	A2 = 8,
	B2 = 0x10,
	C2 = 0x20,
	A3 = 0x40,
	B3 = 0x80,
	C3 = 0x100,
	TopRow = 7,
	MidRow = 0x38,
	BotRow = 0x1C0,
	LeftCol = 0x49,
	MidCol = 0x92,
	RightCol = 0x124,
	Diag1 = 0x111,
	Diag2 = 0x54,
	Full = 0x1FF
}
