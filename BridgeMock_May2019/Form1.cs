﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace BridgeMock_May2019
{
    public partial class Form1 : Form
    {
        private BridgeService bridge;
        //delegate void OutputDelegate(string text);
        delegate void InputDelegate(string text);
        public Form1()
        {
            InitializeComponent();
        }

        public void InputLog(string text)
        {
            if (!richTextBox1.InvokeRequired) { 
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = Color.DarkRed;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = Color.Black;
           }
             else
            {
                InputDelegate d = new InputDelegate(InputLog);
                this.Invoke(d, new object[] { text });
            }
        }

        public void OutputLog(string text)
        {
            if (!richTextBox1.InvokeRequired)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = Color.DarkBlue;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = Color.Black;
            }
            else
            {
                InputDelegate d = new InputDelegate(OutputLog);
                this.Invoke(d, new object[] { text });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry("192.168.1.6");
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bridge = new BridgeService("192.168.1.6", 7001, "GoldenRetriever", InputLog, OutputLog);
            bridge.StartBridge();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            bridge.MessageChannel(txtUserName.Text, txtMessage.Text);
            txtMessage.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            bridge.RegisterNick(txtUserName.Text);
        }

        private void TxtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                bridge.RegisterNick(txtUserName.Text);
            }
        }

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            bridge.JoinChannel(txtUserName.Text, txtChannel.Text);
        }

        private void TxtChannel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bridge.JoinChannel(txtUserName.Text, txtChannel.Text);
            }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            bridge.Action(txtUserName.Text, txtMessage.Text);
            txtMessage.Clear();
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bridge.MessageChannel(txtUserName.Text, txtMessage.Text, txtChannel.Text);
                txtMessage.Clear();
            }
        }
    }
}
