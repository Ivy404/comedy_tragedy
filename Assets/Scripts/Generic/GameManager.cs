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
    public UpgradeSystem upgrades;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        statUpgrade upg = upgrades.getRandomStatUpgrade();
        Debug.Log(upg.upgradeName);
    }

    public void upgradeUIOFF()
    {
        UpgradeScreen.SetActive(false);
    }
}
