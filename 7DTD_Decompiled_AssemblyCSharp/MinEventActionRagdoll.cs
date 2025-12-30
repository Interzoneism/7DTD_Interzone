using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000634 RID: 1588
[Preserve]
public class MinEventActionRagdoll : MinEventActionTargetedBase
{
	// Token: 0x060030B8 RID: 12472 RVA: 0x0014C8A8 File Offset: 0x0014AAA8
	public override void Execute(MinEventParams _params)
	{
		DamageResponse dmResponse = DamageResponse.New(false);
		dmResponse.StunDuration = this.duration;
		dmResponse.Strength = (int)this.force;
		if (this.cvarRef && this.targets.Count > 0)
		{
			dmResponse.StunDuration = this.targets[0].Buffs.GetCustomVar(this.refCvarName);
		}
		Vector3 vector = _params.StartPosition;
		if (vector.y == 0f)
		{
			vector = _params.Self.position;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityAlive entityAlive = this.targets[i];
			if (entityAlive.AttachedToEntity != null)
			{
				entityAlive.Detach();
			}
			Vector3 vector2 = entityAlive.position - vector;
			if (this.scaleY == 0f)
			{
				vector2.y = 0f;
				dmResponse.Source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing, vector2.normalized);
			}
			else
			{
				vector2.y = _params.Self.GetLookVector().y * this.scaleY;
				float num = this.force;
				if (this.massScale > 0f)
				{
					num *= EntityClass.list[entityAlive.entityClass].MassKg * this.massScale;
				}
				dmResponse.Source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Falling, vector2.normalized * num);
			}
			entityAlive.DoRagdoll(dmResponse);
		}
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x0014CA24 File Offset: 0x0014AC24
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "duration")
			{
				if (_attribute.Value.StartsWith("@"))
				{
					this.cvarRef = true;
					this.refCvarName = _attribute.Value.Substring(1);
				}
				else
				{
					this.duration = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				}
				return true;
			}
			if (localName == "force")
			{
				this.force = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "massScale")
			{
				this.massScale = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "scaleY")
			{
				this.scaleY = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0400272F RID: 10031
	[PublicizedFrom(EAccessModifier.Private)]
	public float duration = 2.5f;

	// Token: 0x04002730 RID: 10032
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x04002731 RID: 10033
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName = string.Empty;

	// Token: 0x04002732 RID: 10034
	[PublicizedFrom(EAccessModifier.Private)]
	public float force;

	// Token: 0x04002733 RID: 10035
	[PublicizedFrom(EAccessModifier.Private)]
	public float scaleY;

	// Token: 0x04002734 RID: 10036
	[PublicizedFrom(EAccessModifier.Private)]
	public float massScale;
}
