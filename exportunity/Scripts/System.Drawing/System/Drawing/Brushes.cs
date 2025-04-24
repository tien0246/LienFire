namespace System.Drawing;

public sealed class Brushes
{
	private static SolidBrush aliceBlue;

	private static SolidBrush antiqueWhite;

	private static SolidBrush aqua;

	private static SolidBrush aquamarine;

	private static SolidBrush azure;

	private static SolidBrush beige;

	private static SolidBrush bisque;

	private static SolidBrush black;

	private static SolidBrush blanchedAlmond;

	private static SolidBrush blue;

	private static SolidBrush blueViolet;

	private static SolidBrush brown;

	private static SolidBrush burlyWood;

	private static SolidBrush cadetBlue;

	private static SolidBrush chartreuse;

	private static SolidBrush chocolate;

	private static SolidBrush coral;

	private static SolidBrush cornflowerBlue;

	private static SolidBrush cornsilk;

	private static SolidBrush crimson;

	private static SolidBrush cyan;

	private static SolidBrush darkBlue;

	private static SolidBrush darkCyan;

	private static SolidBrush darkGoldenrod;

	private static SolidBrush darkGray;

	private static SolidBrush darkGreen;

	private static SolidBrush darkKhaki;

	private static SolidBrush darkMagenta;

	private static SolidBrush darkOliveGreen;

	private static SolidBrush darkOrange;

	private static SolidBrush darkOrchid;

	private static SolidBrush darkRed;

	private static SolidBrush darkSalmon;

	private static SolidBrush darkSeaGreen;

	private static SolidBrush darkSlateBlue;

	private static SolidBrush darkSlateGray;

	private static SolidBrush darkTurquoise;

	private static SolidBrush darkViolet;

	private static SolidBrush deepPink;

	private static SolidBrush deepSkyBlue;

	private static SolidBrush dimGray;

	private static SolidBrush dodgerBlue;

	private static SolidBrush firebrick;

	private static SolidBrush floralWhite;

	private static SolidBrush forestGreen;

	private static SolidBrush fuchsia;

	private static SolidBrush gainsboro;

	private static SolidBrush ghostWhite;

	private static SolidBrush gold;

	private static SolidBrush goldenrod;

	private static SolidBrush gray;

	private static SolidBrush green;

	private static SolidBrush greenYellow;

	private static SolidBrush honeydew;

	private static SolidBrush hotPink;

	private static SolidBrush indianRed;

	private static SolidBrush indigo;

	private static SolidBrush ivory;

	private static SolidBrush khaki;

	private static SolidBrush lavender;

	private static SolidBrush lavenderBlush;

	private static SolidBrush lawnGreen;

	private static SolidBrush lemonChiffon;

	private static SolidBrush lightBlue;

	private static SolidBrush lightCoral;

	private static SolidBrush lightCyan;

	private static SolidBrush lightGoldenrodYellow;

	private static SolidBrush lightGray;

	private static SolidBrush lightGreen;

	private static SolidBrush lightPink;

	private static SolidBrush lightSalmon;

	private static SolidBrush lightSeaGreen;

	private static SolidBrush lightSkyBlue;

	private static SolidBrush lightSlateGray;

	private static SolidBrush lightSteelBlue;

	private static SolidBrush lightYellow;

	private static SolidBrush lime;

	private static SolidBrush limeGreen;

	private static SolidBrush linen;

	private static SolidBrush magenta;

	private static SolidBrush maroon;

	private static SolidBrush mediumAquamarine;

	private static SolidBrush mediumBlue;

	private static SolidBrush mediumOrchid;

	private static SolidBrush mediumPurple;

	private static SolidBrush mediumSeaGreen;

	private static SolidBrush mediumSlateBlue;

	private static SolidBrush mediumSpringGreen;

	private static SolidBrush mediumTurquoise;

	private static SolidBrush mediumVioletRed;

	private static SolidBrush midnightBlue;

	private static SolidBrush mintCream;

	private static SolidBrush mistyRose;

	private static SolidBrush moccasin;

	private static SolidBrush navajoWhite;

	private static SolidBrush navy;

	private static SolidBrush oldLace;

	private static SolidBrush olive;

	private static SolidBrush oliveDrab;

	private static SolidBrush orange;

