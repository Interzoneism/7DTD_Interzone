using System;
using System.Collections.Generic;
using GameEvent.SequenceActions;
using GameEvent.SequenceRequirements;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class GameEventActionSequence
{
	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x060027A2 RID: 10146 RVA: 0x001009AF File Offset: 0x000FEBAF
	public GameEventVariables EventVariables
	{
		get
		{
			if (this.eventVariables == null)
			{
				this.eventVariables = new GameEventVariables();
			}
			return this.eventVariables;
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x001009CC File Offset: 0x000FEBCC
	public bool HasTarget()
	{
		if (this.TargetType == GameEventActionSequence.TargetTypes.Entity)
		{
			return this.Target != null && !this.DeadCheck;
		}
		if (this.TargetType == GameEventActionSequence.TargetTypes.POI)
		{
			return this.POIPosition != Vector3i.zero;
		}
		return this.blockValue.type != GameManager.Instance.World.GetBlock(this.POIPosition).type || this.AllowWhileDead;
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x060027A4 RID: 10148 RVA: 0x00100A47 File Offset: 0x000FEC47
	public bool DeadCheck
	{
		get
		{
			return !this.Target.IsAlive() && !this.AllowWhileDead;
		}
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x00100A64 File Offset: 0x000FEC64
	public void SetupTarget()
	{
		if (this.TargetType == GameEventActionSequence.TargetTypes.POI)
		{
			if (this.POIPosition != Vector3i.zero)
			{
				this.POIInstance = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos(this.POIPosition.x, this.POIPosition.z);
				return;
			}
			EntityPlayer entityPlayer = this.Target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.POIInstance = entityPlayer.prefab;
				if (this.POIInstance != null)
				{
					this.POIPosition = this.POIInstance.boundingBoxPosition;
					return;
				}
			}
		}
		else if (this.TargetType == GameEventActionSequence.TargetTypes.Entity)
		{
			EntityPlayer entityPlayer2 = this.Target as EntityPlayer;
			if (entityPlayer2 != null)
			{
				this.POIInstance = entityPlayer2.prefab;
				if (this.POIInstance != null)
				{
					this.POIPosition = this.POIInstance.boundingBoxPosition;
					return;
				}
			}
		}
		else if (this.TargetType == GameEventActionSequence.TargetTypes.Block)
		{
			if (this.POIPosition != Vector3i.zero)
			{
				this.POIInstance = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos(this.POIPosition.x, this.POIPosition.z);
				return;
			}
			this.POIInstance = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)this.TargetPosition.x, (int)this.TargetPosition.z);
		}
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x00100BAA File Offset: 0x000FEDAA
	public void StartSequence(GameEventManager manager)
	{
		this.StartTime = Time.time;
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x00100BB7 File Offset: 0x000FEDB7
	public void Init()
	{
		this.OnInit();
		this.IsComplete = false;
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x00100BC8 File Offset: 0x000FEDC8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnInit()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.Actions.Count; i++)
		{
			if (!list.Contains(this.Actions[i].Phase))
			{
				list.Add(this.Actions[i].Phase);
			}
			this.Actions[i].ActionIndex = i;
		}
		list.Sort();
		if (list.Count > 0)
		{
			this.PhaseMax = list[list.Count - 1] + 1;
			return;
		}
		this.PhaseMax = 0;
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x00100C60 File Offset: 0x000FEE60
	public bool CanPerform(Entity player)
	{
		for (int i = 0; i < this.Requirements.Count; i++)
		{
			if (!this.Requirements[i].CanPerform(player))
			{
				return false;
			}
		}
		for (int j = 0; j < this.Actions.Count; j++)
		{
			if (!this.Actions[j].CanPerform(player))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x00100CC8 File Offset: 0x000FEEC8
	public void HandleVariablesForProperties(DynamicProperties properties)
	{
		if (properties == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> keyValuePair in properties.Params1.Dict)
		{
			if (this.Variables.ContainsKey(keyValuePair.Value))
			{
				properties.Values[keyValuePair.Key] = this.Variables[keyValuePair.Value];
			}
		}
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x00100D58 File Offset: 0x000FEF58
	public void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		if (properties.Values.ContainsKey(GameEventActionSequence.PropAllowUserTrigger))
		{
			this.AllowUserTrigger = StringParsers.ParseBool(properties.Values[GameEventActionSequence.PropAllowUserTrigger], 0, -1, true);
		}
		properties.ParseEnum<GameEventActionSequence.ActionTypes>(GameEventActionSequence.PropActionType, ref this.ActionType);
		if (properties.Values.ContainsKey(GameEventActionSequence.PropAllowWhileDead))
		{
			this.AllowWhileDead = StringParsers.ParseBool(properties.Values[GameEventActionSequence.PropAllowWhileDead], 0, -1, true);
		}
		properties.ParseEnum<GameEventActionSequence.TargetTypes>(GameEventActionSequence.PropTargetType, ref this.TargetType);
		properties.ParseBool(GameEventActionSequence.PropRefundInactivity, ref this.RefundInactivity);
		properties.ParseBool(GameEventActionSequence.PropSingleInstance, ref this.SingleInstance);
		string text = "";
		properties.ParseString(GameEventActionSequence.PropCategory, ref text);
		if (text != "")
		{
			this.CategoryNames = text.Split(',', StringSplitOptions.None);
		}
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x00100E40 File Offset: 0x000FF040
	public void Update()
	{
		bool flag = false;
		int num = this.CurrentPhase;
		for (int i = 0; i < this.Actions.Count; i++)
		{
			if (this.Actions[i].Phase == this.CurrentPhase && !this.Actions[i].IsComplete)
			{
				flag = true;
				BaseAction.ActionCompleteStates actionCompleteStates;
				if (this.AllowRefunds && this.RefundInactivity && Time.time - this.StartTime > 60f)
				{
					actionCompleteStates = BaseAction.ActionCompleteStates.InCompleteRefund;
				}
				else
				{
					actionCompleteStates = this.Actions[i].PerformAction();
				}
				if (actionCompleteStates == BaseAction.ActionCompleteStates.Complete || (actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund && this.Actions[i].IgnoreRefund))
				{
					this.Actions[i].IsComplete = true;
					if (this.Actions[i].PhaseOnComplete != -1)
					{
						num = this.Actions[i].PhaseOnComplete;
					}
				}
				else if (this.AllowRefunds && actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund)
				{
					if (this.ActionType == GameEventActionSequence.ActionTypes.TwitchAction)
					{
						if (this.Requester is EntityPlayerLocal)
						{
							GameEventManager.Current.HandleTwitchRefundNeeded(this.Name, this.Target.entityId, this.ExtraData, this.Tag);
						}
						else
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.Name, this.Target.entityId, this.ExtraData, this.Tag, NetPackageGameEventResponse.ResponseTypes.TwitchRefundNeeded, -1, -1, false), false, this.Requester.entityId, -1, -1, null, 192, false);
						}
						this.IsComplete = true;
					}
					else
					{
						this.Actions[i].IsComplete = true;
					}
				}
			}
		}
		if (!flag)
		{
			this.CurrentPhase++;
		}
		else if (this.CurrentPhase != num)
		{
			this.CurrentPhase = num;
			for (int j = 0; j < this.Actions.Count; j++)
			{
				if (this.Actions[j].Phase >= this.CurrentPhase)
				{
					this.Actions[j].Reset();
				}
			}
		}
		if (this.CurrentPhase >= this.PhaseMax)
		{
			this.IsComplete = true;
			if (this.Requester != null)
			{
				if (this.Requester is EntityPlayerLocal)
				{
					GameEventManager.Current.HandleGameEventCompleted(this.Name, this.Target ? this.Target.entityId : -1, this.ExtraData, this.Tag);
					return;
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.Name, this.Target ? this.Target.entityId : -1, this.ExtraData, this.Tag, NetPackageGameEventResponse.ResponseTypes.Completed, -1, -1, false), false, this.Requester.entityId, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x00101138 File Offset: 0x000FF338
	public void HandleClientPerform(EntityPlayer player, int index)
	{
		this.Actions[index].OnClientPerform(player);
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x0010114C File Offset: 0x000FF34C
	public void AddEntitiesToGroup(string groupName, List<Entity> entityList, bool twitchNegative)
	{
		for (int i = entityList.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = entityList[i] as EntityPlayer;
			if (entityPlayer != null)
			{
				EntityPlayer.TwitchActionsStates twitchActionsEnabled = entityPlayer.TwitchActionsEnabled;
				if (twitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled && (twitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled || twitchNegative))
				{
					entityList.RemoveAt(i);
				}
			}
		}
		if (entityList.Count == 0)
		{
			return;
		}
		if (this.EntityGroups == null)
		{
			this.EntityGroups = new Dictionary<string, List<Entity>>();
		}
		if (this.EntityGroups.ContainsKey(groupName))
		{
			this.EntityGroups[groupName] = entityList;
			return;
		}
		this.EntityGroups.Add(groupName, entityList);
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x001011DC File Offset: 0x000FF3DC
	public void AddEntityToGroup(string groupName, Entity entity)
	{
		if (this.ActionType == GameEventActionSequence.ActionTypes.TwitchAction && entity is EntityPlayer && (entity as EntityPlayer).TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
		{
			return;
		}
		if (this.EntityGroups == null)
		{
			this.EntityGroups = new Dictionary<string, List<Entity>>();
		}
		if (!this.EntityGroups.ContainsKey(groupName))
		{
			this.EntityGroups.Add(groupName, new List<Entity>());
		}
		this.EntityGroups[groupName].Add(entity);
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x0010124C File Offset: 0x000FF44C
	public List<Entity> GetEntityGroup(string groupName)
	{
		if (this.EntityGroups == null || !this.EntityGroups.ContainsKey(groupName))
		{
			return null;
		}
		return this.EntityGroups[groupName];
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x00101274 File Offset: 0x000FF474
	public int GetEntityGroupLiveCount(string groupName)
	{
		if (this.EntityGroups == null || !this.EntityGroups.ContainsKey(groupName))
		{
			return 0;
		}
		int num = 0;
		List<Entity> list = this.EntityGroups[groupName];
		for (int i = 0; i < list.Count; i++)
		{
			EntityAlive entityAlive = list[i] as EntityAlive;
			if (entityAlive != null && entityAlive.IsAlive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x001012D6 File Offset: 0x000FF4D6
	public void ClearEntityGroup(string groupName)
	{
		if (this.EntityGroups == null || !this.EntityGroups.ContainsKey(groupName))
		{
			return;
		}
		this.EntityGroups[groupName].Clear();
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x00101300 File Offset: 0x000FF500
	public GameEventActionSequence Clone()
	{
		GameEventActionSequence gameEventActionSequence = new GameEventActionSequence();
		gameEventActionSequence.Name = this.Name;
		gameEventActionSequence.PhaseMax = this.PhaseMax;
		gameEventActionSequence.CurrentPhase = this.CurrentPhase;
		gameEventActionSequence.AllowUserTrigger = this.AllowUserTrigger;
		gameEventActionSequence.AllowWhileDead = this.AllowWhileDead;
		gameEventActionSequence.ActionType = this.ActionType;
		gameEventActionSequence.CrateShare = this.CrateShare;
		gameEventActionSequence.TargetType = this.TargetType;
		gameEventActionSequence.SingleInstance = this.SingleInstance;
		gameEventActionSequence.RefundInactivity = this.RefundInactivity;
		for (int i = 0; i < this.Actions.Count; i++)
		{
			BaseAction baseAction = this.Actions[i].Clone();
			baseAction.Owner = gameEventActionSequence;
			gameEventActionSequence.Actions.Add(baseAction);
		}
		return gameEventActionSequence;
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x001013C8 File Offset: 0x000FF5C8
	[PublicizedFrom(EAccessModifier.Internal)]
	public DynamicProperties AssignValuesFrom(GameEventActionSequence oldSeq)
	{
		DynamicProperties dynamicProperties = new DynamicProperties();
		HashSet<string> exclude = new HashSet<string>
		{
			GameEventActionSequence.PropAllowUserTrigger
		};
		if (oldSeq.Properties != null)
		{
			dynamicProperties.CopyFrom(oldSeq.Properties, exclude);
		}
		for (int i = 0; i < oldSeq.Requirements.Count; i++)
		{
			BaseRequirement baseRequirement = oldSeq.Requirements[i].Clone();
			baseRequirement.Properties = new DynamicProperties();
			if (oldSeq.Requirements[i].Properties != null)
			{
				baseRequirement.Properties.CopyFrom(oldSeq.Requirements[i].Properties, null);
			}
			baseRequirement.Owner = this;
			baseRequirement.Init();
			this.Requirements.Add(baseRequirement);
		}
		for (int j = 0; j < oldSeq.Actions.Count; j++)
		{
			BaseAction item = oldSeq.Actions[j].HandleAssignFrom(this, oldSeq);
			this.Actions.Add(item);
		}
		return dynamicProperties;
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x001014BC File Offset: 0x000FF6BC
	public void HandleTemplateInit()
	{
		for (int i = 0; i < this.Actions.Count; i++)
		{
			this.Actions[i].HandleTemplateInit(this);
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			this.HandleVariablesForProperties(this.Requirements[j].Properties);
			this.Requirements[j].ParseProperties(this.Requirements[j].Properties);
			this.Requirements[j].Init();
		}
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x00101554 File Offset: 0x000FF754
	public void SetRefundNeeded()
	{
		if (this.ActionType == GameEventActionSequence.ActionTypes.TwitchAction)
		{
			if (this.Requester is EntityPlayerLocal)
			{
				GameEventManager.Current.HandleTwitchRefundNeeded(this.Name, this.Target.entityId, this.ExtraData, this.Tag);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.Name, this.Target.entityId, this.ExtraData, this.Tag, NetPackageGameEventResponse.ResponseTypes.TwitchRefundNeeded, -1, -1, false), false, this.Requester.entityId, -1, -1, null, 192, false);
			}
			this.IsComplete = true;
		}
	}

	// Token: 0x04001E37 RID: 7735
	public string Name;

	// Token: 0x04001E38 RID: 7736
	public int PhaseMax = 1;

	// Token: 0x04001E39 RID: 7737
	public int CurrentPhase;

	// Token: 0x04001E3A RID: 7738
	public string ExtraData = "";

	// Token: 0x04001E3B RID: 7739
	public string Tag = "";

	// Token: 0x04001E3C RID: 7740
	public int ReservedSpawnCount;

	// Token: 0x04001E3D RID: 7741
	public GameEventActionSequence.ActionTypes ActionType;

	// Token: 0x04001E3E RID: 7742
	public bool AllowUserTrigger = true;

	// Token: 0x04001E3F RID: 7743
	public bool AllowWhileDead;

	// Token: 0x04001E40 RID: 7744
	public bool RefundInactivity = true;

	// Token: 0x04001E41 RID: 7745
	public bool CrateShare;

	// Token: 0x04001E42 RID: 7746
	public bool SingleInstance;

	// Token: 0x04001E43 RID: 7747
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropAllowUserTrigger = "allow_user_trigger";

	// Token: 0x04001E44 RID: 7748
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActionType = "action_type";

	// Token: 0x04001E45 RID: 7749
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropAllowWhileDead = "allow_while_dead";

	// Token: 0x04001E46 RID: 7750
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTargetType = "target_type";

	// Token: 0x04001E47 RID: 7751
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRefundInactivity = "refund_inactivity";

	// Token: 0x04001E48 RID: 7752
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCategory = "category";

	// Token: 0x04001E49 RID: 7753
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSingleInstance = "single_instance";

	// Token: 0x04001E4A RID: 7754
	public Dictionary<string, List<Entity>> EntityGroups;

	// Token: 0x04001E4B RID: 7755
	public string[] CategoryNames;

	// Token: 0x04001E4C RID: 7756
	public List<BaseRequirement> Requirements = new List<BaseRequirement>();

	// Token: 0x04001E4D RID: 7757
	public List<BaseAction> Actions = new List<BaseAction>();

	// Token: 0x04001E4E RID: 7758
	public EntityPlayer Requester;

	// Token: 0x04001E4F RID: 7759
	public Entity Target;

	// Token: 0x04001E50 RID: 7760
	public Vector3 TargetPosition;

	// Token: 0x04001E51 RID: 7761
	public Vector3i POIPosition;

	// Token: 0x04001E52 RID: 7762
	public GameEventActionSequence.TargetTypes TargetType;

	// Token: 0x04001E53 RID: 7763
	public PrefabInstance POIInstance;

	// Token: 0x04001E54 RID: 7764
	public BlockValue blockValue;

	// Token: 0x04001E55 RID: 7765
	public int CurrentBossGroupID = -1;

	// Token: 0x04001E56 RID: 7766
	public bool IsComplete;

	// Token: 0x04001E57 RID: 7767
	public bool AllowRefunds = true;

	// Token: 0x04001E58 RID: 7768
	public bool TwitchActivated;

	// Token: 0x04001E59 RID: 7769
	public Dictionary<string, string> Variables = new Dictionary<string, string>();

	// Token: 0x04001E5A RID: 7770
	public GameEventVariables eventVariables;

	// Token: 0x04001E5B RID: 7771
	public DynamicProperties Properties;

	// Token: 0x04001E5C RID: 7772
	public float StartTime = -1f;

	// Token: 0x04001E5D RID: 7773
	public bool HasDespawn;

	// Token: 0x04001E5E RID: 7774
	public GameEventActionSequence OwnerSequence;

	// Token: 0x020004B9 RID: 1209
	public enum ActionTypes
	{
		// Token: 0x04001E60 RID: 7776
		TwitchAction,
		// Token: 0x04001E61 RID: 7777
		TwitchVote,
		// Token: 0x04001E62 RID: 7778
		Game
	}

	// Token: 0x020004BA RID: 1210
	public enum TargetTypes
	{
		// Token: 0x04001E64 RID: 7780
		Entity,
		// Token: 0x04001E65 RID: 7781
		POI,
		// Token: 0x04001E66 RID: 7782
		Block
	}
}
