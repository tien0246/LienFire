using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO;

[DefaultEvent("Changed")]
[IODescription("")]
public class FileSystemWatcher : Component, ISupportInitialize
{
	private enum EventType
	{
		FileSystemEvent = 0,
		ErrorEvent = 1,
		RenameEvent = 2
	}

	private bool inited;

	private bool start_requested;

	private bool enableRaisingEvents;

	private string filter;

	private bool includeSubdirectories;

	private int internalBufferSize;

	private NotifyFilters notifyFilter;

	private string path;

	private string fullpath;

	private ISynchronizeInvoke synchronizingObject;

	private WaitForChangedResult lastData;

	private bool waiting;

	private SearchPattern2 pattern;

	private bool disposed;

	private string mangledFilter;

	private IFileWatcher watcher;

	private object watcher_handle;

	private static object lockobj = new object();

	internal bool Waiting
	{
		get
		{
			return waiting;
		}
		set
		{
			waiting = value;
		}
	}

	internal string MangledFilter
	{
		get
		{
			if (filter != "*.*")
			{
				return filter;
			}
			if (mangledFilter != null)
			{
				return mangledFilter;
			}
			return "*.*";
		}
	}

	internal SearchPattern2 Pattern
	{
		get
		{
			if (pattern == null)
			{
				if (watcher?.GetType() == typeof(KeventWatcher))
				{
					pattern = new SearchPattern2(MangledFilter, ignore: true);
				}
				else
				{
					pattern = new SearchPattern2(MangledFilter);
				}
			}
			return pattern;
		}
	}

	internal string FullPath
	{
		get
		{
			if (fullpath == null)
			{
				if (path == null || path == "")
				{
					fullpath = Environment.CurrentDirectory;
				}
				else
				{
					fullpath = System.IO.Path.GetFullPath(path);
				}
			}
			return fullpath;
		}
	}

	[DefaultValue(false)]
	[IODescription("Flag to indicate if this instance is active")]
	public bool EnableRaisingEvents
	{
		get
		{
			return enableRaisingEvents;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
			start_requested = true;
			if (inited && value != enableRaisingEvents)
			{
				enableRaisingEvents = value;
				if (value)
				{
					Start();
					return;
				}
				Stop();
				start_requested = false;
			}
		}
	}

