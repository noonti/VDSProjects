using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDSCommon.VitualKeyboard
{
    public partial class VirtualKeybardForm : Form
    {
        [DllImport("user32.dll")]
        static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_CHAR = 0x105;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;


        private Control _target = null;


        private List<ucKeyButton>[] _buttons = new List<ucKeyButton>[5];

        private ucKeyButton[] _buttons100 = new ucKeyButton[14];
        private ucKeyButton[] _buttons200 = new ucKeyButton[14];
        private ucKeyButton[] _buttons300 = new ucKeyButton[13];
        private ucKeyButton[] _buttons400 = new ucKeyButton[12];
        private ucKeyButton[] _buttons500 = new ucKeyButton[8];

        // 초기화...
        int _selRow = 2;
        int _selCol = 5;

        ucKeyButton _selectedButton = null;
        bool _initialLoad = false;


        public VirtualKeybardForm()
        {
            InitializeComponent();
            InitButtons();
        }

        public void InitButtons()
        {
            int i = 0;
            List<string> aaa = new List<string>();
            _buttons[i] = new List<ucKeyButton>();

            ucKey0101.SetKeyCode(Keys.Oem3);
            _buttons[i].Add(ucKey0101);


            ucKey0102.SetKeyCode(Keys.D1);
            _buttons[i].Add(ucKey0102);

            ucKey0103.SetKeyCode(Keys.D2);
            _buttons[i].Add(ucKey0103);

            ucKey0104.SetKeyCode(Keys.D3);
            _buttons[i].Add(ucKey0104);

            ucKey0105.SetKeyCode(Keys.D4);
            _buttons[i].Add(ucKey0105);

            ucKey0106.SetKeyCode(Keys.D5);
            _buttons[i].Add(ucKey0106);

            ucKey0107.SetKeyCode(Keys.D6);
            _buttons[i].Add(ucKey0107);

            ucKey0108.SetKeyCode(Keys.D7);
            _buttons[i].Add(ucKey0108);

            ucKey0109.SetKeyCode(Keys.D8);
            _buttons[i].Add(ucKey0109);

            ucKey0110.SetKeyCode(Keys.D9);
            _buttons[i].Add(ucKey0110);

            ucKey0111.SetKeyCode(Keys.D0);
            _buttons[i].Add(ucKey0111);

            ucKey0112.SetKeyCode(Keys.OemMinus);
            _buttons[i].Add(ucKey0112);

            ucKey0113.SetKeyCode(Keys.Oemplus);
            _buttons[i].Add(ucKey0113);

            ucKey0114.SetKeyCode(Keys.Back);
            _buttons[i].Add(ucKey0114);

            i++;
            _buttons[i] = new List<ucKeyButton>();

            ucKey0201.SetKeyCode(Keys.Tab);
            _buttons[i].Add(ucKey0201);

            ucKey0202.SetKeyCode(Keys.Q);
            _buttons[i].Add(ucKey0202);

            ucKey0203.SetKeyCode(Keys.W);
            _buttons[i].Add(ucKey0203);

            ucKey0204.SetKeyCode(Keys.E);
            _buttons[i].Add(ucKey0204);

            ucKey0205.SetKeyCode(Keys.R);
            _buttons[i].Add(ucKey0205);

            ucKey0206.SetKeyCode(Keys.T);
            _buttons[i].Add(ucKey0206);

            ucKey0207.SetKeyCode(Keys.Y);
            _buttons[i].Add(ucKey0207);

            ucKey0208.SetKeyCode(Keys.U);
            _buttons[i].Add(ucKey0208);

            ucKey0209.SetKeyCode(Keys.I);
            _buttons[i].Add(ucKey0209);

            ucKey0210.SetKeyCode(Keys.O);
            _buttons[i].Add(ucKey0210);

            ucKey0211.SetKeyCode(Keys.P);
            _buttons[i].Add(ucKey0211);

            ucKey0212.SetKeyCode(Keys.Oem4);
            _buttons[i].Add(ucKey0212);

            ucKey0213.SetKeyCode(Keys.Oem6);
            _buttons[i].Add(ucKey0213);

            ucKey0214.SetKeyCode(Keys.Oem5);
            _buttons[i].Add(ucKey0214);



            i++;
            _buttons[i] = new List<ucKeyButton>();

            ucKey0301.SetKeyCode(Keys.CapsLock);
            _buttons[i].Add(ucKey0301);

            ucKey0302.SetKeyCode(Keys.A);
            _buttons[i].Add(ucKey0302);

            ucKey0303.SetKeyCode(Keys.S);
            _buttons[i].Add(ucKey0303);

            ucKey0304.SetKeyCode(Keys.D);
            _buttons[i].Add(ucKey0304);

            ucKey0305.SetKeyCode(Keys.F);
            _buttons[i].Add(ucKey0305);

            ucKey0306.SetKeyCode(Keys.G);
            _buttons[i].Add(ucKey0306);

            ucKey0307.SetKeyCode(Keys.H);
            _buttons[i].Add(ucKey0307);

            ucKey0308.SetKeyCode(Keys.J);
            _buttons[i].Add(ucKey0308);

            ucKey0309.SetKeyCode(Keys.K);
            _buttons[i].Add(ucKey0309);

            ucKey0310.SetKeyCode(Keys.L);
            _buttons[i].Add(ucKey0310);

            ucKey0311.SetKeyCode(Keys.OemSemicolon);
            _buttons[i].Add(ucKey0311);

            ucKey0312.SetKeyCode(Keys.OemQuotes);
            _buttons[i].Add(ucKey0312);

            ucKey0313.SetKeyCode(Keys.Enter);
            _buttons[i].Add(ucKey0313);


            i++;
            _buttons[i] = new List<ucKeyButton>();


            ucKey0401.SetKeyCode(Keys.LShiftKey);
            _buttons[i].Add(ucKey0401);

            ucKey0402.SetKeyCode(Keys.Z);
            _buttons[i].Add(ucKey0402);

            ucKey0403.SetKeyCode(Keys.X);
            _buttons[i].Add(ucKey0403);

            ucKey0404.SetKeyCode(Keys.C);
            _buttons[i].Add(ucKey0404);

            ucKey0405.SetKeyCode(Keys.V);
            _buttons[i].Add(ucKey0405);

            ucKey0406.SetKeyCode(Keys.B);
            _buttons[i].Add(ucKey0406);

            ucKey0407.SetKeyCode(Keys.N);
            _buttons[i].Add(ucKey0407);

            ucKey0408.SetKeyCode(Keys.M);
            _buttons[i].Add(ucKey0408);

            ucKey0409.SetKeyCode(Keys.Oemcomma);
            _buttons[i].Add(ucKey0409);

            ucKey0410.SetKeyCode(Keys.OemPeriod);
            _buttons[i].Add(ucKey0410);

            ucKey0411.SetKeyCode(Keys.OemQuestion);
            _buttons[i].Add(ucKey0411);

            ucKey0412.SetKeyCode(Keys.RShiftKey);
            _buttons[i].Add(ucKey0412);



            i++;
            _buttons[i] = new List<ucKeyButton>();

            ucKey0501.SetKeyCode(Keys.LControlKey);
            _buttons[i].Add(ucKey0501);

            ucKey0502.SetKeyCode(Keys.LMenu);
            _buttons[i].Add(ucKey0502);

            ucKey0503.SetKeyCode(Keys.HanjaMode);
            _buttons[i].Add(ucKey0503);

            ucKey0504.SetKeyCode(Keys.Space);
            _buttons[i].Add(ucKey0504);

            ucKey0505.SetKeyCode(Keys.HangulMode);
            _buttons[i].Add(ucKey0505);

            ucKey0506.SetKeyCode(Keys.RMenu);
            _buttons[i].Add(ucKey0506);

            ucKey0507.SetKeyCode(Keys.Apps);
            _buttons[i].Add(ucKey0507);

            ucKey0508.SetKeyCode(Keys.RControlKey);
            _buttons[i].Add(ucKey0508);


            _selRow = 2;
            _selCol = 5;



        }

        public void SetTargetControl(Control target, Form parent)
        {
            if (target != null)
            {
                Form mainform = Application.OpenForms[0];


                Point startPos =  Utility.GetScreenPosition(target, parent) ;// target.PointToScreen(new Point(target.Left, target.Top));
                Console.WriteLine($"target pos={target.Left},{target.Top} --> screen pos = {startPos.X}.{startPos.Y}");
                //this.Location = startPos;
                this.Left =   startPos.X;
                this.Top =  startPos.Y + target.Height;
            }
            _target = target;
        }


        private void DisplaySelectedButton(bool focus)
        {
            if (_selectedButton != null)
            {
                _selectedButton.SetFocus(focus);
            }
        }

        private void MoveNextButton(Keys KeyCode)
        {
            // 기존 버튼 해제...
            DisplaySelectedButton(false);

            switch (KeyCode)
            {
                case Keys.Left: // Left/up move 
                    _selCol--;
                    break;

                case Keys.Right: // right/down move
                    _selCol++;
                    break;
            }

            if (_selCol >= _buttons[_selRow].Count) //LEFT : Move Up
            {
                _selRow++;
                _selCol = 0;
            }
            else if (_selCol < 0)                    // UP
            {
                _selRow--;
                _selCol = 0;
            }
            if (_selRow >= 5)
            {
                _selRow = 0;
            }
            else if (_selRow < 0)
            {
                _selRow = 4;
            }

            _selectedButton = _buttons[_selRow][_selCol];
            _selectedButton.SetFocus(true);


        }




        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            Keys keys = keyData & ~(Keys.Shift | Keys.Control | Keys.Alt);
            Console.WriteLine("ProcessCmdKey  ");
            switch (keys)
            {
                case Keys.Up: // ESC 
                    break;

                case Keys.Down: // ENTER  
                    SendButtonKey();
                    break;
                case Keys.Left: // Left/up move 
                    MoveNextButton(Keys.Left);
                    break;

                case Keys.Right: // right/down move
                    MoveNextButton(Keys.Right);
                    break;
            }

            return base.ProcessCmdKey(ref message, keyData);
        }


        private void SendButtonKey()
        {
            if (_selectedButton != null && _target != null)
            {
                PostMessage(_target.Handle, WM_KEYDOWN, Convert.ToInt32(_selectedButton.GetKeyCode()), 0);
            }
        }

        private void VirtualKeybardForm_Activated(object sender, EventArgs e)
        {
            if (!_initialLoad)
            {
                _selectedButton = _buttons[_selRow][_selCol];
                DisplaySelectedButton(true);
                _initialLoad = true;
            }
        }
    }
}
