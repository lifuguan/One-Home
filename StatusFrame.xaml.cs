using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using System.Text;
using System.Net.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SerialPort
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StatusFrame : Page
    {
        SerialPort serialPort = new SerialPort();

        // 声明Timer 
        private DispatcherTimer _timer;

        private string portName;
        public string PortName
        {
            get => portName;
            set
            {
                portName = value;
                PortStutas.Text = value;
            }
        }

        //使页面UI显示指定地点的数据集
        private enum DataDisplay
        {
            Default, RoomHall, RoomAlpha
        };
        //使页面UI显示指定地点的控制变量 , 以 Default 为初始值
        private int DataStatus = (int)DataDisplay.Default;



        public StatusFrame()
        {

            this.InitializeComponent();
            //string portName = "COM";
            //for (int i = 0; i < 10; i++)
            //{
            //    portName += i.ToString();
            //    serialPort.Open(portName, this);   //取地址，不能通过new开辟新内存
            //    portName = "COM";
            //}

            ////设置Timer来定时读取串口
            //_timer = new DispatcherTimer();
            //// Specifies the timer event interval.
            //// Runs here ONE second
            //_timer.Interval = TimeSpan.FromSeconds(1);
            //// indicate execute event
            //_timer.Tick += ProcSerialStream;
            //// Start a timer event
            //_timer.Start();
        }

        /// <summary>
        /// 截取并处理后的最终数据    _H：大厅     _R：房间
        /// </summary>
        //温度_H
        private string Temperature_H = "";
        //湿度_H
        private string Humidity_H = "";
        //火警数据_H
        private string FireStatus_H = "";
        //光照_H
        private string LightStatus_H = "";
        //红外检测_H
        private string BodyDetect_H = "";

        //温度_R
        private string Temperature_R = "";
        //湿度_R
        private string Humidity_R = "";
        //火警数据_R
        private string FireStatus_R = "";
        //光照_R
        private string LightStatus_R = "";
        //红外检测_R
        private string BodyDetect_R = "";


        #region Process the serial data stream

        /// <summary>
        /// 处理数据流
        /// </summary>
        private void ProcSerialStream(object sender, object e)
        {
            string ResString = ReadDataStream();
            System.Diagnostics.Debug.WriteLine(ResString);   //输入到debug日志的信息
            if ((ResString.StartsWith("A") || ResString.StartsWith("B")) && ResString.EndsWith(";"))
            {
                //测试用语句
                //MessageDialog message_dialog = new MessageDialog(ResString, "退出");
                //message_dialog.ShowAsync();   //不加await修饰符, No异步编程
                string[] SplitData = ResString.Split(' ');

                if (SplitData.Length == 6)
                {
                    if (ResString.StartsWith("A"))
                    {
                        int count = 0;
                        foreach (string data in SplitData)
                        {
                            if (count == 1)
                            {
                                Temperature_H = data;
                                Temperature.Text = Temperature_H;
                            }
                            else if (count == 2)
                            {
                                Humidity_H = data;
                                Humidity.Text = Humidity_H;
                            }
                            else if (count == 3)
                            {
                                FireStatus_H = data;
                                if (int.Parse(FireStatus_H)>=255)
                                {
                                    Hall.Background = 
                                }
                                FireStatus.Text = FireStatus_H;
                            }
                            else if (count == 4)
                            {
                                LightStatus_H = data;
                                LightStatus.Text = LightStatus_H;
                            }
                            else if (count == 5)
                            {
                                BodyDetect_H = data.Substring(0, data.Length - 1);

                            }
                            else
                            {
                            }
                            count++;                           
                        }
                        PostDataAsync("A", Temperature_H, Humidity_H, FireStatus_H, LightStatus_H, BodyDetect_H);
                    }
                    else if (ResString.StartsWith("B"))
                    {
                        int count = 0;
                        foreach (string data in SplitData)
                        {
                            if (count == 1)
                            {
                                Temperature_R = data;
                                Temperature.Text = Temperature_R;
                            }
                            else if (count == 2)
                            {
                                Humidity_R = data;
                                Humidity.Text = Humidity_R;
                            }
                            else if (count == 3)
                            {
                                FireStatus_R = data;
                                FireStatus.Text = FireStatus_R;
                            }
                            else if (count == 4)
                            {
                                LightStatus_R = data;
                                LightStatus.Text = LightStatus_R;
                            }
                            else if (count == 5)
                            {
                                BodyDetect_R = data.Substring(0, data.Length - 1);

                            }
                            else
                            {
                            }
                            count++;                            
                        }
                        PostDataAsync("B", Temperature_R, Humidity_R, FireStatus_R, LightStatus_R, BodyDetect_R);
                    }
                }
                else
                {
                    return;
                }

            }
        }

        #endregion Process the serial data stream

        #region Read the data stream
        /// <summary>
        /// 读取完整的字符串流（以 ";" 为分隔符）
        /// </summary>
        public string ReadDataStream()
        {
            string resString = "";
            int asciiCode = 2;
            do
            {
                uint checkPoint = serialPort.BytesToRead(1);   // checkPoint表示缓存区是否还有字节需要读取，以免进入系统报错
                if (checkPoint != 0)
                {
                    asciiCode = serialPort.ReadByte();   //return single string's ASCII code

                    if (AsciiToChar(asciiCode) == ";")
                    {
                        /// <summary>
                        /// Debug专用提示窗
                        /// </summary>
                        ////弹出提示框
                        //MessageDialog message_dialog = new MessageDialog(resString, "退出");
                        //message_dialog.ShowAsync();   //不加await修饰符, No异步编程
                        return resString + ";";
                    }
                    else
                    {
                        resString += AsciiToChar(asciiCode);
                    }
                }
                else return "";

            } while (AsciiToChar(asciiCode) != ";");
            return "";
        }

        /// <summary>
        /// C# ASCIIcode转string符
        /// </summary>
        public static string AsciiToChar(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        #endregion Read the data stream

        #region Event Contral
        /// <summary>
        /// 控制显示COM UI的函数
        /// </summary>
        private void Hall_Click(object sender, RoutedEventArgs e)
        {
            DataStatus = (int)DataDisplay.RoomHall;
        }

        private void RoomAlpha_Click(object sender, RoutedEventArgs e)
        {
            DataStatus = (int)DataDisplay.RoomAlpha;
        }

        #endregion Event Contral

        #region Post Data

        private static readonly HttpClient client = new HttpClient();
        public async void PostDataAsync(string Type, string pTemperture, string pHumidity, string pFireStatus, string pLightStatus, string pBodyStatus)
        {
            var values = new Dictionary<string, string>
                {
                    {"count", "1" },
                    {"Type_0", Type },
                    {"temperture_0", pTemperture },
                    {"Humidity_0", pHumidity },
                    {"FireStatus_0", pFireStatus },
                    {"LightStatus_0" ,pLightStatus},
                    {"BodyDetect_0", pBodyStatus }
                };
            var content = new FormUrlEncodedContent(values);
            try
            {
                var response = await client.PostAsync("http://115.159.36.210/api/onehome/upload", content);
                System.Diagnostics.Debug.WriteLine(response);
                var responseString = response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion Post Data
    }
}
