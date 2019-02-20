using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Predilatation
{
    public partial class ChooseFunForm : Form
    {
        private MainForm estimate = null;
        private DilatationForm dilatation = null;
        public ChooseFunForm()
        {
            estimate = new MainForm(this);
            dilatation = new DilatationForm(this);
            InitializeComponent();
        }

        private void BtnEstimate_Click(object sender, EventArgs e)
        {
            estimate.Show();
            this.Hide();
        }

        private void BtnDilatation_Click(object sender, EventArgs e)
        {
            dilatation.Show();
            this.Hide();
        }
    }
}
