using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200128E RID: 4750
[Preserve]
public sealed class vp_ComponentPreset
{
	// Token: 0x17000F43 RID: 3907
	// (get) Token: 0x0600949F RID: 38047 RVA: 0x003B4123 File Offset: 0x003B2323
	// (set) Token: 0x060094A0 RID: 38048 RVA: 0x003B412B File Offset: 0x003B232B
	public Type ComponentType
	{
		get
		{
			return this.m_ComponentType;
		}
		set
		{
			this.m_ComponentType = value;
		}
	}

	// Token: 0x060094A1 RID: 38049 RVA: 0x003B4134 File Offset: 0x003B2334
	public static string Save(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(component);
		return vp_ComponentPreset.Save(vp_ComponentPreset, fullPath, false);
	}

	// Token: 0x060094A2 RID: 38050 RVA: 0x003B414C File Offset: 0x003B234C
	public static string Save(vp_ComponentPreset savePreset, string fullPath, bool isDifference = false)
	{
		vp_ComponentPreset.m_FullPath = fullPath;
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(vp_ComponentPreset.m_FullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null)
		{
			if (vp_ComponentPreset.m_ComponentType != null)
			{
				if (vp_ComponentPreset.ComponentType != savePreset.ComponentType)
				{
					return string.Concat(new string[]
					{
						"'",
						vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath),
						"' has the WRONG component type: ",
						vp_ComponentPreset.ComponentType.ToString(),
						".\n\nDo you want to replace it with a ",
						savePreset.ComponentType.ToString(),
						"?"
					});
				}
				if (File.Exists(vp_ComponentPreset.m_FullPath))
				{
					if (isDifference)
					{
						return "This will update '" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' with only the values modified since pressing Play or setting a state.\n\nContinue?";
					}
					return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' already exists.\n\nDo you want to replace it?";
				}
			}
			if (File.Exists(vp_ComponentPreset.m_FullPath))
			{
				return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' has an UNKNOWN component type.\n\nDo you want to replace it?";
			}
		}
		vp_ComponentPreset.ClearTextFile();
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////");
		vp_ComponentPreset.Append("// Component Preset Script");
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////\n");
		vp_ComponentPreset.Append("ComponentType " + savePreset.ComponentType.Name);
		foreach (vp_ComponentPreset.Field field in savePreset.m_Fields)
		{
			string str = "";
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			string text;
			if (fieldFromHandle.FieldType == typeof(float))
			{
				text = ((float)field.Args).ToCultureInvariantString("0.#######");
			}
			else if (fieldFromHandle.FieldType == typeof(Vector4))
			{
				Vector4 vector = (Vector4)field.Args;
				text = string.Concat(new string[]
				{
					vector.x.ToCultureInvariantString("0.#######"),
					" ",
					vector.y.ToCultureInvariantString("0.#######"),
					" ",
					vector.z.ToCultureInvariantString("0.#######"),
					" ",
					vector.w.ToCultureInvariantString("0.#######")
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector3))
			{
				Vector3 vector2 = (Vector3)field.Args;
				text = string.Concat(new string[]
				{
					vector2.x.ToCultureInvariantString("0.#######"),
					" ",
					vector2.y.ToCultureInvariantString("0.#######"),
					" ",
					vector2.z.ToCultureInvariantString("0.#######")
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector2))
			{
				Vector2 vector3 = (Vector2)field.Args;
				text = vector3.x.ToCultureInvariantString("0.#######") + " " + vector3.y.ToCultureInvariantString("0.#######");
			}
			else if (fieldFromHandle.FieldType == typeof(int))
			{
				text = ((int)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(bool))
			{
				text = ((bool)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(string))
			{
				text = (string)field.Args;
			}
			else
			{
				str = "//";
				text = "<NOTE: Type '" + fieldFromHandle.FieldType.Name.ToString() + "' can't be saved to preset.>";
			}
			if (!string.IsNullOrEmpty(text) && fieldFromHandle.Name != "Persist")
			{
				vp_ComponentPreset.Append(str + fieldFromHandle.Name + " " + text);
			}
		}
		return null;
	}

	// Token: 0x060094A3 RID: 38051 RVA: 0x003B45A8 File Offset: 0x003B27A8
	public static string SaveDifference(vp_ComponentPreset initialStatePreset, Component modifiedComponent, string fullPath, vp_ComponentPreset diskPreset)
	{
		if (initialStatePreset.ComponentType != modifiedComponent.GetType())
		{
			vp_ComponentPreset.Error("Tried to save difference between different type components in 'SaveDifference'");
			return null;
		}
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(modifiedComponent);
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.m_ComponentType = vp_ComponentPreset.ComponentType;
		for (int i = 0; i < vp_ComponentPreset.m_Fields.Count; i++)
		{
			if (!initialStatePreset.m_Fields[i].Args.Equals(vp_ComponentPreset.m_Fields[i].Args))
			{
				vp_ComponentPreset2.m_Fields.Add(vp_ComponentPreset.m_Fields[i]);
			}
		}
		foreach (vp_ComponentPreset.Field field in diskPreset.m_Fields)
		{
			bool flag = true;
			foreach (vp_ComponentPreset.Field field2 in vp_ComponentPreset2.m_Fields)
			{
				if (field.FieldHandle == field2.FieldHandle)
				{
					flag = false;
				}
			}
			bool flag2 = false;
			foreach (vp_ComponentPreset.Field field3 in vp_ComponentPreset.m_Fields)
			{
				if (field.FieldHandle == field3.FieldHandle)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				flag = false;
			}
			if (flag)
			{
				vp_ComponentPreset2.m_Fields.Add(field);
			}
		}
		return vp_ComponentPreset.Save(vp_ComponentPreset2, fullPath, true);
	}

	// Token: 0x060094A4 RID: 38052 RVA: 0x003B4760 File Offset: 0x003B2960
	public void InitFromComponent(Component component)
	{
		this.m_ComponentType = component.GetType();
		this.m_Fields.Clear();
		foreach (FieldInfo fieldInfo in this.m_ComponentType.GetFields())
		{
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string)))
			{
				this.m_Fields.Add(new vp_ComponentPreset.Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
	}

	// Token: 0x060094A5 RID: 38053 RVA: 0x003B4874 File Offset: 0x003B2A74
	public static vp_ComponentPreset CreateFromComponent(Component component)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.m_ComponentType = component.GetType();
		foreach (FieldInfo fieldInfo in vp_ComponentPreset.m_ComponentType.GetFields())
		{
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string)))
			{
				vp_ComponentPreset.m_Fields.Add(new vp_ComponentPreset.Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
		return vp_ComponentPreset;
	}

	// Token: 0x060094A6 RID: 38054 RVA: 0x003B4984 File Offset: 0x003B2B84
	public int TryMakeCompatibleWithComponent(vp_Component component)
	{
		this.m_ComponentType = component.GetType();
		List<FieldInfo> list = new List<FieldInfo>(this.m_ComponentType.GetFields());
		int i = this.m_Fields.Count - 1;
		while (i > -1)
		{
			foreach (FieldInfo fieldInfo in list)
			{
				if (fieldInfo.Name.Contains("PositionOffset") || fieldInfo.Name.Contains("RotationOffset"))
				{
					break;
				}
				if (this.m_Fields[i].FieldHandle == fieldInfo.FieldHandle)
				{
					goto IL_B8;
				}
			}
			goto IL_A0;
			IL_B8:
			i--;
			continue;
			IL_A0:
			this.m_Fields.Remove(this.m_Fields[i]);
			goto IL_B8;
		}
		return this.m_Fields.Count;
	}

	// Token: 0x060094A7 RID: 38055 RVA: 0x003B4A70 File Offset: 0x003B2C70
	public bool LoadTextStream(string fullPath)
	{
		vp_ComponentPreset.m_FullPath = fullPath;
		FileInfo fileInfo = new FileInfo(vp_ComponentPreset.m_FullPath);
		if (fileInfo == null || !fileInfo.Exists)
		{
			vp_ComponentPreset.Error("Failed to read file. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		TextReader textReader = fileInfo.OpenText();
		List<string> list = new List<string>();
		string item;
		while ((item = textReader.ReadLine()) != null)
		{
			list.Add(item);
		}
		textReader.Close();
		if (list == null)
		{
			vp_ComponentPreset.Error("Preset is empty. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		this.ParseLines(list);
		return true;
	}

	// Token: 0x060094A8 RID: 38056 RVA: 0x003B4B08 File Offset: 0x003B2D08
	public static bool Load(vp_Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		return vp_ComponentPreset.Apply(component, vp_ComponentPreset);
	}

	// Token: 0x060094A9 RID: 38057 RVA: 0x003B4B2C File Offset: 0x003B2D2C
	public bool LoadFromResources(string resourcePath)
	{
		vp_ComponentPreset.m_FullPath = resourcePath;
		TextAsset textAsset = Resources.Load(vp_ComponentPreset.m_FullPath) as TextAsset;
		if (textAsset == null)
		{
			vp_ComponentPreset.Error("Failed to read file. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		return this.LoadFromTextAsset(textAsset);
	}

	// Token: 0x060094AA RID: 38058 RVA: 0x003B4B7C File Offset: 0x003B2D7C
	public static vp_ComponentPreset LoadFromResources(vp_Component component, string resourcePath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromResources(resourcePath);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	// Token: 0x060094AB RID: 38059 RVA: 0x003B4BA0 File Offset: 0x003B2DA0
	public bool LoadFromTextAsset(TextAsset file)
	{
		vp_ComponentPreset.m_FullPath = file.name;
		List<string> list = new List<string>();
		foreach (string item in file.text.Split('\n', StringSplitOptions.None))
		{
			list.Add(item);
		}
		if (list == null)
		{
			vp_ComponentPreset.Error("Preset is empty. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		this.ParseLines(list);
		return true;
	}

	// Token: 0x060094AC RID: 38060 RVA: 0x003B4C0C File Offset: 0x003B2E0C
	public static vp_ComponentPreset LoadFromTextAsset(vp_Component component, TextAsset file)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(file);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	// Token: 0x060094AD RID: 38061 RVA: 0x003B4C30 File Offset: 0x003B2E30
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Append(string str)
	{
		str = str.Replace("\n", Environment.NewLine);
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(vp_ComponentPreset.m_FullPath, true);
			streamWriter.WriteLine(str);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			vp_ComponentPreset.Error("Failed to write to file: '" + vp_ComponentPreset.m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	// Token: 0x060094AE RID: 38062 RVA: 0x003B4CA4 File Offset: 0x003B2EA4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ClearTextFile()
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(vp_ComponentPreset.m_FullPath, false);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			vp_ComponentPreset.Error("Failed to clear file: '" + vp_ComponentPreset.m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	// Token: 0x060094AF RID: 38063 RVA: 0x003B4D00 File Offset: 0x003B2F00
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseLines(List<string> lines)
	{
		vp_ComponentPreset.m_LineNumber = 0;
		foreach (string str in lines)
		{
			vp_ComponentPreset.m_LineNumber++;
			string text = vp_ComponentPreset.RemoveComments(str);
			if (!string.IsNullOrEmpty(text) && !this.Parse(text))
			{
				return;
			}
		}
		vp_ComponentPreset.m_LineNumber = 0;
	}

	// Token: 0x060094B0 RID: 38064 RVA: 0x003B4D78 File Offset: 0x003B2F78
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Parse(string line)
	{
		line = line.Trim();
		if (string.IsNullOrEmpty(line))
		{
			return true;
		}
		string[] array = line.Split(null, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		if (this.m_ComponentType == null)
		{
			if (!(array[0] == "ComponentType") || array.Length != 2)
			{
				vp_ComponentPreset.PresetError("Unknown ComponentType.");
				return false;
			}
			vp_ComponentPreset.m_Type = Type.GetType(array[1]);
			if (vp_ComponentPreset.m_Type == null)
			{
				vp_ComponentPreset.PresetError("No such ComponentType: '" + array[1] + "'");
				return false;
			}
			this.m_ComponentType = vp_ComponentPreset.m_Type;
			return true;
		}
		else
		{
			FieldInfo fieldInfo = null;
			foreach (FieldInfo fieldInfo2 in vp_ComponentPreset.m_Type.GetFields())
			{
				if (fieldInfo2.Name == array[0])
				{
					fieldInfo = fieldInfo2;
				}
			}
			if (fieldInfo == null)
			{
				if (array[0] != "ComponentType")
				{
					string[] array2 = this.FindMovedParameter(vp_ComponentPreset.m_Type.Name, array[0]);
					if (array2 != null && array2.Length == 2)
					{
						if ((array2[0] == null || (!string.IsNullOrEmpty(array2[0]) && array2[0] == vp_ComponentPreset.m_Type.Name)) && !string.IsNullOrEmpty(array2[1]) && array2[1] != array[0])
						{
							vp_ComponentPreset.PresetWarning(string.Concat(new string[]
							{
								"The parameter '",
								array[0],
								"' has been renamed to '",
								array2[1],
								"'. Please update your presets."
							}));
						}
						else if (array2[0] != null && array2[0] != vp_ComponentPreset.m_Type.Name && (string.IsNullOrEmpty(array2[1]) || array2[1] == array[0]))
						{
							vp_ComponentPreset.PresetWarning(string.Concat(new string[]
							{
								"The parameter '",
								array[0],
								"' has been moved to the '",
								array2[0],
								"' component. Please update your presets."
							}));
						}
						else if (array2[0] != null && array2[0] != vp_ComponentPreset.m_Type.Name && !string.IsNullOrEmpty(array2[1]) && array2[1] != array[0])
						{
							vp_ComponentPreset.PresetWarning(string.Concat(new string[]
							{
								"The parameter '",
								array[0],
								"' has been moved to the '",
								array2[0],
								"' component and renamed to '",
								array2[1],
								"'. Please update your presets."
							}));
						}
						else
						{
							vp_ComponentPreset.PresetWarning(string.Concat(new string[]
							{
								"'",
								vp_ComponentPreset.m_Type.Name,
								"' no longer supports the parameter: '",
								array[0],
								"'. Please update your presets."
							}));
						}
					}
					else
					{
						vp_ComponentPreset.PresetError(string.Concat(new string[]
						{
							"'",
							vp_ComponentPreset.m_Type.Name,
							"' has no such field: '",
							array[0],
							"'"
						}));
					}
				}
				return true;
			}
			vp_ComponentPreset.Field item = new vp_ComponentPreset.Field(fieldInfo.FieldHandle, vp_ComponentPreset.TokensToObject(fieldInfo, array));
			this.m_Fields.Add(item);
			return true;
		}
	}

	// Token: 0x060094B1 RID: 38065 RVA: 0x003B50A4 File Offset: 0x003B32A4
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] FindMovedParameter(string type, string field)
	{
		string[] result;
		if (!this.MovedParameters.TryGetValue(type + "." + field, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060094B2 RID: 38066 RVA: 0x003B50D0 File Offset: 0x003B32D0
	public static bool Apply(vp_Component component, vp_ComponentPreset preset)
	{
		if (preset == null)
		{
			vp_ComponentPreset.Error("Tried to apply a preset that was null in '" + vp_Utility.GetErrorLocation(1, false) + "'");
			return false;
		}
		if (preset.m_ComponentType == null)
		{
			vp_ComponentPreset.Error("Preset ComponentType was null in '" + vp_Utility.GetErrorLocation(1, false) + "'");
			return false;
		}
		if (component == null)
		{
			vp_ComponentPreset.Error("Component was null when attempting to apply preset in '" + vp_Utility.GetErrorLocation(1, false) + "'");
			return false;
		}
		if (component.Type != preset.m_ComponentType)
		{
			string str = "a '";
			Type componentType = preset.m_ComponentType;
			string text = str + ((componentType != null) ? componentType.ToString() : null) + "' preset";
			if (preset.m_ComponentType == null)
			{
				text = "an unknown preset type";
			}
			vp_ComponentPreset.Error(string.Concat(new string[]
			{
				"Applied ",
				text,
				" to a '",
				component.Type.ToString(),
				"' component in '",
				vp_Utility.GetErrorLocation(1, false),
				"'"
			}));
			return false;
		}
		foreach (vp_ComponentPreset.Field field in preset.m_Fields)
		{
			foreach (FieldInfo fieldInfo in component.Fields)
			{
				if (fieldInfo.FieldHandle == field.FieldHandle)
				{
					fieldInfo.SetValue(component, field.Args);
				}
			}
		}
		return true;
	}

	// Token: 0x060094B3 RID: 38067 RVA: 0x003B526C File Offset: 0x003B346C
	public static Type GetFileType(string fullPath)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	// Token: 0x060094B4 RID: 38068 RVA: 0x003B52B0 File Offset: 0x003B34B0
	public static Type GetFileTypeFromAsset(TextAsset asset)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(asset);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	// Token: 0x060094B5 RID: 38069 RVA: 0x003B52F4 File Offset: 0x003B34F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static object TokensToObject(FieldInfo field, string[] tokens)
	{
		if (field.FieldType == typeof(float))
		{
			return vp_ComponentPreset.ArgsToFloat(tokens);
		}
		if (field.FieldType == typeof(Vector4))
		{
			return vp_ComponentPreset.ArgsToVector4(tokens);
		}
		if (field.FieldType == typeof(Vector3))
		{
			return vp_ComponentPreset.ArgsToVector3(tokens);
		}
		if (field.FieldType == typeof(Vector2))
		{
			return vp_ComponentPreset.ArgsToVector2(tokens);
		}
		if (field.FieldType == typeof(int))
		{
			return vp_ComponentPreset.ArgsToInt(tokens);
		}
		if (field.FieldType == typeof(bool))
		{
			return vp_ComponentPreset.ArgsToBool(tokens);
		}
		if (field.FieldType == typeof(string))
		{
			return vp_ComponentPreset.ArgsToString(tokens);
		}
		return null;
	}

	// Token: 0x060094B6 RID: 38070 RVA: 0x003B53F4 File Offset: 0x003B35F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static string RemoveComments(string str)
	{
		string text = "";
		for (int i = 0; i < str.Length; i++)
		{
			switch (vp_ComponentPreset.m_ReadMode)
			{
			case vp_ComponentPreset.ReadMode.Normal:
				if (str[i] == '/' && str[i + 1] == '*')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.BlockComment;
					i++;
				}
				else if (str[i] == '/' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.LineComment;
					i++;
				}
				else
				{
					text += str[i].ToString();
				}
				break;
			case vp_ComponentPreset.ReadMode.LineComment:
				if (i == str.Length - 1)
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
				}
				break;
			case vp_ComponentPreset.ReadMode.BlockComment:
				if (str[i] == '*' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
					i++;
				}
				break;
			}
		}
		return text;
	}

	// Token: 0x060094B7 RID: 38071 RVA: 0x003B54D4 File Offset: 0x003B36D4
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector4 ArgsToVector4(string[] args)
	{
		if (args.Length - 1 != 4)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector4.zero;
		}
		Vector4 result;
		try
		{
			result = new Vector4(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture), Convert.ToSingle(args[4], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				", ",
				args[4],
				"'"
			}));
			return Vector4.zero;
		}
		return result;
	}

	// Token: 0x060094B8 RID: 38072 RVA: 0x003B55B0 File Offset: 0x003B37B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3 ArgsToVector3(string[] args)
	{
		if (args.Length - 1 != 3)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector3.zero;
		}
		Vector3 result;
		try
		{
			result = new Vector3(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				"'"
			}));
			return Vector3.zero;
		}
		return result;
	}

	// Token: 0x060094B9 RID: 38073 RVA: 0x003B5670 File Offset: 0x003B3870
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector2 ArgsToVector2(string[] args)
	{
		if (args.Length - 1 != 2)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector2.zero;
		}
		Vector2 result;
		try
		{
			result = new Vector2(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				"'"
			}));
			return Vector2.zero;
		}
		return result;
	}

	// Token: 0x060094BA RID: 38074 RVA: 0x003B5718 File Offset: 0x003B3918
	[PublicizedFrom(EAccessModifier.Private)]
	public static float ArgsToFloat(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0f;
		}
		float result;
		try
		{
			result = Convert.ToSingle(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0f;
		}
		return result;
	}

	// Token: 0x060094BB RID: 38075 RVA: 0x003B5790 File Offset: 0x003B3990
	[PublicizedFrom(EAccessModifier.Private)]
	public static int ArgsToInt(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0;
		}
		int result;
		try
		{
			result = Convert.ToInt32(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0;
		}
		return result;
	}

	// Token: 0x060094BC RID: 38076 RVA: 0x003B5800 File Offset: 0x003B3A00
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ArgsToBool(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return false;
		}
		if (args[1].ToLower() == "true")
		{
			return true;
		}
		if (args[1].ToLower() == "false")
		{
			return false;
		}
		vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
		return false;
	}

	// Token: 0x060094BD RID: 38077 RVA: 0x003B5874 File Offset: 0x003B3A74
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ArgsToString(string[] args)
	{
		string text = "";
		for (int i = 1; i < args.Length; i++)
		{
			text += args[i];
			if (i < args.Length - 1)
			{
				text += " ";
			}
		}
		return text;
	}

	// Token: 0x060094BE RID: 38078 RVA: 0x003B58B4 File Offset: 0x003B3AB4
	public Type GetFieldType(string fieldName)
	{
		Type result = null;
		foreach (vp_ComponentPreset.Field field in this.m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = fieldFromHandle.FieldType;
			}
		}
		return result;
	}

	// Token: 0x060094BF RID: 38079 RVA: 0x003B5924 File Offset: 0x003B3B24
	public object GetFieldValue(string fieldName)
	{
		object result = null;
		foreach (vp_ComponentPreset.Field field in this.m_Fields)
		{
			if (FieldInfo.GetFieldFromHandle(field.FieldHandle).Name == fieldName)
			{
				result = field.Args;
			}
		}
		return result;
	}

	// Token: 0x060094C0 RID: 38080 RVA: 0x003B5994 File Offset: 0x003B3B94
	public void SetFieldValue(string fieldName, object value)
	{
		foreach (vp_ComponentPreset.Field field in this.m_Fields)
		{
			if (FieldInfo.GetFieldFromHandle(field.FieldHandle).Name == fieldName)
			{
				field.Args = value;
				break;
			}
		}
	}

	// Token: 0x060094C1 RID: 38081 RVA: 0x003B5A04 File Offset: 0x003B3C04
	public static string ExtractFilenameFromPath(string path)
	{
		int num = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if (num == -1)
		{
			return path;
		}
		if (num == path.Length - 1)
		{
			return "";
		}
		return path.Substring(num + 1, path.Length - num - 1);
	}

	// Token: 0x060094C2 RID: 38082 RVA: 0x003B5A54 File Offset: 0x003B3C54
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PresetError(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError(string.Concat(new string[]
		{
			"Preset Error: ",
			vp_ComponentPreset.m_FullPath,
			" (at ",
			vp_ComponentPreset.m_LineNumber.ToString(),
			") ",
			message
		}));
	}

	// Token: 0x060094C3 RID: 38083 RVA: 0x003B5AAC File Offset: 0x003B3CAC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PresetWarning(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogWarning(string.Concat(new string[]
		{
			"Preset Warning: ",
			vp_ComponentPreset.m_FullPath,
			" (at ",
			vp_ComponentPreset.m_LineNumber.ToString(),
			") ",
			message
		}));
	}

	// Token: 0x060094C4 RID: 38084 RVA: 0x003B5B02 File Offset: 0x003B3D02
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Error(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError("Error: " + message);
	}

	// Token: 0x040071B9 RID: 29113
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_FullPath = null;

	// Token: 0x040071BA RID: 29114
	[PublicizedFrom(EAccessModifier.Private)]
	public static int m_LineNumber = 0;

	// Token: 0x040071BB RID: 29115
	[PublicizedFrom(EAccessModifier.Private)]
	public static Type m_Type = null;

	// Token: 0x040071BC RID: 29116
	public static bool LogErrors = true;

	// Token: 0x040071BD RID: 29117
	[PublicizedFrom(EAccessModifier.Private)]
	public static vp_ComponentPreset.ReadMode m_ReadMode = vp_ComponentPreset.ReadMode.Normal;

	// Token: 0x040071BE RID: 29118
	[PublicizedFrom(EAccessModifier.Private)]
	public Type m_ComponentType;

	// Token: 0x040071BF RID: 29119
	[PublicizedFrom(EAccessModifier.Private)]
	public List<vp_ComponentPreset.Field> m_Fields = new List<vp_ComponentPreset.Field>();

	// Token: 0x040071C0 RID: 29120
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, string[]> MovedParameters = new Dictionary<string, string[]>
	{
		{
			"vp_FPCamera.MouseAcceleration",
			new string[]
			{
				"vp_FPInput",
				"MouseLookAcceleration"
			}
		},
		{
			"vp_FPCamera.MouseSensitivity",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSensitivity"
			}
		},
		{
			"vp_FPCamera.MouseSmoothSteps",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSmoothSteps"
			}
		},
		{
			"vp_FPCamera.MouseSmoothWeight",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSmoothWeight"
			}
		},
		{
			"vp_FPCamera.MouseAccelerationThreshold",
			new string[]
			{
				"vp_FPInput",
				"MouseLookAccelerationThreshold"
			}
		},
		{
			"vp_FPInput.ForceCursor",
			new string[]
			{
				"vp_FPInput",
				"MouseCursorForced"
			}
		}
	};

	// Token: 0x0200128F RID: 4751
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ReadMode
	{
		// Token: 0x040071C2 RID: 29122
		Normal,
		// Token: 0x040071C3 RID: 29123
		LineComment,
		// Token: 0x040071C4 RID: 29124
		BlockComment
	}

	// Token: 0x02001290 RID: 4752
	[PublicizedFrom(EAccessModifier.Private)]
	public class Field
	{
		// Token: 0x060094C7 RID: 38087 RVA: 0x003B5C2D File Offset: 0x003B3E2D
		public Field(RuntimeFieldHandle fieldHandle, object args)
		{
			this.FieldHandle = fieldHandle;
			this.Args = args;
		}

		// Token: 0x040071C5 RID: 29125
		public RuntimeFieldHandle FieldHandle;

		// Token: 0x040071C6 RID: 29126
		public object Args;
	}
}
