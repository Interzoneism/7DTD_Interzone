using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008CB RID: 2251
[Preserve]
public class ObjectivePOIStayWithin : BaseObjective
{
	// Token: 0x170006DC RID: 1756
	// (get) Token: 0x06004221 RID: 16929 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x170006DD RID: 1757
	// (get) Token: 0x06004222 RID: 16930 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x170006DE RID: 1758
	// (get) Token: 0x06004223 RID: 16931 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x170006DF RID: 1759
	// (get) Token: 0x06004224 RID: 16932 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ShowInQuestLog
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x001AB046 File Offset: 0x001A9246
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveStayWithin_keyword", false);
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x001AB059 File Offset: 0x001A9259
	public override void SetupDisplay()
	{
		base.Description = string.Format("{0}", this.keyword);
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x001AB071 File Offset: 0x001A9271
	public override void AddHooks()
	{
		QuestEventManager.Current.AddObjectiveToBeUpdated(this);
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x001AB080 File Offset: 0x001A9280
	public override void RemoveHooks()
	{
		QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		if (base.OwnerQuest == base.OwnerQuest.OwnerJournal.ActiveQuest)
		{
			QuestEventManager.Current.QuestBounds = default(Rect);
		}
		if (this.goBounds != null)
		{
			UnityEngine.Object.Destroy(this.goBounds);
		}
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x001AB0D9 File Offset: 0x001A92D9
	public override void Refresh()
	{
		this.SetupDisplay();
		if (base.ObjectiveState == BaseObjective.ObjectiveStates.NotStarted && base.OwnerQuest.CurrentState == Quest.QuestState.InProgress)
		{
			return;
		}
		base.Complete = (base.OwnerQuest.CurrentState != Quest.QuestState.Failed);
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Read(BinaryReader _br)
	{
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Write(BinaryWriter _bw)
	{
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x001AB110 File Offset: 0x001A9310
	public override BaseObjective Clone()
	{
		ObjectivePOIStayWithin objectivePOIStayWithin = new ObjectivePOIStayWithin();
		this.CopyValues(objectivePOIStayWithin);
		return objectivePOIStayWithin;
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x001AB12B File Offset: 0x001A932B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectivePOIStayWithin objectivePOIStayWithin = (ObjectivePOIStayWithin)objective;
		objectivePOIStayWithin.outerRect = this.outerRect;
		objectivePOIStayWithin.innerRect = this.innerRect;
		objectivePOIStayWithin.offset = this.offset;
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x001AB15D File Offset: 0x001A935D
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameObject CreateBoundsViewer()
	{
		if (this.prefabBounds == null)
		{
			this.prefabBounds = Resources.Load<GameObject>("Prefabs/prefabPOIBounds");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefabBounds);
		gameObject.name = "QuestBounds";
		return gameObject;
	}

	// Token: 0x0600422F RID: 16943 RVA: 0x001AB194 File Offset: 0x001A9394
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetPosition()
	{
		Vector3 vector;
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.POIPosition) && base.OwnerQuest.GetPositionData(out vector, Quest.PositionDataTypes.POISize))
		{
			PrefabInstance prefabAtPosition = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabAtPosition(this.position, true);
			if (prefabAtPosition != null)
			{
				base.OwnerQuest.Position = this.position;
				this.positionSet = true;
				this.outerRect = new Rect(this.position.x - this.offset, this.position.z - this.offset, vector.x + this.offset * 2f, vector.z + this.offset * 2f);
				this.innerRect = new Rect(this.position.x, this.position.z, vector.x, vector.z);
				float rotationAngle = prefabAtPosition.RotationAngle;
				this.outerRect = GeometryUtils.RotateRectAboutY(this.outerRect, rotationAngle);
				this.innerRect = GeometryUtils.RotateRectAboutY(this.innerRect, rotationAngle);
				QuestEventManager.Current.QuestBounds = this.outerRect;
				if (this.goBounds == null)
				{
					this.goBounds = this.CreateBoundsViewer();
				}
				this.goBounds.GetComponent<POIBoundsHelper>().SetPosition(new Vector3(this.outerRect.center.x, base.OwnerQuest.OwnerJournal.OwnerPlayer.position.y, this.outerRect.center.y) - Origin.position, new Vector3(this.outerRect.width, 200f, this.outerRect.height));
				base.CurrentValue = 2;
				return this.position;
			}
		}
		return Vector3.zero;
	}

	// Token: 0x06004230 RID: 16944 RVA: 0x001AB363 File Offset: 0x001A9563
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_NeedSetup()
	{
		this.GetPosition() != Vector3.zero;
	}

	// Token: 0x06004231 RID: 16945 RVA: 0x001AB378 File Offset: 0x001A9578
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_Update()
	{
		if (!this.positionSet)
		{
			this.GetPosition();
			return;
		}
		Vector3 vector = base.OwnerQuest.OwnerJournal.OwnerPlayer.position;
		Vector3 vector2 = base.OwnerQuest.Position;
		vector.y = vector.z;
		if (!this.outerRect.Contains(vector))
		{
			base.Complete = false;
			base.ObjectiveState = BaseObjective.ObjectiveStates.Failed;
			base.OwnerQuest.CloseQuest(Quest.QuestState.Failed, null);
			return;
		}
		if (this.innerRect.Contains(vector))
		{
			base.ObjectiveState = BaseObjective.ObjectiveStates.Complete;
			return;
		}
		base.ObjectiveState = BaseObjective.ObjectiveStates.Warning;
	}

	// Token: 0x06004232 RID: 16946 RVA: 0x001AB40B File Offset: 0x001A960B
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectivePOIStayWithin.PropRadius))
		{
			this.offset = (float)StringParsers.ParseSInt32(properties.Values[ObjectivePOIStayWithin.PropRadius], 0, -1, NumberStyles.Integer);
		}
	}

	// Token: 0x04003479 RID: 13433
	public static string PropRadius = "radius";

	// Token: 0x0400347A RID: 13434
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x0400347B RID: 13435
	[PublicizedFrom(EAccessModifier.Private)]
	public bool positionSet;

	// Token: 0x0400347C RID: 13436
	[PublicizedFrom(EAccessModifier.Private)]
	public Rect outerRect;

	// Token: 0x0400347D RID: 13437
	[PublicizedFrom(EAccessModifier.Private)]
	public Rect innerRect;

	// Token: 0x0400347E RID: 13438
	[PublicizedFrom(EAccessModifier.Private)]
	public float offset;

	// Token: 0x0400347F RID: 13439
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject prefabBounds;

	// Token: 0x04003480 RID: 13440
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject goBounds;
}
