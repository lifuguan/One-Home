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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SerialPort
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StatusFrame : Page
    {
        SerialPort serialPort = new SerialPort();
        public StatusFrame()
        {
            
            this.InitializeComponent();
            string portName = "COM";
            for (int i = 0; i < 10; i++)
            {
                portName += i.ToString();
                serialPort.Open(portName);
                portName = "COM";
            }
            
        }
        
        public void PortStatusContral(string PortName)
        {
            PortStutas.Text = PortName;    //问题
        }


        private void Hall_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RoomAlpha_Click(object sender, RoutedEventArgs e)
        {
            
            
            uint length = serialPort.BytesToRead(1);
            int asciiCode = serialPort.ReadByte();   //return single string's ASCII code
            string resString = AsciiToChar(asciiCode);
            int i = 1;
        }
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
    }
}
