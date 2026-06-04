using UnityEngine;
using WebSocketSharp;
using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WsClient_Transport : MonoBehaviour
{
    private Transform[] ropeSphere = new Transform[4];
    private Transform[] ropeMinisphere1 = new Transform[4];
    private Transform[] ropeMinisphere2 = new Transform[4];
    private Transform[] ropeMinisphere3 = new Transform[4];
    private Transform[] ropeMinisphere4 = new Transform[4];
    private Transform[] ropeMinisphere5 = new Transform[4];
    private Transform[] ropeMinisphere6 = new Transform[4];
    private Transform[] ropeMinisphere7 = new Transform[4];

    public Transform board;

    public int level_number;

    private Transform[] cubes = new Transform[14];

    private Vector3[] lastRopePosition = new Vector3[4];
    private Vector3[] ropeVelocity = new Vector3[4];

    private bool[] isGrabbed = new bool[4]; 

    private WebSocket ws;

    private bool positionUpdated = false;
    private Vector3[] newSpherePos = new Vector3[4];
    private Vector3[] newMinisphere1Pos = new Vector3[4];
    private Vector3[] newMinisphere2Pos = new Vector3[4];
    private Vector3[] newMinisphere3Pos = new Vector3[4];
    private Vector3[] newMinisphere4Pos = new Vector3[4];
    private Vector3[] newMinisphere5Pos = new Vector3[4];
    private Vector3[] newMinisphere6Pos = new Vector3[4];
    private Vector3[] newMinisphere7Pos = new Vector3[4];
    private Vector3 newBoardPos;
    private Quaternion newBoardRot;
    private Vector3[] newCubePos = new Vector3[14];
    private Quaternion[] newCubeRot = new Quaternion[14];

    private bool[] owner = new bool[4];

    private bool[] isGrabbing = new bool[4];

    private XRGrabInteractable[] ropeInteractable = new XRGrabInteractable[4];

    private int id = 0;

    public int ropeCount = 4;
    private int cubeNumber = 0;

    private int server_message_id = 0;

    public void StartConnexionToNodeJs(string ip)
    {
        ws = new WebSocket("ws://" + ip + ":8080");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            string[] message = e.Data.Split(";");
            if (message[0] == "grabbed")
            {
                int ball_id = Int32.Parse(message[1]);
                isGrabbed[ball_id] = true;
                if (!isGrabbing[ball_id])
                {
                    ropeInteractable[ball_id].enabled = false;
                    owner[ball_id] = false;
                }
            }
            else if (message[0] == "released")
            {
                int ball_id = Int32.Parse(message[1]);
                isGrabbed[ball_id] = false;
                ropeInteractable[ball_id].enabled = true;
            }
            else if (Int32.Parse(message[0]) > server_message_id)
            {
                server_message_id = Int32.Parse(message[0]);
                positionUpdated = true;
                for (int i = 0; i < ropeCount; i++)
                {
                    if (!isGrabbed[i] || (isGrabbed[i] && !owner[i]))
                    {
                        newSpherePos[i] = new Vector3(float.Parse(message[24*i + 1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 3], CultureInfo.InvariantCulture.NumberFormat));
                    }
                    newMinisphere1Pos[i] = new Vector3(float.Parse(message[24*i + 4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 6], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere2Pos[i] = new Vector3(float.Parse(message[24*i + 7], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 8], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 9], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere3Pos[i] = new Vector3(float.Parse(message[24*i + 10], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 11], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 12], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere4Pos[i] = new Vector3(float.Parse(message[24*i + 13], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 14], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 15], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere5Pos[i] = new Vector3(float.Parse(message[24*i + 16], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 17], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 18], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere6Pos[i] = new Vector3(float.Parse(message[24*i + 19], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 20], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 21], CultureInfo.InvariantCulture.NumberFormat));
                    newMinisphere7Pos[i] = new Vector3(float.Parse(message[24*i + 22], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 23], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*i + 24], CultureInfo.InvariantCulture.NumberFormat));
                }
                newBoardPos = new Vector3(float.Parse(message[24*ropeCount + 1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 3], CultureInfo.InvariantCulture.NumberFormat));
                newBoardRot = new Quaternion(float.Parse(message[24*ropeCount + 4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 6], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 7], CultureInfo.InvariantCulture.NumberFormat));
                for (int i = 0; i < cubeNumber; i++)
                {
                    newCubePos[i] = new Vector3(float.Parse(message[24*ropeCount + 8 + i*7], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 8 + i*7 + 1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 8 + i*7 + 2], CultureInfo.InvariantCulture.NumberFormat));
                    newCubeRot[i] = new Quaternion(float.Parse(message[24*ropeCount + 8 + i*7 + 3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 8 + i*7 + 4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 8 + i*7 + 5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(message[24*ropeCount + 8 + i*7 + 6], CultureInfo.InvariantCulture.NumberFormat));
                }
            }
        };

        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            ws.Send("rope_number;" + id + ";" + ropeCount);
            for (int i = 0; i < ropeCount; i++)
            {
                owner[i] = true;
            }
            for (int i = 0; i < cubeNumber; i++)
            {
                ws.Send("add_cube;" + id + ";" + i + ";" + cubes[i].position.x + ";" + cubes[i].position.y + ";" + cubes[i].position.z);
                id++;
            }
            ws.Send("start;" + id + ";" + level_number);
            id++;
        }
    }

    private void FixedUpdate()
    {
        if (positionUpdated)
        {
            for (int i = 0; i < ropeCount; i++)
            {
                if (!isGrabbed[i] || (isGrabbed[i] && !owner[i]))
                {
                    ropeSphere[i].position = newSpherePos[i];
                }
                ropeMinisphere1[i].position = newMinisphere1Pos[i];
                ropeMinisphere2[i].position = newMinisphere2Pos[i];
                ropeMinisphere3[i].position = newMinisphere3Pos[i];
                ropeMinisphere4[i].position = newMinisphere4Pos[i];
                ropeMinisphere5[i].position = newMinisphere5Pos[i];
                ropeMinisphere6[i].position = newMinisphere6Pos[i];
                ropeMinisphere7[i].position = newMinisphere7Pos[i];
            }
            board.position = newBoardPos;
            board.rotation = newBoardRot;
            for (int i = 0; i < cubeNumber; i++)
            {
                cubes[i].position = newCubePos[i];
                cubes[i].rotation = newCubeRot[i];
            }
        }

        for (int i = 0; i < ropeCount; i++)
        {
            if (isGrabbing[i])
            {
                ropeVelocity[i] = (ropeSphere[i].position - lastRopePosition[i]) / Time.deltaTime;
                lastRopePosition[i] = ropeSphere[i].position;
                //ws.Send("positions_grabbed;" + id + ";" + ropeVelocity[i].x + ";" + ropeVelocity[i].y + ";" + ropeVelocity[i].z + ";" + i);
                ws.Send("positions_grabbed;" + id + ";" + ropeSphere[i].position.x + ";" + ropeSphere[i].position.y + ";" + ropeSphere[i].position.z + ";" + i);
                id++;
            }
        }
    }

    private void Update() {

    }

    public void Grabbed(int ball_id)
    {
        isGrabbing[ball_id] = true;
        ws.Send("grabbed;" + id + ";" + ball_id);
        id++;
        owner[ball_id] = true;
    }

    public void Released(int ball_id)
    {
        isGrabbing[ball_id] = false;
        ws.Send("released;" + id + ";" + ropeVelocity[ball_id].x + ";" + ropeVelocity[ball_id].y + ";" + ropeVelocity[ball_id].z + ";" + ball_id);
        id++;
    }

    public void Reset()
    {
        ws.Send("reset;" + id);
    }

    public bool isTheBallGrabbed(int rope_id)
    {
        return isGrabbed[rope_id];
    }

    public void AddRope(int id, Transform sphere, Transform minisphere1, Transform minisphere2, Transform minisphere3, Transform minisphere4, Transform minisphere5, Transform minisphere6, Transform minisphere7, XRGrabInteractable interactable)
    {
        ropeSphere[id] = sphere;
        ropeMinisphere1[id] = minisphere1;
        ropeMinisphere2[id] = minisphere2;
        ropeMinisphere3[id] = minisphere3;
        ropeMinisphere4[id] = minisphere4;
        ropeMinisphere5[id] = minisphere5;
        ropeMinisphere6[id] = minisphere6;
        ropeMinisphere7[id] = minisphere7;
        ropeInteractable[id] = interactable;
    }

    public void AddCube (int id, Transform cube)
    {
        cubes[id] = cube;
        cubeNumber++;
    }

    private void OnApplicationQuit() {
        ws.Send("leaving");
    }
}