using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A6 RID: 5286
	[Preserve]
	public class UAITaskAttackTargetBlock : UAITaskBase
	{
		// Token: 0x0600A33C RID: 41788 RVA: 0x004100B2 File Offset: 0x0040E2B2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void initializeParameters()
		{
			base.initializeParameters();
		}

		// Token: 0x0600A33D RID: 41789 RVA: 0x004100BC File Offset: 0x0040E2BC
		public override void Start(Context _context)
		{
			base.Start(_context);
			if (_context.ActionData.Target.GetType() == typeof(Vector3))
			{
				this.attackTimeout = _context.Self.GetAttackTimeoutTicks();
				Vector3 vector = (Vector3)_context.ActionData.Target;
				_context.Self.SetLookPosition(_context.Self.CanSee(vector) ? vector : Vector3.zero);
				if (_context.Self.bodyDamage.HasLimbs)
				{
					_context.Self.RotateTo(vector.x, vector.y, vector.z, 30f, 30f);
					return;
				}
			}
			else
			{
				this.Stop(_context);
			}
		}

		// Token: 0x0600A33E RID: 41790 RVA: 0x00410178 File Offset: 0x0040E378
		public override void Update(Context _context)
		{
			base.Update(_context);
			if (_context.ActionData.Target.GetType() == typeof(Vector3))
			{
				Vector3 vector = (Vector3)_context.ActionData.Target;
				this.attackTimeout = Utils.FastMax(this.attackTimeout - 1, 0);
				if (this.attackTimeout > 0)
				{
					return;
				}
				_context.Self.SetLookPosition(vector);
				if (_context.Self.bodyDamage.HasLimbs)
				{
					_context.Self.RotateTo(vector.x, vector.y, vector.z, 30f, 30f);
				}
				if (_context.Self.Attack(false))
				{
					this.attackTimeout = _context.Self.GetAttackTimeoutTicks();
					_context.Self.Attack(true);
					this.Stop(_context);
					return;
				}
			}
			else
			{
				this.Stop(_context);
			}
		}

		// Token: 0x04007E73 RID: 32371
		public int attackTimeout;
	}
}
