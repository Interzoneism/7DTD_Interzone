using System;
using System.Threading;

// Token: 0x02001077 RID: 4215
public class TaskManager
{
	// Token: 0x17000DED RID: 3565
	// (get) Token: 0x06008566 RID: 34150 RVA: 0x00364C7C File Offset: 0x00362E7C
	public static bool Pending
	{
		get
		{
			return TaskManager.rootGroup.Pending;
		}
	}

	// Token: 0x06008567 RID: 34151 RVA: 0x00364C88 File Offset: 0x00362E88
	public static void Init()
	{
		TaskManager.rootGroup = new TaskManager.TaskGroup(null);
		TaskManager.tasks = new WorkBatch<TaskManager.Task>();
	}

	// Token: 0x06008568 RID: 34152 RVA: 0x00364C9F File Offset: 0x00362E9F
	public static void Destroy()
	{
		TaskManager.WaitOnGroup(TaskManager.rootGroup);
	}

	// Token: 0x06008569 RID: 34153 RVA: 0x00364CAB File Offset: 0x00362EAB
	public static void Update()
	{
		TaskManager.tasks.DoWork(new Action<TaskManager.Task>(TaskManager.CompleteTask));
	}

	// Token: 0x0600856A RID: 34154 RVA: 0x00364CC3 File Offset: 0x00362EC3
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CompleteTask(TaskManager.Task _task)
	{
		if (_task.Complete != null)
		{
			_task.Complete();
			TaskManager.OnTaskCompleted(_task);
		}
	}

	// Token: 0x0600856B RID: 34155 RVA: 0x00364CDE File Offset: 0x00362EDE
	public static TaskManager.TaskGroup CreateGroup()
	{
		return new TaskManager.TaskGroup(TaskManager.rootGroup);
	}

	// Token: 0x0600856C RID: 34156 RVA: 0x00364CEA File Offset: 0x00362EEA
	public static TaskManager.TaskGroup CreateGroup(TaskManager.TaskGroup _parent)
	{
		return new TaskManager.TaskGroup(_parent);
	}

	// Token: 0x0600856D RID: 34157 RVA: 0x00364CF4 File Offset: 0x00362EF4
	public static void Schedule(Action _execute, Action _complete)
	{
		TaskManager.Task task = new TaskManager.Task(TaskManager.rootGroup, _execute, _complete);
		TaskManager.OnTaskCreated(task);
		ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(TaskManager.Execute), task, null, false);
	}

	// Token: 0x0600856E RID: 34158 RVA: 0x00364D2C File Offset: 0x00362F2C
	public static void Schedule(TaskManager.TaskGroup _group, Action _execute, Action _complete)
	{
		TaskManager.Task task = new TaskManager.Task(_group, _execute, _complete);
		TaskManager.OnTaskCreated(task);
		ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(TaskManager.Execute), task, null, false);
	}

	// Token: 0x0600856F RID: 34159 RVA: 0x00364D5D File Offset: 0x00362F5D
	public static void WaitOnGroup(TaskManager.TaskGroup _group)
	{
		if (!ThreadManager.MainThreadRef.Equals(Thread.CurrentThread))
		{
			throw new Exception("TaskManager.WaitOnGroup should only be called from the main thread.");
		}
		TaskManager.Update();
		while (_group.Pending)
		{
			Thread.Sleep(1);
			TaskManager.Update();
		}
	}

	// Token: 0x06008570 RID: 34160 RVA: 0x00364D98 File Offset: 0x00362F98
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Execute(ThreadManager.TaskInfo _info)
	{
		TaskManager.Task task = _info.parameter as TaskManager.Task;
		task.Execute();
		if (task.Complete != null)
		{
			TaskManager.tasks.Add(task);
			return;
		}
		TaskManager.OnTaskCompleted(task);
	}

	// Token: 0x06008571 RID: 34161 RVA: 0x00364DD8 File Offset: 0x00362FD8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnTaskCreated(TaskManager.Task task)
	{
		for (TaskManager.TaskGroup taskGroup = task.Group; taskGroup != null; taskGroup = taskGroup.parent)
		{
			Interlocked.Increment(ref taskGroup.pending);
		}
	}

	// Token: 0x06008572 RID: 34162 RVA: 0x00364E04 File Offset: 0x00363004
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnTaskCompleted(TaskManager.Task task)
	{
		for (TaskManager.TaskGroup taskGroup = task.Group; taskGroup != null; taskGroup = taskGroup.parent)
		{
			Interlocked.Decrement(ref taskGroup.pending);
		}
	}

	// Token: 0x0400679F RID: 26527
	[PublicizedFrom(EAccessModifier.Private)]
	public static TaskManager.TaskGroup rootGroup;

	// Token: 0x040067A0 RID: 26528
	[PublicizedFrom(EAccessModifier.Private)]
	public static WorkBatch<TaskManager.Task> tasks;

	// Token: 0x02001078 RID: 4216
	public class TaskGroup
	{
		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x06008574 RID: 34164 RVA: 0x00364E30 File Offset: 0x00363030
		public bool Pending
		{
			get
			{
				return Interlocked.CompareExchange(ref this.pending, 0, 0) != 0;
			}
		}

		// Token: 0x06008575 RID: 34165 RVA: 0x00364E42 File Offset: 0x00363042
		[PublicizedFrom(EAccessModifier.Internal)]
		public TaskGroup(TaskManager.TaskGroup _parent)
		{
			this.parent = _parent;
		}

		// Token: 0x040067A1 RID: 26529
		[PublicizedFrom(EAccessModifier.Internal)]
		public TaskManager.TaskGroup parent;

		// Token: 0x040067A2 RID: 26530
		[PublicizedFrom(EAccessModifier.Internal)]
		public int pending;
	}

	// Token: 0x02001079 RID: 4217
	[PublicizedFrom(EAccessModifier.Internal)]
	public class Task
	{
		// Token: 0x06008576 RID: 34166 RVA: 0x00364E51 File Offset: 0x00363051
		[PublicizedFrom(EAccessModifier.Internal)]
		public Task(TaskManager.TaskGroup _group, Action _execute, Action _complete)
		{
			this.Group = _group;
			this.Execute = _execute;
			this.Complete = _complete;
		}

		// Token: 0x040067A3 RID: 26531
		[PublicizedFrom(EAccessModifier.Internal)]
		public TaskManager.TaskGroup Group;

		// Token: 0x040067A4 RID: 26532
		[PublicizedFrom(EAccessModifier.Internal)]
		public Action Execute;

		// Token: 0x040067A5 RID: 26533
		[PublicizedFrom(EAccessModifier.Internal)]
		public Action Complete;
	}
}
