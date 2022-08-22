using DarkUI.Controls;
using DarkUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;

namespace VDSController
{
    public partial class LaneManageForm : DarkForm
    {
        List<LANE_GROUP> laneGroupList = new List<LANE_GROUP>();

        public LaneManageForm()
        {
            InitializeComponent();
            GetLaneGroupList();
            GetLaneInfoList(laneGroupList);
            SetLaneInfo();
        }

        private void darkButton8_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtLeftLaneGroupName.Text))
            {
                MessageBox.Show("행선지명을 입력하세요", "입력오류");
                return;
            }

            if(cbLeftSortKind.SelectedIndex <0)
            {
                MessageBox.Show("정렬 방식을 선택하세요", "입력오류");
                return;
            }


            LANE_GROUP laneGroup = laneGroupList.Where(x => x.DIRECTION == (int)MOVE_DIRECTION.TO_LEFT).FirstOrDefault();
            laneGroup.LANE_GROUP_NAME = txtLeftLaneGroupName.Text;
            laneGroup.LANE_SORT = cbLeftSortKind.SelectedIndex + 1;
            laneGroup.CSN_NO = 0;
            laneGroup.LANE_COUNT = lvLeftLane.Items.Count;

            SaveLaneGroup(laneGroup);

            //laneGroup.DIRECTION = (int)MOVE_DIRECTION.TO_LEFT; // 1: TO Left  2: TO Right

        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRightLaneGroupName.Text))
            {
                MessageBox.Show("행선지명을 입력하세요", "입력오류");
                return;
            }

            if (cbRightSortKind.SelectedIndex < 0)
            {
                MessageBox.Show("정렬 방식을 선택하세요","입력오류");
                return;
            }

            LANE_GROUP laneGroup = laneGroupList.Where(x => x.DIRECTION == (int)MOVE_DIRECTION.TO_RIGHT).FirstOrDefault();
            laneGroup.LANE_GROUP_NAME = txtRightLaneGroupName.Text;
            laneGroup.LANE_SORT = cbRightSortKind.SelectedIndex + 1;
            laneGroup.CSN_NO = 0;
            laneGroup.LANE_COUNT = lvRightLane.Items.Count;
            SaveLaneGroup(laneGroup);
        }

        private void GetLaneGroupList()
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            laneGroupList = db.GetLaneGroupList(new LANE_GROUP()
            {

            }, out SP_RESULT spResult).ToList();

        }
        private void GetLaneInfoList(List<LANE_GROUP> groupList)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            foreach (var group in groupList)
            {
                group.laneInfoList = db.GetLaneInfoList(new LANE_INFO()
                {
                     LANE_GROUP_ID = group.ID

                }, out SP_RESULT spResult).ToList();
            }
        }

        private void SaveLaneGroup(LANE_GROUP laneGroup)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            db.UpdateLaneGroup(laneGroup, out SP_RESULT spResult);
            if(spResult!=null)
            {
                if(spResult.RESULT_CODE.CompareTo("100")==0)
                {
                    MessageBox.Show("저장에 성공하였습니다", "저장");
                }
                else
                {
                    MessageBox.Show(spResult.ERROR_MESSAGE, "오류");
                }

            }

        }

        private void darkButton7_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void darkButton10_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void SetLaneInfo()
        {
            foreach(var laneGroup in laneGroupList)
            {
                SetLaneGroup(laneGroup);
                SetLaneInfo(laneGroup);
            }
        }

        private void SetLaneGroup(LANE_GROUP laneGroup)
        {
            switch(laneGroup.DIRECTION)
            {
                case (int)MOVE_DIRECTION.TO_LEFT:
                    txtLeftLaneGroupName.Text = laneGroup.LANE_GROUP_NAME;
                    cbLeftSortKind.SelectedIndex = laneGroup.LANE_SORT - 1;
                    break;
                case (int)MOVE_DIRECTION.TO_RIGHT:
                    txtRightLaneGroupName.Text = laneGroup.LANE_GROUP_NAME;
                    cbRightSortKind.SelectedIndex = laneGroup.LANE_SORT - 1;
                    break;
            }
        }

        private void SetLaneInfo(LANE_GROUP laneGroup)
        {

            foreach(var lane in laneGroup.laneInfoList)
            {
                AddLaneInfoToListView(lane);
            }

        }

        private void darkButton1_Click(object sender, EventArgs e)
        {

        }

        private void darkButton1_Click_1(object sender, EventArgs e)
        {
            int direction = int.Parse((sender as DarkButton).Tag.ToString());
            AddLaneInfoForm addLaneInfoForm = new AddLaneInfoForm();
            if(addLaneInfoForm.ShowDialog() == DialogResult.OK)
            {
               LANE_GROUP laneGroup = laneGroupList.Where(x => x.DIRECTION == direction).FirstOrDefault();
               if(laneGroup!=null)
                {
                    LANE_INFO laneInfo = new LANE_INFO();
                    laneInfo.DIRECTION = laneGroup.DIRECTION;
                    laneInfo.LANE_GROUP_ID = laneGroup.ID;
                    laneInfo.LANE_NAME = addLaneInfoForm.laneName;
                    laneInfo.LANE = addLaneInfoForm.lane;
                    if(AddLaneInfo(laneInfo))
                    {
                        laneGroup.laneInfoList.Add(laneInfo);
                        AddLaneInfoToListView(laneInfo);
                    }
                }
            }
        }

        private bool AddLaneInfo(LANE_INFO laneInfo)
        {
            bool result = false;
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            db.AddLaneInfo(laneInfo, out SP_RESULT spResult);
            if (spResult != null)
            {
                if (spResult.RESULT_CODE.CompareTo("100") == 0)
                {
                    MessageBox.Show("저장에 성공하였습니다", "저장");
                    result = true;
                }
                else
                {
                    MessageBox.Show(spResult.ERROR_MESSAGE, "오류");
                }
            }
            return result;
        }

        private bool UpdateLaneInfo(LANE_INFO laneInfo)
        {
            bool result = false;
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            db.UpdateLaneInfo(laneInfo, out SP_RESULT spResult);
            if (spResult != null)
            {
                if (spResult.RESULT_CODE.CompareTo("100") == 0)
                {
                    MessageBox.Show("저장에 성공하였습니다", "저장");
                    result = true;
                }
                else
                {
                    MessageBox.Show(spResult.ERROR_MESSAGE, "오류");
                }
            }
            return result;
        }


        public void AddLaneInfoToListView(LANE_INFO laneInfo)
        {
            ListView list = null;
            ListViewItem item;
            switch (laneInfo.DIRECTION)
            {
                case (int)MOVE_DIRECTION.TO_LEFT:
                    list = lvLeftLane;
                    break;
                case (int)MOVE_DIRECTION.TO_RIGHT:
                    list = lvRightLane;
                    break;
            }

            item = new ListViewItem(laneInfo.LANE_NAME); // 
            item.SubItems.Add(laneInfo.LANE.ToString());
            item.SubItems.Add(laneInfo.DIRECTION == (int)MOVE_DIRECTION.TO_LEFT ? "왼쪽으로" : "오른쪽으로");
            item.Tag = laneInfo;
            list.Items.Add(item);

        }

        

        private void darkButton2_Click(object sender, EventArgs e)
        {
            int direction = int.Parse((sender as DarkButton).Tag.ToString());
            ListView list = null;
            switch (direction)
            {
                case (int)MOVE_DIRECTION.TO_LEFT:
                    list = lvLeftLane;
                    break;
                case (int)MOVE_DIRECTION.TO_RIGHT:
                    list = lvRightLane;
                    break;
            }

            if(list!=null)
            {
                var items = list.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    LANE_INFO laneInfo = (LANE_INFO)item.Tag;
                    if (laneInfo != null)
                    {
                        AddLaneInfoForm addLaneInfoForm = new AddLaneInfoForm();
                        addLaneInfoForm.SetLaneInfo(laneInfo);
                        if (addLaneInfoForm.ShowDialog() == DialogResult.OK)
                        {
                            laneInfo.LANE_NAME = addLaneInfoForm.laneName;
                            laneInfo.LANE = addLaneInfoForm.lane;
                            if (UpdateLaneInfo(laneInfo))
                            {
                                UpdateLaneInfoToListView(item,laneInfo);
                            }
                        }
                    }
                }
            }

        }

        public void UpdateLaneInfoToListView(ListViewItem item ,LANE_INFO laneInfo)
        {
            item.SubItems[0].Text = laneInfo.LANE_NAME;
            item.SubItems[1].Text = laneInfo.LANE.ToString();
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            int direction = int.Parse((sender as DarkButton).Tag.ToString());
            ListView list = null;
            switch (direction)
            {
                case (int)MOVE_DIRECTION.TO_LEFT:
                    list = lvLeftLane;
                    break;
                case (int)MOVE_DIRECTION.TO_RIGHT:
                    list = lvRightLane;
                    break;
            }


            if (list.SelectedIndices.Count > 0 && MessageBox.Show("삭제하시겠습니까?", "삭제", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var items = list.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    LANE_INFO laneInfo = (LANE_INFO)item.Tag;
                    if (laneInfo != null)
                    {
                        TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
                        db.DeleteLaneInfo(laneInfo, out SP_RESULT spResult);

                        if (spResult != null)
                        {
                            if (spResult.RESULT_CODE.CompareTo("100") == 0)
                            {
                                MessageBox.Show("삭제에 성공하였습니다", "삭제");
                                list.Items.Remove(item);
                            }
                            else
                            {
                                MessageBox.Show(spResult.ERROR_MESSAGE, "오류");
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("삭제할 차선을 선택하세요", "오류", MessageBoxButtons.OK);
                    }
                }
            }
        }
    }
}
