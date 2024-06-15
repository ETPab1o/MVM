using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button restartButton;
    public Button resetSpawnPointsButton;
    public Button quitButton;
    public Toggle musicToggle;

    private bool isPaused = false;
    private AudioSource backgroundMusic;

    void Start()
    {
        // Add listeners to buttons
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        resetSpawnPointsButton.onClick.AddListener(ResetSpawnPoints);
        quitButton.onClick.AddListener(QuitGame);
        musicToggle.onValueChanged.AddListener(ToggleMusic);

        // Ensure pause menu is inactive at start
        pauseMenuUI.SetActive(false);

        // Get the background music AudioSource
        backgroundMusic = FindObjectOfType<AudioSource>();

        // Initialize music toggle state
        if (backgroundMusic != null)
        {
            musicToggle.isOn = backgroundMusic.isPlaying;
        }
    }

    void Update()
    {
        // Check for pause input (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
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
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;  // Ensure time scale is reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetSpawnPoints()
    {
        PlayerPrefs.DeleteKey("SpawnPointX");
        PlayerPrefs.DeleteKey("SpawnPointY");
        PlayerPrefs.DeleteKey("SpawnPointZ");
        // Optionally, add code to update the player's spawn point immediately
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.SetSpawnPoint(playerController.initialSpawnPoint);
        }
        Resume();  // Resume game after resetting spawn points
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleMusic(bool isOn)
    {
        if (backgroundMusic != null)
        {
            if (isOn)
            {
                backgroundMusic.Play();
            }
            else
            {
                backgroundMusic.Pause();
            }
        }
    }
}
