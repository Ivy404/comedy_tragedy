using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public PlayerActions player;
    private string mode;

    // Audio stuff
    public AudioMixer audioMixer;

    //UI stuff
    public GameObject UpgradeScreen;
    public GameObject upgrade1;
    public GameObject upgrade2;
    public GameObject upgrade3;
    public UpgradeSystem upgrades;

    public GameObject HUD;
    public GameObject StartScreen;
    public GameObject PauseScreen;

    private statUpgrade upg1;
    private statUpgrade upg2;
    private statUpgrade upg3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeUION();
        Debug.Log("Game start");
        if (player != null)
        {
            mode = player.GetMode();
        }
        // Initialize the starting mode


        // Initalize the wave system

        // Start default music
        AudioManager.audioManagerRef.PlaySound("ComedyMusic");
        AudioManager.audioManagerRef.PlaySound("TragedyMusic");

        if (audioMixer != null)
        {
            audioMixer.SetFloat("VolMusic1", -80);
            audioMixer.SetFloat("VolMusic2", -80);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && mode != player.GetMode())
        {
            SwitchMode(player.GetMode());
        }
    }

    private void SwitchMode(string newMode)
    {
        // Swithc mode functionality

        // Switch music tracks
        SwitchMusicTrack(newMode);
    }

    private void SwitchMusicTrack(string newMode)
    {
        if (newMode == "comedy")
        {
            audioMixer.SetFloat("VolMusic1", -10);
            audioMixer.SetFloat("VolMusic2", -80);
        }
        else
        {
            audioMixer.SetFloat("VolMusic2", -10);
            audioMixer.SetFloat("VolMusic1", -80);
        }
    }

    //UI STUFF
    public void upgradeUION()
    {
        UpgradeScreen.SetActive(true);

        upg1 = upgrades.getRandomStatUpgrade();
        Debug.Log(upg1.maskName);
        upgrade1.GetComponent<SetUpgradeUI>().setUI(upg1.upgradeName, upg1.maskName, upg1.rarity);

        upg2 = upgrades.getRandomStatUpgrade();
        Debug.Log(upg2.maskName);
        upgrade2.GetComponent<SetUpgradeUI>().setUI(upg2.upgradeName, upg2.maskName, upg2.rarity);

        upg3 = upgrades.getRandomStatUpgrade();
        Debug.Log(upg3.maskName);
        upgrade3.GetComponent<SetUpgradeUI>().setUI(upg3.upgradeName, upg3.maskName, upg3.rarity);

    }

    public void SetUpgrade1()
    {
        player.addStatUpgrade(upg1);
    }

    public void SetUpgrade2()
    {
        player.addStatUpgrade(upg2);
    }

    public void SetUpgrade3()
    {
        player.addStatUpgrade(upg3);
    }

    public void upgradeUIOFF()
    {
        UpgradeScreen.SetActive(false);
    }

    public void StartGame()
    {
        StartScreen.SetActive(false);
        HUD.SetActive(true);
    }

    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
        HUD.SetActive(true);
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
