﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using sonil.rpgkit;

namespace sonil.Editors {
	public class WorldMapEditor : EditorWindow {

        #region Main
        private WorldMapData bmd;

		public static WorldMapEditor instance;
		public static Vector2 minsize = new Vector2(950,600);
		public static GUIStyle canvasBackground = "flow background";
		private const float GridMajorSize = 120f;

		public static WorldMapEditor ShowWindow(WorldMapData data)
		{
			if (WorldMapEditor.instance != null) 
			{
				WorldMapEditor.instance.Show ();          //Build in function: Show the EditorWindow.
                WorldMapEditor.instance.init (data);      //reset the scrollView, and assign WorldMapData
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
			return new Rect (0, 0, width - 350, height);
		}

		protected Rect GetInsCanvasSize(float width,float height)
		{
			return new Rect (width - 350, 0, 350, height);
		}     

		public void OnGUI()
		{
			DrawGUI (position.width,position.height);       //position Build in variable :  the actural window width and height
        }

		private void DrawGrid()
		{
			GL.PushMatrix();
			GL.Begin(GL.LINES);
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
			GL.Vertex(p1);  //Called between GL.Begin and GL.End
			GL.Vertex(p2);
		}

		public void DrawGUI(float width,float height) 
		{
			currentEvent = Event.current;
			MapCanvasSize= GetMapCanvasSize(width,height); //Get the MapView window size
			InsCanvasSize= GetInsCanvasSize(width,height); //Get the right helper window size

			if (currentEvent.type == EventType.Repaint) {
				WorldMapEditor.canvasBackground.Draw(MapCanvasSize, false, false, false, false);
			}
            
			Vector2 curScroll= GUI.BeginScrollView (MapCanvasSize, WorldMapScrollPosition,scrollView,true,true); //MapCanvasSize: the MapView window size. scrollView : the background texture size or MaxCanvasSize
            
            if(bmd == null) { Debug.LogError("Bmd is null");return; }

            if (bmd.background != null) {
               GUI.DrawTexture (scrollView, bmd.background);
               GUI.DrawTexture (new Rect (60, 60, 60, 60), bmd.background, ScaleMode.StretchToFill, true); // what's this line used for?
             }

            if (bmd.pathFindingMask != null) { GUI.DrawTexture(scrollView, bmd.pathFindingMask); }

			UpdateScrollPosition (curScroll);
     
			if (currentEvent.type == EventType.Repaint) {DrawGrid ();}
            GLDrawBrushCircle(); //Draw the BrushCircle before the InsCanvasSize
            Brush();
            GUI.EndScrollView ();

			GUI.BeginGroup(InsCanvasSize);
			EditorGUILayout.BeginScrollView(InsScrollPosition,GUILayout.MaxWidth(350));
            QuickToolBar();
            DetailInspectCanvas();    
			EditorGUILayout.EndScrollView ();
			GUI.EndGroup ();       
        }

		protected void OnDisable()
		{
			AssetDatabase.SaveAssets ();
		}

        #endregion

        #region Wei

        #region DetailInspectCanvas

        protected Rect detailInpsectCanvas;
        protected Rect GetDetailInspectCanvasSize(float width, float height) { return new Rect(50, 0, 300, height); }

        void DetailInspectCanvas()
        {
            detailInpsectCanvas = GetDetailInspectCanvasSize(InsCanvasSize.width, InsCanvasSize.height);
            GUILayout.BeginArea(detailInpsectCanvas);
            mode = (MapEditorMode)EditorGUILayout.EnumPopup("MapEditorMode", mode);
            EditorGUILayout.TextField("asd", "asd");
            EditorGUILayout.TextField("asd", "asd");
            NoiseTextureInspector();
            GUILayout.EndArea();
        }

        #region NoiseMap
        static float[,] noiseMap;
        static Texture2D noiseTexture;

        bool showNoiseSetting = false;
        static int powerW = 10;
        static int textureWidth = 1024;
        static int powerH = 10;
        static int textureHeight = 1024;
        static int noiseSeed = 0;
        static float noiseScale = 1;
        static int noiseOctaves = 3;
        static float noisePersistance = 0.5f;
        static float noiseLacunarity = 2.0f;
        static Vector2 noiseOffset = Vector2.zero;
        Wei.Random.NoiseGenerator.NormalizedMode noiseModel = Wei.Random.NoiseGenerator.NormalizedMode.Local;
        static int selectedToolIndex = 0;

        static Color brushColor = Color.black;
        static int brushSize = 20;

        static bool isBrush = true;

        void NoiseTextureInspector()
        {

            showNoiseSetting = EditorGUILayout.Foldout(showNoiseSetting, "NoiseMap");
            if (showNoiseSetting)
            {
                EditorGUI.BeginChangeCheck();
                powerW = EditorGUILayout.IntSlider("Width: " + textureWidth.ToString(), powerW, 1, 10);
                powerH = EditorGUILayout.IntSlider("Height: " + textureHeight.ToString(), powerH, 1, 10);
                noiseSeed = EditorGUILayout.IntSlider("Seed", noiseSeed, 0, 100);
                noiseScale = EditorGUILayout.Slider("Scale", noiseScale, 0.1f, 100.0f);
                noiseOctaves = EditorGUILayout.IntSlider("Octaves", noiseOctaves, 1, 5);
                noisePersistance = EditorGUILayout.Slider("Persistance", noisePersistance, 0.1f, 0.5f);
                noiseLacunarity = EditorGUILayout.Slider("Lacunarity", noiseLacunarity, 1.0f, 10.0f);
                noiseOffset = EditorGUILayout.Vector2Field("Offset", noiseOffset);
                //noiseModel = EditorGUILayout.EnumPopup(noiseModel, Wei.Random.NoiseGenerator.NormalizedMode)
                if (EditorGUI.EndChangeCheck())
                {
                    textureWidth = 1 << powerW;
                    textureHeight = 1 << powerH;
                    ReSetNoiseMap();
                }

                brushColor = EditorGUILayout.ColorField("BrushColor", brushColor);
                brushSize = EditorGUILayout.IntSlider("Brush Size", brushSize, 2, 100);
            }

            if (GUILayout.Button("Generator Noise Map"))
            {
                ReSetNoiseMap();
            }

            if (GUILayout.Button("Save Texture"))
            {
                Wei.Source.SourceHelper.SaveTexture(noiseTexture, "noiseTest");
            }
        }

        #endregion

        #endregion

        #region QuickToolBarGUI

        private string _IcoPpath = "Assets/Editor/Resources";
        private List<EditorToolBarIcon> toolBarObjs;
        private Dictionary<EditorToolBarIcon.BarType, Texture2D> toolBarPreviews;
        private List<EditorToolBarIcon.BarType> toolBarTyes;
        protected Rect quickToolCanvas;
        protected Rect GetQuickToolCanvasSize(float width, float height)
        {
            return new Rect(0, 0, 50, height);
        }

        private void QuickToolBar()
        {
            quickToolCanvas = GetQuickToolCanvasSize(InsCanvasSize.width, InsCanvasSize.height);
            GUILayout.BeginArea(quickToolCanvas);
            EditorGUI.BeginChangeCheck();

            selectedToolIndex = GUILayout.SelectionGrid(selectedToolIndex, GetToolBarGUIContent(), 1, GetToolBarGUIStyle());

            if (EditorGUI.EndChangeCheck())
            {
                if (toolBarTyes[selectedToolIndex] == EditorToolBarIcon.BarType.Erazer)
                {
                    showNoiseSetting = true;
                    brushColor = Color.clear;
                }
            }
            GUILayout.EndArea();
        }

        private void InitToolBarTypes()
        {
            toolBarObjs = Wei.Source.SourceHelper.GetAssetsWithScript<EditorToolBarIcon>(_IcoPpath);
            if(toolBarPreviews == null) { toolBarPreviews = new Dictionary<EditorToolBarIcon.BarType, Texture2D>(); }
            if(toolBarTyes == null) { toolBarTyes = new List<EditorToolBarIcon.BarType>(); }
            
            foreach (EditorToolBarIcon obj in toolBarObjs)
            {
                if (!toolBarPreviews.ContainsKey(obj.type))
                {           
                    //Texture2D preview = GetAssetPreviewTask(obj.gameObject);
                    Texture2D preview = Wei.Source.SourceHelper.TextureFromEditorToolBarIcon(obj);
                    if (preview != null)
                    {
                        toolBarTyes.Add(obj.type);
                        toolBarPreviews.Add(obj.type, preview);
                    }
                    else
                    {
                        Debug.LogErrorFormat("No preview for the Object {0}", obj.type.ToString());
                    }
                }else
                {
                    Debug.LogErrorFormat("There is more than one {0} over here", obj.type.ToString());
                }
            }
        }

        private Texture2D GetAssetPreviewTask(GameObject obj,int callTimes=30)
        {
            Texture2D preview;
            for (int i = 0; i < callTimes; i++) {
                preview = AssetPreview.GetAssetPreview(obj.gameObject);
                if (preview != null) { return preview; }
            }
            return null;
        }

        private GUIContent[] GetToolBarGUIContent()
        {
            if (toolBarObjs == null || toolBarPreviews ==null)
            {
                InitToolBarTypes();
            }

            int totalType = toolBarObjs.Count;
            List<GUIContent> guiContents = new List<GUIContent>();
            if (totalType == toolBarPreviews.Count)
            {   
                for (int i = 0; i < totalType; i++)
                {
                    GUIContent guiContent = new GUIContent();
                    guiContent.text = toolBarObjs[i].type.ToString();
                    Texture2D preview = toolBarPreviews[toolBarObjs[i].type];
                    if (preview != null) { guiContent.image = preview; }
                    else{ Debug.LogErrorFormat("The preview of the {0} is null", toolBarObjs[i].type.ToString()); }
                    guiContents.Add(guiContent);
                }
            }
            return guiContents.ToArray();
        }

        private GUIStyle GetToolBarGUIStyle()
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
            //guiStyle.alignment = TextAnchor.LowerCenter;
            guiStyle.imagePosition = ImagePosition.ImageOnly;
            guiStyle.fixedHeight = 45.0f;
            guiStyle.fixedWidth = 45.0f;
            return guiStyle;
        }

