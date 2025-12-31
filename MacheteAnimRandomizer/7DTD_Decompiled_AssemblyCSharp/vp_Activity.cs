using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001293 RID: 4755
[Preserve]
public class vp_Activity : vp_Event
{
	// Token: 0x060094D9 RID: 38105 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void Empty()
	{
	}

	// Token: 0x060094DA RID: 38106 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool AlwaysOK()
	{
		return true;
	}

	// Token: 0x060094DB RID: 38107 RVA: 0x003B6192 File Offset: 0x003B4392
	public vp_Activity(string name) : base(name)
	{
		this.InitFields();
	}

	// Token: 0x17000F48 RID: 3912
	// (get) Token: 0x060094DC RID: 38108 RVA: 0x003B61B7 File Offset: 0x003B43B7
	// (set) Token: 0x060094DD RID: 38109 RVA: 0x003B61BF File Offset: 0x003B43BF
	public float MinPause
	{
		get
		{
			return this.m_MinPause;
		}
		set
		{
			this.m_MinPause = Mathf.Max(0f, value);
		}
	}

	// Token: 0x17000F49 RID: 3913
	// (get) Token: 0x060094DE RID: 38110 RVA: 0x003B61D2 File Offset: 0x003B43D2
	// (set) Token: 0x060094DF RID: 38111 RVA: 0x003B61DC File Offset: 0x003B43DC
	public float MinDuration
	{
		get
		{
			return this.m_MinDuration;
		}
		set
		{
			this.m_MinDuration = Mathf.Max(0.001f, value);
			if (this.m_MaxDuration == -1f)
			{
				return;
			}
			if (this.m_MinDuration > this.m_MaxDuration)
			{
				this.m_MinDuration = this.m_MaxDuration;
				Debug.LogWarning("Warning: (vp_Activity) Tried to set MinDuration longer than MaxDuration for '" + base.EventName + "'. Capping at MaxDuration.");
			}
		}
	}

	// Token: 0x17000F4A RID: 3914
	// (get) Token: 0x060094E0 RID: 38112 RVA: 0x003B623C File Offset: 0x003B443C
	// (set) Token: 0x060094E1 RID: 38113 RVA: 0x003B6244 File Offset: 0x003B4444
	public float AutoDuration
	{
		get
		{
			return this.m_MaxDuration;
		}
		set
		{
			if (value == -1f)
			{
				this.m_MaxDuration = value;
				return;
			}
			this.m_MaxDuration = Mathf.Max(0.001f, value);
			if (this.m_MaxDuration < this.m_MinDuration)
			{
				this.m_MaxDuration = this.m_MinDuration;
				Debug.LogWarning("Warning: (vp_Activity) Tried to set MaxDuration shorter than MinDuration for '" + base.EventName + "'. Capping at MinDuration.");
			}
		}
	}

