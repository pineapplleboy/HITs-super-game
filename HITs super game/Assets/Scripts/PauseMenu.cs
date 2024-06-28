using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool onPause = false;
    [SerializeField] GameObject PauseMenuObj;

    private void Start()
    {
        PermanentStatsBoost.Load();
    }

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
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().Save();
        GameObject.Find("BaseManager").GetComponent<BaseManagement>().Save();
        GameObject.Find("MainCanvas").GetComponent<InventoryManager>().Save();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Save();
        PermanentStatsBoost.Save();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Stop()
    {
        Time.timeScale = 0;
        onPause = true;
        PauseMenuObj.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        onPause = false;
        PauseMenuObj.SetActive(false);
    }
}
