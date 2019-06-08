using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Properties;
using BenchmarkDotNet.Reports;

namespace Miki.Serialization.Benchmark.Exporter
{
    /// <summary>
    ///     The custom RPlot exporter.
    ///     Changes: Export only the Barplot and Boxplot.
    ///     Source: https://github.com/dotnet/BenchmarkDotNet/blob/master/src/BenchmarkDotNet/Exporters/RPlotExporter.cs
    /// </summary>
    public class CustomRPlotExporter : IExporter
    {
        private const string Template = @"BenchmarkDotNetVersion <- ""$BenchmarkDotNetVersion$ ""
dir.create(Sys.getenv(""R_LIBS_USER""), recursive = TRUE, showWarnings = FALSE)
list.of.packages <- c(""ggplot2"", ""dplyr"", ""gdata"", ""tidyr"", ""grid"", ""gridExtra"", ""Rcpp"")
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[,""Package""])]
if(length(new.packages)) install.packages(new.packages, lib = Sys.getenv(""R_LIBS_USER""), repos = ""https://cran.rstudio.com/"")
library(ggplot2)
library(dplyr)
library(gdata)
library(tidyr)
library(grid)
library(gridExtra)

ends_with <- function(vars, match, ignore.case = TRUE) {
  if (ignore.case)
    match <- tolower(match)
  n <- nchar(match)

  if (ignore.case)
    vars <- tolower(vars)
  length <- nchar(vars)

  substr(vars, pmax(1, length - n + 1), length) == match
}
std.error <- function(x) sqrt(var(x)/length(x))
cummean <- function(x) cumsum(x)/(1:length(x))
BenchmarkDotNetVersionGrob <- textGrob(BenchmarkDotNetVersion, gp = gpar(fontface=3, fontsize=10), hjust=1, x=1)
nicePlot <- function(p) grid.arrange(p, bottom = BenchmarkDotNetVersionGrob)
printNice <- function(p) print(nicePlot(p))
ggsaveNice <- function(fileName, p, ...) {
  cat(paste0(""*** Plot: "", fileName, "" ***\n""))
  ggsave(fileName, plot = nicePlot(p), ...)
  cat(""------------------------------\n"")
}

args <- commandArgs(trailingOnly = TRUE)
files <- if (length(args) > 0) args else list.files()[list.files() %>% ends_with(""-measurements.csv"")]
for (file in files) {
  title <- gsub(""-measurements.csv"", """", basename(file))
  measurements <- read.csv(file, sep = ""$CsvSeparator$"")

  result <- measurements %>% filter(Measurement_IterationStage == ""Result"")
  if (nrow(result[is.na(result$Job_Id),]) > 0)
    result[is.na(result$Job_Id),]$Job_Id <- """"
  if (nrow(result[is.na(result$Params),]) > 0) {
    result[is.na(result$Params),]$Params <- """"
  } else {
    result$Job_Id <- trim(paste(result$Job_Id, result$Params))
  }
  result$Job_Id <- factor(result$Job_Id, levels = unique(result$Job_Id))

  timeUnit <- ""ns""
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- ""us""
  }
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- ""ms""
  }
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- ""sec""
  }

  resultStats <- result %>%
    group_by_(.dots = c(""Target_Method"", ""Job_Id"")) %>%
    summarise(se = std.error(Measurement_Value), Value = mean(Measurement_Value))

  benchmarkBoxplot <- ggplot(result, aes(x=Target_Method, y=Measurement_Value, fill=Job_Id)) +
    guides(fill=guide_legend(title=""Job"")) +
    xlab(""Target"") +
    ylab(paste(""Time,"", timeUnit)) +
    ggtitle(title) +
    geom_boxplot()
  benchmarkBarplot <- ggplot(resultStats, aes(x=Target_Method, y=Value, fill=Job_Id)) +
    guides(fill=guide_legend(title=""Job"")) +
    xlab(""Target"") +
    ylab(paste(""Time,"", timeUnit)) +
    ggtitle(title) +
    geom_bar(position=position_dodge(), stat=""identity"")
    #geom_errorbar(aes(ymin=Value-1.96*se, ymax=Value+1.96*se), width=.2, position=position_dodge(.9))

  printNice(benchmarkBoxplot)
  printNice(benchmarkBarplot)
  ggsaveNice(gsub(""-measurements.csv"", ""-boxplot.png"", file), benchmarkBoxplot)
  ggsaveNice(gsub(""-measurements.csv"", ""-barplot.png"", file), benchmarkBarplot)
  }
}";

        public static readonly IExporter Default = new CustomRPlotExporter();
        public string Name => nameof(CustomRPlotExporter);

        private static readonly object BuildScriptLock = new object();

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            const string scriptFileName = "BuildPlots.R";
            const string logFileName = "BuildPlots.log";
            yield return scriptFileName;

            string fileNamePrefix = Path.Combine(summary.ResultsDirectoryPath, summary.Title);
            MethodInfo method = typeof(CsvMeasurementsExporter).GetMethod("GetArtifactFullName", BindingFlags.NonPublic | BindingFlags.Instance);
            string csvFullPath = (string)method.Invoke(CsvMeasurementsExporter.Default, new object[] { summary });

            string scriptFullPath = Path.Combine(summary.ResultsDirectoryPath, scriptFileName);
            string logFullPath = Path.Combine(summary.ResultsDirectoryPath, logFileName);
            string script = Template.
                Replace("$BenchmarkDotNetVersion$", BenchmarkDotNetInfo.FullTitle).
                Replace("$CsvSeparator$", CsvMeasurementsExporter.Default.Separator);
            lock (BuildScriptLock)
                File.WriteAllText(scriptFullPath, script);

            string rscriptExecutable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Rscript.exe" : "Rscript";
            string rscriptPath;
            string rHome = Environment.GetEnvironmentVariable("R_HOME");
            if (rHome != null)
            {
                rscriptPath = Path.Combine(rHome, "bin", rscriptExecutable);
                if (!File.Exists(rscriptPath))
                {
                    consoleLogger.WriteLineError($"RPlotExporter requires R_HOME to point to the directory containing bin{Path.DirectorySeparatorChar}{rscriptExecutable} (currently points to {rHome})");
                    yield break;
                }
            }
            else // No R_HOME, try the path
            {
                rscriptPath = FindInPath(rscriptExecutable);
                if (rscriptPath == null)
                {
                    consoleLogger.WriteLineError($"RPlotExporter couldn't find {rscriptExecutable} in your PATH and no R_HOME environment variable is defined");
                    yield break;
                }
            }

            var start = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = rscriptPath,
                WorkingDirectory = summary.ResultsDirectoryPath,
                Arguments = $"\"{scriptFullPath}\" \"{csvFullPath}\""
            };
            using (var process = Process.Start(start))
            {
                string output = process?.StandardOutput.ReadToEnd() ?? "";
                string error = process?.StandardError.ReadToEnd() ?? "";
                File.WriteAllText(logFullPath, output + Environment.NewLine + error);
                process?.WaitForExit();
            }

            yield return fileNamePrefix + "-boxplot.png";
            yield return fileNamePrefix + "-barplot.png";
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
            throw new NotSupportedException();
        }

        private static string FindInPath(string fileName)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            if (path == null)
                return null;

            var dirs = path.Split(Path.PathSeparator);
            foreach (string dir in dirs)
            {
                string trimmedDir = dir.Trim('\'', '"');
                try
                {
                    string filePath = Path.Combine(trimmedDir, fileName);
                    if (File.Exists(filePath))
                        return filePath;
                }
                catch (Exception)
                {
                    // Never mind
                }
            }
            return null;
        }
    }
}
