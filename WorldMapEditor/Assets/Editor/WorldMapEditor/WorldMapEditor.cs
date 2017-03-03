using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using sonil.rpgkit;

namespace sonil.Editors {
	public class WorldMapEditor : EditorWindow {
		private WorldMapData bmd;

		public static WorldMapEditor instance;
		public static Vector2 minsize = new Vector2(900,600);
		public static GUIStyle canvasBackground = "flow background";
		private const float GridMajorSize = 120f;

		public static WorldMapEditor ShowWindow(WorldMapData data)
		{
			if (WorldMapEditor.instance != null) 
			{
				WorldMapEditor.instance.Show ();
				WorldMapEditor.instance.init (data);
				return WorldMapEditor.instance;
			}
				
			WorldMapEditor.instance = EditorWindow.GetWindow<WorldMapEditor>("WorldMapEditor");
			WorldMapEditor.instance.init (data);
			WorldMapEditor.instance.minSize = WorldMapEditor.minsize;
			return WorldMapEditor.instance;
		}

		protected Rect MapCanvasSize;
		protected Rect InsCanvasSize;
		MapEditorMode mode = MapEditorMode.location;
		protected Vector2 mousePosition;
		protected Event currentEvent;
		protected Vector2 WorldMapScrollPosition;
		protected Vector2 InsScrollPosition;
		private Rect scrollView;
		private Rect worldViewRect;
		private Vector2 offset;
		public const float MaxCanvasSize = 1200f;

		public void init(WorldMapData data)
		{
			bmd = data;

			if (bmd.background == null) {
				this.scrollView = new Rect (0, 0, MaxCanvasSize, MaxCanvasSize);
			} else {
				this.scrollView = new Rect (0, 0, bmd.background.width, bmd.background.height);
			}
		}

		protected void UpdateScrollPosition(Vector2 position)
		{
			offset = offset + (WorldMapScrollPosition- position);
			WorldMapScrollPosition = position;
			worldViewRect = new Rect(this.MapCanvasSize);
			worldViewRect.y +=  WorldMapScrollPosition.y;
			worldViewRect.x += WorldMapScrollPosition.x;
		}

		protected Rect GetMapCanvasSize(float width,float height)
		{
			return new Rect (0, 0, width - 300, height);
		}

		protected Rect GetInsCanvasSize(float width,float height)
		{
			return new Rect (width - 300, 0, 300, height);
		}

		public void OnGUI()
		{
			DrawGUI (position.width,position.height);
		}

		private void DrawGrid()
		{
			GL.PushMatrix();
			GL.Begin(1);
			DrawGridLines(scrollView,WorldMapEditor.GridMajorSize,Vector2.zero, NodeStyles.gridMajorColor);
			GL.End();
			GL.PopMatrix();
		}

		private void DrawGridLines(Rect rect,float gridSize,Vector2 offset, Color gridColor)
		{
			GL.Color(gridColor);
			for (float i = rect.x+(offset.x<0f?gridSize:0f) + offset.x % gridSize ; i < rect.x + rect.width; i = i + gridSize)
			{
				this.DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.y + rect.height));
			}
			for (float j = rect.y+(offset.y<0f?gridSize:0f) + offset.y % gridSize; j < rect.y + rect.height; j = j + gridSize)
			{
				this.DrawLine(new Vector2(rect.x, j), new Vector2(rect.x + rect.width, j));
			}
		}

		private void DrawLine(Vector2 p1, Vector2 p2)
		{
			GL.Vertex(p1);
			GL.Vertex(p2);
		}

		public void DrawGUI(float width,float height) 
		{
			currentEvent = Event.current;
			MapCanvasSize= GetMapCanvasSize(width,height);
			InsCanvasSize= GetInsCanvasSize(width,height);

			if (currentEvent.type == EventType.Repaint) {
				WorldMapEditor.canvasBackground.Draw(MapCanvasSize, false, false, false, false);
			}

			Vector2 curScroll= GUI.BeginScrollView (MapCanvasSize, WorldMapScrollPosition,scrollView,true,true);

			if (bmd.background != null) {
				GUI.DrawTexture (scrollView, bmd.background);
				GUI.DrawTexture (new Rect (60, 60, 60, 60), bmd.background, ScaleMode.StretchToFill, true);
			}
			UpdateScrollPosition (curScroll);

			if (currentEvent.type == EventType.Repaint) {
				DrawGrid ();
			}

			mousePosition = currentEvent.mousePosition;
			GUI.EndScrollView ();

			GUI.BeginGroup (InsCanvasSize);
			EditorGUILayout.BeginScrollView (InsScrollPosition,GUILayout.MaxWidth(300));
			mode = (MapEditorMode)EditorGUILayout.EnumPopup ("MapEditorMode",mode);
			EditorGUILayout.TextField ("asd", "asd");
			EditorGUILayout.TextField ("asd", "asd");
			EditorGUILayout.EndScrollView ();
			GUI.EndGroup ();
		}

		protected void OnDisable()
		{
			AssetDatabase.SaveAssets ();
		}
	}

	public enum MapEditorMode
	{
		location,
		yewai,
	}
}