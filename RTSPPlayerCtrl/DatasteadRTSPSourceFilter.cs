using System;
//using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
//using System.Collections.Generic;

namespace Datastead
{
    /// Datastead RTSP/RTMP DirectShow source filter.
    public class RTSPFilter
	{
        public const string FilterName = "Datastead RTSP/RTMP/HTTP/ONVIF DirectShow Source Filter";
        // values to pass to SetInt (RTSP_Source_RTSPTransport_int, value)
        public const int RTSPTransportMode_Auto = 0;
        public const int RTSPTransportMode_TCP = 1;
        public const int RTSPTransportMode_UDP = 2;
        public const int RTSPTransportMode_HTTP = 3;
        public const int RTSPTransportMode_Udp_Multicast = 4;

        
        const int EC_USER = 0x8000; // from DirectShow, comment this line if you get an "already defined" error
        public const int EC_RTSPNOTIFY = EC_USER + 0x4000;
        public const int EC_RTSP_PARAM1_DISKFULL = 1;
        public const int EC_RTSP_PARAM1_DEVICELOST_RECONNECTING = 3;
        public const int EC_RTSP_PARAM1_DEVICELOST_RECONNECTED = 4;
        public const int EC_RTSP_PARAM1_RECORDTONEWFILE_COMPLETED = 5;
        public const int EC_RTSP_PARAM1_OPENURLASYNC_CONNECTION_RESULT = 6;
        public const int EC_RTSP_PARAM1_FRAME_CAPTURE_SUCCEEDED = 7;
        public const int EC_RTSP_PARAM1_FRAME_CAPTURE_FAILED = 8;
        public const int EC_RTSP_PARAM1_ONVIF_SNAPSHOT_SUCCEEDED = 9;
        public const int EC_RTSP_PARAM1_ONVIF_SNAPSHOT_FAILED = 10;
		public const int EC_RTSP_DURATION_UPDATED = 11;

        //_________________________________________________________________________________
        // RTSP/RTMP/HTTP/ONVIF DirectShow source filter CLSID

        public static readonly Guid Datastead_RTSP_RTMP_HTTP_ONVIF_CLSID = new Guid("55D1139D-5E0D-4123-9AED-575D7B039569");
        
        public static readonly Guid DatasteadRtspFilterCLSID = Datastead_RTSP_RTMP_HTTP_ONVIF_CLSID; /* for backward compatibility */

        //_________________________________________________________________________________
        // Datastead Video Renderer filter CLSID
        // the Datastead Video Renderer supports the screen lock (Ctrl+Alt+Del)

        public static readonly Guid DatasteadVideoRendererCLSID = new Guid("C7CC1A23-8B8A-4BFD-A96C-B5E735E055BA");

        //_________________________________________________________________________________
        // IDatasteadONVIFPTZ interface
        
