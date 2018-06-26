using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;


namespace SerialPort
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Abstracted COM port interface suitable for UWP applications
    /// 使 COM port 适用于UWP应用程序
    /// </summary>
    public class SerialPort
    {
        ///<summary>
        ///Open port to make a connect
        ///打开串口开始连接
        ///</summary>
        ///<param name="portName">Name of COM port to open</param>
        ///<param name="baudRate">baud rate of COM port 传输速率</param>
        ///<param name="parity">type of data parity</param>
        ///<param name="dataBits">Number of data bits</param>
        ///<param name="stopbits">Number of stop</param>
        public async Task<bool> Open(string portName, uint baudRate = 9600, SerialParity parity = SerialParity.None, ushort dataBits = 8, SerialStopBitCount stopBits = SerialStopBitCount.One)
        {
            //close open port 关闭当前正在打开的串口
            //防止错误覆盖
            Close();
            
            //get a list of devices from the given portname
            string selector = SerialDevice.GetDeviceSelector(portName);


            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            return this.IsOpen;
        }


        ///////////////////////////////////////////////////////////////
        ///<summary>
        ///close the open port connection to the current device
        ///关闭目标串口
        ///</summary>
        
        public void Close()
        {
            //if serial device defined
            if (this.SerialDevice != null)
            {
                this.SerialDevice.Dispose();
                this.SerialDevice = null;
            }

            //if data reader is defined
            if (this.dataReaderObject != null)
            {
                //dispose and clear stream
                this.dataReaderObject.DetachStream();

                //dispose and clear data reader
                this.dataReaderObject.Dispose();
                this.dataReaderObject = null;
            }

            //if data writer is defined
            if (this.dataWriterObject != null)
            {
                //Datach writer stream
                this.dataWriterObject.DetachStream();

                //Dispose and clear data writer
                this.dataWriterObject.Dispose();
                this.dataWriterObject = null;
            }

            ///前面全是准备和清空工作

            //Port now closed
            this.IsOpen = false;
        }

        /////////////////////////////////////////////////////////////
        /// <summary>
        /// Load serial data from the serial port  
        /// 从串口中装载数据
        /// </summary>
        public void LoadSerialData(uint bufferSize)
        {



        }


        /////////////////////////////////////////////////////////////
        ///<summary>
        ///Flag that indicates if COM port is open
        ///</summary>
        public bool IsOpen { get; set; }
        /////////////////////////////////////////////////////////////
        /// <summary>
        /// the data reader used to read data from the COM port
        /// </summary>
        private SerialDevice SerialDevice;
        /////////////////////////////////////////////////////////////
        /// <summary>
        /// the data writer used to send data from the COM port 
        /// </summary>
        private DataWriter dataWriterObject;

        /////////////////////////////////////////////////////////////
        /// <summary>
        /// the data Reader used to send data to the COM port
        /// </summary>
        private DataReader dataReaderObject;


    }
}
