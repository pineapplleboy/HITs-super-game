using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuAnimation : MonoBehaviour
{
    private GameObject Player;
    private float timeAtOnePoint;

    public Vector2 firstPos;
    public Vector2 secondPos;

    private int currPos = 0;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        timeAtOnePoint += Time.deltaTime;

        if(timeAtOnePoint > 3 && Mathf.Abs(Player.transform.position.x - ((currPos == 1) ? firstPos.x : secondPos.x)) > 0.5f)
        {
            Vector2 newPosition = Vector2.Lerp(Player.transform.position, ((currPos == 1) ? firstPos : secondPos), 0.5f * Time.deltaTime);
            Player.transform.position = new Vector3(newPosition.x, newPosition.y, Player.transform.position.z);
            Player.GetComponent<Animator>().SetBool("isRunning", true);
        }

        else if(Mathf.Abs(Player.transform.position.x - ((currPos == 1) ? firstPos.x : secondPos.x)) <= 0.5f)
        {
            timeAtOnePoint = 0;
            currPos = (currPos == 1) ? 0 : 1;
            Player.transform.rotation = Quaternion.Euler(0, (currPos == 0) ? 0 : 180, 0);
            Player.GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void NewGame()
    {
        File.Delete("mainSave");
        File.Delete("mainSaveInventory");
        File.Delete("mainSaveBase");
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