	private static SolidBrush orangeRed;

	private static SolidBrush orchid;

	private static SolidBrush paleGoldenrod;

	private static SolidBrush paleGreen;

	private static SolidBrush paleTurquoise;

	private static SolidBrush paleVioletRed;

	private static SolidBrush papayaWhip;

	private static SolidBrush peachPuff;

	private static SolidBrush peru;

	private static SolidBrush pink;

	private static SolidBrush plum;

	private static SolidBrush powderBlue;

	private static SolidBrush purple;

	private static SolidBrush red;

	private static SolidBrush rosyBrown;

	private static SolidBrush royalBlue;

	private static SolidBrush saddleBrown;

	private static SolidBrush salmon;

	private static SolidBrush sandyBrown;

	private static SolidBrush seaGreen;

	private static SolidBrush seaShell;

	private static SolidBrush sienna;

	private static SolidBrush silver;

	private static SolidBrush skyBlue;

	private static SolidBrush slateBlue;

	private static SolidBrush slateGray;

	private static SolidBrush snow;

	private static SolidBrush springGreen;

	private static SolidBrush steelBlue;

	private static SolidBrush tan;

	private static SolidBrush teal;

	private static SolidBrush thistle;

	private static SolidBrush tomato;

	private static SolidBrush transparent;

	private static SolidBrush turquoise;

	private static SolidBrush violet;

	private static SolidBrush wheat;

	private static SolidBrush white;

	private static SolidBrush whiteSmoke;

	private static SolidBrush yellow;

	private static SolidBrush yellowGreen;

	public static Brush AliceBlue
	{
		get
		{
			if (aliceBlue == null)
			{
				aliceBlue = new SolidBrush(Color.AliceBlue);
			}
			return aliceBlue;
		}
	}

	public static Brush AntiqueWhite
	{
		get
		{
			if (antiqueWhite == null)
			{
				antiqueWhite = new SolidBrush(Color.AntiqueWhite);
			}
			return antiqueWhite;
		}
	}

	public static Brush Aqua
	{
		get
		{
			if (aqua == null)
			{
				aqua = new SolidBrush(Color.Aqua);
			}
			return aqua;
		}
	}

	public static Brush Aquamarine
	{
		get
		{
			if (aquamarine == null)
			{
				aquamarine = new SolidBrush(Color.Aquamarine);
			}
			return aquamarine;
		}
	}

	public static Brush Azure
	{
		get
		{
			if (azure == null)
			{
				azure = new SolidBrush(Color.Azure);
			}
			return azure;
		}
	}

	public static Brush Beige
	{
		get
		{
			if (beige == null)
			{
				beige = new SolidBrush(Color.Beige);
			}
			return beige;
		}
	}

	public static Brush Bisque
	{
		get
		{
			if (bisque == null)
			{
				bisque = new SolidBrush(Color.Bisque);
			}
			return bisque;
		}
	}

	public static Brush Black
	{
		get
		{
			if (black == null)
			{
				black = new SolidBrush(Color.Black);
			}
			return black;
		}
	}

	public static Brush BlanchedAlmond
	{
		get
		{
			if (blanchedAlmond == null)
			{
				blanchedAlmond = new SolidBrush(Color.BlanchedAlmond);
			}
			return blanchedAlmond;
		}
	}

	public static Brush Blue
	{
		get
		{
			if (blue == null)
			{
				blue = new SolidBrush(Color.Blue);
			}
			return blue;
		}
	}

	public static Brush BlueViolet
	{
		get
		{
			if (blueViolet == null)
			{
				blueViolet = new SolidBrush(Color.BlueViolet);
			}
			return blueViolet;
		}
	}

	public static Brush Brown
	{
		get
		{
			if (brown == null)
			{
				brown = new SolidBrush(Color.Brown);
			}
			return brown;
		}
	}

	public static Brush BurlyWood
	{
		get
		{
			if (burlyWood == null)
			{
				burlyWood = new SolidBrush(Color.BurlyWood);
			}
			return burlyWood;
		}
	}

	public static Brush CadetBlue
	{
		get
		{
			if (cadetBlue == null)
			{
				cadetBlue = new SolidBrush(Color.CadetBlue);
			}
			return cadetBlue;
		}
	}

