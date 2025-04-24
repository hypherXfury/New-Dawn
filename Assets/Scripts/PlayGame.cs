using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    public GameObject UIAnim;
    public GameObject UIText;
    public GameObject PauseMenuUI;

    void Start()
    {
        PauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            PauseMenuUI.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        PauseMenuUI.SetActive(false);
    }
    public void PlayTheGame()
    {
        UIAnim.SetActive(true);
        StartCoroutine(PlayNextLevel());
    }

    IEnumerator PlayNextLevel()
    {
        yield return new WaitForSeconds(3f);
        UIText.SetActive(true);
    }

    public void MainLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }
}
