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
    //-- {}{5} very interesting to see MCF Demo already had TLSv1.0, v1.1 disabled 
    //-- 
    /// <summary>
    /// -- content_length can be a little over, OK to use \r\n\r\n
    /// </summary>
    public class SslTcpClientMcfDemo
    {
        private static bool isFinished = false;
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
            Console.WriteLine("SslStreatm Test - Client connected.");
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
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                sslStream.AuthenticateAsClient(serverName, null,
                    (SslProtocols)ServicePointManager.SecurityProtocol, true);


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

            //-- 200 OK with Connection set to 'close'
            //--"\r\n<EOF>";  //-- <EOF> will return 500 error
            //            string requestMessage = "POST /DirectConnect/Measurement/DistributionMeasurement.svc HTTP/1.1\r\n" +
            //"Host: unionline.uniongas.com\r\n" +
            //"Connection: close\r\n" +
            //"Content-Length: 819\r\n" +   //-- 938
            //"Pragma: no-cache\r\n" +
            //"Cache-Control: no-cache\r\n" +
            //"Accept: */*\r\n" +
            //"SOAPAction: https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementData.xsd/IDistributionMeasurement/GetDailyMeasurement\r\n" +
            //"Content-Type: text/xml\r\n" +
            //"Sec-Fetch-Dest: empty\r\n" +
            //"Sec-Fetch-Mode: cors\r\n" +
            //"Sec-Fetch-Site: none\r\n" +
            //"\r\n" +  //-- extra \r\n is IMPORTANT!
            //"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:meas='https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementData.xsd' xmlns:meas1='https://unionline.uniongas.com/DirectConnect/Measurement/MeasurementRequest' xmlns:arr='http://schemas.microsoft.com/2003/10/Serialization/Arrays'>\r\n<soapenv:Header/>\r\n<soapenv:Body>\r\n<meas:GetDailyMeasurement>\r\n<meas:request>\r\n<meas1:Username>wsdm4052</meas1:Username>\r\n<meas1:Password>Uniongas04</meas1:Password>\r\n<meas1:FromDate>2020-07-17</meas1:FromDate>\r\n<meas1:ToDate>2020-07-17</meas1:ToDate>\r\n<meas1:ContractIds>\r\n<arr:string>SA3863</arr:string>\r\n</meas1:ContractIds>\r\n<meas1:CompanyIds>\r\n<arr:int>4052</arr:int>\r\n</meas1:CompanyIds>\r\n</meas:request>\r\n</meas:GetDailyMeasurement  >\r\n</soapenv:Body>\r\n</soapenv:Envelope>" +
            //"\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";

            //-- 200 OK POST 2020.07.19
            string requestMessage = "POST /sap/opu/odata/sap/ZMCF_USER_REGISTRATION_SRV/ZMCFS_USERDATASet?saml2=disabled&sap-client=300 HTTP/1.1\r\n" +
"Host: ebilling.kitchener.ca\r\n" +
"Connection: close\r\n" +
"Content-Length: 230\r\n" +
"Pragma: no-cache\r\n" +
"Cache-Control: no-cache\r\n" +
"Accept: application/json\r\n" +
"X-REQUESTED-WITH: XMLHttpRequest\r\n" +
"Content-Type: application/json\r\n" +
"Sec-Fetch-Site: none\r\n" +
"Sec-Fetch-Mode: cors\r\n" +
"Sec-Fetch-Dest: empty\r\n" +
"\r\n" +
"{\"UserName\":\"Tester20200716\",\"Fname\":\"RONG\",\"Lname\":\"Liao\",\"EmailAddr\":\"rongltest@gmail.com\",\"BuagId\":\"110112070\",\"Password\":\"Waterloo2019\",\"Billno\":\"928256931\",\"Billdate\":\"2020-5-1T00:00:00\",\"Billamt\":\"134.35\"}\r\n" +
"\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";

            byte[] requestBytes = Encoding.UTF8.GetBytes(requestMessage);

            sslStream.Write(requestBytes);
            sslStream.Flush();

            string serverMessage = ReadMessage(sslStream);
            Console.WriteLine("SslStreatm Test - Server says: \r\n {0} \r\n", serverMessage);

            client.Close();
            Console.WriteLine("SslStreatm Test - Client closed.");
        }
        static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            int bytes = -1;
            int count = 0;
            do
            {
                count++;
                //-- if (count == 5) break;
                if (count == 50) break;

                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                //Decoder decoder = Encoding.UTF8.GetDecoder();
                //char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                //decoder.GetChars(buffer, 0, bytes, chars, 0);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                sb.Clear();
                sb.Append(chars);
                messageData.Append(chars);

                // Check for EOF.
                int idx = sb.ToString().IndexOf("\r\n\r\n");
                if ( idx != -1)
                {
                    string strTmp = messageData.ToString().Substring(idx-30, 30);

                    //-- break;
                }
            } while (bytes > 0);

            return messageData.ToString();
        }
        private static void DisplayUsage()
        {
            Console.WriteLine("To start the client specify:");
            Console.WriteLine("clientSync machineName [serverName]");
            Environment.Exit(1);
        }

        public static void Main(string[] args)
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
            machineName = "mcfdemo.kitchener.ca";
            serverCertificateName = "mcfdemo.kitchener.ca";

            RunClient(machineName, serverCertificateName);


            isFinished = true; 

            Console.ReadKey();

        }
    }
}
