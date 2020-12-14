using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Packets;

namespace Client
{
    public partial class ClientForm : Form
    {

        Client _client;
        public ClientForm(Client client)
        {
            InitializeComponent();
            _client = client;
        }

        public void UpdateChatWindow(string message)
        {
            if (MessageWindow.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateChatWindow(message);
                }));
            }
            else
            {
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ChatMessagePacket clientMessage = new ChatMessagePacket(InputField.Text);
            if (InputField.Text != "")
            {
                _client.SendMessage(clientMessage);
                UpdateChatWindow(InputField.Text);
            }
            InputField.Text = null;
        }
    }
}
