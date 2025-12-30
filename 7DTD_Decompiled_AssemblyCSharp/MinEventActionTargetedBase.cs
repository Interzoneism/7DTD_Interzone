using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000663 RID: 1635
public class MinEventActionTargetedBase : MinEventActionBase
{
	// Token: 0x0600316A RID: 12650 RVA: 0x001509E8 File Offset: 0x0014EBE8
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		this.targets.Clear();
		if (this.targetType == MinEventActionTargetedBase.TargetTypes.self)
		{
			if (_params.Self != null && this.isValidTarget(_params.Self, null))
			{
				this.targets.Add(_params.Self);
				return this.singleTargetCheck(_params);
			}
		}
		else if (this.targetType == MinEventActionTargetedBase.TargetTypes.other)
		{
			if (_params.Others != null)
			{
				for (int i = 0; i < _params.Others.Length; i++)
				{
					if (this.isValidTarget(_params.Self, _params.Others[i]))
					{
						MinEventParams minEventParams = new MinEventParams();
						MinEventParams.CopyTo(_params, minEventParams);
						minEventParams.Other = _params.Others[i];
						if (this.singleTargetCheck(minEventParams))
						{
							this.targets.Add(_params.Others[i]);
						}
					}
				}
			}
			else if (_params.Other != null && this.isValidTarget(_params.Self, _params.Other) && this.singleTargetCheck(_params))
			{
				this.targets.Add(_params.Other);
			}
		}
		else if (this.targetType == MinEventActionTargetedBase.TargetTypes.selfAOE)
		{
			if (_params.Self != null)
			{
				this.entsInRange = GameManager.Instance.World.GetLivingEntitiesInBounds(_params.Self, new Bounds(_params.Self.position, Vector3.one * (this.maxRange * 2f)));
				for (int j = 0; j < this.entsInRange.Count; j++)
				{
					if (this.entsInRange[j] != null && this.isValidTarget(_params.Self, this.entsInRange[j]))
					{
						MinEventParams minEventParams2 = new MinEventParams();
						MinEventParams.CopyTo(_params, minEventParams2);
						minEventParams2.Other = this.entsInRange[j];
						if (this.singleTargetCheck(minEventParams2))
						{
							this.targets.Add(this.entsInRange[j]);
						}
					}
				}
				this.entsInRange.Clear();
			}
		}
		else if (this.targetType == MinEventActionTargetedBase.TargetTypes.selfOtherPlayers)
		{
			if (_params.Self != null)
			{
				List<EntityPlayer> players = GameManager.Instance.World.GetPlayers();
				for (int k = 0; k < players.Count; k++)
				{
					EntityPlayer entityPlayer = players[k];
					if (!(entityPlayer == _params.Self) && this.isValidTarget(_params.Self, entityPlayer))
					{
						MinEventParams minEventParams3 = new MinEventParams();
						MinEventParams.CopyTo(_params, minEventParams3);
						minEventParams3.Other = entityPlayer;
						if (this.singleTargetCheck(minEventParams3))
						{
							this.targets.Add(entityPlayer);
						}
					}
				}
			}
		}
		else if (this.targetType == MinEventActionTargetedBase.TargetTypes.otherAOE)
		{
			if (_params.Other != null)
			{
				this.entsInRange = GameManager.Instance.World.GetLivingEntitiesInBounds(_params.Self, new Bounds(_params.Other.position, Vector3.one * (this.maxRange * 2f)));
				for (int l = 0; l < this.entsInRange.Count; l++)
				{
					if (this.entsInRange[l] != null && this.isValidTarget(_params.Self, this.entsInRange[l]))
					{
						MinEventParams minEventParams4 = new MinEventParams();
						MinEventParams.CopyTo(_params, minEventParams4);
						minEventParams4.Other = this.entsInRange[l];
						if (this.singleTargetCheck(minEventParams4))
						{
							this.targets.Add(this.entsInRange[l]);
						}
					}
				}
				this.entsInRange.Clear();
			}
		}
		else if (this.targetType == MinEventActionTargetedBase.TargetTypes.positionAOE)
		{
			this.entsInRange = GameManager.Instance.World.GetLivingEntitiesInBounds(null, new Bounds(_params.Position, Vector3.one * (this.maxRange * 2f)));
			for (int m = 0; m < this.entsInRange.Count; m++)
			{
				EntityAlive entityAlive = this.entsInRange[m];
				if (entityAlive != null && this.isValidTarget(_params.Self, entityAlive) && (!(entityAlive is EntityPlayer) || (entityAlive.getHipPosition() - _params.Position).sqrMagnitude <= this.maxRange * this.maxRange))
				{
					MinEventParams minEventParams5 = new MinEventParams();
					MinEventParams.CopyTo(_params, minEventParams5);
					minEventParams5.Other = entityAlive;
					if (this.singleTargetCheck(minEventParams5))
					{
						this.targets.Add(entityAlive);
					}
				}
			}
			this.entsInRange.Clear();
		}
		return this.targets.Count > 0;
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x00150EA4 File Offset: 0x0014F0A4
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "target")
			{
				this.targetType = EnumUtils.Parse<MinEventActionTargetedBase.TargetTypes>(_attribute.Value, true);
				return true;
			}
			if (localName == "range")
			{
				this.maxRange = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "target_tags")
			{
				this.targetTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x00150F34 File Offset: 0x0014F134
	[PublicizedFrom(EAccessModifier.Private)]
	public bool singleTargetCheck(MinEventParams _tempParams)
	{
		if (this.Requirements.Count > 0)
		{
			bool flag = true;
			if (!this.OrCompare)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					flag &= this.Requirements[i].IsValid(_tempParams);
					if (!flag)
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.Requirements.Count; j++)
				{
					flag = this.Requirements[j].IsValid(_tempParams);
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return flag;
		}
		return true;
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x00150FC0 File Offset: 0x0014F1C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isValidTarget(EntityAlive _self, EntityAlive _other)
	{
		if (this.targetTags.IsEmpty)
		{
			return true;
		}
		if (_self == null && _other != null && _other.HasAnyTags(this.targetTags))
		{
			return true;
		}
		if (_other == null && _self.HasAnyTags(this.targetTags))
		{
			return true;
		}
		if (_other.HasAnyTags(this.targetTags))
		{
			return true;
		}
		if (this.targetTags.Test_AnySet(MinEventActionTargetedBase.party) && _self as EntityPlayer != null && _other as EntityPlayer != null && (_self as EntityPlayer).Party != null && (_self as EntityPlayer).Party.ContainsMember(_other as EntityPlayer))
		{
			return true;
		}
		if (this.targetTags.Test_AnySet(MinEventActionTargetedBase.ally))
		{
			if (_self as EntityPlayer != null && _other as EntityPlayer != null && (_self as EntityPlayer).IsFriendsWith(_other as EntityPlayer))
			{
				return true;
			}
			if (_self as EntityEnemy != null && _other as EntityEnemy != null)
			{
				return true;
			}
			if (FactionManager.Instance != null)
			{
				if (_self as EntityPlayer != null && _other as EntityNPC != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Like)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Love)
					{
						return true;
					}
				}
				if (_self as EntityNPC != null && _other as EntityPlayer != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Like)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Love)
					{
						return true;
					}
				}
				if (_self as EntityNPC != null && _other as EntityNPC != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Like)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Love)
					{
						return true;
					}
				}
			}
		}
		if (this.targetTags.Test_AnySet(MinEventActionTargetedBase.enemy))
		{
			if (_self as EntityEnemy != null && _other as EntityPlayer != null)
			{
				return true;
			}
			if (_self as EntityPlayer != null && _other as EntityEnemy != null)
			{
				return true;
			}
			if (_self as EntityEnemy != null && _other as EntityNPC != null)
			{
				return true;
			}
			if (_self as EntityNPC != null && _other as EntityEnemy != null)
			{
				return true;
			}
			if (FactionManager.Instance != null)
			{
				if (_self as EntityPlayer != null && _other as EntityPlayer != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Hate)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Dislike)
					{
						return true;
					}
				}
				if (_self as EntityPlayer != null && _other as EntityNPC != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Hate)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Dislike)
					{
						return true;
					}
				}
				if (_self as EntityNPC != null && _other as EntityPlayer != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Hate)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Dislike)
					{
						return true;
					}
				}
				if (_self as EntityNPC != null && _other as EntityNPC != null)
				{
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Hate)
					{
						return true;
					}
					if (FactionManager.Instance.GetRelationshipTier(_self, _other) == FactionManager.Relationship.Dislike)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x040027C8 RID: 10184
	[PublicizedFrom(EAccessModifier.Protected)]
	public static FastTags<TagGroup.Global> ally = FastTags<TagGroup.Global>.Parse("ally");

	// Token: 0x040027C9 RID: 10185
	[PublicizedFrom(EAccessModifier.Protected)]
	public static FastTags<TagGroup.Global> party = FastTags<TagGroup.Global>.Parse("party");

	// Token: 0x040027CA RID: 10186
	[PublicizedFrom(EAccessModifier.Protected)]
	public static FastTags<TagGroup.Global> enemy = FastTags<TagGroup.Global>.Parse("enemy");

	// Token: 0x040027CB RID: 10187
	public MinEventActionTargetedBase.TargetTypes targetType;

	// Token: 0x040027CC RID: 10188
	[PublicizedFrom(EAccessModifier.Protected)]
	public FastTags<TagGroup.Global> targetTags = FastTags<TagGroup.Global>.none;

	// Token: 0x040027CD RID: 10189
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<EntityAlive> targets = new List<EntityAlive>();

	// Token: 0x040027CE RID: 10190
	[PublicizedFrom(EAccessModifier.Protected)]
	public float maxRange;

	// Token: 0x040027CF RID: 10191
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> entsInRange;

	// Token: 0x02000664 RID: 1636
	public enum TargetTypes
	{
		// Token: 0x040027D1 RID: 10193
		self,
		// Token: 0x040027D2 RID: 10194
		other,
		// Token: 0x040027D3 RID: 10195
		selfAOE,
		// Token: 0x040027D4 RID: 10196
		otherAOE,
		// Token: 0x040027D5 RID: 10197
		positionAOE,
		// Token: 0x040027D6 RID: 10198
		selfOtherPlayers
	}
}
