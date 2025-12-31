using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020008AC RID: 2220
public abstract class BaseObjective
{
	// Token: 0x14000050 RID: 80
	// (add) Token: 0x060040B0 RID: 16560 RVA: 0x001A56A8 File Offset: 0x001A38A8
	// (remove) Token: 0x060040B1 RID: 16561 RVA: 0x001A56E0 File Offset: 0x001A38E0
	public event ObjectiveValueChanged ValueChanged;

	// Token: 0x170006AD RID: 1709
	// (get) Token: 0x060040B2 RID: 16562 RVA: 0x001A5715 File Offset: 0x001A3915
	// (set) Token: 0x060040B3 RID: 16563 RVA: 0x001A571D File Offset: 0x001A391D
	public byte CurrentVersion { get; set; }

	// Token: 0x170006AE RID: 1710
	// (get) Token: 0x060040B4 RID: 16564 RVA: 0x001A5726 File Offset: 0x001A3926
	// (set) Token: 0x060040B5 RID: 16565 RVA: 0x001A572E File Offset: 0x001A392E
	public BaseObjective.ObjectiveStates ObjectiveState { get; set; }

	// Token: 0x170006AF RID: 1711
	// (get) Token: 0x060040B6 RID: 16566 RVA: 0x001A5737 File Offset: 0x001A3937
	// (set) Token: 0x060040B7 RID: 16567 RVA: 0x001A574D File Offset: 0x001A394D
	public bool Complete
	{
		get
		{
			return this.ObjectiveState == BaseObjective.ObjectiveStates.Complete || this.ObjectiveState == BaseObjective.ObjectiveStates.Warning;
		}
		set
		{
			if (value)
			{
				this.ObjectiveState = BaseObjective.ObjectiveStates.Complete;
				this.DisableModifiers();
			}
		}
	}

