//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNet.OData;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web.Http;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using UnionGasImporter.Models;
//using UnionGasImporter.CustomExtensions;
//using System.Collections;
//using System.Net.Security;
//using System.Net.Sockets;
//using System.Security.Authentication;
//using System.Security.Cryptography.X509Certificates;
//using System.IO;

//namespace UnionGasImporter.Controllers
//{
//    //-- SslStream on SOAP POST 2020.07.10
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UnionGasSslStreamController : ControllerBase
//    {
//        private readonly AppSettings _appSettings;
//        public UnionGasSslStreamController(IOptions<AppSettings> appSettings)
//        {
//            _appSettings = appSettings.Value;

//        }

//        string sTmp = string.Empty;
//        string sResults = string.Empty;
//        string sTmpResult = string.Empty;
//        string sQues = string.Empty;
//        string[] separatingStrings = { "||", "<BR>" };


//        // GET api/[controller]
//        [HttpGet("{dr?}")]
//        public ActionResult<IEnumerable<string>> Get(string dr)
//        {
//            string[] sDateArr = new string[2];
//            DateTime start_date, end_date;
//            string startDate, endDate;

//            if (string.IsNullOrEmpty(dr.Trim()))
//            {
//                start_date = DateTime.Now;
//                end_date = start_date.AddDays(1);
//                startDate = "2020-05-01";
//                endDate = "2020-05-02";
//            }
//            else if (dr.IndexOf("-") > 0)
//            {
//                try
//                {
//                    sDateArr = dr.Split(new Char[] { '-' });
//                    startDate = sDateArr[0].Substring(0, 4) + "-" + sDateArr[0].Substring(4, 2) + "-" + sDateArr[0].Substring(6, 2);
//                    endDate = sDateArr[1].Substring(0, 4) + "-" + sDateArr[1].Substring(4, 2) + "-" + sDateArr[1].Substring(6, 2);

//                    start_date = DateTime.Parse(sDateArr[0].Substring(0, 4) + "-" + sDateArr[0].Substring(4, 2) + "-" + sDateArr[0].Substring(6, 2));
//                    end_date = DateTime.Parse(sDateArr[1].Substring(0, 4) + "-" + sDateArr[1].Substring(4, 2) + "-" + sDateArr[1].Substring(6, 2));
//                    end_date = end_date.AddDays(1);
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception("Date format error! " + ex.Message);
//                }

//            }
//            else
//            {
//                throw new Exception("Date format error!");
//            }


//            string soapInd =

//                "<meas:GetDailyMeasurement>" +

//                       "<meas:request>" +

//                           "<meas1:Username>wsdm4052</meas1:Username>" +

//                           "<meas1:Password>Uniongas04</meas1:Password>" +

//                           $"<meas1:FromDate>{startDate}</meas1:FromDate>" +

//                           $"<meas1:ToDate>{endDate}</meas1:ToDate>" +

//                           "<meas1:ContractIds>" +

//                                   "<arr:string>SA3863</arr:string>" +

//                           "</meas1:ContractIds>" +

//                            "<meas1:CompanyIds>" +

//                                   "<arr:int>4052</arr:int>" +

//                             "</meas1:CompanyIds>" +

//                        "</meas:request>" +

//                  "</meas:GetDailyMeasurement>";


//            string soapReq = _appSettings.APP_SOAP_HEADER_OPEN + soapInd + _appSettings.APP_SOAP_HEADER_CLOSE;

//            //-- var sResult = JsonConvert.DeserializeObject(soapReq.PostDailySoapRequest());

//            dynamic tmpObj = JsonConvert.DeserializeObject(soapReq.PostDailySoapRequest());

//            var sTmp = tmpObj.Meter[2].Measurements.Measurement[0];
//            int i1 = tmpObj.Meter[2].Measurements.Measurement.Count;
//            var sRead = tmpObj.Meter[2].Measurements.Measurement[0].UncorrectedCCF.Value;

//            //-- return Ok(sResult);
//            return Ok(sTmp);

//        }

//        // GET api/values/5
//        //[HttpGet("{id}")]
//        //public ActionResult<string> Get(int id)
//        //{
//        //    return string.Empty;
//        //}

//        // POST api/values
//        [HttpPost]
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/values/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }

//        private static Hashtable certificateErrors = new Hashtable();

//        // The following method is invoked by the RemoteCertificateValidationDelegate.
//        public static bool ValidateServerCertificate(
//              object sender,
//              X509Certificate certificate,
//              X509Chain chain,
//              SslPolicyErrors sslPolicyErrors)
//        {
//            if (sslPolicyErrors == SslPolicyErrors.None)
//                return true;

//            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

//            // Do not allow this client to communicate with unauthenticated servers.
//            return false;
//        }
//        public static void RunClient(string machineName, string serverName)
//        {
//            // Create a TCP/IP client socket.
//            // machineName is the host running the server application.
//            TcpClient client = new TcpClient(machineName, 443);
//            Console.WriteLine("SslStreatm Test - Client connected.");
//            // Create an SSL stream that will close the client's stream.
//            SslStream sslStream = new SslStream(
//                client.GetStream(),
//                false,
//                new RemoteCertificateValidationCallback(ValidateServerCertificate),
//                null
//                );
//            // The server name must match the name on the server certificate.
//            try
//            {
//                sslStream.AuthenticateAsClient(serverName);
//            }
//            catch (AuthenticationException e)
//            {
//                Console.WriteLine("Exception: {0}", e.Message);
//                if (e.InnerException != null)
//                {
//                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
//                }
//                Console.WriteLine("Authentication failed - closing the connection.");
//                client.Close();
//                return;
//            }

