using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerActions player;
    public WaveManager waveManager;
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

    private InputAction upgrade1btn;
    private InputAction upgrade2btn;
    private InputAction upgrade3btn;
    private InputAction pauseBtn;

    private bool isShowingUpgrades = false;
    private bool isPaused=false;

    public GameObject healthBarCom;
    public GameObject healthbarTrag;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //upgradeUION();
        Debug.Log("Game start");
        
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

        upgrade1btn = InputSystem.actions.FindAction("UpgradeLeft");
        upgrade2btn = InputSystem.actions.FindAction("UpgradeUp");
        upgrade3btn = InputSystem.actions.FindAction("UpgradeRight");
        pauseBtn = InputSystem.actions.FindAction("Pause");

        //Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        string comhelth = player.comedyMaskData.health.ToString();
        string traghelth = player.tragedyMaskData.health.ToString();

        healthbarTrag.GetComponent<TextMeshProUGUI>().text = traghelth;
        healthBarCom.GetComponent<TextMeshProUGUI>().text = comhelth;

        if (player != null && mode != player.GetMode())
        {
            Debug.Log("wtf");
            SwitchMode(player.GetMode());
        }
        if ( isShowingUpgrades){
            if (upgrade1btn.WasPressedThisFrame())
            {
                SetUpgrade1();
            }
            if (upgrade2btn.WasPressedThisFrame())
            {
                SetUpgrade2();
            }
            if (upgrade3btn.WasPressedThisFrame())
            {
                SetUpgrade3();
            }
        }

        if (pauseBtn.WasPressedThisFrame())
        {
            if (!isPaused)
            PauseGame();
            else
            ResumeGame();
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
            audioMixer.SetFloat("VolMusic1", -8);
            audioMixer.SetFloat("VolMusic2", -80);
        }
        else
        {
            audioMixer.SetFloat("VolMusic2", -8);
            audioMixer.SetFloat("VolMusic1", -80);
        }
    }

    //UI STUFF

    //Use this to turn on UI Upgrade Screen
    public void upgradeUION()
    {
        isShowingUpgrades = true;
        Time.timeScale = 0;
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
        ResumeGame();
    }

    public void SetUpgrade2()
    {
        player.addStatUpgrade(upg2);
        ResumeGame();
    }

    public void SetUpgrade3()
    {
        player.addStatUpgrade(upg3);
        ResumeGame();
    }

    public void StartGame()
    {
        StartScreen.SetActive(false);
        Time.timeScale = 1;
        HUD.SetActive(true);
    }

    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
        HUD.SetActive(true);
        UpgradeScreen.SetActive(false);
        Time.timeScale = 1;
        if(isShowingUpgrades){
            waveManager.ContinueAfterUpgrading();
            isShowingUpgrades=false;
        }
        isPaused=false;
    }

    public void PauseGame()
    {
        isPaused=true;
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
