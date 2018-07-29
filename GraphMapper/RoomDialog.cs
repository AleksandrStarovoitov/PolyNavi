using System;
using System.Windows.Forms;

namespace GraphMapper
{
    public partial class RoomDialog : Form
	{
		public string RoomName { get; private set; } = "*Unknown*";

		public RoomDialog()
		{
			InitializeComponent();
			this.ActiveControl = textBox1;
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
