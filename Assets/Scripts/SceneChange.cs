using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChange : MonoBehaviour
{
    // Start is called before the first frame update
   public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
