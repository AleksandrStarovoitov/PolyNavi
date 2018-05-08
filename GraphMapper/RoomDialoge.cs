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
		public string RoomName { get; private set; } = "*Unknown*";

		public RoomDialoge()
		{
			InitializeComponent();
			textBox1.KeyDown += (sender, e) =>
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
			RoomName = textBox1.Text;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
