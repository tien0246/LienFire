using System.Security;

namespace System.Net;

public class NetworkCredential : ICredentials, ICredentialsByHost
{
	private string m_domain;

	private string m_userName;

	private SecureString m_password;

	public string UserName
	{
		get
		{
			return InternalGetUserName();
		}
		set
		{
			if (value == null)
			{
				m_userName = string.Empty;
			}
			else
			{
				m_userName = value;
			}
		}
	}

	public string Password
	{
		get
		{
			return InternalGetPassword();
		}
		set
		{
			m_password = UnsafeNclNativeMethods.SecureStringHelper.CreateSecureString(value);
		}
	}

	public SecureString SecurePassword
	{
		get
		{
			return InternalGetSecurePassword().Copy();
		}
		set
		{
			if (value == null)
			{
				m_password = new SecureString();
			}
			else
			{
				m_password = value.Copy();
			}
		}
	}

	public string Domain
	{
		get
		{
			return InternalGetDomain();
		}
		set
		{
			if (value == null)
			{
				m_domain = string.Empty;
			}
			else
			{
				m_domain = value;
			}
		}
	}

	public NetworkCredential()
		: this(string.Empty, string.Empty, string.Empty)
	{
	}

	public NetworkCredential(string userName, string password)
		: this(userName, password, string.Empty)
	{
	}

	public NetworkCredential(string userName, SecureString password)
		: this(userName, password, string.Empty)
	{
	}

	public NetworkCredential(string userName, string password, string domain)
	{
		UserName = userName;
		Password = password;
		Domain = domain;
	}

	public NetworkCredential(string userName, SecureString password, string domain)
	{
		UserName = userName;
		SecurePassword = password;
		Domain = domain;
	}

	internal string InternalGetUserName()
	{
		return m_userName;
	}

	internal string InternalGetPassword()
	{
		return UnsafeNclNativeMethods.SecureStringHelper.CreateString(m_password);
	}

	internal SecureString InternalGetSecurePassword()
	{
		return m_password;
	}

	internal string InternalGetDomain()
	{
		return m_domain;
	}

	internal string InternalGetDomainUserName()
	{
		string text = InternalGetDomain();
		if (text.Length != 0)
		{
			text += "\\";
		}
		return text + InternalGetUserName();
	}

	public NetworkCredential GetCredential(Uri uri, string authType)
	{
		return this;
	}

	public NetworkCredential GetCredential(string host, int port, string authenticationType)
	{
		return this;
	}
}
