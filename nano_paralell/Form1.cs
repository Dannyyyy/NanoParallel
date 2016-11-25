using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nano_paralell
{
    public partial class Form1 : Form
    {
        const int countThread = 3;

        Dictionary<int,Vertex> moduls = new Dictionary<int, Vertex>();

        List<int> regulateList = new List<int>();

       

        public Form1()
        { 
            InitializeComponent();
        }

        public void recordVertex()
        {
            int count = 0;
            if (regulateList.Count % countThread == 0)
            {
                count = regulateList.Count / countThread;
            }
            else
            {
                count = regulateList.Count / countThread + 1;
            }
            for (int i = 0; i < countThread; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (i + j*countThread < regulateList.Count)
                    {
                        textBox1.AppendText(regulateList[i + j*countThread].ToString() + Environment.NewLine);
                    }
                }
                textBox1.AppendText("-----Thread " + (i+1).ToString() + Environment.NewLine);
            }
        }

        public void regulateEndTime()
        {
            Stack<int> existVertex = new Stack<int>();
            for (int i = 1; i <= moduls.Count; i++)
            {
                int min = int.MaxValue;
                int minNumber = 1;
                foreach (KeyValuePair<int, Vertex> modul in moduls)
                {
                    if (modul.Value.EndTime < min && !existVertex.Contains(modul.Value.Number))
                    {
                        min = modul.Value.EndTime;
                        minNumber = modul.Value.Number;
                    }
                }
                regulateList.Add(minNumber);
                existVertex.Push(minNumber);
            }
        }

        public void writeRegulateList()
        {
            foreach (var elem in regulateList)
            {
                textBox1.AppendText("{" + elem.ToString() + "}");
            }
            textBox1.AppendText(Environment.NewLine);
        }

        // Установка начального и конечного модуля
        public void setStartEndModul(int startNumber, int endNumber)
        {
            moduls[startNumber].IsStart = true;
            moduls[endNumber].IsEnd = true;
        }

        public void countTimeVertex()
        {
            foreach (KeyValuePair<int, Vertex> modul in moduls)
            {
                if (modul.Value.InboxVertex.Count() == 0)
                {
                    modul.Value.EndTime = modul.Value.WorkTime;
                }
                else
                {
                    int max = int.MinValue;
                    foreach (var inboxVertex in modul.Value.InboxVertex)
                    {
                        if (moduls[inboxVertex].EndTime > max)
                        {
                            max = moduls[inboxVertex].EndTime;
                        }
                    }
                    modul.Value.EndTime = max + modul.Value.WorkTime;
                }
            }
        }

        public void writeModuls()
        {
            foreach (KeyValuePair<int, Vertex> modul in moduls)
            {
                textBox1.AppendText(modul.Value.Number.ToString() + " ");
                textBox1.AppendText(modul.Value.Description.ToString() + " [");
                textBox1.AppendText(modul.Value.EndTime.ToString() + "] ");
                textBox1.AppendText(modul.Value.WorkTime.ToString() + " { ");
                foreach (var elem in modul.Value.InboxVertex)
                {
                    textBox1.AppendText(elem.ToString() + " ");
                }
                textBox1.AppendText("}  { ");
                foreach (var elem in modul.Value.LeavingVertex)
                {
                    textBox1.AppendText(elem.ToString() + " ");
                }
                textBox1.AppendText("}" + Environment.NewLine);
            }
        }

        public void findInboxVertex()
        {
            foreach (KeyValuePair<int, Vertex> modul in moduls)
            {
                foreach (KeyValuePair<int, Vertex> inboxModul in moduls)
                {
                    foreach (var inbox in inboxModul.Value.LeavingVertex)
                    {
                        if (inbox == modul.Value.Number)
                        {
                            modul.Value.InboxVertex.Add(inboxModul.Value.Number);
                        }
                    }
                }
            }
        }

        //Данные
        private void button1_Click(object sender, EventArgs e)
        {
            FileStream fin = new FileStream("data.txt", FileMode.Open);
            StreamReader strRead = new StreamReader(fin);
            try
            {
                while (!strRead.EndOfStream)
                {
                    string modulAll = strRead.ReadLine();
                    string[] modulPart = modulAll.Split(' ');
                    Vertex modul = new Vertex();

                    modul.Number = Convert.ToInt32(modulPart[0]);
                    modul.Description = modulPart[1];
                    modul.WorkTime = Convert.ToInt32(modulPart[2]);
                    moduls.Add(modul.Number,modul);
                }
            }
            finally
            {
                strRead.Close();
                fin.Close();
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        // Конфигурация
        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fin = new FileStream("config.txt", FileMode.Open);
            StreamReader strRead = new StreamReader(fin);
            try
            {
                string startEnd = strRead.ReadLine();
                string[] positionStartEnd = startEnd.Split(' ');
                setStartEndModul(Convert.ToInt32(positionStartEnd[0]), Convert.ToInt32(positionStartEnd[1]));
                while (!strRead.EndOfStream)
                {
                    string allLine = strRead.ReadLine();
                    string[] linePart = allLine.Split(' ');
                    int number = Convert.ToInt32(linePart[0]);
                    for (int i = 1; i < linePart.Count(); i++)
                    {
                        int vertexNumber = Convert.ToInt32(linePart[i]);
                        moduls[number].LeavingVertex.Add(vertexNumber);
                    }
                }
                findInboxVertex();
                countTimeVertex();
                writeModuls();
                regulateEndTime();
                writeRegulateList();
                recordVertex();
            }
            finally
            {
                strRead.Close();
                fin.Close();
                button2.Enabled = false;
            }
        }
    }
}
