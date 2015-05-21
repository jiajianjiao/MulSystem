using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows;
using System.Configuration;

namespace MultimediaSysClient
{
    /// <summary>
    /// 异步TCP通信客户端类，可以使用Tcp方式向服务器端发起连接，发送和接收报文
    /// </summary>
    public class AsyncTcpClient : IDisposable
    {
        private TcpClient tcpClient;
        private bool disposed = false;
        private int retries = 0;


        #region 构造函数

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteEP">远端服务器终结点</param>
        public AsyncTcpClient(IPEndPoint remoteEP)
            : this(new[] { remoteEP.Address }, remoteEP.Port)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteEP">远端服务器终结点</param>
        /// <param name="localEP">本地客户端终结点</param>
        public AsyncTcpClient(IPEndPoint remoteEP, IPEndPoint localEP)
            : this(new[] { remoteEP.Address }, remoteEP.Port, localEP)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteIPAddress">远端服务器IP地址</param>
        /// <param name="remotePort">远端服务器端口</param>
        public AsyncTcpClient(IPAddress remoteIPAddress, int remotePort)
            : this(new[] { remoteIPAddress }, remotePort)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteIPAddress">远端服务器IP地址</param>
        /// <param name="remotePort">远端服务器端口</param>
        /// <param name="localEP">本地客户端终结点</param>
        public AsyncTcpClient(
          IPAddress remoteIPAddress, int remotePort, IPEndPoint localEP)
            : this(new[] { remoteIPAddress }, remotePort, localEP)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteHostName">远端服务器主机名</param>
        /// <param name="remotePort">远端服务器端口</param>
        public AsyncTcpClient(string remoteHostName, int remotePort)
            : this(Dns.GetHostAddresses(remoteHostName), remotePort)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteHostName">远端服务器主机名</param>
        /// <param name="remotePort">远端服务器端口</param>
        /// <param name="localEP">本地客户端终结点</param>
        public AsyncTcpClient(
          string remoteHostName, int remotePort, IPEndPoint localEP)
            : this(Dns.GetHostAddresses(remoteHostName), remotePort, localEP)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteIPAddresses">远端服务器IP地址列表</param>
        /// <param name="remotePort">远端服务器端口</param>
        public AsyncTcpClient(IPAddress[] remoteIPAddresses, int remotePort)
            : this(remoteIPAddresses, remotePort, null)
        {
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        /// <param name="remoteIPAddresses">远端服务器IP地址列表</param>
        /// <param name="remotePort">远端服务器端口</param>
        /// <param name="localEP">本地客户端终结点</param>
        public AsyncTcpClient(
          IPAddress[] remoteIPAddresses, int remotePort, IPEndPoint localEP)
        {
            this.Addresses = remoteIPAddresses;
            this.Port = remotePort;
            this.LocalIPEndPoint = localEP;
            this.Encoding = Encoding.Default;

            if (this.LocalIPEndPoint != null)
            {
                this.tcpClient = new TcpClient(this.LocalIPEndPoint);
            }
            else
            {
                this.tcpClient = new TcpClient();
            }

            Retries = 3;
            RetryInterval = 5;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否已与服务器建立连接
        /// </summary>
        public bool Connected { get { return tcpClient.Client.Connected; } }
        /// <summary>
        /// 远端服务器的IP地址列表
        /// </summary>
        public IPAddress[] Addresses { get; private set; }
        /// <summary>
        /// 远端服务器的端口
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 连接重试次数
        /// </summary>
        public int Retries { get; set; }
        /// <summary>
        /// 连接重试间隔
        /// </summary>
        public int RetryInterval { get; set; }
        /// <summary>
        /// 远端服务器终结点
        /// </summary>
        public IPEndPoint RemoteIPEndPoint
        {
            get { return new IPEndPoint(Addresses[0], Port); }
        }
        /// <summary>
        /// 本地客户端终结点
        /// </summary>
        protected IPEndPoint LocalIPEndPoint { get; private set; }
        /// <summary>
        /// 通信所使用的编码
        /// </summary>
        public Encoding Encoding { get; set; }

        #endregion

        #region 连接

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <returns>异步TCP客户端</returns>
        public AsyncTcpClient Connect()
        {
            if (!Connected)
            {
                // start the async connect operation
                tcpClient.BeginConnect(Addresses, Port, HandleTcpServerConnected, tcpClient);
            }

            return this;
        }

        /// <summary>
        /// 关闭与服务器的连接
        /// </summary>
        /// <returns>异步TCP客户端</returns>
        public AsyncTcpClient Close()
        {
            if (Connected)
            {
                retries = 0;
                tcpClient.Close();
                RaiseServerDisconnected(Addresses, Port);
            }

            return this;
        }

        #endregion

        #region 接收报文

        private void HandleTcpServerConnected(IAsyncResult ar)
        {
            try
            {
                tcpClient.EndConnect(ar);
                RaiseServerConnected(Addresses, Port);
                retries = 0;
            }
            catch (Exception ex)
            {
                //ExceptionHandler.Handle(ex);
                if (retries > 0)
                {
                    //Logger.Debug(string.Format(CultureInfo.InvariantCulture,"Connect to server with retry {0} failed.", retries));
                    MessageBox.Show(string.Format("第{0}次尝试链接服务器失败。",retries));
                }

                retries++;
                if (retries > Retries)
                {
                    // we have failed to connect to all the IP Addresses, 
                    // connection has failed overall.
                    RaiseServerExceptionOccurred(Addresses, Port, ex);
                    return;
                }
                else
                {
                    //Logger.Debug(string.Format(CultureInfo.InvariantCulture,"Waiting {0} seconds before retrying to connect to server.",RetryInterval));                    
                    Thread.Sleep(TimeSpan.FromSeconds(RetryInterval));
                    Connect();
                    return;
                }
            }

            // we are connected successfully and start asyn read operation.
            //tcpClient.EndConnect(ar);
            //RaiseServerConnected(Addresses, Port);
            //retries = 0;
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            tcpClient.GetStream().BeginRead(
              buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
        }

        private void HandleDatagramReceived(IAsyncResult ar)
        {
            if (tcpClient.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();

                int numberOfReadBytes = 0;
                try
                {
                    numberOfReadBytes = stream.EndRead(ar);
                }
                catch
                {
                    numberOfReadBytes = 0;
                }

                if (numberOfReadBytes == 0)
                {
                    // connection has been closed
                    Close();
                    return;
                }

                // received byte and trigger event notification
                byte[] buffer = (byte[])ar.AsyncState;
                byte[] receivedBytes = new byte[numberOfReadBytes];
                Buffer.BlockCopy(buffer, 0, receivedBytes, 0, numberOfReadBytes);
                RaiseDatagramReceived(tcpClient, receivedBytes);
                RaisePlaintextReceived(tcpClient, receivedBytes);

                // then start reading from the network again
                stream.BeginRead(
                  buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 接收到数据报文事件
        /// </summary>
        public event EventHandler<ReceivedEventArgs<byte[]>> DatagramReceived;
        /// <summary>
        /// 接收到数据报文明文事件
        /// </summary>
        public event EventHandler<ReceivedEventArgs<string>> PlaintextReceived;

        private void RaiseDatagramReceived(TcpClient sender, byte[] datagram)
        {
            if (DatagramReceived != null)
            {
                DatagramReceived(this, new ReceivedEventArgs<byte[]>(sender, datagram));
            }
        }

        private void RaisePlaintextReceived(TcpClient sender, byte[] datagram)
        {
            if (PlaintextReceived != null)
            {
                PlaintextReceived(this, new ReceivedEventArgs<string>(sender, this.Encoding.GetString(datagram, 0, datagram.Length)));
            }
        }

        /// <summary>
        /// 与服务器的连接已建立事件
        /// </summary>
        public event EventHandler<ServerConnectEventArgs> ServerConnected;

        /// <summary>
        /// 与服务器的连接已断开事件
        /// </summary>
        public event EventHandler<ServerConnectEventArgs> ServerDisconnected;
        /// <summary>
        /// 与服务器的连接发生异常事件
        /// </summary>
        public event EventHandler<ServerExceptionOccurredEventArgs> ServerExceptionOccurred;

        private void RaiseServerConnected(IPAddress[] ipAddresses, int port)
        {
            if (ServerConnected != null)
            {
                ServerConnected(this, new ServerConnectEventArgs(ipAddresses, port));
            }
        }

        private void RaiseServerDisconnected(IPAddress[] ipAddresses, int port)
        {
            if (ServerDisconnected != null)
            {
                ServerDisconnected(this, new ServerConnectEventArgs(ipAddresses, port));
            }
        }

        private void RaiseServerExceptionOccurred(IPAddress[] ipAddresses, int port, Exception innerException)
        {
            if (ServerExceptionOccurred != null)
            {
                ServerExceptionOccurred(this, new ServerExceptionOccurredEventArgs(ipAddresses, port, innerException));
            }
        }

        #endregion

        #region 发送报文

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="datagram">报文</param>
        public void Send(byte[] datagram)
        {
            if (datagram == null)
                throw new ArgumentNullException("datagram");

            if (!Connected)
            {
                RaiseServerDisconnected(Addresses, Port);
                throw new InvalidProgramException(
                  "This client has not connected to server.");
            }

            tcpClient.GetStream().BeginWrite(
              datagram, 0, datagram.Length, HandleDatagramWritten, tcpClient);
        }

        private void HandleDatagramWritten(IAsyncResult ar)
        {
            ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="datagram">报文</param>
        public void Send(string datagram)
        {
            datagram = datagram + ConfigurationManager.AppSettings["CustomTerminatorSign"];  //后面加上自定义终结符
            Send(this.Encoding.GetBytes(datagram));
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed 
        /// and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Close();

                        if (tcpClient != null)
                        {
                            tcpClient = null;
                        }
                    }
                    catch (SocketException ex)
                    {
                        throw (ex);
                        // ExceptionHandler.Handle(ex);
                    }
                }

                disposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// Internal class to join the TCP client and buffer together for easy management in the server
    /// </summary>
    internal class TcpClientState
    {
        /// <summary>
        /// Constructor for a new Client
        /// </summary>
        /// <param name="tcpClient">The TCP client</param>
        /// <param name="buffer">The byte array buffer</param>
        public TcpClientState(TcpClient tcpClient, byte[] buffer)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            this.TcpClient = tcpClient;
            this.Buffer = buffer;
        }

        /// <summary>
        /// Gets the TCP Client
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Gets the Buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets the network stream
        /// </summary>
        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }
    }

    /// <summary>
    /// 连接异常事件参数（AsyncTcpClient中使用）
    /// </summary>
    public class ServerExceptionOccurredEventArgs : EventArgs
    {
        /// <summary>
        /// 与服务器的连接发生异常事件参数
        /// </summary>
        /// <param name="ipAddresses">服务器IP地址列表</param>
        /// <param name="port">服务器端口</param>
        /// <param name="innerException">内部异常</param>
        public ServerExceptionOccurredEventArgs(
          IPAddress[] ipAddresses, int port, Exception innerException)
        {
            if (ipAddresses == null)
                throw new ArgumentNullException("ipAddresses");

            this.Addresses = ipAddresses;
            this.Port = port;
            this.Exception = innerException;
        }

        /// <summary>
        /// 服务器IP地址列表
        /// </summary>
        public IPAddress[] Addresses { get; private set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 内部异常
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string s = string.Empty;
            foreach (var item in Addresses)
            {
                s = s + item.ToString() + ',';
            }
            s = s.TrimEnd(',');
            s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);

            return s;
        }
    }

    /// <summary>
    /// 接收到数据报文事件参数（AsyncTcpClient中使用）
    /// </summary>
    /// <typeparam name="T">报文类型</typeparam>
    public class ReceivedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 接收到数据报文事件参数
        /// </summary>
        /// <param name="tcpClient">客户端</param>
        /// <param name="datagram">报文</param>
        public ReceivedEventArgs(TcpClient tcpClient, T datagram)
        {
            TcpClient = tcpClient;
            Datagram = datagram;
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public TcpClient TcpClient { get; private set; }
        /// <summary>
        /// 报文
        /// </summary>
        public T Datagram { get; private set; }
    }

    /// <summary>
    /// 与服务器的连接建立(断开)事件参数（AsyncTcpClient中使用）
    /// </summary>
    public class ServerConnectEventArgs : EventArgs
    {
        /// <summary>
        /// 与服务器的连接已建立事件参数
        /// </summary>
        /// <param name="ipAddresses">服务器IP地址列表</param>
        /// <param name="port">服务器端口</param>
        public ServerConnectEventArgs(IPAddress[] ipAddresses, int port)
        {
            if (ipAddresses == null)
                throw new ArgumentNullException("ipAddresses");

            this.Addresses = ipAddresses;
            this.Port = port;
        }

        /// <summary>
        /// 服务器IP地址列表
        /// </summary>
        public IPAddress[] Addresses { get; private set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string s = string.Empty;
            foreach (var item in Addresses)
            {
                s = s + item.ToString() + ',';
            }
            s = s.TrimEnd(',');
            s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);

            return s;
        }
    }

}
