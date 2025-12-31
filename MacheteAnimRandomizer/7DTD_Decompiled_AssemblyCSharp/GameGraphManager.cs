using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010D2 RID: 4306
public class GameGraphManager
{
	// Token: 0x06008781 RID: 34689 RVA: 0x0036D2EA File Offset: 0x0036B4EA
	public static GameGraphManager Create(EntityPlayerLocal player)
	{
		GameGraphManager gameGraphManager = new GameGraphManager();
		gameGraphManager.player = player;
		gameGraphManager.Init();
		return gameGraphManager;
	}

	// Token: 0x06008782 RID: 34690 RVA: 0x0036D2FE File Offset: 0x0036B4FE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		if (!GameGraphManager.whiteTex)
		{
			GameGraphManager.whiteTex = new Texture2D(1, 1);
			GameGraphManager.whiteTex.FillTexture(Color.white, true, false);
		}
	}

	// Token: 0x06008783 RID: 34691 RVA: 0x0036D329 File Offset: 0x0036B529
	public void Destroy()
	{
		if (GameGraphManager.whiteTex)
		{
			UnityEngine.Object.Destroy(GameGraphManager.whiteTex);
		}
	}

	// Token: 0x06008784 RID: 34692 RVA: 0x0036D344 File Offset: 0x0036B544
	public void Add(string name, GameGraphManager.Graph.Callback callback, int sampleCount, float maxValue, float markerValue = 0f)
	{
		GameGraphManager.Graph graph = this.FindGraph(name);
		if (graph != null)
		{
			this.graphs.Remove(graph);
		}
		if (sampleCount > 0)
		{
			GameGraphManager.Graph graph2 = new GameGraphManager.Graph(this, name, sampleCount, maxValue, markerValue);
			this.graphs.Add(graph2);
			graph2.callback = callback;
		}
	}

	// Token: 0x06008785 RID: 34693 RVA: 0x0036D390 File Offset: 0x0036B590
	public void AddCVar(string name, int count, string cvarName, float maxValue, float markerValue = 0f)
	{
		GameGraphManager.Graph graph = this.FindGraph(name);
		if (graph != null)
		{
			this.graphs.Remove(graph);
		}
		if (count > 0)
		{
			GameGraphManager.Graph graph2 = new GameGraphManager.Graph(this, name, count, maxValue, markerValue);
			this.graphs.Add(graph2);
			graph2.cvarName = cvarName;
		}
	}

	// Token: 0x06008786 RID: 34694 RVA: 0x0036D3DC File Offset: 0x0036B5DC
	public void AddPassiveEffect(string name, int count, PassiveEffects passiveEffect, float maxValue, float markerValue = 0f)
	{
		GameGraphManager.Graph graph = this.FindGraph(name);
		if (graph != null)
		{
			this.graphs.Remove(graph);
		}
		if (count > 0)
		{
			GameGraphManager.Graph graph2 = new GameGraphManager.Graph(this, name, count, maxValue, markerValue);
			this.graphs.Add(graph2);
			graph2.passiveEffect = passiveEffect;
		}
	}

	// Token: 0x06008787 RID: 34695 RVA: 0x0036D428 File Offset: 0x0036B628
	public void AddStat(string name, int count, string statName, float maxValue, float markerValue = 0f)
	{
		GameGraphManager.Graph graph = this.FindGraph(name);
		if (graph != null)
		{
			this.graphs.Remove(graph);
		}
		if (count > 0)
		{
			GameGraphManager.Graph graph2 = new GameGraphManager.Graph(this, name, count, maxValue, markerValue);
			this.graphs.Add(graph2);
			graph2.statName = statName.ToLower();
		}
	}

	// Token: 0x06008788 RID: 34696 RVA: 0x0036D476 File Offset: 0x0036B676
	public void RemoveAll()
	{
		this.graphs.Clear();
	}

	// Token: 0x06008789 RID: 34697 RVA: 0x0036D484 File Offset: 0x0036B684
	public GameGraphManager.Graph FindGraph(string name)
	{
		for (int i = 0; i < this.graphs.Count; i++)
		{
			GameGraphManager.Graph graph = this.graphs[i];
			if (graph.name == name)
			{
				return graph;
			}
		}
		return null;
	}

	// Token: 0x0600878A RID: 34698 RVA: 0x0036D4C8 File Offset: 0x0036B6C8
	public void Draw()
	{
		bool flag = Event.current.type == EventType.Repaint;
		float num = 1f;
		for (int i = 0; i < this.graphs.Count; i++)
		{
			GameGraphManager.Graph graph = this.graphs[i];
			if (flag)
			{
				graph.UpdateValues();
			}
			graph.pos.x = 2f;
			graph.pos.y = num;
			graph.Draw();
			num += (float)(this.graphHeight + 2);
		}
	}

	// Token: 0x0600878B RID: 34699 RVA: 0x0036D543 File Offset: 0x0036B743
	public void SetHeight(int _height)
	{
		this.graphHeight = _height;
		this.graphHeight = Mathf.Clamp(this.graphHeight, 1, 2100);
	}

	// Token: 0x04006946 RID: 26950
	[PublicizedFrom(EAccessModifier.Private)]
	public static Texture2D whiteTex;

	// Token: 0x04006947 RID: 26951
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04006948 RID: 26952
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameGraphManager.Graph> graphs = new List<GameGraphManager.Graph>();

	// Token: 0x04006949 RID: 26953
	[PublicizedFrom(EAccessModifier.Private)]
	public int graphHeight = 100;

	// Token: 0x020010D3 RID: 4307
	public class Graph
	{
		// Token: 0x0600878D RID: 34701 RVA: 0x0036D580 File Offset: 0x0036B780
		public Graph(GameGraphManager _manager, string _name, int _count, float _maxValue, float _markerValue)
		{
			this.manager = _manager;
			this.name = _name;
			this.count = _count;
			this.count = Mathf.Clamp(this.count, 1, 4096);
			this.maxValue = _maxValue;
			this.markerValue = _markerValue;
			this.values = new float[this.count];
		}

		// Token: 0x0600878E RID: 34702 RVA: 0x0036D5E0 File Offset: 0x0036B7E0
		public void AddValue(float value)
		{
			this.index = (this.index + 1) % this.count;
			this.values[this.index] = value;
		}

		// Token: 0x0600878F RID: 34703 RVA: 0x0036D608 File Offset: 0x0036B808
		public void Draw()
		{
			Texture whiteTex = GameGraphManager.whiteTex;
			float width = (float)this.count * 2f + 2f;
			int graphHeight = this.manager.graphHeight;
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(this.pos.x, this.pos.y, width, (float)(graphHeight + 2)), whiteTex, ScaleMode.StretchToFill, false, 0f, new Color(0f, 0f, 0f, 0.9f), 0f, 0f);
			int num = this.index + 1;
			for (int i = 0; i < this.count; i++)
			{
				float num2 = this.values[num % this.count];
				num2 /= this.maxValue;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				float num3 = (float)graphHeight * num2;
				num3 = (float)((int)(num3 + 0.5f));
				Color color = new Color(1f, num2, num2 * 0.6f + 0.4f);
				GUI.DrawTexture(new Rect(this.pos.x + 1f + (float)i * 2f, this.pos.y + 1f + (float)graphHeight - num3, 2f, num3), whiteTex, ScaleMode.StretchToFill, false, 0f, color, 0f, 0f);
				num++;
			}
			if (this.markerValue > 0f)
			{
				GUI.DrawTexture(new Rect(this.pos.x, this.pos.y + (float)graphHeight - this.markerValue / this.maxValue * (float)graphHeight, width, 1f), whiteTex, ScaleMode.StretchToFill, true, 0f, new Color(1f, 1f, 0f, 0.6f), 0f, 0f);
			}
			GUI.color = new Color(0.6f, 0.6f, 1f);
			GUI.Label(new Rect(this.pos.x + 1f, this.pos.y + 1f, 256f, 256f), string.Format("{0} {1}", this.name, this.values[this.index]));
		}

		// Token: 0x06008790 RID: 34704 RVA: 0x0036D854 File Offset: 0x0036BA54
		public void UpdateValues()
		{
			if (GameManager.Instance.World == null)
			{
				return;
			}
			EntityPlayerLocal player = this.manager.player;
			if (this.callback != null)
			{
				float value = this.values[this.index];
				if (this.callback(ref value))
				{
					this.AddValue(value);
					return;
				}
			}
			else if (!string.IsNullOrEmpty(this.cvarName))
			{
				float cvar = player.GetCVar(this.cvarName);
				if (cvar != this.values[this.index])
				{
					this.AddValue(cvar);
					return;
				}
			}
			else if (this.passiveEffect != PassiveEffects.None)
			{
				float value2 = EffectManager.GetValue(this.passiveEffect, null, 0f, player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				if (value2 != this.values[this.index])
				{
					this.AddValue(value2);
					return;
				}
			}
			else if (!string.IsNullOrEmpty(this.statName))
			{
				float num = 0f;
				string a = this.statName;
				if (!(a == "health"))
				{
					if (!(a == "stamina"))
					{
						if (!(a == "coretemp"))
						{
							if (a == "water")
							{
								num = player.PlayerStats.Water.Value;
							}
						}
						else
						{
							num = player.PlayerStats.CoreTemp.Value;
						}
					}
					else
					{
						num = player.PlayerStats.Stamina.Value;
					}
				}
				else
				{
					num = player.PlayerStats.Health.Value;
				}
				if (num != this.values[this.index])
				{
					this.AddValue(num);
				}
			}
		}

		// Token: 0x0400694A RID: 26954
		public string name;

		// Token: 0x0400694B RID: 26955
		public GameGraphManager.Graph.Callback callback;

		// Token: 0x0400694C RID: 26956
		public string cvarName;

		// Token: 0x0400694D RID: 26957
		public PassiveEffects passiveEffect;

		// Token: 0x0400694E RID: 26958
		public string statName;

		// Token: 0x0400694F RID: 26959
		public Vector2 pos;

		// Token: 0x04006950 RID: 26960
		[PublicizedFrom(EAccessModifier.Private)]
		public GameGraphManager manager;

		// Token: 0x04006951 RID: 26961
		[PublicizedFrom(EAccessModifier.Private)]
		public int count;

		// Token: 0x04006952 RID: 26962
		[PublicizedFrom(EAccessModifier.Private)]
		public float maxValue;

		// Token: 0x04006953 RID: 26963
		[PublicizedFrom(EAccessModifier.Private)]
		public float markerValue;

		// Token: 0x04006954 RID: 26964
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] values;

		// Token: 0x04006955 RID: 26965
		[PublicizedFrom(EAccessModifier.Private)]
		public int index;

		// Token: 0x020010D4 RID: 4308
		// (Invoke) Token: 0x06008792 RID: 34706
		public delegate bool Callback(ref float value);
	}
}
