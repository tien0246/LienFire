using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing;

[Serializable]
public class InvalidPrinterException : SystemException
{
	private PrinterSettings _settings;

	protected InvalidPrinterException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_settings = (PrinterSettings)info.GetValue("settings", typeof(PrinterSettings));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("settings", _settings);
	}

	public InvalidPrinterException(PrinterSettings settings)
		: base(GenerateMessage(settings))
	{
		_settings = settings;
	}

	private static string GenerateMessage(PrinterSettings settings)
	{
		if (settings.IsDefaultPrinter)
		{
			return global::SR.Format("No printers are installed.");
		}
		try
		{
			return global::SR.Format("Settings to access printer '{0}' are not valid.", settings.PrinterName);
		}
		catch (SecurityException)
		{
			return global::SR.Format("Settings to access printer '{0}' are not valid.", global::SR.Format("(printer name protected due to security restrictions)"));
		}
	}
}
