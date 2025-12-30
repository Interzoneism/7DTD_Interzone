using System;
using UnityEngine;

// Token: 0x020012DC RID: 4828
public static class vp_MathUtility
{
	// Token: 0x0600965F RID: 38495 RVA: 0x003BCCE5 File Offset: 0x003BAEE5
	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		value = (double.IsNaN((double)value) ? prevValue : value);
		return value;
	}

	// Token: 0x06009660 RID: 38496 RVA: 0x003BCCF8 File Offset: 0x003BAEF8
	public static Vector2 NaNSafeVector2(Vector2 vector, Vector2 prevVector = default(Vector2))
	{
		vector.x = (double.IsNaN((double)vector.x) ? prevVector.x : vector.x);
		vector.y = (double.IsNaN((double)vector.y) ? prevVector.y : vector.y);
		return vector;
	}

	// Token: 0x06009661 RID: 38497 RVA: 0x003BCD4C File Offset: 0x003BAF4C
	public static Vector3 NaNSafeVector3(Vector3 vector, Vector3 prevVector = default(Vector3))
	{
		vector.x = (double.IsNaN((double)vector.x) ? prevVector.x : vector.x);
		vector.y = (double.IsNaN((double)vector.y) ? prevVector.y : vector.y);
		vector.z = (double.IsNaN((double)vector.z) ? prevVector.z : vector.z);
		return vector;
	}

	// Token: 0x06009662 RID: 38498 RVA: 0x003BCDC4 File Offset: 0x003BAFC4
	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, Quaternion prevQuaternion = default(Quaternion))
	{
		quaternion.x = (double.IsNaN((double)quaternion.x) ? prevQuaternion.x : quaternion.x);
		quaternion.y = (double.IsNaN((double)quaternion.y) ? prevQuaternion.y : quaternion.y);
		quaternion.z = (double.IsNaN((double)quaternion.z) ? prevQuaternion.z : quaternion.z);
		quaternion.w = (double.IsNaN((double)quaternion.w) ? prevQuaternion.w : quaternion.w);
		return quaternion;
	}

	// Token: 0x06009663 RID: 38499 RVA: 0x003BCE60 File Offset: 0x003BB060
	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		value.x = ((Mathf.Abs(value.x) < epsilon) ? 0f : value.x);
		value.y = ((Mathf.Abs(value.y) < epsilon) ? 0f : value.y);
		value.z = ((Mathf.Abs(value.z) < epsilon) ? 0f : value.z);
		return value;
	}

	// Token: 0x06009664 RID: 38500 RVA: 0x003BCED4 File Offset: 0x003BB0D4
	public static float SnapToZero(float value, float epsilon = 0.0001f)
	{
		value = ((Mathf.Abs(value) < epsilon) ? 0f : value);
		return value;
	}

	// Token: 0x06009665 RID: 38501 RVA: 0x003BCEEA File Offset: 0x003BB0EA
	public static float ReduceDecimals(float value, float factor = 1000f)
	{
		return Mathf.Round(value * factor) / factor;
	}

	// Token: 0x06009666 RID: 38502 RVA: 0x003BCEF6 File Offset: 0x003BB0F6
	public static bool IsOdd(int val)
	{
		return val % 2 != 0;
	}

	// Token: 0x06009667 RID: 38503 RVA: 0x003BCEFE File Offset: 0x003BB0FE
	public static float Sinus(float rate, float amp, float offset = 0f)
	{
		return Mathf.Cos((Time.time + offset) * rate) * amp;
	}
}
