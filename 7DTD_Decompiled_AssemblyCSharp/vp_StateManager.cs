using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001292 RID: 4754
[Preserve]
public class vp_StateManager
{
	// Token: 0x060094D0 RID: 38096 RVA: 0x003B5D10 File Offset: 0x003B3F10
	public vp_StateManager(vp_Component component, List<vp_State> states)
	{
		this.m_States = states;
		this.m_Component = component;
		this.m_Component.RefreshDefaultState();
		this.m_StateIds = new Dictionary<string, int>(StringComparer.CurrentCulture);
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.StateManager = this;
			if (!this.m_StateIds.ContainsKey(vp_State.Name))
			{
				this.m_StateIds.Add(vp_State.Name, this.m_States.IndexOf(vp_State));
			}
			else
			{
				string[] array = new string[7];
				array[0] = "Warning: ";
				int num = 1;
				Type type = this.m_Component.GetType();
				array[num] = ((type != null) ? type.ToString() : null);
				array[2] = " on '";
				array[3] = this.m_Component.name;
				array[4] = "' has more than one state named: '";
				array[5] = vp_State.Name;
				array[6] = "'. Only the topmost one will be used.";
				Debug.LogWarning(string.Concat(array));
				this.m_States[this.m_DefaultId].StatesToBlock.Add(this.m_States.IndexOf(vp_State));
			}
			if (vp_State.Preset == null)
			{
				vp_State.Preset = new vp_ComponentPreset();
			}
			if (vp_State.TextAsset != null)
			{
				vp_State.Preset.LoadFromTextAsset(vp_State.TextAsset);
			}
		}
		this.m_DefaultId = this.m_States.Count - 1;
	}

	// Token: 0x060094D1 RID: 38097 RVA: 0x003B5EA8 File Offset: 0x003B40A8
	public void ImposeBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].AddBlocker(blocker);
		}
	}

	// Token: 0x060094D2 RID: 38098 RVA: 0x003B5F1C File Offset: 0x003B411C
	public void RelaxBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].RemoveBlocker(blocker);
		}
	}

	// Token: 0x060094D3 RID: 38099 RVA: 0x003B5F90 File Offset: 0x003B4190
	public void SetState(string state, bool setEnabled = true)
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		if (!this.m_StateIds.TryGetValue(state, out this.m_TargetId))
		{
			return;
		}
		if (this.m_TargetId == this.m_DefaultId && !setEnabled)
		{
			Debug.LogWarning(vp_StateManager.m_DefaultStateNoDisableMessage);
			return;
		}
		this.m_States[this.m_TargetId].Enabled = setEnabled;
		this.CombineStates();
		this.m_Component.Refresh();
	}

	// Token: 0x060094D4 RID: 38100 RVA: 0x003B6000 File Offset: 0x003B4200
	public void Reset()
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.Enabled = false;
		}
		this.m_States[this.m_DefaultId].Enabled = true;
		this.m_TargetId = this.m_DefaultId;
		this.CombineStates();
	}

	// Token: 0x060094D5 RID: 38101 RVA: 0x003B6084 File Offset: 0x003B4284
	public void CombineStates()
	{
		for (int i = this.m_States.Count - 1; i > -1; i--)
		{
			if ((i == this.m_DefaultId || (this.m_States[i].Enabled && !this.m_States[i].Blocked && !(this.m_States[i].TextAsset == null))) && this.m_States[i].Preset != null && !(this.m_States[i].Preset.ComponentType == null))
			{
				vp_ComponentPreset.Apply(this.m_Component, this.m_States[i].Preset);
			}
		}
	}

	// Token: 0x060094D6 RID: 38102 RVA: 0x003B6145 File Offset: 0x003B4345
	public bool IsEnabled(string state)
	{
		return vp_StateManager.AppPlaying() && this.m_StateIds.TryGetValue(state, out this.m_TargetId) && this.m_States[this.m_TargetId].Enabled;
	}

	// Token: 0x060094D7 RID: 38103 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool AppPlaying()
	{
		return true;
	}

	// Token: 0x040071CF RID: 29135
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_Component m_Component;

	// Token: 0x040071D0 RID: 29136
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<vp_State> m_States;

	// Token: 0x040071D1 RID: 29137
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, int> m_StateIds;

	// Token: 0x040071D2 RID: 29138
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_AppNotPlayingMessage = "Error: StateManager can only be accessed while application is playing.";

	// Token: 0x040071D3 RID: 29139
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_DefaultStateNoDisableMessage = "Warning: The 'Default' state cannot be disabled.";

	// Token: 0x040071D4 RID: 29140
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_DefaultId;

	// Token: 0x040071D5 RID: 29141
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_TargetId;
}