	public static Brush Chartreuse
	{
		get
		{
			if (chartreuse == null)
			{
				chartreuse = new SolidBrush(Color.Chartreuse);
			}
			return chartreuse;
		}
	}

	public static Brush Chocolate
	{
		get
		{
			if (chocolate == null)
			{
				chocolate = new SolidBrush(Color.Chocolate);
			}
			return chocolate;
		}
	}

	public static Brush Coral
	{
		get
		{
			if (coral == null)
			{
				coral = new SolidBrush(Color.Coral);
			}
			return coral;
		}
	}

	public static Brush CornflowerBlue
	{
		get
		{
			if (cornflowerBlue == null)
			{
				cornflowerBlue = new SolidBrush(Color.CornflowerBlue);
			}
			return cornflowerBlue;
		}
	}

	public static Brush Cornsilk
	{
		get
		{
			if (cornsilk == null)
			{
				cornsilk = new SolidBrush(Color.Cornsilk);
			}
			return cornsilk;
		}
	}

	public static Brush Crimson
	{
		get
		{
			if (crimson == null)
			{
				crimson = new SolidBrush(Color.Crimson);
			}
			return crimson;
		}
	}

	public static Brush Cyan
	{
		get
		{
			if (cyan == null)
			{
				cyan = new SolidBrush(Color.Cyan);
			}
			return cyan;
		}
	}

	public static Brush DarkBlue
	{
		get
		{
			if (darkBlue == null)
			{
				darkBlue = new SolidBrush(Color.DarkBlue);
			}
			return darkBlue;
		}
	}

	public static Brush DarkCyan
	{
		get
		{
			if (darkCyan == null)
			{
				darkCyan = new SolidBrush(Color.DarkCyan);
			}
			return darkCyan;
		}
	}

	public static Brush DarkGoldenrod
	{
		get
		{
			if (darkGoldenrod == null)
			{
				darkGoldenrod = new SolidBrush(Color.DarkGoldenrod);
			}
			return darkGoldenrod;
		}
	}

	public static Brush DarkGray
	{
		get
		{
			if (darkGray == null)
			{
				darkGray = new SolidBrush(Color.DarkGray);
			}
			return darkGray;
		}
	}

	public static Brush DarkGreen
	{
		get
		{
			if (darkGreen == null)
			{
				darkGreen = new SolidBrush(Color.DarkGreen);
			}
			return darkGreen;
		}
	}

	public static Brush DarkKhaki
	{
		get
		{
			if (darkKhaki == null)
			{
				darkKhaki = new SolidBrush(Color.DarkKhaki);
			}
			return darkKhaki;
		}
	}

	public static Brush DarkMagenta
	{
		get
		{
			if (darkMagenta == null)
			{
				darkMagenta = new SolidBrush(Color.DarkMagenta);
			}
			return darkMagenta;
		}
	}

	public static Brush DarkOliveGreen
	{
		get
		{
			if (darkOliveGreen == null)
			{
				darkOliveGreen = new SolidBrush(Color.DarkOliveGreen);
			}
			return darkOliveGreen;
		}
	}

	public static Brush DarkOrange
	{
		get
		{
			if (darkOrange == null)
			{
				darkOrange = new SolidBrush(Color.DarkOrange);
			}
			return darkOrange;
		}
	}

	public static Brush DarkOrchid
	{
		get
		{
			if (darkOrchid == null)
			{
				darkOrchid = new SolidBrush(Color.DarkOrchid);
			}
			return darkOrchid;
		}
	}

	public static Brush DarkRed
	{
		get
		{
			if (darkRed == null)
			{
				darkRed = new SolidBrush(Color.DarkRed);
			}
			return darkRed;
		}
	}

	public static Brush DarkSalmon
	{
		get
		{
			if (darkSalmon == null)
			{
				darkSalmon = new SolidBrush(Color.DarkSalmon);
			}
			return darkSalmon;
		}
	}

	public static Brush DarkSeaGreen
	{
		get
		{
			if (darkSeaGreen == null)
			{
				darkSeaGreen = new SolidBrush(Color.DarkSeaGreen);
			}
			return darkSeaGreen;
		}
	}

