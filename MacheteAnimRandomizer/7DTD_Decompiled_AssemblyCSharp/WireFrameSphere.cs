using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B80 RID: 2944
[RequireComponent(typeof(LineRenderer))]
public class WireFrameSphere : MonoBehaviour
{
	// Token: 0x06005B45 RID: 23365 RVA: 0x002489E0 File Offset: 0x00246BE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.lr = base.gameObject.GetComponent<LineRenderer>();
		this.lr.startWidth = 0.01f;
		this.lr.endWidth = 0.01f;
		this.positions = new List<Vector3>();
		this.player = UnityEngine.Object.FindObjectOfType<LocalPlayerCamera>();
	}

	// Token: 0x06005B46 RID: 23366 RVA: 0x00248A34 File Offset: 0x00246C34
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.player != null)
		{
			float num = Mathf.Abs(this.player.transform.position.magnitude - base.transform.position.magnitude);
			this.lr.startWidth = (this.lr.endWidth = 0.01f * num);
		}
		if (this.radius != this.newRadius)
		{
			this.radius = this.newRadius;
			this.positions.Clear();
			this.positions.AddRange(this.RenderCircleOnPlane(true));
			this.positions.AddRange(this.RenderCircleOnPlane(false));
			this.lr.positionCount = this.positions.Count;
			this.lr.SetPositions(this.positions.ToArray());
		}
	}

	// Token: 0x06005B47 RID: 23367 RVA: 0x00248B18 File Offset: 0x00246D18
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3> RenderCircleOnPlane(bool xyPlane)
	{
		this.numOfVertices = 100 + (int)Math.Pow((double)((int)this.radius), 2.0);
		List<Vector3> list = new List<Vector3>(this.numOfVertices);
		this.angle = 6.2831855f / (float)(this.numOfVertices - 1);
		for (int i = 0; i < this.numOfVertices; i++)
		{
			float x = this.center.x + this.radius * Mathf.Cos((float)i * this.angle);
			float y = this.center.y + (xyPlane ? (this.radius * Mathf.Sin((float)i * this.angle)) : 0f);
			float z = this.center.z + (xyPlane ? 0f : (this.radius * Mathf.Sin((float)i * this.angle)));
			list.Add(new Vector3(x, y, z));
		}
		return list;
	}

	// Token: 0x06005B48 RID: 23368 RVA: 0x0012CE9D File Offset: 0x0012B09D
	public void KillWF()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040045C6 RID: 17862
	public Vector3 center;

	// Token: 0x040045C7 RID: 17863
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float TAU = 6.2831855f;

	// Token: 0x040045C8 RID: 17864
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float radius = 1f;

	// Token: 0x040045C9 RID: 17865
	public float newRadius = 1f;

	// Token: 0x040045CA RID: 17866
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LineRenderer lr;

	// Token: 0x040045CB RID: 17867
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> positions;

	// Token: 0x040045CC RID: 17868
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numOfVertices = 100;

	// Token: 0x040045CD RID: 17869
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float angle;

	// Token: 0x040045CE RID: 17870
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerCamera player;
}
