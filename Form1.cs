using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CyUSB;
using System.Runtime.InteropServices;

namespace USBcontrol_v1
{
    public partial class Form1 : Form
    {
        CyUSBDevice loopDevice = null;
        USBDeviceList usbDevices = null;
        CyBulkEndPoint inEndpoint = null;
        CyBulkEndPoint outEndpoint = null;

        bool bRunning = false;
        bool bSending = false;

        int value;
        long outCount, inCount;

        delegate void UpdateUICallback();
        UpdateUICallback updateUI;

        public Form1()
        {
            InitializeComponent();

            Btn_SendCmd.Enabled = Btn_StrReceiveData.Enabled = false;

            updateUI = new UpdateUICallback(StatusUpdate);

            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);

            usbDevices.DeviceAttached += new EventHandler(usbDevices_Attached);
            usbDevices.DeviceRemoved += new EventHandler(usbDevices_Removed);

            SetDevice();
        }

        private void usbDevices_Attached(object sender,EventArgs e)
        {
            SetDevice();
        }

        private void usbDevices_Removed(object sender, EventArgs e)
        {
            SetDevice();
        }

        private void StatusUpdate()
        {

        }

        private void SetDevice()
        {
            int nCurSelection = 0;
            if(Cmb_DeviceConnect.Items.Count>0)
            {
                nCurSelection = Cmb_DeviceConnect.SelectedIndex;
                Cmb_DeviceConnect.Items.Clear();
                Cmb_INEndpoint.Items.Clear();
                Cmb_OutEndPoint.Items.Clear();

                outCount = inCount = 0;

                BytesOutLabel.Text = outCount.ToString();
                BytesInLabel.Text = inCount.ToString();
            }
            int DeviceCnt = usbDevices.Count;

            for(int nCount =0; nCount < DeviceCnt; nCount++)
            {
                USBDevice fxDevice = usbDevices[nCount];
                String str = null;
                str = "(0x" + fxDevice.VendorID.ToString("X4") + " - 0x" + fxDevice.ProductID.ToString("X4") + ") " + fxDevice.FriendlyName;
                Cmb_DeviceConnect.Items.Add(str);
            }

            if (Cmb_DeviceConnect.Items.Count > 0)
                Cmb_DeviceConnect.SelectedIndex = nCurSelection;

            loopDevice = usbDevices[Cmb_DeviceConnect.SelectedIndex] as CyUSBDevice;

            //Btn_SendCmd.Enabled = (loopDevice != null);
            if(loopDevice != null)
            {
                Btn_SendCmd.Enabled = true;
                Text = loopDevice.FriendlyName;
                GetEndpointsOfNode(loopDevice.Tree);
            }
            else
                Text = "C# Bulkloop - no device";
            if (Cmb_INEndpoint.Items.Count > 0) Cmb_INEndpoint.SelectedIndex = 0;
            if (Cmb_OutEndPoint.Items.Count > 0) Cmb_OutEndPoint.SelectedIndex = 0;

            ConstructEndpoints();
        }

        private void GetEndpointsOfNode(TreeNode devTree)
        {
            Cmb_INEndpoint.Items.Clear();
            Cmb_OutEndPoint.Items.Clear();

            foreach (TreeNode node in devTree.Nodes)
            {
                if(node.Nodes.Count>0)
                {
                    GetEndpointsOfNode(node);
                }
                else
                {
                    CyBulkEndPoint ept = node.Tag as CyBulkEndPoint;
                    if (ept == null)
                    {
                        return;
                    }
                    else if (node.Text.Contains("Bulk in"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        Cmb_INEndpoint.Items.Add(s);
                    }
                    else if (node.Text.Contains("Bulk out"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        Cmb_OutEndPoint.Items.Add(s);
                    }
                }
            }
        }

        private void ConstructEndpoints()
        {
            if (loopDevice != null && Cmb_INEndpoint.Items.Count>0 && Cmb_OutEndPoint.Items.Count>0)
            {
                string sAltOut = Cmb_OutEndPoint.Text.Substring(4, 1); //CyUSBInterface.bAlternateSetting
                byte outAltInterface = Convert.ToByte(sAltOut);

                string sAltIn = Cmb_INEndpoint.Text.Substring(4, 1); //CyUSBInterface.bAlternateSetting
                byte inAltInterface = Convert.ToByte(sAltIn);

                if(inAltInterface != outAltInterface)
                {
                    Text = "Output Endpoint and Input Endpoint should present in the same ALT interface";
                    Btn_SendCmd.Enabled = false;
                    return;
                }

                int aX = Cmb_INEndpoint.Text.LastIndexOf("0x");
                string sAddr = Cmb_INEndpoint.Text.Substring(aX, 4);
                byte addrIn = (byte)Util.HexToInt(sAddr);

                aX = Cmb_OutEndPoint.Text.LastIndexOf("0x");
                sAddr = Cmb_OutEndPoint.Text.Substring(aX, 4);
                byte addrOut = (byte)Util.HexToInt(sAddr);

                outEndpoint = loopDevice.EndPointOf(addrOut) as CyBulkEndPoint;
                inEndpoint = loopDevice.EndPointOf(addrIn) as CyBulkEndPoint;

                if ((outEndpoint != null) && (inEndpoint != null))
                {
                    if ((inEndpoint.Attributes & 0x03) != 0x02 || (outEndpoint.Attributes & 0x03/*0,1 bit for type of transfer*/) != 0x02/*Bulk endpoint*/)
                    {
                        Text = "Device Configuration mismatch";
                        Btn_SendCmd.Enabled = false;
                        return;
                    }
                    outEndpoint.TimeOut = 1000;
                    inEndpoint.TimeOut = 1000;
                }
                else
                {
                    Text = "Device Configuration mismatch";
                    Btn_SendCmd.Enabled = false;
                    return;
                }
            }
        }

        private void Cmb_DeviceConnect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Btn_SendCmd_Click(object sender, EventArgs e)
        {
            int outlen = 128, value;
            bool bResult;
            byte[] Startbuffer = new byte[outlen];
            byte[] Stopbuffer = new byte[outlen];
            // send AAAA
            if (!bSending)
            {
                bRunning = bSending = Btn_SendCmd.Enabled = false;
                value = 170;
                SetoutputDatat(ref Startbuffer,ref outlen, ref value);
                bResult = outEndpoint.XferData(ref Startbuffer,ref outlen);
                if(bResult)
                {
                    TBox_ShowReceiveData.Text = "Output XferData Success! \n\r";
                }
                bResult = false;
                Btn_StrReceiveData.Enabled = true;
            }
            else
            {
                bRunning = false;
                bSending = true;
                value = 187;
                SetoutputDatat(ref Stopbuffer, ref outlen, ref value);
                bResult = outEndpoint.XferData(ref Stopbuffer, ref outlen);
                if (bResult)
                {
                    TBox_ShowReceiveData.Text = "Output XferData Success! \n\r";
                }
                bResult = false;
            }
        }

        private void SetoutputDatat(ref byte[] outputbuffer ,ref int len,ref int FixedData)
        {
            for (int i = 0; i < outputbuffer.Length; i++)
            {
                outputbuffer[i] = (byte)FixedData; // set each element to the value you want
            }
        }

        private void Btn_StrReceiveData_Click(object sender, EventArgs e)
        {
            // start & stop 2 mode

        }

        private void Cmb_OutEndPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConstructEndpoints();
        }

        private void Cmb_INEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConstructEndpoints();
        }
    }

    
}
