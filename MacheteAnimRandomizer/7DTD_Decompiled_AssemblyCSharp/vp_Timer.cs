using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020012E1 RID: 4833
public class vp_Timer : MonoBehaviour
{
	// Token: 0x17000F58 RID: 3928
	// (get) Token: 0x06009679 RID: 38521 RVA: 0x003BD490 File Offset: 0x003BB690
	public bool WasAddedCorrectly
	{
		get
		{
			return Application.isPlaying && !(base.gameObject != vp_Timer.m_MainObject);
		}
	}

	// Token: 0x0600967A RID: 38522 RVA: 0x003BD4B0 File Offset: 0x003BB6B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (!this.WasAddedCorrectly)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
	}

	// Token: 0x0600967B RID: 38523 RVA: 0x003BD4C4 File Offset: 0x003BB6C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		vp_Timer.m_EventBatch = 0;
		while (vp_Timer.m_Active.Count > 0 && vp_Timer.m_EventBatch < vp_Timer.MaxEventsPerFrame)
		{
			if (vp_Timer.m_EventIterator < 0)
			{
				vp_Timer.m_EventIterator = vp_Timer.m_Active.Count - 1;
				return;
			}
			if (vp_Timer.m_EventIterator > vp_Timer.m_Active.Count - 1)
			{
				vp_Timer.m_EventIterator = vp_Timer.m_Active.Count - 1;
			}
			if (Time.time >= vp_Timer.m_Active[vp_Timer.m_EventIterator].DueTime || vp_Timer.m_Active[vp_Timer.m_EventIterator].Id == 0)
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].Execute();
			}
			else if (vp_Timer.m_Active[vp_Timer.m_EventIterator].Paused)
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].DueTime += Time.deltaTime;
			}
			else
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].LifeTime += Time.deltaTime;
			}
			vp_Timer.m_EventIterator--;
			vp_Timer.m_EventBatch++;
		}
	}

	// Token: 0x0600967C RID: 38524 RVA: 0x003BD5ED File Offset: 0x003BB7ED
	public static void In(float delay, vp_Timer.Callback callback, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, 1, -1f);
	}

	// Token: 0x0600967D RID: 38525 RVA: 0x003BD5FF File Offset: 0x003BB7FF
	public static void In(float delay, vp_Timer.Callback callback, int iterations, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, iterations, -1f);
	}

	// Token: 0x0600967E RID: 38526 RVA: 0x003BD611 File Offset: 0x003BB811
	public static void In(float delay, vp_Timer.Callback callback, int iterations, float interval, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, iterations, interval);
	}

	// Token: 0x0600967F RID: 38527 RVA: 0x003BD620 File Offset: 0x003BB820
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, 1, -1f);
	}

	// Token: 0x06009680 RID: 38528 RVA: 0x003BD632 File Offset: 0x003BB832
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, int iterations, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, iterations, -1f);
	}

	// Token: 0x06009681 RID: 38529 RVA: 0x003BD645 File Offset: 0x003BB845
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, int iterations, float interval, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, iterations, interval);
	}

	// Token: 0x06009682 RID: 38530 RVA: 0x003BD655 File Offset: 0x003BB855
	public static void Start(vp_Timer.Handle timerHandle)
	{
		vp_Timer.Schedule(315360000f, delegate
		{
		}, null, null, timerHandle, 1, -1f);
	}

	// Token: 0x06009683 RID: 38531 RVA: 0x003BD68C File Offset: 0x003BB88C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Schedule(float time, vp_Timer.Callback func, vp_Timer.ArgCallback argFunc, object args, vp_Timer.Handle timerHandle, int iterations, float interval)
	{
		if (func == null && argFunc == null)
		{
			Debug.LogError("Error: (vp_Timer) Aborted event because function is null.");
			return;
		}
		if (vp_Timer.m_MainObject == null)
		{
			vp_Timer.m_MainObject = new GameObject("Timers");
			vp_Timer.m_MainObject.AddComponent<vp_Timer>();
			UnityEngine.Object.DontDestroyOnLoad(vp_Timer.m_MainObject);
		}
		time = Mathf.Max(0f, time);
		iterations = Mathf.Max(0, iterations);
		interval = ((interval == -1f) ? time : Mathf.Max(0f, interval));
		vp_Timer.m_NewEvent = null;
		if (vp_Timer.m_Pool.Count > 0)
		{
			vp_Timer.m_NewEvent = vp_Timer.m_Pool[0];
			vp_Timer.m_Pool.Remove(vp_Timer.m_NewEvent);
		}
		else
		{
			vp_Timer.m_NewEvent = new vp_Timer.Event();
		}
		vp_Timer.m_EventCount++;
		vp_Timer.m_NewEvent.Id = vp_Timer.m_EventCount;
		if (func != null)
		{
			vp_Timer.m_NewEvent.Function = func;
		}
		else if (argFunc != null)
		{
			vp_Timer.m_NewEvent.ArgFunction = argFunc;
			vp_Timer.m_NewEvent.Arguments = args;
		}
		vp_Timer.m_NewEvent.StartTime = Time.time;
		vp_Timer.m_NewEvent.DueTime = Time.time + time;
		vp_Timer.m_NewEvent.Iterations = iterations;
		vp_Timer.m_NewEvent.Interval = interval;
		vp_Timer.m_NewEvent.LifeTime = 0f;
		vp_Timer.m_NewEvent.Paused = false;
		vp_Timer.m_Active.Add(vp_Timer.m_NewEvent);
		if (timerHandle != null)
		{
			if (timerHandle.Active)
			{
				timerHandle.Cancel();
			}
			timerHandle.Id = vp_Timer.m_NewEvent.Id;
		}
	}

	// Token: 0x06009684 RID: 38532 RVA: 0x003BD815 File Offset: 0x003BBA15
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Cancel(vp_Timer.Handle handle)
	{
		if (handle == null)
		{
			return;
		}
		if (handle.Active)
		{
			handle.Id = 0;
			return;
		}
	}

	// Token: 0x06009685 RID: 38533 RVA: 0x003BD82C File Offset: 0x003BBA2C
	public static void CancelAll()
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			vp_Timer.m_Active[i].Id = 0;
		}
	}

	// Token: 0x06009686 RID: 38534 RVA: 0x003BD864 File Offset: 0x003BBA64
	public static void CancelAll(string methodName)
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			if (vp_Timer.m_Active[i].MethodName == methodName)
			{
				vp_Timer.m_Active[i].Id = 0;
			}
		}
	}

	// Token: 0x06009687 RID: 38535 RVA: 0x003BD8B1 File Offset: 0x003BBAB1
	public static void DestroyAll()
	{
		vp_Timer.m_Active.Clear();
		vp_Timer.m_Pool.Clear();
	}

	// Token: 0x06009688 RID: 38536 RVA: 0x003BD8C7 File Offset: 0x003BBAC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		SceneManager.sceneLoaded += this.NotifyLevelWasLoaded;
	}

	// Token: 0x06009689 RID: 38537 RVA: 0x003BD8DA File Offset: 0x003BBADA
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= this.NotifyLevelWasLoaded;
	}

	// Token: 0x0600968A RID: 38538 RVA: 0x003BD8F0 File Offset: 0x003BBAF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void NotifyLevelWasLoaded(Scene scene, LoadSceneMode mode)
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			if (vp_Timer.m_Active[i].CancelOnLoad)
			{
				vp_Timer.m_Active[i].Id = 0;
			}
		}
	}

	// Token: 0x0600968B RID: 38539 RVA: 0x003BD938 File Offset: 0x003BBB38
	public static vp_Timer.Stats EditorGetStats()
	{
		vp_Timer.Stats result;
		result.Created = vp_Timer.m_Active.Count + vp_Timer.m_Pool.Count;
		result.Inactive = vp_Timer.m_Pool.Count;
		result.Active = vp_Timer.m_Active.Count;
		return result;
	}

	// Token: 0x0600968C RID: 38540 RVA: 0x003BD984 File Offset: 0x003BBB84
	public static string EditorGetMethodInfo(int eventIndex)
	{
		if (eventIndex < 0 || eventIndex > vp_Timer.m_Active.Count - 1)
		{
			return "Argument out of range.";
		}
		return vp_Timer.m_Active[eventIndex].MethodInfo;
	}

	// Token: 0x0600968D RID: 38541 RVA: 0x003BD9AF File Offset: 0x003BBBAF
	public static int EditorGetMethodId(int eventIndex)
	{
		if (eventIndex < 0 || eventIndex > vp_Timer.m_Active.Count - 1)
		{
			return 0;
		}
		return vp_Timer.m_Active[eventIndex].Id;
	}

	// Token: 0x04007289 RID: 29321
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject m_MainObject = null;

	// Token: 0x0400728A RID: 29322
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<vp_Timer.Event> m_Active = new List<vp_Timer.Event>();

	// Token: 0x0400728B RID: 29323
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<vp_Timer.Event> m_Pool = new List<vp_Timer.Event>();

	// Token: 0x0400728C RID: 29324
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static vp_Timer.Event m_NewEvent = null;

	// Token: 0x0400728D RID: 29325
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int m_EventCount = 0;

	// Token: 0x0400728E RID: 29326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int m_EventBatch = 0;

	// Token: 0x0400728F RID: 29327
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int m_EventIterator = 0;

	// Token: 0x04007290 RID: 29328
	public static int MaxEventsPerFrame = 500;

	// Token: 0x020012E2 RID: 4834
	// (Invoke) Token: 0x06009691 RID: 38545
	public delegate void Callback();

	// Token: 0x020012E3 RID: 4835
	// (Invoke) Token: 0x06009695 RID: 38549
	public delegate void ArgCallback(object args);

	// Token: 0x020012E4 RID: 4836
	public struct Stats
	{
		// Token: 0x04007291 RID: 29329
		public int Created;

		// Token: 0x04007292 RID: 29330
		public int Inactive;

		// Token: 0x04007293 RID: 29331
		public int Active;
	}

	// Token: 0x020012E5 RID: 4837
	[PublicizedFrom(EAccessModifier.Private)]
	public class Event
	{
		// Token: 0x06009698 RID: 38552 RVA: 0x003BDA14 File Offset: 0x003BBC14
		public void Execute()
		{
			if (this.Id == 0 || this.DueTime == 0f)
			{
				this.Recycle();
				return;
			}
			if (this.Function != null)
			{
				this.Function();
			}
			else
			{
				if (this.ArgFunction == null)
				{
					this.Error("Aborted event because function is null.");
					this.Recycle();
					return;
				}
				this.ArgFunction(this.Arguments);
			}
			if (this.Iterations > 0)
			{
				this.Iterations--;
				if (this.Iterations < 1)
				{
					this.Recycle();
					return;
				}
			}
			this.DueTime = Time.time + this.Interval;
		}

		// Token: 0x06009699 RID: 38553 RVA: 0x003BDAB8 File Offset: 0x003BBCB8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Recycle()
		{
			this.Id = 0;
			this.DueTime = 0f;
			this.StartTime = 0f;
			this.CancelOnLoad = true;
			this.Function = null;
			this.ArgFunction = null;
			this.Arguments = null;
			if (vp_Timer.m_Active.Remove(this))
			{
				vp_Timer.m_Pool.Add(this);
			}
		}

		// Token: 0x0600969A RID: 38554 RVA: 0x003BDB16 File Offset: 0x003BBD16
		[PublicizedFrom(EAccessModifier.Private)]
		public void Destroy()
		{
			vp_Timer.m_Active.Remove(this);
			vp_Timer.m_Pool.Remove(this);
		}

		// Token: 0x0600969B RID: 38555 RVA: 0x003BDB30 File Offset: 0x003BBD30
		[PublicizedFrom(EAccessModifier.Private)]
		public void Error(string message)
		{
			Debug.LogError("Error: (" + ((this != null) ? this.ToString() : null) + ") " + message);
		}

		// Token: 0x17000F59 RID: 3929
		// (get) Token: 0x0600969C RID: 38556 RVA: 0x003BDB54 File Offset: 0x003BBD54
		public string MethodName
		{
			get
			{
				if (this.Function != null)
				{
					if (this.Function.Method != null)
					{
						if (this.Function.Method.Name[0] == '<')
						{
							return "delegate";
						}
						return this.Function.Method.Name;
					}
				}
				else if (this.ArgFunction != null && this.ArgFunction.Method != null)
				{
					if (this.ArgFunction.Method.Name[0] == '<')
					{
						return "delegate";
					}
					return this.ArgFunction.Method.Name;
				}
				return null;
			}
		}

		// Token: 0x17000F5A RID: 3930
		// (get) Token: 0x0600969D RID: 38557 RVA: 0x003BDBFC File Offset: 0x003BBDFC
		public string MethodInfo
		{
			get
			{
				string text = this.MethodName;
				if (!string.IsNullOrEmpty(text))
				{
					text += "(";
					if (this.Arguments != null)
					{
						if (this.Arguments.GetType().IsArray)
						{
							object[] array = (object[])this.Arguments;
							foreach (object obj in array)
							{
								text += obj.ToString();
								if (Array.IndexOf<object>(array, obj) < array.Length - 1)
								{
									text += ", ";
								}
							}
						}
						else
						{
							string str = text;
							object arguments = this.Arguments;
							text = str + ((arguments != null) ? arguments.ToString() : null);
						}
					}
					text += ")";
				}
				else
				{
					text = "(function = null)";
				}
				return text;
			}
		}

		// Token: 0x04007294 RID: 29332
		public int Id;

		// Token: 0x04007295 RID: 29333
		public vp_Timer.Callback Function;

		// Token: 0x04007296 RID: 29334
		public vp_Timer.ArgCallback ArgFunction;

		// Token: 0x04007297 RID: 29335
		public object Arguments;

		// Token: 0x04007298 RID: 29336
		public int Iterations = 1;

		// Token: 0x04007299 RID: 29337
		public float Interval = -1f;

		// Token: 0x0400729A RID: 29338
		public float DueTime;

		// Token: 0x0400729B RID: 29339
		public float StartTime;

		// Token: 0x0400729C RID: 29340
		public float LifeTime;

		// Token: 0x0400729D RID: 29341
		public bool Paused;

		// Token: 0x0400729E RID: 29342
		public bool CancelOnLoad = true;
	}

	// Token: 0x020012E6 RID: 4838
	public class Handle
	{
		// Token: 0x17000F5B RID: 3931
		// (get) Token: 0x0600969F RID: 38559 RVA: 0x003BDCDC File Offset: 0x003BBEDC
		// (set) Token: 0x060096A0 RID: 38560 RVA: 0x003BDCF3 File Offset: 0x003BBEF3
		public bool Paused
		{
			get
			{
				return this.Active && this.m_Event.Paused;
			}
			set
			{
				if (this.Active)
				{
					this.m_Event.Paused = value;
				}
			}
		}

		// Token: 0x17000F5C RID: 3932
		// (get) Token: 0x060096A1 RID: 38561 RVA: 0x003BDD09 File Offset: 0x003BBF09
		public float TimeOfInitiation
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.StartTime;
				}
				return 0f;
			}
		}

		// Token: 0x17000F5D RID: 3933
		// (get) Token: 0x060096A2 RID: 38562 RVA: 0x003BDD24 File Offset: 0x003BBF24
		public float TimeOfFirstIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_FirstDueTime;
				}
				return 0f;
			}
		}

		// Token: 0x17000F5E RID: 3934
		// (get) Token: 0x060096A3 RID: 38563 RVA: 0x003BDD3A File Offset: 0x003BBF3A
		public float TimeOfNextIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.DueTime;
				}
				return 0f;
			}
		}

		// Token: 0x17000F5F RID: 3935
		// (get) Token: 0x060096A4 RID: 38564 RVA: 0x003BDD55 File Offset: 0x003BBF55
		public float TimeOfLastIteration
		{
			get
			{
				if (this.Active)
				{
					return Time.time + this.DurationLeft;
				}
				return 0f;
			}
		}

		// Token: 0x17000F60 RID: 3936
		// (get) Token: 0x060096A5 RID: 38565 RVA: 0x003BDD71 File Offset: 0x003BBF71
		public float Delay
		{
			get
			{
				return Mathf.Round((this.m_FirstDueTime - this.TimeOfInitiation) * 1000f) / 1000f;
			}
		}

		// Token: 0x17000F61 RID: 3937
		// (get) Token: 0x060096A6 RID: 38566 RVA: 0x003BDD91 File Offset: 0x003BBF91
		public float Interval
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.Interval;
				}
				return 0f;
			}
		}

		// Token: 0x17000F62 RID: 3938
		// (get) Token: 0x060096A7 RID: 38567 RVA: 0x003BDDAC File Offset: 0x003BBFAC
		public float TimeUntilNextIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.DueTime - Time.time;
				}
				return 0f;
			}
		}

		// Token: 0x17000F63 RID: 3939
		// (get) Token: 0x060096A8 RID: 38568 RVA: 0x003BDDCD File Offset: 0x003BBFCD
		public float DurationLeft
		{
			get
			{
				if (this.Active)
				{
					return this.TimeUntilNextIteration + (float)(this.m_Event.Iterations - 1) * this.m_Event.Interval;
				}
				return 0f;
			}
		}

		// Token: 0x17000F64 RID: 3940
		// (get) Token: 0x060096A9 RID: 38569 RVA: 0x003BDDFE File Offset: 0x003BBFFE
		public float DurationTotal
		{
			get
			{
				if (this.Active)
				{
					return this.Delay + (float)this.m_StartIterations * ((this.m_StartIterations > 1) ? this.Interval : 0f);
				}
				return 0f;
			}
		}

		// Token: 0x17000F65 RID: 3941
		// (get) Token: 0x060096AA RID: 38570 RVA: 0x003BDE33 File Offset: 0x003BC033
		public float Duration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.LifeTime;
				}
				return 0f;
			}
		}

		// Token: 0x17000F66 RID: 3942
		// (get) Token: 0x060096AB RID: 38571 RVA: 0x003BDE4E File Offset: 0x003BC04E
		public int IterationsTotal
		{
			get
			{
				return this.m_StartIterations;
			}
		}

		// Token: 0x17000F67 RID: 3943
		// (get) Token: 0x060096AC RID: 38572 RVA: 0x003BDE56 File Offset: 0x003BC056
		public int IterationsLeft
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.Iterations;
				}
				return 0;
			}
		}

		// Token: 0x17000F68 RID: 3944
		// (get) Token: 0x060096AD RID: 38573 RVA: 0x003BDE6D File Offset: 0x003BC06D
		// (set) Token: 0x060096AE RID: 38574 RVA: 0x003BDE78 File Offset: 0x003BC078
		public int Id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
				if (this.m_Id == 0)
				{
					this.m_Event.DueTime = 0f;
					return;
				}
				this.m_Event = null;
				for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
				{
					if (vp_Timer.m_Active[i].Id == this.m_Id)
					{
						this.m_Event = vp_Timer.m_Active[i];
						break;
					}
				}
				if (this.m_Event == null)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Error: (",
						(this != null) ? this.ToString() : null,
						") Failed to assign event with Id '",
						this.m_Id.ToString(),
						"'."
					}));
				}
				this.m_StartIterations = this.m_Event.Iterations;
				this.m_FirstDueTime = this.m_Event.DueTime;
			}
		}

		// Token: 0x17000F69 RID: 3945
		// (get) Token: 0x060096AF RID: 38575 RVA: 0x003BDF5E File Offset: 0x003BC15E
		public bool Active
		{
			get
			{
				return this.m_Event != null && this.Id != 0 && this.m_Event.Id != 0 && this.m_Event.Id == this.Id;
			}
		}

		// Token: 0x17000F6A RID: 3946
		// (get) Token: 0x060096B0 RID: 38576 RVA: 0x003BDF92 File Offset: 0x003BC192
		public string MethodName
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.MethodName;
				}
				return "";
			}
		}

		// Token: 0x17000F6B RID: 3947
		// (get) Token: 0x060096B1 RID: 38577 RVA: 0x003BDFAD File Offset: 0x003BC1AD
		public string MethodInfo
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.MethodInfo;
				}
				return "";
			}
		}

		// Token: 0x17000F6C RID: 3948
		// (get) Token: 0x060096B2 RID: 38578 RVA: 0x003BDFC8 File Offset: 0x003BC1C8
		// (set) Token: 0x060096B3 RID: 38579 RVA: 0x003BDFDF File Offset: 0x003BC1DF
		public bool CancelOnLoad
		{
			get
			{
				return !this.Active || this.m_Event.CancelOnLoad;
			}
			set
			{
				if (this.Active)
				{
					this.m_Event.CancelOnLoad = value;
					return;
				}
				Debug.LogWarning("Warning: (" + ((this != null) ? this.ToString() : null) + ") Tried to set CancelOnLoad on inactive timer handle.");
			}
		}

		// Token: 0x060096B4 RID: 38580 RVA: 0x003BE017 File Offset: 0x003BC217
		public void Cancel()
		{
			vp_Timer.Cancel(this);
		}

		// Token: 0x060096B5 RID: 38581 RVA: 0x003BE01F File Offset: 0x003BC21F
		public void Execute()
		{
			this.m_Event.DueTime = Time.time;
		}

		// Token: 0x0400729F RID: 29343
		[PublicizedFrom(EAccessModifier.Private)]
		public vp_Timer.Event m_Event;

		// Token: 0x040072A0 RID: 29344
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_Id;

		// Token: 0x040072A1 RID: 29345
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_StartIterations = 1;

		// Token: 0x040072A2 RID: 29346
		[PublicizedFrom(EAccessModifier.Private)]
		public float m_FirstDueTime;
	}
}
