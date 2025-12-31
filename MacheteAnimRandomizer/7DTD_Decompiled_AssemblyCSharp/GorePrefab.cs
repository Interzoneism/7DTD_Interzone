using System;

// Token: 0x020000C7 RID: 199
public class GorePrefab : RootTransformRefEntity
{
	// Token: 0x1700005A RID: 90
	// (set) Token: 0x060004DE RID: 1246 RVA: 0x00023319 File Offset: 0x00021519
	public bool restoreState
	{
		set
		{
			this._restoreState = value;
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00023324 File Offset: 0x00021524
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		if (!this._restoreState && this.RootTransform != null && this.Sound != null && this.Sound != string.Empty)
		{
			this.RootTransform.GetComponent<Entity>().PlayOneShot(this.Sound, false, false, false, null);
		}
	}

	// Token: 0x0400059A RID: 1434
	public string Sound;

	// Token: 0x0400059B RID: 1435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool _restoreState;
}
