using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormRequestCredentials : Form
    {
        public FormRequestCredentials(string strCaption, string strUser)
        {
            InitializeComponent();
            textBoxUser.Text = strUser;
            Text = strCaption;
        }

        public string User
        { get { return textBoxUser.Text; } }
        public string Password
        { get { return textBoxPassword.Text; } }
    }
}