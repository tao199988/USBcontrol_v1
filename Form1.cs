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
        long outCount = 0, inCount = 0;

        const int XFERSIZE = 256;
        byte[] inData = new byte[XFERSIZE];


        Queue<byte[]> Inbuffer = new Queue<byte[]>();
        int QueueSz = 64;
        int Bufsz;
        static byte DefaultBufInitValue = 0xA5;

        Thread tXfers;

        delegate void UpdateUICallback();
        UpdateUICallback updateUI;

        delegate void ExceptionCallback();
        ExceptionCallback handleException;
        string Datashow = null;
        public Form1()
        {
            InitializeComponent();

            Btn_SendCmd.Enabled = Btn_StrReceiveData.Enabled = false;

            updateUI = new UpdateUICallback(StatusUpdate);

            // Setup the callback routine for NullReference exception handling
            handleException = new ExceptionCallback(ThreadException);

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
            
            if(bRunning == false) return;
            for (; Inbuffer.Count > 0;)
            {
                Console.WriteLine(Inbuffer.Count);
                if (Inbuffer.Count > 0)
                {
                    //string Datashow = BitConverter.ToString(Inbuffer.Dequeue());
                    Datashow += BitConverter.ToString(Inbuffer.Dequeue());
                    //TBox_ShowReceiveData.AppendText(Datashow + Environment.NewLine);
                }
            }
            TBox_ShowReceiveData.AppendText(Datashow + Environment.NewLine);
            TBox_ShowReceiveData.AppendText(Environment.NewLine);
            Datashow = null;
        }

        private void SetDevice()
        {
            int nCurSelection = 0;
            if (cboDeviceConnected.Items.Count > 0)
            {
                nCurSelection = cboDeviceConnected.SelectedIndex;
                cboDeviceConnected.Items.Clear();
                cboINEndpoint.Items.Clear();
                cboOutEndPoint.Items.Clear();
                outCount = 0;
                inCount = 0;

            }
            int nDeviceList = usbDevices.Count;
            for (int nCount = 0; nCount < nDeviceList; nCount++)
            {
                USBDevice fxDevice = usbDevices[nCount];
                String strmsg;
                strmsg = "(0x" + fxDevice.VendorID.ToString("X4") + " - 0x" + fxDevice.ProductID.ToString("X4") + ") " + fxDevice.FriendlyName;
                cboDeviceConnected.Items.Add(strmsg);
            }

            if (cboDeviceConnected.Items.Count > 0)
                cboDeviceConnected.SelectedIndex = nCurSelection;

            loopDevice = usbDevices[cboDeviceConnected.SelectedIndex] as CyUSBDevice;

            Btn_SendCmd.Enabled = (loopDevice != null);

            if (loopDevice != null)
                Text = loopDevice.FriendlyName;
            else
                Text = "C# Bulkloop - no device";

            if (loopDevice != null) GetEndpointsOfNode(loopDevice.Tree);
            if (cboINEndpoint.Items.Count > 0) cboINEndpoint.SelectedIndex = 0;
            if (cboOutEndPoint.Items.Count > 0) cboOutEndPoint.SelectedIndex = 0;
            // Set the IN and OUT endpoints per the selected radio buttons.
            ConstructEndpoints();
        }

        private void GetEndpointsOfNode(TreeNode devTree)
        {
            cboINEndpoint.Items.Clear();
            cboOutEndPoint.Items.Clear();

            foreach (TreeNode node in devTree.Nodes)
            {
                if (node.Nodes.Count > 0)
                    GetEndpointsOfNode(node);
                else
                {
                    CyUSBEndPoint ept = node.Tag as CyUSBEndPoint;
                    if (ept == null)
                    {
                        //return;
                    }
                    else if (node.Text.Contains("Bulk in"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        cboINEndpoint.Items.Add(s);
                    }
                    else if (node.Text.Contains("Bulk out"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        cboOutEndPoint.Items.Add(s);
                    }

                }
            }
        }

        private void ConstructEndpoints()
        {
            if (loopDevice != null && cboOutEndPoint.Items.Count > 0 && cboINEndpoint.Items.Count > 0)
            {

                string sAltOut = cboOutEndPoint.Text.Substring(4, 1);
                byte outAltInferface = Convert.ToByte(sAltOut);

                string sAltIn = cboINEndpoint.Text.Substring(4, 1);
                byte inAltInferface = Convert.ToByte(sAltIn);

                if (outAltInferface != inAltInferface)
                {
                    Text = "Output Endpoint and Input Endpoint should present in the same ALT interface";
                    Btn_SendCmd.Enabled = false;
                    return;
                }

                // Get the endpoint
                int aX = cboINEndpoint.Text.LastIndexOf("0x");
                string sAddr = cboINEndpoint.Text.Substring(aX, 4);
                byte addrIn = (byte)Util.HexToInt(sAddr);

                aX = cboOutEndPoint.Text.LastIndexOf("0x");
                sAddr = cboOutEndPoint.Text.Substring(aX, 4);
                byte addrOut = (byte)Util.HexToInt(sAddr);

                outEndpoint = loopDevice.EndPointOf(addrOut) as CyBulkEndPoint;
                inEndpoint = loopDevice.EndPointOf(addrIn) as CyBulkEndPoint;

                if ((outEndpoint != null) && (inEndpoint != null))
                {
                    //make sure that the device configuration doesn't contain the other than bulk endpoint
                    if ((outEndpoint.Attributes & 0x03/*0,1 bit for type of transfer*/) != 0x02/*Bulk endpoint*/)
                    {
                        Text = "Device Configuration mismatch";
                        Btn_SendCmd.Enabled = false;
                        return;

                    }
                    if ((inEndpoint.Attributes & 0x03) != 0x02)
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

 
        private void Btn_SendCmd_Click(object sender, EventArgs e)
        {
            int outlen = 1024, value;
            bool bResult = false;
            byte[] Startbuffer = new byte[outlen];
            byte[] Stopbuffer = new byte[outlen];
            Bufsz = inEndpoint.MaxPktSize * 2;
            // send AAAA
            if (!bSending)
            {
                bRunning = false;
                bSending = true;
               value = 170;
                SetoutputDatat(ref Startbuffer,ref outlen, ref value);
                bResult = outEndpoint.XferData(ref Startbuffer,ref outlen);
                if(bResult)
                {
                    outCount++;
                    TBox_ShowReceiveData.AppendText ("Output XferData Success!" + "AA" + Environment.NewLine);
                }
                bResult = false;
                Btn_StrReceiveData.Enabled = true;
            }
            else
            {
                bRunning = false;
                bSending = false;
                value = 187;
                SetoutputDatat(ref Stopbuffer, ref outlen, ref value);
                bResult = outEndpoint.XferData(ref Stopbuffer, ref outlen);
                if (bResult)
                {
                    outCount++;
                    TBox_ShowReceiveData.AppendText("Output XferData Success!" + "BB" + Environment.NewLine);
                }
                bResult = false;
                Btn_StrReceiveData.Enabled = false;
            }
        }

        //
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
            if(!bRunning)
            {
                bRunning = true;
                
                inCount = 0;
                Btn_SendCmd.Enabled = false;
                Btn_StrReceiveData.Text = "Stop";

                //Create new thread
                tXfers = new Thread(new ThreadStart(TransferThread));
                tXfers.IsBackground = true;
                tXfers.Priority = ThreadPriority.Highest;

                //Starts the new thread
                tXfers.Start();

            }
            else
            {
                //reset button state
                bRunning = false;
                Btn_SendCmd.Enabled = true;
                Btn_StrReceiveData.Text = "Start";

                if (tXfers == null) return;
                //close thread
                if (tXfers.IsAlive)
                {
                    tXfers.Abort();
                    tXfers.Join();
                    tXfers = null;
                }
            }
        }

        private unsafe void TransferThread()
        {
            #region test
            /*int xferLen = XFERSIZE;

            uint timeOut = inEndpoint.TimeOut;
            int len = inEndpoint.MaxPktSize * 2;
           
            byte[] inbuffer = new byte[len];
            byte[] cBufs = new byte[Math.Max(CyConst.OverlapSignalAllocSize, sizeof(OVERLAPPED)) + ((inEndpoint.XferMode == XMODE.BUFFERED) ? len : 0)];
            byte[] inOvLap = new byte[Math.Max(CyConst.OverlapSignalAllocSize, sizeof(OVERLAPPED))];

            CyUSB.OVERLAPPED ovLapStatus = new CyUSB.OVERLAPPED();
            ovLapStatus.hEvent = (IntPtr)PInvoke.CreateEvent(0, 0, 0, 0);

            fixed(byte* bufferPinned = inbuffer, cBuffPinned = cBufs, ovLap = &inOvLap[0])
            {
                Marshal.StructureToPtr(ovLapStatus,(IntPtr)ovLap,true);

                for(; bRunning;)
                {
                    Array.Clear(inbuffer, 0, len);
                    if(inEndpoint.BeginDataXfer(ref cBufs,ref inbuffer,ref len,ref inOvLap) == false)
                    {
                        if(inEndpoint.UsbdStatus == 0xC0000030)
                        {
                            inEndpoint.Reset();
                            if(inEndpoint.BeginDataXfer(ref cBufs, ref inbuffer, ref len, ref inOvLap) == false)
                            {
                                Console.WriteLine("Recv error");
                                Thread.Sleep(10);
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Recv error");
                            Thread.Sleep(10);
                            continue;
                        }    
                    }

                }
            }*/
            #endregion

            byte[][] cmdBufs = new byte[QueueSz][];
            byte[][] xferBufs = new byte[QueueSz][];
            byte[][] ovLaps = new byte[QueueSz][];

            GCHandle cmdBufferHandle = GCHandle.Alloc(cmdBufs[0], GCHandleType.Pinned);
            GCHandle xFerBufferHandle = GCHandle.Alloc(xferBufs[0], GCHandleType.Pinned);
            GCHandle overlapDataHandle = GCHandle.Alloc(ovLaps[0], GCHandleType.Pinned);


            try
            {
                LockNLoad(cmdBufs, xferBufs, ovLaps); 
            }
            catch (NullReferenceException e)
            {
                // This exception gets thrown if the device is unplugged 
                // while we're streaming data
                e.GetBaseException();
                this.Invoke(handleException);
            }

            //////////////////////////////////////////////////////////////////////////////
            ///////////////Release the pinned memory and make it available to GC./////////
            //////////////////////////////////////////////////////////////////////////////
            cmdBufferHandle.Free();
            xFerBufferHandle.Free();
            overlapDataHandle.Free();
        }

        public unsafe void LockNLoad(byte[][] cBufs, byte[][] xBufs, byte[][] oLaps)
        {
            //initial  parameter of asynchronous transfer
            int j = 0;
            int nLocalCount = j;


            GCHandle[] bufSingleTransfer = new GCHandle[QueueSz];
            GCHandle[] bufDataAllocation = new GCHandle[QueueSz];
            GCHandle[] handleOverlap = new GCHandle[QueueSz];


            
            while(j<QueueSz)
            {
                cBufs[j] = new byte[CyConst.SINGLE_XFER_LEN  + ((inEndpoint.XferMode == XMODE.BUFFERED) ? Bufsz : 0)];
                xBufs[j] = new byte[Bufsz];

                for (int iIndex = 0; iIndex < Bufsz; iIndex++)
                    xBufs[j][iIndex] = DefaultBufInitValue;

                int sz = Math.Max(CyConst.OverlapSignalAllocSize, sizeof(OVERLAPPED));
                oLaps[j] = new byte[sz];

                bufSingleTransfer[j] = GCHandle.Alloc(cBufs[j], GCHandleType.Pinned);
                bufDataAllocation[j] = GCHandle.Alloc(xBufs[j], GCHandleType.Pinned);
                handleOverlap[j] = GCHandle.Alloc(oLaps[j], GCHandleType.Pinned);

                unsafe
                {
                    {
                        CyUSB.OVERLAPPED ovLapStatus = new CyUSB.OVERLAPPED();
                        //allocate memory to ovLapStatus
                        ovLapStatus = (CyUSB.OVERLAPPED)Marshal.PtrToStructure(handleOverlap[j].AddrOfPinnedObject(), typeof(CyUSB.OVERLAPPED));
                        ovLapStatus.hEvent = (IntPtr)PInvoke.CreateEvent(0, 0, 0, 0); // system flag
                        //transmit ovLapStatus infos into the memory address(handleOverlap[j].AddrOfPinnedObject())
                        Marshal.StructureToPtr(ovLapStatus, handleOverlap[j].AddrOfPinnedObject(), true);

                        // Pre-load the queue with a request
                        int len = Bufsz;
                        //set-ups 
                        if (inEndpoint.BeginDataXfer(ref cBufs[j], ref xBufs[j], ref len, ref oLaps[j]) == false)
                            Console.WriteLine("BeginDataXfer error");
                    }
                    j++;
                }
            }
            XferData(cBufs, xBufs, oLaps, handleOverlap);//start asynchronous transfer


            // clear parameter after bRunning=false
            unsafe
            {
                for (nLocalCount = 0; nLocalCount < QueueSz; nLocalCount++)
                {
                    CyUSB.OVERLAPPED ovLapStatus = new CyUSB.OVERLAPPED();
                    ovLapStatus = (CyUSB.OVERLAPPED)Marshal.PtrToStructure(handleOverlap[nLocalCount].AddrOfPinnedObject(), typeof(CyUSB.OVERLAPPED));
                    PInvoke.CloseHandle(ovLapStatus.hEvent);

                    /*////////////////////////////////////////////////////////////////////////////////////////////
                     * 
                     * Release the pinned allocation handles.
                     * 
                    ////////////////////////////////////////////////////////////////////////////////////////////*/
                    bufSingleTransfer[nLocalCount].Free();
                    bufDataAllocation[nLocalCount].Free();
                    handleOverlap[nLocalCount].Free();

                    cBufs[nLocalCount] = null;
                    xBufs[nLocalCount] = null;
                    oLaps[nLocalCount] = null;
                }
            }
            GC.Collect();
        }

        public unsafe void XferData(byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, GCHandle[] handleOverlap)
        {
            int k = 0;
            int len = 0;

            CyUSB.OVERLAPPED ovData = new CyUSB.OVERLAPPED();

            for (;bRunning;)
            {

                // WaitForXfer
                unsafe
                {
                    //fixed (byte* tmpOvlap = oLaps[k])
                    {
                        ovData = (CyUSB.OVERLAPPED)Marshal.PtrToStructure(handleOverlap[k].AddrOfPinnedObject(), typeof(CyUSB.OVERLAPPED));
                        if (!inEndpoint.WaitForXfer(ovData.hEvent, 500))//if timeout return false = EndPoint closed
                        {
                            inEndpoint.Abort(); //
                            PInvoke.WaitForSingleObject(ovData.hEvent, 500); //make sure thread close
                        }
                    }
                }

                if(inEndpoint.FinishDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]))
                {
                    if (xBufs[k] != null)
                        Inbuffer.Enqueue(xBufs[k]); //save different queue time data into dataqueue
                    //Console.WriteLine(BitConverter.ToString(xBufs[k]));
                    //TBox_ShowReceiveData.AppendText(xBufs[k] + Environment.NewLine);
                }

                len = Bufsz;
                if (inEndpoint.BeginDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]) == false)
                    Console.WriteLine("BeginDataXfer error");

                k++;
                if (k == QueueSz)  // Only update displayed stats once each time through the queue
                {
                    k = 0;


                    // Call StatusUpdate() in the main thread
                    if (bRunning == true) this.Invoke(updateUI);

                    // For small QueueSz or PPX, the loop is too tight for UI thread to ever get service.   
                    // Without this, app hangs in those scenarios.
                    Thread.Sleep(0);
                }
                Thread.Sleep(0);

            } // End infinite loop
            // Let's recall all the queued buffer and abort the end point.
            inEndpoint.Abort();
            
        }
        public void ThreadException()
        {
            Btn_StrReceiveData.Text = "Start";
            bRunning = false;
            tXfers = null;

            /*StartBtn.Text = "Start";
            bRunning = false;

            t2 = DateTime.Now;
            elapsed = t2 - t1;
            xferRate = (long)(XferBytes / elapsed.TotalMilliseconds);
            xferRate = xferRate / (int)100 * (int)100;

            tListen = null;

            StartBtn.BackColor = Color.Aquamarine;*/

        }

        private void cboINEndpoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ConstructEndpoints();
        }

        private void cboOutEndPoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ConstructEndpoints();
        }

        private void cboDeviceConnected_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetDevice();
        }
    }

    
}