	public static Brush DarkSlateBlue
	{
		get
		{
			if (darkSlateBlue == null)
			{
				darkSlateBlue = new SolidBrush(Color.DarkSlateBlue);
			}
			return darkSlateBlue;
		}
	}

	public static Brush DarkSlateGray
	{
		get
		{
			if (darkSlateGray == null)
			{
				darkSlateGray = new SolidBrush(Color.DarkSlateGray);
			}
			return darkSlateGray;
		}
	}

	public static Brush DarkTurquoise
	{
		get
		{
			if (darkTurquoise == null)
			{
				darkTurquoise = new SolidBrush(Color.DarkTurquoise);
			}
			return darkTurquoise;
		}
	}

	public static Brush DarkViolet
	{
		get
		{
			if (darkViolet == null)
			{
				darkViolet = new SolidBrush(Color.DarkViolet);
			}
			return darkViolet;
		}
	}

	public static Brush DeepPink
	{
		get
		{
			if (deepPink == null)
			{
				deepPink = new SolidBrush(Color.DeepPink);
			}
			return deepPink;
		}
	}

	public static Brush DeepSkyBlue
	{
		get
		{
			if (deepSkyBlue == null)
			{
				deepSkyBlue = new SolidBrush(Color.DeepSkyBlue);
			}
			return deepSkyBlue;
		}
	}

	public static Brush DimGray
	{
		get
		{
			if (dimGray == null)
			{
				dimGray = new SolidBrush(Color.DimGray);
			}
			return dimGray;
		}
	}

	public static Brush DodgerBlue
	{
		get
		{
			if (dodgerBlue == null)
			{
				dodgerBlue = new SolidBrush(Color.DodgerBlue);
			}
			return dodgerBlue;
		}
	}

	public static Brush Firebrick
	{
		get
		{
			if (firebrick == null)
			{
				firebrick = new SolidBrush(Color.Firebrick);
			}
			return firebrick;
		}
	}

	public static Brush FloralWhite
	{
		get
		{
			if (floralWhite == null)
			{
				floralWhite = new SolidBrush(Color.FloralWhite);
			}
			return floralWhite;
		}
	}

	public static Brush ForestGreen
	{
		get
		{
			if (forestGreen == null)
			{
				forestGreen = new SolidBrush(Color.ForestGreen);
			}
			return forestGreen;
		}
	}

	public static Brush Fuchsia
	{
		get
		{
			if (fuchsia == null)
			{
				fuchsia = new SolidBrush(Color.Fuchsia);
			}
			return fuchsia;
		}
	}

	public static Brush Gainsboro
	{
		get
		{
			if (gainsboro == null)
			{
				gainsboro = new SolidBrush(Color.Gainsboro);
			}
			return gainsboro;
		}
	}

	public static Brush GhostWhite
	{
		get
		{
			if (ghostWhite == null)
			{
				ghostWhite = new SolidBrush(Color.GhostWhite);
			}
			return ghostWhite;
		}
	}

	public static Brush Gold
	{
		get
		{
			if (gold == null)
			{
				gold = new SolidBrush(Color.Gold);
			}
			return gold;
		}
	}

	public static Brush Goldenrod
	{
		get
		{
			if (goldenrod == null)
			{
				goldenrod = new SolidBrush(Color.Goldenrod);
			}
			return goldenrod;
		}
	}

	public static Brush Gray
	{
		get
		{
			if (gray == null)
			{
				gray = new SolidBrush(Color.Gray);
			}
			return gray;
		}
	}

	public static Brush Green
	{
		get
		{
			if (green == null)
			{
				green = new SolidBrush(Color.Green);
			}
			return green;
		}
	}

	public static Brush GreenYellow
	{
		get
		{
			if (greenYellow == null)
			{
				greenYellow = new SolidBrush(Color.GreenYellow);
			}
			return greenYellow;
		}
	}

	public static Brush Honeydew
	{
		get
		{
			if (honeydew == null)
			{
				honeydew = new SolidBrush(Color.Honeydew);
			}
			return honeydew;
		}
	}

	public static Brush HotPink
	{
		get
		{
			if (hotPink == null)
			{
				hotPink = new SolidBrush(Color.HotPink);
			}
			return hotPink;
		}
	}

	public static Brush IndianRed
	{
		get
		{
			if (indianRed == null)
			{
				indianRed = new SolidBrush(Color.IndianRed);
			}
			return indianRed;
		}
	}

