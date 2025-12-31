using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000402 RID: 1026
[Preserve]
public class EAITerritorial : EAIBase
{
	// Token: 0x06001EDF RID: 7903 RVA: 0x000BBBF9 File Offset: 0x000B9DF9
	public EAITerritorial()
	{
		this.MutexBits = 1;
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000BED71 File Offset: 0x000BCF71
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000C04E0 File Offset: 0x000BE6E0
	public override bool CanExecute()
	{
		if (this.theEntity.isWithinHomeDistanceCurrentPosition())
		{
			return false;
		}
		ChunkCoordinates homePosition = this.theEntity.getHomePosition();
		Vector3 vector = RandomPositionGenerator.CalcTowards(this.theEntity, 5, 15, 7, homePosition.position.ToVector3());
		if (vector.Equals(Vector3.zero))
		{
			return false;
		}
		this.movePos = vector;
		return true;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x000C053B File Offset: 0x000BE73B
	public override bool Continue()
	{
		return !this.theEntity.getNavigator().noPathAndNotPlanningOne();
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000C0550 File Offset: 0x000BE750
	public override void Start()
	{
		this.theEntity.FindPath(this.movePos, this.theEntity.GetMoveSpeed(), false, this);
	}

	// Token: 0x04001557 RID: 5463
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 movePos;
}
