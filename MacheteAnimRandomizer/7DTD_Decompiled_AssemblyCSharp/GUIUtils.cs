using System;
using UnityEngine;

// Token: 0x020011A5 RID: 4517
public class GUIUtils
{
	// Token: 0x06008D31 RID: 36145 RVA: 0x0038BA70 File Offset: 0x00389C70
	public static void DrawFilledRect(Rect _rect, Color _colFill, bool _bDrawBorder, Color _colBorder)
	{
		if (Event.current.type == EventType.Repaint)
		{
			GUI.color = _colFill;
			GUI.DrawTexture(_rect, Texture2D.whiteTexture);
			GUI.color = Color.white;
			if (_bDrawBorder)
			{
				_rect.width -= 1f;
				_rect.height -= 1f;
				GUIUtils.DrawRect(_rect, _colBorder);
			}
		}
	}

	// Token: 0x06008D32 RID: 36146 RVA: 0x0038BAD5 File Offset: 0x00389CD5
	public static void DrawFilledRect(Rect _rect, Texture2D _tex, bool _bDrawBorder, Color _colBorder)
	{
		if (Event.current.type == EventType.Repaint)
		{
			GUI.DrawTexture(_rect, _tex);
			if (_bDrawBorder)
			{
				GUIUtils.DrawRect(_rect, _colBorder);
			}
		}
	}

	// Token: 0x06008D33 RID: 36147 RVA: 0x0038BAF8 File Offset: 0x00389CF8
	public static void DrawRect(Rect _rect, Color _col)
	{
		GUIUtils.DrawLine(new Vector2(_rect.x, _rect.y), new Vector2(_rect.x, _rect.y + _rect.height), _col);
		GUIUtils.DrawLine(new Vector2(_rect.x, _rect.y + _rect.height), new Vector2(_rect.x + _rect.width, _rect.y + _rect.height), _col);
		GUIUtils.DrawLine(new Vector2(_rect.x + _rect.width, _rect.y + _rect.height), new Vector2(_rect.x + _rect.width, _rect.y), _col);
		GUIUtils.DrawLine(new Vector2(_rect.x + _rect.width, _rect.y), new Vector2(_rect.x, _rect.y), _col);
	}

	// Token: 0x06008D34 RID: 36148 RVA: 0x0038BBF8 File Offset: 0x00389DF8
	public static void DrawArrow(Vector2 pos, Vector2 dir, float size, Color col)
	{
		Vector2 vector = new Vector2(dir.y, -dir.x);
		dir *= size;
		vector *= size;
		Vector2 pointA = new Vector2(pos.x + dir.x, pos.y - dir.y);
		Vector2 vector2 = new Vector2(pos.x + dir.x / 3f, pos.y - dir.y / 3f);
		Vector2 vector3 = new Vector2(vector2.x + vector.x, vector2.y - vector.y);
		Vector2 vector4 = new Vector2(vector2.x + -vector.x, vector2.y - -vector.y);
		GUIUtils.DrawLine(pointA, vector3, col);
		GUIUtils.DrawLine(pointA, vector4, col);
		GUIUtils.DrawLine(vector3, vector4, col);
		vector3 += new Vector2(-vector.x / 2f, vector.y / 2f);
		vector4 += new Vector2(vector.x / 2f, -vector.y / 2f);
		Vector2 vector5 = new Vector2(vector3.x - dir.x, vector3.y + dir.y);
		Vector2 pointB = new Vector2(vector4.x - dir.x, vector4.y + dir.y);
		GUIUtils.DrawLine(vector3, vector5, col);
		GUIUtils.DrawLine(vector4, pointB, col);
		GUIUtils.DrawLine(vector5, pointB, col);
	}

	// Token: 0x06008D35 RID: 36149 RVA: 0x0038BD7C File Offset: 0x00389F7C
	public static void DrawTriangle(Vector2 pos, Vector2 dir, float size, Color col)
	{
		Vector2 vector = new Vector2(dir.y, -dir.x);
		dir *= size;
		vector *= size;
		Vector2 pointA = new Vector2(pos.x + dir.x / 2f, pos.y - dir.y / 2f);
		Vector2 vector2 = new Vector2(pos.x - dir.x / 2f, pos.y + dir.y / 2f);
		Vector2 vector3 = new Vector2(vector2.x + vector.x, vector2.y - vector.y);
		Vector2 pointB = new Vector2(vector2.x + -vector.x, vector2.y - -vector.y);
		GUIUtils.DrawLine(pointA, vector3, col);
		GUIUtils.DrawLine(pointA, pointB, col);
		GUIUtils.DrawLine(vector3, pointB, col);
	}

