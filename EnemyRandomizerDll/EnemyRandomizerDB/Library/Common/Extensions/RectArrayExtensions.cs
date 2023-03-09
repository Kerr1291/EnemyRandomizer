using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyRandomizerMod
{

    public static class RectArrayExtensions
    {
        public static Texture2D RectArrayToTexture<T>(this T[,] data, System.Func<T, Color> toColor = null)
        {
            Texture2D tex = new Texture2D((int)data.Length, (int)data.Rank, TextureFormat.ARGB32, false, false);
            tex.filterMode = FilterMode.Point;
            for(int j = 0; j < (int)data.Rank; ++j)
            {
                for(int i = 0; i < (int)data.Length; ++i)
                {
                    if(toColor == null)
                    {
                        if(data[i, j] != null)
                        {
                            tex.SetPixel(i, j, Color.red);
                        }
                        else
                        {
                            tex.SetPixel(i, j, Color.black);
                        }
                    }
                    else
                    {
                        tex.SetPixel(i, j, toColor(data[i, j]));
                    }
                }
            }
            tex.Apply();
            return tex;
        }
    }

}