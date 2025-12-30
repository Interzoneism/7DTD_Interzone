using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class ElectricWireController : MonoBehaviour
{
	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001ABA RID: 6842 RVA: 0x000A637B File Offset: 0x000A457B
	// (set) Token: 0x06001ABB RID: 6843 RVA: 0x000A6384 File Offset: 0x000A4584
	public float HealthRatio
	{
		get
		{
			return this.healthRatio;
		}
		set
		{
			this.lastHealthRatio = this.healthRatio;
			this.healthRatio = value;
			if (this.lastHealthRatio != -1f)
			{
				if (this.lastHealthRatio > this.brokenPercentage && this.healthRatio <= this.brokenPercentage)
				{
					this.setWireDip(true);
					return;
				}
				if (this.lastHealthRatio <= this.brokenPercentage && this.healthRatio > this.brokenPercentage)
				{
					this.setWireDip(false);
				}
			}
		}
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000A63F8 File Offset: 0x000A45F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void setWireDip(bool dip)
	{
		float wireDip = this.WireNode.GetWireDip();
		if (dip)
		{
			if (Mathf.Approximately(wireDip, 0f))
			{
				this.WireNode.SetWireDip(0.25f);
				this.WireNode.BuildMesh();
				return;
			}
		}
		else if (!Mathf.Approximately(wireDip, 0f))
		{
			this.WireNode.SetWireDip(0f);
			this.WireNode.BuildMesh();
		}
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x000A6468 File Offset: 0x000A4668
	public void Init(DynamicProperties _properties)
	{
		if (_properties.Values.ContainsKey("Buff"))
		{
			if (this.buffActions == null)
			{
				this.buffActions = new List<string>();
			}
			string[] array = _properties.Values["Buff"].Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.buffActions.Add(array[i]);
			}
		}
		if (_properties.Values.ContainsKey("BreakingPercentage"))
		{
			this.breakingPercentage = Mathf.Clamp01(StringParsers.ParseFloat(_properties.Values["BreakingPercentage"], 0, -1, NumberStyles.Any));
		}
		else
		{
			this.breakingPercentage = 0.5f;
		}
		if (_properties.Values.ContainsKey("BrokenPercentage"))
		{
			this.brokenPercentage = Mathf.Clamp01(StringParsers.ParseFloat(_properties.Values["BrokenPercentage"], 0, -1, NumberStyles.Any));
		}
		else
		{
			this.brokenPercentage = 0.25f;
		}
		if (_properties.Values.ContainsKey("DamageReceived"))
		{
			StringParsers.TryParseFloat(_properties.Values["DamageReceived"], out this.damageReceived, 0, -1, NumberStyles.Any);
		}
		else
		{
			this.damageReceived = 0.1f;
		}
		this.healthRatio = -1f;
		this.BlockPosition = this.TileEntityChild.ToWorldPos();
		this.startPoint = this.WireNode.GetStartPosition() + this.WireNode.GetStartPositionOffset();
		this.endPoint = this.WireNode.GetEndPosition() + this.WireNode.GetEndPositionOffset();
		BlockValue block = GameManager.Instance.World.GetBlock(this.BlockPosition);
		float num = 1f - (float)block.damage / (float)block.Block.MaxDamage;
		if (num <= this.brokenPercentage)
		{
			this.setWireDip(true);
			return;
		}
		if (num > this.brokenPercentage)
		{
			this.setWireDip(false);
		}
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x000A6650 File Offset: 0x000A4850
	public void DamageSelf(float damage)
	{
		this.totalDamage += damage;
		if (this.totalDamage < 1f)
		{
			return;
		}
		damage = (float)((int)this.totalDamage);
		this.totalDamage = 0f;
		if (this.chunk == null)
		{
			this.chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(this.BlockPosition);
		}
		BlockValue block = GameManager.Instance.World.GetBlock(this.BlockPosition);
		this.HealthRatio = 1f - (float)block.damage / (float)block.Block.MaxDamage;
		float num = this.HealthRatio;
		float num2 = ((float)block.damage + damage) / (float)block.Block.MaxDamage;
		block.damage = Mathf.Clamp(block.damage + (int)damage, 0, block.Block.MaxDamage);
		GameManager.Instance.World.SetBlockRPC(this.chunk.ClrIdx, this.BlockPosition, block);
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x000A6750 File Offset: 0x000A4950
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		BlockValue block = GameManager.Instance.World.GetBlock(this.BlockPosition);
		this.HealthRatio = 1f - (float)block.damage / (float)block.Block.MaxDamage;
		bool flag = this.HealthRatio < this.brokenPercentage;
		this.HandleParticlesForBroken(flag);
		this.setWireDip(flag);
		if (this.TileEntityParent == null || !this.TileEntityParent.IsPowered)
		{
			if (this.CollidersThisFrame != null && this.CollidersThisFrame.Count > 0)
			{
				this.CollidersThisFrame.Clear();
			}
			return;
		}
		if (this.CollidersThisFrame == null || this.CollidersThisFrame.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.CollidersThisFrame.Count; i++)
		{
			this.touched(this.CollidersThisFrame[i]);
		}
		this.CollidersThisFrame.Clear();
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x000A6834 File Offset: 0x000A4A34
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		if (this.TileEntityParent == null || !this.TileEntityParent.IsPowered)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x000A6884 File Offset: 0x000A4A84
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerStay(Collider other)
	{
		if (this.TileEntityParent == null || !this.TileEntityParent.IsPowered)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000A68D4 File Offset: 0x000A4AD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerExit(Collider other)
	{
		if (this.TileEntityParent == null || !this.TileEntityParent.IsPowered)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x000A6924 File Offset: 0x000A4B24
	[PublicizedFrom(EAccessModifier.Private)]
	public void touched(Collider collider)
	{
		if (this.TileEntityParent == null || this.TileEntityChild == null || this.WireNode == null || collider == null)
		{
			return;
		}
		if (this.TileEntityParent.IsPowered && this.TileEntityChild.IsPowered && collider.transform != null)
		{
			EntityAlive entityAlive = collider.transform.GetComponent<EntityAlive>();
			if (entityAlive == null)
			{
				entityAlive = collider.transform.GetComponentInParent<EntityAlive>();
			}
			if (entityAlive == null && collider.transform.parent != null)
			{
				entityAlive = collider.transform.parent.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive == null)
			{
				entityAlive = collider.transform.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive != null && entityAlive.IsAlive())
			{
				bool flag = false;
				if (this.HealthRatio < this.brokenPercentage)
				{
					this.HandleParticlesForBroken(true);
					return;
				}
				if (!entityAlive.Electrocuted && entityAlive.Buffs.GetCustomVar("ShockImmunity") == 0f && this.buffActions != null)
				{
					for (int i = 0; i < this.buffActions.Count; i++)
					{
						if (entityAlive.emodel != null && entityAlive.emodel.transform != null)
						{
							Transform transform = entityAlive.emodel.transform;
							if (entityAlive.emodel.GetHitTransform(BodyPrimaryHit.Torso) != null)
							{
								entityAlive.Buffs.SetCustomVar("ETrapHit", 1f, true, CVarOperation.set);
								entityAlive.Buffs.AddBuff(this.buffActions[i], this.TileEntityParent.OwnerEntityID, true, true, -1f);
								entityAlive.Electrocuted = true;
								flag = true;
							}
						}
					}
				}
				if (flag)
				{
					this.DamageSelf(this.damageReceived);
				}
			}
		}
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x000A6AFC File Offset: 0x000A4CFC
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetGameObjectPath(Entity e, Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null && transform.parent.name != e.transform.name)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x000A6B58 File Offset: 0x000A4D58
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleParticlesForBroken(bool isBroken)
	{
		if (isBroken && this.TileEntityParent.IsPowered)
		{
			if (this.particleDelay > 0f)
			{
				this.particleDelay -= Time.deltaTime;
			}
			if (isBroken && this.particleDelay <= 0f)
			{
				Vector3 pos = this.WireNode.GetEndPosition() + this.WireNode.GetEndPositionOffset();
				float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(this.BlockPosition.ToVector3())) / 2f;
				ParticleEffect pe = new ParticleEffect("electric_fence_sparks", pos, lightValue, new Color(1f, 1f, 1f, 0.3f), "electric_fence_impact", null, false);
				GameManager.Instance.SpawnParticleEffectServer(pe, -1, true, true);
				this.particleDelay = 1f + UnityEngine.Random.value * 4f;
			}
		}
	}

	// Token: 0x04001180 RID: 4480
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDipValue = 0.25f;

	// Token: 0x04001181 RID: 4481
	public TileEntityPoweredMeleeTrap TileEntityParent;

	// Token: 0x04001182 RID: 4482
	public TileEntityPoweredMeleeTrap TileEntityChild;

	// Token: 0x04001183 RID: 4483
	public IWireNode WireNode;

	// Token: 0x04001184 RID: 4484
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float healthRatio = -1f;

	// Token: 0x04001185 RID: 4485
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> buffActions;

	// Token: 0x04001186 RID: 4486
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static string PropDamageReceived = "Damage_received";

	// Token: 0x04001187 RID: 4487
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float damageReceived;

	// Token: 0x04001188 RID: 4488
	public Vector3i BlockPosition;

	// Token: 0x04001189 RID: 4489
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Chunk chunk;

	// Token: 0x0400118A RID: 4490
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float totalDamage;

	// Token: 0x0400118B RID: 4491
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float particleDelay;

	// Token: 0x0400118C RID: 4492
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float brokenPercentage;

	// Token: 0x0400118D RID: 4493
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float breakingPercentage;

	// Token: 0x0400118E RID: 4494
	public int OwnerEntityID = -1;

	// Token: 0x0400118F RID: 4495
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 startPoint;

	// Token: 0x04001190 RID: 4496
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 endPoint;

	// Token: 0x04001191 RID: 4497
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Collider> CollidersThisFrame;

	// Token: 0x04001192 RID: 4498
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastHealthRatio = 1f;
}
