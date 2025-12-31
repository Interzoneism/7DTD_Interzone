using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008D4 RID: 2260
[Preserve]
public class ObjectiveRandomGotoNPC : ObjectiveRandomGoto
{
	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x06004279 RID: 17017 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NeedsNPCSetPosition
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600427A RID: 17018 RVA: 0x001AD849 File Offset: 0x001ABA49
	public override bool SetupPosition(EntityNPC ownerNPC = null, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		return this.GetPosition(ownerNPC) != Vector3.zero;
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x001AD85C File Offset: 0x001ABA5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 GetPosition(EntityNPC ownerNPC)
	{
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.Location))
		{
			base.OwnerQuest.Position = this.position;
			this.positionSet = true;
			base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.Location, this.NavObjectName, -1);
			base.CurrentValue = 2;
			return this.position;
		}
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.TreasurePoint))
		{
			this.positionSet = true;
			base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.Location, base.OwnerQuest.Position);
			base.CurrentValue = 2;
			return this.position;
		}
		float distance = 50f;
		if (this.Value != null && this.Value != "")
		{
			if (!StringParsers.TryParseFloat(this.Value, out distance, 0, -1, NumberStyles.Any))
			{
				if (this.Value.Contains("-"))
				{
					string[] array = this.Value.Split('-', StringSplitOptions.None);
					float num = StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any);
					float num2 = StringParsers.ParseFloat(array[1], 0, -1, NumberStyles.Any);
					distance = GameManager.Instance.World.GetGameRandom().RandomFloat * (num2 - num) + num;
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Vector3i vector3i = ObjectiveRandomGoto.CalculateRandomPoint(ownerNPC.entityId, distance, base.OwnerQuest.ID, false, this.biomeFilterType, this.biomeFilter);
			if (!GameManager.Instance.World.CheckForLevelNearbyHeights((float)vector3i.x, (float)vector3i.z, 5) || GameManager.Instance.World.GetWaterAt((float)vector3i.x, (float)vector3i.z))
			{
				return Vector3.zero;
			}
			World world = GameManager.Instance.World;
			if (vector3i.y > 0 && world.IsPositionInBounds(vector3i) && !world.IsPositionWithinPOI(vector3i, 5))
			{
				base.FinalizePoint(vector3i.x, vector3i.y, vector3i.z);
				return this.position;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestTreasurePoint>().Setup(ownerNPC.entityId, distance, 1, base.OwnerQuest.QuestCode, 0, -1, 0, false), false);
			base.CurrentValue = 1;
		}
		return Vector3.zero;
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x001ADAB0 File Offset: 0x001ABCB0
	public override BaseObjective Clone()
	{
		ObjectiveRandomGotoNPC objectiveRandomGotoNPC = new ObjectiveRandomGotoNPC();
		this.CopyValues(objectiveRandomGotoNPC);
		objectiveRandomGotoNPC.position = this.position;
		objectiveRandomGotoNPC.positionSet = this.positionSet;
		objectiveRandomGotoNPC.completionDistance = this.completionDistance;
		objectiveRandomGotoNPC.biomeFilter = this.biomeFilter;
		objectiveRandomGotoNPC.biomeFilterType = this.biomeFilterType;
		return objectiveRandomGotoNPC;
	}
}
