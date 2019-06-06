﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Services;

namespace ToP.Bridge
{
    public partial class Main : Form
    {
        private IrcService IrcLink { get; set; }
        private DiscordService DiscordLink { get; set; }
        private BridgeService glue { get; set; }
        delegate void TextBoxDelegate(string text);
        public Main()
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
                TextBoxDelegate d = InputLog;
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
                TextBoxDelegate d = OutputLog;
                this.Invoke(d, new object[] { text });
            }
        }
        public void EventLog(string text)
        {
            if (!richTextBox1.InvokeRequired)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = Color.Black;
            }
            else
            {
                TextBoxDelegate d = EventLog;
                this.Invoke(d, new object[] { text });
            }
        }
        public void DiscordLog(string text)
        {
            if (!richTextBox1.InvokeRequired)
            {
                richTextBox2.SelectionStart = richTextBox1.TextLength;
                richTextBox2.SelectionLength = 0;
                richTextBox2.SelectionColor = Color.Black;
                richTextBox2.AppendText(text + "\r\n");
                richTextBox2.SelectionColor = Color.Black;
            }
            else
            {
                TextBoxDelegate d = DiscordLog;
                this.Invoke(d, new object[] { text });
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BridgeConfig config = ConfigHelper.LoadConfig();
            IrcLink = new IrcService(InputLog, OutputLog, EventLog, config.IRCServer);
            DiscordLink = new DiscordService(DiscordLog, config.DiscordServer);
            glue = new BridgeService(IrcLink, DiscordLink, config);

            DiscordLink.OnChannelMessage += glue.DiscordChannelMessage;
            DiscordLink.OnGuildConnected += glue.DiscordGuildConnected;
            DiscordLink.OnUserUpdated += glue.DiscordUserUpdated;
            IrcLink.OnChannelMessage += glue.IrcChannelMessage;

            // Start the Async Processing
            DiscordLink.MainAsync().GetAwaiter();
            IrcLink.StartBridge(); // Sort of Async.. Fix this later
        }
        

        private void BtnSend_Click(object sender, EventArgs e)
        {
            IrcLink.SendMessage(txtUserName.Text, txtMessage.Text, txtChannel.Text);
            txtMessage.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            IrcLink.RegisterNick(txtUserName.Text);
        }

        private void TxtUserName_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter)
            {
                IrcLink.RegisterNick(txtUserName.Text);
            }
        }

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            IrcLink.JoinChannel(txtUserName.Text, txtChannel.Text);
        }

        private void TxtChannel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                IrcLink.JoinChannel(txtUserName.Text, txtChannel.Text);
            }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            IrcLink.SendAction(txtUserName.Text, txtMessage.Text, txtChannel.Text);
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
                IrcLink.SendMessage(txtUserName.Text, txtMessage.Text, txtChannel.Text);
                txtMessage.Clear();
            }
        }

        private void BtnChangeNick_Click(object sender, EventArgs e)
        {
            IrcLink.ChangeNick(txtUserName.Text, txtChangeNick.Text);
        }

        private void BtnAway_Click(object sender, EventArgs e)
        {
            IrcLink.SetAway(txtUserName.Text, true);
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            IrcLink.SetAway(txtUserName.Text, false);
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            IrcLink.DisconnectUser(txtUserName.Text);
        }
    }
}
