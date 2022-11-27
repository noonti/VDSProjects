#define WITH_EVENTS_MSG
#define WITH_EVENTS_THREAD

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatasteadDirectShow;
using Datastead;
using VDSCommon;
using System.Reflection;

namespace RTSPPlayerCtrl
{
    public partial class RTSPPlayer : UserControl
    {
        public FormAddLogDelegate _addLog = null;
        public Control _control = null;

        public bool _playing = false;

#if WITH_EVENTS_MSG
        public DatasteadDirectShowGraph_WithEventsMsg m_Graph = new DatasteadDirectShowGraph_WithEventsMsg();
#else
        public DatasteadDirectShowGraph_WithEventsThread m_Graph = new DatasteadDirectShowGraph_WithEventsThread();
#endif


        public RTSPPlayer()
        {
            InitializeComponent();
            m_Graph.m_AddToLog = AddToLog;
            //m_Graph.m_PictureFromMemoryBitmap = imgCaptureToMemoryBitmap;
            m_Graph.m_DisplayWindow = plScreen;
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case DatasteadDirectShowGraph_WithEventsMsg.WM_GRAPHNOTIFY:
                    {
                        m_Graph.HandleGraphEvent();
                        break;
                    }
            }
            base.WndProc(ref m);
        }


        public int SetRTSPLogDelegate(Control control, FormAddLogDelegate addLogDelegate)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                _addLog = addLogDelegate;
                if (_control == null)
                    _control = control;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }



        private void AddToLog(string LogString)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                String buf = String.Format("{0}\t[{1}]\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), LOG_TYPE.LOG_INFO, LogString);
                if (_addLog != null && _control != null)
                {
                    _control.BeginInvoke(_addLog, new object[] { LOG_TYPE.LOG_INFO, buf });
                }
                Utility.AddLog(LOG_TYPE.LOG_INFO, LogString);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));


            
        }

        public bool StartStreaming(bool OpenAsynchronously, String url)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                StopStreaming();
                Application.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture; // to get a dot as decimal separator when entering the frame rate

#if WITH_EVENTS_MSG
                m_Graph.GetStandardDirectShowInterfaces(this.Handle);
#else
                m_Graph.GetStandardDirectShowInterfaces();