//            //-- Signal the end of the message using the "<EOF>".
//            //-- byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
//            string requestMessage = "GET /services/cok_st_tax_calculator.aspx HTTP/1.1" +
//"\r\nHost: app2.kitchener.ca" +
//"\r\nConnection: Close\r\n\r\n";
//            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);

//            sslStream.Write(requestBytes);
//            sslStream.Flush();

//            string serverMessage = ReadMessage(sslStream);
//            Console.WriteLine("SslStreatm Test - Server says: \r\n {0} \r\n", serverMessage);

//            client.Close();
//            Console.WriteLine("SslStreatm Test - Client closed.");
//        }

//        public static string ReadMessage(SslStream sslStream)
//        {
//            // Read the  message sent by the server.
//            // The end of the message is signaled using the
//            // "<EOF>" marker.
//            byte[] buffer = new byte[2048];
//            StringBuilder messageData = new StringBuilder();
//            int bytes = -1;
//            do
//            {
//                bytes = sslStream.Read(buffer, 0, buffer.Length);

//                // Use Decoder class to convert from bytes to UTF8
//                // in case a character spans two buffers.
//                Decoder decoder = Encoding.UTF8.GetDecoder();
//                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
//                decoder.GetChars(buffer, 0, bytes, chars, 0);
//                messageData.Append(chars);
//                // Check for EOF.
//                if (messageData.ToString().IndexOf("<EOF>") != -1)
//                {
//                    break;
//                }
//            } while (bytes != 0);

//            return messageData.ToString();
//        }

//    }




//    public class SslTcpClient
//    {
//        private static Hashtable certificateErrors = new Hashtable();

//        // The following method is invoked by the RemoteCertificateValidationDelegate.
//        public static bool ValidateServerCertificate(
//              object sender,
//              X509Certificate certificate,
//              X509Chain chain,
//              SslPolicyErrors sslPolicyErrors)
//        {
//            if (sslPolicyErrors == SslPolicyErrors.None)
//                return true;

//            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

//            // Do not allow this client to communicate with unauthenticated servers.
//            return false;
//        }
//        public static void RunClient(string machineName, string serverName)
//        {
//            // Create a TCP/IP client socket.
//            // machineName is the host running the server application.
//            TcpClient client = new TcpClient(machineName, 443);
//            Console.WriteLine("SslStreatm Test - Client connected.");
//            // Create an SSL stream that will close the client's stream.
//            SslStream sslStream = new SslStream(
//                client.GetStream(),
//                false,
//                new RemoteCertificateValidationCallback(ValidateServerCertificate),
//                null
//                );
//            // The server name must match the name on the server certificate.
//            try
//            {
//                sslStream.AuthenticateAsClient(serverName);
//            }
//            catch (AuthenticationException e)
//            {
//                Console.WriteLine("Exception: {0}", e.Message);
//                if (e.InnerException != null)
//                {
//                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
//                }
//                Console.WriteLine("Authentication failed - closing the connection.");
//                client.Close();
//                return;
//            }

//            //-- Signal the end of the message using the "<EOF>".
//            //-- byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
//            string requestMessage = "GET /services/cok_st_tax_calculator.aspx HTTP/1.1" +
//"\r\nHost: app2.kitchener.ca" +
//"\r\nConnection: Close\r\n\r\n";
//            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);

//            sslStream.Write(requestBytes);
//            sslStream.Flush();

//            string serverMessage = ReadMessage(sslStream);
//            Console.WriteLine("SslStreatm Test - Server says: \r\n {0} \r\n", serverMessage);

//            client.Close();
//            Console.WriteLine("SslStreatm Test - Client closed.");
//        }
//        static string ReadMessage(SslStream sslStream)
//        {
//            // Read the  message sent by the server.
//            // The end of the message is signaled using the
//            // "<EOF>" marker.
//            byte[] buffer = new byte[2048];
//            StringBuilder messageData = new StringBuilder();
//            int bytes = -1;
//            do
//            {
//                bytes = sslStream.Read(buffer, 0, buffer.Length);

//                // Use Decoder class to convert from bytes to UTF8
//                // in case a character spans two buffers.
//                Decoder decoder = Encoding.UTF8.GetDecoder();
//                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
//                decoder.GetChars(buffer, 0, bytes, chars, 0);
//                messageData.Append(chars);
//                // Check for EOF.
//                if (messageData.ToString().IndexOf("<EOF>") != -1)
//                {
//                    break;
//                }
//            } while (bytes != 0);

//            return messageData.ToString();
//        }
//        private static void DisplayUsage()
//        {
//            Console.WriteLine("To start the client specify:");
//            Console.WriteLine("clientSync machineName [serverName]");
//            Environment.Exit(1);
//        }
//        public static void TestMain(string[] args)
//        {
//            string serverCertificateName = null;
//            string machineName = null;
//            if (args == null || args.Length < 1)
//            {
//                DisplayUsage();
//            }
//            // User can specify the machine name and server name.
//            // Server name must match the name on the server's certificate.
//            machineName = args[0];
//            if (args.Length < 2)
//            {
//                serverCertificateName = machineName;
//            }
//            else
//            {
//                serverCertificateName = args[1];
//            }
//            SslTcpClient.RunClient(machineName, serverCertificateName);

//            Console.ReadKey();
//        }
//    }
//}
