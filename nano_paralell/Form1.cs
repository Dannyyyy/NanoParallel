using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nano_paralell
{
    public partial class Form1 : Form
    {
        //
        Dictionary<int, List<int>> populations = new Dictionary<int, List<int>>();
        int key = 0;
        Stack<int> usesOrganizm = new Stack<int>();
        //
        int Q = 5;
        int Z = 40;
        double Pm = 0.9; //вероятность мутации
        double Pvm =  0.95; //вероятность того, что останется более приспособленный
        double Pv = 0.05; //вероятность того, что погибнет менее приспособленный
        //
        private object threadLock = new object();

        List<List<Vertex>> modulsInThread = new List<List<Vertex>>();

        const int countThread = 4;
        int startVertex = 0;
        int endVertex = 0;
        Random rand = new Random();

        Dictionary<int,Vertex> moduls = new Dictionary<int, Vertex>();

        List<int> regulateList = new List<int>();

        //
        List<List<Vertex>> tasks = new List<List<Vertex>>();
        const int countModuls = 80;
        int[] flags = new int[countModuls];
        int[] all = new int[countThread];
        int[] currentTime = new int[countModuls];
        //

        public Form1()
        { 
            InitializeComponent();
            for (int i = 0; i < countThread; i++)
            {
                tasks.Add(new List<Vertex>());
                modulsInThread.Add(new List<Vertex>());
            }
            for (int i = 0; i < countModuls; i++)
            {
                flags[i] = 0;
                currentTime[i] = 0;
            }
            for (int i = 0; i < countThread; i++)
            {
                all[i] = 0;
            }
        }

        // функция, выполянющаяся в потоке
        public void threadFunc(object number)
        {
            int num = Convert.ToInt32(number);
            List<Vertex> threadTasks = new List<Vertex>();
            threadTasks.AddRange(tasks[num]);
            foreach (var modul in tasks[num])
            {
                bool flag = true;
                if (modul.InboxVertex.Count == 0)
                {
                    flag = false;
                }
                while (flag)
                {
                    int ready = 1;
                    foreach (var inboxVertex in modul.InboxVertex)
                    {
                        ready *= flags[inboxVertex];
                    }
                    if (ready == 1)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                        lock (threadLock)
                        {
                            switch (num)
                            {
                                case 0:  textBox3.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                                    break;
                                case 1:
                                    textBox4.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                                    break;
                                case 2:
                                    textBox5.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                                    break;
                                case 3:
                                    textBox6.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                                    break;
                                default:
                                    textBox11.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                                    break;
                            }
                            //textBox2.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is wait" + Environment.NewLine);
                        }
                    }
                }
                lock (threadLock)
                {
                    flags[modul.Number] = 1;
                    all[num] += modul.WorkTime;

                    switch (num)
                    {
                        case 0:
                            textBox3.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is end" + Environment.NewLine);
                            break;
                        case 1:
                            textBox4.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is end" + Environment.NewLine);
                            break;
                        case 2:
                            textBox5.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is end" + Environment.NewLine);
                            break;
                        case 3:
                            textBox6.AppendText("Modul " + modul.Number.ToString() + "(thread " + num.ToString() + ") is end" + Environment.NewLine);
                            break;
                    }
                }
            }
        }

        public void checkCompleteTime()
        {
            List<List<int>> steps = new List<List<int>>();
            List<List<int>> stepsTime = new List<List<int>>();
            for (int i = 0; i < countModuls; i++)
            {
                steps.Add(new List<int>());
                stepsTime.Add(new List<int>());
            }
            Stack<int> existVertex = new Stack<int>();
            for (int i = 0; i < countModuls; i++)
            {
                for (int j = 0; j < countThread; j++)
                {
                    foreach (var elem in modulsInThread[j])
                    {

                        if(!existVertex.Contains(elem.Number))
                        {

                            if (elem.InboxVertex.Count == 0)
                            {
                                steps[i].Add(elem.Number);
                                stepsTime[i].Add(elem.WorkTime);
                                break;
                            }
                            int ready = 1;
                            foreach (var inboxVertex in elem.InboxVertex)
                            {
                                if (!existVertex.Contains(inboxVertex))
                                {
                                    ready *= 0;
                                }
                                else
                                {
                                    ready *= 1;
                                }
                            }
                            if (ready == 1)
                            {
                                steps[i].Add(elem.Number);
                                stepsTime[i].Add(elem.WorkTime);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                foreach (var num in steps[i])
                {
                    existVertex.Push(num);
                }
            }
            int time = 0;
            foreach (var list in stepsTime)
            {
                if (list.Count != 0)
                {
                    time += list.Max();
                }
            }
            int allTime = 0;
            foreach (var modul in moduls)
            {
                allTime += modul.Value.WorkTime;
            }
            MessageBox.Show("Последовательное время выполнения: " + allTime.ToString());
            MessageBox.Show("Параллельное время выполнения: " + time.ToString());
            textBox1.AppendText("Параллельый запуск:" + Environment.NewLine);
            foreach (var list in steps)
            {
                if (list.Count != 0)
                {
                    foreach (var elem in list)
                    {
                        textBox1.AppendText(elem.ToString() + " ");
                    }
                    textBox1.AppendText(Environment.NewLine);
                }
            }
        }

        // подсчет времени выполнения всех модулей
        public void checkAllTime()
        {
            List<List<int>> steps = new List<List<int>>();
            List<List<int>> stepsTime = new List<List<int>>();
            for (int i=0;i<countModuls;i++)
            {
                steps.Add(new List<int>());
                stepsTime.Add(new List<int>());
            }
            Stack<int> existVertex = new Stack<int>();
            for (int i = 0; i < moduls.Count; i++)
            {
                bool[] threadBusy = new bool[countThread];
                for (int j = 0; j < countThread; j++)
                {
                    threadBusy[j] = false;
                }
                foreach (KeyValuePair<int, Vertex> modul in moduls)
                {
                    if (!existVertex.Contains(modul.Value.Number))
                    {
                        if (modul.Value.InboxVertex.Count == 0)
                        {
                            if (!threadBusy[modul.Value.NumThread])
                            {
                                steps[i].Add(modul.Value.Number);
                                stepsTime[i].Add(modul.Value.WorkTime);
                                threadBusy[modul.Value.NumThread] = true;
                            }
                        }
                        else
                        {
                            if (!threadBusy[modul.Value.NumThread])
                            {
                                int ready = 1;
                                foreach (var inboxVertex in modul.Value.InboxVertex)
                                {
                                    if (existVertex.Contains(inboxVertex))
                                    {
                                        ready *= 1;
                                    }
                                    else
                                    {
                                        ready *= 0;
                                    }
                                }
                                if (ready == 1)
                                {
                                    steps[i].Add(modul.Value.Number);
                                    stepsTime[i].Add(modul.Value.WorkTime);
                                    threadBusy[modul.Value.NumThread] = true;
                                }
                            }
                        }
                    }
                }
                foreach (var num in steps[i])
                {
                    existVertex.Push(num);
                }
            }
            int time = 0;
            foreach(var list in stepsTime)
            {
                if (list.Count != 0)
                {
                    time += list.Max();
                }
            }
            MessageBox.Show(time.ToString());
            foreach(var list in steps)
            {
                if (list.Count != 0)
                {
                    foreach (var elem in list)
                    {
                        textBox2.AppendText(elem.ToString() + " ");
                    }
                    textBox2.AppendText(Environment.NewLine);
                }
            }
        }

        // запуск потоков
        public void runTasks()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < countThread; i++)
            {
                Thread thread = new Thread(this.threadFunc);
                thread.Priority = ThreadPriority.Normal;
                threads.Add(thread);
            }

            for (int i = 0; i < countThread; i++)
            {
                threads[i].Start(i);
            } 
        }

        // формирование гена по потокам
        public void recordVertex()
        {
            int count = 0;
            if (regulateList.Count % countThread == 0)
            {
                count = regulateList.Count / countThread;
            }
            else
            {
                count = regulateList.Count / countThread+1;
            }
            for (int i = 0; i < countThread; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (i + j*countThread < regulateList.Count)
                    {
                        tasks[i].Add(moduls[regulateList[i + j * countThread]]);
                        moduls[regulateList[i + j * countThread]].NumThread = i;
                        modulsInThread[i].Add(moduls[regulateList[i + j * countThread]]);
                        switch (i)
                        {
                            case 0:
                                textBox7.AppendText("Modul " + moduls[regulateList[i + j * countThread]].Number.ToString() + Environment.NewLine);
                                break;
                            case 1:
                                textBox8.AppendText("Modul " + moduls[regulateList[i + j * countThread]].Number.ToString() + Environment.NewLine);
                                break;
                            case 2:
                                textBox9.AppendText("Modul " + moduls[regulateList[i + j * countThread]].Number.ToString() + Environment.NewLine);
                                break;
                            case 3:
                                textBox10.AppendText("Modul " + moduls[regulateList[i + j * countThread]].Number.ToString() + Environment.NewLine);
                                break;
                            default:
                                textBox12.AppendText("Modul " + moduls[regulateList[i + j * countThread]].Number.ToString() + Environment.NewLine);
                                break;
                        }
                    }
                }               
                //textBox1.AppendText("-----Thread " + (i+1).ToString() + Environment.NewLine);
            }
        }

        // Создание популяции по regulateList
        public void createPopulation()
        {
            populations.Add(key, regulateList);
            key++;
            for (int i = 1; i < Z; i++)
            {
                List<int> newRegulateList = new List<int>(regulateList);
                foreach (var modul in moduls)
                {
                    if (modul.Value.LeavingVertex.Count > 1 && !modul.Value.LeavingVertex.Contains(endVertex))
                    {
                        
                        if (rand.Next() % 2 == 0)
                        {
                            int firstNum;
                            int secondNum;
                            do
                            {
                                firstNum = modul.Value.LeavingVertex[rand.Next(0, modul.Value.LeavingVertex.Count)];
                                secondNum = modul.Value.LeavingVertex[rand.Next(0, modul.Value.LeavingVertex.Count)];
                            } while (firstNum == secondNum);
                            int firstSearch = 0, secondSearch = 0;
                            for (int k = 0; k < newRegulateList.Count; k++)
                            {
                                if (newRegulateList[k] == firstNum)
                                {
                                    firstSearch = k;
                                }
                                if (newRegulateList[k] == secondNum)
                                {
                                    secondSearch = k;
                                }
                            }
                            newRegulateList[firstSearch] = secondNum;
                            newRegulateList[secondSearch] = firstNum;
                        }
                    }
                }
                populations.Add(key, newRegulateList);
                key++;
            }
        }

        //Выбор организма
        public int choiseOrganizm(int num)
        {
            int i = 0;
            foreach (KeyValuePair<int, List<int>> currentPopulation in populations)
            {
                if (i == num)
                {
                    if (usesOrganizm.Contains(currentPopulation.Key))
                    {
                        return -1;
                    }
                    else
                    {
                        usesOrganizm.Push(currentPopulation.Key);
                        return currentPopulation.Key;
                    }
                }
                i++;
            }
            return -1;
        }

        public void crossing(int first, int second)
        {
            List<int> firstParent = new List<int>(populations[first]);
            List<int> secondParent = new List<int>(populations[second]);
            List<int> firstChildren = new List<int>(createMutation(populations[first]));
            List<int> secondChildren = new List<int>(createMutation(populations[second]));
            populations.Add(key, firstChildren);
            key++;
            populations.Add(key, secondChildren);
            key++;
        }

        // Воспроизводство
        public void reproduction()
        {
            usesOrganizm.Clear();
            
            for (int i = 0; i < Q; i++)
            {
                int firstNum = 0;
                int flag = -1;
                do
                {
                    firstNum = Math.Abs(rand.Next() % Z);
                    flag = choiseOrganizm(firstNum);
                } while (flag == -1);
                firstNum = flag;
                int secondNum = 0;
                flag = -1;
                do
                {
                    secondNum = Math.Abs(rand.Next() % Z);
                    flag = choiseOrganizm(secondNum);
                } while (flag == -1);
                secondNum = flag;
                crossing(firstNum, secondNum); 
            }
        }

        public List<int> createMutation(List<int> main)
        {
            List<int> newRegulateList = new List<int>(main);
            foreach (var modul in moduls)
            {
                if (modul.Value.LeavingVertex.Count > 1 && !modul.Value.LeavingVertex.Contains(endVertex))
                {
                    if (rand.Next() % 2 == 0)
                    {
                        int firstNum;
                        int secondNum;
                        do
                        {
                            firstNum = modul.Value.LeavingVertex[rand.Next(0, modul.Value.LeavingVertex.Count)];
                            secondNum = modul.Value.LeavingVertex[rand.Next(0, modul.Value.LeavingVertex.Count)];
                        } while (firstNum == secondNum);
                        int firstSearch = 0, secondSearch = 0;
                        for (int k = 0; k < newRegulateList.Count; k++)
                        {
                            if (newRegulateList[k] == firstNum)
                            {
                                firstSearch = k;
                            }
                            if (newRegulateList[k] == secondNum)
                            {
                                secondSearch = k;
                            }
                        }
                        newRegulateList[firstSearch] = secondNum;
                        newRegulateList[secondSearch] = firstNum;
                    }
                }
            }
            return newRegulateList;
        }

        public int organizmResult(List<int> organizm)
        {
            Dictionary<int, Vertex> currentModuls = new Dictionary<int, Vertex>(moduls);
            //
            int count = 0;
            if (organizm.Count % countThread == 0)
            {
                count = organizm.Count / countThread;
            }
            else
            {
                count = organizm.Count / countThread + 1;
            }
            for (int i = 0; i < countThread; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (i + j * countThread < organizm.Count)
                    {
                        currentModuls[organizm[i + j * countThread]].NumThread = i;
                    }
                }
            }
            //
            List<List<int>> steps = new List<List<int>>();
            List<List<int>> stepsTime = new List<List<int>>();
            for (int i = 0; i < countModuls; i++)
            {
                steps.Add(new List<int>());
                stepsTime.Add(new List<int>());
            }
            Stack<int> existVertex = new Stack<int>();
            for (int i = 0; i < currentModuls.Count; i++)
            {
                bool[] threadBusy = new bool[countThread];
                for (int j = 0; j < countThread; j++)
                {
                    threadBusy[j] = false;
                }
                foreach (KeyValuePair<int, Vertex> modul in currentModuls)
                {
                    if (!existVertex.Contains(modul.Value.Number))
                    {
                        if (modul.Value.InboxVertex.Count == 0)
                        {
                            if (!threadBusy[modul.Value.NumThread])
                            {
                                steps[i].Add(modul.Value.Number);
                                stepsTime[i].Add(modul.Value.WorkTime);
                                threadBusy[modul.Value.NumThread] = true;
                            }
                        }
                        else
                        {
                            if (!threadBusy[modul.Value.NumThread])
                            {
                                int ready = 1;
                                foreach (var inboxVertex in modul.Value.InboxVertex)
                                {
                                    if (existVertex.Contains(inboxVertex))
                                    {
                                        ready *= 1;
                                    }
                                    else
                                    {
                                        ready *= 0;
                                    }
                                }
                                if (ready == 1)
                                {
                                    steps[i].Add(modul.Value.Number);
                                    stepsTime[i].Add(modul.Value.WorkTime);
                                    threadBusy[modul.Value.NumThread] = true;
                                }
                            }
                        }
                    }
                }
                foreach (var num in steps[i])
                {
                    existVertex.Push(num);
                }
            }
            int time = 0;
            foreach (var list in stepsTime)
            {
                if (list.Count != 0)
                {
                    time += list.Max();
                }
            }
            return time;
        }

        //Мутация
        public void mutation()
        {
            Stack<int> death = new Stack<int>();
            Stack<List<int>> live = new Stack<List<int>>();

            int i = 0;
            foreach (KeyValuePair<int, List<int>> currentPopulation in populations)
            {
                if (i == Z)
                    break;
                //i не увеличивается
                if ((Convert.ToDouble(rand.Next(100)) / 100) <= Pm)
                {
                    List<int> mutant = new List<int>(createMutation(currentPopulation.Value));
                    int parentResult = organizmResult(currentPopulation.Value);
                    double mutantResult = organizmResult(mutant);

                    if (parentResult >= mutantResult)
                    {
                        if ((Convert.ToDouble(rand.Next(100)) / 100) > Pvm)
                        {
                            death.Push(currentPopulation.Key);
                            live.Push(mutant);
                        }
                    }
                    else
                    {
                        if ((Convert.ToDouble(rand.Next(100)) / 100) <= Pvm)
                        {
                            death.Push(currentPopulation.Key);
                            live.Push(mutant);
                        }
                    }
                }
            }
            foreach (int j in death)
            {
                populations.Remove(j);
            }
            foreach (var mutant in live)
            {
                populations.Add(key, mutant);
                key++;
            }
            live.Clear();
            death.Clear();
        }

        //Борьба
        public void fight()
        {
            Stack<int> death = new Stack<int>();

            usesOrganizm.Clear();

            for (int i = 0; i < 2 * Q; i++)
            {
                int firstNum = 0;
                int flag = -1;
                do
                {
                    firstNum = Math.Abs(rand.Next() % (Z + 2 * Q));
                    flag = choiseOrganizm(firstNum);
                } while (flag == -1);
                firstNum = flag;
                int secondNum = 0;
                flag = -1;
                do
                {
                    secondNum = Math.Abs(rand.Next() % (Z + 2 * Q));
                    flag = choiseOrganizm(secondNum);
                } while (flag == -1);
                secondNum = flag;
                double resultFirstOrganizm = organizmResult(populations[firstNum]);
                double resultSecondOrganizm = organizmResult(populations[secondNum]);

                if (resultFirstOrganizm >= resultSecondOrganizm)
                {
                    if ((Convert.ToDouble(rand.Next(100)) / 100) <= Pv)
                    {
                        death.Push(secondNum);
                    }
                    else
                    {
                        death.Push(firstNum);
                    }
                }
                else
                {
                    if ((Convert.ToDouble(rand.Next(100)) / 100) <= Pvm)
                    {
                        death.Push(firstNum);
                    }
                    else
                    {
                        death.Push(secondNum);
                    }
                }
            }
            foreach (int j in death)
            {
                populations.Remove(j);
            }
            death.Clear();
        }

        public void writePopulation()
        {
            foreach (KeyValuePair<int, List<int>> currentPopulation in populations)
            {
                foreach (var elem in currentPopulation.Value)
                {
                    textBox2.AppendText(elem.ToString() + " ");
                }
                textBox2.AppendText(Environment.NewLine);
            }
        }

        // запись в список в зависимости от времени окончания(min -> max)
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

        // Вывод списка по возрастаниб времени убывания
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

        // Просчет времени окончания
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

        // Вывод модулей
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

        // Поиск входящих модулей для каждого модуля
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
                startVertex = Convert.ToInt32(positionStartEnd[0]);
                endVertex = Convert.ToInt32(positionStartEnd[1]);
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
                // поиск зависимостей для модуля
                findInboxVertex();
                // подсчет времени, через которое модуль отработает
                countTimeVertex();
                // вывод результатов
                writeModuls();
                // расположение модулей в зависимости от времени окончания
                regulateEndTime();
            }
            finally
            {
                strRead.Close();
                fin.Close();
                button2.Enabled = false;
                button3.Enabled = true;
            }
        }

        // генетический алгоритм
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            createPopulation();
            for (int h = 0; h < 400; h++)
            {
                reproduction();
                mutation();
                fight();
            }
            MessageBox.Show("Решение найдено.");
            recordVertex();  
            button4.Enabled = true;
        }

        // запуск модулей
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            runTasks();
            checkCompleteTime();
        }
    }
}
