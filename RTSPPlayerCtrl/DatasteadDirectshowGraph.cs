using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.Drawing.Imaging;
using DirectShowLib;
using Datastead;

namespace DatasteadDirectShow
{
    public class DatasteadDirectShowGraph_WithEventsMsg : DatasteadDirectShowBaseGraph
    {
        public IntPtr m_WinFormHandle = IntPtr.Zero;
        public bool GetStandardDirectShowInterfaces(IntPtr WindowHandle)
        {
            bool Result = base.GetStandardDirectShowInterfaces();
            if (Result)
            {
                if (WindowHandle != IntPtr.Zero)
                {
                    int hr = m_mediaEventEx.SetNotifyWindow(WindowHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                    Result = hr == 0;
                }
            }
            return Result;
        }
        public new void HandleGraphEvent()
        {
            bool CompleteGraphAsynchronously = base.HandleGraphEvent();
            if (CompleteGraphAsynchronously)
            {
                RunGraph();
            }

        }
    }
    public class DatasteadDirectShowGraph_WithEventsThread : DatasteadDirectShowBaseGraph
    {
        private AutoResetEvent m_WaitEventCompleted;
        private AutoResetEvent m_WaitThread;
        private Thread m_EventThread = null;
        private SafeWaitHandle m_NotificationHandle = null;
        public new bool GetStandardDirectShowInterfaces()
        {
            bool Result = base.GetStandardDirectShowInterfaces();

            if (Result)
            {

                IntPtr EventHandle;
                m_mediaEventEx.GetEventHandle(out EventHandle);
                m_NotificationHandle = new SafeWaitHandle(EventHandle, false);
                m_EventThread = new Thread(new ParameterizedThreadStart(WaitForEvent));
                m_WaitEventCompleted = new AutoResetEvent(false);
                m_WaitThread = new AutoResetEvent(false);
                m_EventThread.Start(SynchronizationContext.Current);
                m_WaitThread.WaitOne();
            }
            return Result;
        }

        public new void CloseInterfaces()
        {
            if (m_mediaEventEx != null)
            {
                m_WaitEventCompleted.Set();
                m_WaitThread.WaitOne();
            }
            base.CloseInterfaces();
        }

        public void WaitForEvent(object context)
        {
            m_WaitThread.Set();
            SynchronizationContext Context = (SynchronizationContext)context;
            AutoResetEvent NotificationEvent = new AutoResetEvent(true);
            NotificationEvent.SafeWaitHandle = m_NotificationHandle;
            //m_WaitEventCompleted

            WaitHandle[] waitHandleList = new WaitHandle[] { NotificationEvent, m_WaitEventCompleted };

            int Result;
            do
            {
                Result = WaitHandle.WaitAny(waitHandleList, Timeout.Infinite, false);
                if (Result == 0)
                {
                    HandleGraphEventFromThread(Context);
                }
            } while (Result != 1);
            m_WaitThread.Set();
        }
        private void HandleGraphEventFromThread(SynchronizationContext Context)
        {
            bool CompleteGraphAsynchronously = base.HandleGraphEvent();
            if (CompleteGraphAsynchronously && m_GraphCanRun)
            {
                Context.Post(new SendOrPostCallback(delegate
                {
                    RunGraph();
                }), null);

            }
        }


    }
    public class DatasteadDirectShowBaseGraph : RTSPFilter, IDisposable
    {
        public delegate void GraphRunResultCallback(bool Success);
        public GraphRunResultCallback GraphRunResult = null;

        public const int WM_GRAPHNOTIFY = 0x8000 + 1;

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // any other managed resource cleanups you can do here
            GC.SuppressFinalize(this);
        }

