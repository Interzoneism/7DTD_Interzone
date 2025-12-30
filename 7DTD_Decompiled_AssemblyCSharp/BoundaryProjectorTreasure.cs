using System;

// Token: 0x020010F1 RID: 4337
public class BoundaryProjectorTreasure : BoundaryProjector
{
	// Token: 0x17000E41 RID: 3649
	// (get) Token: 0x0600882C RID: 34860 RVA: 0x00371AEC File Offset: 0x0036FCEC
	// (set) Token: 0x0600882D RID: 34861 RVA: 0x00371AF4 File Offset: 0x0036FCF4
	public bool WithinRadius
	{
		get
		{
			return this.withinRadius;
		}
		set
		{
			if (this.withinRadius != value)
			{
				this.withinRadius = value;
				this.HandleWithinRadiusChange();
			}
		}
	}

	// Token: 0x17000E42 RID: 3650
	// (get) Token: 0x0600882E RID: 34862 RVA: 0x00371B0C File Offset: 0x0036FD0C
	public float CurrentRadius
	{
		get
		{
			return this.ProjectorList[0].Projector.orthographicSize;
		}
	}

	// Token: 0x0600882F RID: 34863 RVA: 0x00371B24 File Offset: 0x0036FD24
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetupProjectors()
	{
		base.SetAlpha(0, 1f);
		base.SetAutoRotate(0, true, 2f);
	}

	// Token: 0x06008830 RID: 34864 RVA: 0x00371B3F File Offset: 0x0036FD3F
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleWithinRadiusChange()
	{
		base.SetGlow(0, this.withinRadius);
	}

	// Token: 0x04006A07 RID: 27143
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool withinRadius;
}
