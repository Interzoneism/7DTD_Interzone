using System;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014AB RID: 5291
	[Preserve]
	public class UAITaskWander : UAITaskBase
	{
		// Token: 0x0600A355 RID: 41813 RVA: 0x00410770 File Offset: 0x0040E970
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void initializeParameters()
		{
			base.initializeParameters();
			if (this.Parameters.ContainsKey("max_distance"))
			{
				this.maxWanderDistance = StringParsers.ParseFloat(this.Parameters["max_distance"], 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600A356 RID: 41814 RVA: 0x004107AC File Offset: 0x0040E9AC
		public override void Start(Context _context)
		{
			base.Start(_context);
			int num = 10;
			_context.Self.FindPath(RandomPositionGenerator.CalcAround(_context.Self, num, num), _context.Self.GetMoveSpeed(), false, null);
		}

		// Token: 0x0600A357 RID: 41815 RVA: 0x0041074E File Offset: 0x0040E94E
		public override void Update(Context _context)
		{
			base.Update(_context);
			if (_context.Self.getNavigator().noPathAndNotPlanningOne())
			{
				this.Stop(_context);
			}
		}

		// Token: 0x04007E7C RID: 32380
		public float maxWanderDistance;
	}
}
