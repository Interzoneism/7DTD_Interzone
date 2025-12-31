using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020003BC RID: 956
public abstract class AIDirectorData
{
	// Token: 0x06001D31 RID: 7473 RVA: 0x000B66C2 File Offset: 0x000B48C2
	public static void InitStatic()
	{
		AIDirectorData.noisySounds = new CaseInsensitiveStringDictionary<AIDirectorData.Noise>();
		AIDirectorData.smells = new CaseInsensitiveStringDictionary<AIDirectorData.Smell>();
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x000B66D8 File Offset: 0x000B48D8
	public static void Cleanup()
	{
		if (AIDirectorData.noisySounds != null)
		{
			AIDirectorData.noisySounds.Clear();
		}
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x000B66EB File Offset: 0x000B48EB
	public static void AddNoisySound(string _name, AIDirectorData.Noise _noise)
	{
		AIDirectorData.noisySounds.Add(_name, _noise);
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x000B66F9 File Offset: 0x000B48F9
	public static void AddSmell(string name, AIDirectorData.Smell smell)
	{
		AIDirectorData.smells.Add(name, smell);
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x000B6707 File Offset: 0x000B4907
	public static bool FindNoise(string name, out AIDirectorData.Noise noise)
	{
		if (name == null)
		{
			noise = default(AIDirectorData.Noise);
			return false;
		}
		return AIDirectorData.noisySounds.TryGetValue(name, out noise);
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x000B6721 File Offset: 0x000B4921
	public static bool FindSmell(string name, out AIDirectorData.Smell smell)
	{
		return AIDirectorData.smells.TryGetValue(name, out smell);
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public AIDirectorData()
	{
	}

	// Token: 0x040013F7 RID: 5111
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, AIDirectorData.Noise> noisySounds;

	// Token: 0x040013F8 RID: 5112
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, AIDirectorData.Smell> smells;

	// Token: 0x020003BD RID: 957
	public struct Noise
	{
		// Token: 0x06001D38 RID: 7480 RVA: 0x000B672F File Offset: 0x000B492F
		public Noise(string _source, float _volume, float _duration, float _muffledWhenCrouched, float _heatMapStrength, ulong _heatMapWorldTimeToLive)
		{
			this.volume = _volume;
			this.duration = _duration;
			this.muffledWhenCrouched = _muffledWhenCrouched;
			this.heatMapStrength = _heatMapStrength;
			this.heatMapWorldTimeToLive = _heatMapWorldTimeToLive;
		}

		// Token: 0x040013F9 RID: 5113
		public float volume;

		// Token: 0x040013FA RID: 5114
		public float duration;

		// Token: 0x040013FB RID: 5115
		public float muffledWhenCrouched;

		// Token: 0x040013FC RID: 5116
		public float heatMapStrength;

		// Token: 0x040013FD RID: 5117
		public ulong heatMapWorldTimeToLive;
	}

	// Token: 0x020003BE RID: 958
	[Preserve]
	public class Smell
	{
		// Token: 0x06001D39 RID: 7481 RVA: 0x000B6757 File Offset: 0x000B4957
		public Smell(string _name, float _range, float _beltRange, float _heatMapStrength, ulong _heatMapWorldTimeToLive)
		{
			this.name = _name;
			this.range = _range;
			this.beltRange = _beltRange;
			this.heatMapStrength = _heatMapStrength;
			this.heatMapWorldTimeToLive = _heatMapWorldTimeToLive;
		}

		// Token: 0x040013FE RID: 5118
		public string name;

		// Token: 0x040013FF RID: 5119
		public float range;

		// Token: 0x04001400 RID: 5120
		public float beltRange;

		// Token: 0x04001401 RID: 5121
		public float heatMapStrength;

		// Token: 0x04001402 RID: 5122
		public ulong heatMapWorldTimeToLive;
	}
}
