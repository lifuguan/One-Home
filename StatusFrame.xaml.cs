﻿using System;
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
using Windows.UI.Popups;

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
        /// <summary>
        /// 控制显示COM UI的函数
        /// </summary>
        public void PortStatusContral(string PortName)
        {
            PortStutas.Text = PortName;    //问题
        }


        private void Hall_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void RoomAlpha_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// Debug专用提示窗
            /// </summary>           
            MessageDialog message_dialog = new MessageDialog(ReadDataStream(), "退出");
            message_dialog.ShowAsync();   //不加await修饰符, No异步编程
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
                        return resString;
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
    }
}