	public static Brush Indigo
	{
		get
		{
			if (indigo == null)
			{
				indigo = new SolidBrush(Color.Indigo);
			}
			return indigo;
		}
	}

	public static Brush Ivory
	{
		get
		{
			if (ivory == null)
			{
				ivory = new SolidBrush(Color.Ivory);
			}
			return ivory;
		}
	}

	public static Brush Khaki
	{
		get
		{
			if (khaki == null)
			{
				khaki = new SolidBrush(Color.Khaki);
			}
			return khaki;
		}
	}

	public static Brush Lavender
	{
		get
		{
			if (lavender == null)
			{
				lavender = new SolidBrush(Color.Lavender);
			}
			return lavender;
		}
	}

	public static Brush LavenderBlush
	{
		get
		{
			if (lavenderBlush == null)
			{
				lavenderBlush = new SolidBrush(Color.LavenderBlush);
			}
			return lavenderBlush;
		}
	}

	public static Brush LawnGreen
	{
		get
		{
			if (lawnGreen == null)
			{
				lawnGreen = new SolidBrush(Color.LawnGreen);
			}
			return lawnGreen;
		}
	}

	public static Brush LemonChiffon
	{
		get
		{
			if (lemonChiffon == null)
			{
				lemonChiffon = new SolidBrush(Color.LemonChiffon);
			}
			return lemonChiffon;
		}
	}

	public static Brush LightBlue
	{
		get
		{
			if (lightBlue == null)
			{
				lightBlue = new SolidBrush(Color.LightBlue);
			}
			return lightBlue;
		}
	}

	public static Brush LightCoral
	{
		get
		{
			if (lightCoral == null)
			{
				lightCoral = new SolidBrush(Color.LightCoral);
			}
			return lightCoral;
		}
	}

	public static Brush LightCyan
	{
		get
		{
			if (lightCyan == null)
			{
				lightCyan = new SolidBrush(Color.LightCyan);
			}
			return lightCyan;
		}
	}

	public static Brush LightGoldenrodYellow
	{
		get
		{
			if (lightGoldenrodYellow == null)
			{
				lightGoldenrodYellow = new SolidBrush(Color.LightGoldenrodYellow);
			}
			return lightGoldenrodYellow;
		}
	}

	public static Brush LightGray
	{
		get
		{
			if (lightGray == null)
			{
				lightGray = new SolidBrush(Color.LightGray);
			}
			return lightGray;
		}
	}

	public static Brush LightGreen
	{
		get
		{
			if (lightGreen == null)
			{
				lightGreen = new SolidBrush(Color.LightGreen);
			}
			return lightGreen;
		}
	}

	public static Brush LightPink
	{
		get
		{
			if (lightPink == null)
			{
				lightPink = new SolidBrush(Color.LightPink);
			}
			return lightPink;
		}
	}

	public static Brush LightSalmon
	{
		get
		{
			if (lightSalmon == null)
			{
				lightSalmon = new SolidBrush(Color.LightSalmon);
			}
			return lightSalmon;
		}
	}

	public static Brush LightSeaGreen
	{
		get
		{
			if (lightSeaGreen == null)
			{
				lightSeaGreen = new SolidBrush(Color.LightSeaGreen);
			}
			return lightSeaGreen;
		}
	}

	public static Brush LightSkyBlue
	{
		get
		{
			if (lightSkyBlue == null)
			{
				lightSkyBlue = new SolidBrush(Color.LightSkyBlue);
			}
			return lightSkyBlue;
		}
	}

	public static Brush LightSlateGray
	{
		get
		{
			if (lightSlateGray == null)
			{
				lightSlateGray = new SolidBrush(Color.LightSlateGray);
			}
			return lightSlateGray;
		}
	}

	public static Brush LightSteelBlue
	{
		get
		{
			if (lightSteelBlue == null)
			{
				lightSteelBlue = new SolidBrush(Color.LightSteelBlue);
			}
			return lightSteelBlue;
		}
	}

	public static Brush LightYellow
	{
		get
		{
			if (lightYellow == null)
			{
				lightYellow = new SolidBrush(Color.LightYellow);
			}
			return lightYellow;
		}
	}

