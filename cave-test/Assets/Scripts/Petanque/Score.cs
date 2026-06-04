using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

// Keeps track of the score of the game
public class Score : MonoBehaviour
{ 
    public TMP_Text winningTeamText;
    public TMP_Text nextTeamText;

    public TMP_Text goldScoreText;
    public TMP_Text silverScoreText;

    private List<GameObject> goldBalls = new List<GameObject>();
    private List<GameObject> silverBalls = new List<GameObject>();
    private GameObject but;

    private float shortestDistance;
    private float secondShortestDistance;
    private float thirdShortestDistance;

    private Color winningTeamDisplayColor;
    private string winningTeamDisplayText;

    private Color nextTeamDisplayColor;
    private string nextTeamDisplayText;

    private int goldScore = 0;
    private int silverScore = 0;

    private string first;
    private string second;
    private string third;
    private int actualPoints;

    private bool mustUpdateScore = false;

    public WsClient_Petanque client;

    void Start()
    {
        but = GameObject.FindWithTag("But");
    }

    void Update()
    {
        shortestDistance = 30;
        secondShortestDistance = 30;
        thirdShortestDistance = 30;
        first = "";
        second = "";
        third = "";
        actualPoints = 0;
        // Computes the distances between the balls and the but, checks the first three places
        foreach (GameObject ball in goldBalls)
        {
            float dist = Vector3.Distance(ball.transform.position, but.transform.position);
            if (dist < shortestDistance)
            {
                thirdShortestDistance = secondShortestDistance;
                secondShortestDistance = shortestDistance;
                shortestDistance = dist;
                winningTeamDisplayText = "Gold Team";
                winningTeamDisplayColor = Color.yellow;
                nextTeamDisplayText = "Silver Team";
                nextTeamDisplayColor = Color.gray;
                third = second;
                second = first;
                first = "gold";
            }
            else if (dist < secondShortestDistance)
            {
                thirdShortestDistance = secondShortestDistance;
                secondShortestDistance = dist;
                third = second;
                second = "gold";
            }
            else if (dist < thirdShortestDistance)
            {
                third = "gold";
                thirdShortestDistance = dist;
            }
        }
        foreach (GameObject ball in silverBalls)
        {
            float dist = Vector3.Distance(ball.transform.position, but.transform.position);
            if (dist < shortestDistance)
            {
                thirdShortestDistance = secondShortestDistance;
                secondShortestDistance = shortestDistance;
                shortestDistance = dist;
                winningTeamDisplayText = "Silver Team";
                winningTeamDisplayColor = Color.gray;
                nextTeamDisplayText = "Gold Team";
                nextTeamDisplayColor = Color.yellow;
                third = second;
                second = first;
                first = "silver";
            }
            else if (dist < secondShortestDistance)
            {
                thirdShortestDistance = secondShortestDistance;
                secondShortestDistance = dist;
                third = second;
                second = "silver";
            }
            else if (dist < thirdShortestDistance)
            {
                third = "silver";
                thirdShortestDistance = dist;
            }
        }
        // If no balls have been launched
        if (silverBalls.Count == 0 && goldBalls.Count == 0)
        {
            winningTeamText.text = "Aucune";
            winningTeamText.color = Color.white;
            nextTeamText.text = "N'importe";
            nextTeamText.color = Color.white;
        }
        else
        {
            // Checks points
            if (first == second && silverBalls.Count + goldBalls.Count > 1)
            {
                actualPoints = 2;
                if (first == third && silverBalls.Count + goldBalls.Count > 2)
                {
                    actualPoints = 3;
                }
            }
            else 
            {
                actualPoints = 1;
            }
            winningTeamText.text = winningTeamDisplayText + " (" + actualPoints + ")";
            winningTeamText.color = winningTeamDisplayColor;
            // Checks who's the next team that must play
            if (silverBalls.Count == 3 && goldBalls.Count == 3)
            {
                nextTeamText.text = "Aucune";
                nextTeamText.color = Color.white;
            }
            else if (silverBalls.Count == 3)
            {
                nextTeamText.text = "Gold Team";
                nextTeamText.color = Color.yellow;
            }
            else if (goldBalls.Count == 3)
            {
                nextTeamText.text = "Silver Team";
                nextTeamText.color = Color.grey;
            }
            else
            {
                nextTeamText.text = nextTeamDisplayText;
                nextTeamText.color = nextTeamDisplayColor;
            }
        }

        // Updates the score
        if (mustUpdateScore)
        {
            mustUpdateScore = false;
            goldScoreText.text = goldScore.ToString();
            silverScoreText.text = silverScore.ToString();
        }
    }

    // Computes the new score
    public void NewScore()
    {
        if (first == "gold")
        {
            goldScore++;
            if (second == "gold")
            {
                goldScore++;
                if (third == "gold")
                {
                    goldScore++;
                }
            }
        }
        else if (first == "silver")
        {
            silverScore++;
            if (second == "silver")
            {
                silverScore++;
                if (third == "silver")
                {
                    silverScore++;
                }
            }
        }
        client.SendScore(silverScore, goldScore);
        Debug.Log(silverScore + ", " + goldScore);
    }

    // Updates the score
    public void UpdateScore(int silver, int gold)
    {
        silverScore = silver;
        goldScore = gold;
        mustUpdateScore = true;
    }

    // Add a new ball to a list when in contact with the floor
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "GoldBall" && !goldBalls.Contains(other.gameObject))
        {
            goldBalls.Add(other.gameObject);
        }
        else if (other.tag == "SilverBall" && !silverBalls.Contains(other.gameObject))
        {
            silverBalls.Add(other.gameObject);
        }
    }
    
    // Removes a ball from a list when in contact with the floor
    private void OnTriggerExit(Collider other) {
        if (other.tag == "GoldBall")
        {
            goldBalls.Remove(other.gameObject);
        }
        else if (other.tag == "SilverBall")
        {
            silverBalls.Remove(other.gameObject);
        }
    }
}