	// Token: 0x17000F4B RID: 3915
	// (get) Token: 0x060094E2 RID: 38114 RVA: 0x003B62A8 File Offset: 0x003B44A8
	// (set) Token: 0x060094E3 RID: 38115 RVA: 0x003B630C File Offset: 0x003B450C
	public object Argument
	{
		get
		{
			if (this.m_ArgumentType == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Error: (",
					(this != null) ? this.ToString() : null,
					") Tried to fetch argument from '",
					base.EventName,
					"' but this activity takes no parameters."
				}));
				return null;
			}
			return this.m_Argument;
		}
		set
		{
			if (this.m_ArgumentType == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Error: (",
					(this != null) ? this.ToString() : null,
					") Tried to set argument for '",
					base.EventName,
					"' but this activity takes no parameters."
				}));
				return;
			}
			this.m_Argument = value;
		}
	}

	// Token: 0x060094E4 RID: 38116 RVA: 0x003B6370 File Offset: 0x003B4570
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitFields()
	{
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Condition),
			typeof(vp_Activity.Condition),
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Callback)
		};
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("StartCallbacks"),
			base.GetType().GetField("StopCallbacks"),
			base.GetType().GetField("StartConditions"),
			base.GetType().GetField("StopConditions"),
			base.GetType().GetField("FailStartCallbacks"),
			base.GetType().GetField("FailStopCallbacks")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("AlwaysOK"),
			base.GetType().GetMethod("AlwaysOK"),
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("Empty")
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnStart_",
				0
			},
			{
				"OnStop_",
				1
			},
			{
				"CanStart_",
				2
			},
			{
				"CanStop_",
				3
			},
			{
				"OnFailStart_",
				4
			},
			{
				"OnFailStop_",
				5
			}
		};
		this.StartCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.StopCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.StartConditions = new vp_Activity.Condition(vp_Activity.AlwaysOK);
		this.StopConditions = new vp_Activity.Condition(vp_Activity.AlwaysOK);
		this.FailStartCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.FailStopCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
	}

	// Token: 0x060094E5 RID: 38117 RVA: 0x003B6598 File Offset: 0x003B4798
	public override void Register(object t, string m, int v)
	{
		base.AddExternalMethodToField(t, this.m_Fields[v], m, this.m_DelegateTypes[v]);
		base.Refresh();
	}

	// Token: 0x060094E6 RID: 38118 RVA: 0x003B65B8 File Offset: 0x003B47B8
	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.RemoveExternalMethodFromField(t, this.m_Fields[1]);
		base.RemoveExternalMethodFromField(t, this.m_Fields[2]);
		base.RemoveExternalMethodFromField(t, this.m_Fields[3]);
		base.RemoveExternalMethodFromField(t, this.m_Fields[4]);
		base.RemoveExternalMethodFromField(t, this.m_Fields[5]);
		base.Refresh();
	}

	// Token: 0x060094E7 RID: 38119 RVA: 0x003B6628 File Offset: 0x003B4828
	public bool TryStart(bool startIfAllowed = true)
	{
		if (this.m_Active)
		{
			return false;
		}
		if (Time.time < this.NextAllowedStartTime)
		{
			this.m_Argument = null;
			return false;
		}
		Delegate[] invocationList = this.StartConditions.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (!((vp_Activity.Condition)invocationList[i])())
			{
				this.m_Argument = null;
				if (startIfAllowed)
				{
					this.FailStartCallbacks();
				}
				return false;
			}
		}
		if (startIfAllowed)
		{
			this.Active = true;
		}
		return true;
	}

	// Token: 0x060094E8 RID: 38120 RVA: 0x003B66A0 File Offset: 0x003B48A0
	public bool TryStop(bool stopIfAllowed = true)
	{
		if (!this.m_Active)
		{
			return false;
		}
		if (Time.time < this.NextAllowedStopTime)
		{
			return false;
		}
		Delegate[] invocationList = this.StopConditions.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (!((vp_Activity.Condition)invocationList[i])())
			{
				if (stopIfAllowed)
				{
					this.FailStopCallbacks();
				}
				return false;
			}
		}
		if (stopIfAllowed)
		{
			this.Active = false;
		}
		return true;
	}

	// Token: 0x17000F4C RID: 3916
	// (get) Token: 0x060094EA RID: 38122 RVA: 0x003B67A9 File Offset: 0x003B49A9
	// (set) Token: 0x060094E9 RID: 38121 RVA: 0x003B670C File Offset: 0x003B490C
	public bool Active
	{
		get
		{
			return this.m_Active;
		}
		set
		{
			if (value && !this.m_Active)
			{
				this.m_Active = true;
				this.StartCallbacks();
				this.NextAllowedStopTime = Time.time + this.m_MinDuration;
				if (this.m_MaxDuration > 0f)
				{
					vp_Timer.In(this.m_MaxDuration, delegate()
					{
						this.Stop(0f);
					}, this.m_ForceStopTimer);
					return;
				}
			}
			else if (!value && this.m_Active)
			{
				this.m_Active = false;
				this.StopCallbacks();
				this.NextAllowedStartTime = Time.time + this.m_MinPause;
				this.m_Argument = null;
			}
		}
	}

	// Token: 0x060094EB RID: 38123 RVA: 0x003B67B1 File Offset: 0x003B49B1
	public void Start(float forcedActiveDuration = 0f)
	{
		this.Active = true;
		if (forcedActiveDuration > 0f)
		{
			this.NextAllowedStopTime = Time.time + forcedActiveDuration;
		}
	}

	// Token: 0x060094EC RID: 38124 RVA: 0x003B67CF File Offset: 0x003B49CF
	public void Stop(float forcedPauseDuration = 0f)
	{
		this.Active = false;
		if (forcedPauseDuration > 0f)
		{
			this.NextAllowedStartTime = Time.time + forcedPauseDuration;
		}
	}

	// Token: 0x060094ED RID: 38125 RVA: 0x003B67ED File Offset: 0x003B49ED
	public void Disallow(float duration)
	{
		this.NextAllowedStartTime = Time.time + duration;
	}

	// Token: 0x040071D6 RID: 29142
	public vp_Activity.Callback StartCallbacks;

	// Token: 0x040071D7 RID: 29143
	public vp_Activity.Callback StopCallbacks;

	// Token: 0x040071D8 RID: 29144
	public vp_Activity.Condition StartConditions;

	// Token: 0x040071D9 RID: 29145
	public vp_Activity.Condition StopConditions;

	// Token: 0x040071DA RID: 29146
	public vp_Activity.Callback FailStartCallbacks;

	// Token: 0x040071DB RID: 29147
	public vp_Activity.Callback FailStopCallbacks;

	// Token: 0x040071DC RID: 29148
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Timer.Handle m_ForceStopTimer = new vp_Timer.Handle();

	// Token: 0x040071DD RID: 29149
	[PublicizedFrom(EAccessModifier.Protected)]
	public object m_Argument;

	// Token: 0x040071DE RID: 29150
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool m_Active;

	// Token: 0x040071DF RID: 29151
	public float NextAllowedStartTime;

	// Token: 0x040071E0 RID: 29152
	public float NextAllowedStopTime;

	// Token: 0x040071E1 RID: 29153
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_MinPause;

	// Token: 0x040071E2 RID: 29154
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_MinDuration;

	// Token: 0x040071E3 RID: 29155
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_MaxDuration = -1f;

	// Token: 0x02001294 RID: 4756
	// (Invoke) Token: 0x060094F0 RID: 38128
	public delegate void Callback();

	// Token: 0x02001295 RID: 4757
	// (Invoke) Token: 0x060094F4 RID: 38132
	public delegate bool Condition();
}
