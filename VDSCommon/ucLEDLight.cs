using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDSCommon
{
    public partial class ucLEDLight : UserControl
    {
        int _blinkType;

        //[Category("Title"), Description("제목")]
        //public String Title
        //{
        //    get
        //    {
        //        return this.lbTitle.Text;
        //    }
        //    set
        //    {
        //        this.lbTitle.Text = value;
        //    }

        //}


        public ucLEDLight()
        {
            InitializeComponent();
            _blinkType = 0; // 0: not blik 1: blink just 1 time(100ms)

        }

        

        public void SetBlinkType(int blinkType)
        {
            _blinkType = blinkType;
        }

        public void SetOn(int on)
        {
            switch(_blinkType)
            {
                case 0: // 
                    SetLightOn(on);
                    break;
                case 1:
                    SetLightBlink();
                    break;
            }
        }

        public void SetLightOn(int on)
        {
            ledLight.On = on == 1 ? true : false;
        }

        public void SetLightBlink()
        {
            ledLight.Blink(200);
        }
    }
}
