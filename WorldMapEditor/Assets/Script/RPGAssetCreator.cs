using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using sonil.rpgkit;

namespace sonil.Editors {
	public static class RPGAssetCreator {

		/// <summary>
		/// Creates a custom asset
		/// </summary>
		public static T CreateAsset<T> (string name,bool displayFilePanel) where T : ScriptableObject
		{
			if (displayFilePanel) {
				T asset = null;
				string mPath = EditorUtility.SaveFilePanelInProject (
					"Create Asset of type " + typeof(T).Name,
					"New " + name + ".asset",
					"asset", "");

				asset = CreateAsset<T> (mPath);
				return asset;
			}
			return CreateAsset<T> (name);
		}

		/// <summary>
		/// Creates a custom asset at selected Object
		/// </summary>
		public static T CreateAsset<T> (string name) where T : ScriptableObject
		{
			T asset = null;
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);

			if (path == "") {
				path = "Assets";
			} else if (System.IO.Path.GetExtension (path) != "") {
				path = path.Replace (System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + name + ".asset");
			asset = CreateAssetPath<T> (assetPathAndName);
			return asset;
		}

		/// <summary>
		/// Creates a custom asset at path
		/// </summary>
		public static T CreateAssetPath<T> (string path) where T : ScriptableObject
		{
			if (string.IsNullOrEmpty (path)) {
				return null;
			}
			T data = null;
			data = ScriptableObject.CreateInstance<T> ();
			AssetDatabase.CreateAsset (data, path);
			AssetDatabase.SaveAssets ();
			return data;
		}
	}
}
