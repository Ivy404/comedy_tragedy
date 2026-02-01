using UnityEngine;

public class UiControl : MonoBehaviour
{
    public GameObject HUD;
    public GameObject StartScreen;
    public GameObject PauseScreen;

    public GameObject UpgradeScreen;


    public void StartGame()
    {
        StartScreen.SetActive(false);
        HUD.SetActive(true);
    }

    private void Update()
    {

    }

    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
    }

    public void PauseGame()
    {
        PauseScreen.SetActive(true);
    }

    public void TriggerUpgrades()
    {
        UpgradeScreen.SetActive(true);
    }
}
