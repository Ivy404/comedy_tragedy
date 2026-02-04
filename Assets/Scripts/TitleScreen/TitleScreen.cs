using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Canvas tScreen;
    public void onButtonExit()
    {
        Application.Quit();
    }

    public void onButtonPlay()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void videoEnded()
    {
        tScreen.gameObject.SetActive(true);
    }
}
