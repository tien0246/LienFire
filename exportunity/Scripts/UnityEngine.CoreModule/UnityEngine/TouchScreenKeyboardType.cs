using System;

namespace UnityEngine;

public enum TouchScreenKeyboardType
{
	Default = 0,
	ASCIICapable = 1,
	NumbersAndPunctuation = 2,
	URL = 3,
	NumberPad = 4,
	PhonePad = 5,
	NamePhonePad = 6,
	EmailAddress = 7,
	[Obsolete("Wii U is no longer supported as of Unity 2018.1.")]
	NintendoNetworkAccount = 8,
	Social = 9,
	Search = 10,
	DecimalPad = 11,
	OneTimeCode = 12
}
