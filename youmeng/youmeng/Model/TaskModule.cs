using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMeng.Model
{
    public class TaskModule
    {
        public bool IsRun { get; set; }
        public string Name { get; set; }
        public string AppKey { get; set; }
        public string UserKey { get; set; }
        public string Secretkey { get; set; }
        public bool Manual_Run { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