	// Token: 0x06008D36 RID: 36150 RVA: 0x0038BE64 File Offset: 0x0038A064
	public static void DrawTriangleWide(Vector3 pos, Vector3 facing, Vector3 perpDir, float size, Color _color)
	{
		Vector3 vector = Vector3.Cross(facing, perpDir);
		facing *= size;
		vector *= size;
		Vector3 pointA = pos + facing;
		Vector3 vector2 = pos + vector;
		Vector3 pointB = pos - vector;
		GUIUtils.DrawLineWide(pointA, vector2, _color, _color);
		GUIUtils.DrawLineWide(pointA, pointB, _color, _color);
		GUIUtils.DrawLineWide(vector2, pointB, _color, _color);
	}

	// Token: 0x06008D37 RID: 36151 RVA: 0x0038BEC4 File Offset: 0x0038A0C4
	public static void DrawRectWide(Vector3 pos, Vector3 facing, Vector3 perpDir, float size, Color _color)
	{
		Vector3 a = Vector3.Cross(facing, perpDir);
		facing *= size;
		a *= size;
		Vector3 vector = pos + a / 2f;
		Vector3 vector2 = pos - a / 2f;
		Vector3 vector3 = vector + facing;
		Vector3 pointB = vector2 + facing;
		GUIUtils.DrawLineWide(vector, vector2, _color, _color);
		GUIUtils.DrawLineWide(vector, vector3, _color, _color);
		GUIUtils.DrawLineWide(vector2, pointB, _color, _color);
		GUIUtils.DrawLineWide(vector3, pointB, _color, _color);
	}

