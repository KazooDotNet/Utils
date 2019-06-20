using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace KazooDotNet.Utils
{	
	
	public class QueueWorker<T>
	{
		
		private bool _running;
		private Task _workerTask;
		private readonly Func<T, CancellationToken, Task> _action;
		private CancellationTokenSource _cancelSource;
		private int _requestsThisInterval;
		private int _requestsPerInterval;
		private Timer _timer;
		
		public int WorkerCount { get; set; }
		public Action<T, Exception> ErrorCallback { private get; set; }
		public Action<T> SuccessCallback { private get; set; }
		public int RequestsPerInterval { get; set; }
		public TimeSpan? Interval { get; set; }
		public ConcurrentQueue<T> Queue { get; }

		public QueueWorker(Func<T, CancellationToken, Task> action) : this(new ConcurrentQueue<T>(), action)
		{
		}

		public QueueWorker(ConcurrentQueue<T> queue, Func<T, CancellationToken, Task> action)
		{
			Queue = queue;
			_action = action;
		}

		public QueueWorker<T> Start(int requestsThisInterval = 0)
		{
			if (_cancelSource != null && !_cancelSource.IsCancellationRequested && _workerTask != null &&
			    !_workerTask.IsCompleted) return this;
			if (RequestsPerInterval > 0 && Interval != null)
			{
				_requestsPerInterval = RequestsPerInterval;
				_timer?.Stop();
				_timer?.Dispose();
				_timer = new Timer(Interval.Value.TotalMilliseconds)
				{
					AutoReset = true,
					Enabled = true
				};
				_requestsThisInterval = requestsThisInterval;
				_timer.Elapsed += (sender, evt) => _requestsThisInterval = 0;
			}
			else
			{
				_requestsThisInterval = -1;
				_requestsPerInterval = 0;
			}
			_running = true;
			_cancelSource = new CancellationTokenSource();
			_workerTask = RunWorkersAsync(_cancelSource.Token);
			return this;
		}

		public void Stop()
		{
			_cancelSource?.Cancel();
		}

		public Task StopOnComplete()
		{
			if (_workerTask == null)
				return Task.CompletedTask;
			_running = false;
			return _workerTask;
		}


		private async Task RunWorkersAsync(CancellationToken cancelToken)
		{
			var runningTasks = new List<(Task Task, T Object)>();
			while ((_running || Queue.Count > 0) && !cancelToken.IsCancellationRequested)
			{
				if (Queue.Count == 0)
					await Task.Delay(100, cancelToken);
				while (runningTasks.Count < WorkerCount && Queue.Count > 0 && _requestsThisInterval < _requestsPerInterval)
				{
					if (!Queue.TryDequeue(out var obj)) continue;
					runningTasks.Add((_action.Invoke(obj, cancelToken), obj));
					if (_requestsPerInterval > 0)
						_requestsThisInterval++;
				}

				var tasks = new Task[runningTasks.Count];
				var rti = 0;
				foreach (var rTask in runningTasks)
				{
					tasks[rti] = rTask.Task;
					rti++;
				}
				try
				{
					if (tasks.Length > 0)
						await Task.WhenAny(tasks);
					else
						await Task.Delay(100, cancelToken);
				}
				finally
				{
					for (var i = runningTasks.Count - 1; i > -1; i--)
					{
						var (task, o) = runningTasks[i];
                        if (!task.IsFaulted && task.IsCompleted)
							SuccessCallback?.Invoke(o);
						else if (task.IsFaulted)
							ErrorCallback?.Invoke(o, task.Exception);
						if (task.IsCompleted)
							runningTasks.RemoveAt(i);
					}
				}
			}
		}
		
	}
}
