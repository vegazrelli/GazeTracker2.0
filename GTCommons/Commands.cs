using System.Windows;
using GTCommons.Commands;
using System;
using System.Collections.Generic;

namespace GTCommons
{
    public class GTCommands : Window
    {
        #region Variables 

        private static GTCommands instance;

        private readonly AutotuneCommands autotuneCommands;
        private readonly CalibrationCommands calibrationCommands;
        private readonly CameraCommands cameraCommands;
        private readonly SettingsCommands settingsCommands;
        private readonly TrackerViewerCommands videoViewerCommands;

        #endregion

        #region Events

        public static readonly RoutedEvent NetworkClientEvent = EventManager.RegisterRoutedEvent("NetworkClientEvent",
                                                                                                 RoutingStrategy.Bubble,
                                                                                                 typeof (RoutedEventHandler),
                                                                                                 typeof (GTCommands));

        public static readonly RoutedEvent TrackQualityEvent = EventManager.RegisterRoutedEvent("TrackStatsEvent",
                                                                                                RoutingStrategy.Bubble,
                                                                                                typeof (RoutedEventHandler),
                                                                                                typeof (GTCommands));

        #endregion

        #region Constructor

        private GTCommands()
        {
            settingsCommands = new SettingsCommands();
            calibrationCommands = new CalibrationCommands();
            autotuneCommands = new AutotuneCommands();
            videoViewerCommands = new TrackerViewerCommands();
            cameraCommands = new CameraCommands();
        }

        #endregion

        #region EventHandlers

        public event RoutedEventHandler OnNetworkClient
        {
            add { base.AddHandler(NetworkClientEvent, value); }
            remove { base.RemoveHandler(NetworkClientEvent, value); }
        }

        public event RoutedEventHandler OnTrackingQuality
        {
            add { base.AddHandler(TrackQualityEvent, value); }
            remove { base.RemoveHandler(TrackQualityEvent, value); }
        }

        #endregion

        #region Raise events

        public void NetworkClient()
        {
            var args1 = new RoutedEventArgs();
            args1 = new RoutedEventArgs(NetworkClientEvent, this);
            RaiseEvent(args1);
        }

        public void TrackQuality()
        {
            var args1 = new RoutedEventArgs();
            args1 = new RoutedEventArgs(TrackQualityEvent, this);
            RaiseEvent(args1);
        }

        #endregion

        #region Get/Set

        public SettingsCommands Settings
        {
            get { return settingsCommands; }
        }

        public CalibrationCommands Calibration
        {
            get { return calibrationCommands; }
        }

        public AutotuneCommands Autotune
        {
            get { return autotuneCommands; }
        }

        public TrackerViewerCommands TrackerViewer
        {
            get { return videoViewerCommands; }
        }

        public CameraCommands Camera
        {
            get { return cameraCommands; }
        }

        public static GTCommands Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GTCommands();
                }

                return instance;
            }
        }

        #endregion

		#region Public methods (parse and execute)

		public void ParseAndExecuteCommand(string command)
		{
			if (command == null) return;

			char[] seperator = { ' ' };
			string[] cmd = command.Split(seperator, 50);

			string cmdStr = cmd[0];
			string cmdParam1 = "";
            string cmdParam2 = "";

            if (cmd.Length == 2)
                cmdParam1 = cmd[1];

            else if (cmd.Length == 3)
            {
                cmdParam1 = cmd[1];
                cmdParam2 = cmd[2];
            }

			switch (cmdStr)
			{
				#region Calibration

				case Protocol.CalibrationStart:
					Calibration.Start();
					break;

				case Protocol.CalibrationAbort:
					Calibration.Abort();
					break;

                case Protocol.CalibrationParameters:
                    Calibration.Parameters(cmdParam1);
                    break;

                case Protocol.CalibrationAreaSize:
                    Calibration.AreaSize(int.Parse(cmdParam1), int.Parse(cmdParam2));
                    break;

                case Protocol.CalibrationValidate:
                    Calibration.Accept();
                    break;

                case Protocol.CalibrationQuality:
                    break;


				//    case Commands.CalibrationPointChange:
				//        //OnCalibrationPointChange(Int32.Parse(cmd[1])); // Next point number
				//        break;

				//    case Commands.CalibrationParameters:

				//        CalibrationParameters calParams = new CalibrationParameters();
				//        calParams.ExtractParametersFromString(cmdParam);

				//        if (OnCalibrationParameters != null)
				//            OnCalibrationParameters(calParams);

				//        break;


				//    //case CalibrationAreaSize:
				//    //    break;

				//    //case CalibratitonEnd:
				//    //    if (OnCalibratitonEnd != null)
				//    //        OnCalibratitonEnd();
				//    //    break;

				//    //case CalibrationCheckLevel:
				//    //    if (OnCalibrationCheckLevel != null)
				//    //        OnCalibrationCheckLevel(Int32.Parse(cmd[1]));
				//    //    break;

				case Protocol.CalibrationPoint:
					//Console.WriteLine("New calibration point from dedicated interface: " + command);

					//How many points have been buffeded?
					List<int> CalPointsIndex = new List<int>();
					for (int c = 0; c < cmd.Length; c++)
						if (cmd[c] == "CAL_POINT")
							CalPointsIndex.Add(c);

					for (int c = 0; c < CalPointsIndex.Count; c++)
					{
						//OnCalibrationFeedbackPoint(
						//    long.Parse(cmd[CalPointsIndex[c] + 1]),     //time
						//    int.Parse(cmd[CalPointsIndex[c] + 2]),      //packace number
						//    int.Parse(cmd[CalPointsIndex[c] + 3]),      //targetX
						//    int.Parse(cmd[CalPointsIndex[c] + 4]),      //targetY
						//    int.Parse(cmd[CalPointsIndex[c] + 5]),      //gazeX
						//    int.Parse(cmd[CalPointsIndex[c] + 6]),      //gazeY
						//    float.Parse(cmd[CalPointsIndex[c] + 7]),    //distance - will not be used
						//    int.Parse(cmd[CalPointsIndex[c] + 8]));     //acquisition time 
					}
					break;

				case Protocol.CalibrationUpdateMethod:
					Console.Write("Calibration Update Method Changed to:" + cmdParam1);
					//onCalibrationUpdateMethod(int.Parse(cmdParam));
					//Settings.Instance.Calibration.RecalibrationType = GazeTrackingLibrary.Utils.RecalibrationTypeEnum.Continuous
					break;

			    #endregion
			}
		}

		#endregion
    }
}