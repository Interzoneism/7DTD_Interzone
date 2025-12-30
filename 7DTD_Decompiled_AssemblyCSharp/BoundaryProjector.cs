using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010EE RID: 4334
public class BoundaryProjector : MonoBehaviour
{
	// Token: 0x06008820 RID: 34848 RVA: 0x0037166C File Offset: 0x0036F86C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		Projector[] componentsInChildren = base.transform.GetComponentsInChildren<Projector>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.ProjectorList.Add(new BoundaryProjector.ProjectorEntry
			{
				Projector = componentsInChildren[i],
				EffectData = new BoundaryProjector.ProjectorEffectData()
			});
		}
		this.SetupProjectors();
	}

	// Token: 0x06008821 RID: 34849 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetupProjectors()
	{
	}

	// Token: 0x06008822 RID: 34850 RVA: 0x003716C0 File Offset: 0x0036F8C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		for (int i = 0; i < this.ProjectorList.Count; i++)
		{
			if (this.ProjectorList[i] != null && this.ProjectorList[i].Projector.gameObject.activeSelf)
			{
				BoundaryProjector.ProjectorEntry projectorEntry = this.ProjectorList[i];
				if (projectorEntry.EffectData.AutoRotate)
				{
					Vector3 eulerAngles = projectorEntry.Projector.transform.localRotation.eulerAngles;
					projectorEntry.Projector.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + Time.deltaTime * projectorEntry.EffectData.RotationSpeed, eulerAngles.z);
				}
				if (projectorEntry.EffectData.targetRadius != -1f)
				{
					projectorEntry.Projector.orthographicSize = Mathf.Lerp(projectorEntry.Projector.orthographicSize, projectorEntry.EffectData.targetRadius, Time.deltaTime);
					if (projectorEntry.Projector.orthographicSize == projectorEntry.EffectData.targetRadius)
					{
						projectorEntry.EffectData.targetRadius = -1f;
					}
				}
				if (projectorEntry.EffectData.IsGlowing)
				{
					Color color = projectorEntry.Projector.material.color;
					float num = Mathf.PingPong(Time.time, 0.25f);
					projectorEntry.Projector.material.color = new Color(color.r, color.g, color.b, 0.5f + num * 2f);
				}
			}
		}
		if (this.targetPos != BoundaryProjector.invalidPos)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPos, Time.deltaTime);
			if (base.transform.position == this.targetPos)
			{
				this.targetPos = BoundaryProjector.invalidPos;
			}
		}
	}

	// Token: 0x06008823 RID: 34851 RVA: 0x003718B0 File Offset: 0x0036FAB0
	public void SetRadius(int projectorID, float size)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			if (this.ProjectorList[projectorID].Projector.orthographicSize == -1f || size == 0f)
			{
				this.ProjectorList[projectorID].Projector.orthographicSize = size;
				this.ProjectorList[projectorID].EffectData.targetRadius = -1f;
				return;
			}
			this.ProjectorList[projectorID].EffectData.targetRadius = size;
		}
	}

	// Token: 0x06008824 RID: 34852 RVA: 0x00371948 File Offset: 0x0036FB48
	public void SetAlpha(int projectorID, float alpha)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			Color color = this.ProjectorList[projectorID].Projector.material.color;
			this.ProjectorList[projectorID].Projector.material.color = new Color(color.r, color.g, color.b, alpha);
		}
	}

	// Token: 0x06008825 RID: 34853 RVA: 0x003719C0 File Offset: 0x0036FBC0
	public void SetGlow(int projectorID, bool isGlowing)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			Color color = this.ProjectorList[projectorID].Projector.material.color;
			this.ProjectorList[projectorID].EffectData.IsGlowing = isGlowing;
		}
	}

	// Token: 0x06008826 RID: 34854 RVA: 0x00371A1C File Offset: 0x0036FC1C
	public void SetAutoRotate(int projectorID, bool autoRotate, float rotateSpeed)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			this.ProjectorList[projectorID].EffectData.AutoRotate = autoRotate;
			this.ProjectorList[projectorID].EffectData.RotationSpeed = rotateSpeed;
		}
	}

	// Token: 0x06008827 RID: 34855 RVA: 0x00371A73 File Offset: 0x0036FC73
	public void SetMoveToPosition(Vector3 vNew)
	{
		if (base.transform.position.y == -999f)
		{
			base.transform.position = vNew;
			return;
		}
		this.targetPos = vNew;
	}

	// Token: 0x040069FD RID: 27133
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<BoundaryProjector.ProjectorEntry> ProjectorList = new List<BoundaryProjector.ProjectorEntry>();

	// Token: 0x040069FE RID: 27134
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Vector3 invalidPos = new Vector3(-999f, -999f, -999f);

	// Token: 0x040069FF RID: 27135
	public Vector3 targetPos = BoundaryProjector.invalidPos;

	// Token: 0x04006A00 RID: 27136
	public bool IsInitialized;

	// Token: 0x020010EF RID: 4335
	public class ProjectorEntry
	{
		// Token: 0x04006A01 RID: 27137
		public BoundaryProjector.ProjectorEffectData EffectData;

		// Token: 0x04006A02 RID: 27138
		public Projector Projector;
	}

	// Token: 0x020010F0 RID: 4336
	public class ProjectorEffectData
	{
		// Token: 0x04006A03 RID: 27139
		public bool AutoRotate;

		// Token: 0x04006A04 RID: 27140
		public float RotationSpeed;

		// Token: 0x04006A05 RID: 27141
		public bool IsGlowing;

		// Token: 0x04006A06 RID: 27142
		public float targetRadius = -1f;
	}
}
