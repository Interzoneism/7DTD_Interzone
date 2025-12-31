using System;

// Token: 0x0200005E RID: 94
public static class Submission
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000FAF0 File Offset: 0x0000DCF0
	public static bool Enabled
	{
		get
		{
			if (!Submission.isSubmissionChecked)
			{
				string[] commandLineArgs = GameStartupHelper.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					if (commandLineArgs[i].EqualsCaseInsensitive(Constants.cArgSubmissionBuild))
					{
						Log.Out("Submission Enabled by argument");
						Submission.isSubmission = true;
					}
				}
				Submission.isSubmissionChecked = true;
			}
			return Submission.isSubmission;
		}
	}

	// Token: 0x0400027F RID: 639
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool isSubmissionChecked;

	// Token: 0x04000280 RID: 640
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool isSubmission;
}
