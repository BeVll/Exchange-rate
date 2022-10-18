using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatWithBot
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public int Sender_Id { get; set; }
    }
}
