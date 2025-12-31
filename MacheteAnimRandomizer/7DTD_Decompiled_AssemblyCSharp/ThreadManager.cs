using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x0200107D RID: 4221
public static class ThreadManager
{
	// Token: 0x140000F2 RID: 242
	// (add) Token: 0x0600857D RID: 34173 RVA: 0x00364EE0 File Offset: 0x003630E0
	// (remove) Token: 0x0600857E RID: 34174 RVA: 0x00364F14 File Offset: 0x00363114
	public static event Action UpdateEv;

	// Token: 0x140000F3 RID: 243
	// (add) Token: 0x0600857F RID: 34175 RVA: 0x00364F48 File Offset: 0x00363148
	// (remove) Token: 0x06008580 RID: 34176 RVA: 0x00364F7C File Offset: 0x0036317C
	public static event Action LateUpdateEv;

	// Token: 0x06008581 RID: 34177 RVA: 0x00364FAF File Offset: 0x003631AF
	public static void ReleaseTaskInfo(ThreadManager.TaskInfo _info)
	{
		if (((_info != null) ? _info.evStopped : null) != null)
		{
			_info.evStopped.Close();
			_info.evStopped = null;
		}
	}

	// Token: 0x06008582 RID: 34178 RVA: 0x00364FD4 File Offset: 0x003631D4
	[PublicizedFrom(EAccessModifier.Private)]
	public static ThreadManager.ThreadInfo startThread(string _name, ThreadManager.ThreadFunctionDelegate _threadDelegate, ThreadManager.ThreadFunctionDelegate _threadInit, ThreadManager.ThreadFunctionLoopDelegate _threadLoop, ThreadManager.ThreadFunctionEndDelegate _threadEnd, object _parameter, ThreadManager.ExitCallbackThread _exitCallback, bool _useRealThread = false, bool _isSilent = false)
	{
		ThreadManager.ThreadInfo threadInfo = new ThreadManager.ThreadInfo();
		threadInfo.parameter = _parameter;
		threadInfo.threadDelegate = _threadDelegate;
		threadInfo.threadInit = _threadInit;
		threadInfo.threadLoop = _threadLoop;
		threadInfo.threadEnd = _threadEnd;
		threadInfo.exitCallback = _exitCallback;
		threadInfo.isSilent = _isSilent;
		Dictionary<string, ThreadManager.ThreadInfo> activeThreads = ThreadManager.ActiveThreads;
		lock (activeThreads)
		{
			if (ThreadManager.ActiveThreads.ContainsKey(_name))
			{
				int num = 0;
				string text;
				do
				{
					num++;
					text = _name + num.ToString();
				}
				while (ThreadManager.ActiveThreads.ContainsKey(text));
				_name = text;
			}
			ThreadManager.ActiveThreads.Add(_name, threadInfo);
		}
		threadInfo.name = _name;
		if (_useRealThread)
		{
			Thread thread = new Thread(new ParameterizedThreadStart(ThreadManager.myThreadInvoke));
			thread.Name = _name;
			thread.Start(threadInfo);
			threadInfo.thread = thread;
		}
		else
		{
			ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ThreadManager.myThreadInvoke), threadInfo);
		}
		return threadInfo;
	}

	// Token: 0x06008583 RID: 34179 RVA: 0x003650D4 File Offset: 0x003632D4
	public static ThreadManager.ThreadInfo StartThread(string _name, ThreadManager.ThreadFunctionDelegate _threadDelegate, object _parameter = null, ThreadManager.ExitCallbackThread _exitCallback = null, bool _useRealThread = false, bool _isSilent = false)
	{
		return ThreadManager.startThread(_name, _threadDelegate, null, null, null, _parameter, _exitCallback, _useRealThread, _isSilent);
	}

	// Token: 0x06008584 RID: 34180 RVA: 0x003650F4 File Offset: 0x003632F4
	public static ThreadManager.ThreadInfo StartThread(string _name, ThreadManager.ThreadFunctionDelegate _threadInit, ThreadManager.ThreadFunctionLoopDelegate _threadLoop, ThreadManager.ThreadFunctionEndDelegate _threadEnd, object _parameter = null, ThreadManager.ExitCallbackThread _exitCallback = null, bool _useRealThread = false, bool _isSilent = false)
	{
		return ThreadManager.startThread(_name, null, _threadInit, _threadLoop, _threadEnd, _parameter, _exitCallback, _useRealThread, _isSilent);
	}

	// Token: 0x06008585 RID: 34181 RVA: 0x00365114 File Offset: 0x00363314
	[PublicizedFrom(EAccessModifier.Private)]
	public static void myThreadInvoke(object _threadInfo)
	{
		ThreadManager.ThreadInfo threadInfo = (ThreadManager.ThreadInfo)_threadInfo;
		CustomSampler.Create("ThreadDelegate", false);
		if (!threadInfo.isSilent)
		{
			Log.Out("Started thread " + threadInfo.name);
		}
		Exception e = null;
		try
		{
			if (threadInfo.threadDelegate != null)
			{
				threadInfo.threadDelegate(threadInfo);
			}
			else
			{
				if (threadInfo.threadInit != null)
				{
					threadInfo.threadInit(threadInfo);
				}
				bool exitForException = false;
				try
				{
					int num;
					do
					{
						num = threadInfo.threadLoop(threadInfo);
						if (num > 0)
						{
							Thread.Sleep(num);
						}
					}
					while (num >= 0);
				}
				catch (Exception ex)
				{
					Log.Error("Exception in thread {0}:", new object[]
					{
						threadInfo.name
					});
					Log.Exception(ex);
					e = ex;
					exitForException = true;
				}
				if (threadInfo.threadEnd != null)
				{
					threadInfo.threadEnd(threadInfo, exitForException);
				}
			}
		}
		catch (Exception ex2)
		{
			Log.Error("Exception in thread {0}:", new object[]
			{
				threadInfo.name
			});
			Log.Exception(ex2);
			e = ex2;
		}
		finally
		{
			if (!threadInfo.isSilent)
			{
				Log.Out("Exited thread " + threadInfo.name);
			}
			Dictionary<string, ThreadManager.ThreadInfo> activeThreads = ThreadManager.ActiveThreads;
			lock (activeThreads)
			{
				ThreadManager.ActiveThreads.Remove(threadInfo.name);
			}
			threadInfo.evStopped.Set();
		}
		if (threadInfo.exitCallback != null)
		{
			threadInfo.exitCallback(threadInfo, e);
		}
		Profiler.EndThreadProfiling();
	}

	// Token: 0x06008586 RID: 34182 RVA: 0x003652B0 File Offset: 0x003634B0
	public static ThreadManager.TaskInfo AddSingleTask(ThreadManager.TaskFunctionDelegate _taskDelegate, object _parameter = null, ThreadManager.ExitCallbackTask _exitCallback = null, bool _endEvent = true)
	{
		ThreadManager.TaskInfo taskInfo = new ThreadManager.TaskInfo(_endEvent);
		taskInfo.taskDelegate = _taskDelegate;
		taskInfo.parameter = _parameter;
		taskInfo.exitCallback = _exitCallback;
		taskInfo.name = _taskDelegate.Method.Name;
		ThreadPool.UnsafeQueueUserWorkItem(ThreadManager.queuedTaskDelegate, taskInfo);
		return taskInfo;
	}

	// Token: 0x06008587 RID: 34183 RVA: 0x003652F8 File Offset: 0x003634F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void myQueuedTaskInvoke(object _taskInfo)
	{
		ThreadManager.TaskInfo taskInfo = (ThreadManager.TaskInfo)_taskInfo;
		object obj = ThreadManager.lockObjectQueuedCounter;
		lock (obj)
		{
			ThreadManager.QueuedCount++;
		}
		Exception e = null;
		try
		{
			taskInfo.taskDelegate(taskInfo);
		}
		catch (Exception ex)
		{
			Log.Error("Exception in task");
			Log.Exception(ex);
			e = ex;
		}
		finally
		{
			if (taskInfo.evStopped != null)
			{
				taskInfo.evStopped.Set();
			}
		}
		obj = ThreadManager.lockObjectQueuedCounter;
		lock (obj)
		{
			ThreadManager.QueuedCount--;
		}
		if (taskInfo.exitCallback != null)
		{
			taskInfo.exitCallback(taskInfo, e);
		}
		Profiler.EndThreadProfiling();
	}

	// Token: 0x06008588 RID: 34184 RVA: 0x003653E4 File Offset: 0x003635E4
	public static void AddSingleTaskMainThread(string _name, ThreadManager.MainThreadTaskFunctionDelegate _func, object _parameter = null)
	{
		ThreadManager.MainThreadTaskInfo item = default(ThreadManager.MainThreadTaskInfo);
		item.taskDelegate = _func;
		item.parameter = _parameter;
		item.name = _name;
		object obj = ThreadManager.lockObjectMainThreadTasks;
		lock (obj)
		{
			ThreadManager.mainThreadTasks.Add(item);
		}
	}

	// Token: 0x06008589 RID: 34185 RVA: 0x00365448 File Offset: 0x00363648
	public static void UpdateMainThreadTasks()
	{
		Action updateEv = ThreadManager.UpdateEv;
		if (updateEv != null)
		{
			updateEv();
		}
		int count = ThreadManager.mainThreadTasks.Count;
		if (count == 0)
		{
			return;
		}
		object obj = ThreadManager.lockObjectMainThreadTasks;
		lock (obj)
		{
			List<ThreadManager.MainThreadTaskInfo> list = ThreadManager.mainThreadTasks;
			ThreadManager.mainThreadTasks = ThreadManager.mainThreadTasksCopy;
			ThreadManager.mainThreadTasksCopy = list;
		}
		count = ThreadManager.mainThreadTasksCopy.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				ThreadManager.MainThreadTaskInfo mainThreadTaskInfo = ThreadManager.mainThreadTasksCopy[i];
				mainThreadTaskInfo.taskDelegate(mainThreadTaskInfo.parameter);
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
		}
		ThreadManager.mainThreadTasksCopy.Clear();
	}

	// Token: 0x0600858A RID: 34186 RVA: 0x0036550C File Offset: 0x0036370C
	public static void LateUpdate()
	{
		Action lateUpdateEv = ThreadManager.LateUpdateEv;
		if (lateUpdateEv == null)
		{
			return;
		}
		lateUpdateEv();
	}

	// Token: 0x0600858B RID: 34187 RVA: 0x00365520 File Offset: 0x00363720
	public static void Shutdown()
	{
		Log.Out("Terminating threads");
		using (Dictionary<string, ThreadManager.ThreadInfo>.Enumerator enumerator = ThreadManager.ActiveThreads.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, ThreadManager.ThreadInfo> keyValuePair = enumerator.Current;
				keyValuePair.Value.RequestTermination();
			}
			goto IL_82;
		}
		IL_44:
		using (Dictionary<string, ThreadManager.ThreadInfo>.Enumerator enumerator = ThreadManager.ActiveThreads.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<string, ThreadManager.ThreadInfo> keyValuePair2 = enumerator.Current;
				keyValuePair2.Value.WaitForEnd(30);
			}
		}
		IL_82:
		if (ThreadManager.ActiveThreads.Count <= 0)
		{
			return;
		}
		goto IL_44;
	}

	// Token: 0x0600858C RID: 34188 RVA: 0x003655D8 File Offset: 0x003637D8
	public static void SetMonoBehaviour(MonoBehaviour _monoBehaviour)
	{
		ThreadManager.monoBehaviour = _monoBehaviour;
	}

	// Token: 0x0600858D RID: 34189 RVA: 0x003655E0 File Offset: 0x003637E0
	public static Coroutine StartCoroutine(IEnumerator _e)
	{
		if (!ThreadManager.IsMainThread())
		{
			ThreadManager.AddSingleTaskMainThread("Coroutine", delegate(object _taskInfo)
			{
				ThreadManager.StartCoroutine(_e);
			}, null);
			return null;
		}
		if (ThreadManager.monoBehaviour != null)
		{
			return ThreadManager.monoBehaviour.StartCoroutine(_e);
		}
		return null;
	}

	// Token: 0x0600858E RID: 34190 RVA: 0x00365639 File Offset: 0x00363839
	public static void StopCoroutine(IEnumerator _e)
	{
		ThreadManager.monoBehaviour.StopCoroutine(_e);
	}

	// Token: 0x0600858F RID: 34191 RVA: 0x00365646 File Offset: 0x00363846
	public static void StopCoroutine(Coroutine _coroutine)
	{
		ThreadManager.monoBehaviour.StopCoroutine(_coroutine);
	}

	// Token: 0x06008590 RID: 34192 RVA: 0x00365653 File Offset: 0x00363853
	public static void StopCoroutine(string _methodName)
	{
		ThreadManager.monoBehaviour.StopCoroutine(_methodName);
	}

	// Token: 0x06008591 RID: 34193 RVA: 0x00365660 File Offset: 0x00363860
	public static void RunCoroutine(IEnumerator _e, Action _iterCallback)
	{
		while (_e.MoveNext())
		{
			object obj = _e.Current;
			IEnumerator enumerator = obj as IEnumerator;
			if (enumerator != null)
			{
				ThreadManager.RunCoroutine(enumerator, _iterCallback);
			}
			else
			{
				_iterCallback();
			}
		}
	}

	// Token: 0x17000DEF RID: 3567
	// (get) Token: 0x06008592 RID: 34194 RVA: 0x00365695 File Offset: 0x00363895
	public static bool IsInSyncCoroutine
	{
		get
		{
			return ThreadManager.syncCoroutineNestingLevel > 0;
		}
	}

	// Token: 0x06008593 RID: 34195 RVA: 0x003656A0 File Offset: 0x003638A0
	public static void RunCoroutineSync(IEnumerator _func)
	{
		ThreadManager.syncCoroutineNestingLevel++;
		try
		{
			while (_func.MoveNext())
			{
				object obj = _func.Current;
				IEnumerator enumerator = obj as IEnumerator;
				if (enumerator != null)
				{
					ThreadManager.RunCoroutineSync(enumerator);
				}
			}
		}
		finally
		{
			ThreadManager.syncCoroutineNestingLevel--;
		}
	}

	// Token: 0x06008594 RID: 34196 RVA: 0x003656F8 File Offset: 0x003638F8
	public static IEnumerator CoroutineWrapperWithExceptionCallback(IEnumerator _enumerator, Action<Exception> _exceptionHandler)
	{
		Stack<IEnumerator> stack = new Stack<IEnumerator>();
		stack.Push(_enumerator);
		while (stack.Count > 0)
		{
			IEnumerator enumerator = stack.Peek();
			object obj;
			try
			{
				if (!enumerator.MoveNext())
				{
					stack.Pop();
					continue;
				}
				obj = enumerator.Current;
			}
			catch (Exception obj2)
			{
				_exceptionHandler(obj2);
				yield break;
			}
			IEnumerator enumerator2 = obj as IEnumerator;
			if (enumerator2 != null)
			{
				stack.Push(enumerator2);
			}
			else
			{
				yield return obj;
			}
		}
		yield break;
	}

	// Token: 0x06008595 RID: 34197 RVA: 0x0036570E File Offset: 0x0036390E
	public static void SetMainThreadRef(Thread _mainThreadRef)
	{
		ThreadManager.MainThreadRef = _mainThreadRef;
		ThreadManager.MainThreadId = _mainThreadRef.ManagedThreadId;
	}

	// Token: 0x06008596 RID: 34198 RVA: 0x00365721 File Offset: 0x00363921
	public static bool IsMainThread()
	{
		return Thread.CurrentThread.ManagedThreadId == ThreadManager.MainThreadId;
	}

	// Token: 0x06008597 RID: 34199 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLog(string _messagePart1, string _messagePart2 = null)
	{
	}

	// Token: 0x040067AB RID: 26539
	public const int cEndTime = -1;

	// Token: 0x040067AC RID: 26540
	[PublicizedFrom(EAccessModifier.Private)]
	public const int threadTerminationTimeout = 30;

	// Token: 0x040067AD RID: 26541
	public static Thread MainThreadRef;

	// Token: 0x040067AE RID: 26542
	[PublicizedFrom(EAccessModifier.Private)]
	public static int MainThreadId;

	// Token: 0x040067B1 RID: 26545
	public static Dictionary<string, ThreadManager.ThreadInfo> ActiveThreads = new Dictionary<string, ThreadManager.ThreadInfo>();

	// Token: 0x040067B2 RID: 26546
	public static int QueuedCount;

	// Token: 0x040067B3 RID: 26547
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObjectQueuedCounter = new object();

	// Token: 0x040067B4 RID: 26548
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObjectMainThreadTasks = new object();

	// Token: 0x040067B5 RID: 26549
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<ThreadManager.MainThreadTaskInfo> mainThreadTasks = new List<ThreadManager.MainThreadTaskInfo>(150);

	// Token: 0x040067B6 RID: 26550
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<ThreadManager.MainThreadTaskInfo> mainThreadTasksCopy = new List<ThreadManager.MainThreadTaskInfo>(150);

	// Token: 0x040067B7 RID: 26551
	[PublicizedFrom(EAccessModifier.Private)]
	public static MonoBehaviour monoBehaviour;

	// Token: 0x040067B8 RID: 26552
	[PublicizedFrom(EAccessModifier.Private)]
	public static WaitCallback queuedTaskDelegate = new WaitCallback(ThreadManager.myQueuedTaskInvoke);

	// Token: 0x040067B9 RID: 26553
	[PublicizedFrom(EAccessModifier.Private)]
	public static int syncCoroutineNestingLevel;

	// Token: 0x0200107E RID: 4222
	public class ThreadInfo
	{
		// Token: 0x06008598 RID: 34200 RVA: 0x00365734 File Offset: 0x00363934
		public void RequestTermination()
		{
			this.evRunning.Set();
		}

		// Token: 0x06008599 RID: 34201 RVA: 0x00365742 File Offset: 0x00363942
		public bool TerminationRequested()
		{
			return this.evRunning.WaitOne(0);
		}

		// Token: 0x0600859A RID: 34202 RVA: 0x00365750 File Offset: 0x00363950
		public bool HasTerminated()
		{
			return this.evStopped.WaitOne(0);
		}

		// Token: 0x0600859B RID: 34203 RVA: 0x00365760 File Offset: 0x00363960
		public void WaitForEnd(int timeout = 30)
		{
			this.RequestTermination();
			if (!this.evStopped.WaitOne(timeout * 1000))
			{
				Log.Error(string.Concat(new string[]
				{
					"Thread ",
					this.name,
					" did not finish within ",
					timeout.ToString(),
					"s. Request trace: ",
					StackTraceUtility.ExtractStackTrace()
				}));
				Thread thread = this.thread;
				if (thread == null)
				{
					return;
				}
				thread.Abort();
			}
		}

		// Token: 0x040067BA RID: 26554
		public object parameter;

		// Token: 0x040067BB RID: 26555
		public ThreadManager.ThreadFunctionDelegate threadDelegate;

		// Token: 0x040067BC RID: 26556
		public ThreadManager.ThreadFunctionDelegate threadInit;

		// Token: 0x040067BD RID: 26557
		public ThreadManager.ThreadFunctionLoopDelegate threadLoop;

		// Token: 0x040067BE RID: 26558
		public ThreadManager.ThreadFunctionEndDelegate threadEnd;

		// Token: 0x040067BF RID: 26559
		public string name;

		// Token: 0x040067C0 RID: 26560
		public Thread thread;

		// Token: 0x040067C1 RID: 26561
		public bool isSilent;

		// Token: 0x040067C2 RID: 26562
		public readonly ManualResetEvent evRunning = new ManualResetEvent(false);

		// Token: 0x040067C3 RID: 26563
		public readonly ManualResetEvent evStopped = new ManualResetEvent(false);

		// Token: 0x040067C4 RID: 26564
		public ThreadManager.ExitCallbackThread exitCallback;

		// Token: 0x040067C5 RID: 26565
		public object threadData;
	}

	// Token: 0x0200107F RID: 4223
	public class TaskInfo
	{
		// Token: 0x0600859D RID: 34205 RVA: 0x003657FA File Offset: 0x003639FA
		public TaskInfo(bool _endEvent = true)
		{
			if (_endEvent)
			{
				this.evStopped = new ManualResetEvent(false);
			}
		}

		// Token: 0x0600859E RID: 34206 RVA: 0x00365811 File Offset: 0x00363A11
		public void WaitForEnd()
		{
			this.evStopped.WaitOne();
		}

		// Token: 0x040067C6 RID: 26566
		public string name;

		// Token: 0x040067C7 RID: 26567
		public ThreadManager.TaskFunctionDelegate taskDelegate;

		// Token: 0x040067C8 RID: 26568
		public object parameter;

		// Token: 0x040067C9 RID: 26569
		public ThreadManager.ExitCallbackTask exitCallback;

		// Token: 0x040067CA RID: 26570
		public ManualResetEvent evStopped;
	}

	// Token: 0x02001080 RID: 4224
	public struct MainThreadTaskInfo
	{
		// Token: 0x040067CB RID: 26571
		public string name;

		// Token: 0x040067CC RID: 26572
		public ThreadManager.MainThreadTaskFunctionDelegate taskDelegate;

		// Token: 0x040067CD RID: 26573
		public object parameter;
	}

	// Token: 0x02001081 RID: 4225
	// (Invoke) Token: 0x060085A0 RID: 34208
	public delegate int ThreadFunctionLoopDelegate(ThreadManager.ThreadInfo _threadInfo);

	// Token: 0x02001082 RID: 4226
	// (Invoke) Token: 0x060085A4 RID: 34212
	public delegate void ThreadFunctionEndDelegate(ThreadManager.ThreadInfo _threadInfo, bool _exitForException);

	// Token: 0x02001083 RID: 4227
	// (Invoke) Token: 0x060085A8 RID: 34216
	public delegate void ThreadFunctionDelegate(ThreadManager.ThreadInfo _threadInfo);

	// Token: 0x02001084 RID: 4228
	// (Invoke) Token: 0x060085AC RID: 34220
	public delegate void TaskFunctionDelegate(ThreadManager.TaskInfo _taskInfo);

	// Token: 0x02001085 RID: 4229
	// (Invoke) Token: 0x060085B0 RID: 34224
	public delegate void MainThreadTaskFunctionDelegate(object _parameter);

	// Token: 0x02001086 RID: 4230
	// (Invoke) Token: 0x060085B4 RID: 34228
	public delegate void ExitCallbackThread(ThreadManager.ThreadInfo _ti, Exception _e);

	// Token: 0x02001087 RID: 4231
	// (Invoke) Token: 0x060085B8 RID: 34232
	public delegate void ExitCallbackTask(ThreadManager.TaskInfo _ti, Exception _e);
}
