using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Basic
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private int count;
        private bool flag, MON;
        private Button lastButton;
        private Entry ipAddr;
        private Entry tcpPort;
        private Switch auto, mon;
        private Label lblAuto, lblIp, lblPort, lblMon;
        private Grid grid1;
        private TcpClient clientSocket;
        private NetworkStream serverStream;
        private byte[] outStream;
        private String host;
        private String cmd = "LED=OFF";
        private int port;
        private int h, t;
        private delegate void unitRequest();
        private event unitRequest req;
        private Task monitor;
        Regex reg;
        public MainPage()
        {
            InitializeComponent();
            count = 1;
            flag = true;
            MON = false;
            reg = new Regex(@"\d+");
            t = 0; h = 0;
            req += new unitRequest(request);
             /*monitor = new Task(()=> {
                while (true) 
                {
                     if(MON==true)
                     {
                         req(); 
                         System.Threading.Thread.Sleep(10000);
                     }
                     
                     
                }
            
            });*/
           // monitor.Start();
        }

        protected override void OnAppearing()
        {
            cmd = "LED=OFF";
            object ob1 = "";
            object ob2 = 0;
            if (App.Current.Properties.TryGetValue("IP",  out ob1)) 
            {
                host = (String)ob1;
            }
            if (App.Current.Properties.TryGetValue("PRT", out ob2))
            {
                port = (int)ob2;
            }
            base.OnAppearing();

            ipAddr = new Entry() { Text = host };
            tcpPort = new Entry() { Text = port.ToString() };
            auto = new Switch() { IsToggled = false, HorizontalOptions = LayoutOptions.Start };
            mon = new Switch() { IsToggled = false, HorizontalOptions = LayoutOptions.Start };
            lblAuto = new Label() { Text = "Mode Auto Off", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor=Color.Blue};
            lblIp = new Label() { Text = "\nIP: ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor = Color.Brown };
            lblPort = new Label() { Text = "\nPort: ", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor = Color.Brown };
            lblMon = new Label() { Text = "Mode Monitor Off", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor = Color.Blue };
            auto.Toggled += auto_Toggled;
            mon.Toggled += mon_Toggled;
            grid1 = new Grid 
            {
              RowDefinitions=
                {
                  new RowDefinition {Height = new GridLength(1, GridUnitType.Star) },
                  new RowDefinition {Height = new GridLength(1, GridUnitType.Star) },
                  new RowDefinition {Height = new GridLength(1, GridUnitType.Star) },
                  new RowDefinition {Height = new GridLength(1, GridUnitType.Star) }
                },
              ColumnDefinitions = 
                {
                  new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star) },
                  new ColumnDefinition{Width = new GridLength(3, GridUnitType.Star) }
                }
              
            };

            grid1.Children.Add(lblIp, 0, 0);
            grid1.Children.Add(ipAddr, 1, 0);
            grid1.Children.Add(lblPort, 0, 1);
            grid1.Children.Add(tcpPort, 1, 1);
            grid1.Children.Add(auto, 0, 2);
            grid1.Children.Add(lblAuto, 1, 2);
            grid1.Children.Add(mon, 0, 3);
            grid1.Children.Add(lblMon, 1, 3);
        }

        private void btn1Clicked(object sender, EventArgs e) 
        {
            Console.WriteLine("BTN================="+cmd);
         if (auto.IsToggled) 
          {
                auto.IsToggled = false;
                btn1.Text = "Set Temp";
                btn1.BackgroundColor = Color.Blue;
                cmd = "LED=Auto " + tmpSlider.Value.ToString();
                //txtField1.Text = cmdSend("LED=Auto " + tmpSlider.Value.ToString());
                txtField1.Text = cmdSend(cmd);
                lbl1.Text = "Temp: " + t.ToString();
                lbl2.Text = "Humid: " + h.ToString();
                tmpSlider.Value = t;
                hmdSlider.Value = h;
                auto.IsToggled = true;
               // cmd = "LED=Auto " + tmpSlider.Value.ToString();
            }
            else
          { 
            if (count % 2 == 0) 
            {
                    if (mon.IsToggled)
                    {
                        btn1.Text = "Stop Monitoring";
                        //request();
                        //req();
                        MON = true;
                        monitor = new Task(() => {
                            while (MON)
                            {
                                req();
                               // request();
                                    System.Threading.Thread.Sleep(10000);
                            }

                        });
                        monitor.Start();
                       
                    }
                    else
                    {
                        btn1.Text = "Turn Power Off";
                        btn1.BackgroundColor = Color.Red;
                        txtField1.Text = cmdSend("LED=ON");
                        lbl1.Text = "Temp: " + t.ToString();
                        lbl2.Text = "Humid: " + h.ToString();
                        tmpSlider.Value = t;
                        hmdSlider.Value = h;
                        cmd = "LED=ON";
                    }
            }
            else 
            {
                    if (mon.IsToggled)
                    {
                        btn1.Text = "Start Monitoring";
                        MON=false;
                        monitor = new Task(() => {
                            while (MON)
                            {
                                req();
                                System.Threading.Thread.Sleep(10000);
                            }

                        });
                        monitor.Start();
                    }
                    else
                    {
                        btn1.Text = "Turn Power On";
                        btn1.BackgroundColor = Color.Green;
                        txtField1.Text = cmdSend("LED=OFF");
                        lbl1.Text = "Temp: " + t.ToString();
                        lbl2.Text = "Humid: " + h.ToString();
                        tmpSlider.Value = t;
                        hmdSlider.Value = h;
                        cmd = "LED=OFF";
                    }
            }
             count++;
          }
        }
        private void btn2Clicked(object sender, EventArgs e) 
        {
            txtField1.Text = "Change Settings";
            if (flag)
            {                
                lastButton = lastButton = new Button() { Text = "OK" };
                lastButton.Clicked += lastButtonClicked;                
                stackLayout.Children.Add(grid1);
                stackLayout.Children.Add(lastButton);                
                flag = false;
                btn2.Text = "Hide Settings";
            }
            else 
             { 
                for(int i =0; i<2; i++) stackLayout.Children.RemoveAt(stackLayout.Children.Count - 1);
                flag = true;
                btn2.Text = "Connection Settings";
            }
        }
        private void lastButtonClicked(object sender, EventArgs e) 
        {
            try
            {
                port = int.Parse(tcpPort.Text);
                host = ipAddr.Text;
                txtField1.Text = "Saved: "+host+":"+port.ToString();
                App.Current.Properties["IP"] = host;
                App.Current.Properties["PRT"] = port;

            }
            catch (Exception exc)
            {
                txtField1.Text = exc.Message;
            }
        }

        private void auto_Toggled(object sender, EventArgs e)
        {
            if (auto.IsToggled) 
            {
                mon.IsToggled = false;
                MON = false;
                lblAuto.Text = "Mode Auto On";
                btn1.Text = "Set Temp";
                btn1.BackgroundColor = Color.Blue;
            }
            else
            { 
                lblAuto.Text = "Mode Auto Off";
                btn1.Text = "Start";
                btn1.BackgroundColor = Color.Green;
                count = 1;
            }
        }

        private void mon_Toggled(object sender, EventArgs e) 
        {
            if (mon.IsToggled) 
            {
                auto.IsToggled = false;
                lblMon.Text = "Mode Monitor On";
                btn1.Text = "Start Monitoring";
                btn1.BackgroundColor = Color.Orange;
                count = 0;
            }
            else 
            {
                MON = false;
                lblMon.Text = "Mode Monitor Off";
                btn1.Text = "Start";
                btn1.BackgroundColor = Color.Green;
                count = 1;
            }
        }

        private void temp_SliderChanged(object sender, ValueChangedEventArgs e) 
        {
            int newStep = (int)Math.Round(e.NewValue /1.0);
            tmpSlider.Value = newStep;
            if (auto.IsToggled) txtField1.Text = "Temp set to: "+ tmpSlider.Value.ToString();
        }

        private  void request() 
        {
             Device.BeginInvokeOnMainThread(()=> {
                 txtField1.Text = cmdSend(cmd);
                 lbl1.Text = "Temp: " + t.ToString();
                 lbl2.Text = "Humid: " + h.ToString();
                 tmpSlider.Value = t;
                 hmdSlider.Value = h;
                 btn1.BackgroundColor = Color.Aqua;
                 Thread.Sleep(1500);
                 btn1.BackgroundColor = Color.Orange;

             });
            /*txtField1.Text = cmdSend(cmd);           
            lbl1.Text = "Temp: " + t.ToString();
            lbl2.Text = "Humid: " + h.ToString();
            tmpSlider.Value = t;
            hmdSlider.Value = h;*/
        }

        private String cmdSend(String s) 
        {
            clientSocket = new System.Net.Sockets.TcpClient();
            String returndata;
            try
            {               
                var result = clientSocket.BeginConnect(host, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(4));
                if (!success)
                {
                    throw new Exception("Failed to connect.");
                }
                serverStream = clientSocket.GetStream();
                txtField1.Text = "Connected ...";
            }
            catch (Exception ex) { txtField1.Text = ex.Message; return "Failed to connect"; }
            try
            {
                String str = s; outStream = Encoding.UTF8.GetBytes(" /" + str + "\r\n");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                byte[] inStream = new byte[1207840];
                Console.WriteLine("=============="+ (int)clientSocket.ReceiveBufferSize);
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);               
                returndata = Encoding.UTF8.GetString(inStream);
                //while (returndata.EndsWith("\n") || returndata.EndsWith("\r")) { returndata.Remove(returndata.Length-1); };
                returndata = returndata.Substring(0, 80);
                Console.WriteLine("==============" + returndata);
                MatchCollection matches = reg.Matches(returndata);
                if (matches.Count == 2)
                {
                    t = int.Parse(matches[0].Value);
                    h = int.Parse(matches[1].Value);
                }
                if (matches.Count == 3)
                {
                    t = int.Parse(matches[1].Value);
                    h = int.Parse(matches[2].Value);
                }
                clientSocket.Close();
                return returndata;
            }
            catch (Exception exc)
            {
                return exc.Message;                
            }            
            
        }
    }
}
