using Exchanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Read
    {
        public Thread Thread { get; set; }
        public User User { get; set; }
        public Read(Thread thread, User user)
        {
            Thread = thread;
            User = user;
        }   
    }
}
