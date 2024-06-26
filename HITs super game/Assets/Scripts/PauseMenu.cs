using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool onPause = false;
    [SerializeField] GameObject PauseMenuObj;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onPause)
            {
                Resume();
            }
            else
            {
                Stop();
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void Stop()
    {
        Time.timeScale = 0;
        onPause = true;
        PauseMenuObj.SetActive(true);
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().Save();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        onPause = false;
        PauseMenuObj.SetActive(false);
    }
}
