# Microsoft 2023 Intern Coding Jam (Team OrcaBox)

# About

Our tool is a web browser extension that is triggered once an image is clicked on, the image URL gets saved and then using Azure ComputerVision a text description of the image is generated. After this, we implemented Azure text-to-speech and used a couple of Azure functions to play the audio file generated from Azure through the extension so it can be heard by the user. The extension basically uses AI to describe what is happening in an image, then uses Azure services to generate an audio file that gets outputted by the extension.


# Overview

Demo Video: [Video Link](https://www.youtube.com/watch?v=nmo-BexDSmE)

Powerpoint Presentation: [PPTX Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/VisuaSpeak.pptx)

Business Requirement Document / Proposal [PDF Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/OrcaBox%20Proposal.pdf)

Dev Spec: [PDF Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/Dev%20Spec.pdf)

Demo of the CognitiveService-based Features: [LINK](https://fredinvazquez.github.io/TeamOrcaBox_VisionEar/Index.html)

Mirror site for demo: [LINK](https://truashamu.github.io/OrcaBoxMirrorSite/)


## Architecture

Architecture/Infrastructure diagram for creating an Edge Browser Extension that generates and plays audio-descriptions of images on a webpage upon user interaction with the image.

![System diagram of our solution](https://i.ibb.co/k4nLbSj/Screenshot-2023-07-10-014428.png)

Note that to fit the structure of an Edge Extension, some additional infrastructure was deployed.

1. Azure Functions to turn every CognitiveService-based feature into an endpoint that can accept HTTP requests.
2. Deployment of an Azure Storage Blob Container to store the generated audio files.

Looking forward
1. Need to have remaining secrets stored in an Azure Keyvault
2. Deploy a custom App Service Domain to host all function apps created under a shared domain

## Repository Info

[Extension](https://github.com/hadeelalii/team-OrcaBox/tree/main/team-OrcaBox)

[ComputerVision Service Function App](https://github.com/hadeelalii/team-OrcaBox/tree/main/ComputerVision)

[OpenAI Function App](https://github.com/hadeelalii/team-OrcaBox/tree/main/Description)

[Text-to-Speech Function App](https://github.com/hadeelalii/team-OrcaBox/tree/main/Text-To-Speech)

## Technologies Used

**Extension/Frontend**

HTML5, Edge Extensions Framework

**Azure Cognitive Services**

[Azure OpenAI Services](https://azure.microsoft.com/en-us/products/cognitive-services/openai-service) (Utilized GPT davinci model)

[Cognitive Speech Services](https://azure.microsoft.com/en-us/products/cognitive-services/speech-services/)

[Cognitive Service for Vision](https://azure.microsoft.com/en-us/products/cognitive-services/vision-services)

**Libraries/SDK**

[Semantic Kernel SDK](https://github.com/microsoft/semantic-kernel)

[FFmpeg](https://ffmpeg.org/)

**Deployment**

Azure Functions

Azure Storage (Blob storage container)

## Misc

Handwritten Proposal of Audio-description generation proposal (for vidoes) by Sofia

[Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/Video%20Description%20Generation.pdf)

**Proof-of-concept Files Created for video-based Audio-description Generation**
[Link](https://github.com/hadeelalii/team-OrcaBox/tree/main/Misc)


Implemented scripts to generate POC of Audio-descriptions.
[Link](https://github.com/hadeelalii/team-OrcaBox/tree/main/AudioDescriptionForVideo)
