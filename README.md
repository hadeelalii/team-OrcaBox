# Overview

Demo Video: [Video Link](https://www.youtube.com/watch?v=nmo-BexDSmE)

Powerpoint Presentation: [PPTX Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/VisuaSpeak.pptx)

Business Requirement Document / Proposal [PDF Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/OrcaBox%20Proposal.pdf)

Dev Spec: [PDF Link](https://github.com/hadeelalii/team-OrcaBox/blob/main/Document/Dev%20Spec.pdf)



## Architecture

Architecture/Infrastructure diagram for creating an Edge Browser Extension that generates and plays audio-descriptions of images on a webpage upon user interaction with the image.

![System diagram of our solution](https://i.ibb.co/k4nLbSj/Screenshot-2023-07-10-014428.png)

Note that to fit the structure of an Edge Extension, some additional infrastructure was deployed.

1. Azure Functions to turn every CognitiveService-based feature into an endpoint that can accept HTTP requests.
2. Deployment of an Azure Storage Blob Container to store the generated audio files.
