using System;
using System.Threading;

// Token: 0x0200034E RID: 846
public class DynamicMeshRegionBuilder
{
	// Token: 0x170002CB RID: 715
	// (get) Token: 0x060018DA RID: 6362 RVA: 0x0009836F File Offset: 0x0009656F
	public static World world
	{
		get
		{
			return GameManager.Instance.World;
		}
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x0009837B File Offset: 0x0009657B
	public bool AddNewItem(DynamicMeshRegion region)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start when not ready. Current Status: " + this.Status.ToString());
			return false;
		}
		this.Region = region;
		this.Status = DynamicMeshBuilderStatus.StartingExport;
		return true;
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x000983B8 File Offset: 0x000965B8
	public void RequestStop(bool forceStop = false)
	{
		this.StopRequested = true;
		if (forceStop)
		{
			try
			{
				Thread thread = this.thread;
				if (thread != null)
				{
					thread.Abort();
				}
			}
			catch (Exception)
			{
			}
			this.Status = DynamicMeshBuilderStatus.Stopped;
		}
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x000983FC File Offset: 0x000965FC
	public void StartThread()
	{
		this.thread = new Thread(delegate()
		{
			try
			{
				while (!this.StopRequested)
				{
					if (GameManager.Instance == null)
					{
						return;
					}
					if (GameManager.Instance.World == null)
					{
						return;
					}
					if (this.Status == DynamicMeshBuilderStatus.Ready || this.Status == DynamicMeshBuilderStatus.Complete)
					{
						Thread.Sleep(100);
					}
					else
					{
						if (this.Status != DynamicMeshBuilderStatus.StartingExport)
						{
							Log.Error("Builder thread and wrong state: " + this.Status.ToString());
							this.Status = DynamicMeshBuilderStatus.Error;
							return;
						}
						throw new NotImplementedException("No build method");
					}
				}
			}
			catch (Exception ex)
			{
				this.Error = "Builder error: " + ex.Message;
				Log.Error(this.Error);
			}
			this.Status = DynamicMeshBuilderStatus.Stopped;
		});
		this.thread.Start();
	}

	// Token: 0x04000FDB RID: 4059
	public string Error;

	// Token: 0x04000FDC RID: 4060
	public bool StopRequested;

	// Token: 0x04000FDD RID: 4061
	public DynamicMeshBuilderStatus Status;

	// Token: 0x04000FDE RID: 4062
	public ExportMeshResult Result = ExportMeshResult.Missing;

	// Token: 0x04000FDF RID: 4063
	public DynamicMeshRegion Region;

	// Token: 0x04000FE0 RID: 4064
	[PublicizedFrom(EAccessModifier.Private)]
	public Thread thread;
}
