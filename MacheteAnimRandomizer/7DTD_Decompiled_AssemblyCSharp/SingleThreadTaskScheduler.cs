using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine.Profiling;

// Token: 0x02001212 RID: 4626
public sealed class SingleThreadTaskScheduler : TaskScheduler, IDisposable
{
	// Token: 0x0600904B RID: 36939 RVA: 0x00398E04 File Offset: 0x00397004
	public SingleThreadTaskScheduler(string threadGroupName, string threadName)
	{
		this.m_threadGroupName = threadGroupName;
		this.m_threadName = threadName;
		this.m_taskThread = new Thread(new ThreadStart(this.TaskThread))
		{
			Name = this.m_threadName,
			IsBackground = true
		};
		this.m_taskCancellationSource = new CancellationTokenSource();
		this.m_taskFactory = new TaskFactory(this.m_taskCancellationSource.Token, TaskCreationOptions.None, TaskContinuationOptions.None, this);
		this.m_running = true;
		this.m_taskThread.Start();
	}

	// Token: 0x0600904C RID: 36940 RVA: 0x00398E9C File Offset: 0x0039709C
	public void Dispose()
	{
		this.m_taskFactory = null;
		CancellationTokenSource taskCancellationSource = this.m_taskCancellationSource;
		if (taskCancellationSource != null)
		{
			taskCancellationSource.Cancel();
		}
		this.m_taskCancellationSource = null;
		this.m_running = false;
		AutoResetEvent waitHandle = this.m_waitHandle;
		if (waitHandle != null)
		{
			waitHandle.Set();
		}
		Thread taskThread = this.m_taskThread;
		if (taskThread != null)
		{
			taskThread.Interrupt();
		}
		Thread taskThread2 = this.m_taskThread;
		if (taskThread2 != null)
		{
			taskThread2.Join();
		}
		this.m_taskThread = null;
		AutoResetEvent waitHandle2 = this.m_waitHandle;
		if (waitHandle2 != null)
		{
			waitHandle2.Close();
		}
		this.m_waitHandle = null;
	}

	// Token: 0x17000EFA RID: 3834
	// (get) Token: 0x0600904D RID: 36941 RVA: 0x00398F22 File Offset: 0x00397122
	public Thread Thread
	{
		get
		{
			return this.m_taskThread;
		}
	}

	// Token: 0x17000EFB RID: 3835
	// (get) Token: 0x0600904E RID: 36942 RVA: 0x00398F2A File Offset: 0x0039712A
	public TaskFactory Factory
	{
		get
		{
			return this.m_taskFactory;
		}
	}

	// Token: 0x17000EFC RID: 3836
	// (get) Token: 0x0600904F RID: 36943 RVA: 0x00398F32 File Offset: 0x00397132
	public bool IsCurrentThread
	{
		get
		{
			return Thread.CurrentThread == this.m_taskThread;
		}
	}

	// Token: 0x06009050 RID: 36944 RVA: 0x00398F41 File Offset: 0x00397141
	public Task ExecuteNoWait(Action task)
	{
		if (!this.IsCurrentThread)
		{
			return this.m_taskFactory.StartNew(task);
		}
		task();
		return Task.CompletedTask;
	}

	// Token: 0x06009051 RID: 36945 RVA: 0x00398F63 File Offset: 0x00397163
	public Task<T> ExecuteNoWait<T>(Func<T> task)
	{
		if (!this.IsCurrentThread)
		{
			return this.m_taskFactory.StartNew<T>(task);
		}
		return Task.FromResult<T>(task());
	}

	// Token: 0x06009052 RID: 36946 RVA: 0x00398F85 File Offset: 0x00397185
	public void ExecuteAndWait(Action task)
	{
		if (this.IsCurrentThread)
		{
			task();
			return;
		}
		this.m_taskFactory.StartNew(task).Wait();
	}

	// Token: 0x06009053 RID: 36947 RVA: 0x00398FA7 File Offset: 0x003971A7
	public T ExecuteAndWait<T>(Func<T> task)
	{
		if (!this.IsCurrentThread)
		{
			return this.m_taskFactory.StartNew<T>(task).Result;
		}
		return task();
	}

