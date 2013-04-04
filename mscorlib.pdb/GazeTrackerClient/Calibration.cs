using System;

namespace GazeTrackerClient
{
    public class Calibration
    {
        private readonly CalibrationParameters calParams;
        private readonly Client client;

        #region Events

        #region Delegates

        public delegate void AbortHandler(bool success);

        public delegate void CheckLevelHandler(int newCheckLevel);

        public delegate void EndHandler(int quality);

        public delegate void ParametersHandler(CalibrationParameters calParams);

        public delegate void PointChangeHandler(string nextPointNumber);

        public delegate void QualityHandler(int quality);

        public delegate void StartHandler(int numberOfPoints);

        public delegate void ValidateHandler(
            string eye, double rootSqrMeanX, double rootSqrMeanY, double rootSqrMeanDeviation);

        #endregion

        public event AbortHandler OnAbort;

        public event StartHandler OnStart;

        public event PointChangeHandler OnPointChange;

        public event ParametersHandler OnParameters;

        public event EndHandler OnEnd;

        public event CheckLevelHandler OnCheckLevel;

        public event ValidateHandler OnValidate;

        public event QualityHandler OnQuality;

        #endregion

        public Calibration(Client cli)
        {
            client = cli;

            // Set default parameters
            calParams = new CalibrationParameters();
            calParams.SetDefaultParameters();
        }

        #region Commands Sending/Outgoing

        public void Start()
        {
            client.SendCommand(Commands.CalibrationStart);
        }

        public void Abort()
        {
            client.SendCommand(Commands.CalibrationAbort);
        }

        public void CalibrationParameters(CalibrationParameters calParams)
        {
            client.SendCommand(Commands.CalibrationParameters, calParams.ParametersAsString);
        }

        public void AcceptPoint()
        {
            client.SendCommand(Commands.CalibrationAcceptPoint);
        }

        public void AreaSize(int width, int height)
        {
            client.SendCommand(Commands.CalibrationAreaSize, width + " " + height);
        }

        public void Default()
        {
            calParams.SetDefaultParameters();
            CalibrationParameters(calParams);
        }

        /// <summary>
        /// Checklevel for the calibration (0=none, 1=weak, 2=medium, 3=strong)
        /// </summary>
        /// <param name="checkLevel">0=none, 1=weak, 2=medium, 3=strong</param>
        public void CheckLevel(int checkLev)
        {
            client.SendCommand(Commands.CalibrationCheckLevel, checkLev.ToString());
        }

        public void Point(int x, int y)
        {
            client.SendCommand(Commands.CalibrationPoint, x + " " + y + " ");
        }

        public void StartDriftCorrection()
        {
            client.SendCommand(Commands.CalibrationStartDriftCorrection);
        }

        public void Validate()
        {
            client.SendCommand(Commands.CalibrationValidate);
        }

        public void Quality()
        {
            client.SendCommand(Commands.CalibrationQuality);
        }

        public void PointChange()
        {
            client.SendCommand(Commands.CalibrationPointChange);
        }

        #endregion

        #region Commands Incoming

        public void ExtractDataAndRaiseEvent(string data)
        {
            char[] seperator = {' '};
            string[] cmd = data.Split(seperator, 5);
            string command = cmd[0];

            string parameter1 = "";
            string parameter2 = "";


            if (cmd.Length == 2)
                parameter1 = cmd[1];

            else
            {
                parameter1 = cmd[1];
                parameter2 = cmd[2];
            }

          

            switch (command)
            {
                case Commands.CalibrationStart:
                    if (OnStart != null)
                        OnStart(Int32.Parse(parameter1)); // Number of points
                    break;

                case Commands.CalibrationAbort:
                    if (OnAbort != null)
                        OnAbort(true);
                    break;

                case Commands.CalibrationPoint:
                    if (OnPointChange != null)
                    OnPointChange(parameter1 + " " + parameter2); // Next point number
                    break;

                /*case Commands.CalibrationPointChange:
                    if (OnPointChange != null)
                    OnPointChange(parameter1 + " " + parameter2); // Next point number
                    break;
                */
                case Commands.CalibrationParameters:
                    if (OnParameters != null)
                    {
                        var calParams = new CalibrationParameters();
                        calParams.ExtractParametersFromString(parameter1);
                        OnParameters(calParams);
                    }
                    break;

                case Commands.CalibrationEnd:
                    if (OnEnd != null)
                        OnEnd(Int32.Parse(parameter1));
                    break;

                case Commands.CalibrationCheckLevel:
                    if (OnCheckLevel != null)
                        OnCheckLevel(Int32.Parse(parameter1));
                    break;

                case Commands.CalibrationValidate:
                    if (OnValidate != null)
                        OnValidate(cmd[1], double.Parse(cmd[2]), double.Parse(cmd[3]), double.Parse(cmd[4]));
                    break;
            }
        }

        #endregion
    }
}