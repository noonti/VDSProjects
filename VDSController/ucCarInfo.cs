using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.DataType;

namespace VDSController
{
    public partial class ucCarInfo : UserControl
    {
        public bool bMoving = true;
        public int moveOffset = 15;
        public String carInfo = String.Empty;
        public ucCarInfo()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        

        public void SetCarInfo(TrafficDataEvent trafficDataEvent)
        {
            carInfo = String.Format($"{trafficDataEvent.speed} km/h \n{trafficDataEvent.length / 100} m");
            lbCarInfo.Text = carInfo ;
            Utility.VEHICLE_LENGTH_CATEGORY category = Utility.GetVehicleLengthCategory(trafficDataEvent.length / 100);

            if(trafficDataEvent.direction == 2) // TO LEFT
            {
                switch(category)
                {
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "small_left.png"));
                        break;
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "middle_left.png"));
                        break;
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "large_left.png"));
                        break;

                }

            }
            else if(trafficDataEvent.direction == 1) // TO RIGHT
            {
                switch (category)
                {
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "small_right.png"));
                        break;
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "middle_right.png"));
                        break;
                    case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE:
                        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "large_right.png"));
                        break;

                }

            }

            //switch (dirction)
            //{
            //    case 1:
            //        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "car_left.png"));
            //        break;
            //    case 2:
            //        pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "car_right.png"));
            //        break;
            //}

        }


        public void SetCarInfo(String carInfo, int dirction)
        {
            lbCarInfo.Text = carInfo;
            switch(dirction)
            {
                case 1:
                    pictureBox1.Load(String.Format(@"{0}\Image\{1}",Utility.GetApplicationPath(), "car_left.png"));
                    break;
                case 2:
                    pictureBox1.Load(String.Format(@"{0}\Image\{1}", Utility.GetApplicationPath(), "car_right.png"));
                    break;
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bMoving = !bMoving;
            }
        }
    }
}
