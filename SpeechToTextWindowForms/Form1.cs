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
using Nest;
using Elasticsearch.Net;
using System.Collections.Generic;

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
                var signalrdata = message.Split('@');
                foreach(var sig in signalrdata)
                {
                    messagesList.Items.Add(sig);
                }

                // messagesList.Items.Add(newMessage);
                //textBox1.Text = newMessage;

            });
            #endregion

            try
            {
                await connection.StartAsync();
               messagesList.Items.Add("Connection started");
                //textBox1.Text = "Connection started";
            }
            catch (Exception ex)
            {
                 messagesList.Items.Add(ex.Message);
                //textBox1.Text =ex.Message;
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
                
                var sitem=listView1.Items.Add(sdr[1].ToString());
                if(sdr[1].ToString().ToLower()=="rupeshkumar")
                {
                    sitem.ForeColor = Color.Red;
                }
                else if (sdr[1].ToString().ToLower() == "avusharlakamalaker")
                {
                    sitem.ForeColor = Color.Red;
                }
                else
                {
                    sitem.ForeColor = Color.Green;
                }
                
            }

        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
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
        StringBuilder signalrsb = new StringBuilder();
        StringBuilder customersb = new StringBuilder();
        List<LineConversation> LineConversations = new List<LineConversation>();
        public async Task ContinuousRecognitionAutoDetectLanguageEng()
        {
            
            int lineId = 0;
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
                    signalrsb.Append($"{AgentName}: {e.Result.Text}");
                    signalrsb.Append("@");
                    var lineconverstion = new LineConversation
                    {
                        LineId = lineId + 1,
                        Speaker= AgentName,
                        LineText= e.Result.Text
                    };
                    LineConversations.Add(lineconverstion);
                    lineId++;
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
                    signalrsb.Append($"Customer: {e.Result.Text}");
                    sb.Append("@");
                    customersb.Append($"{e.Result.Text}");
                    var lineconverstion = new LineConversation
                    {
                        LineId = lineId + 1,
                        Speaker = "Customer",
                        LineText = e.Result.Text
                    };
                    LineConversations.Add(lineconverstion);
                    lineId++;
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
            AnalyzeEmotion();
                  var speechConversation = new SpeechConversation
            {
                Agent = AgentName,
                Customer = "Customer",
                Conversation = sb.ToString(),
                StartTime = DateTime.Now,
                LineConversations=LineConversations
            };
            //var response = await client.IndexAsync<SpeechConversation>(speechConversation, x => x.Index("speech"));
            //Console.WriteLine(response.Id);
        }
        private void customerEmotiontext(string text)
        {
            if (this.Emotion.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(customerEmotiontext);
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

        public async Task getdata()
        {

            var config = SpeechConfig.FromSubscription("c2733300c04e4a68884c220da5a4d848", "westeurope");
            var autoDetectSourceLanguageConfigs = AutoDetectSourceLanguageConfig.FromLanguages(new string[] { "en-US" });
            var stopMicRecognitions = new TaskCompletionSource<int>();
            var stopSpeakerRecognition = new TaskCompletionSource<int>();
            var pushStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(48000, 16, 2));
            var speakerInput = AudioConfig.FromStreamInput(pushStream);
            var micInput = AudioConfig.FromDefaultMicrophoneInput();
            var stopSpeakerRecognitions = new TaskCompletionSource<int>();
            var capture = new WasapiLoopbackCapture();
            var micrecognizers = new SpeechRecognizer(config, autoDetectSourceLanguageConfigs, micInput);

            capture.StopRecording();

            pushStream.Close();
            var speakerRecognizer = new SpeechRecognizer(config, autoDetectSourceLanguageConfigs, speakerInput);
            await speakerRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            await micrecognizers.StopContinuousRecognitionAsync().ConfigureAwait(false);
            MessageBox.Show("Successfully ended conversation");

        }

        public ConversationSummary PostMethod(string baseURL,string jsonContent)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL);
            request.Method = "POST";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            string responsestring = "";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    responsestring = sr.ReadToEnd();
                }
                var summaryresult = JsonConvert.DeserializeObject<ConversationSummary>(responsestring);
                return summaryresult;
            }
            catch
            {
                throw;
            }
        }

        public SummaryResult PostAISummaryMethod(string baseURL, string jsonContent)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL);
            request.Method = "POST";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            string responsestring = "";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    responsestring = sr.ReadToEnd();
                }
                var summaryresult = JsonConvert.DeserializeObject<SummaryResult>(responsestring);
                return summaryresult;
            }
            catch
            {
                throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SendStatus(signalrsb.ToString());
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
               messagesList.Items.Add(ex.Message);
                //textBox1.Text= ex.Message;
            }
            #endregion
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

    
        //private async void button3_Click_1(object sender, EventArgs e)
        //{
        //    string URL = "https://text2emotion.azurewebsites.net/emotion?text=" + textBox2.Text;
        //    var data = webGetMethod(URL);
        //    if (!string.IsNullOrEmpty(data))
        //    {
        //        Emotion.Items.Clear();
        //        var dtresu = data.Split(',');
        //        Emotion.Items.Add(dtresu[0].Remove(0, 1));
        //        Emotion.Items.Add(dtresu[1]);
        //        Emotion.Items.Add(dtresu[2]);
        //        Emotion.Items.Add(dtresu[3]);
        //        Emotion.Items.Add(dtresu[4].Remove(dtresu[4].Length - 2));
        //    }
        //}
        private async void AnalyzeEmotion()
        {
            string URL = "https://text2emotion.azurewebsites.net/emotion?text=" + customersb.ToString();
            var data = webGetMethod(URL);
            setEmotion(data);
           
        }
        private void setEmotion(string text)
        {
            if (this.Emotion.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setEmotion);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (!string.IsNullOrEmpty(text))
                {
                    Emotion.Items.Clear();
                    var dtresu = text.Split(',');
                    this.Emotion.Items.Add(dtresu[0].Remove(0, 1));
                    this.Emotion.Items.Add(dtresu[1]);
                    this.Emotion.Items.Add(dtresu[2]);
                    this.Emotion.Items.Add(dtresu[3]);
                    this.Emotion.Items.Add(dtresu[4].Remove(dtresu[4].Length - 2));
                }
                
            }
        }
        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;
            var nodes = new Uri[]
    {
        new Uri("http://localhost:9200/"),

    };
            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);

            return elasticClient;
        }

        string recorddata = string.Empty;
        private void button3_Click_3(object sender, EventArgs e)
        {
            string summaryurl = "https://localhost:5001/api/Summary";
            var jsonstring = JsonConvert.SerializeObject(LineConversations);
            var SummaryResult = PostMethod(summaryurl, jsonstring);
            problemstatement.Text = "Problem Statement : " + SummaryResult.ProblemStatement;
            solution.Text = "Solution : " + SummaryResult.Solution;
            ViewAISummary();
        }
        public void ViewAISummary()
        {
            var summary = new SummaryRequest()
            {
                prompt = sb.ToString()
            };
            string summaryurl = "https://localhost:5001/api/Summary/AIResult";
            var jsonstring = JsonConvert.SerializeObject(summary);
            var AISummaryResult = PostAISummaryMethod(summaryurl, jsonstring);
            var SummaryResultTest = AISummaryResult?.choices[0]?.text;
            setAiSummary(SummaryResultTest);
        }
        private void setAiSummary(string text)
        {
            if (this.AiSummary.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setAiSummary);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.AiSummary.Text =text;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            recorddata = "summary";
            recordSummary();
        }

        private void recordSummary()
        {
            StartMicRecognition();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            recorddata = "problem";
            recordproblemstatement();
        }

        private void recordproblemstatement()
        {
            StartMicRecognition();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            recorddata = "solution";
            recordSolution();
        }

        private void recordSolution()
        {
            StartMicRecognition();
        }
        StringBuilder problemstatemt = new StringBuilder();
        StringBuilder problemsolution = new StringBuilder();
        public async Task StartMicRecognition()
        {
            problemstatemt = new StringBuilder();
            problemsolution = new StringBuilder();
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
                    var text = $"{e.Result.Text}";
                    if (text.ToLower().Contains("problem is"))
                    {
                        problemstatemt.Append(text);
                    }
                    else
                    {
                        problemsolution.Append(text);
                    }
                   
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
                Console.WriteLine("\n Session stopped event.");
                Console.WriteLine("\n Stop recognition.");
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
                    var recognize = ($"Customer: {e.Result.Text}");
                   
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    var speech = ($"NOMATCH: Speech could not be recognized.");
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
            if (recorddata=="summary")
            {
                setproblemstatement(problemstatemt.ToString());
                setsolution(problemsolution.ToString());
            }
            else if(recorddata == "problem")
            {
                setproblemstatement(problemstatemt.ToString());
            }
            else if (recorddata == "solution")
            {
                setsolution(problemsolution.ToString());
            }
        }

        private void setproblemstatement(string text)
        {
            if (this.problemstatement.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setproblemstatement);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.problemstatement.Text ="Problem Statement : " + text;
            }
        }

        private void setsolution(string text)
        {
            if (this.solution.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setsolution);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.solution.Text ="Solution : "+ text;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            await getdata();
        }
    }
}
