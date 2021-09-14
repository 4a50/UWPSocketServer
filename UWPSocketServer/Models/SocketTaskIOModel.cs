using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPSocketServer.Models
{
    public class SocketTaskIOModel
    {
        public string ID { get; set; }
        public Task Read { get; set; }
        public Task Write { get; set; }
    }
}
