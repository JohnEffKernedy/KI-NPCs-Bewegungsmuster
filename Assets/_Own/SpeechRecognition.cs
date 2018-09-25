using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SpeechRecognition : MonoBehaviour, SCL_IClientSocketHandlerDelegate
{
    //private TcpListener server;
    //private TcpClient client;
    //private Thread tcpListenerThread;
    //private bool programRunning;

    private SCL_SocketServer socketServer;
    private readonly object valueLock = new object();
    private float value;

    public int port = 5000;

    public delegate void SpeechResult(string text);
    public static SpeechResult OnSpeechResult;

    void Start()
    {
        //// Start TcpServer background thread 		
        //tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        //tcpListenerThread.IsBackground = true;
        //tcpListenerThread.Start();
        //programRunning = true;

        SCL_IClientSocketHandlerDelegate clientSocketHandlerDelegate = this;
        this.socketServer = new SCL_SocketServer(clientSocketHandlerDelegate, 5, "\n", port, Encoding.UTF8);
        this.socketServer.StartListeningForConnections();
    }

    void OnApplicationQuit()
    {
        this.socketServer.Cleanup();
        this.socketServer = null;
    }

    private void ListenForIncomingRequests()
    {
        //try
        //{
        //    server = new TcpListener(IPAddress.Any, 5000);
        //
        //    // Start listening for client requests.
        //    server.Start();
        //
        //    // Buffer for reading data
        //    Byte[] bytes = new Byte[1024];
        //
        //    // Enter the listening loop.
        //    while (programRunning)
        //    {
        //        Debug.Log("Waiting for a connection... ");
        //
        //        // Perform a blocking call to accept requests.
        //        // You could also user server.AcceptSocket() here.
        //        client = server.AcceptTcpClient();
        //        Debug.Log("Client connected from " + client.Client.RemoteEndPoint.ToString() + ".");
        //
        //        // Get a stream object for reading and writing
        //        NetworkStream stream = client.GetStream();
        //
        //        int i;
        //
        //        // Loop to receive all the data sent by the client.
        //        while (programRunning)
        //        {
        //            try
        //            {
        //                i = stream.Read(bytes, 0, bytes.Length);
        //                if (i == 0)
        //                {
        //                    throw new Exception();
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                break;
        //            }
        //            // Translate data bytes to a UTF8 string.
        //            string text = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
        //            if (OnSpeechResult != null)
        //            {
        //                OnSpeechResult(text);
        //            }
        //            else
        //            {
        //                Debug.Log(text);
        //            }
        //            System.Threading.Thread.Sleep(1);
        //        }
        //
        //        // Shutdown and end connection
        //        stream.Close();
        //        client.Close();
        //        System.Threading.Thread.Sleep(1);
        //    }
        //}
        //catch (SocketException socketException)
        //{
        //    Debug.Log("SocketException " + socketException.ToString());
        //}
    }

    void OnDisable()
    {
        //programRunning = false;
        //if (server != null)
        //{
        //    server.Stop();
        //}
        //if (tcpListenerThread != null)
        //{
        //    tcpListenerThread.Abort();
        //}
    }

    public void ClientSocketHandlerDidReadMessage(SCL_ClientSocketHandler handler, string message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(message));
    }

    public IEnumerator ThisWillBeExecutedOnTheMainThread(string message)
    {
        if (OnSpeechResult != null)
        {
            OnSpeechResult(message);
        }
        else
        {
            Debug.Log(message);
        }

        yield return null;
    }
}