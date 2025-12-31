using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000276 RID: 630
[Preserve]
public class ConsoleCmdVisitMap : ConsoleCmdAbstract
{
	// Token: 0x060011D9 RID: 4569 RVA: 0x0006F78A File Offset: 0x0006D98A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Visit an given area of the map. Optionally run the density check on each visited chunk.";
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0006F791 File Offset: 0x0006D991
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "\n\t\t\t|Usage:\n\t\t\t|  1. visitmap <x1> <z1> <x2> <z2> [check]\n\t\t\t|  2. visitmap full [check]\n\t\t\t|  3. visitmap stop\n\t\t\t|1. Start visiting the map in the rectangle specified with the two edges defined by\n\t\t\t|   coordinate pairs x1/z1 and x2/z2. If the parameter \"check\" is added each visited\n\t\t\t|   chunk will be checked for density issues.\n\t\t\t|2. Start visiting the full map. If the parameter \"check\" is added each visited\n\t\t\t|   chunk will be checked for density issues.\n\t\t\t|3. Stop the current visitmap run.\n\t\t\t".Unindent(true);
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0006F79E File Offset: 0x0006D99E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"visitmap"
		};
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060011DC RID: 4572 RVA: 0x0006F7AE File Offset: 0x0006D9AE
	public bool IsRunning
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.mapVisitor != null && this.mapVisitor.IsRunning();
		}
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0006F7C8 File Offset: 0x0006D9C8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1 && _params.Count != 2 && _params.Count != 4 && _params.Count != 5)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, 2, 4 or 5, found " + _params.Count.ToString() + ".");
			return;
		}
		bool checkDensities = _params[_params.Count - 1].EqualsCaseInsensitive("check");
		if (_params.Count == 1)
		{
			if (_params[0].EqualsCaseInsensitive("stop"))
			{
				this.stop();
				return;
			}
			if (_params[0].EqualsCaseInsensitive("full"))
			{
				Vector3i start;
				Vector3i end;
				GameManager.Instance.World.GetWorldExtent(out start, out end);
				this.visit(start, end, false);
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Illegal arguments");
			return;
		}
		else
		{
			if (_params.Count != 2)
			{
				if (_params.Count == 4 || _params.Count == 5)
				{
					int x;
					if (!int.TryParse(_params[0], out x))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given x1 coordinate is not a valid integer");
						return;
					}
					int z;
					if (!int.TryParse(_params[1], out z))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given z1 coordinate is not a valid integer");
						return;
					}
					int x2;
					if (!int.TryParse(_params[2], out x2))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given x2 coordinate is not a valid integer");
						return;
					}
					int z2;
					if (!int.TryParse(_params[3], out z2))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given z2 coordinate is not a valid integer");
						return;
					}
					this.visit(new Vector3i(x, 0, z), new Vector3i(x2, 0, z2), checkDensities);
				}
				return;
			}
			if (_params[0].EqualsCaseInsensitive("full"))
			{
				Vector3i start2;
				Vector3i end2;
				GameManager.Instance.World.GetWorldExtent(out start2, out end2);
				this.visit(start2, end2, checkDensities);
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Illegal arguments");
			return;
		}
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0006F99C File Offset: 0x0006DB9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void visit(Vector3i _start, Vector3i _end, bool _checkDensities)
	{
		if (this.mapVisitor != null && this.mapVisitor.IsRunning())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("VisitMap already running. You can stop it with \"visitmap stop\".");
			return;
		}
		this.mapVisitor = new MapVisitor(_start, _end);
		this.mapVisitor.OnVisitChunk += this.LogChunk;
		this.mapVisitor.OnVisitChunk += this.RenderMinimap;
		if (_checkDensities)
		{
			this.densityMismatches = new List<Chunk.DensityMismatchInformation>();
			this.mapVisitor.OnVisitChunk += this.CheckDensities;
		}
		this.mapVisitor.OnVisitMapDone += this.OnDone;
		this.mapVisitor.Start();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Started visiting the map between {0} and {1}", this.mapVisitor.WorldPosStart, this.mapVisitor.WorldPosEnd));
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x0006FA84 File Offset: 0x0006DC84
	[PublicizedFrom(EAccessModifier.Private)]
	public void stop()
	{
		if (!this.IsRunning)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("VisitMap not running.");
			return;
		}
		this.mapVisitor.Stop();
		this.mapVisitor = null;
		this.densityMismatches = null;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("VisitMap stopped.");
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x0006FAD1 File Offset: 0x0006DCD1
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckDensities(Chunk _chunk, int _done, int _total, float _elapsed)
	{
		this.densityMismatches.AddRange(_chunk.CheckDensities(false));
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0006FAE8 File Offset: 0x0006DCE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogChunk(Chunk _chunk, int _done, int _total, float _elapsed)
	{
		if (_done % 200 != 0)
		{
			return;
		}
		float value = (float)(_total - _done) * (_elapsed / (float)_done);
		Log.Out("VisitMap ({3:00}%): {0} / {1} chunks done (estimated time left {2} seconds)", new object[]
		{
			_done,
			_total,
			value.ToCultureInvariantString("0.00"),
			Mathf.RoundToInt(100f * ((float)_done / (float)_total))
		});
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0006FB52 File Offset: 0x0006DD52
	[PublicizedFrom(EAccessModifier.Private)]
	public void RenderMinimap(Chunk _chunk, int _done, int _total, float _elapsed)
	{
		_chunk.GetMapColors();
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x0006FB5C File Offset: 0x0006DD5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDone(int _chunks, float _duration)
	{
		Log.Out("VisitMap done, visited {0} chunks in {1} seconds (average {2} chunks/sec).", new object[]
		{
			_chunks,
			_duration.ToCultureInvariantString("0.00"),
			((float)_chunks / _duration).ToCultureInvariantString("0.00")
		});
		this.writeDensityMismatchFile();
		this.mapVisitor = null;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void writeDensityMismatchFile()
	{
		if (this.densityMismatches == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder("[\n");
		for (int i = 0; i < this.densityMismatches.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(this.densityMismatches[i].ToJsonString());
			stringBuilder.Append('\n');
		}
		stringBuilder.Append("]");
		SdFile.WriteAllText(((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../") : (Application.dataPath + "/../")) + "densitymismatch.json", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("x;y;z;Density;IsTerrain;BvType");
		for (int j = 0; j < this.densityMismatches.Count; j++)
		{
			stringBuilder2.AppendLine(string.Format("{0};{1};{2};{3};{4};{5}", new object[]
			{
				this.densityMismatches[j].x,
				this.densityMismatches[j].y,
				this.densityMismatches[j].z,
				this.densityMismatches[j].density,
				this.densityMismatches[j].isTerrain,
				this.densityMismatches[j].bvType
			}));
		}
		SdFile.WriteAllText(((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../") : (Application.dataPath + "/../")) + "densitymismatch.csv", stringBuilder2.ToString());
		this.densityMismatches = null;
	}

	// Token: 0x04000BCF RID: 3023
	[PublicizedFrom(EAccessModifier.Private)]
	public MapVisitor mapVisitor;

	// Token: 0x04000BD0 RID: 3024
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Chunk.DensityMismatchInformation> densityMismatches;
}
