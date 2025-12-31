using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000A49 RID: 2633
public class SpawnPointManager : ISelectionBoxCallback
{
	// Token: 0x0600507A RID: 20602 RVA: 0x001FF7BB File Offset: 0x001FD9BB
	public SpawnPointManager(bool _bAddToSelectionBoxes = true)
	{
		this.bAddToSelectionBoxes = _bAddToSelectionBoxes;
		if (_bAddToSelectionBoxes)
		{
			SelectionBoxManager.Instance.GetCategory("StartPoint").SetCallback(this);
		}
	}

	// Token: 0x0600507B RID: 20603 RVA: 0x001FF7ED File Offset: 0x001FD9ED
	public void Cleanup()
	{
		if (this.bAddToSelectionBoxes)
		{
			SelectionBoxManager.Instance.GetCategory("StartPoint").Clear();
		}
	}

	// Token: 0x0600507C RID: 20604 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool OnSelectionBoxActivated(string _category, string _name, bool _bActivated)
	{
		return true;
	}

	// Token: 0x0600507D RID: 20605 RVA: 0x001FF80C File Offset: 0x001FDA0C
	public void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector)
	{
		Vector3i vector3i = Vector3i.Parse(_name);
		Vector3i vector3i2 = vector3i + new Vector3i(_moveVector);
		SelectionBoxManager.Instance.GetCategory(_category).GetBox(_name).SetPositionAndSize(vector3i2, Vector3i.one);
		SelectionCategory category = SelectionBoxManager.Instance.GetCategory(_category);
		Vector3i vector3i3 = vector3i2;
		category.RenameBox(_name, vector3i3.ToString() ?? "");
		SpawnPoint spawnPoint = this.spawnPointList.Find(vector3i);
		if (spawnPoint != null)
		{
			spawnPoint.spawnPosition.position = vector3i2.ToVector3();
		}
	}

	// Token: 0x0600507E RID: 20606 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
	}

	// Token: 0x0600507F RID: 20607 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxMirrored(Vector3i _axis)
	{
	}

	// Token: 0x06005080 RID: 20608 RVA: 0x001FF89C File Offset: 0x001FDA9C
	public bool OnSelectionBoxDelete(string _category, string _name)
	{
		Vector3i blockPos = Vector3i.Parse(_name);
		SpawnPoint spawnPoint = this.spawnPointList.Find(blockPos);
		if (spawnPoint != null)
		{
			this.spawnPointList.Remove(spawnPoint);
		}
		return true;
	}

	// Token: 0x06005081 RID: 20609 RVA: 0x001FF8CE File Offset: 0x001FDACE
	public bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria)
	{
		return _criteria == EnumSelectionBoxAvailabilities.CanShowProperties;
	}

	// Token: 0x06005082 RID: 20610 RVA: 0x001FF8D4 File Offset: 0x001FDAD4
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
		string text;
		string text2;
		if (SelectionBoxManager.Instance.GetSelected(out text, out text2) && text.Equals("StartPoint"))
		{
			_windowManager.SwitchVisible(XUiC_StartPointEditor.ID, false, true);
		}
	}

	// Token: 0x06005083 RID: 20611 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxRotated(string _category, string _name)
	{
	}

	// Token: 0x06005084 RID: 20612 RVA: 0x001FF90C File Offset: 0x001FDB0C
	public bool Load(string _path)
	{
		if (!SdFile.Exists(_path + "/spawnpoints.xml"))
		{
			return false;
		}
		try
		{
			foreach (XElement element in new XmlFile(_path, "spawnpoints", false, false).XmlDoc.Root.Elements("spawnpoint"))
			{
				Vector3 position = StringParsers.ParseVector3(element.GetAttribute("position"), 0, -1);
				Vector3 vector = Vector3.zero;
				if (element.HasAttribute("rotation"))
				{
					vector = StringParsers.ParseVector3(element.GetAttribute("rotation"), 0, -1);
				}
				this.spawnPointList.Add(new SpawnPoint(position, vector.y));
			}
		}
		catch (Exception ex)
		{
			Log.Error("Loading spawnpoints xml file for level '" + Path.GetFileName(_path) + "': " + ex.Message);
		}
		return true;
	}

	// Token: 0x06005085 RID: 20613 RVA: 0x001FFA1C File Offset: 0x001FDC1C
	public bool Save(string _path)
	{
		bool result;
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.CreateXmlDeclaration();
			XmlElement node = xmlDocument.AddXmlElement("spawnpoints");
			for (int i = 0; i < this.spawnPointList.Count; i++)
			{
				SpawnPoint spawnPoint = this.spawnPointList[i];
				Vector3 position = spawnPoint.spawnPosition.position;
				string value = string.Concat(new string[]
				{
					position.x.ToCultureInvariantString(),
					",",
					position.y.ToCultureInvariantString(),
					",",
					position.z.ToCultureInvariantString()
				});
				node.AddXmlElement("spawnpoint").SetAttrib("position", value).SetAttrib("rotation", "0," + spawnPoint.spawnPosition.heading.ToCultureInvariantString() + ",0");
			}
			xmlDocument.SdSave(_path + "/spawnpoints.xml");
			result = true;
		}
		catch (Exception ex)
		{
			Log.Error(ex.ToString());
			result = false;
		}
		return result;
	}

	// Token: 0x04003D9E RID: 15774
	public SpawnPointList spawnPointList = new SpawnPointList();

	// Token: 0x04003D9F RID: 15775
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool bAddToSelectionBoxes;
}
