using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014AC RID: 5292
	[Preserve]
	public class UAIAction
	{
		// Token: 0x170011D0 RID: 4560
		// (get) Token: 0x0600A359 RID: 41817 RVA: 0x004107E8 File Offset: 0x0040E9E8
		// (set) Token: 0x0600A35A RID: 41818 RVA: 0x004107F0 File Offset: 0x0040E9F0
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170011D1 RID: 4561
		// (get) Token: 0x0600A35B RID: 41819 RVA: 0x004107F9 File Offset: 0x0040E9F9
		// (set) Token: 0x0600A35C RID: 41820 RVA: 0x00410801 File Offset: 0x0040EA01
		public float Weight { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600A35D RID: 41821 RVA: 0x0041080A File Offset: 0x0040EA0A
		public UAIAction(string _name, float _weight)
		{
			this.Name = _name;
			this.Weight = _weight;
			this.considerations = new List<UAIConsiderationBase>();
			this.tasks = new List<UAITaskBase>();
		}

		// Token: 0x0600A35E RID: 41822 RVA: 0x00410838 File Offset: 0x0040EA38
		public float GetScore(Context _context, object _target, float min = 0f)
		{
			float num = 1f;
			if (this.considerations.Count == 0)
			{
				return num * this.Weight;
			}
			if (this.tasks.Count == 0)
			{
				return 0f;
			}
			for (int i = 0; i < this.considerations.Count; i++)
			{
				if (0f > num || num < min)
				{
					return 0f;
				}
				num *= this.considerations[i].ComputeResponseCurve(this.considerations[i].GetScore(_context, _target));
			}
			return (num + (1f - num) * (float)(1 - 1 / this.considerations.Count) * num) * this.Weight;
		}

		// Token: 0x0600A35F RID: 41823 RVA: 0x004108E6 File Offset: 0x0040EAE6
		public void AddConsideration(UAIConsiderationBase _c)
		{
			this.considerations.Add(_c);
		}

		// Token: 0x0600A360 RID: 41824 RVA: 0x004108F4 File Offset: 0x0040EAF4
		public void AddTask(UAITaskBase _t)
		{
			this.tasks.Add(_t);
		}

		// Token: 0x0600A361 RID: 41825 RVA: 0x00410902 File Offset: 0x0040EB02
		public List<UAIConsiderationBase> GetConsiderations()
		{
			return this.considerations;
		}

		// Token: 0x0600A362 RID: 41826 RVA: 0x0041090A File Offset: 0x0040EB0A
		public List<UAITaskBase> GetTasks()
		{
			return this.tasks;
		}

		// Token: 0x04007E7F RID: 32383
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAITaskBase> tasks;

		// Token: 0x04007E80 RID: 32384
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAIConsiderationBase> considerations;
	}
}
