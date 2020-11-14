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
    //-- Not working yet via 443
    public class SslTcpClientPostUG
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
            Console.WriteLine("SslStreatm SOAP Test - Client connected.");
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

            string strSoap =
"POST /DirectConnect/Measurement/DistributionMeasurement.svc HTTP/1.1" + "\r\n" +
"Host: unionline.uniongas.com" + "\r\n" +
"Connection: Close" + "\r\n" +
"SOAPAction: https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementData.xsd/IDistributionMeasurement/GetHourlyMeasurement" + "\r\n" +
"Content-Type: text/xml; charset=UTF-8" + "\r\n" +
"Accept: */*" + "\r\n" +
"Accept-Language: en-US,en;q=0.9" + "\r\n\r\n" +
"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:meas='https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementData.xsd' xmlns:meas1='https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementRequest' xmlns:arr='http://schemas.microsoft.com/2003/10/Serialization/Arrays'>" +
   "<soapenv:Header/>" +
   "<soapenv:Body>" +
      "<meas:GetHourlyMeasurement>" +
         "<meas:request>" +
            "<meas1:Username>wsdm4052</meas1:Username>" +
            "<meas1:Password>Uniongas04</meas1:Password>" +
            "<meas1:FromDate>2020-07-10</meas1:FromDate>" +
            "<meas1:ToDate>2020-07-10</meas1:ToDate>" +
            "<meas1:ContractIds>" +
               "<arr:string>SA3863</arr:string>" +
            "</meas1:ContractIds>" +
            "<meas1:CompanyIds>" +
               "<arr:int>4052</arr:int>" +
            "</meas1:CompanyIds>" +
         "</meas:request>" +
      "</meas:GetHourlyMeasurement>" +
   "</soapenv:Body>" +
"</soapenv:Envelope><EOF>\r\n";

//            string requestMessage = "GET / HTTP/1.1" +
//"\r\nHost: ebilling.kitchener.ca" +
//"\r\nConnection: Close\r\n\r\n";
//            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);
            byte[] requestBytes = Encoding.UTF8.GetBytes(strSoap);

            sslStream.Write(requestBytes);
            sslStream.Flush();

            string serverMessage = ReadMessage(sslStream);
            Console.WriteLine("SslStreatm SOAP Test - Server says: \r\n {0} \r\n", serverMessage);

            client.Close();
            Console.WriteLine("SslStreatm SOAP Test - Client closed.");
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

        public static void TESTUG_Main(string[] args)
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
            machineName = "unionline.uniongas.com";
            serverCertificateName = "unionline.uniongas.com";

            RunClient(machineName, serverCertificateName);

            Console.ReadKey();
        }
    }
}
