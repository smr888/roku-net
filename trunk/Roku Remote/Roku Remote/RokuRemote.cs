using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Roku_Remote
{
    public partial class RokuRemote : Form
    {
        Socket skt;
        string rokuhost;
        int rokuport;

        public RokuRemote()
        {            
            InitializeComponent();
            rokuhost = Properties.Settings.Default.rokuipaddress;
            rokuport = Properties.Settings.Default.rokuport;
        }

        private void SendKeyPressString(string stringtosend)
        {
            stringtosend = "POST /keypress/" + stringtosend + " HTTP/1.1\r\n\r\n";
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            skt.Connect(rokuhost, rokuport);
            if (skt.Connected)
            {
                int bytes = skt.Send(Encoding.ASCII.GetBytes(stringtosend),SocketFlags.None);                
                Console.WriteLine(bytes + " Bytes Sent");                
            }
            Thread.Sleep(100);
            //byte [] bytes1 = new byte[8000];
            //skt.Receive(bytes1);
            skt.Close();
        }

        private void SendKeyAlphaString(string stringtosend)
        {
            // stringtosend = "POST /keypress/Lit_" + stringtosend + " HTTP/1.1\r\n\r\n";
            char[] charstosend = stringtosend.ToArray();
            foreach (var chartosend in charstosend)
            {
                if( chartosend == ' ' )
                    SendKeyPressString("Lit_%20" + chartosend);
                else 
                    SendKeyPressString("Lit_" + chartosend);                
            }
            SendKeyPressString("Enter");
            textBox1.Text = "";
        }

        private void RokuRemote_Load(object sender, EventArgs e)
        {
            notifyIcon1.Text = "Roku Remote Control";
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);
        }

        void RokuRemote_MinimumSizeChanged(object sender, System.EventArgs e)
        {
            
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        void RokuRemote_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        void RokuRemote_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    SendKeyPressString("Up");
                    break;
                case Keys.Down:
                    SendKeyPressString("Down");
                    break;
                case Keys.Right:
                    SendKeyPressString("Right");
                    break;
                case Keys.Left:
                    SendKeyPressString("Left");
                    break;
                case Keys.Home:
                    SendKeyPressString("Home");
                    break;
                case Keys.Back:
                    SendKeyPressString("Back");
                    break;
                case Keys.PageUp:
                    SendKeyPressString("Fwd");
                    break;
                case Keys.PageDown:
                    SendKeyPressString("Rev");
                    break;

            }
        }

        void RokuRemote_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //MessageBox.Show(e.KeyChar.ToString());
            switch (e.KeyChar)
            {
                case ' ':
                    if (textBox1.Text.Length == 0)
                    {
                        textBox1.Text = textBox1.Text.Trim();
                        SendKeyPressString("Play");
                    }                   
                    break;
                case '\r':
                    if (textBox1.Text.Length > 0)
                        SendKeyAlphaString(textBox1.Text);
                    else
                        SendKeyPressString("Select");
                    break;
                case '\b':
                    SendKeyPressString("Back");
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string teststring = textBox1.Text.Trim();
            if (teststring.Length == 0)
                textBox1.Text = "";
        }

    }
}
