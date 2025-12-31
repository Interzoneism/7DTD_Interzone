using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001291 RID: 4753
[Preserve]
[Serializable]
public class vp_State
{
	// Token: 0x060094C8 RID: 38088 RVA: 0x003B5C43 File Offset: 0x003B3E43
	public vp_State(string typeName, string name = "Untitled", string path = null, TextAsset asset = null)
	{
		this.TypeName = typeName;
		this.Name = name;
		this.TextAsset = asset;
	}

	// Token: 0x17000F44 RID: 3908
	// (get) Token: 0x060094C9 RID: 38089 RVA: 0x003B5C61 File Offset: 0x003B3E61
	// (set) Token: 0x060094CA RID: 38090 RVA: 0x003B5C69 File Offset: 0x003B3E69
	public bool Enabled
	{
		get
		{
			return this.m_Enabled;
		}
		set
		{
			this.m_Enabled = value;
			if (this.StateManager == null)
			{
				return;
			}
			if (this.m_Enabled)
			{
				this.StateManager.ImposeBlockingList(this);
				return;
			}
			this.StateManager.RelaxBlockingList(this);
		}
	}

	// Token: 0x17000F45 RID: 3909
	// (get) Token: 0x060094CB RID: 38091 RVA: 0x003B5C9C File Offset: 0x003B3E9C
	public bool Blocked
	{
		get
		{
			return this.CurrentlyBlockedBy.Count > 0;
		}
	}

	// Token: 0x17000F46 RID: 3910
	// (get) Token: 0x060094CC RID: 38092 RVA: 0x003B5CAC File Offset: 0x003B3EAC
	public int BlockCount
	{
		get
		{
			return this.CurrentlyBlockedBy.Count;
		}
	}

	// Token: 0x17000F47 RID: 3911
	// (get) Token: 0x060094CD RID: 38093 RVA: 0x003B5CB9 File Offset: 0x003B3EB9
	public List<vp_State> CurrentlyBlockedBy
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CurrentlyBlockedBy == null)
			{
				this.m_CurrentlyBlockedBy = new List<vp_State>();
			}
			return this.m_CurrentlyBlockedBy;
		}
	}

	// Token: 0x060094CE RID: 38094 RVA: 0x003B5CD4 File Offset: 0x003B3ED4
	public void AddBlocker(vp_State blocker)
	{
		if (!this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Add(blocker);
		}
	}

	// Token: 0x060094CF RID: 38095 RVA: 0x003B5CF0 File Offset: 0x003B3EF0
	public void RemoveBlocker(vp_State blocker)
	{
		if (this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Remove(blocker);
		}
	}

	// Token: 0x040071C7 RID: 29127
	public vp_StateManager StateManager;

	// Token: 0x040071C8 RID: 29128
	public string TypeName;

	// Token: 0x040071C9 RID: 29129
	public string Name;

	// Token: 0x040071CA RID: 29130
	public TextAsset TextAsset;

	// Token: 0x040071CB RID: 29131
	public vp_ComponentPreset Preset;

	// Token: 0x040071CC RID: 29132
	public List<int> StatesToBlock;

	// Token: 0x040071CD RID: 29133
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Enabled;

	// Token: 0x040071CE RID: 29134
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<vp_State> m_CurrentlyBlockedBy;
}