	public static Brush Lime
	{
		get
		{
			if (lime == null)
			{
				lime = new SolidBrush(Color.Lime);
			}
			return lime;
		}
	}

	public static Brush LimeGreen
	{
		get
		{
			if (limeGreen == null)
			{
				limeGreen = new SolidBrush(Color.LimeGreen);
			}
			return limeGreen;
		}
	}

	public static Brush Linen
	{
		get
		{
			if (linen == null)
			{
				linen = new SolidBrush(Color.Linen);
			}
			return linen;
		}
	}

	public static Brush Magenta
	{
		get
		{
			if (magenta == null)
			{
				magenta = new SolidBrush(Color.Magenta);
			}
			return magenta;
		}
	}

	public static Brush Maroon
	{
		get
		{
			if (maroon == null)
			{
				maroon = new SolidBrush(Color.Maroon);
			}
			return maroon;
		}
	}

	public static Brush MediumAquamarine
	{
		get
		{
			if (mediumAquamarine == null)
			{
				mediumAquamarine = new SolidBrush(Color.MediumAquamarine);
			}
			return mediumAquamarine;
		}
	}

	public static Brush MediumBlue
	{
		get
		{
			if (mediumBlue == null)
			{
				mediumBlue = new SolidBrush(Color.MediumBlue);
			}
			return mediumBlue;
		}
	}

	public static Brush MediumOrchid
	{
		get
		{
			if (mediumOrchid == null)
			{
				mediumOrchid = new SolidBrush(Color.MediumOrchid);
			}
			return mediumOrchid;
		}
	}

	public static Brush MediumPurple
	{
		get
		{
			if (mediumPurple == null)
			{
				mediumPurple = new SolidBrush(Color.MediumPurple);
			}
			return mediumPurple;
		}
	}

	public static Brush MediumSeaGreen
	{
		get
		{
			if (mediumSeaGreen == null)
			{
				mediumSeaGreen = new SolidBrush(Color.MediumSeaGreen);
			}
			return mediumSeaGreen;
		}
	}

	public static Brush MediumSlateBlue
	{
		get
		{
			if (mediumSlateBlue == null)
			{
				mediumSlateBlue = new SolidBrush(Color.MediumSlateBlue);
			}
			return mediumSlateBlue;
		}
	}

	public static Brush MediumSpringGreen
	{
		get
		{
			if (mediumSpringGreen == null)
			{
				mediumSpringGreen = new SolidBrush(Color.MediumSpringGreen);
			}
			return mediumSpringGreen;
		}
	}

	public static Brush MediumTurquoise
	{
		get
		{
			if (mediumTurquoise == null)
			{
				mediumTurquoise = new SolidBrush(Color.MediumTurquoise);
			}
			return mediumTurquoise;
		}
	}

	public static Brush MediumVioletRed
	{
		get
		{
			if (mediumVioletRed == null)
			{
				mediumVioletRed = new SolidBrush(Color.MediumVioletRed);
			}
			return mediumVioletRed;
		}
	}

	public static Brush MidnightBlue
	{
		get
		{
			if (midnightBlue == null)
			{
				midnightBlue = new SolidBrush(Color.MidnightBlue);
			}
			return midnightBlue;
		}
	}

	public static Brush MintCream
	{
		get
		{
			if (mintCream == null)
			{
				mintCream = new SolidBrush(Color.MintCream);
			}
			return mintCream;
		}
	}

	public static Brush MistyRose
	{
		get
		{
			if (mistyRose == null)
			{
				mistyRose = new SolidBrush(Color.MistyRose);
			}
			return mistyRose;
		}
	}

	public static Brush Moccasin
	{
		get
		{
			if (moccasin == null)
			{
				moccasin = new SolidBrush(Color.Moccasin);
			}
			return moccasin;
		}
	}

	public static Brush NavajoWhite
	{
		get
		{
			if (navajoWhite == null)
			{
				navajoWhite = new SolidBrush(Color.NavajoWhite);
			}
			return navajoWhite;
		}
	}

	public static Brush Navy
	{
		get
		{
			if (navy == null)
			{
				navy = new SolidBrush(Color.Navy);
			}
			return navy;
		}
	}

	public static Brush OldLace
	{
		get
		{
			if (oldLace == null)
			{
				oldLace = new SolidBrush(Color.OldLace);
			}
			return oldLace;
		}
	}

