using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using VideoDescription;
using static VideoDescription.SceneData;
class Program
{
    
    static void Main()
    {
        string filePath = @"C:\Users\t-sofiayang\source\repos\VideoDescription\RawData.json"; // Replace with your JSON file name
        string imageSaveFileFolder = @"C:\Users\t-sofiayang\Pictures\CodingJamImageExtract";
        string videoFilePath = @"C:\Users\t-sofiayang\source\repos\VideoDescription\Video.mp4";

        try
        {
            string json = File.ReadAllText(filePath);
            JObject jsonObject = JObject.Parse(json);

            JArray scenesArray = (JArray)jsonObject["videos"][0]["insights"]["scenes"];
            List<SceneData> sceneDataList = ExtractSceneData(scenesArray);

            // Print the extracted scene data
            int count = 0;
            foreach (SceneData sceneData in sceneDataList)
            {
                Console.WriteLine("Start: " + sceneData.Start);
                Console.WriteLine("End: " + sceneData.End);
                /*
                string curStr = imageSaveFileFolder + @"\" + count + ".png";
                count++;
                SaveImageFromVideo(videoFilePath, curStr, sceneData.Start); */
                Console.WriteLine();
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found: " + filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        try
        {
            List<TranscriptData> transcriptDataList = ExtractTranscriptData(filePath);

            // Printing the extracted transcript data
            foreach (TranscriptData transcriptData in transcriptDataList)
            {
                Console.WriteLine("Id: " + transcriptData.Id);
                Console.WriteLine("Text: " + transcriptData.Text);
                Console.WriteLine("Confidence: " + transcriptData.Confidence);
                Console.WriteLine("SpeakerId: " + transcriptData.SpeakerId);
                Console.WriteLine("Language: " + transcriptData.Language);
                Console.WriteLine("Start: " + transcriptData.Start);
                Console.WriteLine("End: " + transcriptData.End);
                Console.WriteLine();
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found: " + filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }


    }

    static List<TranscriptData> ExtractTranscriptData(string filePath)
    {
        string json = File.ReadAllText(filePath);
        JObject jsonObject = JObject.Parse(json);

        JArray transcriptArray = (JArray)jsonObject["videos"][0]["insights"]["transcript"];

        List<TranscriptData> transcriptDataList = new List<TranscriptData>();

        foreach (JObject transcriptObject in transcriptArray)
        {
            int id = (int)transcriptObject["id"];
            string text = (string)transcriptObject["text"];
            double confidence = (double)transcriptObject["confidence"];
            int speakerId = (int)transcriptObject["speakerId"];
            string language = (string)transcriptObject["language"];

            JToken instanceToken = transcriptObject["instances"][0];
            TimeSpan start = TimeSpan.Parse(instanceToken["start"].ToString());
            TimeSpan end = TimeSpan.Parse(instanceToken["end"].ToString());

            TranscriptData transcriptData = new TranscriptData
            {
                Id = id,
                Text = text,
                Confidence = confidence,
                SpeakerId = speakerId,
                Language = language,
                Start = start,
                End = end
            };

            transcriptDataList.Add(transcriptData);
        }

        return transcriptDataList;
    }

    // Check if a time is between two other times.
    static bool IsTimeBetween(TimeSpan startTime, TimeSpan endTime, TimeSpan checkTime)
    {
        if (endTime < startTime)
        {
            // Adjust endTime if it represents the next day
            endTime = endTime.Add(TimeSpan.FromDays(1));
        }

        return (checkTime >= startTime) && (checkTime <= endTime);
    }


    // Extracting all scenes
    static List<SceneData> ExtractSceneData(JArray scenesArray)
    {
        List<SceneData> sceneDataList = new List<SceneData>();

        foreach (JObject sceneObject in scenesArray)
        {
            JToken startToken = sceneObject["instances"][0]["start"];
            JToken endToken = sceneObject["instances"][0]["end"];

            TimeSpan start = TimeSpan.Parse(startToken?.ToString());
            TimeSpan end = TimeSpan.Parse(endToken?.ToString());

            SceneData sceneData = new SceneData
            {
                Start = start,
                End = end
            };

            sceneDataList.Add(sceneData);
        }

        return sceneDataList;
    }

    // Extracting all Keyframe
    static void SaveImageFromVideo(string videoFilePath, string outputImagePath, TimeSpan timestamp)
    {
        string ffmpegPath = @"C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg\bin\ffmpeg.exe"; // Provide the path to the FFmpeg executable
        string timestampString = timestamp.ToString(@"hh\:mm\:ss"); // Convert the TimeSpan to FFmpeg's timestamp format

        // Run FFmpeg command to extract the image
        string arguments = $"-ss {timestampString} -i \"{videoFilePath}\" -frames:v 1 -q:v 2 \"{outputImagePath}\"";
        ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
