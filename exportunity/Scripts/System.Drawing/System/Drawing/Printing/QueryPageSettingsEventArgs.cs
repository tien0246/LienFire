namespace System.Drawing.Printing;

public class QueryPageSettingsEventArgs : PrintEventArgs
{
	private PageSettings _pageSettings;

	internal bool PageSettingsChanged;

	public PageSettings PageSettings
	{
		get
		{
			PageSettingsChanged = true;
			return _pageSettings;
		}
		set
		{
			if (value == null)
			{
				value = new PageSettings();
			}
			_pageSettings = value;
			PageSettingsChanged = true;
		}
	}

	public QueryPageSettingsEventArgs(PageSettings pageSettings)
	{
		_pageSettings = pageSettings;
	}
}