        ~DatasteadDirectShowBaseGraph()      // finalizer
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseInterfaces();
                }
                _disposed = true;
            }
        }


        // Application-defined message to notify app of filtergraph events
        public enum VideoRendererToUse { vr_None, vr_Default, vr_DatasteadRenderer, vr_StandardRenderer, vr_VMR7, vr_VMR9, vr_NullRenderer };

        private object m_lock = new object();
        protected bool m_GraphCanRun = true;

        public FilterGraph m_filtergraph = null;
        public IMediaControl m_mediaControl = null;
        public IMediaEventEx m_mediaEventEx = null;
        public IGraphBuilder m_graphBuilder = null;
        public IBaseFilter m_DatasteadRTSPSource = null;
        public IDatasteadRTSPSourceConfig m_DatasteadRTSPSourceConfig = null;
        public IDatasteadONVIFPTZ m_DatasteadONVIFPTZ = null;
        public DatasteadRTSPConfigHelper m_DatasteadRTSPConfigHelper = null;
        public IDatasteadRTSPSampleCallback m_DatasteadRTSPSampleCallback = null;
        public ICaptureGraphBuilder2 m_captureGraphBuilder = null;
        private IVideoWindow m_videoWindow = null;
        public DsROTEntry m_rot = null;

        public bool m_VideoStreamEnabled = true;
        public bool m_AudioStreamEnabled = true;
        public bool m_VideoStreamVisible = true;
        public bool m_AudioStreamRendered = true;

        public delegate void GraphEvent(int EventCode, int Param1, int Param2);
        public GraphEvent m_GraphEventCallback = null;

        public delegate void AddToLog(string LogString);
        public AddToLog m_AddToLog = null;
        public Control m_DisplayWindow = null;
        public PictureBox m_PictureFromMemoryBitmap = null;

        public VideoRendererToUse m_VideoRendererToUse = VideoRendererToUse.vr_Default;

        static string vmr7FilterName = "Video Mixing Renderer 7";
        static string vmr9FilterName = "Video Mixing Renderer 9";

        public object Lock
        {
            get
            {
                return m_lock;
            }
        }

        public bool GraphCanRun
        {
            get
            {
                return m_GraphCanRun;
            }
            set
            {
                m_GraphCanRun = value;
            }
        }

        public string GetAudioDevices()
        {
            lock (m_lock)
            {
                IBaseFilter DatasteadRTSPSource = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(RTSPFilter.Datastead_RTSP_RTMP_HTTP_ONVIF_CLSID));
                IDatasteadRTSPSourceConfig DatasteadRTSPSourceConfig = (IDatasteadRTSPSourceConfig)DatasteadRTSPSource;
                DatasteadRTSPConfigHelper RTSPConfigHelper = new DatasteadRTSPConfigHelper(DatasteadRTSPSourceConfig);
                IntPtr AudioDevices;
                string Result = "";
                if (RTSPConfigHelper.GetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_GetAudioDevices_str, out AudioDevices) == 0)
                {
                    Result = Marshal.PtrToStringAuto(AudioDevices);
                }
                Marshal.ReleaseComObject(DatasteadRTSPSourceConfig);
                Marshal.ReleaseComObject(DatasteadRTSPSource);
                return Result;
            }
        }

        public string[] GetAudioDevicesAsItems()
        {
            lock (m_lock)
            {
                string AudioDevicesGot = GetAudioDevices();
                string[] AudioDevices = AudioDevicesGot.Split('\n');
                return AudioDevices;
            }
        }

        public bool RenderVideoPin()
        {
            lock (m_lock)
            {
                bool Result = false;
                IPin pVideoOutPin;
                if (m_captureGraphBuilder.FindPin(m_DatasteadRTSPSource, PinDirection.Output, null, MediaType.Video, true, 0, out pVideoOutPin) == 0)
                {
                    IBaseFilter RendererFilter = null;

                    if ((m_VideoRendererToUse == VideoRendererToUse.vr_Default) || (m_VideoRendererToUse == VideoRendererToUse.vr_DatasteadRenderer) || (m_VideoRendererToUse == VideoRendererToUse.vr_StandardRenderer))
                    {
                        bool Rendered = false;

                        if (m_VideoRendererToUse == VideoRendererToUse.vr_Default)
                        {
                            if (m_graphBuilder.Render(pVideoOutPin) == 0)
                            {
                                Rendered = true;
                            }
                        }
                        else if (m_VideoRendererToUse == VideoRendererToUse.vr_DatasteadRenderer)
                        {
                            RendererFilter = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(DatasteadVideoRendererCLSID));
                            if (RendererFilter != null)
                            {
                                if (m_graphBuilder.AddFilter(RendererFilter, "Datastead Video Renderer") == 0)
                                {
                                    if (m_captureGraphBuilder.RenderStream(null, null, m_DatasteadRTSPSource, null, RendererFilter) == 0)
                                    {
                                        Rendered = true;
                                    }
                                }
                            }
                        }
                        else if (m_VideoRendererToUse == VideoRendererToUse.vr_StandardRenderer)
                        {
                            RendererFilter = (IBaseFilter)new VideoRenderer();
                            if (RendererFilter != null)
                            {
                                if (m_graphBuilder.AddFilter(RendererFilter, "Video Renderer") == 0)
                                {
                                    if (m_captureGraphBuilder.RenderStream(null, null, m_DatasteadRTSPSource, null, RendererFilter) == 0)
                                    {
                                        Rendered = true;
                                    }
                                }
                            }
                        }

                        if (Rendered)
                        {
                            m_videoWindow = (IVideoWindow)m_graphBuilder;

                            m_videoWindow.put_Visible(OABool.False);

                            m_videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);

                            // Use helper function to position video window in client rect 
                            // of main application window
                            ResizeVideoWindow();

                            m_videoWindow.put_Owner(m_DisplayWindow.Handle);

                            // Make the video window visible, now that it is properly positioned
                            m_videoWindow.put_Visible(OABool.True);

                            Result = true;

                        }
                        if (RendererFilter != null)
                        {
                            Marshal.ReleaseComObject(RendererFilter);
                        }
                    }
                    else if (m_VideoRendererToUse == VideoRendererToUse.vr_VMR7)
                    {
                        VideoMixingRenderer vmr7 = new VideoMixingRenderer();
                        if (m_graphBuilder.AddFilter((IBaseFilter)vmr7, vmr7FilterName) == 0)
                        {
                            IVMRFilterConfig vmr7Config = (IVMRFilterConfig)vmr7;
                            vmr7Config.SetRenderingMode(VMRMode.Windowless);

                            IVMRWindowlessControl VMRWindowlessControl7 = (IVMRWindowlessControl)vmr7;
                            VMRWindowlessControl7.SetVideoClippingWindow(m_DisplayWindow.Handle);
                            VMRWindowlessControl7.SetAspectRatioMode(VMRAspectRatioMode.None);
                            if (m_captureGraphBuilder.RenderStream(null, MediaType.Video, m_DatasteadRTSPSource, null, (IBaseFilter)vmr7) == 0)
                            {
                                ResizeVideoWindow();
                            }
                            Marshal.ReleaseComObject(VMRWindowlessControl7);
                            Marshal.ReleaseComObject(vmr7Config);
                            Result = true;
                        }
                        Marshal.ReleaseComObject(vmr7);
                    }
                    else if (m_VideoRendererToUse == VideoRendererToUse.vr_VMR9)
                    {
                        VideoMixingRenderer9 vmr9 = new VideoMixingRenderer9();
                        if (m_graphBuilder.AddFilter((IBaseFilter)vmr9, vmr9FilterName) == 0)
                        {
                            IVMRFilterConfig9 vmr9Config = (IVMRFilterConfig9)vmr9;
                            vmr9Config.SetRenderingMode(VMR9Mode.Windowless);
                            vmr9Config.SetNumberOfStreams(1);

                            IVMRWindowlessControl9 VMRWindowlessControl9 = (IVMRWindowlessControl9)vmr9;
                            VMRWindowlessControl9.SetVideoClippingWindow(m_DisplayWindow.Handle);
                            VMRWindowlessControl9.SetAspectRatioMode(VMR9AspectRatioMode.None);
                            if (m_captureGraphBuilder.RenderStream(null, MediaType.Video, m_DatasteadRTSPSource, null, (IBaseFilter)vmr9) == 0)
                            {
                                ResizeVideoWindow();
                            }
                            Marshal.ReleaseComObject(VMRWindowlessControl9);
                            Marshal.ReleaseComObject(vmr9Config);
                            Result = true;
                        }
                        Marshal.ReleaseComObject(vmr9);
                    }
                    else if (m_VideoRendererToUse == VideoRendererToUse.vr_NullRenderer)
                    {
                        NullRenderer Null_Renderer = new NullRenderer();
                        if (m_graphBuilder.AddFilter((IBaseFilter)Null_Renderer, "Null Renderer") == 0)
                        {
                            if (m_captureGraphBuilder.RenderStream(null, MediaType.Video, m_DatasteadRTSPSource, null, (IBaseFilter)Null_Renderer) == 0)
                            {
                                // ok
                            }
                            Result = true;
                        }
                        Marshal.ReleaseComObject(Null_Renderer);
                    }
                }
                return Result;
            }
        }

        public bool RenderAudioPin()
        {
            lock (m_lock)
            {
                bool Result = false;
                IPin pPin;
                if (m_captureGraphBuilder.FindPin(m_DatasteadRTSPSource, PinDirection.Output, null, MediaType.Audio, true, 0, out pPin) == 0)
                {
                    if (m_graphBuilder.Render(pPin) == 0)
                    {
                        Result = true;
                    }
                    pPin = null;
                }
                return Result;
            }
        }

        protected bool GetStandardDirectShowInterfaces()
        {
            bool Result = false;
            lock (m_lock)
            {
                if (m_filtergraph == null)
                {
                    m_filtergraph = new FilterGraph();
                    if (m_filtergraph != null)
                    {
                        m_graphBuilder = (IGraphBuilder)m_filtergraph;
                        m_mediaControl = (IMediaControl)m_graphBuilder;
                        m_mediaEventEx = (IMediaEventEx)m_graphBuilder;

                        m_captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                        Result = m_captureGraphBuilder.SetFiltergraph(m_graphBuilder) == 0;
                    }
                }
            }
            return Result;
        }

        public bool AddToROT()
        {
            bool Result = false;
            if (m_graphBuilder != null)
            {
                lock (m_lock)
                {
                    // Add our graph to the running object table, which will allow
                    // the GraphEdit application to "spy" on our graph
                    m_rot = new DsROTEntry(m_graphBuilder);
                    Result = m_rot != null;
                }
            }
            return Result;
        }

        public bool AddDatasteadFilterAndQueryFilterInterfaces()
        {
            bool FilterAdded = false;
            lock (m_lock)
            {
                m_DatasteadRTSPSource = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(RTSPFilter.Datastead_RTSP_RTMP_HTTP_ONVIF_CLSID));
                if (m_DatasteadRTSPSource != null)
                {
                    if (m_graphBuilder.AddFilter(m_DatasteadRTSPSource, RTSPFilter.FilterName) == 0)
                    { // if ok
                        m_DatasteadRTSPSourceConfig = (IDatasteadRTSPSourceConfig)m_DatasteadRTSPSource;
                        try
                        {
                            m_DatasteadONVIFPTZ = (IDatasteadONVIFPTZ)m_DatasteadRTSPSource;
                        }
                        catch (InvalidCastException)
                        {
                            m_DatasteadONVIFPTZ = null;
                        }
                        m_DatasteadRTSPConfigHelper = new DatasteadRTSPConfigHelper(m_DatasteadRTSPSourceConfig);
                        m_DatasteadRTSPSampleCallback = (IDatasteadRTSPSampleCallback)m_DatasteadRTSPSource;
                        FilterAdded = true;
                    }
                }
                else
                {
                    MessageBox.Show(RTSPFilter.FilterName + "not registered! This demo is built with \"Any\" as target platform, so it will run the x86 filter on a 32bit OS and the x64 filter on a 64bit OS, verify that you have registered the corresponding filter.");
                }
            }
            return FilterAdded;
        }

        public void CloseInterfaces() // invoked also in the Form1.Designer.cs -> dispose
        {
            lock (m_lock)
            {
                if (m_mediaControl != null)
                {
                    m_mediaControl.Stop();
                }

                // Relinquish ownership (IMPORTANT!) of the video window.
                // Failing to call put_Owner can lead to assert failures within
                // the video renderer, as it still assumes that it has a valid
                // parent window.
                if (m_videoWindow != null)
                {
                    m_videoWindow.put_Visible(OABool.False);
                    m_videoWindow.put_Owner(IntPtr.Zero);
                }

                bool ExistingRTSPFilterOrFilterGraphInstance = false;

                if (m_DatasteadRTSPSourceConfig != null)
                {
                    m_DatasteadRTSPSourceConfig.Action(RTSPConfigParam.RTSP_Action_CancelPendingConnection.ToString(), "");
                    ExistingRTSPFilterOrFilterGraphInstance = true;
                }

                // Remove filter graph from the running object table
                if (m_rot != null)
                {
                    m_rot.Dispose();
                    m_rot = null;
                }

                // Release DirectShow interfaces
                if (m_DatasteadRTSPSourceConfig != null) { Marshal.ReleaseComObject(m_DatasteadRTSPSourceConfig); m_DatasteadRTSPSourceConfig = null; }
                if (m_DatasteadONVIFPTZ != null) { Marshal.ReleaseComObject(m_DatasteadONVIFPTZ); m_DatasteadONVIFPTZ = null; }
                if (m_DatasteadRTSPSampleCallback != null) { Marshal.ReleaseComObject(m_DatasteadRTSPSampleCallback); m_DatasteadRTSPSampleCallback = null; }
                if (m_DatasteadRTSPSource != null) { Marshal.ReleaseComObject(m_DatasteadRTSPSource); m_DatasteadRTSPSource = null; }
                if (m_mediaControl != null) { Marshal.ReleaseComObject(m_mediaControl); m_mediaControl = null; }
                if (m_mediaEventEx != null) { Marshal.ReleaseComObject(m_mediaEventEx); m_mediaEventEx = null; }
                if (m_videoWindow != null) { Marshal.ReleaseComObject(m_videoWindow); m_videoWindow = null; }
                if (m_graphBuilder != null) { Marshal.ReleaseComObject(m_graphBuilder); m_graphBuilder = null; }
                if (m_captureGraphBuilder != null) { Marshal.ReleaseComObject(m_captureGraphBuilder); m_captureGraphBuilder = null; }
                if (m_DatasteadRTSPConfigHelper != null) { m_DatasteadRTSPConfigHelper = null; };
                if (m_filtergraph != null)
                {
                    ExistingRTSPFilterOrFilterGraphInstance = true;
                    Marshal.ReleaseComObject(m_filtergraph);
                    m_filtergraph = null;
                }
                GC.Collect();
                if (ExistingRTSPFilterOrFilterGraphInstance)
                {
                    if (m_AddToLog != null)
                    {
                        m_AddToLog("-instances released-");
                    }
                }
            }
        }

        public bool HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode;
            IntPtr evParam1, evParam2;
            bool CompleteGraphAsynchronously = false;
            bool CanCloseInterfaces = false;

            lock (m_lock)
            {
                if (m_mediaEventEx != null)
                {
                    if (m_mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
                    {
                        if (m_mediaControl != null)
                        {
                            if ((int)evCode == EC_RTSPNOTIFY)
                            {
                                switch (evParam1.ToInt32())
                                {
                                    case EC_RTSP_PARAM1_OPENURLASYNC_CONNECTION_RESULT:
                                        {
                                            if (evParam2 != IntPtr.Zero)
                                            {
                                                /// we have received the notification that the URL is now connected, we can render the pins and run the graph.
                                                CompleteGraphAsynchronously = true;
                                            }
                                            else
                                            {
                                                if (m_AddToLog != null) m_AddToLog("error: asynchronous URL connection failed, check if the URL is correct or the server is alive");
                                                CanCloseInterfaces = true;
                                            }

                                        }
                                        break;
                                    case EC_RTSP_PARAM1_DEVICELOST_RECONNECTING:
                                        {
                                            if (m_AddToLog != null) m_AddToLog("reconnecting...");
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_DEVICELOST_RECONNECTED:
                                        {
                                            if (m_AddToLog != null) m_AddToLog("reconnected.");
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_RECORDTONEWFILE_COMPLETED:
                                        {
                                            IntPtr LastRecordingFilename;
                                            m_DatasteadRTSPConfigHelper.GetStr(RTSPConfigParam.RTSP_LastRecorded_FileName_str, out LastRecordingFilename);
                                            if (m_AddToLog != null) m_AddToLog("recording to " + Marshal.PtrToStringUni(LastRecordingFilename) + " completed: ");

                                            int RecordedFileSizeKb;
                                            m_DatasteadRTSPConfigHelper.GetInt(RTSPConfigParam.RTSP_LastRecorded_FileSizeKb_int, out RecordedFileSizeKb);
                                            if (m_AddToLog != null) m_AddToLog("file size in Kb: " + RecordedFileSizeKb.ToString());

                                            int RecordedDurationMs;
                                            m_DatasteadRTSPConfigHelper.GetInt(RTSPConfigParam.RTSP_LastRecorded_ClipDurationMs_int, out RecordedDurationMs);
                                            if (m_AddToLog != null) m_AddToLog("duration in ms: " + RecordedDurationMs.ToString());

                                            int FrameCount;
                                            m_DatasteadRTSPConfigHelper.GetInt(RTSPConfigParam.RTSP_LastRecorded_VideoFrameCount_int, out FrameCount);
                                            if (m_AddToLog != null) m_AddToLog("frame count: " + FrameCount.ToString());

                                            IntPtr NewRecordingFilename;
                                            m_DatasteadRTSPConfigHelper.GetStr(RTSPConfigParam.RTSP_CurrentRecording_FileName_str, out NewRecordingFilename);
                                            if (m_AddToLog != null) m_AddToLog("now recording to: " + Marshal.PtrToStringUni(NewRecordingFilename));
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_FRAME_CAPTURE_SUCCEEDED:
                                        {
                                            if (evParam2 == IntPtr.Zero) // in this case this is a frame capture to file
                                            {
                                                if (m_AddToLog != null) m_AddToLog("frame capture to file successful.");
                                            }
                                            else
                                            {
                                                if (m_AddToLog != null) m_AddToLog("frame capture to memory bitmap successful.");
                                                if (m_PictureFromMemoryBitmap != null)
                                                {
                                                    m_PictureFromMemoryBitmap.Image = Image.FromHbitmap(evParam2);
                                                }
                                            }
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_FRAME_CAPTURE_FAILED:
                                        {
                                            if (m_AddToLog != null) m_AddToLog("frame capture failed, GetLastError return code: " + evParam2.ToString());
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_ONVIF_SNAPSHOT_SUCCEEDED:
                                        {
                                            if (m_AddToLog != null) m_AddToLog("ONVIF snapshot succeeded");
                                        }
                                        break;
                                    case EC_RTSP_PARAM1_ONVIF_SNAPSHOT_FAILED:
                                        {
                                            if (m_AddToLog != null) m_AddToLog("ONVIF snapshot failed");
                                        }
                                        break;

                                }
                            }
                            else if (evCode == EventCode.DeviceLost)
                            {
                                if (m_AddToLog != null) m_AddToLog("device lost!");
                            }

                        }
                        if (m_GraphEventCallback != null)
                        {
                            m_GraphEventCallback((int)evCode, evParam1.ToInt32(), evParam2.ToInt32());
                        }
                        // Free event parameters to prevent memory leaks associated with
                        // event parameter data.  While this application is not interested
                        // in the received events, applications should always process them.
                        hr = m_mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                        DsError.ThrowExceptionForHR(hr);
                    }
                }
            }

            if (CanCloseInterfaces)
            {
                CloseInterfaces();
            }

            return CompleteGraphAsynchronously;
        }
        public bool RunGraph()
        {
            lock (m_lock)
            {
                int hr = 0;

                string Step = "";

                IntPtr StreamInfo;
                hr = m_DatasteadRTSPConfigHelper.GetStr(RTSPFilter.RTSPConfigParam.RTSP_Source_StreamInfo_str, out StreamInfo);
                if (hr == 0) // if success
                {
                    if (m_AddToLog != null) m_AddToLog("Connected.\r\n\r\n" + Marshal.PtrToStringUni(StreamInfo));
                }
                else
                {
                    DsError.ThrowExceptionForHR(hr);
                }

                // END DATASTEAD RTSP INITIALIZATION CODE 
                // ---------------------------------------------------------

                if (m_VideoStreamVisible)
                {
                    RenderVideoPin();
                }

                if (m_AudioStreamRendered)
                {
                    RenderAudioPin();
                }


                // Start previewing video data
                Step = "run graph";

                hr = m_mediaControl.Run();
                if (hr >= 0)
                { // when Run() succeeds it may also return 1 (S_FALSE), that means that not all the filters have yet completed the transition. This is normal.
                    // ok

                }
                else
                {
                    m_AddToLog("error: CompleteGraph() failed at step \"" + Step + "\"");
                }

                if (GraphRunResult != null)
                {
                    GraphRunResult(hr >= 0);
                }

                return hr >= 0;
            }
        }

        public void ResizeVideoWindow()
        {
            lock (m_lock)
            {
                if (m_DisplayWindow == null) return;
                if (m_VideoRendererToUse == VideoRendererToUse.vr_VMR9)
                {
                    IBaseFilter RenderFilter;
                    if (m_graphBuilder.FindFilterByName (vmr9FilterName, out RenderFilter) == 0)
                    {
                        IVMRWindowlessControl9 VMRWindowlessControl9 = (IVMRWindowlessControl9)RenderFilter;
                        VMRWindowlessControl9.SetVideoPosition(null, DsRect.FromRectangle(m_DisplayWindow.ClientRectangle));
                        Marshal.ReleaseComObject(VMRWindowlessControl9);
                        Marshal.ReleaseComObject(RenderFilter);
                    }
                }
                else if (m_VideoRendererToUse == VideoRendererToUse.vr_VMR7)
                {
                    IBaseFilter RenderFilter;
                    if (m_graphBuilder.FindFilterByName(vmr7FilterName, out RenderFilter) == 0)
                    {
                        IVMRWindowlessControl VMRWindowlessControl7 = (IVMRWindowlessControl)RenderFilter;
                        VMRWindowlessControl7.SetVideoPosition(null, DsRect.FromRectangle(m_DisplayWindow.ClientRectangle));
                        Marshal.ReleaseComObject(VMRWindowlessControl7);
                        Marshal.ReleaseComObject(RenderFilter);
                    }
                }
                if (m_videoWindow != null)
                {
                    m_videoWindow.SetWindowPosition(0, 0, m_DisplayWindow.ClientSize.Width, m_DisplayWindow.ClientSize.Height);
                }
            }
        }

        public class DatasteadRTSPConfigHelper
        {
            private IDatasteadRTSPSourceConfig m_DatasteadRTSPSourceConfig;

            public DatasteadRTSPConfigHelper(IDatasteadRTSPSourceConfig DatasteadRTSPSourceConfig)
            {
                m_DatasteadRTSPSourceConfig = DatasteadRTSPSourceConfig;
            }

            public int GetBool(RTSPConfigParam ParamID, out bool Value)
            {
                return m_DatasteadRTSPSourceConfig.GetBool(ParamID.ToString(), out Value);
            }

            public int GetDouble(RTSPConfigParam ParamID, out double Value)
            {
                return m_DatasteadRTSPSourceConfig.GetDouble(ParamID.ToString(), out Value);
            }

            public int GetInt(RTSPConfigParam ParamID, out int Value)
            {
                return m_DatasteadRTSPSourceConfig.GetInt(ParamID.ToString(), out Value);
            }

            public int GetStr(RTSPConfigParam ParamID, out IntPtr Value)
            {
                return m_DatasteadRTSPSourceConfig.GetStr(ParamID.ToString(), out Value);
            }

            public int SetBool(RTSPConfigParam ParamID, bool Value)
            {
                return m_DatasteadRTSPSourceConfig.SetBool(ParamID.ToString(), Value);
            }

            public int SetDouble(RTSPConfigParam ParamID, double Value)
            {
                return m_DatasteadRTSPSourceConfig.SetDouble(ParamID.ToString(), Value);
            }

            public int SetInt(RTSPConfigParam ParamID, int Value)
            {
                return m_DatasteadRTSPSourceConfig.SetInt(ParamID.ToString(), Value);
            }

            public int SetStr(RTSPConfigParam ParamID, string Value)
            {
                return m_DatasteadRTSPSourceConfig.SetStr(ParamID.ToString(), Value);
            }

            public int Action(RTSPConfigParam ActionID, string Option)
            {
                return m_DatasteadRTSPSourceConfig.Action(ActionID.ToString(), Option);
            }
        }
    }

    public class DatasteadHelper
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        public static Bitmap MakeBitmapFromBitmapBits(IntPtr Buffer, Int32 BufferSize, Int32 VideoWidth, Int32 VideoHeight, Int32 BitCount, Int32 Stride)
        {
            PixelFormat pixelFormat;
            if (BitCount == 24)
            {
                pixelFormat = PixelFormat.Format24bppRgb;
            }
            else if (BitCount == 32)
            {
                pixelFormat = PixelFormat.Format32bppRgb;
            }
            else
            {
                return null; 
            }
            GC.Collect(); // recommended, otherwise if these Bitmaps are created at 30 fps, the garbage collector may not be free them fast enough and the memory be comsumed

            bool TopDown = false;
            if (VideoHeight < 0)
            {
                VideoHeight = -VideoHeight;
                TopDown = true;
            }
            Bitmap bitmap = new Bitmap(VideoWidth, VideoHeight, pixelFormat);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, VideoWidth, VideoHeight), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            if (TopDown)
            {
                CopyMemory(bmpData.Scan0, Buffer, (uint)BufferSize); // CopyMemory is the direct way, otherwise we have to make a double Marshal.Copy through an intermediary byte[] buffer
            }
            else
            {
                Int64 pSrc = Buffer.ToInt64();
                Int64 pDest = bmpData.Scan0.ToInt64();
                pSrc = pSrc + ((VideoHeight - 1) * Stride);
                for (int i = 0; i < VideoHeight; i++)
                {
                    uint LineSize = (uint)Stride;
                    CopyMemory((IntPtr)pDest, (IntPtr)pSrc, LineSize); // CopyMemory is the direct way, otherwise we have to make a double Marshal.Copy through an intermediary byte[] buffer
                    pSrc -= LineSize;
                    pDest += LineSize;
                }
            }
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }
    }
}