	// Token: 0x06009054 RID: 36948 RVA: 0x00398FCC File Offset: 0x003971CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void TaskThread()
	{
		Log.Out("[" + this.m_threadGroupName + "] Started SingleThreadTaskScheduler Thread: " + this.m_threadName);
		try
		{
			while (this.m_running)
			{
				try
				{
					this.<TaskThread>g__ProcessTasks|24_0();
				}
				finally
				{
				}
				try
				{
					this.m_waitHandle.WaitOne();
				}
				finally
				{
				}
			}
		}
		catch (ThreadInterruptedException)
		{
			Log.Out("[" + this.m_threadGroupName + "] Interrupted SingleThreadTaskScheduler Thread: " + this.m_threadName);
		}
		finally
		{
			Profiler.EndThreadProfiling();
			Log.Out("[" + this.m_threadGroupName + "] Stopped SingleThreadTaskScheduler Thread: " + this.m_threadName);
		}
	}

	// Token: 0x17000EFD RID: 3837
	// (get) Token: 0x06009055 RID: 36949 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int MaximumConcurrencyLevel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06009056 RID: 36950 RVA: 0x0039909C File Offset: 0x0039729C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{
		return Thread.CurrentThread == this.m_taskThread && (!taskWasPreviouslyQueued || this.TryDequeue(task)) && base.TryExecuteTask(task);
	}

	// Token: 0x06009057 RID: 36951 RVA: 0x003990C4 File Offset: 0x003972C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void QueueTask(Task task)
	{
		LinkedList<Task> tasks = this.m_tasks;
		lock (tasks)
		{
			this.m_tasks.AddLast(task);
			this.m_waitHandle.Set();
		}
	}

	// Token: 0x06009058 RID: 36952 RVA: 0x00399118 File Offset: 0x00397318
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool TryDequeue(Task task)
	{
		LinkedList<Task> tasks = this.m_tasks;
		bool result;
		lock (tasks)
		{
			result = this.m_tasks.Remove(task);
		}
		return result;
	}

	// Token: 0x06009059 RID: 36953 RVA: 0x00399160 File Offset: 0x00397360
	[PublicizedFrom(EAccessModifier.Protected)]
	public override IEnumerable<Task> GetScheduledTasks()
	{
		bool flag = false;
		IEnumerable<Task> tasks;
		try
		{
			Monitor.TryEnter(this.m_tasks, ref flag);
			if (!flag)
			{
				throw new NotSupportedException();
			}
			tasks = this.m_tasks;
		}
		finally
		{
			if (flag)
			{
				Monitor.Exit(this.m_tasks);
			}
		}
		return tasks;
	}

	// Token: 0x0600905B RID: 36955 RVA: 0x003991F0 File Offset: 0x003973F0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <TaskThread>g__ProcessTasks|24_0()
	{
		for (;;)
		{
			Task value;
			try
			{
				LinkedList<Task> tasks = this.m_tasks;
				lock (tasks)
				{
					if (this.m_tasks.Count == 0)
					{
						break;
					}
					value = this.m_tasks.First.Value;
					this.m_tasks.RemoveFirst();
				}
			}
			finally
			{
			}
			try
			{
				base.TryExecuteTask(value);
				continue;
			}
			finally
			{
			}
			break;
		}
	}

	// Token: 0x04006F36 RID: 28470
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_ProcessTasksMarker = new ProfilerMarker("SingleThreadTaskScheduler.ProcessTasks");

	// Token: 0x04006F37 RID: 28471
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_WaitForTasksMarker = new ProfilerMarker("SingleThreadTaskScheduler.WaitForTasks");

	// Token: 0x04006F38 RID: 28472
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_TryGetTaskMarker = new ProfilerMarker("SingleThreadTaskScheduler.TryGetTask");

	// Token: 0x04006F39 RID: 28473
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_ExecuteTaskMarker = new ProfilerMarker("SingleThreadTaskScheduler.ExecuteTask");

	// Token: 0x04006F3A RID: 28474
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string m_threadGroupName;

	// Token: 0x04006F3B RID: 28475
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string m_threadName;

	// Token: 0x04006F3C RID: 28476
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly LinkedList<Task> m_tasks = new LinkedList<Task>();

	// Token: 0x04006F3D RID: 28477
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_running;

	// Token: 0x04006F3E RID: 28478
	[PublicizedFrom(EAccessModifier.Private)]
	public Thread m_taskThread;

	// Token: 0x04006F3F RID: 28479
	[PublicizedFrom(EAccessModifier.Private)]
	public AutoResetEvent m_waitHandle = new AutoResetEvent(false);

	// Token: 0x04006F40 RID: 28480
	[PublicizedFrom(EAccessModifier.Private)]
	public CancellationTokenSource m_taskCancellationSource;

	// Token: 0x04006F41 RID: 28481
	[PublicizedFrom(EAccessModifier.Private)]
	public TaskFactory m_taskFactory;
}
