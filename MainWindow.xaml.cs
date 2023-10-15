using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public class Bad
        {
            public string Name;
            public long Syze;
            public int Quantity;
            public Bad(string name, long syze, int quantity)
            {
                this.Name = name;
                this.Syze = syze;
                this.Quantity = quantity; 
            }

        }

        List<string> W = new List<string>();       //коллекция плохих слов
        List<string> F = new List<string>();        //коллекция слов из файла
        List<Bad> B = new List<Bad>();        //коллекция  файлов

        string[] strArray;                                      //массив  подстрок
        string strk;                                                //строка в которую читается файл
        string path1;  // путь к плохим словам
                       //        string path;// путь к папке с файлами
                       //        string newPath;// путь к новой папке
        bool status = true; //кнопка пауза
        public int x;
        public int y;
        public int q;
        public async void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text files(*.txt)|*.txt";
            if (openFile.ShowDialog() != true) return;
            path1 = openFile.FileName;

            ReadFile(path1);
            await Task.Delay(1000);
            Split();
            PackedW();
        }
        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {

            status = false;
            await Task.Delay(1000);
            CopyF();
            CreatF();

        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (status)
            {
                status = false;
                btnPause.Content = @"Пауза";
            }
            else
            {
                status = true;
                btnPause.Content = @"Пуск";
            }

        }
        private async void btnReport_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            txt1.Text = "           КОЛИЧЕСТВО НАЙДЕННЫХ ФАЙЛОВ  - " +y+ "\r\n";
            txt1.Text += "           ПУТЬ К СКОПИРОВАННЫМ ФАЙЛАМ:   " + "\r\n" + Directory.GetCurrentDirectory() +
                                            @"\Плохие файлы" + "\r\n";
            txt1.Text += "           ПУТЬ К ИЗМЕНЕННЫМ ФАЙЛАМ:   " + "\r\n" + Directory.GetCurrentDirectory() +
                                           @"\Измененные файлы" + "\r\n";
            txt1.Text += "           СПИСОК СКОПИРОВАННЫХ ФАЙЛОВ:   " + "\r\n";
            vs = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Плохие файлы" ,"*.txt");
            Print();
            txt1.Text += "           СПИСОК ИЗМЕНЕННЫХ ФАЙЛОВ:   " + "\r\n";
            foreach (var item in B)
            {
                txt1.Text += item.Name + "   " + item.Syze.ToString() + "  байт,  " + "  количество плохих слов  - " + item.Quantity.ToString()  + "\r\n"; ; 

            }
           
        }

      

        public void ReadFile(string path) // читается файл в strk
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs);
            strk = reader.ReadToEnd();
            fs.Close();
        }
        public void Split() // strk разбивается на массив подстрок strArray, удаляются знаки и пробелы
        {
            strArray = strk.Split(new char[] { '\n', '\r', ',', ' ', '!', '?', '<', '>', '.', '"' },
                                   StringSplitOptions.RemoveEmptyEntries);
        }
        void PackedF() //формируем лист слов из файла
        {
            F.Clear();
            for (int i = 0; i < strArray.Length; i++)
            {
                string v = strArray[i].ToString();
                F.Add(v);
            }
        }
        public void PackedW()//формируем лист плохих слов 
        {
            for (int i = 0; i < strArray.Length; i++)
            {

                string v = strArray[i].ToString();
                W.Add(v);
            }
            txt1.Text = "Плохие слова:  "  +"\r\n";
            foreach (var item in W)
            {
                txt1.Text += item + ",   ";
            }
        }
        public string[] vs = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");// массив названий файлов    

        public void Print()         //печать плохих слов
        {
            foreach (var item in vs)
            {
                FileInfo ff = new FileInfo(item);
                
                txt1.Text += ff.Name + " ,  ";
                txt1.Text += ff.Length.ToString() + "  байт\r\n";
              
            }
        }
        public void CopyF() //читаем, проверяем и  копируем подходящие файлы в новую папку
        {
            //                          await Task.Delay(1000);
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Плохие файлы"))
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Плохие файлы", true);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Плохие файлы");
            for (int i = 0; i < vs.Length; i++)
            {
                ReadFile(vs[i]);
                Split();
                PackedF();
                FileInfo fileInf = new FileInfo(vs[i]);
                q = 0;
                for (int j = 0; j < F.Count; j++)
                {
                    //                    await Task.Delay(1000);
                    for (int f = 0; f < W.Count; f++)
                    {
                       
                       
                        if (F[j] == W[f])
                        {
                            q++;
                            
                           
                            if (fileInf.Exists)
                            {
                                fileInf.CopyTo(Directory.GetCurrentDirectory() + @"\Плохие файлы" +
                                                 @"\" + fileInf.Name.ToString(), true);
                                //                                break;
                            }
                        }
                      
                    }
                }
                if (q != 0)
                {
                    Bad bad = new Bad(fileInf.Name, fileInf.Length, q);
                    B.Add(bad);
                }
            }
        }
        
        public async void CreatF()
        {
          
            string[] newvs = Directory.GetFiles(Directory.GetCurrentDirectory()
                + @"\Плохие файлы", "*.txt");//массив названий новых файлов
            y = newvs.Length;
           if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Измененные файлы"))
           Directory.Delete(Directory.GetCurrentDirectory() + @"\Измененные файлы", true);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Измененные файлы");

            for (int i = 0; i < newvs.Length; i++)
            {
               
                await Task.Delay(500);
                
                FileInfo fileInf = new FileInfo(newvs[i]);
                if (fileInf.Exists)
                {
                    fileInf.CopyTo(Directory.GetCurrentDirectory() + @"\Измененные файлы" +
                                                    @"\" + fileInf.Name.ToString(), true);

                    ReadFile(Directory.GetCurrentDirectory() + @"\Измененные файлы" +
                                                    @"\" + fileInf.Name.ToString());
                    txtbl1.Text = "Проверка файла:  " + fileInf.Name.ToString();
                    x = strk.Length;
                    await Dispatcher.InvokeAsync(() => progbar_start_async1());
                   
                    for (int z = 0; z < W.Count; z++)                       
                    {                                             
                        strk = strk.Replace(W[z].ToString(), @"*******");
                      
                        while (status)
                        {
                            await Task.Delay(500);
                            if (!status) break;
                        }
                        await Task.Delay(500);
                       
                    }
                 
                    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\Измененные файлы" +
                                                                                                @"\" + fileInf.Name.ToString());
                    writer.Write(strk);
                    writer.Close();
                }
            }
//            await Task.Delay(500);
            MessageBox.Show("Все файлы найдены скопированы и созданы измененные файлы. Для просмотра отчета нажмите кнопку Отчет.");

        }
        async void progbar_start_async1()
        {
            pb1.Value = 0;
            pb1.Maximum = x;
            for (int i = 1; i <= x; i++)
            {
                await Task.Delay(100);
                pb1.Value += x / 10;
                while (status)
                {
                    await Task.Delay(300);
                  
                    if (!status) break;
                }
                await Task.Delay(500);
            }
        }

    }
}