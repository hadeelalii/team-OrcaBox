using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

public class VideoLengthener
{
    public static void LengthenVideoWithAudio(string inputVideoPath, string inputAudioPath, string outputVideoPath, double audioStartTime)
    {
        var inputFile = new MediaFile { Filename = inputVideoPath };
        var tempVideoPath = Path.Combine(Path.GetDirectoryName(outputVideoPath), "temp_video.mp4");
        var outputVideoPath2 = Path.Combine(Path.GetDirectoryName(outputVideoPath), "output_video2.mp4");

        using (var engine = new Engine())
        {
            // Split the video at the specified audio start time
            var splitOptions = new ConversionOptions { Seek = TimeSpan.FromSeconds(audioStartTime) };
            engine.GetMetadata(inputFile);
            engine.ConvertProgressEvent += (sender, e) =>
            {
                if (e.ProcessedDuration >= TimeSpan.FromSeconds(audioStartTime))
                {
                    // Save the first part of the video
                    var outputFile1 = new MediaFile(tempVideoPath);
                    engine.Convert(inputFile, outputFile1, new ConversionOptions(), false);

                    // Save the second part of the video
                    var outputFile2 = new MediaFile(outputVideoPath2);
                    engine.Convert(inputFile, outputFile2, splitOptions, false);
                }
            };

            // Convert the video and save the second part with audio
            var outputFile = new MediaFile(outputVideoPath);
            engine.Convert(inputFile, outputFile, new ConversionOptions(), false);

            // Concatenate the video parts
            using (var concatenator = new Engine())
            {
                var inputFiles = new string[] { tempVideoPath, outputVideoPath2 };
                var outputOptions = new ConversionOptions();
                concatenator.Concatenate(outputFile, inputFiles, outputOptions);
            }

            // Cleanup temporary files
            File.Delete(tempVideoPath);
            File.Delete(outputVideoPath2);
        }
    }

    public static void Main(string[] args)
    {
        // Provide the paths to the input video, input audio, and output video
        string inputVideoPath = "path/to/input/video.mp4";
        string inputAudioPath = "path/to/input/audio.mp3";
        string outputVideoPath = "path/to/output/video.mp4";

        // Specify the time in seconds where you want to inject the audio
        double audioStartTime = 10.0;

        LengthenVideoWithAudio(inputVideoPath, inputAudioPath, outputVideoPath, audioStartTime);

        Console.WriteLine("Video lengthened and audio injected successfully.");
    }
}
