using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wei.Random
{
    public static class NoiseGenerator
    {

        public enum NormalizedMode { Local, Global }

        /// <summary>
        /// GenerateNoiseMap by some random math
        /// </summary>
        /// <param name="mapWidth">the map's width by pixels</param>
        /// <param name="mapHeight">the map's height by pixels</param>
        /// <param name="seed"> randmom seed </param>
        /// <param name="scale">the scale of the NoiseMap</param>
        /// <param name="octaves">the stack for the Noise may. we add all stack of the NoiseMap together make more detail for the NoiseMap</param>
        /// <param name="persistance">Change the amplitude for each stack,if the first stack's possible max Height is 1. then the next stack possible height will be persistance, the thrid statck persistance*persistance, the total possible max Height will be 1 +  persistance + persistance * persistance</param>
        /// <param name="lacunarity">Change the frequency for each stack</param>
        /// <param name="offset"></param>
        /// <param name="normalizedMode"></param>
        /// <returns></returns>
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizedMode normalizedMode)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            if (scale == 0)
            {
                scale = 0.0001f;
            }

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves]; // because now octaves represent the total of the Vector2, so It should be a passitive int or z
            float maxPossibleHeight = 0;
            float amplitude = 1; //up down value
            float frequency = 1; //
            for (int j = 0; j < octaves; ++j)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets[j] = new Vector2(offsetX, offsetY);
                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {

                    amplitude = 1; //up down value
                    frequency = 1; //
                    float noiseHeight = 0;

                    // x - halfWidth and y - halfHeight means move the origin point to the center of the map when we scale the map 
                    // when we scale we move to the origiin to the center, then we zoom in or zoom out.
                    for (int i = 0; i < octaves; ++i)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency * 1.000003f; //just wanna make sure sampleX and sampleY not intger all the time.
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency * 1.000003f;
                        //the perlinValue is the greyscale values represent values from 0..1
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;  // now the value become [-1,1]
                                                                                          //Debug.Log(perlinValue);

                        //octaves 1, main outline. octaves 2,boulders. octaves 3 small rocks.
                        noiseHeight += perlinValue * amplitude; // here those three stack overleped.
                                                                // the first noiseHeight will be perlinValue, the second noisHeight will be (noiseHeight + noiseHeight * persistance)...(noiseHeight + noiseHeight * persistance^2);
                        amplitude *= persistance; //a1 =p^0; a2 = p^1; a3 = p^2; change the undown amplitude value of each stack
                        frequency *= lacunarity; //f1 = l^0; f2 = l^1; f3 = l^2; change the frequency for each stack.
                    }

                    // When an set numbers is not increase order, or decrease order.
                    // and we wanta get the min and max....we can use this mothed.
                    //   Max)------------------------->
                    //           noiseHeight
                    //<------------------------(Min
                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {
                    //if noiseMap[x,y] = minNoiseHeight, then return 0; if...... return 1;
                    //before the range of noiseMap[x,y] is [minNoiseHeight,MaxNoiseHeight].
                    //now we covert the range to [0,1] based on the origin noiseMap[x,y]
                    if (normalizedMode == NormalizedMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                    else
                    {
                        float normalizeHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                    }
                }
            }
            return noiseMap;
        }
    }
}