	// Token: 0x06008D38 RID: 36152 RVA: 0x0038BF48 File Offset: 0x0038A148
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool clip_test(float p, float q, ref float u1, ref float u2)
	{
		bool result = true;
		if ((double)p < 0.0)
		{
			float num = q / p;
			if (num > u2)
			{
				result = false;
			}
			else if (num > u1)
			{
				u1 = num;
			}
		}
		else if ((double)p > 0.0)
		{
			float num = q / p;
			if (num < u1)
			{
				result = false;
			}
			else if (num < u2)
			{
				u2 = num;
			}
		}
		else if ((double)q < 0.0)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06008D39 RID: 36153 RVA: 0x0038BFB0 File Offset: 0x0038A1B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = p2.x - p1.x;
		if (GUIUtils.clip_test(-num3, p1.x - bounds.xMin, ref num, ref num2) && GUIUtils.clip_test(num3, bounds.xMax - p1.x, ref num, ref num2))
		{
			float num4 = p2.y - p1.y;
			if (GUIUtils.clip_test(-num4, p1.y - bounds.yMin, ref num, ref num2) && GUIUtils.clip_test(num4, bounds.yMax - p1.y, ref num, ref num2))
			{
				if ((double)num2 < 1.0)
				{
					p2.x = p1.x + num2 * num3;
					p2.y = p1.y + num2 * num4;
				}
				if ((double)num > 0.0)
				{
					p1.x += num * num3;
					p1.y += num * num4;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06008D3A RID: 36154 RVA: 0x0038C0AC File Offset: 0x0038A2AC
	public static void BeginGroup(Rect position)
	{
		GUIUtils.clippingEnabled = true;
		GUIUtils.clippingBounds = new Rect(0f, 0f, position.width, position.height);
		GUI.BeginGroup(position);
	}

	// Token: 0x06008D3B RID: 36155 RVA: 0x0038C0DC File Offset: 0x0038A2DC
	public static void EndGroup()
	{
		GUI.EndGroup();
		GUIUtils.clippingBounds = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		GUIUtils.clippingEnabled = false;
	}

	// Token: 0x06008D3C RID: 36156 RVA: 0x0038C10C File Offset: 0x0038A30C
	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
	{
		if (GUIUtils.clippingEnabled && !GUIUtils.segment_rect_intersection(GUIUtils.clippingBounds, ref pointA, ref pointB))
		{
			return;
		}
		if (!GUIUtils.lineMaterial)
		{
			GUIUtils.createLineMaterial();
		}
		GUIUtils.lineMaterial.SetPass(0);
		GL.Begin(1);
		GL.Color(color);
		GL.Vertex3(pointA.x, pointA.y, 0f);
		GL.Vertex3(pointB.x, pointB.y, 0f);
		GL.End();
	}

	// Token: 0x06008D3D RID: 36157 RVA: 0x0038C18C File Offset: 0x0038A38C
	public static GUIUtils.IntRect RectIntersection(GUIUtils.IntRect r1, GUIUtils.IntRect r2)
	{
		int num = r1.x;
		int num2 = r1.y;
		int x = r2.x;
		int y = r2.y;
		long num3 = (long)num;
		num3 += (long)r1.width;
		long num4 = (long)num2;
		num4 += (long)r1.height;
		long num5 = (long)x;
		num5 += (long)r2.width;
		long num6 = (long)y;
		num6 += (long)r2.height;
		if (num < x)
		{
			num = x;
		}
		if (num2 < y)
		{
			num2 = y;
		}
		if (num3 > num5)
		{
			num3 = num5;
		}
		if (num4 > num6)
		{
			num4 = num6;
		}
		num3 -= (long)num;
		num4 -= (long)num2;
		if (num3 < -2147483648L)
		{
			num3 = 2147483647L;
		}
		if (num4 < -2147483648L)
		{
			num4 = 2147483647L;
		}
		return new GUIUtils.IntRect(num, num2, (int)num3, (int)num4);
	}

	// Token: 0x06008D3E RID: 36158 RVA: 0x0038C254 File Offset: 0x0038A454
	[PublicizedFrom(EAccessModifier.Private)]
	public static void createLineMaterial()
	{
		GUIUtils.lineMaterial = new Material(Resources.Load("Shaders/DrawLine", typeof(Shader)) as Shader);
	}

	// Token: 0x06008D3F RID: 36159 RVA: 0x0038C27C File Offset: 0x0038A47C
	public static void DrawLine(Vector3 pointA, Vector3 pointB, Color color)
	{
		if (!GUIUtils.lineMaterial)
		{
			GUIUtils.createLineMaterial();
		}
		GL.PushMatrix();
		GUIUtils.lineMaterial.SetPass(0);
		GL.Begin(1);
		GL.Color(color);
		GL.Vertex(pointA);
		GL.Vertex(pointB);
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x06008D40 RID: 36160 RVA: 0x0038C2CD File Offset: 0x0038A4CD
	public static void SetupLines(Camera _camera, float _lineWidth)
	{
		GUIUtils.lineCamera = _camera;
		GUIUtils.lineHalfWidth = _lineWidth * 0.5f;
	}

	// Token: 0x06008D41 RID: 36161 RVA: 0x0038C2E1 File Offset: 0x0038A4E1
	public static void DrawLineWide(Vector3 pointA, Vector3 pointB, Color color)
	{
		GUIUtils.DrawLineWide(pointA, pointB, color, color);
	}

	// Token: 0x06008D42 RID: 36162 RVA: 0x0038C2EC File Offset: 0x0038A4EC
	public static void DrawLineWide(Vector3 pointA, Vector3 pointB, Color colorA, Color colorB)
	{
		if (!GUIUtils.lineMaterial)
		{
			GUIUtils.createLineMaterial();
		}
		Vector3 vector = GUIUtils.lineCamera.WorldToScreenPoint(pointA);
		Vector3 a = GUIUtils.lineCamera.WorldToScreenPoint(pointB);
		Vector3 b = Vector3.Cross((a - vector).normalized, Vector3.forward) * GUIUtils.lineHalfWidth;
		GL.PushMatrix();
		GUIUtils.lineMaterial.SetPass(0);
		GL.Begin(7);
		GL.Color(colorA);
		GL.Vertex(GUIUtils.lineCamera.ScreenToWorldPoint(vector - b));
		GL.Vertex(GUIUtils.lineCamera.ScreenToWorldPoint(vector + b));
		GL.Color(colorB);
		GL.Vertex(GUIUtils.lineCamera.ScreenToWorldPoint(a + b));
		GL.Vertex(GUIUtils.lineCamera.ScreenToWorldPoint(a - b));
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x04006DB7 RID: 28087
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool clippingEnabled;

	// Token: 0x04006DB8 RID: 28088
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Rect clippingBounds;

	// Token: 0x04006DB9 RID: 28089
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Material lineMaterial;

	// Token: 0x04006DBA RID: 28090
	[PublicizedFrom(EAccessModifier.Private)]
	public static Camera lineCamera;

	// Token: 0x04006DBB RID: 28091
	[PublicizedFrom(EAccessModifier.Private)]
	public static float lineHalfWidth;

	// Token: 0x020011A6 RID: 4518
	public class IntRect
	{
		// Token: 0x06008D44 RID: 36164 RVA: 0x0038C3CB File Offset: 0x0038A5CB
		public IntRect(int _x, int _y, int _width, int _height)
		{
			this.x = _x;
			this.y = _y;
			this.width = _width;
			this.height = _height;
		}

		// Token: 0x06008D45 RID: 36165 RVA: 0x0038C3F0 File Offset: 0x0038A5F0
		public IntRect(float _x, float _y, float _width, float _height)
		{
			this.x = (int)_x;
			this.y = (int)_y;
			this.width = (int)_width;
			this.height = (int)_height;
		}

		// Token: 0x04006DBC RID: 28092
		public int x;

		// Token: 0x04006DBD RID: 28093
		public int y;

		// Token: 0x04006DBE RID: 28094
		public int width;

		// Token: 0x04006DBF RID: 28095
		public int height;
	}
}
