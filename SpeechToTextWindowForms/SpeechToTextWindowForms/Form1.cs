using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNetCore.SignalR.Client;

namespace SpeechToTextWindowForms
{
    public partial class Form1 : Form
    {

        SqlConnection sqlconnection = new SqlConnection(ConfigurationManager.ConnectionStrings["key"].ConnectionString);
        private bool _dragging = false;
       
        private Point _start_point=new Point(0,0);
        HubConnection connection;
        public Form1()
        {
           
            InitializeComponent();
            Conversation.BackColor = Color.Beige;
            Conversation.DrawMode = DrawMode.OwnerDrawFixed;
            Conversation.DrawItem += new DrawItemEventHandler(listBox1_SetColor);
            Getdata();
            //FindAgents();
            label19.Text = Environment.GetEnvironmentVariable("USERNAME");
            //WindowState = FormWindowState.Maximized;
            this.Dock = DockStyle.Fill;
           
            connection = new HubConnectionBuilder()
              .WithUrl("https://signalrchat20210312211311.azurewebsites.net/ChatHub")
              .Build();
               ConnectToHub();
        }
        
        private async void ConnectToHub()
        {
            #region snippet_ConnectionOn
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
               
                var newMessage = $"{user}: {message}";
                // messagesList.Items.Add(newMessage);
                textBox1.Text = newMessage;

            });
            #endregion

            try
            {
                await connection.StartAsync();
               // messagesList.Items.Add("Connection started");
                textBox1.Text = "Connection started";
            }
            catch (Exception ex)
            {
                // messagesList.Items.Add(ex.Message);
                textBox1.Text =ex.Message;
            }
        }
        private void Getdata()
        {
            if (sqlconnection.State == ConnectionState.Closed)
            {
                sqlconnection.Open();
            }
            SqlCommand cmd = new SqlCommand("select * from [dbo].[User]", sqlconnection);
            SqlDataReader sdr;
            sdr = cmd.ExecuteReader();
            while (sdr.Read())
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
        private void customertext(string text)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(customertext);
                this.Invoke(d, new object[] { text });
            }
            else
            {

                //this.textBox2.Text += text;
                this.listBox1.Items.Add(text);
            }
        }
        void listBox1_SetColor(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                Brush myBrush = Brushes.White;

                var sayi = ((ListBox)sender).Items[e.Index].ToString();
                if (sayi.Contains("Customer:"))
                {
                    myBrush = Brushes.Red;

                }
                else
                {
                    myBrush = Brushes.Green;
                }

                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }
        StringBuilder sb = new StringBuilder();
        StringBuilder customersb = new StringBuilder();
        public async Task ContinuousRecognitionAutoDetectLanguageEng()
        {

            //var client = EsClient();
            
            var AgentName = Environment.GetEnvironmentVariable("USERNAME");
            
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
                    sb.Append($"{AgentName}: {e.Result.Text}");
                    sb.Append("\n");
                    
                    //listBox1.Items.Add($"Agent: {e.Result.Text}");
                    SetText($"{AgentName}: {e.Result.Text}");
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
                   var recognize=($"Customer: {e.Result.Text}");
                    sb.Append($"Customer: {e.Result.Text}");
                    sb.Append("\n");
                    customersb.Append($"{e.Result.Text}");
                    SetText($"Customer: {e.Result.Text}");
                    customertext($" {e.Result.Text}");
                    //var length = this.listBox1.Items.Count;
                    //if(length%3==0)
                    //{
                        
                    //    string URL = "https://text2emotion.azurewebsites.net/emotion?text=" + sb.ToString();
                    //    var data = webGetMethod(URL);
                    //    Emotion.Items.Add(data);
                    //}
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                  var speech=($"NOMATCH: Speech could not be recognized.");
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
            //string URL = "https://text2emotion.azurewebsites.net/emotion?text=" + sb.ToString();
            //var data = webGetMethod(URL);
            //customerEmotiontext(data);
            //TextWriter txt = new StreamWriter(@"D:\Titans\demo.txt");
            //txt.Write(sb.ToString());
            //txt.Close();
            //var response = await client.IndexAsync<SpeechConversation>(speechConversation, x => x.Index("speech"));
            //Console.WriteLine(response.Id);
        }

        private void customerEmotiontext(string text)
        {
            if (this.Emotion.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(customertext);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.Emotion.Items.Add(text);
            }
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

        private async void button1_Click(object sender, EventArgs e)
        {
            await ContinuousRecognitionAutoDetectLanguageEng();

            
        }
        public string webGetMethod(string URL)
        {
            string jsonString = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 7.1; Trident/5.0)";
            request.Accept = "/";
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            jsonString = sr.ReadToEnd();
            sr.Close();
            return jsonString;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
       
        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SendStatus(sb.ToString());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private async void SendStatus(string message)
        {

            #region snippet_ErrorHandling
            try
            {
                #region snippet_InvokeAsync
                var name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (name.Contains('\\'))
                    name = name.Split('\\')[1];
                await connection.InvokeAsync("SendMessage",
                    name, message);
                #endregion
            }
            catch (Exception ex)
            {
               // messagesList.Items.Add(ex.Message);
                textBox1.Text= ex.Message;
            }
            #endregion
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

        private void Conversation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void messagesList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       
        private void panel1_MouseDown_1(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void panel1_MouseUp_1(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string postData = "hi i am going to hyd and happy";
          
            //listBox1.Items.Add(data);
            //Console.WriteLine(data);
        }

        private async void button3_Click_1(object sender, EventArgs e)
        {
            string URL = "https://text2emotion.azurewebsites.net/emotion?text=" + customersb.ToString();
            var data = webGetMethod(URL);
            Emotion.Items.Add(data);
            
        }

        private async void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            
        }
    }
}
