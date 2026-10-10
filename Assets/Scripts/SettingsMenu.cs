using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : ManagedUI
{

    [SerializeField] private AudioMixer audio;
    [SerializeField] private TMPro.TMP_Dropdown resDropdown;
    [SerializeField] private Button back;

    private Resolution[] resolutions;



    void Start()
    {
        // Loads save/settings/other data

        // Display resolution settings
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> options = new List<string>();

        int curRes = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            Debug.Log(resolutions[i] + ", " + Screen.currentResolution + ", " + (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height));

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                curRes = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = curRes;
        resDropdown.RefreshShownValue();
        
    }

    public void SetMasterVolume(float volume)
    {
        audio.SetFloat("MasterVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audio.SetFloat("SfxVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audio.SetFloat("MusicVolume", volume);
    }

    public void SetGraphicsLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public override void Initialize()
    {
        back.onClick.AddListener(() => UIManager.ShowLast());
    }
}
