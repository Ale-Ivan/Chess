using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //for loading a scene

namespace com.Ale.Chess
{
    public class Utils : MonoBehaviour
    {
        public void SceneLoader(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        static public Vector3 PointFromGrid(Vector2Int gridPoint)
        {
            float x = 0.01f * gridPoint.x;
            float z = 0.01f * gridPoint.y;
            return new Vector3(x, -0.1f, z);
        }

        static public Vector2Int GridPoint(int col, int row)
        {
            return new Vector2Int(col, row);
        }

        static public Vector2Int GridFromPoint(Vector3 point)
        {
            int col = Mathf.FloorToInt(point.x);
            int row = Mathf.FloorToInt(point.z);
            return new Vector2Int(col, row);
        }

    }
}

