using UnityEngine;
using WebSocketSharp;
using System;
using System.Globalization;
using System.Collections.Generic; 
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Manages the connexion between the client and the Node.js server
public class WsClient_Petanque : MonoBehaviour
{
    public GameObject secondBalls;

    public Score score;

    private Transform[] petanqueBall = new Transform[13];

    private List<Vector3>[] lastBallPosition = new List<Vector3>[13];
    private List<float>[] time = new List<float>[13];
    private Vector3[] ballVelocity = new Vector3[13];

    private bool[] isGrabbed = new bool[13]; 

    private WebSocket ws;

    private bool[] positionUpdated = new bool[13];
    private Vector3[] newPos = new Vector3[13];

    private bool[] owner = new bool[13];

    private bool[] isGrabbing = new bool[13];

    private XRGrabInteractable[] ballInteractable = new XRGrabInteractable[13];

    private int id = 0;

    private int ballCount = 7;

    private int server_message_id = 0;

    // The fonction called when the client is trying to connect to Node.js
    public void StartConnexionToNodeJs(string ip)
    {
        ws = new WebSocket("ws://" + ip + ":8080");
        ws.Connect();
        // Add the callback when messages are received
        ws.OnMessage += (sender, e) =>
        {
            string[] message = e.Data.Split(";");
            // Received when a ball has been grabbed
            if (message[0] == "grabbed")
            {
                int ball_id = Int32.Parse(message[1]);
                isGrabbed[ball_id] = true;
                // The interactions are blocked for other players
                if (!isGrabbing[ball_id])
                {
                    ballInteractable[ball_id].enabled = false;
                    owner[ball_id] = false;
                }
            }
            // Received when a ball has been released, the interactions are reactivated
            else if (message[0] == "released")
            {
                int ball_id = Int32.Parse(message[1]);
                isGrabbed[ball_id] = false;
                ballInteractable[ball_id].enabled = true;
            }
            // When a fourth player has joined, 6 more balls need to be added
            else if (message[0] == "more_balls")
            {
                ballCount = 13;
                secondBalls.SetActive(true);
            }
            // Update the score displayed
            else if (message[0] == "update_score")
            {
                score.UpdateScore(Int32.Parse(message[1]), Int32.Parse(message[2]));
            }
            // Received the new ball positions
            else if (Int32.Parse(message[0]) > server_message_id)
            {
                server_message_id = Int32.Parse(message[0]);
                for (int i = 0; i < ballCount; i++)
                {
                    // A ball is not updated for a player grabbing it
                    if (!isGrabbed[i] || (isGrabbed[i] && !owner[i]))
                    {
                        positionUpdated[i] = true;
                        newPos[i] = new Vector3(float.Parse(message[3*i + 1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[3*i + 2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[3*i + 3], CultureInfo.InvariantCulture.NumberFormat));
                    }
                }
            }
        };

        // Initialisation the lists for the ball velocity on releasing
        for (int i = 0; i < 13; i++)
        {
            lastBallPosition[i] = new List<Vector3>();
            time[i] = new List<float>();
        }

        // If the player is the first to join, they sends the balls to the node.js server
        if (GameObject.FindGameObjectsWithTag("Player").Length == 1)
        {
            for (int i = 0; i < 13; i++)
            {
                owner[i] = true;
            }
            for (int i = 0; i < 7; i++)
            {
                ws.Send("add_ball;" + i + ";" + petanqueBall[i].position.x + ";" + petanqueBall[i].position.y + ";" + petanqueBall[i].position.z);
            }
        }
        // If the player is the fourth, more balls are added
        else if (GameObject.FindGameObjectsWithTag("Player").Length == 4)
        {
            ws.Send("more_balls");
        }
    }

    private void FixedUpdate()
    {
        // The ball positions are updated
        for (int i = 0; i < ballCount; i++)
        {
            if (positionUpdated[i])
            {
                positionUpdated[i] = false;
                petanqueBall[i].position = newPos[i];
            }
        }
    }

    private void Update() {
        // If a ball is grabbed, its velocity is computed with the last ten frames and the ball position is send by the player to the server
        for (int i = 0; i < ballCount; i++)
        {
            if (isGrabbing[i])
            {
                if (time[i].Count == 10)
                {
                    time[i].RemoveAt(0);
                }
                time[i].Add(Time.deltaTime);
                float sampleTime = 0.0f;
                foreach (float t in time[i])
                {
                    sampleTime += t;
                }
                if (lastBallPosition[i].Count != 0)
                {
                    ballVelocity[i] = (petanqueBall[i].position - lastBallPosition[i][0]) / sampleTime;
                }
                if (lastBallPosition[i].Count == 10)
                {
                    lastBallPosition[i].RemoveAt(0);
                }
                lastBallPosition[i].Add(petanqueBall[i].position);
                ws.Send("positions_grabbed;" + id + ";" + petanqueBall[i].position.x + ";" + petanqueBall[i].position.y + ";" + petanqueBall[i].position.z + ";" + i);
                id++;
            }
        }
    }

    // Called when a ball is grabbed, sends the information to the server
    public void Grabbed(int ball_id)
    {
        isGrabbing[ball_id] = true;
        ws.Send("grabbed;" + id + ";" + ball_id);
        id++;
        owner[ball_id] = true;
    }

    // Called when a ball is released, sends the information and the velocity to the server
    public void Released(int ball_id)
    {
        isGrabbing[ball_id] = false;
        ws.Send("released;" + id + ";" + ballVelocity[ball_id].x + ";" + ballVelocity[ball_id].y + ";" + ballVelocity[ball_id].z + ";" + ball_id);
        time[ball_id].Clear();
        lastBallPosition[ball_id].Clear();
        id++;
    }

    // Called at the beginning, add a ball transform and grab component to their list
    public void AddBall(int ball_id, Transform transform, XRGrabInteractable grabComponent)
    {
        petanqueBall[ball_id] = transform;
        ballInteractable[ball_id] = grabComponent;
        // If more balls need to be added, the new balls are sent to the server
        if (ballCount == 13 && owner[ball_id])
        {
            ws.Send("add_ball;" + ball_id + ";" + petanqueBall[ball_id].position.x + ";" + petanqueBall[ball_id].position.y + ";" + petanqueBall[ball_id].position.z);
        }
    }

    // Called when someone wants to reset positions or when a next round begins, reset the balls to their positions on the table
    public void ResetBalls()
    {
        for (int i = 0; i < ballCount; i++)
        {
            ws.Send("reset_ball;" + id + ";" + i);
        }
    }

    // Sends the new score to the other by the server, 
    public void SendScore(int silverScore, int goldScore)
    {
        ws.Send("update_score;" + silverScore + ";" + goldScore);
    }

    public bool isTheBallGrabbed(int ball_id)
    {
        return isGrabbed[ball_id];
    }

    private void OnApplicationQuit() {
        ws.Send("leaving");
    }
}