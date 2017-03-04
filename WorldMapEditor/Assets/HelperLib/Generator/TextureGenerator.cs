using UnityEngine;

namespace Wei.Generator
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);//get the first parameter
            int height = heightMap.GetLength(1);//get the second parameter

            //the colorMap is a one D array, but the noiseMap is a 2 D array.
            Color[] colorMap = new Color[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // y*width we get the row, + x then we get the col
                    //Lerp : return (b-a)*t +a;
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }
            return TextureFromColorMap(colorMap, width, height);
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap,float alpha)
        {
            int width = heightMap.GetLength(0);//get the first parameter
            int height = heightMap.GetLength(1);//get the second parameter

            //the colorMap is a one D array, but the noiseMap is a 2 D array.
            Color[] colorMap = new Color[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // y*width we get the row, + x then we get the col
                    //Lerp : return (b-a)*t +a;
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                    colorMap[y * width + x].a = alpha;
                }
            }
            return TextureFromColorMap(colorMap, width, height);
        }
    }
}
