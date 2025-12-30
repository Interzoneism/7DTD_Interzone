using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000655 RID: 1621
[Preserve]
public class MinEventActionExplode : MinEventActionTargetedBase
{
	// Token: 0x06003137 RID: 12599 RVA: 0x0014F674 File Offset: 0x0014D874
	public override void Execute(MinEventParams _params)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		new Vector3(0f, 0.5f, 0f);
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityAlive entityAlive = this.targets[i];
			if (entityAlive != null)
			{
				ExplosionData explosionData = new ExplosionData
				{
					BlastPower = this.blastPower,
					BlockDamage = (float)this.blockDamage,
					BlockRadius = (float)this.blockRadius,
					BlockTags = this.blockTags,
					EntityDamage = (float)this.entityDamage,
					EntityRadius = this.entityRadius,
					DamageType = this.damageType,
					ParticleIndex = 13
				};
				GameManager.Instance.ExplosionServer(0, entityAlive.getHeadPosition(), entityAlive.GetBlockPosition(), entityAlive.qrotation, explosionData, entityAlive.entityId, 0.1f, false, null);
			}
		}
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0014F774 File Offset: 0x0014D974
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
			if (num <= 430148698U)
			{
				if (num != 69632989U)
				{
					if (num != 203327429U)
					{
						if (num == 430148698U)
						{
							if (localName == "block_tags")
							{
								this.blockTags = _attribute.Value;
							}
						}
					}
					else if (localName == "damage_type")
					{
						this.damageType = (EnumDamageTypes)Enum.Parse(typeof(EnumDamageTypes), _attribute.Value);
					}
				}
				else if (localName == "entity_radius")
				{
					this.entityRadius = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
				}
			}
			else if (num <= 1332589382U)
			{
				if (num != 796463477U)
				{
					if (num == 1332589382U)
					{
						if (localName == "block_damage")
						{
							this.blockDamage = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
							return true;
						}
					}
				}
				else if (localName == "block_radius")
				{
					this.blockRadius = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
				}
			}
			else if (num != 2865571118U)
			{
				if (num == 3191003235U)
				{
					if (localName == "blast_power")
					{
						this.blastPower = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
						return true;
					}
				}
			}
			else if (localName == "entity_damage")
			{
				this.entityDamage = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			}
		}
		return flag;
	}

	// Token: 0x0400278A RID: 10122
	[PublicizedFrom(EAccessModifier.Protected)]
	public int blastPower = 75;

	// Token: 0x0400278B RID: 10123
	[PublicizedFrom(EAccessModifier.Protected)]
	public int blockDamage = 1;

	// Token: 0x0400278C RID: 10124
	[PublicizedFrom(EAccessModifier.Protected)]
	public int blockRadius = 4;

	// Token: 0x0400278D RID: 10125
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityDamage = 5000;

	// Token: 0x0400278E RID: 10126
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityRadius = 3;

	// Token: 0x0400278F RID: 10127
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockTags = "";

	// Token: 0x04002790 RID: 10128
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumDamageTypes damageType = EnumDamageTypes.Heat;
}
