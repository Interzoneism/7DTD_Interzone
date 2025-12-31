using System;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000C7F RID: 3199
public struct RemoteWorldInfo
{
	// Token: 0x060062BB RID: 25275 RVA: 0x00281EC8 File Offset: 0x002800C8
	public RemoteWorldInfo(string gameName, string worldName, VersionInformation gameVersion, long saveSize)
	{
		this.gameName = gameName;
		this.worldName = worldName;
		this.gameVersion = gameVersion;
		this.saveSize = saveSize;
	}

	// Token: 0x060062BC RID: 25276 RVA: 0x00281EE8 File Offset: 0x002800E8
	public static bool TryRead(string filePath, out RemoteWorldInfo remoteWorldInfo)
	{
		if (!SdFile.Exists(filePath))
		{
			remoteWorldInfo = default(RemoteWorldInfo);
			return false;
		}
		bool result;
		try
		{
			XElement root = SdXDocument.Load(filePath).Root;
			string text;
			string text2;
			string serializedVersionInformation;
			VersionInformation versionInformation;
			string s;
			long num;
			if (root == null)
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Could not find root node.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!root.TryGetAttribute("gameName", out text))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Could not find gameName attribute.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!root.TryGetAttribute("worldName", out text2))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Could not find worldName attribute.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!root.TryGetAttribute("gameVersion", out serializedVersionInformation))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Could not find gameVersion attribute.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!VersionInformation.TryParseSerializedString(serializedVersionInformation, out versionInformation))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Failed to parse gameVersion value.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!root.TryGetAttribute("saveSize", out s))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Could not find saveSize attribute.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else if (!long.TryParse(s, out num))
			{
				Debug.LogError("Failed to read RemoteWorldInfo at path \"" + filePath + "\". Failed to parse saveSize value.");
				remoteWorldInfo = default(RemoteWorldInfo);
				result = false;
			}
			else
			{
				remoteWorldInfo = new RemoteWorldInfo(text, text2, versionInformation, num);
				result = true;
			}
		}
		catch (Exception arg)
		{
			Debug.LogError(string.Format("Failed to read RemoteWorldInfo at path \"{0}\". Failed with exception: \n\n{1}", filePath, arg));
			remoteWorldInfo = default(RemoteWorldInfo);
			result = false;
		}
		return result;
	}

	// Token: 0x060062BD RID: 25277 RVA: 0x002820C8 File Offset: 0x002802C8
	public void Write(string filePath)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.CreateXmlDeclaration();
		XmlElement element = xmlDocument.AddXmlElement("RemoteWorldInfo");
		element.SetAttrib("gameName", this.gameName);
		element.SetAttrib("worldName", this.worldName);
		element.SetAttrib("gameVersion", this.gameVersion.SerializableString);
		element.SetAttrib("saveSize", this.saveSize.ToString());
		xmlDocument.SdSave(filePath);
	}

	// Token: 0x04004A50 RID: 19024
	public const string FileNameString = "RemoteWorldInfo.xml";

	// Token: 0x04004A51 RID: 19025
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameNameString = "gameName";

	// Token: 0x04004A52 RID: 19026
	[PublicizedFrom(EAccessModifier.Private)]
	public const string WorldNameString = "worldName";

	// Token: 0x04004A53 RID: 19027
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameVersionString = "gameVersion";

	// Token: 0x04004A54 RID: 19028
	[PublicizedFrom(EAccessModifier.Private)]
	public const string SaveSizeString = "saveSize";

	// Token: 0x04004A55 RID: 19029
	public readonly string gameName;

	// Token: 0x04004A56 RID: 19030
	public readonly string worldName;

	// Token: 0x04004A57 RID: 19031
	public readonly VersionInformation gameVersion;

	// Token: 0x04004A58 RID: 19032
	public readonly long saveSize;
}
