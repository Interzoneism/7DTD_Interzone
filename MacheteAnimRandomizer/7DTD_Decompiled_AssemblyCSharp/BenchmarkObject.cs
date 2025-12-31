using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// Token: 0x02001130 RID: 4400
public class BenchmarkObject
{
	// Token: 0x06008A4E RID: 35406 RVA: 0x0037FACC File Offset: 0x0037DCCC
	[Conditional("PROFILEx")]
	public static void StartTimer(string _benchmarkName, object _watchObject)
	{
		BenchmarkObject.BenchmarkContainer benchmarkContainer = new BenchmarkObject.BenchmarkContainer(_benchmarkName);
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			BenchmarkObject.benchmarks[_watchObject] = benchmarkContainer;
		}
		benchmarkContainer.startTick = DateTime.Now.Ticks;
	}

	// Token: 0x06008A4F RID: 35407 RVA: 0x0037FB2C File Offset: 0x0037DD2C
	[Conditional("PROFILEx")]
	public static void SwitchObject(object _old, object _new)
	{
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_old))
			{
				BenchmarkObject.BenchmarkContainer value = BenchmarkObject.benchmarks[_old];
				BenchmarkObject.benchmarks.Remove(_old);
				BenchmarkObject.benchmarks[_new] = value;
			}
			else
			{
				Log.Out("SWITCHOBJECT: Object not found");
			}
		}
	}

	// Token: 0x06008A50 RID: 35408 RVA: 0x0037FBA4 File Offset: 0x0037DDA4
	[Conditional("PROFILEx")]
	public static void UpdateName(object _watchObject, string _nameAppend)
	{
		BenchmarkObject.BenchmarkContainer benchmarkContainer = null;
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_watchObject))
			{
				benchmarkContainer = BenchmarkObject.benchmarks[_watchObject];
			}
		}
		if (benchmarkContainer != null)
		{
			BenchmarkObject.BenchmarkContainer benchmarkContainer2 = benchmarkContainer;
			benchmarkContainer2.name += _nameAppend;
			return;
		}
		Log.Out("UPDATENAME: Object not found: " + _nameAppend);
	}

	// Token: 0x06008A51 RID: 35409 RVA: 0x0037FC20 File Offset: 0x0037DE20
	[Conditional("PROFILEx")]
	public static void StopTimer(object _watchObject)
	{
		long ticks = DateTime.Now.Ticks;
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_watchObject))
			{
				BenchmarkObject.benchmarks[_watchObject].endTick = ticks;
			}
			else
			{
				Log.Out("STOPTIMER: Object not found");
			}
		}
	}

	// Token: 0x06008A52 RID: 35410 RVA: 0x0037FC94 File Offset: 0x0037DE94
	[Conditional("PROFILEx")]
	public static void PrintAll()
	{
		if (BenchmarkObject.benchmarks.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (KeyValuePair<object, BenchmarkObject.BenchmarkContainer> keyValuePair in BenchmarkObject.benchmarks)
			{
				if (keyValuePair.Value.name.Length > num)
				{
					num = keyValuePair.Value.name.Length;
				}
			}
			string format = "{0} {1} {2} {3}" + Environment.NewLine;
			stringBuilder.Append(string.Format(format, new object[]
			{
				"Name",
				"Start",
				"End",
				"Duration"
			}));
			foreach (BenchmarkObject.BenchmarkContainer benchmarkContainer in from b in BenchmarkObject.benchmarks.Values.ToList<BenchmarkObject.BenchmarkContainer>()
			orderby b.startTick
			select b)
			{
				stringBuilder.Append(string.Format(format, new object[]
				{
					benchmarkContainer.name,
					benchmarkContainer.startTick / 10L,
					benchmarkContainer.endTick / 10L,
					benchmarkContainer.ticks / 10L
				}));
			}
			SdFile.WriteAllText(GameIO.GetGameDir("") + "durations.txt", stringBuilder.ToString());
		}
	}

	// Token: 0x04006C3D RID: 27709
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<object, BenchmarkObject.BenchmarkContainer> benchmarks = new Dictionary<object, BenchmarkObject.BenchmarkContainer>();

	// Token: 0x02001131 RID: 4401
	public class BenchmarkContainer
	{
		// Token: 0x17000E71 RID: 3697
		// (get) Token: 0x06008A55 RID: 35413 RVA: 0x0037FE4C File Offset: 0x0037E04C
		// (set) Token: 0x06008A56 RID: 35414 RVA: 0x0037FE54 File Offset: 0x0037E054
		public string name
		{
			get
			{
				return this.pName;
			}
			set
			{
				this.pName = value;
			}
		}

		// Token: 0x17000E72 RID: 3698
		// (get) Token: 0x06008A57 RID: 35415 RVA: 0x0037FE5D File Offset: 0x0037E05D
		public long ticks
		{
			get
			{
				return this.endTick - this.startTick;
			}
		}

		// Token: 0x06008A58 RID: 35416 RVA: 0x0037FE6C File Offset: 0x0037E06C
		public BenchmarkContainer(string _name)
		{
			this.pName = _name;
		}

		// Token: 0x04006C3E RID: 27710
		[PublicizedFrom(EAccessModifier.Private)]
		public string pName;

		// Token: 0x04006C3F RID: 27711
		public long startTick;

		// Token: 0x04006C40 RID: 27712
		public long endTick;
	}
}
