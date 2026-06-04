using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.IO;
using TMPro;

// Manages the connexion between the unity server and the client
public class StartConnection_Petanque : MonoBehaviour
{
    public GameObject HeadsetCamera;
    public GameObject CAVECamera;
    public GameObject InitialCamera;

    public GameObject NetworkCanva;
    public GameObject DisplayCanva;
    public GameObject CharacterCanva;
    public GameObject ServerSelectCanva;

    public TMP_InputField inputIP;

    public WsClient_Petanque client;

    public GameObject errorText;

    private AudioSource audio;

    private Process process;

    private string ipAdress;

    public bool isCAVE;

    public void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Start()
    {
        HeadsetCamera.SetActive(false);
        CAVECamera.SetActive(false);
        Cursor.visible = true;
    }

    // Launch the Node.js server, the Unity server and connects it to Node.js
    public void JoinAsServer()
    {
        NetworkCanva.SetActive(false);
        RunNodeScript();
        Thread.Sleep(3000);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(GetLocalIPAddress(), (ushort)7777);
        UnityEngine.Debug.Log(GetLocalIPAddress());
        NetworkManager.Singleton.StartServer();
        client.StartConnexionToNodeJs(GetLocalIPAddress());
    }

    // Finds the IP Adress of the PC
    string GetLocalIPAddress()
    {
        string localIP = "";
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        return string.IsNullOrEmpty(localIP) ? "IP non trouvée" : localIP;
    }

    // Runs a terminal in background and launch the Node.js server
    void RunNodeScript()
    {
        string serverPath = Path.Combine(Application.streamingAssetsPath, "WebSockets/Petanque", "server.js");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = @"server.js",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(serverPath)
        };

        process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
        process.ErrorDataReceived += (s, e) => UnityEngine.Debug.LogError(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    
    public void JoinAsClient()
    {
        NetworkCanva.SetActive(false);
        CharacterCanva.SetActive(true);
    }

    // Connects to server
    public void JoinIP()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(inputIP.text, (ushort)7777);
        ipAdress = inputIP.text;
        UnityEngine.Debug.Log("Trying connexion to " + ipAdress);
        if (NetworkManager.Singleton.StartClient())
        {
            UnityEngine.Debug.Log("Client here");
            ServerSelectCanva.SetActive(false);
            DisplayCanva.SetActive(true);
        }
        else 
        {
            errorText.SetActive(true);
        }
    }

    // Activates the CAVE camera and connects to Node.js server
    public void JoinAsCAVE()
    {
        isCAVE = true;

        InitialCamera.SetActive(false);
        CAVECamera.SetActive(true);

        DisplayCanva.SetActive(false);

        Cursor.visible = false;

        audio.Play();

        client.StartConnexionToNodeJs(ipAdress);
    }

    // Activates the Headset camera and connects to Node.js server
    public void JoinAsHeadset()
    {
        isCAVE = false;
        
        InitialCamera.SetActive(false);
        HeadsetCamera.SetActive(true);

        DisplayCanva.SetActive(false);

        Cursor.visible = false;

        audio.Play();

        client.StartConnexionToNodeJs(ipAdress);
    }

    private void OnApplicationQuit()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
        }
    }
}
