using System;
using System.Windows.Forms;

namespace GraphMapper
{
    public partial class FloorNumberDialog : Form
    {
        public int FloorNumber { get; set; } = 1;
        public int FloorPartNumber { get; set; } = 1;

        public FloorNumberDialog()
        {
            InitializeComponent();
            floorNumberNumericUpDown.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    buttonOk.PerformClick();
                }
            };
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    buttonOk.PerformClick();
                }
            };
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            FloorNumber = (int)floorNumberNumericUpDown.Value;
            FloorPartNumber = (int)floorPartNumericUpDown.Value;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