        [ComImport, Guid("B7773ACC-7990-4965-A0E8-E22BAF0ABA81"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadONVIFPTZ
        {
            [PreserveSig] int GetPosition([Out] out double Pan, [Out] out double Tilt, [Out] out double Zoom, [Out] out Int64 UTCTime, [Out] out int IsMoving);
            [PreserveSig] int SetPosition([In] double Pan, [In] double Tilt, [In] double Zoom, [In] double SpeedRatio, [In] bool IsRelative);
            [PreserveSig] int StartMove([In, MarshalAs(UnmanagedType.LPWStr)] string PTZType, [In] bool OppositeDirection, [In] double SpeedRatio, [In] int DurationMs);
            [PreserveSig] int StopMove([In, MarshalAs(UnmanagedType.LPWStr)] string PTZType);
            [PreserveSig] int Preset([In, MarshalAs(UnmanagedType.LPWStr)] string Action, [In, MarshalAs(UnmanagedType.LPWStr)] string PresetName);
            [PreserveSig] int SendAuxiliaryCommand([In, MarshalAs(UnmanagedType.LPWStr)] string AuxiliaryCommand);
            [PreserveSig] int GetLimits([Out] out double Pan_Min, [Out] out double Pan_Max, [Out] out double Tilt_Min, [Out] out double Tilt_Max, [Out] out double Zoom_Min, [Out] out double Zoom_Max);
        }

        /* possible values for the StartMove(...) and StopMove functions */
        /* not case sensitive */

        public static class PTZTypes
        {
            public static string Pan = "Pan";
            public static string Tilt = "Tilt";
            public static string Zoom = "Zoom";
        }

        /* possible values for the Preset() function */
        /* not case sensitive */

        public static class PTZPresets
        {
            public static string Create = "Create";
            public static string Remove = "Remove";
            public static string Goto = "Goto";
        }

        //_________________________________________________________________________________
        // IDatasteadRTSPSourceConfig interface (MAIN FILTER CONFIGURATION INTERFACE)

        [ComImport, Guid("D7557B82-3FA4-4F4F-B7CF-96108202E4AF"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSourceConfig
        {
            [PreserveSig] int GetBool     ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out bool Value);
            [PreserveSig] int GetDouble   ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out double Value);
            [PreserveSig] int GetInt      ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out int Value);
            [PreserveSig] int GetStr      ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out IntPtr Value);
			
            [PreserveSig] int SetBool     ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [In] bool Value);
            [PreserveSig] int SetDouble   ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [In] double Value);
            [PreserveSig] int SetInt      ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [In] int Value);
            [PreserveSig] int SetStr      ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [In, MarshalAs(UnmanagedType.LPWStr)] string Value);
			
            [PreserveSig] int Action      ([In, MarshalAs(UnmanagedType.LPWStr)] string ActionID, [In, MarshalAs(UnmanagedType.LPWStr)] string Option);
        }

        // IDatasteadRTSPSourceConfig2 : IDatasteadRTSPSourceConfig
        [ComImport, Guid("932C369A-3335-45BB-B5F4-A6CF77F9B40C"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSourceConfig2 : IDatasteadRTSPSourceConfig
        {
            [PreserveSig] int GetIntPtr   ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out IntPtr Value);
        }

        // IDatasteadRTSPSourceConfig3 : IDatasteadRTSPSourceConfig2
        [ComImport, Guid("6FCA28BE-7F96-467A-BB08-0611FC8162B7"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSourceConfig3 : IDatasteadRTSPSourceConfig2
        {
            [PreserveSig] int GetInt64    ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [Out] out Int64 Value);
            [PreserveSig] int SetInt64    ([In, MarshalAs(UnmanagedType.LPWStr)] string ParamID,  [In] Int64 Value);
        }
        //_________________________________________________________________________________
        // Current state: returned by DatasteadRTSPSourceConfig.GetInt (RTSP_Source_GetState_int, out SourceState)
        //_________________________________________________________________________________

        public enum TSourceState { state_disconnected, state_connecting_async, state_connecting_sync, state_reconnecting, state_connected, state_previewing, state_recording_paused, state_recording_active };

        //_________________________________________________________________________________
        // Filter configuration identifiers that are supported as ParamID by the IDatasteadRTSPSourceConfig functions
        //_________________________________________________________________________________
        public enum RTSPConfigParam
        {

            //===========================
            // ACTIONS
            //===========================

            RTSP_Action_OpenURL,
            /*
            Set the URL and connects the filter synchronously
            This function must be invoked while configuring the filter, at last, after setting all the optional parameters, if needed.
            Returns S_OK upon success
            */

            RTSP_Action_OpenURLAsync,
            /*
            Set the URL and initiates the connection, but returns immediately without waiting for the connection to complete.
            The filter is connecting in the background and will notify when the connection complete through the ImediaEventEx notification or a callback function (see below).
            Note that invoking OpenURLAsync EXITS IMMEDIATELY without waiting for the connection to complete. So you must wait for callback before trying to render the pins, because the pin formats are not available until the filter connection is completed.
            
            This function must be invoked while configuring the filter, at last, after setting all the optional parameters, if needed.
            
            The function initiates the connection and returns S_OK if the URL syntax is correct.
            
            Then, when the filter completes the connection, the application can get notified in 2 ways:
            
            1) the EC_RTSPNOTIFY (param1, param2) graph event occurs with:
            param1 = EC_RTSP_PARAM1_OPENURLASYNC_CONNECTION_RESULT
            param2 = 1 if the connection succeeds, 0 if the connection fails.
            
            For sample code search for "HandleGraphEvent()" in Form1.cs of the C# demo project
            
            2) if OpenURLAsyncCompletionCB has been configured with SetAsyncOpenURLCallback, the callback occurs and the Result parameter returns S_OK upon success, or an error code upon failure
            */

            RTSP_Action_IsURLResponding,
            /*
            Test if the URL is responding, without starting the live stream
            Returns S_OK upon success
            */

            RTSP_Action_GetONVIFSnapshot,
            /*
            captures synchronously a snapshot from an ONVIF URL
            */

            RTSP_Action_GetONVIFSnapshotAsync,
            /*
            captures asynchronously a snapshot from an ONVIF URL
            */

            RTSP_Action_RecordToNewFileNow,
            /*
            Close the current file being written and starts writing to a new file specified as parameter. The new file must have the same extension than the previous one.
            
            - if no file name is specified as parameter, the current file is closed, reopened and overwritten.
            
            - to temporarily suspend the recording without sopping the graph, pass a file name having the same extension and "nul" as name, e.g. if recording in MP4, pass nul.mp4 as parameter (as is, without file path). The recording remains suspended until you pass a new valid file name to resume the recording.
            
            Note: this action applies only while the graph is running and recording.
            To start a new recording graph:
            - first set the recording file name with SetStr (RTSP_Source_RecordingFileName_str, filename)
            - then invoke Action (RTSP_Action_OpenURL, URL)  or Action (RTSP_Action_OpenURLAsync, URL)
            */

            RTSP_Action_CancelPendingConnection,
            /*
            Cancels a pending URL connection, previously initiated by  RTSP_Action_OpenURLAsync
            It can be invoked e.g. when exiting the application, just before clearing the graph, to ensure any pending connection is cancelled immediately.
            */

            RTSP_Action_PauseURL,
            /*
            Pauses the video stream
            */

            RTSP_Action_ResumeURL,
            /*
            Resumes the video stream
            */

            RTSP_Action_PauseRecording,
            /*
            Pauses the recording of the current file, while the preview keeps running.
            */

            RTSP_Action_ResumeRecording,
            /*
            Resumes the recording of the current file, if previously paused
            */

            RTSP_Action_CaptureFrame,
            /*
            Captures a frame as snapshot. The format of the captured frame depends on the Option parameter:
            
            - file name:
            the next frame is captured in the format specified by the extension. The supported formats are: BMP, TIFF, PNG, JPG
            E.g. to capture a JPEG image:
            DatasteadRTSPSourceConfig.Action (RTSP_Action_CaptureFrame, "c:\folder\nextimage.jpg")
            
            - HBITMAP (keyword):
            the next frame is captured to a bitmap handle, and this bitmap handle is returned by an EC_RTSPNOTIFY (EC_RTSP_PARAM1_FRAME_CAPTURE_SUCCEEDED, BitmapHandle) notification event sent to the filter graph.
            E.g.:
            DatasteadRTSPSourceConfig.Action (RTSP_Action_CaptureFrame, "HBITMAP")
            
            note: do not delete the bitmap handle, it may be reused for the next capture and will be released by the filter
            */

            RTSP_Action_UpdateDuration,
            /*
            Reserved
            */

            //===========================
            // SOURCE URL
            //===========================

            RTSP_Source_RecordingFileName_str,
            /*
            [urlparam: recordingfilename]
            Sets the recording file name. Setting this property enables the recording of the RTSP stream to a file. The extension determines the format of the recording.
            
            The formats supported by the current version are:
            mp4, flv, mov, avi, mkv
            
            Examples:
            c:\folder\recfile.mp4
            c:\folder\recfile.flv
            c:\folder\recfile.mov
            c:\folder\recfile.avi
            c:\folder\recfile.mkv
            
            To configure the filter in recording mode without starting immediately the recording, set a nul file name without path with the desired extension, e.g.:
            nul.mp4
            Then, once the filter is running, when you want to really start the recording, just invoke:
            Action (RTSP_Action_RecordToNewFileNow, c:\folder\realfilename.mp4)
            to start writing to the file.
            
            Remarks:
            - the filter does not include an H264 encoder, it just saves the native H264 samples to the recording file.
            
            - if the audio recording is enabled, it encodes the audio stream to PCM, MP3 or AAC depending on the recording format selected.
            - if the recording file name is set while the filter is running, this closes the current file being recorded and starts saving to a new file on the fly.
            */

            RTSP_Source_RecordingBacktimedStartSeconds_int,
            /*
            [urlparam: backtimedstart]
            see the "Backtimed Recording" chapter of the manual
            */

            RTSP_Source_Recording_Title_str,
            /*
            [urlparam: title]
            Sets a title for the video clip (for containers that support this feature, like MP4)
            */

            RTSP_Source_PlayableWhileRecording_int,
            /*
            [urlparam: playablewhilerecording]
            0: the clip is not playable while recording (default)
            1: the clip is playable while recording if the container supports this possibility (like MP4 or ASF)
            2 : idem, different mode
            */

            RTSP_Source_ContinuousRecording_bool,
            /*
            [urlparam: continuousrecording]
            When enabled, the recording does not stop when the graph is stopped / restarted.The recording stops only when the graph is destroyed(default: disabled)
            */

            RTSP_Source_MaxAnalyzeDuration_int,
            /*
            [urlparam: maxanalyzeduration]
            Maximum duration of the anaysis of the stream during the initial connection, expressed in milliseconds, e.g. ">maxanalyzeduration = 1000"
            */

            RTSP_Source_AutoReconnect_bool,
            /*
            [urlparam: autoreconnect]
            Enables/disables the automatic reconnection of the filter. Default: enabled
            */

            RTSP_Source_NoTranscoding_bool,
            /*
            [urlparam: notranscoding]
            Records the audio stream "as is", instead of recompressing it to AAC. Default: false
            */

            RTSP_Source_DeviceLostTimeOut_int,
            /*
            [urlparam: devicelosttimeout]
            If no frame is received after this timeout (expressed in milliseconds, default = 10000) the auto reconnection (if autoreonnect=1) or device lost event (if autoreconnect=0) occurs (see the Auto reconnection chapter). Default: 10 sec. (10000)
            */

            RTSP_Source_BufferDuration_int,
            /*
            [urlparam: buffer]
            Specifies the buffering duration in milliseconds. Default: 0 if no audio, 1000 milliseconds if audio
            */

            RTSP_Source_LowDelay_int,
            /*
            [urlparam: lowdelay]
            Used to enable/disable the low delay mode.
            Enabling the low delay mode may help to reduce the latency.
            However, when enabled, in some rare cases the video may appear jerky. In this case this setting must be kept disabled to avoid the jerkiness.
            -1 : auto
            0: disabled
            1: enabled
            */

            RTSP_Source_SampleDeliveryMode_int,
            /*
            [urlparam: sampledeliverymode]
            Reserved
            */

            RTSP_Source_TimestampDelayMs_int,
            /*
            [urlparam: timestampdelayms]
            Adds (or remove if negative) the specified latency to the sample timestamps of the output pins
            */

            RTSP_Source_ConnectionTimeOut_int,
            /*
            [urlparam: timeout]
            Connection timeout in milliseconds
            Default: 20000 (20 seconds)
            */

            RTSP_Source_RTSPTransport_int,
            /*
            [urlparam: rtsptransport]
            RTSP transport mode:
            0: automatic
            1: tcp
            2: udp
            3: http
            4: udp_multicast
            */

            RTSP_Source_RTSPRange_str,
            /*
            [urlparam: rtsprange]
            Optional Rtsp range specification (e.g. to start playing a clip stored on the RTSP source at the specified date/time). E.g.: ">rtsprange=clock=20150217T153000Z-"
            */

            RTSP_Source_HTTPProxy_str,
            /*
            [urlparam: httpproxy]
            Specifies the http proxy to use, if needed, for the http/https URLs
            */

            RTSP_Source_MpegTS_Program_str,
            /*
            [urlparam: program]
            in a MPEG-TS stream with several programs, specifies the name of the program to use (by default the 1st program found is used)
            */

            RTSP_Source_Format_str,
            /*
            [urlparam: srcformat]
            Used to specify the input format for some HTTP URLs if the filter does not detect them properly.
            The possible values are:
            "mjpeg": IP camera, HTTP in JPG or MJPEG mode
            "mxg": IP camera, HTTP in MXPEG mode
            "jpeg:WidthxHeight": specifies the image dimensions when the RTSP stream is a MJPEG stream and the size is not properly detected by the filter
            */

            RTSP_Source_FrameRate_double,
            /*
            [urlparam: srcframerate]
            Used to specify the native frame rate of the video stream in the case it would not be properly detected (this has been reported with some video streams configured in Variable Bit Rate mode (VBR))
            */

            RTSP_Source_AverageTimePerFrame100ns_int,
            /*
            Retrieves the average time per video frame, expressed in 100ns units
            */

            RTSP_Source_DurationMs_int,
            /*
            Retrieves the duration of the clip or URL, if any (if the source is not a live source), expressed in milliseconds
            */

            RTSP_Source_Duration100ns_int64,
            /*
            Retrieves the duration of the clip or URL, if any (if the source is not a live source), expressed in 100 ns units
            */

            RTSP_Source_AuthUser_str,
            /*
            authentication user name, if required
            */

            RTSP_Source_AuthPassword_str,
            /*
            authentication password, if required
            */

            RTSP_Source_StreamInfo_str,
            /*
            Retrieves information about the streams
            Note: this is a "displayable" multi-line string, each line is separated CR/LF characters.
            */

            RTSP_Source_Metadata_str,
            /*
            Retrives the metadata as a string made of values separated by cr/lf characters
            */

            RTSP_Source_StartTime_int,
            /*
            [urlparam: starttime]
            If the source URL supports seeking, you can specify the start time expressed in milliseconds.
            E.g. if the start time should be 2 min 30 sec -> 2x60 + 30 = 150 seconds = 150000 milliseconds, invoke SetInt ("Source_StartTime_int", 150000)
            */

            RTSP_Source_Threads_int,
            /*
            [urlparam: threads]
            Number of threads assigned to the decoding (and eventually encoding) of the source.
            Default: 1
            0: auto
            */

            RTSP_Source_ThreadPriority_int,
            /*
            [urlparam: threadpriority]
            sets the priority of the decoding threads:
            0: THREAD_PRIORITY_NORMAL (default)
            1: THREAD_PRIORITY_ABOVE_NORMAL
            2: THREAD_PRIORITY_HIGHEST
            3: THREAD_PRIORITY_TIME_CRITICAL
            */

            RTSP_Source_IsURLConnected_bool,
            /*
            Returns true if the URL is connected.
            It returns false if:
            - the URL is not yet connected
            - the URL is reconnecting when AutoReconnect is enabled
            */

            RTSP_Source_GetAudioDevices_str,
            /*
            Retrieves the list of the DirectShow audio capture devices (microphone, line input, webcam mic., etc...) currently available on the PC.
            It is returned as a "displayable" string that contains the devices separated by a "\n" (line feed or chr(10) character), e.g.:
            Microphone (Realtek High Definition Audio)\nMicrophone (HD Webcam C525\nDecklink Audio Capture
            */

            RTSP_Source_SetAudioDevice_str,
            /*
            Sets the name of the audio capture device to use. The name must be one of the names returned by GetStr (RTSP_Source_GetAudioDevices_str,...)
            Setting this property invalidates the audio of the RTSP source or IP camera (if any), and selects the use of the specified audio capture device instead.
            */

            RTSP_Source_GetURL_str,
            /*
            retrives the current URL
            */

            RTSP_Source_GetState_int,
            /*
            returns the current source state. Possible values include:
            state_disconnected, state_connecting_async, state_connecting_sync, state_reconnecting, state_connected, state_previewing, state_recording_paused, state_recording_active
            */

            RTSP_Source_Axis_IrCutFilter_str,
            /*
            sets or retrieve the state of the IR Cut Filter of Axis cameras
            the supported values are: enabled / disabled / auto
            */

            //===========================
            // ONVIF RELATED (requires to be connected with an onvif://... URL syntax)
            //===========================

            RTSP_ONVIF_LastJPEGSnapshotBuffer_intptr,
            /*
            returns a pointer to the memory buffer containing the last ONVIF JPEG snapshot
            */

            RTSP_ONVIF_LastJPEGSnapshotSize_int,
            /*
            returns the size of the memory buffer containing the last ONVIF JPEG snapshot
            */

            RTSP_ONVIF_Info_Manufacturer_str,
            /*
            retrieves the name of the manufacturer of the IP camera or DVR (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_Model_str,
            /*
            retrieves the model of the IP camera or DVR (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_HardwareId_str,
            /*
            retrieves the hardware identifier of the IP camera or DVR (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_SerialNumber_str,
            /*
            retrieves the serial number of the IP camera or DVR (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_FirmwareVersion_str,
            /*
            retrieves the firmware version of the IP camera or DVR (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_PTZInfo_str,
            /*
            retrieves the PTZ information model of the IP camera or DVR, as a string of values separated by cr/lf characters (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_PTZLimits_str,
            /*
            retrieves the min/max values of Pan, Tilt or Zoom of the IP camera, as a string of values separated by cr/lf characters (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_PTZPresets_str,
            /*
            retrieves the list of the presets of the IP camera, as a string of values separated by cr/lf characters (requires to be connected with an onvif://... URL syntax)
            */

            RTSP_ONVIF_Info_MacAddress_str,
            /*
            retrieves the MAC address of the network interface of the camera
            */

            RTSP_ONVIF_Info_AuxiliaryCommands_str,
            /*
            retrieves the list of the auxiliary commands available for this camera, as a string made of words separated by "\r\n" characters
            */

            //===========================
            // VIDEO ENCODING (To record with a different video codec than the native codec of the video source - Requires more CPU)
            //===========================

            RTSP_VideoEncoder_Codec_str,
            /*
            [urlparam: vcodec]
            Specifies a video codec, to record in a different format, video size or bitrate (instead of the native codec format), e.g. "h264"
            */

            RTSP_VideoEncoder_BitRateKbps_int,
            /*
            [urlparam: vbitrate]
            Specifies the bitrate expressed in Kbps, if the a video codec has been specified for the recording (see RTSP_VideoEncoder_Codec_str)
            */

            RTSP_VideoEncoder_Quality_int,
            /*
            [urlparam: vquality]
            If specified and greater than 0, enables the VBR mode and specifies a quality value that depends on the codec (e.g. try values in the 1..100 range)
            */

            RTSP_VideoEncoder_GopSize_int,
            /*
            [urlparam: vgopsize]
            Specifies the key frame spacing (e.g. a Gop of 30 at 30fps creates a key frame every 30 frames).
            */

            RTSP_VideoEncoder_Profile_str,
            /*
            [urlparam: vprofile]
            Specifies a profile name for the encoder (e.g. "baseline" for H264)
            */

            RTSP_VideoEncoder_Cabac_bool,
            /*
            [urlparam: vcabac]
            Enables the cabac mode for the encoder (context-adaptative coding)
            */

            RTSP_VideoEncoder_Width_int,
            /*
            [urlparam: vwidth]
            Specifies the video width for the encoder
            */

            RTSP_VideoEncoder_Height_int,
            /*
            [urlparam: vheight]
            Specifies the video height for the encoder
            */

            //===========================
            // AUDIO ENCODING (To record with a different audio codec than the native codec of the video source)
            //===========================

            RTSP_AudioEncoder_Codec_str,
            /*
            [urlparam: acodec]
            Specifies a codec name for the audio encoding, e.g. "aac", "mp3", ...
            */

            RTSP_AudioEncoder_BitRateKbps_int,
            /*
            [urlparam: abitrate]
            Specifies a bitrate in Kpbs for the audio encoding
            */

            RTSP_AudioEncoder_SampleRate_int,
            /*
            [urlparam: asamplerate]
            Specifies a sample rate for the audio encoding (e.g. 44100, 22050, etc...)
            */

            //===========================
            // VIDEO OUTPUT PIN
            //===========================

            RTSP_VideoStream_Enabled_bool,
            /*
            [urlparam: videostreamenabled]
            Enables/disables the video decompression and the rendering of the video pin. Default: true
            */


            RTSP_VideoStream_Synchronized_bool,
            /*
            [urlparam: vidsync]
            If disabled, the filter removes the sample times, so the samples are rendered as fast as possible (the samples are not scheduled for rendering). Default: true
            */


            RTSP_VideoStream_Recorded_bool,
            /*
            [urlparam: videostreamrecorded]
            If the recording is enabled (by setting Source_RecordingFileName_str) and the RTSP URL outputs audio and video, allows record audio only by disabling the recording of the video stream. Default: true
            */

            RTSP_VideoStream_Decode_KeyFrames_Only_bool,
            /*
            [urlparam: set ">maxframerate=-1"]
            If true, only I-Frames are decoded. Can be enabled/disabled on the fly without stopping/restarting the graph. Saves decoding CPU by previewing at 1 fps or so, depending on the GOP size (key frame spacing). Default: false
            */

            RTSP_VideoStream_Index_int,
            /*
            [urlparam: videostreamindex]
            If the RTSP URL outputs more than 1 video stream, you can specify the index of the video stream to use (in the 0..n-1 range). Default: 0
            */

            RTSP_VideoStream_MpegTS_pid_str,
            /*
            [urlparam: vpid]
            in a MPEG-TS stream with several video streams, specifies the PID of the video stream to use (by default the 1st video stream found is used)
            */

            RTSP_VideoStream_PinFormat_str,
            /*
            [urlparam: videopinformat]
            By default the video pin can connect in RGB32 or RGB24 format.
            This property allows to force one of the following pin formats(not case-sensitive) RGB32, RGB24, RGB565, RGB555, NV12, UYVY, I420
            */

            RTSP_VideoStream_Width_int,
            /*
            [urlparam: width]
            Used to specify a non-default frame width for the video pin
            note: when the URL is connected, GetInt(RTSP_VideoStream_Width_int, Value) returns the video width of the decoded video stream
            */

            RTSP_VideoStream_Height_int,
            /*
            [urlparam: height]
            Used to specify a non-default frame height for the video pin
            note: when the URL is connected, GetInt(RTSP_VideoStream_Height_int, Value) returns the video height of the decoded video stream
            */

            RTSP_VideoStream_AspectRatio_double,
            /*
            [urlparam: aspectratio]
            Specifies how the aspect ratio of the video output pin is handled:
            0.0 -> applies the aspect ratio of the stream format, if specified
            1.0 -> use the width and height of the native video frame, "as is"
            other values -> applies the aspect ratio specified
            */

            RTSP_VideoStream_TopDown_bool,
            /*
            Makes the image of the video output pin "top down"
            */

            RTSP_VideoStream_MaxFrameRate_double,
            /*
            [urlparam: maxframerate]
            Used to specify the frame rate of the video pin. If this parameter is not specified the output frame rate is the native frame rate of the video stream
            Note: passing -1 as value let enable the keyframe-only decoding mode, only the key frames are decoded. In this case the frame rate depend on the key frame spacing of the IP camera or RTSP / RTMP source.
            */

            RTSP_VideoStream_Filter_str,
            /*
            [urlparam: videofilter]
            Specifies a Ffmpeg video filter to use, e.g.hflip for an horizontal flipping, vflip for a top - down image
            Note : depending on the context, some filters may not be  useable
            */

            RTSP_VideoStream_HWAcceleration_int,
            /*
            [urlparam: hwaccel]
            Enables hardware-accelerated decoding:
            0: no hardware acceleration
            1: dxva2 acceleration
            2: Intel QuickSync acceleration
            3: NVidia CUVID acceleration
            */

            RTSP_VideoStream_Deinterlacing_int,
            /*
            [urlparam: deint]
            Enables the deinterlacing:
            0: no deinterlacing (default)
            1: yadif deinterlacing
            2: w3fdif deinterlacing (consumes more CPU)
            */

            RTSP_VideoStream_ConfigureTextOverlay_str,
            /*
            [urlparam: textoverlay]
            Enables a text overlay
            To enable more than one overlay, invoke the function more than one time with a different overlay ID.
            Note: the overlay(s) must be enabled before loading the URL. If they must not be displayed immediately, set an empty string, then update it while the filter is running.
            The syntax is explained in the Text Overlay chapter of the PDF manual
            */

            RTSP_VideoStream_ConfigureHueBrightSat_str,
            /*
            [urlparam: brighthuesat]
            Enables the brightness/hue/saturation adjustment.
            Note: must be enabled before loading the URL. If they must not be applied immediately, set the default values (b=0,h=0,s=1), then update them while the filter is running.
            The syntax is explained in the Brightness/Hue/Saturation chapter of the PDF manual
            */

            RTSP_VideoStream_DelayMs_int,
            /*
            [urlparam: videodelay]
            Adds the specified latency (in milliseconds) to the video stream, relatively to the audio stream. Designed to have "manual" control over the audio/video sync.
            */

            //===========================
            // AUDIO OUTPUT PIN
            //===========================

            RTSP_AudioStream_Enabled_bool,
            /*
            [urlparam: audiostreamenabled]
            Enables/disables the audio decompression and the rendering of the audio pin. Default: true
            */

            RTSP_AudioStream_Recorded_bool,
            /*
            [urlparam: audiostreamrecorded]
            If the recording is enabled (by setting Source_RecordingFileName_str) and the RTSP URL outputs audio and video, allows record video only by disabling the recording of the audio stream. Default: true
            */

            RTSP_AudioStream_Index_int,
            /*
            [urlparam: audiostreamindex]
            If the RTSP URL outputs more than 1 audio stream, you can specify the index of the audio stream to use (in the 0..n-1 range). Default: 0
            */

            RTSP_AudioStream_MpegTS_pid_str,
            /*
            [urlparam: apid]
            in a MPEG-TS stream with several audio streams, specifies the PID of the audio stream to use (by default the 1st audio stream found is used)
            */

            RTSP_AudioStream_Filter_str,
            /*
            [urlparam: audiofilter]
            Specifies a Ffmpeg audio filter to use. Note: depending on the context, some filters may not be useable
            */

            RTSP_AudioStream_Volume_int,
            /*
            [urlparam: audiovolume]
            specifies a non-default audio volume, in the 0..65535 range (0 = muted)
            */

            RTSP_AudioStream_DelayMs_int,
            /*
            [urlparam: audiodelay]
            Adds the specified latency (in milliseconds) to the audio stream, relatively to the video stream. Designed to have "manual" control over the audio/video sync.
            */

            //===========================
            // FRAME CAPTURE
            //===========================

            RTSP_FrameCapture_Width_int,
            /*
            [urlparam: framecapturewidth]
            Specifies a non-default width for the next captured frame (by default the native width of the video frame is used)
            */

            RTSP_FrameCapture_Height_int,
            /*
            [urlparam: framecaptureheight]
            Specifies a non-default height for the next captured frame (by default the native height of the video frame is used)
            */

            RTSP_FrameCapture_Time_int,
            /*
            [urlparam: framecapturetime]
            Schedules the sream time the next frame will be captured, expressed in milliseconds
            */

            RTSP_FrameCapture_FileName_str,
            /*
            [urlparam: framecapturefilename]
            Specifies the full path and file name of the next frame to capture.
            The extension specifies the format, the supported formats are: BMP, JPG, PNG, TIFF, e.g. "c:\folder\nextframe.png"
            */

            RTSP_CurrentRecording_FileSizeKb_int,
            /*
            Returns the file size progress (in Kb) of the current recording
            */

            RTSP_CurrentRecording_ClipDurationMs_int,
            /*
            Returns the duration In milliseconds of the current recording
            */

            RTSP_CurrentRecording_VideoFrameCount_int,
            /*
            Returns the video frame count of the current recording
            */

            RTSP_CurrentRecording_FileName_str,
            /*
            Returns the file name of the current recording
            */

            RTSP_LastRecorded_FileSizeKb_int,
            /*
            Returns the file size in Kb of the last file recorded
            */

            RTSP_LastRecorded_ClipDurationMs_int,
            /*
            Returns the duration In milliseconds of the last file recorded
            */

            RTSP_LastRecorded_VideoFrameCount_int,
            /*
            Returns the video frame count of the last file recorded
            */

            RTSP_LastRecorded_FileName_str,
            /*
            Returns the name of the last file recorded
            */


            //============================================
            // RE-STREAMING (destination URL, encoding format of the destination URL)
            //============================================

            RTSP_Dest_URL_str,
            /*
            [urlparam: desturl]
            Sets the re-streaming URL.
            
            Examples (at the end of the RTSP URL)
            
            RTSP server on port 6000 (the IP address is the address of a network card on the PC running the filter)
            >desturl=rtspsrv://192.168.0.25:6000
            
            UDP unicast on port 5000 (the IP address is the IP address of the client PC
            >desturl=udp://192.168.0.200:5000
            
            UDP multicast on port 4000
            >desturl=udp://239.255.0.10:4000
            
            Programmatical example:
            DatasteadRTSPSourceConfig.SetStr ("RTSP_Dest_URL_str", "rtspsrv://192.168.0.25:6000")
            */

            RTSP_Dest_Video_BitRate_int,
            /*
            [urlparam: destvideobitrate]
            Sets the re-streaming video bit rate expressed in kb/s
            */

            RTSP_Dest_Video_Quality_int,
            /*
            [urlparam: destvideoquality]
            Sets the re-streaming video quality in the 0..31 range
            (-1 = disabled, 0 = best quality, other values decrease the quality)
            Note: setting a value enables the VBR encoding mode
            */

            RTSP_Dest_Video_KeyFrameInterval_int,
            /*
            [urlparam: destvideokeyframeinterval]
            Sets the key frame spacing (default 30)
            */

            //===========================
            // MISC.
            //===========================

            RTSP_Recording_MP4TagTimeUTC_bool,
            /*
            [urlparam: mp4tagutc]
            - if enabled, the tag time of the MP4 file is set as UTC to be Quicktime-compliant
            - if disabled, the tag time of the MP4 file is aet as local time, to be EXIF-compliant
            default: disabled
            */

            RTSP_Filter_Version_int,
            /*
            Retuns the filter version number
            */

            RTSP_Filter_Version_str,
            /*
            Retuns the filter version as string
            */

            RTSP_Filter_Build_int,
            /*
            Retuns the filter build number
            */

            RTSP_Filter_LicenseKey_str,
            /*
            Sets the license key
            */

        };

        public delegate void OpenURLAsyncCompletionCB (IntPtr Sender, IntPtr CustomParam, UInt32 Result);
		
        [ComImport,
            Guid("58D66E77-B19C-4F9A-8B47-DFE1CA87B642"),
            System.Security.SuppressUnmanagedCodeSecurity,
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSourceCallbackConfig
        {
            [PreserveSig] int SetAsyncOpenURLCallback(OpenURLAsyncCompletionCB CallbackFunctionPtr, IntPtr Sender, IntPtr CustomParam);
        }

        /** <summary> 
        <para>type of the sample callback function</para>
        */
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DatasteadRTSP_RawSampleCallback(IntPtr Sender, Int32 StreamNumber, IntPtr CodecName, Int64 SampleTime_Absolute, Int64 SampleTime_Relative, IntPtr Buffer, Int32 BufferSize, IntPtr InfoString);
        public delegate void DatasteadRTSP_VideoSampleCallback(IntPtr Sender, Int64 SampleTime_Absolute, Int64 SampleTime_Relative, IntPtr Buffer, Int32 BufferSize, Int32 VideoWidth, Int32 VideoHeight, Int32 BitCount, Int32 Stride);
        public delegate void DatasteadRTSP_AudioSampleCallback(IntPtr Sender, Int64 SampleTime_Absolute, Int64 SampleTime_Relative, IntPtr Buffer, Int32 BufferSize, Int32 SamplesPerSec, Int32 Channels, Int32 BitsPerSample);

        /** <summary> 
        <para>uncompressed audio and video sample callbacks</para>
        <para>Makes and return a copy of each sample uncompressed</para>
        */
        [ComImport,
            Guid("799485B3-0DA1-4F74-90E2-5684C9CD949B"),
            System.Security.SuppressUnmanagedCodeSecurity,
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSampleCallback
        {
            /** <summary> 
            <para>this function is used to configure the callback that occurs after invoking Action (RTSP_Action_OpenURLAsync, ...) when the URL opening completes. Parameters:</para>
            <para>DatasteadRTSPSampleCallback: address of your callback function</para>
            <para>Sender: pass a pointer to the current instance of your class, it will be returned as parameter when the callback occurs</para>
            */
            [PreserveSig]
            int SetRawSampleCallback(IntPtr SampleCallback, IntPtr Sender);
            int SetVideoRGBSampleCallback(IntPtr SampleCallback, IntPtr Sender);
            int SetAudioPCMSampleCallback(IntPtr SampleCallback, IntPtr Sender);
        }
        
        /** <summary> 
        <para>direct access to the uncompressed RGB video sample before it is delivered to the output pin</para>
        <para>From this callback you can modify the video sample (e.g. apply an overlay) before it is delived</para>
        */
        [ComImport,
            Guid("845F8BAF-A774-4C46-A17A-F037613B9FC2"),
            System.Security.SuppressUnmanagedCodeSecurity,
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDatasteadRTSPSampleCallback2
        {
            /** <summary> 
            <para>this function is used to configure the callback that occurs after invoking Action (RTSP_Action_OpenURLAsync, ...) when the URL opening completes. Parameters:</para>
            <para>DatasteadRTSPSampleCallback: address of your callback function</para>
            <para>Sender: pass a pointer to the current instance of your class, it will be returned as parameter when the callback occurs</para>
            */
            [PreserveSig]
            int SetVideoRGBOverlayCallback(IntPtr SampleCallback, IntPtr Sender);
        }
        
	}
}

