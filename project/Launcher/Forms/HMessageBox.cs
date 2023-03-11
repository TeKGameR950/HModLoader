using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Launcher.Forms
{
    public partial class HMessageBox : Form
    {
        public HMessageBox()
        {
            InitializeComponent();
        }

        public HMessageBox(string title, string description, MessageBoxButtons buttons = MessageBoxButtons.OK,string path = "")
        {
            Opacity = 0;
            InitializeComponent();

            _title.Text = title;
            _label.Text = description;

            Height = _label.Height + 30 + 50 + 10;
            if (path != "")
            {
                _path.Visible = true;
                _path.Text = path;
                _path.ReadOnly = true;
                Height += 30;
            }
            Width = _label.Width + 20;
            this.RoundCorners();
            this.InitFormUtils();
            if (buttons != (MessageBoxButtons)10)
            {
                _okbtn.Visible = false;
                _yesbtn.Visible = false;
                _nobtn.Visible = false;
                _cancelbtn.Visible = false;
            }
            if (buttons != (MessageBoxButtons)10)
            {
                switch (buttons)
                {
                    case MessageBoxButtons.OK:
                        _okbtn.Visible = true;
                        break;
                    case MessageBoxButtons.YesNo:
                        _yesbtn.Visible = true;
                        _nobtn.Visible = true;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        _yesbtn.Visible = true;
                        _nobtn.Visible = true;
                        _cancelbtn.Visible = true;
                        break;
                    case MessageBoxButtons.OKCancel:
                        _okbtn.Visible = true;
                        _cancelbtn.Visible = true;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        _cancelbtn.Visible = true;
                        break;
                }
            }

            _close.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            _nobtn.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.No;
                Close();
            };
            _nolabel.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.No;
                Close();
            };
            _yesbtn.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.Yes;
                Close();
            };
            _yeslabel.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.Yes;
                Close();
            };
            _cancelbtn.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            _cancellabel.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            _okbtn.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
            _oklabel.MouseDown += (a, b) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
            Opacity = 1;
        }
    }
}
