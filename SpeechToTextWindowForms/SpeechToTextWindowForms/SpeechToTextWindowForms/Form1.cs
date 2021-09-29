using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using Microsoft.CognitiveServices.Speech.Transcription;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;
using System.Configuration;

namespace SpeechToTextWindowForms
{
    public partial class Form1 : Form
    {

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["key"].ConnectionString);
        private bool _dragging = false;
        private Point _offdet;
        private Point _start_point=new Point(0,0);
        public Form1()
        {
           
            InitializeComponent();
            //listBox2.Items.Add("Poorna");
            //listBox2.Items.Add("Ramana");
            FindAgents();
            label19.Text = Environment.GetEnvironmentVariable("USERNAME");
            //WindowState = FormWindowState.Maximized;
            this.Dock = DockStyle.Fill;
            Getdata();
        }
        private void Getdata()
        {
            if(connection.State==ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand cmd = new SqlCommand("select * from [dbo].[User]", connection);
            SqlDataReader sdr;
            sdr = cmd.ExecuteReader();
            while(sdr.Read())
            {
                listView1.Items.Add(sdr[1].ToString());
            }

        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            if (panel3.Visible == true)
            {
                panel3.Visible = false;
            }
            else
            {
                panel3.Visible = true;
            }
        }

        private void uContacts1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void uContacts3_Load(object sender, EventArgs e)
        {

        }

        private void uContacts1_Load_1(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private int _count = 0;
        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {

            if (this.Conversation.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.Conversation.Items.Add(text);
            }
        }
        StringBuilder sb = new StringBuilder();
        public async Task ContinuousRecognitionAutoDetectLanguageEng()
        {
           
            //var client = EsClient();
            
            var config = SpeechConfig.FromSubscription("c2733300c04e4a68884c220da5a4d848", "westeurope");

            var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromLanguages(new string[] { "en-US" });

            var stopMicRecognition = new TaskCompletionSource<int>();
            var stopSpeakerRecognition = new TaskCompletionSource<int>();
            var micInput = AudioConfig.FromDefaultMicrophoneInput();
            var micrecognizer = new SpeechRecognizer(config, autoDetectSourceLanguageConfig, micInput);
            micrecognizer.Recognizing += (s, e) =>
            {
                //Console.WriteLine($"Agent:{e.Result.Text}");
            };

            micrecognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Agent: {e.Result.Text}");
                    sb.Append($"Agent: {e.Result.Text}");
                    //listBox1.Items.Add($"Agent: {e.Result.Text}");
                    SetText($"Agent: {e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            micrecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopMicRecognition.TrySetResult(0);
            };

            micrecognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            micrecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopMicRecognition.TrySetResult(0);
            };

            await micrecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            var pushStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(48000, 16, 2));
            var speakerInput = AudioConfig.FromStreamInput(pushStream);
            var speakerRecognizer = new SpeechRecognizer(config, autoDetectSourceLanguageConfig, speakerInput);
            speakerRecognizer.Recognizing += (s, e) =>
            {
                //Console.WriteLine($"Client: Text={e.Result.Text}");
            };

            speakerRecognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Customer: {e.Result.Text}");
                    sb.Append($"Customer: {e.Result.Text}");
                    //listBox1.Items.Add($"Customer: {e.Result.Text}");
                    SetText($"Customer: {e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            speakerRecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopSpeakerRecognition.TrySetResult(0);
            };

            speakerRecognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\nSession started event.");
            };

            speakerRecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\nSession stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopSpeakerRecognition.TrySetResult(0);
            };

            await speakerRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
            var capture = new WasapiLoopbackCapture();

            capture.DataAvailable += async (s, e) =>
            {
                if (_count == 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(capture.WaveFormat));
                    _count++;
                }
                var resampledByte = ToPCM16(e.Buffer, e.BytesRecorded, capture.WaveFormat); //ResampleWasapi(s, e);
                pushStream.Write(resampledByte); // try to push buffer here
            };
            capture.RecordingStopped += (s, e) =>
            {

                capture.Dispose();
            };
            capture.StartRecording();
            Console.WriteLine("Record Started, Press Any key to stop the record");
            Console.ReadLine();
            capture.StopRecording();

            pushStream.Close();

            Task.WaitAny(new[] { stopSpeakerRecognition.Task, stopMicRecognition.Task });
            await speakerRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            await micrecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            TextWriter txt = new StreamWriter(@"D:\Titans\demo.txt");
            txt.Write(sb.ToString());
            txt.Close();
            //var response = await client.IndexAsync<SpeechConversation>(speechConversation, x => x.Index("speech"));
            //Console.WriteLine(response.Id);
        }

        public byte[] ToPCM16(byte[] inputBuffer, int length, WaveFormat format)
        {
            if (length == 0)
                return new byte[0]; // No bytes recorded, return empty array.

            // Create a WaveStream from the input buffer.
            using (var memStream = new MemoryStream(inputBuffer, 0, length))
            {
                using (var inputStream = new RawSourceWaveStream(memStream, format))
                {

                    // Convert the input stream to a WaveProvider in 16bit PCM format with sample rate of 48000 Hz.
                    var convertedPCM = new SampleToWaveProvider16(
                        new WdlResamplingSampleProvider(
                            new WaveToSampleProvider(inputStream),
                            48000)
                            );

                    byte[] convertedBuffer = new byte[length];

                    using (var stream = new MemoryStream())
                    {
                        int read;

                        // Read the converted WaveProvider into a buffer and turn it into a Stream.
                        while ((read = convertedPCM.Read(convertedBuffer, 0, length)) > 0)
                            stream.Write(convertedBuffer, 0, read);


                        // Return the converted Stream as a byte array.
                        return stream.ToArray();
                    }
                }
            }
        }

        System.Windows.Forms.Timer tmr = null;


        private async void button1_Click(object sender, EventArgs e)
        {
            await ContinuousRecognitionAutoDetectLanguageEng();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private Thread findPC;
        private string IP = "127.0.0.1";
        private bool serverRunning = false;
        TcpListener listener;
        private Thread serverThread;
        int flag = 0;
        int fileReceived = 0;
        string savePath;
        TcpClient client;
        private bool isConnected = false;
        string fileName = "";
        string senderIP;
        string senderMachineName;
        private void FindAgents()
        {
            try
            {
                findPC = new Thread(new ThreadStart(searchPC));
                findPC.Start();
                while (!findPC.IsAlive) ;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }

        void searchPC()
        {
            bool isNetworkUp = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkUp)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        this.IP = ip.ToString();
                    }
                }
                //Invoke((MethodInvoker)delegate
                //{
                //    infoLabel.Text = "This Computer: " + this.IP;
                //});
                string[] ipRange = IP.Split('.');
                for (int i = 100; i < 255; i++)
                {
                    Ping ping = new Ping();
                    //string testIP = "192.168.1.67";
                    string testIP = ipRange[0] + '.' + ipRange[1] + '.' + ipRange[2] + '.' + i.ToString();
                    if (testIP != this.IP)
                    {
                       // ping.PingCompleted += new PingCompletedEventHandler(pingCompletedEvent);
                        ping.SendAsync(testIP, 100, testIP);
                    }
                }

                
                //Starting this program as a server.
                if (!serverRunning)
                    startServer();
            }
            else
            {
                
                MessageBox.Show("Not connected to LAN");
            }
        }

        //void pingCompletedEvent(object sender, PingCompletedEventArgs e)
        //{
        //    string ip = (string)e.UserState;
        //    if (e.Reply.Status == IPStatus.Success)
        //    {
        //        string name;
        //        try
        //        {
        //            IPHostEntry hostEntry = Dns.GetHostEntry(ip);
        //            name = hostEntry.HostName;
        //        }
        //        catch (SocketException ex)
        //        {
        //            name = ex.Message;
        //        }
        //        Invoke((MethodInvoker)delegate
        //        {
                    
        //            ListViewItem item = new ListViewItem();
        //            item.Text = ip;
        //            item.SubItems.Add(name);
        //            listView1.Items.Add(item);
        //        });
        //    }
        //}
        
        void startServer()
        {
            try
            {
                serverRunning = true;
                listener = new TcpListener(IPAddress.Parse(IP), 11000);
                listener.Start();
                serverThread = new Thread(new ThreadStart(serverTasks));
                serverThread.Start();
                while (!serverThread.IsAlive) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       
        void serverTasks()
        {
            try
            {
                while (true)
                {
                    if (fileReceived == 1)
                    {
                        if (MessageBox.Show("Save File?", "File received", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            File.Delete(savePath);
                            fileReceived = 0;
                        }
                        else
                        {
                            fileReceived = 0;
                        }
                    }

                    client = listener.AcceptTcpClient();
                    //Invoke((MethodInvoker)delegate
                    //{
                    //    notificationPanel.Visible = true;
                    //    notificationTempLabel.Text = "File coming..." + "\n" + fileName + "\n" + "From: " + senderIP + " " + senderMachineName;
                    //    fileNotificationLabel.Text = "File Coming from " + senderIP + " " + senderMachineName;
                    //});
                    isConnected = true;
                    NetworkStream stream = client.GetStream();
                    if (flag == 1 && isConnected)
                    {
                        //savePath = savePathLabel.Text + "\\" + fileName;
                        using (var output = File.Create(savePath))
                        {
                            // read the file divided by 1KB
                            var buffer = new byte[1024];
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, bytesRead);
                            }
                            //MessageBox.Show("ok");
                            flag = 0;
                            client.Close();
                            isConnected = false;
                            fileName = "";
                            //Invoke((MethodInvoker)delegate
                            //{
                            //    notificationTempLabel.Text = "";
                            //    notificationPanel.Visible = false;
                            //    fileNotificationLabel.Text = "";
                            //});
                            fileReceived = 1;
                        }
                    }
                    else if (flag == 0 && isConnected)
                    {
                        Byte[] bytes = new Byte[256];
                        String data = null;
                        int i;
                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        }
                        string[] msg = data.Split('@');
                        fileName = msg[0];
                        senderIP = msg[1];
                        senderMachineName = msg[2];
                        client.Close();
                        isConnected = false;
                        flag = 1;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                flag = 0;
                isConnected = false;
                if (client != null)
                    client.Close();
            }
        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }
        Socket socketForClient;
       
        private void button2_Click(object sender, EventArgs e)
        {
            var targetIP = listView1.SelectedItems[0].Text;
            var targetName = listView1.SelectedItems[0].SubItems[1].Text;
           
            try
            {
                Ping p = new Ping();
                PingReply r;
                r = p.Send(targetIP);
                if (!(r.Status == IPStatus.Success))
                {
                    MessageBox.Show("Target computer is not available.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                   
                    //closing the server
                    listener.Stop();
                    serverThread.Abort();
                    serverThread.Join();
                    serverRunning = false;
                    //now making this program a client
                    socketForClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketForClient.Connect(new IPEndPoint(IPAddress.Parse(targetIP), 11000));
                    string fileName = "demo.txt";
                    //long fileSize = new FileInfo(fileNameLabel.Text).Length;
                    byte[] fileNameData = Encoding.Default.GetBytes(fileName + "@" + this.IP + "@" + Environment.MachineName);
                    socketForClient.Send(fileNameData);
                    socketForClient.Shutdown(SocketShutdown.Both);
                    socketForClient.Close();
                    socketForClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketForClient.Connect(new IPEndPoint(IPAddress.Parse(targetIP), 11000));
                    socketForClient.SendFile(@"D:\Titans\demo.txt");
                    socketForClient.Shutdown(SocketShutdown.Both);
                    socketForClient.Close();
                    //notification.Abort();
                    //notification.Join();
                    //notificationTempLabel.Text = "";
                    //notificationPanel.Visible = false;
                    //Invoke((MethodInvoker)delegate
                    //{
                    //    f2.Dispose();
                    //});
                    MessageBox.Show("File sent to " + targetIP + " " + targetName, "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (socketForClient != null)
                {
                    socketForClient.Shutdown(SocketShutdown.Both);
                    socketForClient.Close();
                }
            }
            finally
            {
                for (int i = 0; i < listView1.SelectedIndices.Count; i++)
                {
                    listView1.Items[this.listView1.SelectedIndices[i]].Selected = false;
                }
                
                //again making this program a server
                startServer();
            }
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point=new Point(e.X, e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p=PointToScreen(e.Location);
                Location=new Point(p.X-this._start_point.X, p.Y -this._start_point.Y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
    }
}
