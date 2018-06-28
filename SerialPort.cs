using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;

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
        ///<param name="pAddress">StaatusFrame类的实例的地址</param>
        ///<param name="baudRate">baud rate of COM port 传输速率</param>
        ///<param name="parity">type of data parity</param>
        ///<param name="dataBits">Number of data bits</param>
        ///<param name="stopbits">Number of stop</param>
        public async Task<bool> Open(string portName, StatusFrame pAddress, uint baudRate = 9600, SerialParity parity = SerialParity.None, ushort dataBits = 8, SerialStopBitCount stopBits = SerialStopBitCount.One)
        {
            //close open port 关闭当前正在打开的串口
            //防止错误覆盖
            Close();
            
            //get a list of devices from the given portname
            string selector = SerialDevice.GetDeviceSelector(portName);

            // Get a list of devices that match the given name
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            // If any device found...
            if (devices.Any())
            {
                // Get first device (should be only device)
                DeviceInformation deviceInfo = devices.First();

                // Create a serial port device from the COM port device ID
                this.SerialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);

                // If serial device is valid...
                if (this.SerialDevice != null)
                {
                    // Setup serial port configuration
                    this.SerialDevice.StopBits = stopBits;
                    this.SerialDevice.Parity = parity;
                    this.SerialDevice.BaudRate = baudRate;
                    this.SerialDevice.DataBits = dataBits;

                    // Create a single device writer for this port connection
                    this.dataWriterObject = new DataWriter(this.SerialDevice.OutputStream);

                    // Create a single device reader for this port connection
                    this.dataReaderObject = new DataReader(this.SerialDevice.InputStream);

                    // Allow partial reads of the input stream
                    this.dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;


                    pAddress.PortName = portName;
                    
                    // Port is now open
                    this.IsOpen = true;
                }

            }
         
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

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Write data to the open COM port
        /// 写入数据
        /// </summary>
        /// <param name="data">Array of data byes to be written</param>
        public async void WriteAsync(byte[] data)
        {
            // Write block of data to serial port
            this.dataWriterObject.WriteBytes(data);

            // Transfer data to the serial device now
            await this.dataWriterObject.StoreAsync();

            // Flush the data out to the serial device now
            await this.dataWriterObject.FlushAsync();
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Read a byte from the open COM port
        /// 读出数据
        /// </summary>
        /// <returns>The next byte received by the specified port</returns>
        public byte ReadByte()
        {
            // Get the next byte of data from the port
            return (byte)this.dataReaderObject.ReadByte();
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return the number of bytes to read from COM port
        /// </summary>
        public uint BytesToRead(uint bufferSize)
        {
            // Load buffer of bytes from port
            LoadSerialDataAsync(bufferSize);

            // Total bytes to read are those unconsumed from the port
            return this.dataReaderObject.UnconsumedBufferLength;
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Load serial data from the serial port
        /// </summary>
        public void LoadSerialDataAsync(uint bufferSize)
        {
            // If no data left to consume in buffer...
            if (dataReaderObject.UnconsumedBufferLength == 0)
            {
                try
                {
                    IAsyncOperation<uint> taskLoad = dataReaderObject.LoadAsync(bufferSize);
                    taskLoad.AsTask().Wait(0);
                }
                // Dump exceptions
                catch
                {
                    /// <summary>
                    /// Debug专用提示窗
                    /// </summary>           
                    MessageDialog message_dialog = new MessageDialog("hello bug", "退出");
                    message_dialog.ShowAsync();   //不加await修饰符, No异步编程
                }
            }
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
