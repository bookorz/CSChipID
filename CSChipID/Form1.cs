using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using FTChipID;

/* 
 * 
 * 
 PLEASE NOTE
  You must add a reference to the FTChipIDNet.dll in order to use this sample
  To do this:
    1. Click on Solution explorer tab.
    2. Right click the References tree.
    3. Choose Add Reference option.
    4. Browse to the FTChipIDNet.dll (as a personal preference I place this in my bin directory)
    5. Click OK
*
*
*/
namespace CSChipID
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnFindDevices;
		private System.Windows.Forms.ColumnHeader colNumber;
		private System.Windows.Forms.ColumnHeader colSerial;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colLocationID;
		private System.Windows.Forms.ColumnHeader colChipID;
		private System.Windows.Forms.ListView lstDevices;
        private RichTextBox rtbEEPROM;
        private Button tbReadEEPRom;
        private Button tbWriteEEPRom;
        private EEPROM rom = new EEPROM();
        private TextBox tbSN;
        private Label label1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lstDevices = new System.Windows.Forms.ListView();
            this.colNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSerial = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLocationID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChipID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnFindDevices = new System.Windows.Forms.Button();
            this.rtbEEPROM = new System.Windows.Forms.RichTextBox();
            this.tbReadEEPRom = new System.Windows.Forms.Button();
            this.tbWriteEEPRom = new System.Windows.Forms.Button();
            this.tbSN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstDevices
            // 
            this.lstDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNumber,
            this.colSerial,
            this.colDescription,
            this.colLocationID,
            this.colChipID});
            this.lstDevices.FullRowSelect = true;
            this.lstDevices.Location = new System.Drawing.Point(16, 9);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.Size = new System.Drawing.Size(384, 240);
            this.lstDevices.TabIndex = 0;
            this.lstDevices.UseCompatibleStateImageBehavior = false;
            this.lstDevices.View = System.Windows.Forms.View.Details;
            this.lstDevices.SelectedIndexChanged += new System.EventHandler(this.lstDevices_SelectedIndexChanged);
            // 
            // colNumber
            // 
            this.colNumber.Text = "Number";
            this.colNumber.Width = 30;
            // 
            // colSerial
            // 
            this.colSerial.Text = "Serial";
            this.colSerial.Width = 80;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 100;
            // 
            // colLocationID
            // 
            this.colLocationID.Text = "LocationID";
            this.colLocationID.Width = 70;
            // 
            // colChipID
            // 
            this.colChipID.Text = "ChipID";
            this.colChipID.Width = 100;
            // 
            // btnFindDevices
            // 
            this.btnFindDevices.Location = new System.Drawing.Point(16, 258);
            this.btnFindDevices.Name = "btnFindDevices";
            this.btnFindDevices.Size = new System.Drawing.Size(104, 27);
            this.btnFindDevices.TabIndex = 1;
            this.btnFindDevices.Text = "Find 232R Devices";
            this.btnFindDevices.Click += new System.EventHandler(this.btnFindDevices_Click);
            // 
            // rtbEEPROM
            // 
            this.rtbEEPROM.Location = new System.Drawing.Point(421, 9);
            this.rtbEEPROM.Name = "rtbEEPROM";
            this.rtbEEPROM.Size = new System.Drawing.Size(339, 495);
            this.rtbEEPROM.TabIndex = 2;
            this.rtbEEPROM.Text = "";
            // 
            // tbReadEEPRom
            // 
            this.tbReadEEPRom.Location = new System.Drawing.Point(316, 258);
            this.tbReadEEPRom.Name = "tbReadEEPRom";
            this.tbReadEEPRom.Size = new System.Drawing.Size(84, 27);
            this.tbReadEEPRom.TabIndex = 3;
            this.tbReadEEPRom.Text = "Read EEPRom";
            this.tbReadEEPRom.UseVisualStyleBackColor = true;
            this.tbReadEEPRom.Visible = false;
            this.tbReadEEPRom.Click += new System.EventHandler(this.tbReadEEPRom_Click);
            // 
            // tbWriteEEPRom
            // 
            this.tbWriteEEPRom.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWriteEEPRom.Location = new System.Drawing.Point(281, 447);
            this.tbWriteEEPRom.Name = "tbWriteEEPRom";
            this.tbWriteEEPRom.Size = new System.Drawing.Size(119, 27);
            this.tbWriteEEPRom.TabIndex = 3;
            this.tbWriteEEPRom.Text = "Write Rom";
            this.tbWriteEEPRom.UseVisualStyleBackColor = true;
            this.tbWriteEEPRom.Click += new System.EventHandler(this.tbWriteEEPRom_Click);
            // 
            // tbSN
            // 
            this.tbSN.Font = new System.Drawing.Font("·s²Ó©úÅé", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbSN.Location = new System.Drawing.Point(120, 375);
            this.tbSN.Name = "tbSN";
            this.tbSN.Size = new System.Drawing.Size(280, 30);
            this.tbSN.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 376);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 22);
            this.label1.TabIndex = 5;
            this.label1.Text = "Department";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(782, 576);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSN);
            this.Controls.Add(this.tbWriteEEPRom);
            this.Controls.Add(this.tbReadEEPRom);
            this.Controls.Add(this.rtbEEPROM);
            this.Controls.Add(this.btnFindDevices);
            this.Controls.Add(this.lstDevices);
            this.Name = "MainForm";
            this.Text = "ChipID";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
        }

		private void btnFindDevices_Click(object sender, System.EventArgs e)
		{
			int numDevices = 0, LocID = 0, ChipID = 0;
			//string SerialBuffer = "01234567890123456789012345678901234567890123456789";
			//string Description = "01234567890123456789012345678901234567890123456789";
            string SerialBuffer = "".PadRight(50,' ');
            string Description = "".PadRight(50, ' ');
            ListViewItem item;

			lstDevices.Items.Clear();
			
			try 
			{
				FTChipID.ChipID.GetNumDevices(ref numDevices);
				if (numDevices > 0)
				{
					for (int i = 0; i < numDevices; i++)
					{
						item = new ListViewItem();
						item.Text = i.ToString();
						FTChipID.ChipID.GetDeviceSerialNumber(i, ref SerialBuffer, 50);
						item.SubItems.Add(SerialBuffer);
						FTChipID.ChipID.GetDeviceDescription(i, ref Description, 50);
						item.SubItems.Add(Description);
						FTChipID.ChipID.GetDeviceLocationID(i, ref LocID);
						item.SubItems.Add("0x" + LocID.ToString("X"));
						FTChipID.ChipID.GetDeviceChipID(i, ref ChipID);
						item.SubItems.Add("0x" + ChipID.ToString("X"));
						lstDevices.Items.Add(item);
					}
                    rtbEEPROM.Text = rom.getContent(true);
                }
			}

			catch(FTChipID.ChipIDException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

        private void tbReadEEPRom_Click(object sender, EventArgs e)
        {
            rtbEEPROM.Text = rom.getContent(true);
        }

        private void tbWriteEEPRom_Click(object sender, EventArgs e)
        {

            foreach (ListViewItem tmpLstView in lstDevices.Items)
            {
                //if (tmpLstView.Selected == true)
                //{
                    string sn = tmpLstView.SubItems[1].Text;
                    string Cp = tmpLstView.SubItems[4].Text;
                    rom.encode(sn, Cp, tbSN.Text);
                    rtbEEPROM.Text = rom.getContent(false);
                //}
            }

            btnFindDevices_Click(null,null);
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
