using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Wei.Source
{
    public static class SourceHelper
    {

        static string path
        {
            get
            {
                Directory.CreateDirectory(Application.dataPath + "\\Resources\\Textures\\");
                return Application.dataPath + "\\Resources\\Textures\\";
            }
        }

        public static void SaveTexture(Texture2D texture, string textureName)
        {
            byte[] bytes = texture.EncodeToJPG();

            //1 .Open File stream and Writer
            FileStream stream = new FileStream(path + textureName + ".png", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            Debug.Log("Writting a File");
            //3. Write
            for (int i = 0; i < bytes.Length; i++)
            {
                writer.Write(bytes[i]);
            }

            //3.Close
            writer.Close();
            stream.Close();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static List<T> GetListFromEnum<T>()
        {
            List<T> enumList = new List<T>();
            System.Array enums = System.Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                enumList.Add(e);
            }
            return enumList;
        }

        public static List<T> GetAssetsWithScript<T>(string path) where T : MonoBehaviour
        {
            T tmp;
            string assetPath;
            GameObject asset;

            List<T> assetList = new List<T>();
#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new string[] { path });

            for (int i = 0; i < guids.Length; i++)
            {
                assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                tmp = asset.GetComponent<T>();
                if (tmp != null)
                {
                    assetList.Add(tmp);
                }
            }
#endif
            return assetList;
        }

        public static Texture2D TextureFromEditorToolBarIcon(EditorToolBarIcon obj)
        {
            return obj.preview;
        }
    }
}