        #endregion

        #region Actions

        void ReSetNoiseMap()
        {
            noiseMap = Wei.Random.NoiseGenerator.GenerateNoiseMap(textureWidth, textureHeight, noiseSeed, noiseScale, noiseOctaves, noisePersistance, noiseLacunarity, noiseOffset, Wei.Random.NoiseGenerator.NormalizedMode.Local);
            noiseTexture = Wei.Generator.TextureGenerator.TextureFromHeightMap(noiseMap, 0.5f);
            bmd.pathFindingMask = noiseTexture;
        }

        void Brush()
        {
            if (!showNoiseSetting) { return; }

            var eventType = Event.current.type;

            Vector2 mouseMapCanvasPos = Event.current.mousePosition;

            if (!MapCanvasSize.Contains(mouseMapCanvasPos)) { return; }

            if (eventType == EventType.MouseDown || eventType == EventType.MouseDrag)
            {
                if (noiseTexture == null)
                {
                    if (bmd.pathFindingMask == null) { ReSetNoiseMap(); return; }
                    noiseTexture = bmd.pathFindingMask;
                }
                else
                {

                    int centerX = (int)mouseMapCanvasPos.x;
                    int centerY = (int)mouseMapCanvasPos.y;
                    centerX = centerX + (int)WorldMapScrollPosition.x;
                    centerY = centerY + (int)WorldMapScrollPosition.y;
                    centerY = textureHeight - centerY;


                    for (int y = -brushSize / 2; y < brushSize / 2; y++)
                    {
                        if (centerY + y < 0 || centerY + y >= textureHeight) { continue; }
                        for (int x = -brushSize / 2; x < brushSize / 2; x++)
                        {
                            if (centerX + x < 0 || centerX + x >= textureWidth) { continue; }
                            if (x * x + y * y <= brushSize * brushSize * 0.25f)
                            {
                                noiseTexture.SetPixel(centerX + x, centerY + y, brushColor);
                            }
                        }
                    }
                    noiseTexture.Apply();
                }
                Repaint();
            }
        }

        void GLDrawBrushCircle()
        {
            if (!showNoiseSetting) { return; }
            var eventType = Event.current.type;
            Vector2 center = Event.current.mousePosition;
            if (!MapCanvasSize.Contains(center)) { return; }

            float radius = brushSize / 2.0f;
            int subDivision = 20;
            float setp = 360 / (float)subDivision;
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(new Color(1, 0, 0, 1));
            for (int i = 0; i <= subDivision; i++)
            {
                float x = radius * Mathf.Cos(Mathf.Deg2Rad * setp * i);
                float y = radius * Mathf.Sin(Mathf.Deg2Rad * setp * i);
                Vector2 p1 = new Vector2(x, y);
                GL.Vertex(center + p1);

                float x2 = radius * Mathf.Cos(Mathf.Deg2Rad * setp * (i + 1));
                float y2 = radius * Mathf.Sin(Mathf.Deg2Rad * setp * (i + 1));
                Vector2 p2 = new Vector2(x2, y2);
                GL.Vertex(center + p2);
            }
            GL.End();
            GL.PopMatrix();
            Repaint();
        }

        #endregion

        #endregion

        //OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update.
        //private void OnInspectorUpdate(){}
    }

    public enum MapEditorMode
	{
		location,
		yewai,
	}

}