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
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;

namespace VDSWebAPIServer.Forms
{
    public partial class UserManageForm : DarkForm
    {

        USER_INFO searchUser = new USER_INFO();


        int currentPage = 1;
        int totalPage = 1;

        String operatonMode = String.Empty;


        public UserManageForm()
        {
            InitializeComponent();
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            lvUserInfo.Items.Clear();
            searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
        }

        private void searchUserInfo(int page, int pageSize)
        {
            if (rdgApprove_1.Checked)
                searchUser.APPROVE_YN = "Y";
            else if (rdgApprove_2.Checked)
                searchUser.APPROVE_YN = "N";
            else
                searchUser.APPROVE_YN = null;


            if (rdgUserType_1.Checked)
                searchUser.USER_TYPE = 1;
            else if (rdgUserType_2.Checked)
                searchUser.USER_TYPE = 2;
            else
                searchUser.USER_TYPE = 0;


            searchUser.USER_ID = txtUserId.Text;
            searchUser.USER_NAME = txtUserName.Text;

            //페이징 정보 리셋
            searchUser.PAGE_SIZE = pageSize;
            searchUser.CURRENT_PAGE = page;
            //페이징 정보 리셋

            UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);


            var result = userDB.GetUserInfoList(searchUser, out SP_RESULT spResult).ToList();

            if (spResult.RESULT_CODE.CompareTo("500") == 0)
            {
                //MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                Utility.ShowMessageBox("오류", spResult.ERROR_MESSAGE, 1);
                return;
            }
            else
            {
                if (spResult.RESULT_COUNT == 0)
                {
                    //MessageBox.Show("조건에 맞는 데이터가 없습니다", "정보", MessageBoxButtons.OK);
                    Utility.ShowMessageBox("조회결과", "조건에 맞는 데이터가 없습니다", 1);
                    return;
                }
                else
                {
                    totalPage = ApiUtility.GetTotalPageCount(spResult.RESULT_COUNT, GlobalCommonData.PAGE_SIZE);

                    lbPageInfo.Text = String.Format($"{page}/{totalPage}");
                    SetUserInfoToListView(result);
                }

            }

        }

        private void SetUserInfoToListView(List<USER_INFO> userList)
        {
            lvUserInfo.Items.Clear();

            foreach (var user in userList)
            {
                AddUserInfoToListView(lvUserInfo, user);
            }
        }

        private void AddUserInfoToListView(ListView lvView, USER_INFO userInfo)
        {
            ListViewItem item;
            // 아이디, 이름, 유형, 이메일, 휴대전화 , 승인여부, 등록일
            item = new ListViewItem(userInfo.USER_ID); // 아이디
            item.SubItems.Add(userInfo.USER_NAME); // 이름
            switch (userInfo.USER_TYPE)
            {
                case 0:
                case 1:
                    item.SubItems.Add("일반"); // 유형
                    break;
                case 2:
                    item.SubItems.Add("관리자"); // 유형
                    break;
            }
            item.SubItems.Add(userInfo.DEPT_NAME); // 소속부서
            item.SubItems.Add(userInfo.APPROVE_YN); // 승인여부
            item.SubItems.Add(userInfo.REG_DATE.ToString("yyyy-MM-dd HH:mm:ss")); // 휴대전화

            item.Tag = userInfo;

            lvView.Items.Add(item);

        }

