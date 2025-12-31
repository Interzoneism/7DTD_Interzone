using System;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000C80 RID: 3200
public struct PlayerMetaInfo
{
	// Token: 0x060062BE RID: 25278 RVA: 0x00282145 File Offset: 0x00280345
	public PlayerMetaInfo(PlatformUserIdentifierAbs nativeId, string name, int level, float distanceWalked)
	{
		this.nativeId = nativeId;
		this.name = name;
		this.level = level;
		this.distanceWalked = distanceWalked;
	}

	// Token: 0x060062BF RID: 25279 RVA: 0x00282164 File Offset: 0x00280364
	public static bool TryRead(string filePath, out PlayerMetaInfo playerMetaInfo)
	{
		if (!SdFile.Exists(filePath))
		{
			Debug.LogError("Failed to read PlayerMetaInfo. No file found at path: " + filePath);
			playerMetaInfo = default(PlayerMetaInfo);
			return false;
		}
		bool result;
		try
		{
			XElement root = SdXDocument.Load(filePath).Root;
			if (root == null)
			{
				Debug.LogError("Failed to read PlayerMetaInfo at path \"" + filePath + "\". Could not find root node.");
				playerMetaInfo = default(PlayerMetaInfo);
				result = false;
			}
			else
			{
				string text;
				if (!root.TryGetAttribute("name", out text))
				{
					Debug.LogWarning("No name in PlayerMetaInfo at path \"" + filePath + "\". Could not find name attribute.");
					text = null;
				}
				string text2;
				PlatformUserIdentifierAbs platformUserIdentifierAbs;
				if (!root.TryGetAttribute("nativeid", out text2))
				{
					Debug.LogWarning("No native id in PlayerMetaInfo at path \"" + filePath + "\". Could not find nativeid attribute.");
					platformUserIdentifierAbs = null;
				}
				else if (!PlatformUserIdentifierAbs.TryFromCombinedString(text2, out platformUserIdentifierAbs))
				{
					Debug.LogError("Could not parse native id from PlayerMetaInfo at path \"" + filePath + "\". Combined id string: " + text2);
					playerMetaInfo = default(PlayerMetaInfo);
					return false;
				}
				string s;
				int num;
				string s2;
				float num2;
				if (!root.TryGetAttribute("level", out s) || !int.TryParse(s, out num))
				{
					Debug.LogError("Failed to read PlayerMetaInfo at path \"" + filePath + "\". Could not find level attribute.");
					playerMetaInfo = default(PlayerMetaInfo);
					result = false;
				}
				else if (!root.TryGetAttribute("distanceWalked", out s2) || !float.TryParse(s2, out num2))
				{
					Debug.LogError("Failed to read PlayerMetaInfo at path \"" + filePath + "\". Could not find distanceWalked attribute.");
					playerMetaInfo = default(PlayerMetaInfo);
					result = false;
				}
				else
				{
					playerMetaInfo = new PlayerMetaInfo(platformUserIdentifierAbs, text, num, num2);
					result = true;
				}
			}
		}
		catch (Exception arg)
		{
			Debug.LogError(string.Format("Failed to read PlayerMetaInfo at path \"{0}\". Failed with exception: \n\n{1}", filePath, arg));
			playerMetaInfo = default(PlayerMetaInfo);
			result = false;
		}
		return result;
	}

	// Token: 0x060062C0 RID: 25280 RVA: 0x00282324 File Offset: 0x00280524
	public void Write(string filePath)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.CreateXmlDeclaration();
		XmlElement xmlElement = xmlDocument.AddXmlElement("PlayerMetaInfo");
		if (this.nativeId != null)
		{
			xmlElement.SetAttribute("nativeid", this.nativeId.CombinedString);
		}
		if (this.name != null)
		{
			xmlElement.SetAttribute("name", this.name);
		}
		xmlElement.SetAttribute("level", this.level.ToString());
		xmlElement.SetAttribute("distanceWalked", this.distanceWalked.ToString());
		xmlDocument.SdSave(filePath);
	}

	// Token: 0x04004A59 RID: 19033
	public const string EXT = "meta";

	// Token: 0x04004A5A RID: 19034
	[PublicizedFrom(EAccessModifier.Private)]
	public const string nativeIdAttr = "nativeid";

	// Token: 0x04004A5B RID: 19035
	[PublicizedFrom(EAccessModifier.Private)]
	public const string nameAttr = "name";

	// Token: 0x04004A5C RID: 19036
	[PublicizedFrom(EAccessModifier.Private)]
	public const string levelAttr = "level";

	// Token: 0x04004A5D RID: 19037
	[PublicizedFrom(EAccessModifier.Private)]
	public const string distanceWalkedAttr = "distanceWalked";

	// Token: 0x04004A5E RID: 19038
	public readonly PlatformUserIdentifierAbs nativeId;

	// Token: 0x04004A5F RID: 19039
	public readonly string name;

	// Token: 0x04004A60 RID: 19040
	public readonly int level;

	// Token: 0x04004A61 RID: 19041
	public readonly float distanceWalked;
}
