using System;
using UnityEngine.Scripting;

// Token: 0x020008A4 RID: 2212
[Preserve]
public class RestorePowerQuestData : BaseQuestData
{
	// Token: 0x170006A8 RID: 1704
	// (get) Token: 0x06004075 RID: 16501 RVA: 0x001A4A31 File Offset: 0x001A2C31
	// (set) Token: 0x06004076 RID: 16502 RVA: 0x001A4A39 File Offset: 0x001A2C39
	public Vector3i PrefabPosition { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06004077 RID: 16503 RVA: 0x001A4A42 File Offset: 0x001A2C42
	public RestorePowerQuestData(int _questCode, int _entityID, Vector3i _position, string _completeEvent)
	{
		this.CompleteEvent = _completeEvent;
		this.questCode = _questCode;
		this.entityList.Add(_entityID);
		this.PrefabPosition = _position;
	}

	// Token: 0x06004078 RID: 16504 RVA: 0x001A4A77 File Offset: 0x001A2C77
	public void UpdatePosition(Vector3i _pos)
	{
		this.PrefabPosition = _pos;
	}

	// Token: 0x06004079 RID: 16505 RVA: 0x001A4A80 File Offset: 0x001A2C80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void RemoveFromDictionary()
	{
		QuestEventManager.Current.BlockActivateQuestDictionary.Remove(this.questCode);
	}

	// Token: 0x0600407A RID: 16506 RVA: 0x001A4A98 File Offset: 0x001A2C98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnRemove(EntityPlayer player)
	{
		if (this.CompleteEvent != "")
		{
			GameEventManager.Current.HandleAction(this.CompleteEvent, null, player, false, this.PrefabPosition, "", "", false, true, "", null);
		}
	}

	// Token: 0x040033A2 RID: 13218
	public string CompleteEvent = "";
}
