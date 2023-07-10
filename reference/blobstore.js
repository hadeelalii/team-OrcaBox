const { BlobServiceClient } = require("@azure/storage-blob");
const fs = require("fs");

const connectionString =
  "DefaultEndpointsProtocol=https;AccountName=codingjamstorage;AccountKey=VyrkKS0lFoIn58hNuLPCygV1TG/JPqJ3c7+oLU9aMFV/u4oRNefKib+FEI2c3SHr5hITkuDU9eXw+AStfRVqkw==;EndpointSuffix=core.windows.net";
const containerName = "mp3";

async function uploadWavToAzure(filePath) {
  // Create a new BlobServiceClient
  const blobServiceClient =
    BlobServiceClient.fromConnectionString(connectionString);

  // Get a reference to a container
  const containerClient = blobServiceClient.getContainerClient(containerName);

  // Get a unique name for the blob
  const fileName = `${Date.now()}.mp3`;

  // Get a block blob client
  const blockBlobClient = containerClient.getBlockBlobClient(fileName);

  // Upload the file
  await blockBlobClient.uploadFile(filePath);

  // Get the URL of the newly uploaded blob
  const blobUrl = blockBlobClient.url;

  return blobUrl;
}

const filePath = "C:\\Users\\t-sofiayang\\Art\\mcr-helena.mp3";

uploadWavToAzure(filePath)
  .then((blobUrl) => {
    console.log("Blob URL:", blobUrl);
  })
  .catch((error) => {
    console.error("Error uploading MP3 file:", error);
  });
