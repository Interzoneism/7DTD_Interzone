using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008C7 RID: 2247
[Preserve]
public class ObjectiveInteractWithNPC : BaseObjective
{
	// Token: 0x060041EA RID: 16874 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupObjective()
	{
	}

	// Token: 0x060041EB RID: 16875 RVA: 0x001AA257 File Offset: 0x001A8457
	public override void SetupDisplay()
	{
		base.Description = Localization.Get("ObjectiveTalkToTrader_keyword", false);
		this.StatusText = "";
	}

	// Token: 0x170006D7 RID: 1751
	// (get) Token: 0x060041EC RID: 16876 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool PlayObjectiveComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060041ED RID: 16877 RVA: 0x001AA278 File Offset: 0x001A8478
	public override void AddHooks()
	{
		QuestEventManager.Current.NPCInteract += this.Current_NPCInteract;
		if (this.useClosest)
		{
			List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(null, new Bounds(base.OwnerQuest.Position, new Vector3(50f, 50f, 50f)));
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				if (entitiesInBounds[i] is EntityNPC)
				{
					base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.QuestGiver, entitiesInBounds[i].position);
					base.OwnerQuest.QuestGiverID = entitiesInBounds[i].entityId;
					base.OwnerQuest.QuestFaction = ((EntityNPC)entitiesInBounds[i]).NPCInfo.QuestFaction;
					base.OwnerQuest.RallyMarkerActivated = true;
					base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.QuestGiver, this.NavObjectName, -1);
					return;
				}
			}
		}
	}

	// Token: 0x060041EE RID: 16878 RVA: 0x001AA370 File Offset: 0x001A8570
	public override void RemoveHooks()
	{
		QuestEventManager.Current.NPCInteract -= this.Current_NPCInteract;
	}

	// Token: 0x060041EF RID: 16879 RVA: 0x001AA388 File Offset: 0x001A8588
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_NPCInteract(EntityNPC npc)
	{
		if ((!base.OwnerQuest.QuestClass.ReturnToQuestGiver || base.OwnerQuest.QuestGiverID == -1 || base.OwnerQuest.CheckIsQuestGiver(npc.entityId)) && base.OwnerQuest.CheckRequirements())
		{
			if (base.OwnerQuest.QuestFaction == 0)
			{
				base.OwnerQuest.QuestFaction = npc.NPCInfo.QuestFaction;
			}
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x060041F0 RID: 16880 RVA: 0x001AA408 File Offset: 0x001A8608
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, this.PlayObjectiveComplete, null);
		}
	}

	// Token: 0x060041F1 RID: 16881 RVA: 0x001AA44C File Offset: 0x001A864C
	public override BaseObjective Clone()
	{
		ObjectiveInteractWithNPC objectiveInteractWithNPC = new ObjectiveInteractWithNPC();
		this.CopyValues(objectiveInteractWithNPC);
		objectiveInteractWithNPC.useClosest = this.useClosest;
		return objectiveInteractWithNPC;
	}

	// Token: 0x060041F2 RID: 16882 RVA: 0x001AA473 File Offset: 0x001A8673
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveInteractWithNPC.PropUseClosest))
		{
			this.useClosest = StringParsers.ParseBool(properties.Values[ObjectiveInteractWithNPC.PropUseClosest], 0, -1, true);
		}
	}

	// Token: 0x0400346C RID: 13420
	public static string PropUseClosest = "use_closest";

	// Token: 0x0400346D RID: 13421
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useClosest;
}
