using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nano_paralell
{
    class Vertex
    {
        int number;
        bool start;
        bool end;
        string description;
        int workTime;
        int endTime;
        List<int> inboxVertex;
        List<int> leavingVertex;
        int numThread;

        public Vertex()
        {
            EndTime = 0;
            inboxVertex = new List<int>();
            leavingVertex = new List<int>();
        }

        public int Number
        {
            set { number = value; }
            get { return number; }
        }

        public bool IsStart
        {
            set { start = value; }
            get { return start; }
        }

        public bool IsEnd
        {
            set { end = value; }
            get { return end; }
        }

        public string Description
        {
            set { description = value; }
            get { return description; }
        }

        public int WorkTime
        {
            set { workTime = value; }
            get { return workTime; }
        }

        public int EndTime
        {
            set { endTime = value; }
            get { return endTime; }
        }

        public int NumThread
        {
            set { numThread = value; }
            get { return numThread; }
        }

        public List<int> InboxVertex
        {
            set { inboxVertex = value; }
            get { return inboxVertex; }
        }

        public List<int> LeavingVertex
        {
            set { leavingVertex = value; }
            get { return leavingVertex; }
        }

    }
}