	public static Brush Olive
	{
		get
		{
			if (olive == null)
			{
				olive = new SolidBrush(Color.Olive);
			}
			return olive;
		}
	}

	public static Brush OliveDrab
	{
		get
		{
			if (oliveDrab == null)
			{
				oliveDrab = new SolidBrush(Color.OliveDrab);
			}
			return oliveDrab;
		}
	}

	public static Brush Orange
	{
		get
		{
			if (orange == null)
			{
				orange = new SolidBrush(Color.Orange);
			}
			return orange;
		}
	}

	public static Brush OrangeRed
	{
		get
		{
			if (orangeRed == null)
			{
				orangeRed = new SolidBrush(Color.OrangeRed);
			}
			return orangeRed;
		}
	}

	public static Brush Orchid
	{
		get
		{
			if (orchid == null)
			{
				orchid = new SolidBrush(Color.Orchid);
			}
			return orchid;
		}
	}

	public static Brush PaleGoldenrod
	{
		get
		{
			if (paleGoldenrod == null)
			{
				paleGoldenrod = new SolidBrush(Color.PaleGoldenrod);
			}
			return paleGoldenrod;
		}
	}

	public static Brush PaleGreen
	{
		get
		{
			if (paleGreen == null)
			{
				paleGreen = new SolidBrush(Color.PaleGreen);
			}
			return paleGreen;
		}
	}

	public static Brush PaleTurquoise
	{
		get
		{
			if (paleTurquoise == null)
			{
				paleTurquoise = new SolidBrush(Color.PaleTurquoise);
			}
			return paleTurquoise;
		}
	}

	public static Brush PaleVioletRed
	{
		get
		{
			if (paleVioletRed == null)
			{
				paleVioletRed = new SolidBrush(Color.PaleVioletRed);
			}
			return paleVioletRed;
		}
	}

	public static Brush PapayaWhip
	{
		get
		{
			if (papayaWhip == null)
			{
				papayaWhip = new SolidBrush(Color.PapayaWhip);
			}
			return papayaWhip;
		}
	}

	public static Brush PeachPuff
	{
		get
		{
			if (peachPuff == null)
			{
				peachPuff = new SolidBrush(Color.PeachPuff);
			}
			return peachPuff;
		}
	}

	public static Brush Peru
	{
		get
		{
			if (peru == null)
			{
				peru = new SolidBrush(Color.Peru);
			}
			return peru;
		}
	}

	public static Brush Pink
	{
		get
		{
			if (pink == null)
			{
				pink = new SolidBrush(Color.Pink);
			}
			return pink;
		}
	}

	public static Brush Plum
	{
		get
		{
			if (plum == null)
			{
				plum = new SolidBrush(Color.Plum);
			}
			return plum;
		}
	}

	public static Brush PowderBlue
	{
		get
		{
			if (powderBlue == null)
			{
				powderBlue = new SolidBrush(Color.PowderBlue);
			}
			return powderBlue;
		}
	}

	public static Brush Purple
	{
		get
		{
			if (purple == null)
			{
				purple = new SolidBrush(Color.Purple);
			}
			return purple;
		}
	}

	public static Brush Red
	{
		get
		{
			if (red == null)
			{
				red = new SolidBrush(Color.Red);
			}
			return red;
		}
	}

	public static Brush RosyBrown
	{
		get
		{
			if (rosyBrown == null)
			{
				rosyBrown = new SolidBrush(Color.RosyBrown);
			}
			return rosyBrown;
		}
	}

	public static Brush RoyalBlue
	{
		get
		{
			if (royalBlue == null)
			{
				royalBlue = new SolidBrush(Color.RoyalBlue);
			}
			return royalBlue;
		}
	}

	public static Brush SaddleBrown
	{
		get
		{
			if (saddleBrown == null)
			{
				saddleBrown = new SolidBrush(Color.SaddleBrown);
			}
			return saddleBrown;
		}
	}

	public static Brush Salmon
	{
		get
		{
			if (salmon == null)
			{
				salmon = new SolidBrush(Color.Salmon);
			}
			return salmon;
		}
	}

