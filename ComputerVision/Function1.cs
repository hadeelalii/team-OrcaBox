using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Azure.AI.Vision.Common;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ComputerVision
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Process input data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string imageURL = data?.imageURL;

            // CV Key, Endpoint
            string endpoint = "https://codingjamcomputervision.cognitiveservices.azure.com/";
            string key = "ce5103dcddee492595cc316ecbb1e62a";

            // Initialize CV 4.0
            var serviceOptions = new VisionServiceOptions(
                endpoint,
                new AzureKeyCredential(key));

            using var imageSource = VisionSource.FromUrl(
                new Uri(imageURL));

            // Initialize features
            var analysisOptions = new ImageAnalysisOptions()
            {
                Features = ImageAnalysisFeature.Caption | ImageAnalysisFeature.Text | ImageAnalysisFeature.Tags | ImageAnalysisFeature.Objects,
                Language = "en"
            };

            // Analyze
            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);
            var result = analyzer.Analyze();

            string returnText = "";
            // If it is analyzed
            if (result.Reason == ImageAnalysisResultReason.Analyzed)
            {

                if (result.Caption != null)
                {
                    Console.WriteLine(" Caption:");
                    Console.WriteLine($"   \"{result.Caption.Content}\", Confidence {result.Caption.Confidence:0.0000}");
                }

                if (result.Objects != null)
                {
                    Console.WriteLine(" Objects:");
                    foreach (var detectedObject in result.Objects)
                    {
                        Console.WriteLine($"   \"{detectedObject.Name}\", Bounding box {detectedObject.BoundingBox}, Confidence {detectedObject.Confidence:0.0000}");
                    }
                }
                
                if (result.Text != null)
                {
                    Console.WriteLine($" Text:");
                    foreach (var line in result.Text.Lines)
                    {
                        string pointsToString = "{" + string.Join(',', line.BoundingPolygon.Select(pointsToString => pointsToString.ToString())) + "}";
                        Console.WriteLine($"   Line: '{line.Content}', Bounding polygon {pointsToString}");
                        returnText += " ";
                        returnText += line.Content;

                        foreach (var word in line.Words)
                        {
                            pointsToString = "{" + string.Join(',', word.BoundingPolygon.Select(pointsToString => pointsToString.ToString())) + "}";
                            Console.WriteLine($"     Word: '{word.Content}', Bounding polygon {pointsToString}, Confidence {word.Confidence:0.0000}");
                        }
                    }
                }

            }
            else
            {
                var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                Console.WriteLine(" Analysis failed.");
                Console.WriteLine($"   Error reason : {errorDetails.Reason}");
                Console.WriteLine($"   Error code : {errorDetails.ErrorCode}");
                Console.WriteLine($"   Error message: {errorDetails.Message}");
            }

            ResponseModel returnedModel = new ResponseModel();
            returnedModel.Caption = result.Caption.Content;
            returnedModel.Objects = new string[result.Objects.Count];
            returnedModel.ocrText = returnText;
            
            for (int i=0; i<result.Objects.Count; i++)
            {
                returnedModel.Objects[i] = result.Objects[i].Name;
            }

            int tagsCount = 0;
            for (int i = 0; i < result.Tags.Count; i++)
            {
                if (result.Tags[i].Confidence > 0.85)
                {
                    tagsCount++;
                }
            }
            returnedModel.tags = new string[tagsCount];

            for (int i=0; i < tagsCount; i++)
            {
                returnedModel.tags[i] = result.Tags[i].Name;
            }

            // Now for vision 3.2
            ComputerVisionClient client =
             new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
             { Endpoint = endpoint };

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>() {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
            ImageAnalysis results = await client.AnalyzeImageAsync(imageURL, visualFeatures: features);

            returnedModel.landmarks = new string[results.Categories.Count];
            Console.WriteLine("Categories:");
            foreach (var category in results.Categories)
            {
                Console.WriteLine($"{category.Name} with confidence {category.Score}");
            }
            Console.WriteLine();

            Console.WriteLine("Landmarks:");

            int counter = 0;
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Landmarks != null)
                {
                    foreach (var landmark in category.Detail.Landmarks)
                    {
                        returnedModel.landmarks[counter++] = landmark.Name;
                        Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
                        
                    }
                }
            }


            if (results.ImageType.ClipArtType >= 2)
            {
                returnedModel.imageType = "Clip Art";
            } else
            {
                if (results.ImageType.LineDrawingType >= 2)
                {
                    returnedModel.imageType = "Line Drawing";
                } else
                {
                    returnedModel.imageType = "Photo";
                }
            }

            if (results.Color.IsBWImg)
            {
                returnedModel.colorScheme = "The image is black and white";
            } else
            {
                returnedModel.colorScheme = "The dominant colors in the image are: " + string.Join(",", results.Color.DominantColors);


            }

            return new OkObjectResult(returnedModel);
            
        }
    }
}