	// Token: 0x170006B0 RID: 1712
	// (get) Token: 0x060040B8 RID: 16568 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return false;
		}
	}

	// Token: 0x170006B1 RID: 1713
	// (get) Token: 0x060040B9 RID: 16569 RVA: 0x001A575F File Offset: 0x001A395F
	// (set) Token: 0x060040BA RID: 16570 RVA: 0x001A5767 File Offset: 0x001A3967
	public QuestClass OwnerQuestClass { get; set; }

	// Token: 0x170006B2 RID: 1714
	// (get) Token: 0x060040BB RID: 16571 RVA: 0x001A5770 File Offset: 0x001A3970
	// (set) Token: 0x060040BC RID: 16572 RVA: 0x001A5778 File Offset: 0x001A3978
	public Quest OwnerQuest { get; set; }

	// Token: 0x170006B3 RID: 1715
	// (get) Token: 0x060040BD RID: 16573 RVA: 0x001A5781 File Offset: 0x001A3981
	// (set) Token: 0x060040BE RID: 16574 RVA: 0x001A5789 File Offset: 0x001A3989
	public byte Phase { get; set; }

	// Token: 0x060040BF RID: 16575 RVA: 0x001A5794 File Offset: 0x001A3994
	public BaseObjective()
	{
		this.ObjectiveState = BaseObjective.ObjectiveStates.NotStarted;
		this.Phase = 1;
	}

	// Token: 0x170006B4 RID: 1716
	// (get) Token: 0x060040C0 RID: 16576 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x170006B5 RID: 1717
	// (get) Token: 0x060040C1 RID: 16577 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool PlayObjectiveComplete
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170006B6 RID: 1718
	// (get) Token: 0x060040C2 RID: 16578 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool RequiresZombies
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060040C3 RID: 16579 RVA: 0x001A57E4 File Offset: 0x001A39E4
	[PublicizedFrom(EAccessModifier.Internal)]
	public void ChangeStatus(bool isSuccess)
	{
		this.ObjectiveState = (isSuccess ? BaseObjective.ObjectiveStates.Complete : BaseObjective.ObjectiveStates.Failed);
		if (isSuccess)
		{
			this.OwnerQuest.RallyMarkerActivated = true;
			this.OwnerQuest.RemoveMapObject();
			this.OwnerQuest.Tracked = true;
			this.OwnerQuest.OwnerJournal.TrackedQuest = this.OwnerQuest;
			this.OwnerQuest.OwnerJournal.RefreshTracked();
			this.OwnerQuest.OwnerJournal.ActiveQuest = this.OwnerQuest;
			this.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
			return;
		}
		this.OwnerQuest.CloseQuest(Quest.QuestState.Failed, null);
	}

	// Token: 0x170006B7 RID: 1719
	// (get) Token: 0x060040C4 RID: 16580 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool UpdateUI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170006B8 RID: 1720
	// (get) Token: 0x060040C5 RID: 16581 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool NeedsNPCSetPosition
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170006B9 RID: 1721
	// (get) Token: 0x060040C6 RID: 16582 RVA: 0x001A587D File Offset: 0x001A3A7D
	// (set) Token: 0x060040C7 RID: 16583 RVA: 0x001A589A File Offset: 0x001A3A9A
	public string Description
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupDisplay();
				this.displaySetup = true;
			}
			return this.description;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.description = value;
		}
	}

	// Token: 0x170006BA RID: 1722
	// (get) Token: 0x060040C8 RID: 16584 RVA: 0x001A58A3 File Offset: 0x001A3AA3
	// (set) Token: 0x060040C9 RID: 16585 RVA: 0x001A58C0 File Offset: 0x001A3AC0
	public virtual string StatusText
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupDisplay();
				this.displaySetup = true;
			}
			return this.statusText;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.statusText = value;
		}
	}

	// Token: 0x170006BB RID: 1723
	// (get) Token: 0x060040CA RID: 16586 RVA: 0x001A58C9 File Offset: 0x001A3AC9
	// (set) Token: 0x060040CB RID: 16587 RVA: 0x001A58D1 File Offset: 0x001A3AD1
	public byte CurrentValue
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			this.currentValue = value;
			this.SetupDisplay();
			if (this.ValueChanged != null)
			{
				this.ValueChanged();
			}
		}
	}

	// Token: 0x170006BC RID: 1724
	// (get) Token: 0x060040CC RID: 16588 RVA: 0x001A58F3 File Offset: 0x001A3AF3
	// (set) Token: 0x060040CD RID: 16589 RVA: 0x001A58FB File Offset: 0x001A3AFB
	public bool Optional { get; set; }

	// Token: 0x170006BD RID: 1725
	// (get) Token: 0x060040CE RID: 16590 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool AlwaysComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170006BE RID: 1726
	// (get) Token: 0x060040CF RID: 16591 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool ShowInQuestLog
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060040D0 RID: 16592 RVA: 0x001A5904 File Offset: 0x001A3B04
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void CopyValues(BaseObjective objective)
	{
		objective.ID = this.ID;
		objective.Value = this.Value;
		objective.Optional = this.Optional;
		objective.currentValue = this.currentValue;
		objective.Phase = this.Phase;
		objective.NavObjectName = this.NavObjectName;
		objective.HiddenObjective = this.HiddenObjective;
		objective.ForcePhaseFinish = this.ForcePhaseFinish;
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				objective.AddModifier(this.Modifiers[i].Clone());
			}
		}
	}

	// Token: 0x060040D1 RID: 16593 RVA: 0x001A59A6 File Offset: 0x001A3BA6
	public virtual void HandleVariables()
	{
		this.ID = this.OwnerQuest.ParseVariable(this.ID);
		this.Value = this.OwnerQuest.ParseVariable(this.Value);
	}

	// Token: 0x060040D2 RID: 16594 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupQuestTag()
	{
	}

	// Token: 0x060040D3 RID: 16595 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupObjective()
	{
	}

	// Token: 0x060040D4 RID: 16596 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupDisplay()
	{
	}

	// Token: 0x060040D5 RID: 16597 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool SetupPosition(EntityNPC ownerNPC = null, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		return false;
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool SetupActivationList(Vector3 prefabPos, List<Vector3i> activateList)
	{
		return false;
	}

	// Token: 0x060040D7 RID: 16599 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetPosition(Vector3 position, Vector3 size)
	{
	}

	// Token: 0x060040D8 RID: 16600 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetPosition(Quest.PositionDataTypes dataType, Vector3i position)
	{
	}

	// Token: 0x060040D9 RID: 16601 RVA: 0x001A59D8 File Offset: 0x001A3BD8
	public void HandleAddHooks()
	{
		this.AddHooks();
		if (!this.Complete && this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].OwnerObjective = this;
				this.Modifiers[i].HandleAddHooks();
			}
		}
		if (this.useUpdateLoop)
		{
			QuestEventManager.Current.AddObjectiveToBeUpdated(this);
		}
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x001A5A48 File Offset: 0x001A3C48
	public void HandleRemoveHooks()
	{
		this.RemoveHooks();
		this.RemoveNavObject();
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].HandleRemoveHooks();
			}
		}
		if (this.useUpdateLoop)
		{
			QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		}
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void AddHooks()
	{
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x001A5AA3 File Offset: 0x001A3CA3
	public virtual void AddNavObject(Vector3 position)
	{
		if (this.NavObjectName != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(this.NavObjectName, position, "", false, -1, null);
		}
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RemoveHooks()
	{
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x001A5AD6 File Offset: 0x001A3CD6
	public virtual void RemoveNavObject()
	{
		if (this.NavObject != null)
		{
			NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
			this.NavObject = null;
		}
	}

	// Token: 0x060040DF RID: 16607 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Refresh()
	{
	}

	// Token: 0x060040E0 RID: 16608 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RemoveObjectives()
	{
	}

	// Token: 0x060040E1 RID: 16609 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandleCompleted()
	{
	}

	// Token: 0x060040E2 RID: 16610 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandlePhaseCompleted()
	{
	}

	// Token: 0x060040E3 RID: 16611 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandleFailed()
	{
	}

	// Token: 0x060040E4 RID: 16612 RVA: 0x001A5AF7 File Offset: 0x001A3CF7
	public virtual void ResetObjective()
	{
		this.CurrentValue = 0;
	}

	// Token: 0x060040E5 RID: 16613 RVA: 0x001A5B00 File Offset: 0x001A3D00
	public virtual void Read(BinaryReader _br)
	{
		this.CurrentVersion = _br.ReadByte();
		this.currentValue = _br.ReadByte();
	}

	// Token: 0x060040E6 RID: 16614 RVA: 0x001A5B1A File Offset: 0x001A3D1A
	public virtual void Write(BinaryWriter _bw)
	{
		_bw.Write(BaseObjective.FileVersion);
		_bw.Write(this.CurrentValue);
	}

	// Token: 0x060040E7 RID: 16615 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseObjective Clone()
	{
		return null;
	}

	// Token: 0x060040E8 RID: 16616 RVA: 0x001A5B33 File Offset: 0x001A3D33
	public void HandleUpdate(float deltaTime)
	{
		if (this.Phase == this.OwnerQuest.CurrentPhase)
		{
			this.Update(deltaTime);
		}
	}

	// Token: 0x060040E9 RID: 16617 RVA: 0x001A5B50 File Offset: 0x001A3D50
	public virtual void Update(float deltaTime)
	{
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 1f;
			switch (this.CurrentValue)
			{
			case 0:
				this.UpdateState_NeedSetup();
				return;
			case 1:
				this.UpdateState_WaitingForServer();
				return;
			case 2:
				this.UpdateState_Update();
				return;
			case 3:
				this.UpdateState_Completed();
				QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_NeedSetup()
	{
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_WaitingForServer()
	{
	}

	// Token: 0x060040EC RID: 16620 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_Update()
	{
	}

	// Token: 0x060040ED RID: 16621 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_Completed()
	{
	}

	// Token: 0x060040EE RID: 16622 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool SetLocation(Vector3 pos, Vector3 size)
	{
		return false;
	}

	// Token: 0x060040EF RID: 16623 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string ParseBinding(string bindingName)
	{
		return "";
	}

	// Token: 0x060040F0 RID: 16624 RVA: 0x001A5BC0 File Offset: 0x001A3DC0
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.OwnerQuestClass.HandleVariablesForProperties(properties);
		if (properties.Values.ContainsKey(BaseObjective.PropID))
		{
			this.ID = properties.Values[BaseObjective.PropID];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropValue))
		{
			this.Value = properties.Values[BaseObjective.PropValue];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropPhase))
		{
			this.Phase = Convert.ToByte(properties.Values[BaseObjective.PropPhase]);
			if (this.Phase > this.OwnerQuestClass.HighestPhase)
			{
				this.OwnerQuestClass.HighestPhase = this.Phase;
			}
		}
		if (properties.Values.ContainsKey(BaseObjective.PropOptional))
		{
			bool optional;
			StringParsers.TryParseBool(properties.Values[BaseObjective.PropOptional], out optional, 0, -1, true);
			this.Optional = optional;
		}
		if (properties.Values.ContainsKey(BaseObjective.PropNavObject))
		{
			this.NavObjectName = properties.Values[BaseObjective.PropNavObject];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropHidden))
		{
			this.HiddenObjective = StringParsers.ParseBool(properties.Values[BaseObjective.PropHidden], 0, -1, true);
		}
		properties.ParseBool(BaseObjective.PropForcePhaseFinish, ref this.ForcePhaseFinish);
	}

	// Token: 0x060040F1 RID: 16625 RVA: 0x001A5D1E File Offset: 0x001A3F1E
	public void AddModifier(BaseObjectiveModifier modifier)
	{
		if (this.Modifiers == null)
		{
			this.Modifiers = new List<BaseObjectiveModifier>();
		}
		this.Modifiers.Add(modifier);
		modifier.OwnerObjective = this;
	}

	// Token: 0x060040F2 RID: 16626 RVA: 0x001A5D48 File Offset: 0x001A3F48
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisableModifiers()
	{
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].HandleRemoveHooks();
			}
		}
	}

	// Token: 0x040033C6 RID: 13254
	public static byte FileVersion = 0;

	// Token: 0x040033C7 RID: 13255
	public static string PropID = "id";

	// Token: 0x040033C8 RID: 13256
	public static string PropValue = "value";

	// Token: 0x040033C9 RID: 13257
	public static string PropPhase = "phase";

	// Token: 0x040033CA RID: 13258
	public static string PropOptional = "optional";

	// Token: 0x040033CB RID: 13259
	public static string PropNavObject = "nav_object";

	// Token: 0x040033CC RID: 13260
	public static string PropHidden = "hidden";

	// Token: 0x040033CD RID: 13261
	public static string PropForcePhaseFinish = "force_phase_finish";

	// Token: 0x040033CF RID: 13263
	public string ID;

	// Token: 0x040033D1 RID: 13265
	public string Value;

	// Token: 0x040033D5 RID: 13269
	[PublicizedFrom(EAccessModifier.Private)]
	public bool displaySetup;

	// Token: 0x040033D7 RID: 13271
	[PublicizedFrom(EAccessModifier.Protected)]
	public string keyword = "";

	// Token: 0x040033D8 RID: 13272
	[PublicizedFrom(EAccessModifier.Private)]
	public string description = "";

	// Token: 0x040033D9 RID: 13273
	[PublicizedFrom(EAccessModifier.Private)]
	public string statusText = "";

	// Token: 0x040033DA RID: 13274
	public bool HiddenObjective;

	// Token: 0x040033DB RID: 13275
	public bool ForcePhaseFinish;

	// Token: 0x040033DC RID: 13276
	[PublicizedFrom(EAccessModifier.Protected)]
	public NavObject NavObject;

	// Token: 0x040033DD RID: 13277
	[PublicizedFrom(EAccessModifier.Protected)]
	public string NavObjectName = "";

	// Token: 0x040033DE RID: 13278
	public List<BaseObjectiveModifier> Modifiers;

	// Token: 0x040033DF RID: 13279
	public DynamicProperties Properties;

	// Token: 0x040033E0 RID: 13280
	[PublicizedFrom(EAccessModifier.Protected)]
	public byte currentValue;

	// Token: 0x040033E2 RID: 13282
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;

	// Token: 0x020008AD RID: 2221
	public enum ObjectiveStates
	{
		// Token: 0x040033E4 RID: 13284
		NotStarted,
		// Token: 0x040033E5 RID: 13285
		InProgress,
		// Token: 0x040033E6 RID: 13286
		Warning,
		// Token: 0x040033E7 RID: 13287
		Complete,
		// Token: 0x040033E8 RID: 13288
		Failed
	}

	// Token: 0x020008AE RID: 2222
	public enum ObjectiveTypes
	{
		// Token: 0x040033EA RID: 13290
		AnimalKill,
		// Token: 0x040033EB RID: 13291
		Assemble,
		// Token: 0x040033EC RID: 13292
		BlockPickup,
		// Token: 0x040033ED RID: 13293
		BlockPlace,
		// Token: 0x040033EE RID: 13294
		BlockUpgrade,
		// Token: 0x040033EF RID: 13295
		Buff,
		// Token: 0x040033F0 RID: 13296
		ExchangeItemFrom,
		// Token: 0x040033F1 RID: 13297
		Fetch,
		// Token: 0x040033F2 RID: 13298
		FetchKeep,
		// Token: 0x040033F3 RID: 13299
		CraftItem,
		// Token: 0x040033F4 RID: 13300
		Repair,
		// Token: 0x040033F5 RID: 13301
		Scrap,
		// Token: 0x040033F6 RID: 13302
		SkillsPurchased,
		// Token: 0x040033F7 RID: 13303
		Time,
		// Token: 0x040033F8 RID: 13304
		Wear,
		// Token: 0x040033F9 RID: 13305
		WindowOpen,
		// Token: 0x040033FA RID: 13306
		ZombieKill
	}

	// Token: 0x020008AF RID: 2223
	public enum ObjectiveValueTypes
	{
		// Token: 0x040033FC RID: 13308
		Boolean,
		// Token: 0x040033FD RID: 13309
		Number,
		// Token: 0x040033FE RID: 13310
		Time,
		// Token: 0x040033FF RID: 13311
		Distance
	}

	// Token: 0x020008B0 RID: 2224
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum UpdateStates
	{
		// Token: 0x04003401 RID: 13313
		NeedSetup,
		// Token: 0x04003402 RID: 13314
		WaitingForServer,
		// Token: 0x04003403 RID: 13315
		Update,
		// Token: 0x04003404 RID: 13316
		Completed
	}
}
