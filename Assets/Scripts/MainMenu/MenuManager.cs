using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject xwing;
    public GameStats gameStats;
    public SpaceShipStats shipStats;
    private TextMeshProUGUI[] stats;
    private int scene;
    private GameObject[] statsObjects;
    void Start()
    {
        scene = SceneManager.GetActiveScene().buildIndex;
        if (scene == 0)
        {

        }
        else if (scene == 2 || scene == 1)
        {
            statsObjects = GameObject.FindGameObjectsWithTag("Stats");
            stats  = new TextMeshProUGUI[statsObjects.Length];
            UpdateStats();
            if(scene == 2 && !gameStats.win)
            {
                GameObject.Find("WinLose").GetComponent<TextMeshProUGUI>().text = "Game Over";
            }    
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
    public void Demo()
    {
        SceneManager.LoadScene(3);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void UpdateStats()
    {
        for (int i = 0; i < statsObjects.Length; i++)
        {
            stats[i] = statsObjects[i].GetComponent<TextMeshProUGUI>();
            switch (statsObjects[i].name)
            {
                case "Score":
                    stats[i].text = "Score: " + gameStats.score;
                    break;
                case "Rings":
                    if(scene == 1)
                    {
                        stats[i].text = "Rings: " + gameStats.rings;
                    }
                    else
                    {
                        stats[i].text = "Rings: " + (gameStats.maxRings - gameStats.rings) + "/" + gameStats.maxRings;
                    }    
                    break;
                case "Teseracts":
                    stats[i].text = "Teseracts: " + (gameStats.maxTeseracts - gameStats.teseracts) + "/" + gameStats.maxTeseracts;
                    break;
                case "TimeLeft":
                    stats[i].text = "Time Left: " + TimeSpan.FromSeconds(gameStats.time).ToString(@"mm\:ss");
                    break;
                case "Velocity":
                    stats[i].text = MathF.Round(shipStats.linearVelocity.magnitude * 3.6f) * 10 + " K/h";
                    break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (scene == 0 || scene == 2)
        {
            xwing.transform.Rotate(0, -50f * Time.deltaTime, 0);
        }
        else if (scene == 1)
        {
            UpdateStats();
        }
        
    }
}
