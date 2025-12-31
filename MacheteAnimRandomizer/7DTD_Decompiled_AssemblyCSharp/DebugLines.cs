using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F3B RID: 3899
public class DebugLines : MonoBehaviour
{
	// Token: 0x06007C1E RID: 31774 RVA: 0x00323B72 File Offset: 0x00321D72
	public static DebugLines Create(string _name, Transform _parentT, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		return debugLines;
	}

	// Token: 0x06007C1F RID: 31775 RVA: 0x00323BA8 File Offset: 0x00321DA8
	public static DebugLines Create(string _name, Transform _parentT, Vector3 _pos1, Vector3 _pos2, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, _pos1 - Origin.position);
		lineRenderer.SetPosition(1, _pos2 - Origin.position);
		return debugLines;
	}

	// Token: 0x06007C20 RID: 31776 RVA: 0x00323C14 File Offset: 0x00321E14
	public static DebugLines CreateAttached(string _name, Transform _parentT, Vector3 _pos1, Vector3 _pos2, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		lineRenderer.positionCount = 2;
		Vector3 position = _parentT.InverseTransformPoint(_pos1 - Origin.position);
		lineRenderer.SetPosition(0, position);
		Vector3 position2 = _parentT.InverseTransformPoint(_pos2 - Origin.position);
		lineRenderer.SetPosition(1, position2);
		return debugLines;
	}

	// Token: 0x06007C21 RID: 31777 RVA: 0x00323C98 File Offset: 0x00321E98
	[PublicizedFrom(EAccessModifier.Private)]
	public static DebugLines Create(string _name, Transform _parentT)
	{
		DebugLines debugLines = null;
		string text = "DebugLines";
		if (_name != null)
		{
			text += _name;
			DebugLines.lines.TryGetValue(_name, out debugLines);
		}
		if (!debugLines)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Debug/DebugLines"), _parentT);
			gameObject.name = text;
			debugLines = gameObject.transform.GetComponent<DebugLines>();
			if (_name != null)
			{
				debugLines.keyName = _name;
				DebugLines.lines[_name] = debugLines;
			}
		}
		else
		{
			debugLines.transform.SetParent(_parentT, false);
		}
		debugLines.line = debugLines.GetComponent<LineRenderer>();
		debugLines.line.positionCount = 0;
		return debugLines;
	}

	// Token: 0x06007C22 RID: 31778 RVA: 0x00323D34 File Offset: 0x00321F34
	public void AddPoint(Vector3 _pos)
	{
		int positionCount = this.line.positionCount;
		this.line.positionCount = positionCount + 1;
		Vector3 position = _pos - Origin.position;
		if (!this.line.useWorldSpace)
		{
			position = base.transform.InverseTransformPoint(position);
		}
		this.line.SetPosition(positionCount, position);
	}

	// Token: 0x06007C23 RID: 31779 RVA: 0x00323D90 File Offset: 0x00321F90
	public void AddCube(Vector3 _cornerPos1, Vector3 _cornerPos2)
	{
		Vector3 pos = _cornerPos1;
		Vector3 vector = _cornerPos2 - _cornerPos1;
		this.AddPoint(pos);
		pos.x += vector.x;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.z += vector.z;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.x -= vector.x;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.z -= vector.z;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.x += vector.x;
		this.AddPoint(pos);
		pos.z += vector.z;
		this.AddPoint(pos);
		pos.x -= vector.x;
		this.AddPoint(pos);
		pos.z -= vector.z;
		this.AddPoint(pos);
	}

	// Token: 0x06007C24 RID: 31780 RVA: 0x00323F16 File Offset: 0x00322116
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.duration -= Time.deltaTime;
		if (this.duration <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06007C25 RID: 31781 RVA: 0x00323F42 File Offset: 0x00322142
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (this.keyName != null)
		{
			DebugLines.lines.Remove(this.keyName);
		}
	}

	// Token: 0x04005EE7 RID: 24295
	public static Vector3 InsideOffsetV = new Vector3(0.05f, 0.05f, 0.05f);

	// Token: 0x04005EE8 RID: 24296
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cName = "DebugLines";

	// Token: 0x04005EE9 RID: 24297
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Dictionary<string, DebugLines> lines = new Dictionary<string, DebugLines>();

	// Token: 0x04005EEA RID: 24298
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string keyName;

	// Token: 0x04005EEB RID: 24299
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float duration;

	// Token: 0x04005EEC RID: 24300
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LineRenderer line;
}
