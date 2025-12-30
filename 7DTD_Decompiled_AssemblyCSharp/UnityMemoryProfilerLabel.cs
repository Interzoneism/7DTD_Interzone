using System;
using System.Text;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class UnityMemoryProfilerLabel : MonoBehaviour
{
	// Token: 0x060000DC RID: 220 RVA: 0x0000A8FD File Offset: 0x00008AFD
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (this.label == null)
		{
			base.enabled = !base.gameObject.TryGetComponent<UILabel>(out this.label);
			if (!base.enabled)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000A93C File Offset: 0x00008B3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.totalRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 1, ProfilerRecorderOptions.Default);
		this.totalReservedRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory", 1, ProfilerRecorderOptions.Default);
		this.systemRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory", 1, ProfilerRecorderOptions.Default);
		this.gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory", 1, ProfilerRecorderOptions.Default);
		this.gcUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 1, ProfilerRecorderOptions.Default);
		this.gcAllocInFrameMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 1, ProfilerRecorderOptions.Default);
		this.gfxUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx Used Memory", 1, ProfilerRecorderOptions.Default);
		this.mainThreadRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15, ProfilerRecorderOptions.Default);
		this.meshBytesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Mesh Memory", 1, ProfilerRecorderOptions.Default);
		this.meshCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Mesh Count", 1, ProfilerRecorderOptions.Default);
		this.textureBytesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Bytes", 1, ProfilerRecorderOptions.Default);
		this.textureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Used Textures Count", 1, ProfilerRecorderOptions.Default);
		this.renderTextureBytesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render Textures Bytes", 1, ProfilerRecorderOptions.Default);
		this.renderTextureCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render Textures Count", 1, ProfilerRecorderOptions.Default);
		this.setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count", 1, ProfilerRecorderOptions.Default);
		this.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, ProfilerRecorderOptions.Default);
		this.verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count", 1, ProfilerRecorderOptions.Default);
	}

	// Token: 0x060000DE RID: 222 RVA: 0x0000AAE4 File Offset: 0x00008CE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.sb.Clear();
		this.sb.AppendLine("MEMORY");
		if (this.systemRecorder.Valid)
		{
			this.sb.AppendLine("System Used Memory " + UnityMemoryProfilerLabel.ToSize(this.systemRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		if (this.totalReservedRecorder.Valid)
		{
			this.sb.AppendLine("Total Reserved Memory " + UnityMemoryProfilerLabel.ToSize(this.totalReservedRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		if (this.totalRecorder.Valid)
		{
			this.sb.AppendLine("Total Used Memory " + UnityMemoryProfilerLabel.ToSize(this.totalRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		if (this.gcReservedMemoryRecorder.Valid)
		{
			this.sb.AppendLine("GC Reserved Memory " + UnityMemoryProfilerLabel.ToSize(this.gcReservedMemoryRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		if (this.gcUsedMemoryRecorder.Valid)
		{
			this.sb.AppendLine("GC Used Memory " + UnityMemoryProfilerLabel.ToSize(this.gcUsedMemoryRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		if (this.gcAllocInFrameMemoryRecorder.Valid)
		{
			this.sb.AppendLine("GC Allocated This Frame " + UnityMemoryProfilerLabel.ToSize(this.gcAllocInFrameMemoryRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.MiB) + "MiB");
		}
		if (this.mainThreadRecorder.Valid)
		{
			this.sb.AppendLine("Main Thread Memory " + UnityMemoryProfilerLabel.ToSize(this.mainThreadRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.MiB) + "MiB");
		}
		if (this.gfxUsedMemoryRecorder.Valid)
		{
			this.sb.AppendLine("GFX Used Memory " + UnityMemoryProfilerLabel.ToSize(this.gfxUsedMemoryRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
		}
		this.sb.AppendLine();
		this.sb.AppendLine("Rendering");
		if (this.meshCountRecorder.Valid)
		{
			this.sb.AppendLine(string.Format("Mesh Count {0}", this.meshCountRecorder.LastValue));
		}
		if (this.meshBytesRecorder.Valid)
		{
			if (this.meshBytesRecorder.LastValue > 1073741824L)
			{
				this.sb.AppendLine("Mesh Memory " + UnityMemoryProfilerLabel.ToSize(this.meshBytesRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
			}
			else
			{
				this.sb.AppendLine("Mesh Memory " + UnityMemoryProfilerLabel.ToSize(this.meshBytesRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.MiB) + "MiB");
			}
		}
		if (this.textureCountRecorder.Valid && this.textureCountRecorder.LastValue > 0L)
		{
			this.sb.AppendLine(string.Format("Used Textures Count {0}", this.textureCountRecorder.LastValue));
		}
		if (this.textureBytesRecorder.Valid && this.textureBytesRecorder.LastValue > 0L)
		{
			if (this.meshBytesRecorder.LastValue > 1073741824L)
			{
				this.sb.AppendLine("Used Textures " + UnityMemoryProfilerLabel.ToSize(this.textureBytesRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.GiB) + "GiB");
			}
			else
			{
				this.sb.AppendLine("Used Textures " + UnityMemoryProfilerLabel.ToSize(this.textureBytesRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.MiB) + "MiB");
			}
		}
		if (this.renderTextureCountRecorder.Valid)
		{
			this.sb.AppendLine(string.Format("Render Textures {0}", this.renderTextureCountRecorder.LastValue));
		}
		if (this.renderTextureBytesRecorder.Valid)
		{
			this.sb.AppendLine("Render Textures " + UnityMemoryProfilerLabel.ToSize(this.renderTextureBytesRecorder.LastValue, UnityMemoryProfilerLabel.SiSizeUnits.MiB) + "MiB");
		}
		if (this.setPassCallsRecorder.Valid)
		{
			this.sb.AppendLine(string.Format("SetPass Calls: {0}", this.setPassCallsRecorder.LastValue));
		}
		if (this.drawCallsRecorder.Valid)
		{
			this.sb.AppendLine(string.Format("Draw Calls: {0}", this.drawCallsRecorder.LastValue));
		}
		if (this.verticesRecorder.Valid)
		{
			this.sb.AppendLine(string.Format("Vertices: {0}", this.verticesRecorder.LastValue));
		}
		this.label.text = this.sb.ToString();
	}

	// Token: 0x060000DF RID: 223 RVA: 0x0000AF88 File Offset: 0x00009188
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.totalRecorder.Dispose();
		this.totalReservedRecorder.Dispose();
		this.systemRecorder.Dispose();
		this.gcReservedMemoryRecorder.Dispose();
		this.gcUsedMemoryRecorder.Dispose();
		this.gcAllocInFrameMemoryRecorder.Dispose();
		this.gfxUsedMemoryRecorder.Dispose();
		this.mainThreadRecorder.Dispose();
		this.textureBytesRecorder.Dispose();
		this.textureCountRecorder.Dispose();
		this.renderTextureBytesRecorder.Dispose();
		this.renderTextureCountRecorder.Dispose();
		this.setPassCallsRecorder.Dispose();
		this.drawCallsRecorder.Dispose();
		this.verticesRecorder.Dispose();
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x0000B03C File Offset: 0x0000923C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ToSize(long _value, UnityMemoryProfilerLabel.SiSizeUnits _unit)
	{
		return ((double)_value / Math.Pow(1024.0, (double)((long)_unit))).ToString("0.0000");
	}

	// Token: 0x040000FA RID: 250
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public UILabel label;

	// Token: 0x040000FB RID: 251
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder totalRecorder;

	// Token: 0x040000FC RID: 252
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder totalReservedRecorder;

	// Token: 0x040000FD RID: 253
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder systemRecorder;

	// Token: 0x040000FE RID: 254
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder gcReservedMemoryRecorder;

	// Token: 0x040000FF RID: 255
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder gcUsedMemoryRecorder;

	// Token: 0x04000100 RID: 256
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder gcAllocInFrameMemoryRecorder;

	// Token: 0x04000101 RID: 257
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder gfxUsedMemoryRecorder;

	// Token: 0x04000102 RID: 258
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder mainThreadRecorder;

	// Token: 0x04000103 RID: 259
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder meshBytesRecorder;

	// Token: 0x04000104 RID: 260
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder meshCountRecorder;

	// Token: 0x04000105 RID: 261
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder textureBytesRecorder;

	// Token: 0x04000106 RID: 262
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder textureCountRecorder;

	// Token: 0x04000107 RID: 263
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder renderTextureBytesRecorder;

	// Token: 0x04000108 RID: 264
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder renderTextureCountRecorder;

	// Token: 0x04000109 RID: 265
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder setPassCallsRecorder;

	// Token: 0x0400010A RID: 266
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder drawCallsRecorder;

	// Token: 0x0400010B RID: 267
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProfilerRecorder verticesRecorder;

	// Token: 0x0400010C RID: 268
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public StringBuilder sb = new StringBuilder(500);

	// Token: 0x02000022 RID: 34
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SiSizeUnits
	{
		// Token: 0x0400010E RID: 270
		Byte,
		// Token: 0x0400010F RID: 271
		KiB,
		// Token: 0x04000110 RID: 272
		MiB,
		// Token: 0x04000111 RID: 273
		GiB,
		// Token: 0x04000112 RID: 274
		TiB,
		// Token: 0x04000113 RID: 275
		PiB,
		// Token: 0x04000114 RID: 276
		EiB,
		// Token: 0x04000115 RID: 277
		ZiB,
		// Token: 0x04000116 RID: 278
		YiB
	}
}
