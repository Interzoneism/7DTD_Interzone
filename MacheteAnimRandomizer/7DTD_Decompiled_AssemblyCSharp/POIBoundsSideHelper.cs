using System;
using UnityEngine;

// Token: 0x020004CD RID: 1229
public class POIBoundsSideHelper : MonoBehaviour
{
	// Token: 0x06002810 RID: 10256 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x0010452E File Offset: 0x0010272E
	public void Setup()
	{
		this.MeshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x0010453C File Offset: 0x0010273C
	public void SetSize(Vector3 size)
	{
		switch (this.SideType)
		{
		case POIBoundsSideHelper.SideTypes.PositiveX:
			base.transform.localPosition = new Vector3(size.z * 0.5f, 0f, 0f);
			base.transform.localScale = new Vector3(size.x, 100f, 6f);
			return;
		case POIBoundsSideHelper.SideTypes.NegativeX:
			base.transform.localPosition = new Vector3(size.z * -0.5f, 0f, 0f);
			base.transform.localScale = new Vector3(size.x, 100f, 6f);
			return;
		case POIBoundsSideHelper.SideTypes.PositiveZ:
			base.transform.localPosition = new Vector3(0f, 0f, size.x * 0.5f);
			base.transform.localScale = new Vector3(size.z, 100f, 6f);
			return;
		case POIBoundsSideHelper.SideTypes.NegativeZ:
			base.transform.localPosition = new Vector3(0f, 0f, size.x * -0.5f);
			base.transform.localScale = new Vector3(size.z, 100f, 6f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x00104682 File Offset: 0x00102882
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 24)
		{
			this.Owner.AddSideEntered(this);
		}
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x0010469F File Offset: 0x0010289F
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 24)
		{
			this.Owner.RemoveSideEntered(this);
		}
	}

	// Token: 0x04001ECC RID: 7884
	public POIBoundsSideHelper.SideTypes SideType;

	// Token: 0x04001ECD RID: 7885
	public POIBoundsHelper Owner;

	// Token: 0x04001ECE RID: 7886
	public MeshRenderer MeshRenderer;

	// Token: 0x020004CE RID: 1230
	public enum SideTypes
	{
		// Token: 0x04001ED0 RID: 7888
		PositiveX,
		// Token: 0x04001ED1 RID: 7889
		NegativeX,
		// Token: 0x04001ED2 RID: 7890
		PositiveZ,
		// Token: 0x04001ED3 RID: 7891
		NegativeZ
	}
}
