using DarkUI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.API.Model;

namespace VDSWebAPIServer
{
    public class VDSListItem : ListViewItem
    {
        VDS_CONTROLLER vdsController;
        List<Control> controlList = new List<Control>();
        Control _parentControl;

        LedBulb onlineLed = new LedBulb();
        LedBulb fontLed = new LedBulb();
        LedBulb rearLed = new LedBulb();
        LedBulb fanLed = new LedBulb();
        LedBulb heaterLed = new LedBulb();

        DarkButton rtspStreamingBtn = new DarkButton();
        DarkButton remoteConfigBtn = new DarkButton();
        DarkButton modifyBtn = new DarkButton();
        DarkButton deleteBtn = new DarkButton();

        public VDSListItem(VDS_CONTROLLER controller, Control parent) : base(controller.GROUP_NAME)
        {

            _parentControl = parent;
            vdsController = controller;
            SubItems.Add(vdsController.CONTROLLER_ID);
            SubItems.Add(vdsController.CONTROLLER_NAME);
            SubItems.Add(vdsController.VDS_TYPE_NAME);
            SubItems.Add(vdsController.USE_YN);
            SubItems.Add(vdsController.PROTOCOL_NAME);
            SubItems.Add(vdsController.IP_ADDRESS);

            SubItems.Add("오프라인");
            SubItems.Add("-");
            SubItems.Add("-");
            SubItems.Add("-");
            SubItems.Add("-");

            SubItems.Add("°C");
            SubItems.Add("영상보기");
            SubItems.Add("원격설정");
            SubItems.Add("수정");
            SubItems.Add("삭제");

        }

        //public void SetControl()
        //{
        //    int index = 7;

        //    return;
        //    // 통신상태 LED
        //    //onlineLed.Parent = _parentControl;
        //    //ListViewSubItem subItem = null;
        //    //subItem = SubItems[index++];
        //    //Rectangle rt = new Rectangle();
        //    //rt = subItem.Bounds;
        //    //onlineLed.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //    //controlList.Add(onlineLed);

        //    ListViewSubItem subItem = null;
        //    subItem = SubItems[index++];
        //    subItem.Text = "";


        //    fontLed.Parent = _parentControl;
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    Rectangle rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    fontLed.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //    controlList.Add(fontLed);

        //    rearLed.Parent = _parentControl;
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    rearLed.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //    controlList.Add(rearLed);

        //    fanLed.Parent = _parentControl;
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    fanLed.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //    controlList.Add(fanLed);

        //    heaterLed.Parent = _parentControl;
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    heaterLed.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //    controlList.Add(heaterLed);


          
        //    index++;// 함체온도 스킵


        //    rtspStreamingBtn.Parent = _parentControl;
        //    rtspStreamingBtn.Font = new Font("굴림",9);
        //    rtspStreamingBtn.Text = "영상보기";
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    rtspStreamingBtn.SetBounds(rt.X, rt.Y, 80, rt.Height);
        //    controlList.Add(rtspStreamingBtn);


        //    remoteConfigBtn.Parent = _parentControl;
        //    remoteConfigBtn.Font = new Font("굴림", 9);
        //    remoteConfigBtn.Text = "원격설정";
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    remoteConfigBtn.SetBounds(rt.X, rt.Y, 80, rt.Height);
        //    controlList.Add(remoteConfigBtn);


        //    modifyBtn.Parent = _parentControl;
        //    modifyBtn.Font = new Font("굴림", 9);
        //    modifyBtn.Text = "수정";
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    modifyBtn.SetBounds(rt.X, rt.Y, 80, rt.Height);
        //    controlList.Add(modifyBtn);


        //    deleteBtn.Parent = _parentControl;
        //    deleteBtn.Font = new Font("굴림", 9);
        //    deleteBtn.Text = "삭제";
        //    subItem = null;
        //    subItem = SubItems[index++];
        //    rt = new Rectangle();
        //    rt = subItem.Bounds;
        //    deleteBtn.SetBounds(rt.X, rt.Y, 80, rt.Height);
        //    controlList.Add(deleteBtn);



        //    // 최종 수신 시간 
        //    // 앞문 열림 LED
        //    // 뒷문 열림 LED
        //    // Fan    LED
        //    // Heater  LED 
        //    // 함체 온도 TExt
        //    // 영상보기 button
        //    // 원격 설정 button  
        //    // 수정 button 
        //    // 삭제 button 

        //    //SubItems.Add(vdsController.MODIFY_DATE);
        //    //SubItems.Add(vdsController.REG_DATE);

        //    //ProgressBar pb = new ProgressBar();
        //    //pb.Minimum = 0;
        //    //pb.Maximum = 100;
        //    //pb.Value = 30;
        //    //pb.Parent = lvView;
        //    //ListViewItem.ListViewSubItem ps = null;
        //    //ps = item.SubItems[3];
        //    //Rectangle rt = new Rectangle();
        //    //rt = ps.Bounds;
        //    //pb.SetBounds(rt.X, rt.Y, 100, rt.Height);


        //    //LedBulb led = new LedBulb();
        //    //led.Parent = lvView;
        //    //ps = item.SubItems[4];
        //    //rt = new Rectangle();
        //    //rt = ps.Bounds;
        //    //led.SetBounds(rt.X, rt.Y, 100, rt.Height);
        //}

        public void DisposeControl()
        {
            for(int i=0;i<controlList.Count;i++)
            {
                controlList[i].Dispose();
            }
            controlList.Clear();
        }

        public void UpdateVDSControllerInfo(VDS_CONTROLLER controller)
        {
            int index = 2;

            ListViewSubItem subItem = null;

            subItem = SubItems[index++]; // 2
            subItem.Text = controller.CONTROLLER_NAME;


            subItem = SubItems[index++]; // 3
            subItem.Text = controller.VDS_TYPE_NAME;

           

            subItem = SubItems[index++]; // 4
            subItem.Text = controller.USE_YN;

            subItem = SubItems[index++]; // 5
            subItem.Text = controller.PROTOCOL_NAME;

            subItem = SubItems[index++]; // 6
            subItem.Text = controller.IP_ADDRESS;



            subItem = SubItems[index++]; // 7
            subItem.Text = String.IsNullOrEmpty(controller.LAST_HEARTBEAT_TIME)?"오프라인":String.Format($"온라인({controller.LAST_HEARTBEAT_TIME})");

            if(!String.IsNullOrEmpty(controller.LAST_HEARTBEAT_TIME) && controller.rackStatus!=null)
            {
                subItem = SubItems[index++]; // 8
                subItem.Text = controller.rackStatus.IsFrontDoorOpen == 1 ? "열림" : "닫힘";


                subItem = SubItems[index++]; // 9
                subItem.Text = controller.rackStatus.IsRearDoorOpen == 1 ? "열림" : "닫힘";


                subItem = SubItems[index++]; // 10
                subItem.Text = controller.rackStatus.IsFanOn == 1 ? "작동" : "미작동";

                subItem = SubItems[index++]; // 11
                subItem.Text = controller.rackStatus.IsHeaterOn == 1 ? "작동" : "미작동";


                subItem = SubItems[index++]; // 12
                subItem.Text = String.Format($"{controller.rackStatus.Temperature} °C");

            }
            else
            {
                subItem = SubItems[index++]; // 8
                subItem.Text = "-";


                subItem = SubItems[index++]; // 9
                subItem.Text = "-";


                subItem = SubItems[index++]; // 10
                subItem.Text = "-";

                subItem = SubItems[index++]; // 11
                subItem.Text = "-";


                subItem = SubItems[index++]; // 12
                subItem.Text = "°C";
            }


        }
    }
}
