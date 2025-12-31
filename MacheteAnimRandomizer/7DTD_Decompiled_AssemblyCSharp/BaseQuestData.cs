using System;
using System.Collections.Generic;

// Token: 0x020008A3 RID: 2211
public class BaseQuestData
{
	// Token: 0x0600406B RID: 16491 RVA: 0x001A4968 File Offset: 0x001A2B68
	public void AddSharedQuester(int _entityID)
	{
		if (!this.entityList.Contains(_entityID))
		{
			this.entityList.Add(_entityID);
			EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(_entityID) as EntityPlayer;
			if (entityPlayer != null)
			{
				this.OnAdd(entityPlayer);
			}
		}
	}

	// Token: 0x0600406C RID: 16492 RVA: 0x001A49B0 File Offset: 0x001A2BB0
	public void RemoveSharedQuester(EntityPlayer _player)
	{
		if (this.entityList.Contains(_player.entityId))
		{
			this.entityList.Remove(_player.entityId);
		}
		if (this.entityList.Count == 0)
		{
			this.OnRemove(_player);
			this.RemoveFromDictionary();
		}
	}

	// Token: 0x0600406D RID: 16493 RVA: 0x001A49FC File Offset: 0x001A2BFC
	public bool ContainsEntity(int _entityID)
	{
		return this.entityList.Contains(_entityID);
	}

	// Token: 0x0600406E RID: 16494 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetModifier(string _name)
	{
	}

	// Token: 0x0600406F RID: 16495 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RemoveFromDictionary()
	{
	}

	// Token: 0x06004070 RID: 16496 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCreated()
	{
	}

	// Token: 0x06004071 RID: 16497 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnAdd(EntityPlayer player)
	{
	}

	// Token: 0x06004072 RID: 16498 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnRemove(EntityPlayer player)
	{
	}

	// Token: 0x06004073 RID: 16499 RVA: 0x001A4A0A File Offset: 0x001A2C0A
	public void Remove()
	{
		this.entityList.Clear();
		this.OnRemove(null);
	}

	// Token: 0x0400339F RID: 13215
	[PublicizedFrom(EAccessModifier.Protected)]
	public int questCode;

	// Token: 0x040033A0 RID: 13216
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<int> entityList = new List<int>();
}
