using UnityEngine;

public class EndScene : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Game has quit.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
