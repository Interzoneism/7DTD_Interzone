using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000AD6 RID: 2774
public abstract class TGMAbstract
{
	// Token: 0x06005561 RID: 21857
	public abstract void SetSeed(int _seed);

	// Token: 0x06005562 RID: 21858
	public abstract float GetValue(float _x, float _z, float _biomeIntens);

	// Token: 0x06005563 RID: 21859 RVA: 0x000F387A File Offset: 0x000F1A7A
	public virtual Vector3 GetNormal(float _x, float _z, float _biomeIntens)
	{
		return Vector3.up;
	}

	// Token: 0x06005564 RID: 21860 RVA: 0x0022D1A4 File Offset: 0x0022B3A4
	public virtual void Init()
	{
		this.baseHeight = (this.properties.Values.ContainsKey("BaseHeight") ? StringParsers.ParseFloat(this.properties.Values["BaseHeight"], 0, -1, NumberStyles.Any) : -15f);
	}

	// Token: 0x06005565 RID: 21861 RVA: 0x0022D1F6 File Offset: 0x0022B3F6
	public virtual float GetBaseHeight()
	{
		return this.baseHeight;
	}

	// Token: 0x06005566 RID: 21862 RVA: 0x0022D1FE File Offset: 0x0022B3FE
	[PublicizedFrom(EAccessModifier.Protected)]
	public TGMAbstract()
	{
	}

	// Token: 0x040041FF RID: 16895
	[PublicizedFrom(EAccessModifier.Protected)]
	public float baseHeight;

	// Token: 0x04004200 RID: 16896
	public DynamicProperties properties = new DynamicProperties();

	// Token: 0x04004201 RID: 16897
	public bool IsSeedSet;
}
