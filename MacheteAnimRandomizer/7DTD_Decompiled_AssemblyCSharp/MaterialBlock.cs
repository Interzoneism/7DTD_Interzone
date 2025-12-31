using System;
using System.Collections.Generic;

// Token: 0x02000166 RID: 358
public class MaterialBlock
{
	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x0004561A File Offset: 0x0004381A
	// (set) Token: 0x06000AB7 RID: 2743 RVA: 0x00045622 File Offset: 0x00043822
	public float ExplosionResistance
	{
		get
		{
			return this.explosionResistance;
		}
		set
		{
			this.explosionResistance = Utils.FastClamp01(value);
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00045630 File Offset: 0x00043830
	[PublicizedFrom(EAccessModifier.Private)]
	static MaterialBlock()
	{
		MaterialBlock.air = new MaterialBlock();
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00045646 File Offset: 0x00043846
	public static void Cleanup()
	{
		MaterialBlock.materials = new Dictionary<string, MaterialBlock>();
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00045654 File Offset: 0x00043854
	public MaterialBlock()
	{
		this.Properties = new DynamicProperties();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000456BE File Offset: 0x000438BE
	public MaterialBlock(string _id) : this()
	{
		this.id = _id;
		this.IsCollidable = true;
		this.LightOpacity = 0;
		MaterialBlock.materials[_id] = this;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x000456E7 File Offset: 0x000438E7
	public static MaterialBlock fromString(string _name)
	{
		if (!MaterialBlock.materials.ContainsKey(_name))
		{
			return null;
		}
		return MaterialBlock.materials[_name];
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00045703 File Offset: 0x00043903
	public string GetLocalizedMaterialName()
	{
		return Localization.Get("material" + this.id, false);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0004571B File Offset: 0x0004391B
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool CheckDamageIgnore(FastTags<TagGroup.Global> _tags, EntityAlive _entity)
	{
		if (!this.IgnoreDamageFromTag.IsEmpty && _tags.Test_AnySet(this.IgnoreDamageFromTag))
		{
			if (_entity != null)
			{
				_entity.PlayOneShot("keystone_impact_overlay", false, false, false, null);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04000960 RID: 2400
	public static MaterialBlock air;

	// Token: 0x04000961 RID: 2401
	public static Dictionary<string, MaterialBlock> materials = new Dictionary<string, MaterialBlock>();

	// Token: 0x04000962 RID: 2402
	public DynamicProperties Properties;

	// Token: 0x04000963 RID: 2403
	public bool StabilitySupport = true;

	// Token: 0x04000964 RID: 2404
	public DataItem<float> Hardness;

	// Token: 0x04000965 RID: 2405
	public StepSound stepSound;

	// Token: 0x04000966 RID: 2406
	public bool IsCollidable;

	// Token: 0x04000967 RID: 2407
	public int LightOpacity;

	// Token: 0x04000968 RID: 2408
	public int StabilityGlue = 6;

	// Token: 0x04000969 RID: 2409
	public bool IsLiquid;

	// Token: 0x0400096A RID: 2410
	public string DamageCategory;

	// Token: 0x0400096B RID: 2411
	public string SurfaceCategory;

	// Token: 0x0400096C RID: 2412
	public string ParticleCategory;

	// Token: 0x0400096D RID: 2413
	public string ParticleDestroyCategory;

	// Token: 0x0400096E RID: 2414
	public string ForgeCategory;

	// Token: 0x0400096F RID: 2415
	public int FertileLevel;

	// Token: 0x04000970 RID: 2416
	public bool IsPlant;

	// Token: 0x04000971 RID: 2417
	public float MovementFactor = 1f;

	// Token: 0x04000972 RID: 2418
	public float Friction = 1f;

	// Token: 0x04000973 RID: 2419
	public DataItem<int> Mass;

	// Token: 0x04000974 RID: 2420
	public int MaxDamage;

	// Token: 0x04000975 RID: 2421
	public int MaxIncomingDamage = int.MaxValue;

	// Token: 0x04000976 RID: 2422
	public float Experience = 1f;

	// Token: 0x04000977 RID: 2423
	public string id;

	// Token: 0x04000978 RID: 2424
	public FastTags<TagGroup.Global> IgnoreDamageFromTag = FastTags<TagGroup.Global>.none;

	// Token: 0x04000979 RID: 2425
	public bool IsGroundCover;

	// Token: 0x0400097A RID: 2426
	public bool CanDestroy = true;

	// Token: 0x0400097B RID: 2427
	[PublicizedFrom(EAccessModifier.Private)]
	public float explosionResistance;
}
