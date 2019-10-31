namespace GeoReVi
{
  using System;
  using System.Collections.Generic;
using System.Reflection;

  class Params
  {
    #region command line options
    public static string OptXSize = "-XSize";
    public static string OptYSize = "-YSize";
    public static string OptXDPI = "-XDPI";
    public static string OptYDPI = "-YDPI";

    public static string OptInputPath = "-Input";
    public static string OptOutPath = "-Output";

    public static string OptScaleMode = "-ScaleMode";
    public static string OptScaleModeOriginal = "Original";
    public static string OptScaleModeTarget = "Target";
    #endregion command line options

    #region message texts
    public static string ParamValueInvalidSizeMsg = "Command line parameter '{0}' is invalid because '{1}' is not a known option (this must be either '{2}' or '{3}').";
    public static string ParamValueInvalidStringMsg = "Command line parameter '{0}' is invalid because it has a zero length.";
    public static string ParamValueInvalidMsg = "Command line parameter '{0}' is invalid because the value '{1}' is invalid.";
    public static string ParamValueMissingMsg = "Command line parameter '{0}' is invalid because the value is missing.";

    public static string ToolUsageMsg = "XAML to PNG command line conversion tool.\n" +
                                        string.Format("Usage: {0}\n", Assembly.GetEntryAssembly().GetName().Name)

    + string.Format("{0} <size> - X size in pixels of converted png output image\n", Params.OptXSize)
    + string.Format("{0} <size> - Y size in pixels of converted png output image\n", Params.OptYSize)
    + string.Format("{0} <size> - Assumed X (Dots Per Inch) DPI of XAML input\n", Params.OptXDPI)
    + string.Format("{0} <size> - Assumed Y (Dots Per Inch) DPI of XAML input\n", Params.OptYDPI)

    + string.Format("{0} path to input XAML file or directory containing input XAML files - File name and path of XAML input file or path to directory containing XAML input files.\n", Params.OptInputPath)
    + string.Format("{0} path to ouput PNG file or directory that will contain ouput PNG files- File name and path of PNG output file or path to directory where PNG output files are to be saved.\n", Params.OptOutPath)

    + string.Format("{0} - Scale mode can be based on Original size or based on ouput size.\n", Params.OptScaleMode)
    + string.Format("  {0}\n", Params.OptScaleModeOriginal)
    + string.Format("  {0}\n", Params.OptScaleModeTarget);
    #endregion message texts

    #region Constructor
    public Params()
    {
      this.XSize = this.YSize = 64;           // Defaults for optional parameters
      this.XDPI = this.YDPI = 96;
      this.ThisScale = ScaleTO.TargetSize;
      this.InputPath = this.OutputPath = string.Empty;
                                             
                                             // Input parameter is the only required parameter
      this.InputPath = this.OutputPath = string.Empty;

      this.InputFiles = new List<string>();
      this.OutputFiles = new List<string>();
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// X Output size in pixels
    /// </summary>
    public int XSize { get; set; }

    /// <summary>
    /// Y Output size in pixels
    /// </summary>
    public int YSize { get; set; }

    /// <summary>
    /// X Dots per Image (DPI) assumed to convert input
    /// </summary>
    public int XDPI { get; set; }

    /// <summary>
    /// Y Dots per Image (DPI) assumed to convert input
    /// </summary>
    public int YDPI { get; set; }

    /// <summary>
    /// File name and path (or just directory) of input file(s)
    /// </summary>
    public string InputPath { get; set; }

    /// <summary>
    /// File name and path (or just directory) of output file(s)
    /// </summary>
    public string OutputPath { get; set; }

    /// <summary>
    /// List of input files.
    /// </summary>
    public List<string> InputFiles { get; set; }

    /// <summary>
    /// List of output files.
    /// </summary>
    public List<string> OutputFiles { get; set; }

    /// <summary>
    /// Parameter to determine different modes of scaling.
    /// </summary>
    public ScaleTO ThisScale { get; set; }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Parse command line options and return a command line <seealso cref="Params"/> object
    /// plus error code and string (if any).
    /// </summary>
    /// <param name="args"></param>
    /// <param name="progParams"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    static public int ParseCmdLine(string[] args, out Params progParams, out string strError)
    {
      strError = string.Empty;
      progParams = new Params();

      if (args.Length == 0)
      {
         strError = Params.ToolUsageMsg;
         return -2;
      }

      for (int i = 0; i < args.Length; i++)
      {
        int iRet = 0, iNumber;
        string sValue;

        if (args[i].ToUpper() == OptXSize.ToUpper())
        {
          if ((iRet = Params.GetIntValue(args, i, OptXSize, out iNumber, out strError)) != 0)
            return iRet;

          progParams.XSize = iNumber;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptYSize.ToUpper())
        {
          if ((iRet = Params.GetIntValue(args, i, OptYSize, out iNumber, out strError)) != 0)
            return iRet;

          progParams.YSize = iNumber;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptXDPI.ToUpper())
        {
          if ((iRet = Params.GetIntValue(args, i, OptXDPI, out iNumber, out strError)) != 0)
            return iRet;

          progParams.XDPI = iNumber;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptYDPI.ToUpper())
        {
          if ((iRet = Params.GetIntValue(args, i, OptYDPI, out iNumber, out strError)) != 0)
            return iRet;

          progParams.YDPI = iNumber;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptInputPath.ToUpper())
        {
          if ((iRet = Params.GetStringValue(args, i, OptInputPath, out sValue, out strError)) != 0)
            return iRet;

          progParams.InputPath = sValue;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptOutPath.ToUpper())
        {
          if ((iRet = Params.GetStringValue(args, i, OptInputPath, out sValue, out strError)) != 0)
            return iRet;

          progParams.OutputPath = sValue;

          i++;
          continue;
        }

        if (args[i].ToUpper() == OptScaleMode.ToUpper())
        {
          ScaleTO thisScale;

          if ((iRet = Params.GetScaleValue(args, i, OptScaleMode, out thisScale, out strError)) != 0)
            return iRet;

          progParams.ThisScale = thisScale;

          i++;
          continue;
        }

        strError = string.Format("Unknown parameter: '{0}'", args[i]) + "\n\n" + Params.ToolUsageMsg;
        return -1;
      }

      try 
	    {	        
        if (System.IO.File.Exists(progParams.InputPath) == false)
        {
          if (System.IO.Directory.Exists(progParams.InputPath) == true || System.IO.Directory.Exists(progParams.InputPath + "\\") == true)
          {
            // Scan directory and Convert many source file(s) into many destination file(s)
            return ScanDirectoryAddFiles(progParams, "*.xaml", out strError);
          }
          else
          {
            strError = string.Format("Input path '{0}' does not point to an existing XAML file.", progParams.InputPath);
            return -12;
          }
        }
	    }
	    catch
	    {
        strError = string.Format("Input path '{0}' does not point to an existing XAML file.", progParams.InputPath);
        return -13;
      }

      int iReturn;
      string strOutpath;
      if ((iReturn = GetOutputPath(progParams, progParams.InputPath, out strOutpath, out strError)) != 0)
        return iReturn;

      // Convert just one source file into one destination file
      progParams.InputFiles.Add(progParams.InputPath);
      progParams.OutputFiles.Add(progParams.OutputPath);

      for (int i = 0; i < progParams.InputFiles.Count; i++)
      {
        if (progParams.InputFiles[i].ToUpper() == progParams.OutputFiles[i].ToUpper())
        {
          strError = string.Format("Output and Input path must not be equal ('{0}', '{1}').", progParams.InputFiles[i], progParams.OutputFiles[i]);
          return -11;
        }          
      }

      return 0;
    }

    /// <summary>
    /// Scan input directory and generate a collection of input and output files stored in <paramref name="progParams"/>.
    /// </summary>
    /// <param name="progParams"></param>
    /// <param name="searchPattern"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    public static int ScanDirectoryAddFiles(Params progParams, string searchPattern, out string strError)
    {
      strError = string.Empty;

      // Scan directory and convert many source file(s) into many destination file(s)
      string[] dirs = System.IO.Directory.GetFiles(progParams.InputPath, searchPattern);

      foreach (string inputFile in dirs)
      {
        int iRet;
        string strOutPath;
        if ((iRet = GetOutputPath(progParams, inputFile, out strOutPath, out strError)) != 0)
          return iRet;

        progParams.InputFiles.Add(inputFile);
        progParams.OutputFiles.Add(strOutPath);
      }

      return 0;
    }

    /// <summary>
    /// Determine the output path in dependence of whether the:
    /// 1) OutputPath has length of zero     -> Output is equal input (with *.png extension instead of *.xaml)
    /// 2) OutputPath points to a directory. -> Output is based on output directory plus file name from input plus *.xaml extension
    /// 3) OutputPath points to a file.      -> Output is written to this file.
    /// </summary>
    /// <param name="progParams"></param>
    /// <param name="inputPath"></param>
    /// <param name="strOutpath"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    public static int GetOutputPath(Params progParams, string inputPath, out string strOutpath, out string strError)
    {
      strError = strOutpath = string.Empty;

      // Compute output based on input but with different file extension "*.png"
      if (progParams.OutputPath.Length == 0)
      {
        try
        {
          string sFileName = System.IO.Path.GetFileNameWithoutExtension(inputPath) + ".png";
          strOutpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(inputPath), sFileName);
        }
        catch
        {
          strError = string.Format("Input path '{0}' is not valid.", inputPath);
          return -14;
        }
      }
      else
      {
        try
        {
          // Complete output path with output filename if the path points at an existing directory
          if (System.IO.Directory.Exists(progParams.OutputPath) == true)
          {
            string sFileName = System.IO.Path.GetFileNameWithoutExtension(inputPath) + ".png";
            strOutpath = System.IO.Path.Combine(progParams.OutputPath, sFileName);
          }
          else
            strOutpath = progParams.OutputPath;
        }
        catch
        {
          strError = string.Format("Output path '{0}' is not valid directory ouput path.", inputPath);
          return -15;
        }
      }

      return 0;
    }

    /// <summary>
    /// Standard method to convert command line parameter string to int datatype.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="CurrIndex"></param>
    /// <param name="ParamName"></param>
    /// <param name="intNumber"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    public static int GetIntValue(string[] args,
                                  int CurrIndex,
                                  string ParamName,
                                  out int intNumber,
                                  out string strError)
    {
      strError = string.Empty;
      intNumber = -1;

      if (args.Length <= (CurrIndex + 1))
      {
        strError = string.Format(ParamValueMissingMsg, ParamName);
        return -1;
      }

      try
      {
        intNumber = int.Parse(args[CurrIndex + 1]);
      }
      catch (Exception)
      {
        strError = string.Format(ParamValueInvalidMsg, ParamName, args[CurrIndex + 1]);
        return -2;
      }

      return 0;
    }

    /// <summary>
    /// Standard method to convert command line parameter into string datatype.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="CurrIndex"></param>
    /// <param name="ParamName"></param>
    /// <param name="stringValue"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    public static int GetStringValue(string[] args,
                                     int CurrIndex,
                                     string ParamName,
                                     out string stringValue,
                                     out string strError)
    {
      strError = string.Empty;
      stringValue = string.Empty;

      if (args.Length <= (CurrIndex + 1))
      {
        strError = string.Format(ParamValueMissingMsg, ParamName);
        return -1;
      }

      stringValue = args[CurrIndex + 1];

      if (stringValue.Trim().Length == 0)
      {
        strError = string.Format(ParamValueInvalidStringMsg, ParamName);
        return -1;
      }

      return 0;
    }

    /// <summary>
    /// Standard method to convert command line parameter string into an enaum <seealso cref="ScaleTO"/> datatype.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="CurrIndex"></param>
    /// <param name="ParamName"></param>
    /// <param name="scaleValue"></param>
    /// <param name="strError"></param>
    /// <returns></returns>
    public static int GetScaleValue(string[] args,
                                    int CurrIndex,
                                    string ParamName,
                                    out ScaleTO scaleValue,
                                    out string strError)
    {
      strError = string.Empty;
      scaleValue = ScaleTO.OriginalSize;

      if (args.Length <= (CurrIndex + 1))
      {
        strError = string.Format(ParamValueMissingMsg, ParamName);
        return -1;
      }

      string stringValue = args[CurrIndex + 1];

      if (stringValue.Trim().Length == 0)
      {
        strError = string.Format(ParamValueInvalidStringMsg, ParamName);
        return -1;
      }

      if (stringValue.ToUpper() == OptScaleModeOriginal.ToUpper())
      {
        scaleValue = ScaleTO.OriginalSize;
        return 0;
      }

      if (stringValue.ToUpper() == OptScaleModeTarget.ToUpper())
      {
        scaleValue = ScaleTO.TargetSize;
        return 0;
      }

      strError = string.Format(ParamValueInvalidSizeMsg, ParamName, stringValue, OptScaleModeOriginal, OptScaleModeTarget);
      return -1;
    }
    #endregion Methods
  }
}
