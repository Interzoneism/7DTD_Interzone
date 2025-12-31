using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004CB RID: 1227
public class POIBoundsHelper : MonoBehaviour
{
	// Token: 0x06002809 RID: 10249 RVA: 0x00104194 File Offset: 0x00102394
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (this.SideHelpers != null && this.SideHelpers.Count > 0)
		{
			this.SideHelpers[0].Setup();
			this.material = new Material(this.SideHelpers[0].MeshRenderer.material);
			this.fullAlpha = this.material.color.a;
			this.material.color = new Color(this.material.color.r, this.material.color.g, this.material.color.b, 0f);
			for (int i = 0; i < this.SideHelpers.Count; i++)
			{
				this.SideHelpers[i].Owner = this;
				this.SideHelpers[i].Setup();
				this.SideHelpers[i].MeshRenderer.material = this.material;
				this.SideHelpers[i].MeshRenderer.enabled = false;
			}
		}
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x001042B8 File Offset: 0x001024B8
	public void SetSidesVisible(bool visible)
	{
		for (int i = 0; i < this.SideHelpers.Count; i++)
		{
			this.SideHelpers[i].MeshRenderer.enabled = visible;
		}
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x001042F4 File Offset: 0x001024F4
	public void SetPosition(Vector3 position, Vector3 size)
	{
		base.transform.position = position;
		for (int i = 0; i < this.SideHelpers.Count; i++)
		{
			this.SideHelpers[i].SetSize(size);
		}
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x00104338 File Offset: 0x00102538
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		switch (this.CurrentState)
		{
		case POIBoundsHelper.WallVisibilityStates.None:
		case POIBoundsHelper.WallVisibilityStates.Visible:
			break;
		case POIBoundsHelper.WallVisibilityStates.Showing:
		{
			float num = Mathf.MoveTowards(this.material.color.a, this.fullAlpha, Time.deltaTime);
			this.material.color = new Color(this.material.color.r, this.material.color.g, this.material.color.b, num);
			if (num == this.fullAlpha)
			{
				this.showTime = 3f;
				this.CurrentState = POIBoundsHelper.WallVisibilityStates.Visible;
				return;
			}
			break;
		}
		case POIBoundsHelper.WallVisibilityStates.ReadyToHide:
			this.showTime -= Time.deltaTime;
			if (this.showTime <= 0f)
			{
				this.CurrentState = POIBoundsHelper.WallVisibilityStates.Hiding;
				return;
			}
			break;
		case POIBoundsHelper.WallVisibilityStates.Hiding:
		{
			float num2 = Mathf.MoveTowards(this.material.color.a, 0f, Time.deltaTime);
			this.material.color = new Color(this.material.color.r, this.material.color.g, this.material.color.b, num2);
			if (num2 == 0f)
			{
				this.CurrentState = POIBoundsHelper.WallVisibilityStates.None;
				this.SetSidesVisible(false);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x00104486 File Offset: 0x00102686
	public void AddSideEntered(POIBoundsSideHelper side)
	{
		if (!this.ActivatedHelpers.Contains(side))
		{
			this.ActivatedHelpers.Add(side);
		}
		if (this.ActivatedHelpers.Count > 0)
		{
			this.CurrentState = POIBoundsHelper.WallVisibilityStates.Showing;
			this.SetSidesVisible(true);
		}
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x001044BE File Offset: 0x001026BE
	public void RemoveSideEntered(POIBoundsSideHelper side)
	{
		if (this.ActivatedHelpers.Contains(side))
		{
			this.ActivatedHelpers.Remove(side);
		}
		if (this.ActivatedHelpers.Count == 0)
		{
			this.CurrentState = POIBoundsHelper.WallVisibilityStates.ReadyToHide;
			this.showTime = 3f;
		}
	}

	// Token: 0x04001EC0 RID: 7872
	public List<POIBoundsSideHelper> SideHelpers = new List<POIBoundsSideHelper>();

	// Token: 0x04001EC1 RID: 7873
	public List<POIBoundsSideHelper> ActivatedHelpers = new List<POIBoundsSideHelper>();

	// Token: 0x04001EC2 RID: 7874
	public POIBoundsHelper.WallVisibilityStates CurrentState;

	// Token: 0x04001EC3 RID: 7875
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material material;

	// Token: 0x04001EC4 RID: 7876
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float showTime = 3f;

	// Token: 0x04001EC5 RID: 7877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fullAlpha = -1f;

	// Token: 0x020004CC RID: 1228
	public enum WallVisibilityStates
	{
		// Token: 0x04001EC7 RID: 7879
		None,
		// Token: 0x04001EC8 RID: 7880
		Showing,
		// Token: 0x04001EC9 RID: 7881
		Visible,
		// Token: 0x04001ECA RID: 7882
		ReadyToHide,
		// Token: 0x04001ECB RID: 7883
		Hiding
	}
}
