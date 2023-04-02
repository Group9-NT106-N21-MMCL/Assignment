using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace TCP_Client_Server
{
    public partial class Client : Form
    {
        IPEndPoint IP;
        Socket client;
        int port = 9000;
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(IP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Thread Listen = new Thread(Receive);
            Listen.IsBackground = true;
            Listen.Start();

        }

        private void Send()
        {
            if (messageBox.Text != String.Empty)
            {
                string msg = $"{userName.Text} said : {messageBox.Text}\r\n";
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                client.SendTo(buffer,IP);
                log.AppendText("You said :" + messageBox.Text + "\r\n");
            }
            messageBox.Clear();
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[2048];
                    client.Receive(data);
                    string message = Encoding.UTF8.GetString(data);
                    log.AppendText(message);
                }
            }
            catch(Exception ex)
            {
                log.AppendText(ex.Message + "\r\n");
            }
        }
        private void exitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            Connect();
            log.AppendText("Connected\r\n");
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            client.Close();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            Send();
            messageBox.Clear();
        }
    }
}
