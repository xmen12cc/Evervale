using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerCamera;

    private void Start()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        playerCamera.GetComponent<PlayerCamera>().enabled = false;
    }

    public void UnPauseGame()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        playerCamera.GetComponent<PlayerCamera>().enabled = true;
    }
}