        private void darkButton8_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;


        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            ResetForm();
            operatonMode = "ADD";
        }

        private void ResetForm()
        {
            rdgUser_TYPE_1.Checked = false;
            rdgUser_TYPE_2.Checked = false;
            txtUSER_ID.Text = String.Empty;
            txtPASSWD.Text = String.Empty;
            txtUSER_NAME.Text = String.Empty;


            txtDEPT_NAME.Text = String.Empty;

         
            rdgAPPROVE_YN_Y.Checked = false;
            rdgAPPROVE_YN_N.Checked = false;


        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            USER_INFO userInfo = CheckForm();
            bool bResult = false;

            if (userInfo != null)
            {
                switch (operatonMode)
                {
                    case "ADD":
                        bResult = AddUserInfo(userInfo);
                        break;
                    case "UPDATE":
                        bResult = UpdateUserInfo(userInfo);
                        break;
                }
            }
        }

        private USER_INFO CheckForm()
        {
            USER_INFO result = new USER_INFO();
            if (!rdgUser_TYPE_1.Checked && !rdgUser_TYPE_2.Checked)
            {
                // MessageBox.Show("사용자 유형을 선택하세요");
                Utility.ShowMessageBox("입력", "사용자 유형을 선택하세요", 1);
                return null;
            }

            if (rdgUser_TYPE_1.Checked)
                result.USER_TYPE = 1;
            else if (rdgUser_TYPE_2.Checked)
                result.USER_TYPE = 2;


            if (String.IsNullOrEmpty(txtUSER_ID.Text))
            {
                //MessageBox.Show("아이디를 입력하세요");
                Utility.ShowMessageBox("입력", "아이디를 입력하세요", 1);
                return null;
            }

            result.USER_ID = txtUSER_ID.Text;

            if (String.IsNullOrEmpty(txtPASSWD.Text))
            {
                //MessageBox.Show("비밀번호를 입력하세요");
                Utility.ShowMessageBox("입력", "비밀번호를 입력하세요", 1);
                return null;
            }
            result.PASSWD = txtPASSWD.Text;


            if (String.IsNullOrEmpty(txtUSER_NAME.Text))
            {
                //MessageBox.Show("이름을 입력하세요");
                Utility.ShowMessageBox("입력", "이름을 입력하세요", 1);
                return null;
            }
            result.USER_NAME = txtUSER_NAME.Text;

            result.DEPT_NAME = txtDEPT_NAME.Text;

            result.APPROVE_YN = rdgAPPROVE_YN_Y.Checked ? "Y" : "N";

            return result;
        }


        private bool AddUserInfo(USER_INFO userInfo)
        {
            bool result = true;

            UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);

            userDB.AddUserInfo(ref userInfo, out SP_RESULT spResult);

            if (!spResult.IS_SUCCESS)
            {
                if (spResult.RESULT_CODE.CompareTo("500") == 0)
                {
                    MessageBox.Show(spResult.ERROR_MESSAGE);
                }

                result = false;
            }
            else
                searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
            return result;

        }

        private bool UpdateUserInfo(USER_INFO userInfo)
        {
            bool result = true;
            UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);
            userDB.UpdateUserInfo(userInfo, out SP_RESULT spResult);

            if (!spResult.IS_SUCCESS)
            {
                if (spResult.RESULT_CODE.CompareTo("100") == 0 && spResult.RESULT_COUNT == 0)
                {
                    //MessageBox.Show("회원 정보가 존재하지 않습니다");
                    Utility.ShowMessageBox("정보", "회원 정보가 존재하지 않습니다", 1);
                }
                else if (spResult.RESULT_CODE.CompareTo("500") == 0)
                {
                    //MessageBox.Show(spResult.ERROR_MESSAGE);
                    Utility.ShowMessageBox("오류", spResult.ERROR_MESSAGE, 1);
                }

                result = false;
            }
            else
                searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);

            return result;

        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            if (lvUserInfo.SelectedIndices.Count > 0 && Utility.ShowMessageBox("삭제", "삭제하시겠습니까?", 2) == DialogResult.OK)
            {
                var items = lvUserInfo.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    //Console.WriteLine($"선택한 아이템 :{item.Text}");
                    USER_INFO userInfo = (USER_INFO)item.Tag;
                    if (userInfo != null)
                    {
                        UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);
                        userDB.DeleteUserInfo(new USER_INFO()
                        {
                            USER_ID = userInfo.USER_ID
                        }, out SP_RESULT spResult);

                        if (spResult.RESULT_CODE.CompareTo("500") == 0)
                        {
                            //MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                            Utility.ShowMessageBox("오류", spResult.ERROR_MESSAGE, 1);
                            return;
                        }
                        else
                        {
                            searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
                        }

                    }
                }

            }
        }

        private void darkButton7_Click(object sender, EventArgs e)
        {
            String caption = String.Empty;
            if (rdgAPPROVE_YN_Y.Checked)
                caption = "승인처리하시겠습니까?";
            else if (rdgAPPROVE_YN_N.Checked)
                caption = "미승인처리하시겠습니까?";
            else
            {
                Utility.ShowMessageBox("선택", "승인여부를 선택하세요", 1);
                return;
            }
            if (lvUserInfo.SelectedIndices.Count > 0 && Utility.ShowMessageBox("승인", caption, 2) == DialogResult.OK)
            {
                var items = lvUserInfo.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    //Console.WriteLine($"선택한 아이템 :{item.Text}");
                    USER_INFO userInfo = (USER_INFO)item.Tag;
                    if (userInfo != null)
                    {
                        UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);
                        userDB.ApproveUserInfo(new USER_INFO()
                        {
                            USER_ID = userInfo.USER_ID,
                            APPROVE_YN = rdgAPPROVE_YN_Y.Checked ? "Y" : "N",
                        }, out SP_RESULT spResult);

                        if (spResult.RESULT_CODE.CompareTo("500") == 0)
                        {
                            Utility.ShowMessageBox("오류", spResult.ERROR_MESSAGE, 1);
                            return;
                        }
                        else
                        {
                            searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
                        }

                    }
                }

            }
        }

        private void lvUserInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = lvUserInfo.SelectedItems;
            foreach (ListViewItem item in items)
            {
                //Console.WriteLine($"선택한 아이템 :{item.Text}");
                USER_INFO userInfo = (USER_INFO)item.Tag;
                if (userInfo != null)
                {
                    operatonMode = "UPDATE";
                    var user = GetUserInfo(userInfo);
                    if (user != null)
                    {
                        SetUserInfo(user);
                    }

                }
            }
        }

        private USER_INFO GetUserInfo(USER_INFO data)
        {
            USER_INFO result = null;
            UserInfoOperation userDB = new UserInfoOperation(VDSConfig.MA_DB_CONN);
            result = userDB.GetUserInfo(data, out SP_RESULT spResult);
            if (!spResult.IS_SUCCESS)
            {
                if (spResult.RESULT_CODE.CompareTo("100") == 0 && spResult.RESULT_COUNT == 0)
                {
                    //MessageBox.Show("회원 정보가 존재하지 않습니다");
                    Utility.ShowMessageBox("정보", "회원 정보가 존재하지 않습니다", 1);
                }
                else if (spResult.RESULT_CODE.CompareTo("500") == 0)
                {
                    Utility.ShowMessageBox("오류", spResult.ERROR_MESSAGE, 1);
                }
                result = null;
            }
            return result;
        }

        private void SetUserInfo(USER_INFO userInfo)
        {
            ResetForm();

            switch (userInfo.USER_TYPE)
            {
                case 0:
                case 1:
                    rdgUser_TYPE_1.Checked = true;
                    break;
                case 2:
                    rdgUser_TYPE_2.Checked = true;
                    break;

            }

            txtUSER_ID.Text = userInfo.USER_ID;
            txtPASSWD.Text = userInfo.PASSWD;
            txtUSER_NAME.Text = userInfo.USER_NAME;

            txtDEPT_NAME.Text = userInfo.DEPT_NAME;

            switch (userInfo.APPROVE_YN)
            {
                case "Y":
                    rdgAPPROVE_YN_Y.Checked = true;
                    break;
                case "N":
                    rdgAPPROVE_YN_N.Checked = true;
                    break;
            }


        }

        private void darkButton6_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
                currentPage--;
            else
            {
                //MessageBox.Show("첫번째 페이지입니다", "정보", MessageBoxButtons.OK);
                Utility.ShowMessageBox("정보", "첫번째 페이지입니다", 1);
                return;
            }

            searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPage)
                currentPage++;
            else
            {
                //MessageBox.Show("마지막 페이지입니다", "정보", MessageBoxButtons.OK);
                Utility.ShowMessageBox("정보", "마지막 페이지입니다", 1);
                return;
            }
            searchUserInfo(currentPage, GlobalCommonData.PAGE_SIZE);
        }
    }
}
