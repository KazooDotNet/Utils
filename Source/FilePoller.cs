using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace KazooDotNet.Utils
{
    public class FilePoller : IDisposable
	{
		public EventEmitter<object, FileSystemEventArgs> Created { get; }
		public EventEmitter<object, FileSystemEventArgs> Changed { get; }
		public EventEmitter<object, FileSystemEventArgs> Deleted { get; }

		private readonly ConcurrentDictionary<string, (DateTime, long)> _files;

		private string[] _extensions;
		public string[] Extensions
		{
			get => _extensions;
			set
			{
				if (value == null)
					_extensions = null;
				else
					_extensions = value.Where(v => v != null).Select(v =>
					{
						v = v.ToLowerInvariant();
						return v.StartsWith(".") ? v : $".{v}";
					}).ToArray();
			}
		}
		public Func<string, bool> Filter { get; set; }

		private bool _includeSubs;
		public bool IncludeSubdirectories
		{
			get => _includeSubs;
			set
			{
				_includeSubs = value;
				_watcher.IncludeSubdirectories = value;
			}
		}

		private bool _enabled;

		public bool Enabled
		{
			get => _enabled;
			set
			{
				if (value && !_enabled)
					Rescan(null, null);
				_enabled = value;
				_watcher.EnableRaisingEvents = value;


			}
		}

		private FileSystemWatcher _watcher;
		private Timer _timer;
		private bool _polling;
		private string _path;


		public FilePoller(string path, int rescanInterval)
		{
			Created = new EventEmitter<object, FileSystemEventArgs>();
			Changed = new EventEmitter<object, FileSystemEventArgs>();
			Deleted = new EventEmitter<object, FileSystemEventArgs>();

			_files = new ConcurrentDictionary<string, (DateTime, long)>();
			_path = path;
			_watcher = new FileSystemWatcher
			{
				Path = path,
				EnableRaisingEvents = false
			};
			_watcher.Created += FileCreated;
			_watcher.Changed += FileChanged;
			_watcher.Deleted += FileDeleted;
			_timer = new Timer(rescanInterval)
			{
				AutoReset = true,
				Enabled = true
			};
			_timer.Elapsed += Rescan;
		}


		public void Dispose()
		{
			_timer.Dispose();
			_watcher?.Dispose();
		}

		private bool IsMatch(string filename)
		{
			if (Filter != null)
				return filename != null && Filter.Invoke(filename);
			if (Extensions != null)
				return filename != null && Enumerable.Contains(Extensions, Path.GetExtension(filename).ToLowerInvariant());
			return true;
		}

		private void FileDeleted(object sender, FileSystemEventArgs e)
		{
			if (IsMatch(e.Name))
			{
				_files.TryRemove(e.FullPath, out _);
				Deleted.Invoke(sender, e);
			}
		}

		private void FileChanged(object sender, FileSystemEventArgs e)
		{
			if (IsMatch(e.Name))
			{
				UpdateInfo(e.FullPath);
				Changed.Invoke(sender, e);
			}

		}

		private void FileCreated(object sender, FileSystemEventArgs e)
		{
			if (IsMatch(e.Name))
			{
				UpdateInfo(e.FullPath);
				Created.Invoke(sender, e);
			}
		}

		private void UpdateInfo(string path, FileInfo info = null)
		{
			if (info == null) info = new FileInfo(path);
			_files[path] = (info.LastWriteTimeUtc, info.Length);
		}

		private void Rescan(object o, ElapsedEventArgs e)
		{
			if (_polling) return;
			try
			{
				_polling = true;
				var list = DirSearch(o, _path);
				foreach (var key in _files.Keys)
					if (!list.Contains(key))
					{
						Deleted.Invoke(o,
							new FileSystemEventArgs(WatcherChangeTypes.Deleted, Path.GetDirectoryName(key), Path.GetFileName(key)));
						_files.TryRemove(key, out _);
					}
			}
			finally
			{
				_polling = false;
			}

		}

		private List<string>  DirSearch(object o, string dir, List<string> list = null)
		{
			if (list == null) list = new List<string>();

			foreach (var f in Directory.EnumerateFiles(dir).Where(IsMatch))
			{
				list.Add(f);
				var found = _files.TryGetValue(f, out var tf);
				var info = new FileInfo(f);
				if (found)
				{
					if (tf.Item1 == info.LastWriteTimeUtc && tf.Item2 == info.Length) continue;
					Changed.Invoke(o, new FileSystemEventArgs(WatcherChangeTypes.Changed, dir, Path.GetFileName(f)));
				}
				else
				{
					Created.Invoke(o, new FileSystemEventArgs(WatcherChangeTypes.Created, dir, Path.GetFileName(f)));
				}
				UpdateInfo(f, info);
			}
			if (_includeSubs)
				foreach (var d in Directory.GetDirectories(dir))
					DirSearch(o, d, list);
			return list;
		}


	}
}
