using System;
using UnityEngine.Scripting;

// Token: 0x02000EA0 RID: 3744
[Preserve]
public class XUiC_VehiclePartStackGrid : XUiC_ItemPartStackGrid
{
	// Token: 0x17000C0E RID: 3086
	// (get) Token: 0x0600763A RID: 30266 RVA: 0x00075E2B File Offset: 0x0007402B
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Vehicle;
		}
	}

	// Token: 0x17000C0F RID: 3087
	// (get) Token: 0x0600763B RID: 30267 RVA: 0x00302352 File Offset: 0x00300552
	// (set) Token: 0x0600763C RID: 30268 RVA: 0x0030235A File Offset: 0x0030055A
	public Vehicle CurrentVehicle { get; set; }

	// Token: 0x0600763D RID: 30269 RVA: 0x00302363 File Offset: 0x00300563
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x0600763E RID: 30270 RVA: 0x0030236B File Offset: 0x0030056B
	public void SetMods(ItemValue[] mods)
	{
		base.SetParts(mods);
	}
}
