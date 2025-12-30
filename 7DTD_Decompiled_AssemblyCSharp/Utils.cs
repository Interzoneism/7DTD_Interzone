using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Platform;
using Unity.Collections;
using UnityEngine;

// Token: 0x0200124B RID: 4683
public static class Utils
{
	// Token: 0x17000F1E RID: 3870
	// (get) Token: 0x06009242 RID: 37442 RVA: 0x003A415B File Offset: 0x003A235B
	public static CultureInfo StandardCulture
	{
		get
		{
			return Utils.ci;
		}
	}

	// Token: 0x06009243 RID: 37443 RVA: 0x003A4164 File Offset: 0x003A2364
	public static void InitStatic()
	{
		Utils.ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
		Utils.ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		Utils.ci.NumberFormat.CurrencyDecimalSeparator = ".";
		Utils.ci.NumberFormat.NumberDecimalSeparator = ".";
		Utils.ci.NumberFormat.NumberGroupSeparator = ",";
		Utils.ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
		Utils.ci.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
		Utils.ci.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
		Utils.ci.DateTimeFormat.LongTimePattern = "HH:mm:ss";
		if (!GlobalCultureInfo.SetDefaultCulture(Utils.ci))
		{
			Log.Warning("Setting global culture failed!");
		}
		DynamicMethod dynamicMethod = new DynamicMethod("Memset", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, null, new Type[]
		{
			typeof(IntPtr),
			typeof(byte),
			typeof(int)
		}, typeof(Utils), true);
		ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
		ilgenerator.Emit(OpCodes.Ldarg_0);
		ilgenerator.Emit(OpCodes.Ldarg_1);
		ilgenerator.Emit(OpCodes.Ldarg_2);
		ilgenerator.Emit(OpCodes.Initblk);
		ilgenerator.Emit(OpCodes.Ret);
		Utils.MemsetDelegate = (Action<IntPtr, byte, int>)dynamicMethod.CreateDelegate(typeof(Action<IntPtr, byte, int>));
	}

	// Token: 0x06009244 RID: 37444 RVA: 0x003A42D8 File Offset: 0x003A24D8
	public static string GetChildPath(this Transform t, Transform child)
	{
		string text = child.name;
		Transform parent = child.parent;
		while (parent != null && parent != t)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		if (parent == null)
		{
			throw new Exception(t.name + " does not have a child " + child.name);
		}
		return text;
	}