	[SettingsBindable(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("*.*")]
	[IODescription("File name filter pattern")]
	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			if (value == null || value == "")
			{
				value = "*";
			}
			if (!string.Equals(filter, value, PathInternal.StringComparison))
			{
				filter = ((value == "*.*") ? "*" : value);
				pattern = null;
				mangledFilter = null;
			}
		}
	}

	[DefaultValue(false)]
	[IODescription("Flag to indicate we want to watch subdirectories")]
	public bool IncludeSubdirectories
	{
		get
		{
			return includeSubdirectories;
		}
		set
		{
			if (includeSubdirectories != value)
			{
				includeSubdirectories = value;
				if (value && enableRaisingEvents)
				{
					Stop();
					Start();
				}
			}
		}
	}

	[DefaultValue(8192)]
	[Browsable(false)]
	public int InternalBufferSize
	{
		get
		{
			return internalBufferSize;
		}
		set
		{
			if (internalBufferSize != value)
			{
				if (value < 4096)
				{
					value = 4096;
				}
				internalBufferSize = value;
				if (enableRaisingEvents)
				{
					Stop();
					Start();
				}
			}
		}
	}

	[IODescription("Flag to indicate which change event we want to monitor")]
	[DefaultValue(NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite)]
	public NotifyFilters NotifyFilter
	{
		get
		{
			return notifyFilter;
		}
		set
		{
			if (notifyFilter != value)
			{
				notifyFilter = value;
				if (enableRaisingEvents)
				{
					Stop();
					Start();
				}
			}
		}
	}

	[SettingsBindable(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[IODescription("The directory to monitor")]
	[Editor("System.Diagnostics.Design.FSWPathEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("")]
	public string Path
	{
		get
		{
			return path;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
			value = ((value == null) ? string.Empty : value);
			if (!string.Equals(path, value, PathInternal.StringComparison))
			{
				bool flag = false;
				Exception ex = null;
				try
				{
					flag = Directory.Exists(value);
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (ex != null)
				{
					throw new ArgumentException(global::SR.Format("The directory name {0} is invalid.", value), "Path");
				}
				if (!flag)
				{
					throw new ArgumentException(global::SR.Format("The directory name '{0}' does not exist.", value), "Path");
				}
				path = value;
				fullpath = null;
				if (enableRaisingEvents)
				{
					Stop();
					Start();
				}
			}
		}
	}

	[Browsable(false)]
	public override ISite Site
	{
		get
		{
			return base.Site;
		}
		set
		{
			base.Site = value;
			if (Site != null && Site.DesignMode)
			{
				EnableRaisingEvents = true;
			}
		}
	}

	[Browsable(false)]
	[DefaultValue(null)]
	[IODescription("The object used to marshal the event handler calls resulting from a directory change")]
	public ISynchronizeInvoke SynchronizingObject
	{
		get
		{
			return synchronizingObject;
		}
		set
		{
			synchronizingObject = value;
		}
	}

	[IODescription("Occurs when a file/directory change matches the filter")]
	public event FileSystemEventHandler Changed;

	[IODescription("Occurs when a file/directory creation matches the filter")]
	public event FileSystemEventHandler Created;

	[IODescription("Occurs when a file/directory deletion matches the filter")]
	public event FileSystemEventHandler Deleted;

	[Browsable(false)]
	public event ErrorEventHandler Error;

	[IODescription("Occurs when a file/directory rename matches the filter")]
	public event RenamedEventHandler Renamed;

	public FileSystemWatcher()
	{
		notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
		enableRaisingEvents = false;
		filter = "*";
		includeSubdirectories = false;
		internalBufferSize = 8192;
		path = "";
		InitWatcher();
	}

	public FileSystemWatcher(string path)
		: this(path, "*")
	{
	}

	public FileSystemWatcher(string path, string filter)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		if (path == string.Empty)
		{
			throw new ArgumentException("Empty path", "path");
		}
		if (!Directory.Exists(path))
		{
			throw new ArgumentException("Directory does not exist", "path");
		}
		inited = false;
		start_requested = false;
		enableRaisingEvents = false;
		this.filter = filter;
		if (this.filter == "*.*")
		{
			this.filter = "*";
		}
		includeSubdirectories = false;
		internalBufferSize = 8192;
		notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
		this.path = path;
		synchronizingObject = null;
		InitWatcher();
	}

	[EnvironmentPermission(SecurityAction.Assert, Read = "MONO_MANAGED_WATCHER")]
	private void InitWatcher()
	{
		lock (lockobj)
		{
			if (watcher_handle != null)
			{
				return;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_MANAGED_WATCHER");
			int num = 0;
			bool flag = false;
			if (environmentVariable == null)
			{
				num = InternalSupportsFSW();
			}
			switch (num)
			{
			case 1:
				flag = DefaultWatcher.GetInstance(out watcher);
				watcher_handle = this;
				break;
			case 2:
				flag = FAMWatcher.GetInstance(out watcher, gamin: false);
				watcher_handle = this;
				break;
			case 3:
				flag = KeventWatcher.GetInstance(out watcher);
				watcher_handle = this;
				break;
			case 4:
				flag = FAMWatcher.GetInstance(out watcher, gamin: true);
				watcher_handle = this;
				break;
			case 6:
				flag = CoreFXFileSystemWatcherProxy.GetInstance(out watcher);
				watcher_handle = (watcher as CoreFXFileSystemWatcherProxy).NewWatcher(this);
				break;
			}
			if (num == 0 || !flag)
			{
				if (string.Compare(environmentVariable, "disabled", ignoreCase: true) == 0)
				{
					NullFileWatcher.GetInstance(out watcher);
				}
				else
				{
					DefaultWatcher.GetInstance(out watcher);
					watcher_handle = this;
				}
			}
			inited = true;
		}
	}

	[Conditional("DEBUG")]
	[Conditional("TRACE")]
	private void ShowWatcherInfo()
	{
		Console.WriteLine("Watcher implementation: {0}", (watcher != null) ? watcher.GetType().ToString() : "<none>");
	}

	public void BeginInit()
	{
		inited = false;
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			try
			{
				watcher?.StopDispatching(watcher_handle);
				watcher?.Dispose(watcher_handle);
			}
			catch (Exception)
			{
			}
			watcher_handle = null;
			watcher = null;
			disposed = true;
			base.Dispose(disposing);
			GC.SuppressFinalize(this);
		}
	}

	~FileSystemWatcher()
	{
		if (!disposed)
		{
			Dispose(disposing: false);
		}
	}

	public void EndInit()
	{
		inited = true;
		if (start_requested)
		{
			EnableRaisingEvents = true;
		}
	}

	private void RaiseEvent(Delegate ev, EventArgs arg, EventType evtype)
	{
		if (disposed || (object)ev == null)
		{
			return;
		}
		if (synchronizingObject == null)
		{
			Delegate[] invocationList = ev.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				switch (evtype)
				{
				case EventType.RenameEvent:
					((RenamedEventHandler)obj)(this, (RenamedEventArgs)arg);
					break;
				case EventType.ErrorEvent:
					((ErrorEventHandler)obj)(this, (ErrorEventArgs)arg);
					break;
				case EventType.FileSystemEvent:
					((FileSystemEventHandler)obj)(this, (FileSystemEventArgs)arg);
					break;
				}
			}
		}
		else
		{
			synchronizingObject.BeginInvoke(ev, new object[2] { this, arg });
		}
	}

	protected void OnChanged(FileSystemEventArgs e)
	{
		RaiseEvent(this.Changed, e, EventType.FileSystemEvent);
	}

	protected void OnCreated(FileSystemEventArgs e)
	{
		RaiseEvent(this.Created, e, EventType.FileSystemEvent);
	}

	protected void OnDeleted(FileSystemEventArgs e)
	{
		RaiseEvent(this.Deleted, e, EventType.FileSystemEvent);
	}

	protected void OnError(ErrorEventArgs e)
	{
		RaiseEvent(this.Error, e, EventType.ErrorEvent);
	}

	protected void OnRenamed(RenamedEventArgs e)
	{
		RaiseEvent(this.Renamed, e, EventType.RenameEvent);
	}

	public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
	{
		return WaitForChanged(changeType, -1);
	}

	public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
	{
		WaitForChangedResult result = default(WaitForChangedResult);
		bool flag = EnableRaisingEvents;
		if (!flag)
		{
			EnableRaisingEvents = true;
		}
		bool flag2;
		lock (this)
		{
			waiting = true;
			flag2 = Monitor.Wait(this, timeout);
			if (flag2)
			{
				result = lastData;
			}
		}
		EnableRaisingEvents = flag;
		if (!flag2)
		{
			result.TimedOut = true;
		}
		return result;
	}

	internal void DispatchErrorEvents(ErrorEventArgs args)
	{
		if (!disposed)
		{
			OnError(args);
		}
	}

	internal void DispatchEvents(FileAction act, string filename, ref RenamedEventArgs renamed)
	{
		if (disposed)
		{
			return;
		}
		if (waiting)
		{
			lastData = default(WaitForChangedResult);
		}
		switch (act)
		{
		case FileAction.Added:
			lastData.Name = filename;
			lastData.ChangeType = WatcherChangeTypes.Created;
			Task.Run(delegate
			{
				OnCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, path, filename));
			});
			break;
		case FileAction.Removed:
			lastData.Name = filename;
			lastData.ChangeType = WatcherChangeTypes.Deleted;
			Task.Run(delegate
			{
				OnDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, filename));
			});
			break;
		case FileAction.Modified:
			lastData.Name = filename;
			lastData.ChangeType = WatcherChangeTypes.Changed;
			Task.Run(delegate
			{
				OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Changed, path, filename));
			});
			break;
		case FileAction.RenamedOldName:
			if (renamed != null)
			{
				OnRenamed(renamed);
			}
			lastData.OldName = filename;
			lastData.ChangeType = WatcherChangeTypes.Renamed;
			renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, path, filename, "");
			break;
		case FileAction.RenamedNewName:
		{
			lastData.Name = filename;
			lastData.ChangeType = WatcherChangeTypes.Renamed;
			if (renamed == null)
			{
				renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, path, "", filename);
			}
			RenamedEventArgs renamed_ref = renamed;
			Task.Run(delegate
			{
				OnRenamed(renamed_ref);
			});
			renamed = null;
			break;
		}
		}
	}

	private void Start()
	{
		if (!disposed && watcher_handle != null)
		{
			watcher?.StartDispatching(watcher_handle);
		}
	}

	private void Stop()
	{
		if (!disposed && watcher_handle != null)
		{
			watcher?.StopDispatching(watcher_handle);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int InternalSupportsFSW();
}
