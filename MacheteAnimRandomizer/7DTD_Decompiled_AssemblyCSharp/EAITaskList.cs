using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000401 RID: 1025
[Preserve]
public class EAITaskList
{
	// Token: 0x06001ED7 RID: 7895 RVA: 0x000C0214 File Offset: 0x000BE414
	public EAITaskList(EAIManager _manager)
	{
		this.allTasks = new List<EAITaskEntry>();
		this.executingTasks = new List<EAITaskEntry>();
		this.executeDelayScale = 0.85f + _manager.random.RandomFloat * 0.25f;
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x000C0265 File Offset: 0x000BE465
	public void AddTask(int _priority, EAIBase _eai)
	{
		this.allTasks.Add(new EAITaskEntry(_priority, _eai));
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06001ED9 RID: 7897 RVA: 0x000C0279 File Offset: 0x000BE479
	public List<EAITaskEntry> Tasks
	{
		get
		{
			return this.allTasks;
		}
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x000C0281 File Offset: 0x000BE481
	public List<EAITaskEntry> GetExecutingTasks()
	{
		return this.executingTasks;
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x000C028C File Offset: 0x000BE48C
	public T GetTask<T>() where T : class
	{
		for (int i = 0; i < this.allTasks.Count; i++)
		{
			T t = this.allTasks[i].action as T;
			if (t != null)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000C02E0 File Offset: 0x000BE4E0
	public void OnUpdateTasks()
	{
		this.startedTasks.Clear();
		int i = 0;
		while (i < this.allTasks.Count)
		{
			EAITaskEntry eaitaskEntry = this.allTasks[i];
			if (!eaitaskEntry.isExecuting)
			{
				goto IL_7A;
			}
			if (!this.isBestTask(eaitaskEntry) || !eaitaskEntry.action.Continue())
			{
				this.executingTasks.Remove(eaitaskEntry);
				eaitaskEntry.isExecuting = false;
				eaitaskEntry.executeTime = eaitaskEntry.action.executeDelay * this.executeDelayScale;
				eaitaskEntry.action.Reset();
				goto IL_7A;
			}
			IL_10D:
			i++;
			continue;
			IL_7A:
			eaitaskEntry.executeTime -= 0.05f;
			eaitaskEntry.action.executeWaitTime += 0.05f;
			if (eaitaskEntry.executeTime > 0f)
			{
				goto IL_10D;
			}
			eaitaskEntry.executeTime = eaitaskEntry.action.executeDelay * this.executeDelayScale;
			if (this.isBestTask(eaitaskEntry))
			{
				if (eaitaskEntry.action.CanExecute())
				{
					this.startedTasks.Add(eaitaskEntry);
					this.executingTasks.Add(eaitaskEntry);
					eaitaskEntry.isExecuting = true;
				}
				eaitaskEntry.action.executeWaitTime = 0f;
				goto IL_10D;
			}
			goto IL_10D;
		}
		for (int j = 0; j < this.startedTasks.Count; j++)
		{
			this.startedTasks[j].action.Start();
		}
		for (int k = 0; k < this.executingTasks.Count; k++)
		{
			this.executingTasks[k].action.Update();
		}
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000C0468 File Offset: 0x000BE668
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBestTask(EAITaskEntry _task)
	{
		int i = 0;
		while (i < this.executingTasks.Count)
		{
			EAITaskEntry eaitaskEntry = this.executingTasks[i++];
			if (eaitaskEntry != _task)
			{
				if (eaitaskEntry.priority > _task.priority)
				{
					if (eaitaskEntry.action.IsContinuous())
					{
						continue;
					}
				}
				else if (this.areTasksCompatible(_task, eaitaskEntry))
				{
					continue;
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000C04C4 File Offset: 0x000BE6C4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool areTasksCompatible(EAITaskEntry _task, EAITaskEntry _other)
	{
		return (_task.action.MutexBits & _other.action.MutexBits) == 0;
	}

	// Token: 0x04001553 RID: 5459
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> allTasks;

	// Token: 0x04001554 RID: 5460
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> executingTasks;

	// Token: 0x04001555 RID: 5461
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> startedTasks = new List<EAITaskEntry>();

	// Token: 0x04001556 RID: 5462
	[PublicizedFrom(EAccessModifier.Private)]
	public float executeDelayScale;
}
