using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using sonil.rpgkit;

namespace sonil.Editors {
	[CustomEditor(typeof(WorldMapData),true)]
	public class WorldMapInspector : Editor {
		public void OnEnable() {
			EditorApplication.projectWindowItemOnGUI += OnDoubleClick;
		}

		public void  OnDisable() {
			EditorApplication.projectWindowItemOnGUI -= OnDoubleClick;
		}

		public override void OnInspectorGUI (){
			base.OnInspectorGUI ();
		}

		protected override void OnHeaderGUI (){
			base.OnHeaderGUI ();
		}

		public virtual void OnDoubleClick(string guid,Rect rect){
			if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 
				&& rect.Contains (Event.current.mousePosition)) {
				WorldMapEditor.ShowWindow((WorldMapData)target);
			}
		}
	}
}