using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDSCommon.VitualKeyboard
{
    public partial class ucKeyButton : UserControl
    {
        //Control target = null;

        Keys key;

        bool bLoaded = false;
        public ucKeyButton()
        {
            InitializeComponent();

        }


        public void SetFocus(bool focused)
        {
            DrawSelection(focused);
        }


        private void DrawSelection(bool focused)
        {
            //int margin = 5;

            //if (this.Width > 0 && this.Height > 0)
            //{
            //    Rectangle region = new Rectangle(pbButton.PointToScreen(new Point(margin, margin)), new Size(this.Width - 2 * margin, this.Height - 2 * margin));
            //    ControlPaint.DrawReversibleFrame(region,
            //        Color.Red, FrameStyle.Dashed);
            //}

            if (focused)
                pbButton.BorderStyle = BorderStyle.Fixed3D;
            else
                pbButton.BorderStyle = BorderStyle.None;

        }


        private void ucKeyButton_Load(object sender, EventArgs e)
        {
            if (!bLoaded)
            {
                String tag = this.Tag != null ? this.Tag.ToString() : null;
                pbButton.Image = GetKeyboardImage(tag);// Properties.Resources.K0105;

                bLoaded = true;
            }
        }

        private Keys GetKeys(String tag)
        {
            Keys result = Keys.None;

            return result;
        }


        private Bitmap GetKeyboardImage(String tag)
        {
            // Properties.Resources.K0101

            if (String.IsNullOrEmpty(tag))
            {
                key = GetKeys(tag);
                return Properties.Resources.K0105;
            }

            else
                return (Bitmap)Properties.Resources.ResourceManager.GetObject(tag);

        }

        public void SetKeyCode(Keys k)
        {
            key = k;
        }
        public Keys GetKeyCode()
        {
            return key;
        }
    }
}
