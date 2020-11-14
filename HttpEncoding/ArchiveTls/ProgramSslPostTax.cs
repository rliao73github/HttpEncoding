using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace HttpEncoding
{
    //-- via 443
    public class SslTcpClientPostTax
    {
        private static Hashtable certificateErrors = new Hashtable();

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        public static void RunClient(string machineName, string serverName)
        {
            // Create a TCP/IP client socket.
            // machineName is the host running the server application.
            TcpClient client = new TcpClient(machineName, 443);
            Console.WriteLine("SslStreatm tax Test - Client connected.");
            // Create an SSL stream that will close the client's stream.
            SslStream sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );
            // The server name must match the name on the server certificate.
            try
            {
                sslStream.AuthenticateAsClient(serverName);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }

            //-- Signal the end of the message using the "<EOF>".
            //-- set required http header
            //restRequest.AddHeader("Accept", "application/xml");
            //restRequest.AddHeader("Content-Type", "text/xml");
            //restRequest.AddHeader("SOAPAction", "https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementData.xsd/IDistributionMeasurement/GetDailyMeasurement");

            string requestMessage = "POST /modules/tax/Partial/Calculator/Calculate.aspx HTTP/1.1" +
"\r\nHost: www.kitchener.ca" +
"\r\nConnection: Close" +
"\r\nContent-Length: 311" + 
"\r\nX-Requested-With: XMLHttpRequest" +
"\r\nContent-Type: application/x-www-form-urlencoded; charset=UTF-8" +
"\r\nStreetNumber=79&StreetName=REDTAIL+ST&StreetUnit=&SearchStreetAddressTermsOfUse=on" +
            "\r\n\r\n";
                        //byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);
                        byte[] requestBytes = Encoding.UTF8.GetBytes(requestMessage);

            sslStream.Write(requestBytes);
            sslStream.Flush();

            string serverMessage = ReadMessage(sslStream);
            Console.WriteLine("SslStreatm tax Test - Server says: \r\n {0} \r\n", serverMessage);

            client.Close();
            Console.WriteLine("SslStreatm tax Test - Client closed.");
        }
        static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            int count = 0;
            do
            {
                count++;
                if (count == 2)
                    break;

                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);

                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }
        private static void DisplayUsage()
        {
            Console.WriteLine("To start the client specify:");
            Console.WriteLine("clientSync machineName [serverName]");
            Environment.Exit(1);
        }

        public static void TEST_Main(string[] args)
        {
            string serverCertificateName = null;
            string machineName = null;
            if (args == null || args.Length < 1)
            {
                DisplayUsage();
            }
            // User can specify the machine name and server name.
            // Server name must match the name on the server's certificate.
            machineName = args[0];
            if (args.Length < 2)
            {
                serverCertificateName = machineName;
            }
            else
            {
                serverCertificateName = args[1];
            }
            machineName = "www.kitchener.ca";
            serverCertificateName = "www.kitchener.ca";

            RunClient(machineName, serverCertificateName);

            Console.ReadKey();
        }
    }
}
