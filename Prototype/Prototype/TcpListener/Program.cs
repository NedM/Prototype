
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PrototypeTcpListener
{
    public class Program
    {
        private const int DEFAULT_PORT = 54321;

        private static IPAddress _ipAddress = null;
        private static int _portNumber = 0;

        public static void Main(string[] args)
        {
            // Catch all unhandled exceptions in all threads.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length > 0)
            {
                _ipAddress = IPAddress.Parse(args[0]);
                if (args.Length > 1)
                {
                    _portNumber = int.Parse(args[1]);
                }
            }

            if (_ipAddress == null)
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _ipAddress = ip;
                        break;
                    }
                }
            }

            if (_portNumber == 0)
            {
                _portNumber = DEFAULT_PORT;
            }

            TcpListener listener = null;
            try
            {
                Console.WriteLine("Starting listener on {0}:{1}", _ipAddress, _portNumber);

                listener = new TcpListener(new IPEndPoint(_ipAddress, _portNumber));

                //Start listening for client requests
                listener.Start();
                Console.WriteLine("Started the TCP listener.");

                bool listen = true;
                Byte[] bytes = new Byte[256];
                string data = null;

                //Infinite listener loop
                while (listen)
                {
                    Console.WriteLine("Waiting for TCP connection...");

                    //Socket socketConnection = listener.AcceptSocket();
                    TcpClient clientConnection = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = clientConnection.GetStream();
                    int bytesRead = 0;
                    //Loop to read all the data
                    while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate bytes to string data
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                        Console.WriteLine("Received: {0}", data);

                        string response = "Message received and understood.";
                        byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(response);

                        stream.Write(responseBytes, 0, response.Length);
                        Console.WriteLine("Sent: {0}", response);

                        Console.WriteLine("Press [ENTER] to continue. Type \"exit\" to quit:");
                        string userInput = Console.ReadLine();
                        if (userInput.ToLower().CompareTo("exit") == 0)
                        {
                            listen = false;
                            Console.WriteLine("Stopping the TCP listener loop...");
                        }
                    }

                    //Close the connection
                    clientConnection.Close();
                }
            }
            catch (SocketException sockEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Socket error!".ToUpper());
                Console.ResetColor();
                Console.WriteLine(sockEx.ToString());
            }
            finally
            {
                if (null != listener)
                {
                    Console.WriteLine("Stopping TCP listener...");
                    listener.Stop();
                    Console.WriteLine("Stopped listener.");
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unhandled Exception:");
                Console.ResetColor();
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                // in release mode we don't throw an exception, if the logging failed
                // in debug rethrow to make development easier
#if DEBUG
                throw ex;
#endif
            }
        }
    }
}
