using System;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A9 RID: 5289
	[Preserve]
	public class UAITaskFleeFromTarget : UAITaskBase
	{
		// Token: 0x0600A34D RID: 41805 RVA: 0x0041048C File Offset: 0x0040E68C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void initializeParameters()
		{
			base.initializeParameters();
			if (this.Parameters.ContainsKey("max_distance"))
			{
				this.maxFleeDistance = StringParsers.ParseFloat(this.Parameters["max_distance"], 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600A34E RID: 41806 RVA: 0x004104C8 File Offset: 0x0040E6C8
		public override void Start(Context _context)
		{
			base.Start(_context);
			EntityAlive entityAlive = UAIUtils.ConvertToEntityAlive(_context.ActionData.Target);
			if (entityAlive != null)
			{
				_context.Self.detachHome();
				_context.Self.FindPath(RandomPositionGenerator.CalcAway(_context.Self, 0, (int)this.maxFleeDistance, (int)this.maxFleeDistance, entityAlive.position), _context.Self.GetMoveSpeedPanic(), false, null);
				return;
			}
			_context.ActionData.Failed = true;
		}

		// Token: 0x0600A34F RID: 41807 RVA: 0x00410546 File Offset: 0x0040E746
		public override void Update(Context _context)
		{
			base.Update(_context);
			if (_context.Self.getNavigator().noPathAndNotPlanningOne())
			{
				_context.Self.setHomeArea(new Vector3i(_context.Self.position), 10);
				this.Stop(_context);
			}
		}

		// Token: 0x04007E78 RID: 32376
		[PublicizedFrom(EAccessModifier.Private)]
		public float maxFleeDistance;
	}
}
