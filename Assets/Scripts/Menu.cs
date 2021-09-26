using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public AudioManager AudioManager = null;
    public GameObject SettingsObject = null;
    public Toggle SettingsFastGame = null;
    public Toggle SettingsMuteMusic = null;
    public Toggle SettingsMuteSFX = null;
    public GameObject MenuObject = null;
    public GameObject HelpObject = null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SettingsFastGame.isOn = Settings.FastGame;
        SettingsMuteSFX.isOn = Settings.MuteSFX;
        SettingsMuteMusic.isOn = Settings.MuteMusic;
        if (Settings.MuteMusic)
        {
            AudioManager.Stop("MenuMusic");
        }
        else
        {
            AudioManager.PlayMusic("MenuMusic");
        }
    }

    public void onStartClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(1);
    }

    public void onHelpClick()
    {
        MenuObject.SetActive(false);
        HelpObject.SetActive(true);
    }

    public void onSettingsClick()
    {
        MenuObject.SetActive(false);
        SettingsObject.SetActive(true);
    }

    public void onQuitClick()
    {
        Application.Quit();
    }

    public void onSettingsBack()
    {
        SaveSettings();
        SettingsObject.SetActive(false);
        MenuObject.SetActive(true);
    }

    public void onHelpBack()
    {
        HelpObject.SetActive(false);
        MenuObject.SetActive(true);
    }

    private void SaveSettings()
    {
        Settings.FastGame = SettingsFastGame.isOn;
        Settings.MuteMusic = SettingsMuteMusic.isOn;
        Settings.MuteSFX = SettingsMuteSFX.isOn;
        if (Settings.MuteMusic)
        {
            AudioManager.Stop("MenuMusic");
        }
        else
        {
            AudioManager.PlayMusic("MenuMusic");
        }
    }
}
