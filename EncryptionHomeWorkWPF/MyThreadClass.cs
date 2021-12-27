using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EncryptionHomeWorkWPF
{
    public class MyThreadClass
    {
        public delegate void MyEventHandler(long Max, int Progress, char ch);
        public event MyEventHandler MyEvent;

        public string OldValue { get; set; }
        public string Path1 { get; set; }
        public string PassWord { get; set; }
        public MyThreadClass(string path, string pass)
        {

            Path1 = path;
            PassWord = pass;
        }
        public void Do(object state)
        {
            CancellationToken MyToken = (CancellationToken)state;
            string text = File.ReadAllText(Path1);
            var Counter = text.Length;
            using (var Fs = new FileStream(Path1, FileMode.Open))
            {
                using (StreamWriter sw = new StreamWriter(Fs, Encoding.UTF8))
                {
                    char ch;
                    int val;
                    int ProgressBarValue = 0;
                    OldValue = text;
                    Fs.SetLength(0);
                    for (int i = 0; i < Counter; i++)
                    {
                        if (MyToken.IsCancellationRequested)
                        {
                            ProgressBarValue = 0;

                            if (MyEvent != null)
                                MyEvent(100, 0, ' ');
                            sw.Close();
                            File.WriteAllText(Path1, OldValue);
                            break;
                        }
                        else
                        {
                            ch = text[i];
                            sw.Write((char)(ch ^ int.Parse(PassWord)));
                            Thread.Sleep(30);
                            ProgressBarValue++;
                            if (MyEvent != null)
                                MyEvent(Counter, ProgressBarValue, (char)(ch ^ int.Parse(PassWord)));
                        }
                    }
                }
            }

        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~MyThreadClass()
        {
            Dispose();
        }

    }
}
