using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public void LoadScene(string inputId)
    {
        SceneManager.LoadScene(inputId);
    }
}
