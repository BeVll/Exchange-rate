﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Exchanger
{
    public class Client
    {
        public TcpClient client = new TcpClient();
        NetworkStream stream;
        public string Connect(string address, int port, string login, string pass)
        {
            try
            {
                client.Connect(IPAddress.Parse(address), port);
                stream = client.GetStream();
                SendMessage($"/Login: {login} Password: {pass}");
                Log($"Client: /Login: {login} Password: {pass}");
                return GetConnect();
            }
            catch (Exception ex)
            {
                Log($"Connect Error: {ex.Message}");
                return $"Connect error: {ex.Message}";
            }
        }
        public void Log(string str)
        {
            string log = $"[{DateTime.Now.ToString()}] {str}";
            if (File.Exists("log.txt"))
                File.AppendAllText("log.txt", log);
            else
                File.WriteAllText("log.txt", log);
        }
        public string GetExchange(string cur)
        {
            SendMessage(cur);
            return GetMessage();
        }
        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log($"SendMessage Error: {ex.Message}");
                
            }
        }
        public string GetConnect()
        {
            byte[] data = new byte[64]; 
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            return builder.ToString();
        }
 
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
           

            return builder.ToString();
        }
        public void Close()
        {
            client.Close();
            stream.Close();
        }
    }
}
