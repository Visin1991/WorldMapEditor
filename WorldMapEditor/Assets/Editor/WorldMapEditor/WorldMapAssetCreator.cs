using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using sonil.rpgkit;

namespace sonil.Editors {
	public static class WorldMapAssetCreator {
		[MenuItem ("Assets/sonil's rpg kit/Create/WorldMapAsset")]
		public static void CreateWorldMapAsset ()
		{
			WorldMapData d = RPGAssetCreator.CreateAsset<WorldMapData> ("WorldMapAsset", false);
			AssetDatabase.SaveAssets ();
		}
	}
}
