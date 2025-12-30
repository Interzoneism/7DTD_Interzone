using System;
using GameSparks.RT;
using UnityEngine;

// Token: 0x0200001B RID: 27
public static class RTDataExtensions
{
	// Token: 0x060000AB RID: 171 RVA: 0x0000997C File Offset: 0x00007B7C
	public static RTData SetVector2(this RTData data, uint index, Vector2 vector2)
	{
		data.SetRTVector(index, new RTVector(vector2.x, vector2.y));
		return data;
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00009998 File Offset: 0x00007B98
	public static Vector2? GetVector2(this RTData data, uint index)
	{
		if (data.GetRTVector(index) != null)
		{
			RTVector value = data.GetRTVector(index).Value;
			return new Vector2?(new Vector2(value.x.Value, value.y.Value));
		}
		return null;
	}

	// Token: 0x060000AD RID: 173 RVA: 0x000099F2 File Offset: 0x00007BF2
	public static RTData SetVector3(this RTData data, uint index, Vector3 vector3)
	{
		data.SetRTVector(index, new RTVector(vector3.x, vector3.y, vector3.z));
		return data;
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00009A14 File Offset: 0x00007C14
	public static Vector3? GetVector3(this RTData data, uint index)
	{
		if (data.GetRTVector(index) == null)
		{
			return null;
		}
		RTVector value = data.GetRTVector(index).Value;
		if (value.z == null)
		{
			return null;
		}
		return new Vector3?(new Vector3(value.x.Value, value.y.Value, value.z.Value));
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00009A92 File Offset: 0x00007C92
	public static RTData SetVector4(this RTData data, uint index, Vector4 vector4)
	{
		data.SetRTVector(index, new RTVector(vector4.x, vector4.y, vector4.z, vector4.w));
		return data;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00009ABC File Offset: 0x00007CBC
	public static Vector4? GetVector4(this RTData data, uint index)
	{
		if (data.GetRTVector(index) == null)
		{
			return null;
		}
		RTVector value = data.GetRTVector(index).Value;
		if (value.w == null)
		{
			return null;
		}
		return new Vector4?(new Vector4(value.x.Value, value.y.Value, value.z.Value, value.w.Value));
	}
}