	// Token: 0x06009245 RID: 37445 RVA: 0x003A4348 File Offset: 0x003A2548
	public static void SetTagsRecursively(Transform t, string tag)
	{
		t.gameObject.tag = tag;
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			Utils.SetTagsRecursively(t.GetChild(i), tag);
		}
	}

	// Token: 0x06009246 RID: 37446 RVA: 0x003A4384 File Offset: 0x003A2584
	public static void SetTagsIfNoneRecursively(Transform t, string tag)
	{
		if (t.gameObject.CompareTag("Untagged"))
		{
			t.gameObject.tag = tag;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			Utils.SetTagsIfNoneRecursively(t.GetChild(i), tag);
		}
	}

	// Token: 0x06009247 RID: 37447 RVA: 0x003A43D0 File Offset: 0x003A25D0
	public static void SetTagsIfMatchRecursively(Transform t, string matchTag, string tag)
	{
		if (t.gameObject.CompareTag(matchTag))
		{
			t.gameObject.tag = tag;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			Utils.SetTagsIfMatchRecursively(t.GetChild(i), matchTag, tag);
		}
	}

	// Token: 0x06009248 RID: 37448 RVA: 0x003A4418 File Offset: 0x003A2618
	public static void DestroyWithTagRecursively(Transform t, string tag)
	{
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			Utils.DestroyWithTagRecursively(t.GetChild(i), tag);
		}
		if (t.gameObject.CompareTag(tag))
		{
			UnityEngine.Object.Destroy(t.gameObject);
		}
	}

	// Token: 0x06009249 RID: 37449 RVA: 0x003A4460 File Offset: 0x003A2660
	public static void SetLayerRecursively(GameObject go, int newLayer)
	{
		go.GetComponentsInChildren<Transform>(true, Utils.setLayerRecursivelyList);
		List<Transform> list = Utils.setLayerRecursivelyList;
		if (newLayer == 2)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				list[i].gameObject.layer = newLayer;
			}
		}
		else
		{
			for (int j = list.Count - 1; j >= 0; j--)
			{
				Transform transform = list[j];
				if (transform.CompareTag("LargeEntityBlocker"))
				{
					transform.gameObject.layer = 19;
				}
				else
				{
					transform.gameObject.layer = newLayer;
				}
			}
		}
		Utils.setLayerRecursivelyList.Clear();
	}

	// Token: 0x0600924A RID: 37450 RVA: 0x003A44F8 File Offset: 0x003A26F8
	public static void SetLayerRecursively(GameObject go, int newLayer, string[] excludeTags)
	{
		go.GetComponentsInChildren<Transform>(true, Utils.setLayerRecursivelyList);
		List<Transform> list = Utils.setLayerRecursivelyList;
		if (newLayer == 2)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Utils.SetLayerWithExclusionList(list[i].gameObject, newLayer, excludeTags);
			}
		}
		else
		{
			for (int j = list.Count - 1; j >= 0; j--)
			{
				Transform transform = list[j];
				if (transform.CompareTag("LargeEntityBlocker"))
				{
					Utils.SetLayerWithExclusionList(transform.gameObject, 19, excludeTags);
				}
				else
				{
					Utils.SetLayerWithExclusionList(transform.gameObject, newLayer, excludeTags);
				}
			}
		}
		Utils.setLayerRecursivelyList.Clear();
	}

	// Token: 0x0600924B RID: 37451 RVA: 0x003A4594 File Offset: 0x003A2794
	public static void SetLayerWithExclusionList(GameObject go, int layer, string[] excludeTags)
	{
		if (excludeTags != null)
		{
			foreach (string tag in excludeTags)
			{
				if (go.CompareTag(tag))
				{
					return;
				}
			}
		}
		go.layer = layer;
	}

	// Token: 0x0600924C RID: 37452 RVA: 0x003A45CC File Offset: 0x003A27CC
	public static void SetColliderLayerRecursively(GameObject go, int newLayer)
	{
		go.GetComponentsInChildren<Collider>(true, Utils.setColliderLayerRecursivelyList);
		for (int i = Utils.setColliderLayerRecursivelyList.Count - 1; i >= 0; i--)
		{
			Utils.setColliderLayerRecursivelyList[i].gameObject.layer = newLayer;
		}
	}

	// Token: 0x0600924D RID: 37453 RVA: 0x003A4614 File Offset: 0x003A2814
	public static void MoveTaggedToLayer(GameObject go, string tag, int newLayer)
	{
		go.GetComponentsInChildren<Transform>(true, Utils.setLayerRecursivelyList);
		List<Transform> list = Utils.setLayerRecursivelyList;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			Transform transform = list[i];
			if (transform.CompareTag(tag))
			{
				transform.gameObject.layer = newLayer;
			}
		}
		Utils.setLayerRecursivelyList.Clear();
	}

	// Token: 0x0600924E RID: 37454 RVA: 0x003A4670 File Offset: 0x003A2870
	public static void DrawBounds(Bounds _bounds, Color _color, float _duration)
	{
		Vector3 center = _bounds.center;
		Vector3 extents = _bounds.extents;
		Vector3 vector = new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z);
		Vector3 vector2 = new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z);
		Vector3 vector3 = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z);
		Vector3 vector4 = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z);
		Vector3 vector5 = new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z);
		Vector3 vector6 = new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z);
		Vector3 vector7 = new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z);
		Vector3 vector8 = new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z);
		UnityEngine.Debug.DrawLine(vector, vector2, _color, _duration);
		UnityEngine.Debug.DrawLine(vector2, vector4, _color, _duration);
		UnityEngine.Debug.DrawLine(vector4, vector3, _color, _duration);
		UnityEngine.Debug.DrawLine(vector3, vector, _color, _duration);
		UnityEngine.Debug.DrawLine(vector5, vector6, _color, _duration);
		UnityEngine.Debug.DrawLine(vector6, vector8, _color, _duration);
		UnityEngine.Debug.DrawLine(vector8, vector7, _color, _duration);
		UnityEngine.Debug.DrawLine(vector7, vector5, _color, _duration);
		UnityEngine.Debug.DrawLine(vector, vector5, _color, _duration);
		UnityEngine.Debug.DrawLine(vector2, vector6, _color, _duration);
		UnityEngine.Debug.DrawLine(vector4, vector8, _color, _duration);
		UnityEngine.Debug.DrawLine(vector3, vector7, _color, _duration);
	}

	// Token: 0x0600924F RID: 37455 RVA: 0x003A487C File Offset: 0x003A2A7C
	public static void DrawAxisLines(Vector3 centerPos, float radius, Color color, float duration)
	{
		Vector3 start = centerPos;
		Vector3 end = centerPos;
		start.x -= radius;
		end.x += radius;
		UnityEngine.Debug.DrawLine(start, end, color, duration);
		start.x = centerPos.x;
		end.x = centerPos.x;
		start.z -= radius;
		end.z += radius;
		UnityEngine.Debug.DrawLine(start, end, color, duration);
		start.z = centerPos.z;
		end.z = centerPos.z;
		start.y -= radius;
		end.y += radius;
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	// Token: 0x06009250 RID: 37456 RVA: 0x003A4924 File Offset: 0x003A2B24
	public static void DrawBoxLines(Vector3 minPos, Vector3 maxPos, Color color, float duration)
	{
		Utils.DrawBoxLinesHorizontal(minPos, maxPos, color, duration);
		Utils.DrawBoxLinesVerticle(minPos, maxPos, color, duration);
	}

	// Token: 0x06009251 RID: 37457 RVA: 0x003A4938 File Offset: 0x003A2B38
	public static void DrawBoxLinesHorizontal(Vector3 minPos, Vector3 maxPos, Color color, float duration)
	{
		Vector3 maxPos2 = new Vector3(maxPos.x, minPos.y, maxPos.z);
		Vector3 minPos2 = new Vector3(minPos.x, maxPos.y, minPos.z);
		Utils.DrawRectLinesHorzontal(minPos, maxPos2, color, duration);
		Utils.DrawRectLinesHorzontal(minPos2, maxPos, color, duration);
	}

	// Token: 0x06009252 RID: 37458 RVA: 0x003A4988 File Offset: 0x003A2B88
	public static void DrawBoxLinesVerticle(Vector3 minPos, Vector3 maxPos, Color color, float duration)
	{
		Vector3 vector = new Vector3(0f, maxPos.y - minPos.y);
		UnityEngine.Debug.DrawRay(minPos, vector, color, duration);
		UnityEngine.Debug.DrawRay(new Vector3(minPos.x, minPos.y, maxPos.z), vector, color, duration);
		UnityEngine.Debug.DrawRay(maxPos, -vector, color, duration);
		UnityEngine.Debug.DrawRay(new Vector3(maxPos.x, minPos.y, minPos.z), vector, color, duration);
	}

	// Token: 0x06009253 RID: 37459 RVA: 0x003A4A04 File Offset: 0x003A2C04
	public static void DrawCubeLines(Vector3 minPos, float size, Color color, float duration)
	{
		Vector3 maxPos = minPos;
		maxPos.x += size;
		maxPos.y += size;
		maxPos.z += size;
		Utils.DrawBoxLines(minPos, maxPos, color, duration);
	}

	// Token: 0x06009254 RID: 37460 RVA: 0x003A4A40 File Offset: 0x003A2C40
	public static void DrawRectLinesHorzontal(Vector3 minPos, Vector3 maxPos, Color color, float duration)
	{
		Vector3 vector = new Vector3(maxPos.x, minPos.y, minPos.z);
		Vector3 vector2 = new Vector3(minPos.x, minPos.y, maxPos.z);
		UnityEngine.Debug.DrawLine(minPos, vector2, color, duration);
		UnityEngine.Debug.DrawLine(vector2, maxPos, color, duration);
		UnityEngine.Debug.DrawLine(maxPos, vector, color, duration);
		UnityEngine.Debug.DrawLine(vector, minPos, color, duration);
	}

	// Token: 0x06009255 RID: 37461 RVA: 0x003A4AA3 File Offset: 0x003A2CA3
	public static void DrawCylinderLinesHorzontal(Vector3 centerPos, float radius, float height, Color startColor, Color endColor, int segments, float duration)
	{
		Utils.DrawCircleLinesHorzontal(centerPos, radius, startColor, endColor, segments, duration);
		centerPos.y += height;
		Utils.DrawCircleLinesHorzontal(centerPos, radius, startColor, endColor, segments, duration);
	}

	// Token: 0x06009256 RID: 37462 RVA: 0x003A4AD0 File Offset: 0x003A2CD0
	public static void DrawCircleLinesHorzontal(Vector3 centerPos, float radius, Color startColor, Color endColor, int segments, float duration)
	{
		if (segments < 3)
		{
			segments = 3;
		}
		float num = -6.2831855f / (float)segments;
		float num2 = 1.5707964f;
		float num3 = 0f;
		float num4 = 1f / (float)(segments - 1);
		Vector3 start;
		start.y = centerPos.y;
		start.x = centerPos.x + Mathf.Cos(num2) * radius;
		start.z = centerPos.z + Mathf.Sin(num2) * radius;
		Vector3 vector;
		vector.y = centerPos.y;
		for (int i = 0; i < segments; i++)
		{
			num2 += num;
			vector.x = centerPos.x + Mathf.Cos(num2) * radius;
			vector.z = centerPos.z + Mathf.Sin(num2) * radius;
			Color color = Color.Lerp(startColor, endColor, num3);
			UnityEngine.Debug.DrawLine(start, vector, color, duration);
			start.x = vector.x;
			start.z = vector.z;
			num3 += num4;
		}
	}

	// Token: 0x06009257 RID: 37463 RVA: 0x003A4BC8 File Offset: 0x003A2DC8
	public static void DrawLine(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor, int segments, float duration = 0f)
	{
		if (segments <= 1)
		{
			UnityEngine.Debug.DrawLine(startPos, endPos, startColor, duration);
			return;
		}
		Vector3 vector = (endPos - startPos) * (1f / (float)segments);
		float num = 0f;
		float num2 = 1f / (float)(segments - 1);
		for (int i = 0; i < segments; i++)
		{
			Color color = Color.Lerp(startColor, endColor, num);
			UnityEngine.Debug.DrawRay(startPos, vector, color, duration);
			startPos += vector;
			num += num2;
		}
	}

	// Token: 0x06009258 RID: 37464 RVA: 0x003A4C3C File Offset: 0x003A2E3C
	public static void DrawRay(Vector3 startPos, Vector3 dir, Color startColor, Color endColor, int segments, float duration = 0f)
	{
		Utils.DrawLine(startPos, startPos + dir, startColor, endColor, segments, duration);
	}

	// Token: 0x06009259 RID: 37465 RVA: 0x003A4C54 File Offset: 0x003A2E54
	public static void DrawOutline(Rect position, string text, GUIStyle style, Color outColor, Color inColor)
	{
		style.normal.textColor = outColor;
		float num = position.x;
		position.x = num - 1f;
		GUI.Label(position, text, style);
		position.x += 2f;
		GUI.Label(position, text, style);
		num = position.x;
		position.x = num - 1f;
		num = position.y;
		position.y = num - 1f;
		GUI.Label(position, text, style);
		position.y += 2f;
		GUI.Label(position, text, style);
		num = position.y;
		position.y = num - 1f;
		style.normal.textColor = inColor;
		GUI.Label(position, text, style);
	}

	// Token: 0x0600925A RID: 37466 RVA: 0x003A4D1C File Offset: 0x003A2F1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastAbsInt(int _x)
	{
		if (_x >= 0)
		{
			return _x;
		}
		if (_x == -2147483648)
		{
			return int.MaxValue;
		}
		return -_x;
	}

	// Token: 0x0600925B RID: 37467 RVA: 0x003A4D34 File Offset: 0x003A2F34
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastAbs(float _x)
	{
		if (_x < 0f)
		{
			return -_x;
		}
		return _x;
	}

	// Token: 0x0600925C RID: 37468 RVA: 0x003A4D42 File Offset: 0x003A2F42
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Fastfloor(float x)
	{
		if (x < 0f)
		{
			return (int)(x - 0.9999999f);
		}
		return (int)x;
	}

	// Token: 0x0600925D RID: 37469 RVA: 0x003A4D57 File Offset: 0x003A2F57
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Fastfloor(double x)
	{
		if (x < 0.0)
		{
			return (int)(x - 0.9999998807907104);
		}
		return (int)x;
	}

	// Token: 0x0600925E RID: 37470 RVA: 0x003A4D74 File Offset: 0x003A2F74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastMax(int v1, int v2)
	{
		if (v1 < v2)
		{
			return v2;
		}
		return v1;
	}

	// Token: 0x0600925F RID: 37471 RVA: 0x003A4D74 File Offset: 0x003A2F74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastMax(float v1, float v2)
	{
		if (v1 < v2)
		{
			return v2;
		}
		return v1;
	}

	// Token: 0x06009260 RID: 37472 RVA: 0x003A4D74 File Offset: 0x003A2F74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte FastMax(byte v1, byte v2)
	{
		if (v1 < v2)
		{
			return v2;
		}
		return v1;
	}

	// Token: 0x06009261 RID: 37473 RVA: 0x003A4D7D File Offset: 0x003A2F7D
	public static float FastMax(float v1, float v2, float v3, float v4)
	{
		if (v1 >= v2 && v1 >= v3 && v1 >= v4)
		{
			return v1;
		}
		if (v2 >= v1 && v2 >= v3 && v2 >= v4)
		{
			return v2;
		}
		if (v3 >= v1 && v3 >= v2 && v3 >= v4)
		{
			return v3;
		}
		return v4;
	}

	// Token: 0x06009262 RID: 37474 RVA: 0x003A4DAA File Offset: 0x003A2FAA
	public static int FastMax(int v1, int v2, int v3, int v4)
	{
		if (v1 >= v2 && v1 >= v3 && v1 >= v4)
		{
			return v1;
		}
		if (v2 >= v1 && v2 >= v3 && v2 >= v4)
		{
			return v2;
		}
		if (v3 >= v1 && v3 >= v2 && v3 >= v4)
		{
			return v3;
		}
		return v4;
	}

	// Token: 0x06009263 RID: 37475 RVA: 0x003A4DD7 File Offset: 0x003A2FD7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastMin(float v1, float v2)
	{
		if (v1 >= v2)
		{
			return v2;
		}
		return v1;
	}

	// Token: 0x06009264 RID: 37476 RVA: 0x003A4DD7 File Offset: 0x003A2FD7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastMin(int v1, int v2)
	{
		if (v1 >= v2)
		{
			return v2;
		}
		return v1;
	}

	// Token: 0x06009265 RID: 37477 RVA: 0x003A4DE0 File Offset: 0x003A2FE0
	public static int FastMin(int v1, int v2, int v3, int v4)
	{
		if (v1 <= v2 && v1 <= v3 && v1 <= v4)
		{
			return v1;
		}
		if (v2 <= v1 && v2 <= v3 && v2 <= v4)
		{
			return v2;
		}
		if (v3 <= v1 && v3 <= v2 && v3 <= v4)
		{
			return v3;
		}
		return v4;
	}

	// Token: 0x06009266 RID: 37478 RVA: 0x003A4E0D File Offset: 0x003A300D
	public static float FastMin(float v1, float v2, float v3, float v4)
	{
		if (v1 <= v2 && v1 <= v3 && v1 <= v4)
		{
			return v1;
		}
		if (v2 <= v1 && v2 <= v3 && v2 <= v4)
		{
			return v2;
		}
		if (v3 <= v1 && v3 <= v2 && v3 <= v4)
		{
			return v3;
		}
		return v4;
	}

	// Token: 0x06009267 RID: 37479 RVA: 0x003A4E3A File Offset: 0x003A303A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastClamp(float _v, float _min, float _max)
	{
		if (_v <= _min)
		{
			return _min;
		}
		if (_v >= _max)
		{
			return _max;
		}
		return _v;
	}

	// Token: 0x06009268 RID: 37480 RVA: 0x003A4E49 File Offset: 0x003A3049
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastClamp(int _v, int _min, int _max)
	{
		if (_v <= _min)
		{
			return _min;
		}
		if (_v >= _max)
		{
			return _max;
		}
		return _v;
	}

	// Token: 0x06009269 RID: 37481 RVA: 0x003A4E58 File Offset: 0x003A3058
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastClamp01(float _v)
	{
		if (_v <= 0f)
		{
			return 0f;
		}
		if (_v >= 1f)
		{
			return 1f;
		}
		return _v;
	}

	// Token: 0x0600926A RID: 37482 RVA: 0x003A4E77 File Offset: 0x003A3077
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double FastClamp01(double _v)
	{
		if (_v <= 0.0)
		{
			return 0.0;
		}
		if (_v >= 1.0)
		{
			return 1.0;
		}
		return _v;
	}

	// Token: 0x0600926B RID: 37483 RVA: 0x003A4EA6 File Offset: 0x003A30A6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastLerp(float a, float b, float t)
	{
		return a + (b - a) * Utils.FastClamp01(t);
	}

	// Token: 0x0600926C RID: 37484 RVA: 0x0038F45C File Offset: 0x0038D65C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastLerpUnclamped(float a, float b, float t)
	{
		return a + (b - a) * t;
	}

	// Token: 0x0600926D RID: 37485 RVA: 0x003A4EB4 File Offset: 0x003A30B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastMoveTowards(float current, float target, float maxDelta)
	{
		if (Utils.FastAbs(target - current) <= maxDelta)
		{
			return target;
		}
		return current + Mathf.Sign(target - current) * maxDelta;
	}

	// Token: 0x0600926E RID: 37486 RVA: 0x003A4ECF File Offset: 0x003A30CF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastRoundToInt(float f)
	{
		return (int)Math.Round((double)f);
	}

	// Token: 0x0600926F RID: 37487 RVA: 0x003A4ED9 File Offset: 0x003A30D9
	public static int FastRoundToIntAndMod(float _f, int _mod)
	{
		if (_mod == 0)
		{
			return 0;
		}
		if (_f <= 0f)
		{
			return (_mod + (int)(_f - 0.5f) % _mod) % _mod;
		}
		return (int)(_f + 0.5f) % _mod;
	}

	// Token: 0x06009270 RID: 37488 RVA: 0x003A4F04 File Offset: 0x003A3104
	public static Texture2D LoadRawStampFile(string _filepath)
	{
		Texture2D result;
		using (Stream stream = SdFile.OpenRead(_filepath))
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				int num = (int)Mathf.Sqrt((float)(binaryReader.BaseStream.Length / 6L));
				Texture2D texture2D = new Texture2D(num, num, TextureFormat.RGBAFloat, false, false);
				Color[] array = new Color[num * num];
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = (float)binaryReader.ReadUInt16() / 65535f;
					float a = (float)binaryReader.ReadUInt16() / 65535f;
					float b = (float)binaryReader.ReadUInt16() / 65535f;
					array[i] = new Color(num2, num2, b, a);
				}
				texture2D.SetPixels(array);
				texture2D.Apply();
				result = texture2D;
			}
		}
		return result;
	}

	// Token: 0x06009271 RID: 37489 RVA: 0x003A4FF0 File Offset: 0x003A31F0
	public static Color[] LoadRawStampFileArray(string _filepath)
	{
		Color[] result;
		using (Stream stream = SdFile.OpenRead(_filepath))
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				int num = (int)Mathf.Sqrt((float)(binaryReader.BaseStream.Length / 6L));
				Color[] array = new Color[num * num];
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = (float)binaryReader.ReadUInt16() / 65535f;
					float a = (float)binaryReader.ReadUInt16() / 65535f;
					float b = (float)binaryReader.ReadUInt16() / 65535f;
					array[i] = new Color(num2, num2, b, a);
				}
				result = array;
			}
		}
		return result;
	}

	// Token: 0x06009272 RID: 37490 RVA: 0x003A50C0 File Offset: 0x003A32C0
	public static void SaveRawStampFile(string _filepath, Texture2D _image)
	{
		using (Stream stream = SdFile.Open(_filepath, FileMode.CreateNew))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
				Color[] pixels = _image.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					binaryWriter.Write((ushort)(pixels[i].r * 65535f));
					binaryWriter.Write((ushort)(pixels[i].g * 65535f));
					binaryWriter.Write((ushort)(pixels[i].b * 65535f));
				}
			}
		}
	}

	// Token: 0x06009273 RID: 37491 RVA: 0x003A5180 File Offset: 0x003A3380
	public static void SaveRawStampFile(string _filepath, Color[] _image)
	{
		using (Stream stream = SdFile.Open(_filepath, FileMode.CreateNew))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
				for (int i = 0; i < _image.Length; i++)
				{
					binaryWriter.Write((ushort)(_image[i].r * 65535f));
					binaryWriter.Write((ushort)(_image[i].g * 65535f));
					binaryWriter.Write((ushort)(_image[i].b * 65535f));
				}
			}
		}
	}

	// Token: 0x06009274 RID: 37492 RVA: 0x003A5238 File Offset: 0x003A3438
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Repeat(float _value, float _length)
	{
		return Utils.FastClamp(_value - (float)Utils.Fastfloor(_value / _length) * _length, 0f, _length);
	}

	// Token: 0x06009275 RID: 37493 RVA: 0x003A5254 File Offset: 0x003A3454
	public static float DeltaAngle(float _angle1, float _angle2)
	{
		float num = Utils.Repeat(_angle2 - _angle1, 360f);
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}

	// Token: 0x06009276 RID: 37494 RVA: 0x003A5280 File Offset: 0x003A3480
	public static float GetAngleBetween(Vector3 _dir1, Vector3 _dir2)
	{
		float num = Mathf.Atan2(_dir1.z, _dir1.x) * 57.29578f;
		float num2 = Mathf.Atan2(_dir2.z, _dir2.x) * 57.29578f;
		float num3 = num - num2;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		if (num3 < -180f)
		{
			num3 += 360f;
		}
		return num3;
	}

	// Token: 0x06009277 RID: 37495 RVA: 0x003A52E0 File Offset: 0x003A34E0
	public static int Get4HitDirectionAsInt(Vector3 _direction, Vector3 _myLook)
	{
		float angleBetween = Utils.GetAngleBetween(_direction, _myLook);
		if (angleBetween > -45f && angleBetween <= 45f)
		{
			return 1;
		}
		if (angleBetween < -45f && angleBetween >= -135f)
		{
			return 3;
		}
		if (angleBetween > 45f && angleBetween <= 135f)
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06009278 RID: 37496 RVA: 0x003A532C File Offset: 0x003A352C
	public static int Get6HitDirectionAsInt(Vector3 _direction, Vector3 _myLook)
	{
		float angleBetween = Utils.GetAngleBetween(_direction, _myLook);
		if (angleBetween > -45f && angleBetween <= 45f)
		{
			return 1;
		}
		if (angleBetween < -45f && angleBetween >= -120f)
		{
			return 3;
		}
		if (angleBetween > 45f && angleBetween <= 120f)
		{
			return 2;
		}
		if (angleBetween > 120f && angleBetween < 160f)
		{
			return 4;
		}
		if (angleBetween < -120f && angleBetween > -160f)
		{
			return 5;
		}
		return 0;
	}

	// Token: 0x06009279 RID: 37497 RVA: 0x003A539C File Offset: 0x003A359C
	public static Utils.EnumHitDirection GetHitDirection4Sides(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		targetDir = targetDir.normalized;
		float num = Vector3.Dot(Vector3.Cross(fwd, targetDir), up);
		if (num > 0.75f)
		{
			return Utils.EnumHitDirection.Left;
		}
		if (num < -0.75f)
		{
			return Utils.EnumHitDirection.Right;
		}
		if ((double)Mathf.Abs(targetDir.y) > 0.05 && (double)Mathf.Abs(targetDir.x / targetDir.y) < 0.1 && (double)Mathf.Abs(targetDir.z / targetDir.y) < 0.1)
		{
			return Utils.EnumHitDirection.Back;
		}
		if (Mathf.Abs(Mathf.Atan2(targetDir.z, targetDir.x) - Mathf.Atan2(fwd.z, fwd.x)) > 1f)
		{
			return Utils.EnumHitDirection.Front;
		}
		return Utils.EnumHitDirection.Back;
	}

	// Token: 0x0600927A RID: 37498 RVA: 0x003A545C File Offset: 0x003A365C
	public static Quaternion BlockFaceToRotation(BlockFace _blockFace)
	{
		switch (_blockFace)
		{
		case BlockFace.Top:
			return Quaternion.identity;
		case BlockFace.Bottom:
			return Quaternion.AngleAxis(180f, Vector3.forward);
		case BlockFace.North:
			return Quaternion.AngleAxis(90f, Vector3.right);
		case BlockFace.West:
			return Quaternion.AngleAxis(90f, Vector3.forward);
		case BlockFace.South:
			return Quaternion.AngleAxis(-90f, Vector3.right);
		case BlockFace.East:
			return Quaternion.AngleAxis(-90f, Vector3.forward);
		default:
			return Quaternion.identity;
		}
	}

	// Token: 0x0600927B RID: 37499 RVA: 0x003A54E4 File Offset: 0x003A36E4
	public static Vector3 BlockFaceToVector(BlockFace _face)
	{
		switch (_face)
		{
		case BlockFace.Top:
			return new Vector3(0f, 1f, 0f);
		case BlockFace.Bottom:
			return new Vector3(0f, -1f, 0f);
		case BlockFace.North:
			return new Vector3(0f, 0f, 1f);
		case BlockFace.West:
			return new Vector3(-1f, 0f, 0f);
		case BlockFace.South:
			return new Vector3(0f, 0f, -1f);
		case BlockFace.East:
			return new Vector3(1f, 0f, 0f);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x0600927C RID: 37500 RVA: 0x003A5594 File Offset: 0x003A3794
	public static void MoveInBlockFaceDirection(Vector3[] _vertices, BlockFace _face, float d)
	{
		Vector3 zero = Vector3.zero;
		switch (_face)
		{
		case BlockFace.Top:
			zero = new Vector3(0f, d, 0f);
			break;
		case BlockFace.Bottom:
			zero = new Vector3(0f, -d, 0f);
			break;
		case BlockFace.North:
			zero = new Vector3(0f, 0f, d);
			break;
		case BlockFace.West:
			zero = new Vector3(-d, 0f, 0f);
			break;
		case BlockFace.South:
			zero = new Vector3(0f, 0f, -d);
			break;
		case BlockFace.East:
			zero = new Vector3(d, 0f, 0f);
			break;
		}
		for (int i = 0; i < _vertices.Length; i++)
		{
			_vertices[i] += zero;
		}
	}

	// Token: 0x0600927D RID: 37501 RVA: 0x003A5668 File Offset: 0x003A3868
	public static string EncryptOrDecrypt(string text, string key)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			stringBuilder.Append(text[i] ^ key[i % key.Length]);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600927E RID: 37502 RVA: 0x003A56B0 File Offset: 0x003A38B0
	public static string Crypt(string text)
	{
		return Utils.EncryptOrDecrypt(text, "X");
	}

	// Token: 0x0600927F RID: 37503 RVA: 0x003A56BD File Offset: 0x003A38BD
	public static string UnCryptFromBase64(char[] text)
	{
		return Utils.Crypt(Utils.FromBase64(text));
	}

	// Token: 0x06009280 RID: 37504 RVA: 0x003A56CA File Offset: 0x003A38CA
	public static string ToBase64(string _str)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(_str));
	}

	// Token: 0x06009281 RID: 37505 RVA: 0x003A56DC File Offset: 0x003A38DC
	public static string FromBase64(string _encodedStr)
	{
		string result;
		try
		{
			byte[] bytes = Convert.FromBase64String(_encodedStr);
			result = Encoding.UTF8.GetString(bytes);
		}
		catch (FormatException e)
		{
			Log.Error("FBase64 Exception");
			Log.Exception(e);
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x06009282 RID: 37506 RVA: 0x003A5728 File Offset: 0x003A3928
	public static string FromBase64(char[] _bytes)
	{
		string result;
		try
		{
			byte[] bytes = Convert.FromBase64CharArray(_bytes, 0, _bytes.Length);
			result = Encoding.UTF8.GetString(bytes);
		}
		catch (FormatException e)
		{
			Log.Error("FBase64A Exception");
			Log.Exception(e);
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x06009283 RID: 37507 RVA: 0x003A5778 File Offset: 0x003A3978
	public static string HashString(string password)
	{
		if (password.Length == 0)
		{
			return password;
		}
		return password.GetHashCode().ToString();
	}

	// Token: 0x06009284 RID: 37508 RVA: 0x003A579D File Offset: 0x003A399D
	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	// Token: 0x06009285 RID: 37509 RVA: 0x003A57D8 File Offset: 0x003A39D8
	public static Color Saturate(Color _c)
	{
		if (_c.r < 0f)
		{
			_c.r = 0f;
		}
		if (_c.r > 1f)
		{
			_c.r = 1f;
		}
		if (_c.g < 0f)
		{
			_c.g = 0f;
		}
		if (_c.g > 1f)
		{
			_c.g = 1f;
		}
		if (_c.b < 0f)
		{
			_c.b = 0f;
		}
		if (_c.b > 1f)
		{
			_c.b = 1f;
		}
		return _c;
	}

	// Token: 0x06009286 RID: 37510 RVA: 0x003A587C File Offset: 0x003A3A7C
	public static ushort ToColor5(Color col)
	{
		return (ushort)((int)(col.r * 31f + 0.5f) << 10 | (int)(col.g * 31f + 0.5f) << 5 | (int)(col.b * 31f + 0.5f));
	}

	// Token: 0x06009287 RID: 37511 RVA: 0x003A58CA File Offset: 0x003A3ACA
	public static Color FromColor5(ushort col)
	{
		return new Color((float)(col >> 10 & 31) / 31f, (float)(col >> 5 & 31) / 31f, (float)(col & 31) / 31f);
	}

	// Token: 0x06009288 RID: 37512 RVA: 0x003A58F8 File Offset: 0x003A3AF8
	public static Color32 FromColor5To32(ushort col)
	{
		return new Color32((byte)((float)(col >> 10 & 31) / 31f * 255f), (byte)((float)(col >> 5 & 31) / 31f * 255f), (byte)((float)(col & 31) / 31f * 255f), byte.MaxValue);
	}

	// Token: 0x06009289 RID: 37513 RVA: 0x003A594C File Offset: 0x003A3B4C
	public static GameRandom RandomFromSeedOnPos(int x, int y, int seed)
	{
		GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
		long num = (long)gameRandom.RandomInt / 2L * 2L + 1L;
		long num2 = (long)gameRandom.RandomInt / 2L * 2L + 1L;
		int seed2 = (int)((long)((long)x << 4) * num + (long)((long)y << 4) * num2 ^ (long)seed);
		gameRandom.SetSeed(seed2);
		return gameRandom;
	}

	// Token: 0x0600928A RID: 37514 RVA: 0x003A59A0 File Offset: 0x003A3BA0
	public static GameRandom RandomFromSeedOnPos(int _x, int _y, int _z, int _seed)
	{
		int seed = _seed + _x + (_z << 14) + (_y << 24);
		return GameRandomManager.Instance.CreateGameRandom(seed);
	}

	// Token: 0x0600928B RID: 37515 RVA: 0x003A59C8 File Offset: 0x003A3BC8
	public static string DescribeTimeSince(DateTime _newer, DateTime _older)
	{
		TimeSpan timeSpan = _newer - _older;
		if (timeSpan.TotalMinutes < 1.0)
		{
			return Localization.Get("xuiLastSeenNow", false);
		}
		if (timeSpan.TotalMinutes < 60.0)
		{
			int num = (int)timeSpan.TotalMinutes;
			if (num != 1)
			{
				return string.Format(Localization.Get("xuiLastSeenMinutes", false), num);
			}
			return Localization.Get("xuiLastSeen1Minute", false);
		}
		else if (timeSpan.TotalHours < 24.0)
		{
			int num2 = (int)timeSpan.TotalHours;
			if (num2 != 1)
			{
				return string.Format(Localization.Get("xuiLastSeenHours", false), num2);
			}
			return Localization.Get("xuiLastSeen1Hour", false);
		}
		else
		{
			int num3 = (int)timeSpan.TotalDays;
			if (num3 != 1)
			{
				return string.Format(Localization.Get("xuiLastSeenDays", false), num3);
			}
			return Localization.Get("xuiLastSeen1Day", false);
		}
	}

	// Token: 0x0600928C RID: 37516 RVA: 0x003A5AAE File Offset: 0x003A3CAE
	public static float ToCelsius(float fromFahrenheit)
	{
		return (fromFahrenheit - 32f) * 0.5555556f;
	}

	// Token: 0x0600928D RID: 37517 RVA: 0x003A5ABD File Offset: 0x003A3CBD
	public static float ToRelativeCelsius(float fromFahrenheit)
	{
		return fromFahrenheit * 0.5555556f;
	}

	// Token: 0x0600928E RID: 37518 RVA: 0x003A5AC8 File Offset: 0x003A3CC8
	public static string EscapeBbCodes(string _input, bool _removeInstead = false, bool _allowUrls = false)
	{
		if (string.IsNullOrEmpty(_input))
		{
			return _input;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		int num = 0;
		while (i < _input.Length)
		{
			ValueTuple<int, int, bool> valueTuple = Utils.FindNextBbCode(_input, i, _allowUrls);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			if (item == -1)
			{
				stringBuilder.Append(_input.Substring(num));
				break;
			}
			stringBuilder.Append(_input.Substring(num, item - num));
			if (!_removeInstead)
			{
				int num2 = 0;
				while (_input[item + num2] == '[')
				{
					num2++;
					stringBuilder.Append("[");
				}
				stringBuilder.Append("[/c]");
				stringBuilder.Append(_input.Substring(item + num2, item2 - num2));
			}
			i = item + item2;
			num = i;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600928F RID: 37519 RVA: 0x003A5B8C File Offset: 0x003A3D8C
	public static ValueTuple<int, int, bool> FindNextBbCode(string _input, int _startIndex, bool _allowUrls = false)
	{
		int i = _startIndex;
		int length = _input.Length;
		while (i < length)
		{
			if (_input[i] != '[')
			{
				i++;
			}
			else
			{
				Match match = Utils.nestedEscapePattern.Match(_input.Substring(i, length - i));
				if (match.Success)
				{
					int index = match.Index;
					if (index == 0)
					{
						return new ValueTuple<int, int, bool>(i, match.Length, true);
					}
					if (index == 1)
					{
						int num = _input.IndexOf(']', i + match.Length);
						if (num == i + match.Length)
						{
							return new ValueTuple<int, int, bool>(i, num + 1 - i, true);
						}
						if (num == -1)
						{
							i += 4;
							continue;
						}
						string text = "[" + _input.Substring(i + match.Length, num - (i + match.Length)) + "]";
						ValueTuple<int, int, bool> valueTuple = Utils.FindNextBbCode(text, 0, false);
						int item = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						if (item == 0 && item2 == text.Length)
						{
							return new ValueTuple<int, int, bool>(i, num + 1 - i, true);
						}
					}
				}
				if (i + 3 > length || _input[i] != '[')
				{
					i++;
				}
				else
				{
					if (_input[i + 2] == ']')
					{
						char c = _input[i + 1];
						if (c <= 'I')
						{
							if (c <= 'B')
							{
								if (c != '-' && c != 'B')
								{
									goto IL_1A8;
								}
							}
							else if (c != 'C' && c != 'I')
							{
								goto IL_1A8;
							}
						}
						else if (c <= 'b')
						{
							switch (c)
							{
							case 'S':
							case 'T':
							case 'U':
								break;
							default:
								if (c != 'b')
								{
									goto IL_1A8;
								}
								break;
							}
						}
						else if (c != 'c' && c != 'i')
						{
							switch (c)
							{
							case 's':
							case 't':
							case 'u':
								break;
							default:
								goto IL_1A8;
							}
						}
						return new ValueTuple<int, int, bool>(i, 3, false);
					}
					IL_1A8:
					if (i + 4 <= length && _input[i + 1] == '/' && _input[i + 3] == ']')
					{
						char c = _input[i + 2];
						if (c <= 'I')
						{
							if (c <= 'B')
							{
								if (c != '-' && c != 'B')
								{
									goto IL_255;
								}
							}
							else if (c != 'C' && c != 'I')
							{
								goto IL_255;
							}
						}
						else if (c <= 'b')
						{
							switch (c)
							{
							case 'S':
							case 'T':
							case 'U':
								break;
							default:
								if (c != 'b')
								{
									goto IL_255;
								}
								break;
							}
						}
						else if (c != 'c' && c != 'i')
						{
							switch (c)
							{
							case 's':
							case 't':
							case 'u':
								break;
							default:
								goto IL_255;
							}
						}
						return new ValueTuple<int, int, bool>(i, 4, false);
					}
					IL_255:
					if (i + 4 <= length && _input[i + 3] == ']' && Utils.hexAlphaMatcher.IsMatch(_input.Substring(i + 1, 2)))
					{
						return new ValueTuple<int, int, bool>(i, 4, false);
					}
					if (i + 5 <= length && _input[i + 4] == ']' && _input[i + 1] == 's' && _input[i + 2] == 'u' && (_input[i + 3] == 'b' || _input[i + 3] == 'p'))
					{
						return new ValueTuple<int, int, bool>(i, 5, false);
					}
					if (i + 6 <= length && _input[i + 5] == ']')
					{
						string a = _input.Substring(i, 6);
						if (a.EqualsCaseInsensitive("[/sub]") || a.EqualsCaseInsensitive("[/sup]"))
						{
							return new ValueTuple<int, int, bool>(i, 6, false);
						}
					}
					if (i + 5 < length && _input.Substring(i, 5).EqualsCaseInsensitive("[url="))
					{
						int num2 = _input.IndexOf(']', i + 5);
						if (num2 != -1)
						{
							if (_allowUrls)
							{
								i = num2;
								continue;
							}
							return new ValueTuple<int, int, bool>(i, num2 - i + 1, false);
						}
					}
					if (!_allowUrls && i + 6 <= length && _input.Substring(i, 6).EqualsCaseInsensitive("[/url]"))
					{
						return new ValueTuple<int, int, bool>(i, 6, false);
					}
					if (i + 8 <= length && _input[i + 7] == ']' && Utils.hexRgbMatcher.IsMatch(_input.Substring(i + 1, 6)))
					{
						return new ValueTuple<int, int, bool>(i, 8, false);
					}
					if (i + 10 <= length && _input[i + 9] == ']' && Utils.hexRgbaMatcher.IsMatch(_input.Substring(i + 1, 8)))
					{
						return new ValueTuple<int, int, bool>(i, 10, false);
					}
					i++;
				}
			}
		}
		return new ValueTuple<int, int, bool>(-1, 0, false);
	}

	// Token: 0x06009290 RID: 37520 RVA: 0x003A5F98 File Offset: 0x003A4198
	public static string GetVisibileTextWithBbCodes(string _input)
	{
		int i = 0;
		StringBuilder stringBuilder = new StringBuilder();
		while (i < _input.Length)
		{
			ValueTuple<int, int, bool> valueTuple = Utils.FindNextBbCode(_input, i, false);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			bool item3 = valueTuple.Item3;
			if (item == -1)
			{
				stringBuilder.Append(_input, i, _input.Length - i);
				break;
			}
			stringBuilder.Append(_input, i, item - i);
			if (item3)
			{
				string value = Utils.nestedEscapePattern.Match(_input.Substring(i)).Value;
				value.Replace("[/c]", "");
				stringBuilder.Append(value);
				stringBuilder.Append(_input, value.Length, item2 - value.Length);
			}
			i = item + item2;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06009291 RID: 37521 RVA: 0x003A6057 File Offset: 0x003A4257
	public static string CreateGameMessage(string _senderName, string _message)
	{
		if (string.IsNullOrEmpty(_senderName))
		{
			return _message;
		}
		return _senderName + ": " + _message;
	}

	// Token: 0x06009292 RID: 37522 RVA: 0x003A6070 File Offset: 0x003A4270
	public static string GetCancellationMessage()
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			return Localization.Get("msgConnectingToServerCancel", false);
		}
		return string.Format(Localization.Get("msgConnectingToServerCancelTemplate", false), LocalPlayerUI.primaryUI.playerInput.PermanentActions.Cancel.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
	}

	// Token: 0x06009293 RID: 37523 RVA: 0x003A60D8 File Offset: 0x003A42D8
	public static void Fill<T>(this T[] array, T value)
	{
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = value;
		}
	}

	// Token: 0x06009294 RID: 37524 RVA: 0x003A6100 File Offset: 0x003A4300
	public static void Memset(byte[] array, byte what, int length)
	{
		GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		Utils.MemsetDelegate(gchandle.AddrOfPinnedObject(), what, length);
		gchandle.Free();
	}

	// Token: 0x06009295 RID: 37525 RVA: 0x003A6130 File Offset: 0x003A4330
	public static void CalculateMeshTangents(List<Vector3> _vertices, List<int> _indices, List<Vector3> _normals, List<Vector2> _uvs, List<Vector4> _tangents, Mesh mesh, bool _bIgnoreUvs = false)
	{
		int count = _vertices.Count;
		if (count > 786432)
		{
			return;
		}
		Array.Clear(Utils.tan1Temp, 0, count);
		Array.Clear(Utils.tan2Temp, 0, count);
		int count2 = _indices.Count;
		for (int i = 0; i < count2; i += 3)
		{
			int num = _indices[i];
			int num2 = _indices[i + 1];
			int num3 = _indices[i + 2];
			Vector3 vector = _vertices[num];
			Vector3 vector2 = _vertices[num2];
			Vector3 vector3 = _vertices[num3];
			Vector2 vector4;
			Vector2 vector5;
			Vector2 vector6;
			if (_bIgnoreUvs)
			{
				vector4 = Vector2.zero;
				vector5 = Vector2.zero;
				vector6 = Vector2.zero;
			}
			else
			{
				vector4 = _uvs[num];
				vector5 = _uvs[num2];
				vector6 = _uvs[num3];
			}
			float num4 = vector2.x - vector.x;
			float num5 = vector3.x - vector.x;
			float num6 = vector2.y - vector.y;
			float num7 = vector3.y - vector.y;
			float num8 = vector2.z - vector.z;
			float num9 = vector3.z - vector.z;
			float num10 = vector5.x - vector4.x;
			float num11 = vector6.x - vector4.x;
			float num12 = vector5.y - vector4.y;
			float num13 = vector6.y - vector4.y;
			float num14 = num10 * num13 - num11 * num12;
			float num15 = (num14 == 0f) ? 0f : (1f / num14);
			float num16 = (num13 * num4 - num12 * num5) * num15;
			float num17 = (num13 * num6 - num12 * num7) * num15;
			float num18 = (num13 * num8 - num12 * num9) * num15;
			float num19 = (num10 * num5 - num11 * num4) * num15;
			float num20 = (num10 * num7 - num11 * num6) * num15;
			float num21 = (num10 * num9 - num11 * num8) * num15;
			Vector3[] array = Utils.tan1Temp;
			int num22 = num;
			array[num22].x = array[num22].x + num16;
			Vector3[] array2 = Utils.tan1Temp;
			int num23 = num;
			array2[num23].y = array2[num23].y + num17;
			Vector3[] array3 = Utils.tan1Temp;
			int num24 = num;
			array3[num24].z = array3[num24].z + num18;
			Vector3[] array4 = Utils.tan1Temp;
			int num25 = num2;
			array4[num25].x = array4[num25].x + num16;
			Vector3[] array5 = Utils.tan1Temp;
			int num26 = num2;
			array5[num26].y = array5[num26].y + num17;
			Vector3[] array6 = Utils.tan1Temp;
			int num27 = num2;
			array6[num27].z = array6[num27].z + num18;
			Vector3[] array7 = Utils.tan1Temp;
			int num28 = num3;
			array7[num28].x = array7[num28].x + num16;
			Vector3[] array8 = Utils.tan1Temp;
			int num29 = num3;
			array8[num29].y = array8[num29].y + num17;
			Vector3[] array9 = Utils.tan1Temp;
			int num30 = num3;
			array9[num30].z = array9[num30].z + num18;
			Vector3[] array10 = Utils.tan2Temp;
			int num31 = num;
			array10[num31].x = array10[num31].x + num19;
			Vector3[] array11 = Utils.tan2Temp;
			int num32 = num;
			array11[num32].y = array11[num32].y + num20;
			Vector3[] array12 = Utils.tan2Temp;
			int num33 = num;
			array12[num33].z = array12[num33].z + num21;
			Vector3[] array13 = Utils.tan2Temp;
			int num34 = num2;
			array13[num34].x = array13[num34].x + num19;
			Vector3[] array14 = Utils.tan2Temp;
			int num35 = num2;
			array14[num35].y = array14[num35].y + num20;
			Vector3[] array15 = Utils.tan2Temp;
			int num36 = num2;
			array15[num36].z = array15[num36].z + num21;
			Vector3[] array16 = Utils.tan2Temp;
			int num37 = num3;
			array16[num37].x = array16[num37].x + num19;
			Vector3[] array17 = Utils.tan2Temp;
			int num38 = num3;
			array17[num38].y = array17[num38].y + num20;
			Vector3[] array18 = Utils.tan2Temp;
			int num39 = num3;
			array18[num39].z = array18[num39].z + num21;
		}
		if (_normals.Count == 0)
		{
			Utils.tempNormals.Clear();
			mesh.GetNormals(Utils.tempNormals);
		}
		for (int j = 0; j < count; j++)
		{
			Vector3 lhs = (_normals.Count > 0) ? _normals[j] : Utils.tempNormals[j];
			Vector3 vector7 = Utils.tan1Temp[j];
			Vector3.OrthoNormalize(ref lhs, ref vector7);
			Vector4 item = new Vector4(vector7.x, vector7.y, vector7.z, (Vector3.Dot(Vector3.Cross(lhs, vector7), Utils.tan2Temp[j]) < 0f) ? -1f : 1f);
			_tangents.Add(item);
		}
	}

	// Token: 0x06009296 RID: 37526 RVA: 0x003A6574 File Offset: 0x003A4774
	public static void CalculateMeshTangents(ArrayListMP<Vector3> _vertices, ArrayListMP<int> _indices, ArrayListMP<Vector3> _normals, ArrayListMP<Vector2> _uvs, ArrayListMP<Vector4> _tangents, Mesh _mesh, bool _bIgnoreUvs = false)
	{
		int count = _vertices.Count;
		if (count > 786432)
		{
			return;
		}
		Array.Clear(Utils.tan1Temp, 0, count);
		Array.Clear(Utils.tan2Temp, 0, count);
		int count2 = _indices.Count;
		for (int i = 0; i < count2; i += 3)
		{
			int num = _indices[i];
			int num2 = _indices[i + 1];
			int num3 = _indices[i + 2];
			Vector3 vector = _vertices[num];
			Vector3 vector2 = _vertices[num2];
			Vector3 vector3 = _vertices[num3];
			Vector2 vector4;
			Vector2 vector5;
			Vector2 vector6;
			if (_bIgnoreUvs)
			{
				vector4 = Vector2.zero;
				vector5 = Vector2.zero;
				vector6 = Vector2.zero;
			}
			else
			{
				vector4 = _uvs[num];
				vector5 = _uvs[num2];
				vector6 = _uvs[num3];
			}
			float num4 = vector2.x - vector.x;
			float num5 = vector3.x - vector.x;
			float num6 = vector2.y - vector.y;
			float num7 = vector3.y - vector.y;
			float num8 = vector2.z - vector.z;
			float num9 = vector3.z - vector.z;
			float num10 = vector5.x - vector4.x;
			float num11 = vector6.x - vector4.x;
			float num12 = vector5.y - vector4.y;
			float num13 = vector6.y - vector4.y;
			float num14 = num10 * num13 - num11 * num12;
			float num15 = (num14 == 0f) ? 0f : (1f / num14);
			float num16 = (num13 * num4 - num12 * num5) * num15;
			float num17 = (num13 * num6 - num12 * num7) * num15;
			float num18 = (num13 * num8 - num12 * num9) * num15;
			float num19 = (num10 * num5 - num11 * num4) * num15;
			float num20 = (num10 * num7 - num11 * num6) * num15;
			float num21 = (num10 * num9 - num11 * num8) * num15;
			Vector3[] array = Utils.tan1Temp;
			int num22 = num;
			array[num22].x = array[num22].x + num16;
			Vector3[] array2 = Utils.tan1Temp;
			int num23 = num;
			array2[num23].y = array2[num23].y + num17;
			Vector3[] array3 = Utils.tan1Temp;
			int num24 = num;
			array3[num24].z = array3[num24].z + num18;
			Vector3[] array4 = Utils.tan1Temp;
			int num25 = num2;
			array4[num25].x = array4[num25].x + num16;
			Vector3[] array5 = Utils.tan1Temp;
			int num26 = num2;
			array5[num26].y = array5[num26].y + num17;
			Vector3[] array6 = Utils.tan1Temp;
			int num27 = num2;
			array6[num27].z = array6[num27].z + num18;
			Vector3[] array7 = Utils.tan1Temp;
			int num28 = num3;
			array7[num28].x = array7[num28].x + num16;
			Vector3[] array8 = Utils.tan1Temp;
			int num29 = num3;
			array8[num29].y = array8[num29].y + num17;
			Vector3[] array9 = Utils.tan1Temp;
			int num30 = num3;
			array9[num30].z = array9[num30].z + num18;
			Vector3[] array10 = Utils.tan2Temp;
			int num31 = num;
			array10[num31].x = array10[num31].x + num19;
			Vector3[] array11 = Utils.tan2Temp;
			int num32 = num;
			array11[num32].y = array11[num32].y + num20;
			Vector3[] array12 = Utils.tan2Temp;
			int num33 = num;
			array12[num33].z = array12[num33].z + num21;
			Vector3[] array13 = Utils.tan2Temp;
			int num34 = num2;
			array13[num34].x = array13[num34].x + num19;
			Vector3[] array14 = Utils.tan2Temp;
			int num35 = num2;
			array14[num35].y = array14[num35].y + num20;
			Vector3[] array15 = Utils.tan2Temp;
			int num36 = num2;
			array15[num36].z = array15[num36].z + num21;
			Vector3[] array16 = Utils.tan2Temp;
			int num37 = num3;
			array16[num37].x = array16[num37].x + num19;
			Vector3[] array17 = Utils.tan2Temp;
			int num38 = num3;
			array17[num38].y = array17[num38].y + num20;
			Vector3[] array18 = Utils.tan2Temp;
			int num39 = num3;
			array18[num39].z = array18[num39].z + num21;
		}
		if (_normals.Count == 0)
		{
			Utils.tempNormals.Clear();
			_mesh.GetNormals(Utils.tempNormals);
		}
		for (int j = 0; j < count; j++)
		{
			Vector3 lhs = (_normals.Count > 0) ? _normals[j] : Utils.tempNormals[j];
			Vector3 vector7 = Utils.tan1Temp[j];
			Vector3.OrthoNormalize(ref lhs, ref vector7);
			Vector4 item = new Vector4(vector7.x, vector7.y, vector7.z, (Vector3.Dot(Vector3.Cross(lhs, vector7), Utils.tan2Temp[j]) < 0f) ? -1f : 1f);
			_tangents.Add(item);
		}
	}

	// Token: 0x06009297 RID: 37527 RVA: 0x003A69B8 File Offset: 0x003A4BB8
	public static void CalculateMeshTangents(VoxelMesh _vm, bool _bIgnoreUvs = false)
	{
		int count = _vm.m_Vertices.Count;
		if (count > 786432)
		{
			return;
		}
		Array.Clear(Utils.tan1Temp, 0, count);
		Array.Clear(Utils.tan2Temp, 0, count);
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		Vector2 vector3 = Vector2.zero;
		int count2 = _vm.m_Indices.Count;
		for (int i = 0; i < count2; i += 3)
		{
			int num = _vm.m_Indices[i];
			int num2 = _vm.m_Indices[i + 1];
			int num3 = _vm.m_Indices[i + 2];
			Vector3 vector4 = _vm.m_Vertices[num];
			Vector3 vector5 = _vm.m_Vertices[num2];
			Vector3 vector6 = _vm.m_Vertices[num3];
			if (!_bIgnoreUvs)
			{
				vector = _vm.m_Uvs[num];
				vector2 = _vm.m_Uvs[num2];
				vector3 = _vm.m_Uvs[num3];
			}
			float num4 = vector5.x - vector4.x;
			float num5 = vector6.x - vector4.x;
			float num6 = vector5.y - vector4.y;
			float num7 = vector6.y - vector4.y;
			float num8 = vector5.z - vector4.z;
			float num9 = vector6.z - vector4.z;
			float num10 = vector2.x - vector.x;
			float num11 = vector3.x - vector.x;
			float num12 = vector2.y - vector.y;
			float num13 = vector3.y - vector.y;
			float num14 = num10 * num13 - num11 * num12;
			float num15 = (num14 == 0f) ? 0f : (1f / num14);
			float num16 = (num13 * num4 - num12 * num5) * num15;
			float num17 = (num13 * num6 - num12 * num7) * num15;
			float num18 = (num13 * num8 - num12 * num9) * num15;
			float num19 = (num10 * num5 - num11 * num4) * num15;
			float num20 = (num10 * num7 - num11 * num6) * num15;
			float num21 = (num10 * num9 - num11 * num8) * num15;
			Vector3[] array = Utils.tan1Temp;
			int num22 = num;
			array[num22].x = array[num22].x + num16;
			Vector3[] array2 = Utils.tan1Temp;
			int num23 = num;
			array2[num23].y = array2[num23].y + num17;
			Vector3[] array3 = Utils.tan1Temp;
			int num24 = num;
			array3[num24].z = array3[num24].z + num18;
			Vector3[] array4 = Utils.tan1Temp;
			int num25 = num2;
			array4[num25].x = array4[num25].x + num16;
			Vector3[] array5 = Utils.tan1Temp;
			int num26 = num2;
			array5[num26].y = array5[num26].y + num17;
			Vector3[] array6 = Utils.tan1Temp;
			int num27 = num2;
			array6[num27].z = array6[num27].z + num18;
			Vector3[] array7 = Utils.tan1Temp;
			int num28 = num3;
			array7[num28].x = array7[num28].x + num16;
			Vector3[] array8 = Utils.tan1Temp;
			int num29 = num3;
			array8[num29].y = array8[num29].y + num17;
			Vector3[] array9 = Utils.tan1Temp;
			int num30 = num3;
			array9[num30].z = array9[num30].z + num18;
			Vector3[] array10 = Utils.tan2Temp;
			int num31 = num;
			array10[num31].x = array10[num31].x + num19;
			Vector3[] array11 = Utils.tan2Temp;
			int num32 = num;
			array11[num32].y = array11[num32].y + num20;
			Vector3[] array12 = Utils.tan2Temp;
			int num33 = num;
			array12[num33].z = array12[num33].z + num21;
			Vector3[] array13 = Utils.tan2Temp;
			int num34 = num2;
			array13[num34].x = array13[num34].x + num19;
			Vector3[] array14 = Utils.tan2Temp;
			int num35 = num2;
			array14[num35].y = array14[num35].y + num20;
			Vector3[] array15 = Utils.tan2Temp;
			int num36 = num2;
			array15[num36].z = array15[num36].z + num21;
			Vector3[] array16 = Utils.tan2Temp;
			int num37 = num3;
			array16[num37].x = array16[num37].x + num19;
			Vector3[] array17 = Utils.tan2Temp;
			int num38 = num3;
			array17[num38].y = array17[num38].y + num20;
			Vector3[] array18 = Utils.tan2Temp;
			int num39 = num3;
			array18[num39].z = array18[num39].z + num21;
		}
		Vector3[] items = _vm.m_Normals.Items;
		int num40 = _vm.m_Tangents.Alloc(count);
		Vector4[] items2 = _vm.m_Tangents.Items;
		for (int j = 0; j < count; j++)
		{
			Vector3 lhs = items[j];
			Vector3 vector7 = Utils.tan1Temp[j];
			Vector3.OrthoNormalize(ref lhs, ref vector7);
			Vector4 vector8;
			vector8.x = vector7.x;
			vector8.y = vector7.y;
			vector8.z = vector7.z;
			vector8.w = (float)((Vector3.Dot(Vector3.Cross(lhs, vector7), Utils.tan2Temp[j]) < 0f) ? -1 : 1);
			items2[num40++] = vector8;
		}
	}

	// Token: 0x06009298 RID: 37528 RVA: 0x003A6E3C File Offset: 0x003A503C
	public unsafe static void CalculateMeshTangents(ReadOnlySpan<Vector3> _vertices, ReadOnlySpan<int> _indices, ReadOnlySpan<Vector3> _normals, ReadOnlySpan<Vector2> _uvs, Span<Vector4> _tangents, bool _bIgnoreUvs = false)
	{
		int length = _vertices.Length;
		if (length > 786432)
		{
			return;
		}
		NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		try
		{
			NativeArray<Vector3> nativeArray2 = new NativeArray<Vector3>(length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			try
			{
				Span<Vector3> span = nativeArray;
				Span<Vector3> span2 = nativeArray2;
				int length2 = _indices.Length;
				for (int i = 0; i < length2; i += 3)
				{
					int index = *_indices[i];
					int index2 = *_indices[i + 1];
					int index3 = *_indices[i + 2];
					Vector3 vector = *_vertices[index];
					Vector3 vector2 = *_vertices[index2];
					Vector3 vector3 = *_vertices[index3];
					Vector2 vector4;
					Vector2 vector5;
					Vector2 vector6;
					if (_bIgnoreUvs)
					{
						vector4 = Vector2.zero;
						vector5 = Vector2.zero;
						vector6 = Vector2.zero;
					}
					else
					{
						vector4 = *_uvs[index];
						vector5 = *_uvs[index2];
						vector6 = *_uvs[index3];
					}
					float num = vector2.x - vector.x;
					float num2 = vector3.x - vector.x;
					float num3 = vector2.y - vector.y;
					float num4 = vector3.y - vector.y;
					float num5 = vector2.z - vector.z;
					float num6 = vector3.z - vector.z;
					float num7 = vector5.x - vector4.x;
					float num8 = vector6.x - vector4.x;
					float num9 = vector5.y - vector4.y;
					float num10 = vector6.y - vector4.y;
					float num11 = num7 * num10 - num8 * num9;
					float num12 = (num11 == 0f) ? 0f : (1f / num11);
					float num13 = (num10 * num - num9 * num2) * num12;
					float num14 = (num10 * num3 - num9 * num4) * num12;
					float num15 = (num10 * num5 - num9 * num6) * num12;
					float num16 = (num7 * num2 - num8 * num) * num12;
					float num17 = (num7 * num4 - num8 * num3) * num12;
					float num18 = (num7 * num6 - num8 * num5) * num12;
					span[index].x += num13;
					span[index].y += num14;
					span[index].z += num15;
					span[index2].x += num13;
					span[index2].y += num14;
					span[index2].z += num15;
					span[index3].x += num13;
					span[index3].y += num14;
					span[index3].z += num15;
					span2[index].x += num16;
					span2[index].y += num17;
					span2[index].z += num18;
					span2[index2].x += num16;
					span2[index2].y += num17;
					span2[index2].z += num18;
					span2[index3].x += num16;
					span2[index3].y += num17;
					span2[index3].z += num18;
				}
				for (int j = 0; j < length; j++)
				{
					Vector3 lhs = *_normals[j];
					Vector3 vector7 = *span[j];
					Vector3.OrthoNormalize(ref lhs, ref vector7);
					Vector4 vector8 = new Vector4(vector7.x, vector7.y, vector7.z, (Vector3.Dot(Vector3.Cross(lhs, vector7), *span2[j]) < 0f) ? -1f : 1f);
					*_tangents[j] = vector8;
				}
			}
			finally
			{
				((IDisposable)nativeArray2).Dispose();
			}
		}
		finally
		{
			((IDisposable)nativeArray).Dispose();
		}
	}

	// Token: 0x06009299 RID: 37529 RVA: 0x003A72C0 File Offset: 0x003A54C0
	public static int WrapInt(int value, int min, int max)
	{
		int num = max - min;
		while (value < min)
		{
			value += num;
		}
		while (value > max)
		{
			value -= num;
		}
		return value;
	}

	// Token: 0x0600929A RID: 37530 RVA: 0x003A72E8 File Offset: 0x003A54E8
	public static int WrapIndex(int index, int arraySize)
	{
		while (index < 0)
		{
			index += arraySize;
		}
		while (index >= arraySize)
		{
			index -= arraySize;
		}
		return index;
	}

	// Token: 0x0600929B RID: 37531 RVA: 0x003A7304 File Offset: 0x003A5504
	public static float WrapFloat(float value, float min, float max)
	{
		float num = max - min;
		if (value < min)
		{
			value += num;
		}
		else if (value > max)
		{
			value -= num;
		}
		return value;
	}

	// Token: 0x0600929C RID: 37532 RVA: 0x003A732C File Offset: 0x003A552C
	public static void CleanupMaterialsOfRenderers<T>(T renderers) where T : IList<Renderer>
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].GetSharedMaterials(Utils.tempMats);
			Utils.CleanupMaterials<List<Material>>(Utils.tempMats);
		}
		Utils.tempMats.Clear();
	}

	// Token: 0x0600929D RID: 37533 RVA: 0x003A737D File Offset: 0x003A557D
	public static void CleanupMaterialsOfRenderer(Renderer renderer)
	{
		Utils.CleanupMaterials<Material[]>(renderer.sharedMaterials);
	}

	// Token: 0x0600929E RID: 37534 RVA: 0x003A738C File Offset: 0x003A558C
	public static void CleanupMaterials<T>(T mats) where T : IList<Material>
	{
		for (int i = 0; i < mats.Count; i++)
		{
			Material material = mats[i];
			if (material && material.GetInstanceID() < 0)
			{
				UnityEngine.Object.Destroy(material);
			}
		}
	}

	// Token: 0x0600929F RID: 37535 RVA: 0x003A73D7 File Offset: 0x003A55D7
	public static void MarkMaterialAsSafeForManualCleanup(Material mat)
	{
		mat.name += " (Instance)";
	}

	// Token: 0x060092A0 RID: 37536 RVA: 0x003A73F0 File Offset: 0x003A55F0
	public static void RestartGame(Utils.ERestartAntiCheatMode _antiCheatMode = Utils.ERestartAntiCheatMode.KeepAntiCheatMode)
	{
		Regex regex = new Regex("^(.*)(output_log_client__)(\\d{4}-\\d{2}-\\d{2}__\\d{2}-\\d{2}-\\d{2})(\\.txt)$");
		Regex regex2 = new Regex("^(.*)(output_log__)(\\d{4}-\\d{2}-\\d{2}__\\d{2}-\\d{2}-\\d{2})(\\.txt)$", RegexOptions.IgnoreCase);
		string[] commandLineArgs = GameStartupHelper.GetCommandLineArgs();
		string text = commandLineArgs[0];
		bool flag;
		if (_antiCheatMode != Utils.ERestartAntiCheatMode.ForceOn)
		{
			if (_antiCheatMode == Utils.ERestartAntiCheatMode.KeepAntiCheatMode)
			{
				IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
				flag = (antiCheatClient != null && antiCheatClient.ClientAntiCheatEnabled());
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		if (flag2)
		{
			RuntimePlatform platform = Application.platform;
			if (platform > RuntimePlatform.WindowsPlayer)
			{
				if (platform != RuntimePlatform.LinuxPlayer)
				{
					switch (platform)
					{
					case RuntimePlatform.LinuxServer:
						break;
					case RuntimePlatform.WindowsServer:
						goto IL_8C;
					case RuntimePlatform.OSXServer:
						goto IL_EA;
					default:
						goto IL_119;
					}
				}
				text = Path.GetDirectoryName(text);
				text = ((text.Length > 0) ? (text + "/") : "") + "7DaysToDie_EAC";
				goto IL_133;
			}
			if (platform == RuntimePlatform.OSXPlayer)
			{
				goto IL_EA;
			}
			if (platform != RuntimePlatform.WindowsPlayer)
			{
				goto IL_119;
			}
			IL_8C:
			text = Path.GetDirectoryName(text);
			text = ((text.Length > 0) ? (text + "/") : "") + "7DaysToDie_EAC.exe";
			goto IL_133;
			IL_EA:
			text = Path.GetDirectoryName(text);
			text = ((text.Length > 0) ? (text + "/") : "") + "7DaysToDie_EAC";
			goto IL_133;
			IL_119:
			Log.Error(string.Format("Restarting the game not supported on this platform ({0})", Application.platform));
			return;
		}
		IL_133:
		StringBuilder stringBuilder = new StringBuilder();
		if (!flag2)
		{
			stringBuilder.Append("-noeac");
		}
		for (int i = 1; i < commandLineArgs.Length; i++)
		{
			if (!flag2 || i > 1)
			{
				stringBuilder.Append(' ');
			}
			string text2 = commandLineArgs[i];
			if (text2.EqualsCaseInsensitive("-logfile"))
			{
				stringBuilder.Append(text2);
				stringBuilder.Append(' ');
				i++;
				text2 = commandLineArgs[i];
				Match match;
				if ((match = regex.Match(text2)).Success)
				{
					string str = DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss");
					text2 = match.Groups[1].Value + match.Groups[2].Value + str + match.Groups[4].Value;
				}
				else if ((match = regex2.Match(text2)).Success)
				{
					string str2 = DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss");
					text2 = match.Groups[1].Value + match.Groups[2].Value + str2 + match.Groups[4].Value;
				}
				else
				{
					text2 += ".restarted.txt";
				}
			}
			if (text2.IndexOf(' ') >= 0)
			{
				stringBuilder.Append('"');
				stringBuilder.Append(text2);
				stringBuilder.Append('"');
			}
			else
			{
				stringBuilder.Append(text2);
			}
		}
		stringBuilder.Append(" -skipintro");
		stringBuilder.Append(" -skipnewsscreen=true");
		Process.Start(new ProcessStartInfo(text)
		{
			UseShellExecute = true,
			WorkingDirectory = SdDirectory.GetCurrentDirectory(),
			Arguments = stringBuilder.ToString()
		});
		Log.Out("Restarting game: " + text + " " + stringBuilder.ToString());
		Application.Quit();
	}

	// Token: 0x060092A1 RID: 37537 RVA: 0x003A7724 File Offset: 0x003A5924
	public unsafe static void GetBytes(int _value, byte[] _target, int _targetOffset = 0)
	{
		Utils.GetBytes((byte*)(&_value), 4, _target, _targetOffset);
	}

	// Token: 0x060092A2 RID: 37538 RVA: 0x003A7724 File Offset: 0x003A5924
	public unsafe static void GetBytes(uint _value, byte[] _target, int _targetOffset = 0)
	{
		Utils.GetBytes((byte*)(&_value), 4, _target, _targetOffset);
	}

	// Token: 0x060092A3 RID: 37539 RVA: 0x003A7731 File Offset: 0x003A5931
	public unsafe static void GetBytes(long _value, byte[] _target, int _targetOffset = 0)
	{
		Utils.GetBytes((byte*)(&_value), 8, _target, _targetOffset);
	}

	// Token: 0x060092A4 RID: 37540 RVA: 0x003A7740 File Offset: 0x003A5940
	public unsafe static void GetBytes(byte* _ptr, int _count, byte[] _target, int _targetOffset = 0)
	{
		for (int i = 0; i < _count; i++)
		{
			_target[_targetOffset + i] = _ptr[i];
		}
	}

	// Token: 0x060092A5 RID: 37541 RVA: 0x003A7764 File Offset: 0x003A5964
	public static string MaskIp(string _input)
	{
		if (string.IsNullOrEmpty(_input))
		{
			return _input;
		}
		StringBuilder stringBuilder = new StringBuilder(_input);
		int i;
		if ((i = _input.IndexOfAny(Utils.ipSeparatorChars)) < 0)
		{
			return _input;
		}
		while (i >= 0)
		{
			if (i > 0)
			{
				stringBuilder[i - 1] = '*';
			}
			i = _input.IndexOfAny(Utils.ipSeparatorChars, i + 1);
		}
		stringBuilder[_input.Length - 1] = '*';
		return stringBuilder.ToString();
	}

	// Token: 0x060092A6 RID: 37542 RVA: 0x003A77CC File Offset: 0x003A59CC
	public static void ForceMaterialsInstance(GameObject go)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
		}
	}

	// Token: 0x060092A7 RID: 37543 RVA: 0x003A77F8 File Offset: 0x003A59F8
	public static bool IsValidWebUrl(ref string _input)
	{
		if (string.IsNullOrEmpty(_input))
		{
			return false;
		}
		if (_input.StartsWith("http://") || _input.StartsWith("https://"))
		{
			return true;
		}
		if (_input.Contains("://"))
		{
			return false;
		}
		_input = "http://" + _input;
		return true;
	}

	// Token: 0x060092A8 RID: 37544 RVA: 0x003A784D File Offset: 0x003A5A4D
	public static bool OpenSystemBrowser(string _url)
	{
		if (!Utils.IsValidWebUrl(ref _url))
		{
			return false;
		}
		Application.OpenURL(_url);
		return true;
	}

	// Token: 0x060092A9 RID: 37545 RVA: 0x003A7864 File Offset: 0x003A5A64
	public static bool ArrayEquals(byte[] _a, byte[] _b)
	{
		if (_a == _b)
		{
			return true;
		}
		if (_a == null || _b == null)
		{
			return false;
		}
		if (_a.Length != _b.Length)
		{
			return false;
		}
		for (int i = 0; i < _a.Length; i++)
		{
			if (_a[i] != _b[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060092AA RID: 37546 RVA: 0x003A78A4 File Offset: 0x003A5AA4
	public static bool ArrayEquals(int[] _a, int[] _b)
	{
		if (_a == _b)
		{
			return true;
		}
		if (_a == null || _b == null)
		{
			return false;
		}
		if (_a.Length != _b.Length)
		{
			return false;
		}
		for (int i = 0; i < _a.Length; i++)
		{
			if (_a[i] != _b[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060092AB RID: 37547 RVA: 0x003A78E4 File Offset: 0x003A5AE4
	public static string GenerateGuid()
	{
		byte[] array = Guid.NewGuid().ToByteArray();
		StringBuilder stringBuilder = new StringBuilder(32);
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x17000F1F RID: 3871
	// (get) Token: 0x060092AC RID: 37548 RVA: 0x003A7934 File Offset: 0x003A5B34
	public static uint CurrentUnixTime
	{
		get
		{
			return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}
	}

	// Token: 0x04007031 RID: 28721
	public static string[] ExcludeLayerZoom = new string[]
	{
		"VisibleOnZoom"
	};

	// Token: 0x04007032 RID: 28722
	[PublicizedFrom(EAccessModifier.Private)]
	public static CultureInfo ci;

	// Token: 0x04007033 RID: 28723
	public static readonly Regex hexAlphaMatcher = new Regex("[0-9a-fA-F]{2}");

	// Token: 0x04007034 RID: 28724
	public static readonly Regex hexRgbMatcher = new Regex("[0-9a-fA-F]{6}");

	// Token: 0x04007035 RID: 28725
	public static readonly Regex hexRgbaMatcher = new Regex("[0-9a-fA-F]{8}");

	// Token: 0x04007036 RID: 28726
	public static readonly Regex nestedEscapePattern = new Regex("(\\[+\\/c\\](?:\\/c\\])*)");

	// Token: 0x04007037 RID: 28727
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Transform> setLayerRecursivelyList = new List<Transform>();

	// Token: 0x04007038 RID: 28728
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Collider> setColliderLayerRecursivelyList = new List<Collider>();

	// Token: 0x04007039 RID: 28729
	[PublicizedFrom(EAccessModifier.Private)]
	public static Action<IntPtr, byte, int> MemsetDelegate;

	// Token: 0x0400703A RID: 28730
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3[] tan1Temp = new Vector3[786432];

	// Token: 0x0400703B RID: 28731
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3[] tan2Temp = new Vector3[786432];

	// Token: 0x0400703C RID: 28732
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Vector3> tempNormals = new List<Vector3>();

	// Token: 0x0400703D RID: 28733
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Material> tempMats = new List<Material>();

	// Token: 0x0400703E RID: 28734
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] ipSeparatorChars = new char[]
	{
		'.',
		':'
	};

	// Token: 0x0200124C RID: 4684
	public enum EnumHitDirection : byte
	{
		// Token: 0x04007040 RID: 28736
		Front,
		// Token: 0x04007041 RID: 28737
		Back,
		// Token: 0x04007042 RID: 28738
		Left,
		// Token: 0x04007043 RID: 28739
		Right,
		// Token: 0x04007044 RID: 28740
		Explosion,
		// Token: 0x04007045 RID: 28741
		None
	}

	// Token: 0x0200124D RID: 4685
	public enum ERestartAntiCheatMode
	{
		// Token: 0x04007047 RID: 28743
		KeepAntiCheatMode,
		// Token: 0x04007048 RID: 28744
		ForceOff,
		// Token: 0x04007049 RID: 28745
		ForceOn
	}
}
