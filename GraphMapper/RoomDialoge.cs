using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphMapper
{
	public partial class RoomDialoge : Form
	{
		public int RoomNumber { get; private set; }

		public RoomDialoge()
		{
			InitializeComponent();
			roomNumberNumericUpDown.KeyDown += (sender, e) =>
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
			RoomNumber = (int)roomNumberNumericUpDown.Value;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
