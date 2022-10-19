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
            request_count = 0;
        }
        public User(int id, NetworkStream stream, string login, string password)
        {
            Id = id;
            Stream = stream;
            Login = login;
            Password = password;
            request_count = 0;
        }
        public User(string login, string password) { 
            Login = login; 
            Password = password;
            request_count = 0;
        }
        public string GetMessage()
        { 
            try
            {
                if (Client.Connected == true)
                {
                    string message = string.Empty;
                    byte[] data_from_client = new byte[100];
                    int bytes = Stream.Read(data_from_client, 0, data_from_client.Length);
                    if (request_count < server.max_req)
                    {
                        message = Encoding.Unicode.GetString(data_from_client);
                        message = message.Replace("\0", "");
                        Regex regex = new Regex("(\\w+)-(\\w+)");
                        Match match = regex.Match(message);
                        string cur1 = match.Groups[1].Value;
                        string cur2 = match.Groups[2].Value;
                        double rate = ex.GetCurrency(cur1, cur2);
                        Log($"{Login}: {message}");
                        server.SendMessage(rate.ToString(), Id);
                        Log($"Server: {rate}");
                        lastsennded = rate.ToString();
                        request_count++;

                        return message;
                    }
                    server.SendMessage("{Error} Disconecting...The maximum number of requests has been reached. You can try after 1 minute", Id);
                    return "Disconnecting...The maximum number of requests has been reached";
                }
                else
                {
                    Client.Close();
                    Stream.Close();
                    server.RemoveClient(this);
                    Log($"Server: User {Login} is disconnected!");
                    return $"Server: User {Login} is disconnected!";
                }
            }
            catch (Exception ex)
            {
                Log($"GetMessage Error: {ex.Message}");
                return ex.Message;
            }
        }
        public void Log(string str)
        {
            lock (this)
            {
                string log = $"[{DateTime.Now.ToString()}] {str}";
                using (FileStream fs = new FileStream("log.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(log);
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}
