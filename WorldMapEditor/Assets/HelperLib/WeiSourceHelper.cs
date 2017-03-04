using System.IO;
using UnityEngine;

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
            Debug.Log("Fnished");
        }
    }
}
