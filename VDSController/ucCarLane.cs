using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.DataType;
using System.Threading;
using VDSCommon;

namespace VDSController
{
    public partial class ucCarLane : UserControl
    {
        public int _direction;
        public int _lane;

        public int _interval; // timer interval 
        public String _caption;

        List<ucCarInfo> carList = new List<ucCarInfo>();
        //Timer timer = new Timer();

        Thread carMoveThread;
        public ucCarLane()
        {
            InitializeComponent();


            _lane = 0;
            _direction = 1; // 1: TO LEFT 2: TO RIGHT

            //timer.Interval = 100;
            //timer.Tick += MoveCar;

        }


        public void SetCarDirection(int direction)
        {
            _direction = direction;

        }

        public void StartTimer()
        {
            //timer.Start();
        }

        public int AddCar(TrafficDataEvent trafficDataEvent)
        {
            ucCarInfo car = new ucCarInfo();
            try
            {
                car.SetCarInfo(trafficDataEvent);
                car.moveOffset = GetMoveOffset(trafficDataEvent);
                pnLane.Controls.Add(car);

#if false
            // 중앙에서 출발
            if (_direction == 2) // TO LEFT
                car.Location = new Point(pnLane.Width / 2 - car.Width, (pnLane.Height - car.Height) / 2);
            else                // TO RIGHT
                car.Location = new Point(pnLane.Width / 2, (pnLane.Height - car.Height) / 2);
#else
                // 양끝에서 출발
                if (_direction == 2) // TO LEFT
                    car.Location = new Point(pnLane.Width - car.Width, (pnLane.Height - car.Height) / 2);
                else                // TO RIGHT
                    car.Location = new Point(0, (pnLane.Height - car.Height) / 2);

#endif
                carList.Add(car);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"ADdCra error...{ex.StackTrace.ToString()}");
            }
            
            return carList.Count;
        }

        //public int AddCar(String caption)
        //{
        //    //return 0;
        //    ucCarInfo car = new ucCarInfo();
        //    //car.AutoSize = false;
        //    //car.Width = 64;
        //    //car.Height = 64; 
        //    car.SetCarInfo(caption,_direction);
        //    pnLane.Controls.Add(car);

        //    if (_direction == 1) // TO LEFT
        //        car.Location = new Point(pnLane.Width / 2 - car.Width, (pnLane.Height - car.Height) / 2);
        //    else                // TO RIGHT
        //        car.Location = new Point(pnLane.Width / 2, (pnLane.Height - car.Height) / 2);
        //    carList.Add(car);
        //    return carList.Count;
        //}


        public void MoveCar()
        {
            List<ucCarInfo> delList = new List<ucCarInfo>();
            int offset = 15;
            //if (_direction == 1) // to Left
            //    offset = -offset;
            foreach (var car in carList)
            {
                if (_direction == 2) // to Left
                    offset = -car.moveOffset;
                else
                    offset = car.moveOffset;

                if (car.bMoving)
                    car.Location = new Point(car.Location.X + offset, car.Location.Y);

                //Console.WriteLine($"MoveCar: {car.carInfo}, car.moveOffset={car.moveOffset} , offset={offset} ");

                if ((_direction == 2 && car.Location.X < -car.Width) ||
                    (_direction == 1 && car.Location.X > pnLane.Width)
                    )
                    delList.Add(car);
            }

            foreach (var car in delList)
            {
                pnLane.Controls.Remove(car);
                carList.Remove(car);
            }
            //if (pnLane.Controls.Count + carList.Count > 5)
            //    Console.WriteLine("Lane={2} ,MoveCar....control count={0}, carList.Count = {1}", pnLane.Controls.Count, carList.Count, _lane);
        }

        //private void ucCarLane_MouseMove(object sender, MouseEventArgs e)
        //{

        //}

        //private void pnLane_MouseDown(object sender, MouseEventArgs e)
        //{
        //}

        //private void ucCarLane_Load(object sender, EventArgs e)
        //{

        //}

        private void ucCarLane_Resize(object sender, EventArgs e)
        {
            pbCenter.Location = new Point(pnLane.Width / 2, 0);
            pbCenter.Width = 5;
            pbCenter.Height = pnLane.Height;
        }

        private void ucCarLane_Load(object sender, EventArgs e)
        {
            //StartCarMoveThread();
        }

        public void StartCarMoveThread()
        {
            if (carMoveThread == null)
            {
                carMoveThread = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            this.Invoke(new Action(delegate ()
                            {
                                MoveCar();
                                Update();


                            }));
                            Thread.Sleep(100);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
                }
                );
            }
            carMoveThread.Start();

        }

        public int GetMoveOffset(TrafficDataEvent trafficDataEvent)
        {
            int result = 15;
//#if false
//            int unit = 10;
//            if (targetInfo.SPEED_X100 > 100)
//                result = unit*7;
//            else if (targetInfo.SPEED_X100 > 90)
//                result = unit * 5;
//            else if (targetInfo.SPEED_X100 > 80)
//                result = unit * 3;
//            else
//                result = unit;
//#else
            
//            int distance = 60; // 화면상 보이는 통과 거리(m). 보이는 것은 30미터 이나 늦추기 위해 60으로..
//            double elapsed_time = 0;


//            //targetInfo.SPEED_X100
//            // 30 미터 통과 시 소요시간 계산
//            elapsed_time = (distance * 3600) / (trafficDataEvent.speed * 1000);

//            // 소요시간(elapsed_time) 내에 pnLane 을 지나가야 하므로 interval 로 나눈 갯수 구한다.

//                result = (int) (pnLane.Width /( (elapsed_time * 1000 / _interval)));
//            //pnLane.Width 

            
            
//#endif
            return result;
        }
    }
}
