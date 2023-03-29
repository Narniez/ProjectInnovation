using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour
{
    public Texture2D map;

    public GameObject cube;
    void Start()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y= 0; y < map.height; y++)
            {
                Color currentPixelColor = map.GetPixel(x, y);
                Debug.Log("asdasdsa");
                if (currentPixelColor == Color.black) {
                    Instantiate(cube, new Vector3(x,0,y), Quaternion.identity);
                }
            }
        }
    }
}