#endif

                m_Graph.AddToROT(); // to be able to spy the graph from GraphEdit, remove this statement for production


                if (!m_Graph.AddDatasteadFilterAndQueryFilterInterfaces())
                {
                    AddToLog("error: failed to create Datastead RTSP filter instance");
                }
                else
                {


                    /* PCM audio sample callback: uncomment the 2 lines below to activate it */
                    //RTSPFilter.DatasteadRTSP_AudioSampleCallback AudioSampleCallback = new RTSPFilter.DatasteadRTSP_AudioSampleCallback(RTSP_AudioSampleCallback);
                    //m_Graph.m_DatasteadRTSPSampleCallback.SetAudioPCMSampleCallback (Marshal.GetFunctionPointerForDelegate(AudioSampleCallback), IntPtr.Zero);

                    /* raw sample callback: uncomment the 2 lines below to activate it */
                    //RTSPFilter.DatasteadRTSP_RawSampleCallback RawSampleCallback = new RTSPFilter.DatasteadRTSP_RawSampleCallback(RTSP_RawSampleCallback);
                    //m_Graph.m_DatasteadRTSPSampleCallback.SetRawSampleCallback(Marshal.GetFunctionPointerForDelegate(RawSampleCallback), IntPtr.Zero);

                    m_Graph.m_DatasteadRTSPSourceConfig.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Filter_LicenseKey_str.ToString(), "DTSTDRTSP:964566626658750802812-59pownateco");

                    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_Enabled_bool, true);

                    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_AudioStream_Enabled_bool, true);

                    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_AudioStream_Recorded_bool, false);

                    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_Recorded_bool, false);

                    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_Source_AutoReconnect_bool, true);

                    //if (m_DecodeKeyFramesOnly)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_Decode_KeyFrames_Only_bool, true);
                    //}

                    //if (chkEnableTextOverlay.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_ConfigureTextOverlay_str, txtTextOverlay.Text);
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_ConfigureTextOverlay_str, txtTextOverlay2.Text);
                    //    // it is possible to add more overlays
                    //}

                    //if (chkBrightHueSat.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_ConfigureHueBrightSat_str, txtBrightHueSat.Text);
                    //}

                    //if (rdbTransportDefault.Checked)
                    {
                        m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RTSPTransport_int, RTSPFilter.RTSPTransportMode_Auto);
                    }
                    //else if (rdbTransportUDP.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RTSPTransport_int, RTSPFilter.RTSPTransportMode_UDP);
                    //}
                    //else if (rdbTransportTCP.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RTSPTransport_int, RTSPFilter.RTSPTransportMode_TCP);
                    //}
                    //else if (rdbTransportHTTP.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RTSPTransport_int, RTSPFilter.RTSPTransportMode_HTTP);
                    //}
                    //else if (rdbTransportMulticast.Checked)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RTSPTransport_int, RTSPFilter.RTSPTransportMode_Udp_Multicast);
                    //}

                    //if (chkVidSync.CheckState != CheckState.Indeterminate)
                    {
                        m_Graph.m_DatasteadRTSPConfigHelper.SetBool(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_Synchronized_bool, true);
                    }

                    //if (chkLowDelay.CheckState != CheckState.Indeterminate)
                    //{
                    //    m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_LowDelay_int, Convert.ToInt32(chkLowDelay.Checked)); // 0 = disabled, 1 = enabled
                    //}

                    //if (chkUseSelectedAudioDeviceAsAudioSource.Checked)
                    //{
                    //    if (lstAudioDevices.Items.Count > 0)
                    //    {
                    //        if (lstAudioDevices.SelectedIndex > -1)
                    //        {
                    //            string SelectedAudioDevice = lstAudioDevices.Items[lstAudioDevices.SelectedIndex].ToString();
                    //            m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_SetAudioDevice_str, SelectedAudioDevice);
                    //        }
                    //    }
                    //}

                    //int BufferValue;
                    //if (Int32.TryParse(tbBuffering.Text, out BufferValue))
                    //{
                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_BufferDuration_int, BufferValue);
                    //    LogIfError(hr, "set buffer duration");
                    //}

                    //int TimePositionValue;
                    //if (Int32.TryParse(tbTimePosition.Text, out TimePositionValue))
                    //{
                    //    if (TimePositionValue > 0)
                    //    {
                    //        int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_StartTime_int, TimePositionValue);
                    //        LogIfError(hr, "set start time");
                    //    }
                    //}

                    //double FrameRate;
                    //if (double.TryParse(tbFrameRate.Text, out FrameRate))
                    //{
                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetDouble(RTSPFilter.RTSPConfigParam.RTSP_VideoStream_MaxFrameRate_double, FrameRate);
                    //    LogIfError(hr, "set frame rate");
                    //}

                    //if (tbUserName.Text != string.Empty)
                    //{
                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_AuthUser_str, tbUserName.Text);
                    //    LogIfError(hr, "set user authentication");
                    //}

                    //if (tbPassword.Text != string.Empty)
                    //{
                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_AuthPassword_str, tbPassword.Text);
                    //    LogIfError(hr, "set password authentication");
                    //}

                    //if (chkRestreamURL.Checked)
                    //{
                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Dest_URL_str, edtRestreamURL.Text);
                    //    LogIfError(hr, "set restream URL");
                    //}


                    //if (cbRecord.Checked)
                    //{

                    //    int hr = m_Graph.m_DatasteadRTSPConfigHelper.SetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_RecordingFileName_str, tbRecordingFileName.Text);
                    //    LogIfError(hr, "set recording file name");
                    //    if (m_StartWithRecordingPaused)
                    //    {
                    //        hr = m_Graph.m_DatasteadRTSPConfigHelper.Action(RTSPFilter.RTSPConfigParam.RTSP_Action_PauseRecording, null);
                    //        LogIfError(hr, "start with recording paused");
                    //    }

                    //    if (txtBacktimedStartSeconds.Text != "0")
                    //    {
                    //        int BacktimedRecordingStartSeconds = 0;
                    //        if (int.TryParse(txtBacktimedStartSeconds.Text, out BacktimedRecordingStartSeconds))
                    //        {
                    //            hr = m_Graph.m_DatasteadRTSPConfigHelper.SetInt(RTSPFilter.RTSPConfigParam.RTSP_Source_RecordingBacktimedStartSeconds_int, BacktimedRecordingStartSeconds);
                    //            LogIfError(hr, "set backtimed recording start in seconds");
                    //        }
                    //        if (BacktimedRecordingStartSeconds > 0)
                    //        {
                    //            if (!m_StartWithRecordingPaused)
                    //            {
                    //                MessageBox.Show("Note: when specifying a number of backtimed start recording seconds, the recording should be started in \"paused\" mode, then resume the when needed (to catch the few seconds before the resume)");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            MessageBox.Show("oops, invalid backtimed start recording seconds value!");
                    //        }
                    //    }
                    //}

                    string URL = url;

                    m_Graph.GraphRunResult = GraphRunResultCallback;
                    m_Graph.m_VideoRendererToUse = DatasteadDirectShowGraph_WithEventsMsg.VideoRendererToUse.vr_StandardRenderer;
                    if (OpenAsynchronously)
                    {
                        // Action (RTSP_Action_OpenURLAsync) opens the URL asynchrnously
                        int hr = m_Graph.m_DatasteadRTSPConfigHelper.Action(RTSPFilter.RTSPConfigParam.RTSP_Action_OpenURLAsync, URL);
                        LogIfError(hr, "open URL asynchronously");
                        /// starting from here the filter connects asynchrnously to the URL and notify the graph through IMediaEventEx by sending EC_RTSPNOTIFY (EC_RTSP_PARAM1_OPENURLASYNC_SUCCEEDED, 0), see the HandleGraphEvent function below
                        if (hr == 0)
                        {
                            AddToLog("Connecting asynchronously...");
                        }
                        else
                        {
                            m_Graph.CloseInterfaces();
                        }
                    }
                    else
                    {
                        // Action (RTSP_Action_OpenURL) opens the URL synchrnously
                        AddToLog("Connecting synchronously...");
                        Application.DoEvents(); /// to update the log window
                        int hr = m_Graph.m_DatasteadRTSPConfigHelper.Action(RTSPFilter.RTSPConfigParam.RTSP_Action_OpenURL, URL);
                        LogIfError(hr, "open URL synchronously");
                        if (hr == 0)
                        {
                            m_Graph.RunGraph();
                        }
                        else
                        {
                            m_Graph.CloseInterfaces();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return true;
        }

        public void StopStreaming()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                m_Graph.CloseInterfaces();
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private void LogIfError(int hr, string LogString)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                UInt32 HResult = (UInt32)hr;
                if (HResult > 0x80000000) // In DirectShow errors correspond to an HResult hex value >= 0x80000000
                {
                    AddToLog(LogString + ": error 0x" + HResult.ToString("X8"));
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void GraphRunResultCallback(bool Success)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                AddToLog("running");
                //btnPauseRecording.Enabled = cbRecord.Checked;
                //btnResumeRecording.Enabled = cbRecord.Checked;
                //btnGenerateNewFile.Enabled = cbRecord.Checked;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public int SaveCurrentFrame(String fileName)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 1;
            try
            {
                if (m_Graph.m_DatasteadRTSPSourceConfig != null)
                {
                    m_Graph.m_DatasteadRTSPConfigHelper.Action(RTSPFilter.RTSPConfigParam.RTSP_Action_CaptureFrame, fileName);
                }
                else
                    nResult = 0;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                nResult = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }
    }
}
