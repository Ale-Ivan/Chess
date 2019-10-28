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

    }
}