	public static Brush SandyBrown
	{
		get
		{
			if (sandyBrown == null)
			{
				sandyBrown = new SolidBrush(Color.SandyBrown);
			}
			return sandyBrown;
		}
	}

	public static Brush SeaGreen
	{
		get
		{
			if (seaGreen == null)
			{
				seaGreen = new SolidBrush(Color.SeaGreen);
			}
			return seaGreen;
		}
	}

	public static Brush SeaShell
	{
		get
		{
			if (seaShell == null)
			{
				seaShell = new SolidBrush(Color.SeaShell);
			}
			return seaShell;
		}
	}

	public static Brush Sienna
	{
		get
		{
			if (sienna == null)
			{
				sienna = new SolidBrush(Color.Sienna);
			}
			return sienna;
		}
	}

	public static Brush Silver
	{
		get
		{
			if (silver == null)
			{
				silver = new SolidBrush(Color.Silver);
			}
			return silver;
		}
	}

	public static Brush SkyBlue
	{
		get
		{
			if (skyBlue == null)
			{
				skyBlue = new SolidBrush(Color.SkyBlue);
			}
			return skyBlue;
		}
	}

	public static Brush SlateBlue
	{
		get
		{
			if (slateBlue == null)
			{
				slateBlue = new SolidBrush(Color.SlateBlue);
			}
			return slateBlue;
		}
	}

	public static Brush SlateGray
	{
		get
		{
			if (slateGray == null)
			{
				slateGray = new SolidBrush(Color.SlateGray);
			}
			return slateGray;
		}
	}

	public static Brush Snow
	{
		get
		{
			if (snow == null)
			{
				snow = new SolidBrush(Color.Snow);
			}
			return snow;
		}
	}

	public static Brush SpringGreen
	{
		get
		{
			if (springGreen == null)
			{
				springGreen = new SolidBrush(Color.SpringGreen);
			}
			return springGreen;
		}
	}

	public static Brush SteelBlue
	{
		get
		{
			if (steelBlue == null)
			{
				steelBlue = new SolidBrush(Color.SteelBlue);
			}
			return steelBlue;
		}
	}

	public static Brush Tan
	{
		get
		{
			if (tan == null)
			{
				tan = new SolidBrush(Color.Tan);
			}
			return tan;
		}
	}

	public static Brush Teal
	{
		get
		{
			if (teal == null)
			{
				teal = new SolidBrush(Color.Teal);
			}
			return teal;
		}
	}

	public static Brush Thistle
	{
		get
		{
			if (thistle == null)
			{
				thistle = new SolidBrush(Color.Thistle);
			}
			return thistle;
		}
	}

	public static Brush Tomato
	{
		get
		{
			if (tomato == null)
			{
				tomato = new SolidBrush(Color.Tomato);
			}
			return tomato;
		}
	}

	public static Brush Transparent
	{
		get
		{
			if (transparent == null)
			{
				transparent = new SolidBrush(Color.Transparent);
			}
			return transparent;
		}
	}

	public static Brush Turquoise
	{
		get
		{
			if (turquoise == null)
			{
				turquoise = new SolidBrush(Color.Turquoise);
			}
			return turquoise;
		}
	}

	public static Brush Violet
	{
		get
		{
			if (violet == null)
			{
				violet = new SolidBrush(Color.Violet);
			}
			return violet;
		}
	}

	public static Brush Wheat
	{
		get
		{
			if (wheat == null)
			{
				wheat = new SolidBrush(Color.Wheat);
			}
			return wheat;
		}
	}

	public static Brush White
	{
		get
		{
			if (white == null)
			{
				white = new SolidBrush(Color.White);
			}
			return white;
		}
	}

	public static Brush WhiteSmoke
	{
		get
		{
			if (whiteSmoke == null)
			{
				whiteSmoke = new SolidBrush(Color.WhiteSmoke);
			}
			return whiteSmoke;
		}
	}

	public static Brush Yellow
	{
		get
		{
			if (yellow == null)
			{
				yellow = new SolidBrush(Color.Yellow);
			}
			return yellow;
		}
	}

	public static Brush YellowGreen
	{
		get
		{
			if (yellowGreen == null)
			{
				yellowGreen = new SolidBrush(Color.YellowGreen);
			}
			return yellowGreen;
		}
	}

	private Brushes()
	{
	}
}
