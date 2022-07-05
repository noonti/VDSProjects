using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;
using VDSCommon;
using VDSCommon.API.Model;

namespace VDSWebAPIServer
{
    public partial class ucTreeView : UserControl
    {

        ChangeVDSGroupsEventDelegate _changeVDSGrupHandler = null;
        CommonOperation commonOp = new CommonOperation(AdminConfig.GetDBConn());
        public ucTreeView()
        {
            InitializeComponent();
            SP_RESULT spResult;
            var vdsGroupList = commonOp.GetVDSGroupsList(new VDS_GROUPS() {
                PARENT_ID = 0,
                USE_YN = "Y"
            } , out spResult).ToList();

            AddVDSGroupsListToTreeView(vdsGroupList);
        }

        public void SetChangeVDSGroupsEventDelegate(ChangeVDSGroupsEventDelegate changeVDSGroupHandler)
        {
            _changeVDSGrupHandler = changeVDSGroupHandler;
        }

        public void AddVDSGroupsListToTreeView(List<VDS_GROUPS> vdsGroupList)
        {
            foreach(var vdsGroups in vdsGroupList)
            {
                AddVDSGroupsToTree(vdsGroups);
            }
            tvVDSGroups.ExpandAll();
        }

        private void AddVDSGroupsToTree(VDS_GROUPS vdsGroups)
        {
            TreeNode childNode = new TreeNode(vdsGroups.TITLE);
            childNode.Tag = vdsGroups;
            TreeNode parentNode = FindParentNode(tvVDSGroups.Nodes, vdsGroups);
            if(parentNode==null)
            {
                tvVDSGroups.Nodes.Add(childNode);
            }
            else
            {
                parentNode.Nodes.Add(childNode);
            }

        }

        private TreeNode FindParentNode(TreeNodeCollection nodes, VDS_GROUPS vdsGroups)
        {
            TreeNode result = null;
            VDS_GROUPS childGroup;
            foreach(TreeNode child in nodes)
            {
                childGroup = (VDS_GROUPS)child.Tag;
                if (childGroup.ID == vdsGroups.PARENT_ID)
                {
                    result = child;
                    return result;
                }
                else
                    result = FindParentNode(child.Nodes, vdsGroups);
            }
            return result;

        }

        private void tvVDSGroups_Click(object sender, EventArgs e)
        {
            //GetSelectedVDSGroup();
        }

        public VDS_GROUPS GetSelectedVDSGroup()
        {
            VDS_GROUPS vdsGroup = null;
            if (tvVDSGroups.SelectedNode != null)
            {
                vdsGroup = (VDS_GROUPS)tvVDSGroups.SelectedNode.Tag;
                if (vdsGroup != null)
                {
                    Console.WriteLine($"GetSelectedVDSGroup = {vdsGroup.TITLE}");
                    _changeVDSGrupHandler?.Invoke(vdsGroup);
                }
            }
            return vdsGroup;
        }

        private void tvVDSGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GetSelectedVDSGroup();
        }
    }
}
