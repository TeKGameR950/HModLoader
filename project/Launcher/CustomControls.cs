using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class HCheckbox : Panel
{
    public EventHandler<bool> OnChanged;

    public bool Checked
    {
        get
        {
            return _checked;
        }
        set
        {
            OnChanged?.Invoke(this,value);
            _checked = value;
            this.Invalidate();
        }
    }
    public bool _checked;

    public HCheckbox()
    {
        Size = new Size(30, 30);
        Tag = "r:5";
        Cursor = Cursors.Hand;
        MouseDown += (a, b) =>
        {
            Checked = !Checked;
            this.Invalidate();
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Checked)
            e.Graphics.DrawImage(Launcher.Properties.Resources.check_solid, new Rectangle(2, 2, Width-6, Height-6));
        
    }
}

public class TransparentPanel : Panel
{
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
            return cp;
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {

    }
}

public class PageForm : Form
{
    public bool PageOpened = false;

    public virtual void OnPageOpened()
    {

    }
}
