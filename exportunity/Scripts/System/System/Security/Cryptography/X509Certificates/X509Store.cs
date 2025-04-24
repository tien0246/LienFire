using System.Security.Permissions;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509Store : IDisposable
{
	private string _name;

	private StoreLocation _location;

	private X509Certificate2Collection list;

	private OpenFlags _flags;

	private Mono.Security.X509.X509Store store;

	public X509Certificate2Collection Certificates
	{
		get
		{
			if (list == null)
			{
				list = new X509Certificate2Collection();
			}
			else if (store == null)
			{
				list.Clear();
			}
			return list;
		}
	}

	public StoreLocation Location => _location;

	public string Name => _name;

	private X509Stores Factory
	{
		get
		{
			if (_location == StoreLocation.CurrentUser)
			{
				return X509StoreManager.CurrentUser;
			}
			return X509StoreManager.LocalMachine;
		}
	}

	public bool IsOpen => store != null;

	private bool IsReadOnly => (_flags & OpenFlags.ReadWrite) == 0;

	internal Mono.Security.X509.X509Store Store => store;

	[System.MonoTODO("Mono's stores are fully managed. Always returns IntPtr.Zero.")]
	public IntPtr StoreHandle => IntPtr.Zero;

	public X509Store()
		: this("MY", StoreLocation.CurrentUser)
	{
	}

	public X509Store(string storeName)
		: this(storeName, StoreLocation.CurrentUser)
	{
	}

	public X509Store(StoreName storeName)
		: this(storeName, StoreLocation.CurrentUser)
	{
	}

	public X509Store(StoreLocation storeLocation)
		: this("MY", storeLocation)
	{
	}

	public X509Store(StoreName storeName, StoreLocation storeLocation)
	{
		if (storeName < StoreName.AddressBook || storeName > StoreName.TrustedPublisher)
		{
			throw new ArgumentException("storeName");
		}
		if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
		{
			throw new ArgumentException("storeLocation");
		}
		if (storeName == StoreName.CertificateAuthority)
		{
			_name = "CA";
		}
		else
		{
			_name = storeName.ToString();
		}
		_location = storeLocation;
	}

	public X509Store(StoreName storeName, StoreLocation storeLocation, OpenFlags openFlags)
		: this(storeName, storeLocation)
	{
		_flags = openFlags;
	}

	public X509Store(string storeName, StoreLocation storeLocation, OpenFlags openFlags)
		: this(storeName, storeLocation)
	{
		_flags = openFlags;
	}

	[System.MonoTODO("Mono's stores are fully managed. All handles are invalid.")]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	public X509Store(IntPtr storeHandle)
	{
		if (storeHandle == IntPtr.Zero)
		{
			throw new ArgumentNullException("storeHandle");
		}
		throw new CryptographicException("Invalid handle.");
	}

	public X509Store(string storeName, StoreLocation storeLocation)
	{
		if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
		{
			throw new ArgumentException("storeLocation");
		}
		_name = storeName;
		_location = storeLocation;
	}

	public void Add(X509Certificate2 certificate)
	{
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		if (!IsOpen)
		{
			throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
		}
		if (IsReadOnly)
		{
			throw new CryptographicException(global::Locale.GetText("Store is read-only."));
		}
		if (!Exists(certificate))
		{
			try
			{
				store.Import(new Mono.Security.X509.X509Certificate(certificate.RawData));
			}
			finally
			{
				Certificates.Add(certificate);
			}
		}
	}

	[System.MonoTODO("Method isn't transactional (like documented)")]
	public void AddRange(X509Certificate2Collection certificates)
	{
		if (certificates == null)
		{
			throw new ArgumentNullException("certificates");
		}
		if (certificates.Count == 0)
		{
			return;
		}
		if (!IsOpen)
		{
			throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
		}
		if (IsReadOnly)
		{
			throw new CryptographicException(global::Locale.GetText("Store is read-only."));
		}
		X509Certificate2Enumerator enumerator = certificates.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Certificate2 current = enumerator.Current;
			if (!Exists(current))
			{
				try
				{
					store.Import(new Mono.Security.X509.X509Certificate(current.RawData));
				}
				finally
				{
					Certificates.Add(current);
				}
			}
		}
	}

	public void Close()
	{
		store = null;
		if (list != null)
		{
			list.Clear();
		}
	}

	public void Dispose()
	{
		Close();
	}

	public void Open(OpenFlags flags)
	{
		if (string.IsNullOrEmpty(_name))
		{
			throw new CryptographicException(global::Locale.GetText("Invalid store name (null or empty)."));
		}
		string storeName = ((!(_name == "Root")) ? _name : "Trust");
		bool create = (flags & OpenFlags.OpenExistingOnly) != OpenFlags.OpenExistingOnly;
		store = Factory.Open(storeName, create);
		if (store == null)
		{
			throw new CryptographicException(global::Locale.GetText("Store {0} doesn't exists.", _name));
		}
		_flags = flags;
		foreach (Mono.Security.X509.X509Certificate certificate in store.Certificates)
		{
			X509Certificate2 x509Certificate = new X509Certificate2(certificate.RawData);
			x509Certificate.Impl.PrivateKey = certificate.RSA;
			Certificates.Add(x509Certificate);
		}
	}

	public void Remove(X509Certificate2 certificate)
	{
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		if (!IsOpen)
		{
			throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
		}
		if (!Exists(certificate))
		{
			return;
		}
		if (IsReadOnly)
		{
			throw new CryptographicException(global::Locale.GetText("Store is read-only."));
		}
		try
		{
			store.Remove(new Mono.Security.X509.X509Certificate(certificate.RawData));
		}
		finally
		{
			Certificates.Remove(certificate);
		}
	}

	[System.MonoTODO("Method isn't transactional (like documented)")]
	public void RemoveRange(X509Certificate2Collection certificates)
	{
		if (certificates == null)
		{
			throw new ArgumentNullException("certificates");
		}
		if (certificates.Count == 0)
		{
			return;
		}
		if (!IsOpen)
		{
			throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
		}
		bool flag = false;
		X509Certificate2Enumerator enumerator = certificates.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Certificate2 current = enumerator.Current;
			if (Exists(current))
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		if (IsReadOnly)
		{
			throw new CryptographicException(global::Locale.GetText("Store is read-only."));
		}
		try
		{
			enumerator = certificates.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Certificate2 current2 = enumerator.Current;
				store.Remove(new Mono.Security.X509.X509Certificate(current2.RawData));
			}
		}
		finally
		{
			Certificates.RemoveRange(certificates);
		}
	}

	private bool Exists(X509Certificate2 certificate)
	{
		if (store == null || list == null || certificate == null)
		{
			return false;
		}
		X509Certificate2Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Certificate2 current = enumerator.Current;
			if (certificate.Equals(current))
			{
				return true;
			}
		}
		return false;
	}
}
