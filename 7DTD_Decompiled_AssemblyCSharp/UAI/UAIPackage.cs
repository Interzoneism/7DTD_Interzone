using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014B4 RID: 5300
	[Preserve]
	public class UAIPackage
	{
		// Token: 0x170011D4 RID: 4564
		// (get) Token: 0x0600A377 RID: 41847 RVA: 0x00410EEB File Offset: 0x0040F0EB
		// (set) Token: 0x0600A378 RID: 41848 RVA: 0x00410EF3 File Offset: 0x0040F0F3
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170011D5 RID: 4565
		// (get) Token: 0x0600A379 RID: 41849 RVA: 0x00410EFC File Offset: 0x0040F0FC
		// (set) Token: 0x0600A37A RID: 41850 RVA: 0x00410F04 File Offset: 0x0040F104
		public float Weight { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600A37B RID: 41851 RVA: 0x00410F0D File Offset: 0x0040F10D
		public UAIPackage(string _name = "", float _weight = 1f)
		{
			this.Name = _name;
			this.Weight = _weight;
			this.actionList = new List<UAIAction>();
		}

		// Token: 0x0600A37C RID: 41852 RVA: 0x00410F30 File Offset: 0x0040F130
		public float DecideAction(Context _context, out UAIAction _chosenAction, out object _chosenTarget)
		{
			float num = 0f;
			_chosenAction = null;
			_chosenTarget = null;
			for (int i = 0; i < this.actionList.Count; i++)
			{
				int num2 = 0;
				int num3 = 0;
				while (num3 < _context.ConsiderationData.EntityTargets.Count && num2 <= UAIBase.MaxEntitiesToConsider)
				{
					float score = this.actionList[i].GetScore(_context, _context.ConsiderationData.EntityTargets[num3], 0f);
					if (score > num)
					{
						num = score;
						_chosenAction = this.actionList[i];
						_chosenTarget = _context.ConsiderationData.EntityTargets[num3];
					}
					num2++;
					num3++;
				}
				int num4 = 0;
				while (num4 < _context.ConsiderationData.WaypointTargets.Count && num4 <= UAIBase.MaxWaypointsToConsider)
				{
					float score2 = this.actionList[i].GetScore(_context, _context.ConsiderationData.WaypointTargets[num4], 0f);
					if (score2 > num)
					{
						num = score2;
						_chosenAction = this.actionList[i];
						_chosenTarget = _context.ConsiderationData.WaypointTargets[num4];
					}
					num4++;
				}
			}
			return num;
		}

		// Token: 0x0600A37D RID: 41853 RVA: 0x0041106C File Offset: 0x0040F26C
		public List<UAIAction> GetActions()
		{
			return this.actionList;
		}

		// Token: 0x0600A37E RID: 41854 RVA: 0x00411074 File Offset: 0x0040F274
		public void AddAction(UAIAction _action)
		{
			this.actionList.Add(_action);
		}

		// Token: 0x04007E9A RID: 32410
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAIAction> actionList;
	}
}
