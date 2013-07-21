using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadingDemo
{
    public partial class FormDisplay : Form
    {
        public FormDisplay()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SetGridContent();
        }

        private void SetGridContent()
        {
            dgvDisplay.DataSource = FormMain.SingleTon.GetBagContent().Select(str => new { Content = str }).ToList();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormDisplay_Load(object sender, EventArgs e)
        {
            SetGridContent();
        }
    }
}
