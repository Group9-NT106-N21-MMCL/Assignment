using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
namespace Server
{
    public partial class Server : Form
    {
        IPEndPoint IP;
        Socket server;
        int port = 9000;
        List<Socket> clientList;
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Connect()
        {
            IP = new IPEndPoint(IPAddress.Any, port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(IP);
            clientList = new List<Socket>();
            
            Thread Listen = new Thread(()=>{
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread recv = new Thread(Receive);
                        recv.IsBackground = true;
                        recv.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any,port);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            });
            Listen.IsBackground = true;
            Listen.Start();


        }
        private void Send(Socket client)
        {
            if (messageBox.Text != String.Empty)
            {
                string msg = $"Server said : {messageBox.Text}\r\n";
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                client.SendTo(buffer,IP);
            }
        }
        private void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[2048];
                    client.Receive(data);
                    string message = Encoding.UTF8.GetString(data);
                    log.AppendText(message);
                    foreach(Socket c in clientList)
                    {
                        if(c != null && message != String.Empty && c != client)
                        {
                            c.Send(Encoding.UTF8.GetBytes(message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.AppendText(ex.Message + "\r\n");
                clientList.Remove(client);
                client.Close();
            }
        }
        private void Server_Load(object sender, EventArgs e)
        {

        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            foreach(Socket client in clientList)
            {
                Send(client);   
            }
            messageBox.Clear();
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            Connect();
            log.AppendText("Server Started\r\n");
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            server.Close();
            log.AppendText("Server Closed\r\n");
        }
    }
}
