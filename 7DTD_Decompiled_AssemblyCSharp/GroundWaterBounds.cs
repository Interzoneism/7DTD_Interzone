using System;
using Unity.Mathematics;

// Token: 0x02000B5F RID: 2911
public struct GroundWaterBounds
{
	// Token: 0x1700093B RID: 2363
	// (get) Token: 0x06005A80 RID: 23168 RVA: 0x002449B9 File Offset: 0x00242BB9
	public bool IsGroundWater
	{
		get
		{
			return this.state > 0;
		}
	}

	// Token: 0x06005A81 RID: 23169 RVA: 0x002449C4 File Offset: 0x00242BC4
	public GroundWaterBounds(int _groundHeight, int _waterHeight)
	{
		this.state = 1;
		this.waterHeight = (byte)math.clamp(_waterHeight, 0, 255);
		this.bottom = (byte)math.clamp(_groundHeight, 0, (int)this.waterHeight);
	}

	// Token: 0x04004534 RID: 17716
	[PublicizedFrom(EAccessModifier.Private)]
	public byte state;

	// Token: 0x04004535 RID: 17717
	public byte waterHeight;

	// Token: 0x04004536 RID: 17718
	public byte bottom;
}
