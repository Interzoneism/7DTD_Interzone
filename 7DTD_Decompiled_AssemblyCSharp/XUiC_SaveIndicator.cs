using System;
using System.Diagnostics;
using UnityEngine.Scripting;

// Token: 0x02000DF0 RID: 3568
[Preserve]
public class XUiC_SaveIndicator : XUiController
{
	// Token: 0x06006FD4 RID: 28628 RVA: 0x002D9E08 File Offset: 0x002D8008
	public override void Init()
	{
		base.Init();
		this.m_window = (XUiV_Window)base.ViewComponent;
		this.m_tailTimer = new Stopwatch();
		this.m_saveDataManager = SaveDataUtils.SaveDataManager;
		this.m_saveDataManager.CommitStarted += this.OnCommitStarted;
		this.m_saveDataManager.CommitFinished += this.OnCommitFinished;
		this.ID = base.WindowGroup.ID;
		base.xui.saveIndicator = this;
		this.m_window.TargetAlpha = 0.0015f;
	}

	// Token: 0x06006FD5 RID: 28629 RVA: 0x002D9EA0 File Offset: 0x002D80A0
	public override void Cleanup()
	{
		base.Cleanup();
		if (this.m_saveDataManager != null)
		{
			this.m_saveDataManager.CommitStarted -= this.OnCommitStarted;
			this.m_saveDataManager.CommitFinished -= this.OnCommitFinished;
			this.m_saveDataManager = null;
		}
		this.m_window = null;
	}

	// Token: 0x06006FD6 RID: 28630 RVA: 0x002D9EF7 File Offset: 0x002D80F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnCommitStarted()
	{
		this.m_commitInProgress = true;
	}

	// Token: 0x06006FD7 RID: 28631 RVA: 0x002D9F00 File Offset: 0x002D8100
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnCommitFinished()
	{
		this.m_tailTimer.Restart();
		this.m_commitInProgress = false;
	}

	// Token: 0x06006FD8 RID: 28632 RVA: 0x002D9F14 File Offset: 0x002D8114
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.m_commitInProgress && this.m_tailTimer.IsRunning && this.m_tailTimer.Elapsed >= XUiC_SaveIndicator.TailDuration)
		{
			this.m_tailTimer.Stop();
		}
		bool flag = this.m_commitInProgress || this.m_tailTimer.IsRunning;
		this.m_window.TargetAlpha = (flag ? 1f : 0.0015f);
	}

	// Token: 0x040054DE RID: 21726
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly TimeSpan TailDuration = TimeSpan.FromSeconds(2.0);

	// Token: 0x040054DF RID: 21727
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Window m_window;

	// Token: 0x040054E0 RID: 21728
	[PublicizedFrom(EAccessModifier.Private)]
	public ISaveDataManager m_saveDataManager;

	// Token: 0x040054E1 RID: 21729
	[PublicizedFrom(EAccessModifier.Private)]
	public Stopwatch m_tailTimer;

	// Token: 0x040054E2 RID: 21730
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_commitInProgress;

	// Token: 0x040054E3 RID: 21731
	public string ID = "";
}
