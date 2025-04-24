using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc;

public sealed class OdbcConnectionStringBuilder : DbConnectionStringBuilder
{
	private enum Keywords
	{
		Dsn = 0,
		Driver = 1
	}

	private static readonly string[] s_validKeywords;

	private static readonly Dictionary<string, Keywords> s_keywords;

	private string[] _knownKeywords;

	private string _dsn = "";

	private string _driver = "";

	public override object this[string keyword]
	{
		get
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			if (s_keywords.TryGetValue(keyword, out var value))
			{
				return GetAt(value);
			}
			return base[keyword];
		}
		set
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			if (value != null)
			{
				if (s_keywords.TryGetValue(keyword, out var value2))
				{
					switch (value2)
					{
					case Keywords.Driver:
						Driver = ConvertToString(value);
						break;
					case Keywords.Dsn:
						Dsn = ConvertToString(value);
						break;
					default:
						throw ADP.KeywordNotSupported(keyword);
					}
				}
				else
				{
					base[keyword] = value;
					ClearPropertyDescriptors();
					_knownKeywords = null;
				}
			}
			else
			{
				Remove(keyword);
			}
		}
	}

	[DisplayName("Driver")]
	public string Driver
	{
		get
		{
			return _driver;
		}
		set
		{
			SetValue("Driver", value);
			_driver = value;
		}
	}

	[DisplayName("Dsn")]
	public string Dsn
	{
		get
		{
			return _dsn;
		}
		set
		{
			SetValue("Dsn", value);
			_dsn = value;
		}
	}

	public override ICollection Keys
	{
		get
		{
			string[] array = _knownKeywords;
			if (array == null)
			{
				array = s_validKeywords;
				int num = 0;
				foreach (string key in base.Keys)
				{
					bool flag = true;
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i] == key)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						num++;
					}
				}
				if (0 < num)
				{
					string[] array3 = new string[array.Length + num];
					array.CopyTo(array3, 0);
					int num2 = array.Length;
					foreach (string key2 in base.Keys)
					{
						bool flag2 = true;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i] == key2)
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							array3[num2++] = key2;
						}
					}
					array = array3;
				}
				_knownKeywords = array;
			}
			return new ReadOnlyCollection<string>(array);
		}
	}

	static OdbcConnectionStringBuilder()
	{
		string[] array = new string[2];
		array[1] = "Driver";
		array[0] = "Dsn";
		s_validKeywords = array;
		s_keywords = new Dictionary<string, Keywords>(2, StringComparer.OrdinalIgnoreCase)
		{
			{
				"Driver",
				Keywords.Driver
			},
			{
				"Dsn",
				Keywords.Dsn
			}
		};
	}

	public OdbcConnectionStringBuilder()
		: this(null)
	{
	}

	public OdbcConnectionStringBuilder(string connectionString)
		: base(useOdbcRules: true)
	{
		if (!string.IsNullOrEmpty(connectionString))
		{
			base.ConnectionString = connectionString;
		}
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < s_validKeywords.Length; i++)
		{
			Reset((Keywords)i);
		}
		_knownKeywords = s_validKeywords;
	}

	public override bool ContainsKey(string keyword)
	{
		ADP.CheckArgumentNull(keyword, "keyword");
		if (!s_keywords.ContainsKey(keyword))
		{
			return base.ContainsKey(keyword);
		}
		return true;
	}

	private static string ConvertToString(object value)
	{
		return DbConnectionStringBuilderUtil.ConvertToString(value);
	}

	private object GetAt(Keywords index)
	{
		return index switch
		{
			Keywords.Driver => Driver, 
			Keywords.Dsn => Dsn, 
			_ => throw ADP.KeywordNotSupported(s_validKeywords[(int)index]), 
		};
	}

	public override bool Remove(string keyword)
	{
		ADP.CheckArgumentNull(keyword, "keyword");
		if (base.Remove(keyword))
		{
			if (s_keywords.TryGetValue(keyword, out var value))
			{
				Reset(value);
			}
			else
			{
				ClearPropertyDescriptors();
				_knownKeywords = null;
			}
			return true;
		}
		return false;
	}

	private void Reset(Keywords index)
	{
		switch (index)
		{
		case Keywords.Driver:
			_driver = "";
			break;
		case Keywords.Dsn:
			_dsn = "";
			break;
		default:
			throw ADP.KeywordNotSupported(s_validKeywords[(int)index]);
		}
	}

	private void SetValue(string keyword, string value)
	{
		ADP.CheckArgumentNull(value, keyword);
		base[keyword] = value;
	}

	public override bool TryGetValue(string keyword, out object value)
	{
		ADP.CheckArgumentNull(keyword, "keyword");
		if (s_keywords.TryGetValue(keyword, out var value2))
		{
			value = GetAt(value2);
			return true;
		}
		return base.TryGetValue(keyword, out value);
	}
}
