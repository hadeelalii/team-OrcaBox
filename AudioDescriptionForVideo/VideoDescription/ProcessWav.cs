using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDescription
{
    using System.Diagnostics;
    using System.IO;

    class ProcessWav
    {
        static void Main(string[] args)
        {
            string folderPath = "path/to/your/folder";

            // Get a list of all WAV files in the folder
            string[] wavFiles = Directory.GetFiles(folderPath, "*.wav");

            // Iterate through each WAV file
            foreach (string wavFile in wavFiles)
            {
                // Construct the FFmpeg command
                string ffmpegCommand = $"ffmpeg -i \"{wavFile}\" -af \"silencedetect=n=-50dB:d=0.5\" -f null -";

                // Execute the FFmpeg command
                Process ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = "cmd.exe";
                ffmpegProcess.StartInfo.Arguments = $"/C {ffmpegCommand}";
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.StartInfo.CreateNoWindow = true;
                ffmpegProcess.Start();

                // Read the FFmpeg output
                string output = ffmpegProcess.StandardOutput.ReadToEnd();
                ffmpegProcess.WaitForExit();

                // Parse the output to extract the silence duration
                double silenceDuration = ParseSilenceDuration(output);

                // Construct the final FFmpeg command to trim the silence
                string outputFilePath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(wavFile) + "_trimmed.wav");
                string trimCommand = $"ffmpeg -i \"{wavFile}\" -af \"atrim=0:{silenceDuration}\" \"{outputFilePath}\"";

                // Execute the trim command
                Process trimProcess = new Process();
                trimProcess.StartInfo.FileName = "cmd.exe";
                trimProcess.StartInfo.Arguments = $"/C {trimCommand}";
                trimProcess.Start();
                trimProcess.WaitForExit();
            }

            Console.WriteLine("Silence removal complete.");
        }

        static double ParseSilenceDuration(string ffmpegOutput)
        {
            // Parse the FFmpeg output to extract the silence duration
            string[] lines = ffmpegOutput.Split(Environment.NewLine);
            string durationLine = lines.FirstOrDefault(line => line.Contains("silence_duration"));
            string durationString = durationLine.Split(':')[1].Trim();

            if (double.TryParse(durationString, out double duration))
            {
                return duration;
            }

            throw new Exception("Failed to parse silence duration.");
        }
    }

}
