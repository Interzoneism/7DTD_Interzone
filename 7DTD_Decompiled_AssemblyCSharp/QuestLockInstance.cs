using System;
using System.Collections.Generic;

// Token: 0x0200090E RID: 2318
public class QuestLockInstance
{
	// Token: 0x060044E2 RID: 17634 RVA: 0x001B9499 File Offset: 0x001B7699
	public QuestLockInstance(int lockedByEntityID)
	{
		this.AddQuester(lockedByEntityID);
		this.LockedOutUntil = 0UL;
		this.IsLocked = true;
	}

	// Token: 0x060044E3 RID: 17635 RVA: 0x001B94C9 File Offset: 0x001B76C9
	public void AddQuester(int entityID)
	{
		if (!this.LockedByEntities.Contains(entityID))
		{
			this.LockedByEntities.Add(entityID);
		}
	}

	// Token: 0x060044E4 RID: 17636 RVA: 0x001B94E8 File Offset: 0x001B76E8
	public void AddQuesters(int[] entityIDs)
	{
		for (int i = 0; i < entityIDs.Length; i++)
		{
			if (!this.LockedByEntities.Contains(entityIDs[i]))
			{
				this.LockedByEntities.Add(entityIDs[i]);
			}
		}
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x001B9521 File Offset: 0x001B7721
	public void RemoveQuester(int entityID)
	{
		if (this.LockedByEntities.Contains(entityID))
		{
			this.LockedByEntities.Remove(entityID);
		}
		if (this.LockedByEntities.Count == 0)
		{
			this.SetUnlocked();
		}
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x001B9551 File Offset: 0x001B7751
	public void SetUnlocked()
	{
		if (this.IsLocked)
		{
			this.IsLocked = false;
			if (!GameUtils.IsPlaytesting())
			{
				this.LockedOutUntil = GameManager.Instance.World.GetWorldTime() + 2000UL;
			}
		}
	}

	// Token: 0x060044E7 RID: 17639 RVA: 0x001B9585 File Offset: 0x001B7785
	public bool CheckQuestLock()
	{
		return !this.IsLocked && GameManager.Instance.World.GetWorldTime() > this.LockedOutUntil;
	}

	// Token: 0x0400361A RID: 13850
	public bool IsLocked = true;

	// Token: 0x0400361B RID: 13851
	public List<int> LockedByEntities = new List<int>();

	// Token: 0x0400361C RID: 13852
	public ulong LockedOutUntil;
}
