using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ObjectFallDetector : MonoBehaviour
{
    private int score = 0;

    public TMP_Text scoreText; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedCube")
        {
            score++;
            scoreText.text = score.ToString();
        }
        else if(other.tag == "GreenCube")
        {
            score += 2;
            scoreText.text = score.ToString();
        }
        else if(other.tag == "BlueCube")
        {
            score += 3;
            scoreText.text = score.ToString();
        }
    }
}
