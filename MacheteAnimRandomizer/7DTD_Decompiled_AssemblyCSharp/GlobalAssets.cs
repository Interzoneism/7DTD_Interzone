using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000FB4 RID: 4020
public static class GlobalAssets
{
	// Token: 0x06008018 RID: 32792 RVA: 0x00341384 File Offset: 0x0033F584
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> LoadShaderMappings()
	{
		return JsonUtility.FromJson<AssetMappings>(File.ReadAllText(Path.Combine(Addressables.RuntimePath, "shaders.json"))).ToDictionary();
	}

	// Token: 0x06008019 RID: 32793 RVA: 0x003413A4 File Offset: 0x0033F5A4
	public static Shader FindShader(string name)
	{
		if (GlobalAssets.shaders == null)
		{
			GlobalAssets.shaders = GlobalAssets.LoadShaderMappings();
		}
		string key;
		if (GlobalAssets.shaders.TryGetValue(name, out key))
		{
			return LoadManager.LoadAssetFromAddressables<Shader>(key, null, null, false, true, false).Asset;
		}
		return Shader.Find(name);
	}

	// Token: 0x04006309 RID: 25353
	public const string ShaderMappingFile = "shaders.json";

	// Token: 0x0400630A RID: 25354
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> shaders;
}
