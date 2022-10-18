using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace Exchanger
{
    public class Server
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public List<User> clients = new List<User>(); // все подключения
        private string Ip = "127.0.0.1";
        private int port = 8888;
        public List<User> users = new List<User>();
        public bool working = false;
        public int id = 0;
        public int max_users = 0;
        public int max_req = 0;
        public void AddUser(User user)
        {
            users.Add(user);
        }
        public void WaitConnect()
        {
            while (working)
            {
                
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                User user2 = new User();
                user2.Stream = tcpClient.GetStream();
                user2.Client = tcpClient;
                user2.Id = id;
                id++;
                try
                {
                    if (clients.Count < max_users)
                    {
                        string message = string.Empty;
                        byte[] data_from_client = new byte[100];
                        StringBuilder string_from_client = new StringBuilder();

                        int bytes = user2.Stream.Read(data_from_client, 0, data_from_client.Length);

                        string str = Encoding.Unicode.GetString(data_from_client);
                        str = str.Replace("\0", "");
                        if (str.Contains("/Login:") == true)
                        {
                            Regex regex = new Regex("/Login: (\\w+) Password: (\\w+)");

                            Match match = regex.Match(str);
                            string login = match.Groups[1].Value;
                            string pass = match.Groups[2].Value;
                            if (users.Exists(s => s.Login == login) == true)
                            {
                                User user = users.Where(s => s.Login == login).FirstOrDefault();
                                if (user.Password == pass)
                                {
                                    user.Id = user2.Id;
                                    user.Stream = user2.Stream;
                                    user.Client = tcpClient;
                                    user.server = this;
                                    clients.Add(user);
                                    SendMessage("connected", user.Id);
                                    Log($"Server: {user.Login} is connected!");
                                }
                                else
                                {
                                    SendMessage("wrong pass", user.Id);
                                    Log($"Server: {user.Login} wrong pass!");
                                }

                            }
                            else
                            {
                                Log($"Server: {login} wrong pass!");
                                byte[] data = Encoding.Unicode.GetBytes("wrong login");
                                user2.Stream.Write(data, 0, data.Length);
                            }

                        }
                    }
                    else
                    {
                        byte[] data = Encoding.Unicode.GetBytes("{Error} Maximum number of users on the server");
                        user2.Stream.Write(data, 0, data.Length);
                        user2.Stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }
        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Parse(Ip), port);
            tcpListener.Start();
            working = true;
            Thread thread = new Thread(() => WaitConnect());
            thread.Start();
        }
        protected void AddConnection(User client)
        {
            clients.Add(client);
        }
       
       
        public void SendMessage(string message, int id)
        {
            try
            {
                Log($"Server: {message} [to {clients.Where(s => s.Id == id).First().Login}]!");
                byte[] data = Encoding.Unicode.GetBytes(message);
                clients.Where(s => s.Id == id).First().Stream.Write(data, 0, data.Length);
            }
            catch(Exception ex)
            {
                Log($"SendMessage Error: {ex.Message}");
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
                }
            }
        }
       
        protected internal void Disconnect()
        {
            working = false;
            for(int i = 0; i < clients.Count; i++)
            {
                SendMessage("Server is closed!", clients[i].Id);
            }
            tcpListener.Stop(); 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Stream.Close();
                clients[i].Client.Close();
            }
            Environment.Exit(0);
        }

    }
}