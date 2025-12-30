using System;

// Token: 0x02000376 RID: 886
public class MeshTiming
{
	// Token: 0x06001A25 RID: 6693 RVA: 0x000A3280 File Offset: 0x000A1480
	public string Details()
	{
		double num = this.CopyVerts + this.CopyUv + this.CopyUv2 + this.CopyUv3 + this.CopyUv4 + this.CopyColours + this.CopyTriangles + this.CopyNormals + this.CopyTangents + this.UploadMesh + this.NormalRecalc;
		return string.Format("\r\nCopyVerts: {0}\r\nCopyUv:{1}\r\nCopyUv2:{2}\r\nCopyUv3:{3}\r\nCopyUv4:{4}\r\nCopyColours:{5}\r\nCopyTriangles:{6}\r\nCopyNormals:{7}\r\nCopyTangents:{8}\r\nUploadMesh:{9}\r\nNormalRecalc:{10}\r\nTotal: {11}\r\n", new object[]
		{
			this.CopyVerts,
			this.CopyUv,
			this.CopyUv2,
			this.CopyUv3,
			this.CopyUv4,
			this.CopyColours,
			this.CopyTriangles,
			this.CopyNormals,
			this.CopyTangents,
			this.UploadMesh,
			this.NormalRecalc,
			num
		});
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001A26 RID: 6694 RVA: 0x000A3391 File Offset: 0x000A1591
	public double time
	{
		get
		{
			return this.GetTime();
		}
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x000A339C File Offset: 0x000A159C
	public double GetTime()
	{
		return (DateTime.Now - this.Start).TotalMilliseconds;
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x000A33C1 File Offset: 0x000A15C1
	public void Reset()
	{
		this.Start = DateTime.Now;
	}

	// Token: 0x040010ED RID: 4333
	public double CopyVerts;

	// Token: 0x040010EE RID: 4334
	public double CopyUv;

	// Token: 0x040010EF RID: 4335
	public double CopyUv2;

	// Token: 0x040010F0 RID: 4336
	public double CopyUv3;

	// Token: 0x040010F1 RID: 4337
	public double CopyUv4;

	// Token: 0x040010F2 RID: 4338
	public double CopyColours;

	// Token: 0x040010F3 RID: 4339
	public double CopyTriangles;

	// Token: 0x040010F4 RID: 4340
	public double CopyNormals;

	// Token: 0x040010F5 RID: 4341
	public double CopyTangents;

	// Token: 0x040010F6 RID: 4342
	public double UploadMesh;

	// Token: 0x040010F7 RID: 4343
	public double NormalRecalc;

	// Token: 0x040010F8 RID: 4344
	public DateTime Start;
}
