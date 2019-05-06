using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    #region Unity_functions
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        AudioManager audio = FindObjectOfType<AudioManager>();

        if (SceneManager.GetActiveScene().name.Equals("ReleaseLevel1") || SceneManager.GetActiveScene().name.Equals("ReleaseIntroScene"))
        {
            audio.Stop("TitleMusic");
            audio.Play("GameMusic");
        }
        else
        {
            audio.Stop("GameMusic");
            audio.Play("TitleMusic");
        }
    }
    #endregion

    #region scene_transitions
    public void StartGame()
    {
        SceneManager.LoadScene("ReleaseIntroScene");
        AudioManager audio = FindObjectOfType<AudioManager>();
        if (!audio.IsPlaying("GameMusic"))
        {
            audio.Stop("TitleMusic");
            audio.Play("GameMusic");
        }
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene("ReleaseLevel1");
    }

    public void Instructions()
    {
        SceneManager.LoadScene("ReleaseInstructionsScene");
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("ReleaseCreditsScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("ReleaseTitleScene");
        AudioManager audio = FindObjectOfType<AudioManager>();
        if (!audio.IsPlaying("TitleMusic"))
        {
            audio.Stop("GameMusic");
            audio.Play("TitleMusic");
        }
    }

    public void GoToEndScene()
    {
        SceneManager.LoadScene("ReleaseEndScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region end_game_functions
    public void SceneRestartOnDeath(float transitionTime)
    {
        StartCoroutine(SceneRestartRoutine(transitionTime));
    }

    IEnumerator SceneRestartRoutine(float transitionTime)
    {
        float currTime = 0f;

        while (currTime <= transitionTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

}
