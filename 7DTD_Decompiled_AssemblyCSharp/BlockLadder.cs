using System;
using UnityEngine.Scripting;

// Token: 0x02000106 RID: 262
[Preserve]
public class BlockLadder : Block
{
	// Token: 0x060006EB RID: 1771 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsElevator()
	{
		return true;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x000308E4 File Offset: 0x0002EAE4
	public override bool IsElevator(int rotation)
	{
		return BlockLadder.climbableRotations[rotation] > 0;
	}

	// Token: 0x040007DC RID: 2012
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] climbableRotations = new byte[]
	{
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		0,
		1,
		0,
		1,
		1,
		0,
		1,
		0,
		0,
		1,
		0,
		1,
		1,
		0,
		1,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};
}
