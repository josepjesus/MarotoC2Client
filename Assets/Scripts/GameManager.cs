using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text playerNameText;

    public Player player;
    private int playerLife = 10;

    private int greenCount = 0;
    private int blueCount = 0;
    private int redCount = 0;

    public Text greenText, blueText, redText;

    void Start()
    {
        player = FindObjectOfType<Player>();
        DateTime getBirth = DateTime.Parse(player.Date);
        playerNameText.text = player.Nickname + " (" + HowManyYears(getBirth) + ")";
    }

   

    int HowManyYears(DateTime date)
    {
        DateTime now = DateTime.Now;
        return (now.Year - date.Year - 1) +
            (((now.Month > date.Month) ||
            ((now.Month == date.Month) && (now.Day >= date.Day))) ? 1 : 0);
    }

    public void addAtCount(int num)
    {
        if (num == 3)
        {
            greenCount++;
            Debug.Log(greenCount);
            greenText.text = greenCount.ToString();
            
        }
        else if (num == 2)
        {
            blueCount++;
            Debug.Log(blueCount);
            blueText.text = blueCount.ToString();
            
        }
        else if (num == 1)
        {
            redCount++;
            Debug.Log(redCount);
            redText.text = redCount.ToString();
        }
    }

    public void restLife(string name)
    {
        if (name == "Ball 1(Clone)")
        {
            playerLife = playerLife - 2;
        }
        else if (name == "Ball 2(Clone)")
        {
            playerLife = playerLife - 3;
        }
        else if (name == "Ball 3(Clone)")
        {
            playerLife = playerLife - 1;
        }

        if (playerLife <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        Debug.Log(playerLife);
    }
}