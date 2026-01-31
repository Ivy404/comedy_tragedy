using UnityEngine;

public class UiControl : MonoBehaviour
{
    public GameObject HUD;
    public GameObject StartScreen;
    public GameObject PauseScreen;

    public void StartGame()
    {
        StartScreen.SetActive(false);
        HUD.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
    }

    public void PauseGame()
    {
        PauseScreen.SetActive(true);
    }
}
