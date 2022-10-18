using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Windows.UI.Xaml;
using ChatWithBot;

namespace Exchanger
{
    public class User
    {

        public int Id { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Server server { get; set; }
        public string lastsennded { get; set; }
        public int request_count { get; set; }
        
        Exchanger ex = new Exchanger();
        public User()
        {

        }
        public User(int id, NetworkStream stream, string login, string password)
        {
            Id = id;
            Stream = stream;
            Login = login;
            Password = password;
        }
        public User(string login, string password) { 
            Login = login; 
            Password = password; 
        }
        public string GetMessage()
        {
           
            try
            {
                if (request_count >= server.max_req)
                {
                    string message = string.Empty;
                    byte[] data_from_client = new byte[100];
                    int bytes = Stream.Read(data_from_client, 0, data_from_client.Length);
                    message = Encoding.Unicode.GetString(data_from_client);
                    message = message.Replace("\0", "");
                    Regex regex = new Regex("(\\w+)-(\\w+)");
                    Match match = regex.Match(message);
                    string cur1 = match.Groups[1].Value;
                    string cur2 = match.Groups[2].Value;
                    double rate = ex.GetCurrency(cur1, cur2);
                    Log($"{Login}: {message}");
                    ;
                    server.SendMessage(rate.ToString(), Id);
                    Log($"Server: {rate}");
                    lastsennded = rate.ToString();
                    request_count++;
                   
                    return message;
                }
                server.SendMessage("{Error} Disconecting...\nThe maximum number of requests has been reached", Id);
                return "The maximum number of requests has been reached\nDisconnecting...";
            }
            catch (Exception ex)
            {
                Log($"GetMessage Error: {ex.Message}");
                return ex.Message;
            }
        }
        public void Log(string str)
        {
            string log = $"[{DateTime.Now.ToString()}] {str}";
            if (File.Exists(log))
                File.AppendAllText("log.txt", log);
            else
                File.WriteAllText("log.txt", log);
        }
    }
}
