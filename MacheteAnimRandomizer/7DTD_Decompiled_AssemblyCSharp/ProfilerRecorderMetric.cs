using System;
using System.Text;
using Unity.Profiling;

// Token: 0x0200127D RID: 4733
public class ProfilerRecorderMetric : IMetric
{
	// Token: 0x17000F32 RID: 3890
	// (get) Token: 0x0600941A RID: 37914 RVA: 0x003B1740 File Offset: 0x003AF940
	// (set) Token: 0x0600941B RID: 37915 RVA: 0x003B1748 File Offset: 0x003AF948
	public string Header { get; set; }

	// Token: 0x0600941C RID: 37916 RVA: 0x003B1751 File Offset: 0x003AF951
	public void AppendLastValue(StringBuilder builder)
	{
		ProfilerUtils.AppendLastValue(this.recorder, builder);
	}

	// Token: 0x0600941D RID: 37917 RVA: 0x003B175F File Offset: 0x003AF95F
	public void Cleanup()
	{
		this.recorder.Dispose();
	}

	// Token: 0x04007148 RID: 29000
	public ProfilerRecorder recorder;
}
