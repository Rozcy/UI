using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject SettingMenuUI;
    private bool isSettingsActive = false;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider masterSlider;

    public AudioSource musicSource;
    public AudioSource soundSource;

    public Button fullscreenButton;
    public Animator fullscreenButtonAnimator;
    public string normalOnTrigger = "Normal_on";
    public string normalOffTrigger = "Normal_off";
    public string highlightedTrigger = "Highlighted";
    public string pressedTrigger = "Pressed";

    private bool isFullscreen;
    private bool isAnimating;

    void Start()
    {
        // Load saved settings when the game starts
        LoadSettings();

        // Add listeners to the master volume slider
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                if (!isSettingsActive)
                    Resume();
                else
                    CloseSettingsMenu();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OpenSettingsnMenu()
    {
        SettingMenuUI.SetActive(true);
        isSettingsActive = true;

        // Set the slider values based on the current volume settings
        musicSlider.value = musicSlider.value;
        soundSlider.value = soundSlider.value;
        masterSlider.value = masterSlider.value;

        // Set the fullscreen button state based on the current setting
        fullscreenButtonAnimator.SetTrigger(isFullscreen ? normalOnTrigger : normalOffTrigger);
    }

    public void CloseSettingsMenu()
    {
        SettingMenuUI.SetActive(false);
        isSettingsActive = false;

        // Save settings when the settings menu is closed
        SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume * masterSlider.value;
    }

    public void SetSoundVolume(float volume)
    {
        soundSource.volume = volume * masterSlider.value;
    }

    public void SetMasterVolume(float volume)
    {
        //musicSource.volume = musicVolume * volume;
        //soundSource.volume = soundVolume * volume;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating)
            fullscreenButtonAnimator.SetTrigger(isFullscreen ? normalOnTrigger : normalOffTrigger);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            fullscreenButtonAnimator.SetTrigger(pressedTrigger);
            Invoke("ToggleFullscreen", fullscreenButtonAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;

        // Save the fullscreen setting
        SaveSettings();

        // Trigger the appropriate normal state animation
        fullscreenButtonAnimator.SetTrigger(isFullscreen ? normalOnTrigger : normalOffTrigger);
    }

    void SaveSettings()
    {
        // Save the current settings to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // Load the saved settings from PlayerPrefs
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;

        // Apply the loaded settings
        SetMusicVolume(musicSlider.value);
        SetSoundVolume(soundSlider.value);
        SetMasterVolume(masterSlider.value);
        Screen.fullScreen = isFullscreen;

        // Set the fullscreen button state based on the saved setting
        fullscreenButtonAnimator.SetTrigger(isFullscreen ? normalOnTrigger : normalOffTrigger);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}