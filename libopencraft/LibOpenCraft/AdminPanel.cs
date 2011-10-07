using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibOpenCraft
{
    public delegate void NewEvent(string type);
    public partial class AdminPanel : Form
    {
        public event NewEvent OnRestart;
        public AdminPanel()
        {
            InitializeComponent();
        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {

        }

        private void Restart_Click(object sender, EventArgs e)
        {
            OnRestart("Restart");
        }
    }
}
