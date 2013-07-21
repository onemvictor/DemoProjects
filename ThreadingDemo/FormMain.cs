using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ThreadingDemo
{
    public partial class FormMain : Form
    {
        private static FormMain _singleForm;

        public static FormMain SingleTon
        {
            get
            {
                if (_singleForm == null)
                {
                    _singleForm = new FormMain();
                }
                return _singleForm;
            }
        }

        private List<string> _stringBag=new List<string>();

        private Object _thisLock=new Object();

        private Thread _thread1;

        private Thread _thread2;

        private volatile bool _shouldStop1;
        private volatile bool _shouldStop2;

        private FormMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            DefineThreadJobs();
            base.OnLoad(e);
        }

        private void DefineThreadJobs()
        {
            _thread1 = new Thread(WriteWithThread1);
            _thread2 = new Thread(WriteWithThread2);
        }

        private void AddToBagWithLock(string str)
        {
            lock (_thisLock)
            {
                _stringBag.Add(str);
            }
        }

        private void WriteWithThread1()
        {
            var random = new Random();
            var charByte=new byte[1];
            var autoreset = new AutoResetEvent(false);

            while (!_shouldStop1)
            {
                random.NextBytes(charByte);
                string str = "Thread1 Writes:" + (char)charByte[0];
                AddToBagWithLock(str);
                autoreset.Reset();
                this.Invoke((MethodInvoker)(() =>
                    {
                        dgv1.Rows.Add();
                        dgv1.Rows[dgv1.Rows.Count - 1].Cells[0].Value = str;
                        autoreset.Set();
                    }));
                autoreset.WaitOne();
                Thread.Sleep(1000);
            }
        }

        private void WriteWithThread2()
        {
            var random = new Random();
            var charByte = new byte[1];
            var autoreset = new AutoResetEvent(false);

            while (!_shouldStop2)
            {
                random.NextBytes(charByte);
                string str = "Thread2 Writes:" + (char)charByte[0];
                AddToBagWithLock(str);
                autoreset.Reset();
                this.Invoke((MethodInvoker)(() =>
                {
                    dgv2.Rows.Add();
                    dgv2.Rows[dgv2.Rows.Count - 1].Cells[0].Value = str;
                    autoreset.Set();
                }));
                autoreset.WaitOne();
                Thread.Sleep(1000);
            }
        }

        private void btnTh1_Click(object sender, EventArgs e)
        {
            if (btnTh1.Text == "Start Thread 1")
            {
                _thread1.Start();
                btnTh1.Text="Stop Thread 1";
                return;
            }
            if (btnTh1.Text == "Stop Thread 1")
            {
                _shouldStop1 = true;
                while (_thread1.IsAlive)
                {
                    Thread.Sleep(100);
                }
                btnTh1.Text = "Start Thread 1";
                return;
            }

        }

        private void btnTh2_Click(object sender, EventArgs e)
        {
            if (btnTh2.Text == "Start Thread 2")
            {
                _thread2.Start();
                btnTh2.Text = "Stop Thread 2";
                return;
            }
            if (btnTh2.Text == "Stop Thread 2")
            {
                _shouldStop2 = true;
                while (_thread2.IsAlive)
                {
                    Thread.Sleep(100);
                }
                btnTh2.Text = "Start Thread 2";
                return;
            }

        }

        internal List<string> GetBagContent()
        {
            lock (_thisLock)
            {
                return _stringBag.ToList();
            }
        }
        private void btnRead_Click(object sender, EventArgs e)
        {
            var content = GetBagContent();
            new FormDisplay().ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _shouldStop1 = true;
            _shouldStop2 = true;
            while (_thread1.IsAlive || _thread2.IsAlive)
            {
                Thread.Sleep(100);
            }

            Application.Exit();
        }
        
    }
}
