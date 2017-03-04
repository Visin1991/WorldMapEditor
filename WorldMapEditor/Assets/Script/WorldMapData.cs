using UnityEngine;
using System.IO;

namespace sonil.rpgkit {
	[System.Serializable]
	public class WorldMapData : ScriptableObject {
		public Texture2D background;
        public Texture2D pathFindingMask;
	}
}
