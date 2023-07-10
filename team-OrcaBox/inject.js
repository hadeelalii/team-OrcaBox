// This is the stuff for the fetches

// Computer Vision API
async function getImageDetails(imgUrl) {
  const endpoint =
    "https://computervision20230708184948.azurewebsites.net/api/Function1?code=ZYpUM47W8Twr0i7F5ITER-Av3MC1s6mzAE1PCnel3Y03AzFuy1ZGEA==";

  const body = {
    imageURL: imgUrl,
  };

  try {
    const response = await fetch(endpoint, {
      method: "POST",
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      throw new Error("Request failed");
    }

    const result = await response.json();
    return result;
  } catch (error) {
    console.error("Error:", error);
    return null;
  }
}

// OpenAI Cognitive Services API
async function getDescription(json) {
  try {
    const response = await fetch(
      "https://descriptiongeneration.azurewebsites.net/api/Function1?code=bAaX5tT-e129abnsOCa3xi7bC3IMV8y-uHDIQ31No7LcAzFulwoV7w==",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(json),
      }
    );

    console.log("Response Status: " + response.status);
    if (response.status == 200) {
      const data = await response.json();
      console.log(data.description);
      return data.description;
    } else {
      throw new Error("Request failed with status " + response.status);
    }
  } catch (error) {
    console.error("An error occurred:", error);
    return null;
  }
}

// Text-to-Speech API
async function processDescription(description) {
  try {
    const requestBody = {
      description: description,
    };

    const response = await fetch(
      "https://nodefuncttest.azurewebsites.net/api/HttpTrigger1?code=BBP1SXP4Szi7eTXzmHuMl-4Xq7i3uA1nd3Ps1sloXAA-AzFugEFkNg==",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestBody),
      }
    );

    if (response.ok) {
      const data = await response.text();
      return data;
    } else {
      throw new Error("Request failed with status " + response.status);
    }
  } catch (error) {
    console.error("An error occurred:", error);
    return null;
  }
}

// End of the stuff for the fetches

// Function to pass in image URL and get audio URL in blobs
async function main(url) {
  console.log("nananana");
  const imageData = await getImageDetails(url);
  console.log(imageData);
  const description = await getDescription(imageData);
  console.log(description);
  const processedDescription = await processDescription(description);
  console.log(processedDescription);
  return processedDescription;
}

function getImageUrls() {
  const images = document.querySelectorAll("img");
  const backgroundImages = getBackgroundImages();
  const inlineStyleImages = getInlineStyleImages();

  const urls = Array.from(images).map((img) => img.src);
  urls.push(...backgroundImages);
  urls.push(...inlineStyleImages);

  return urls;
}

function getBackgroundImages() {
  const elements = document.querySelectorAll("*");
  const urls = [];

  elements.forEach((element) => {
    const backgroundImage =
      getComputedStyle(element).getPropertyValue("background-image");

    if (
      backgroundImage &&
      backgroundImage !== "none" &&
      backgroundImage.startsWith("url")
    ) {
      const url = extractUrlFromBackgroundImage(backgroundImage);
      if (url) {
        urls.push(url);
      }
    }
  });

  return urls;
}

function extractUrlFromBackgroundImage(backgroundImage) {
  const regex = /url\(['"]?(.*?)['"]?\)/;
  const match = regex.exec(backgroundImage);
  return match ? match[1] : null;
}

function getInlineStyleImages() {
  const elements = document.querySelectorAll("*");
  const urls = [];

  elements.forEach((element) => {
    const style = element.getAttribute("style");
    if (style) {
      const regex = /url\(['"]?(.*?)['"]?\)/g;
      let match;
      while ((match = regex.exec(style)) !== null) {
        urls.push(match[1]);
      }
    }
  });

  return urls;
}

document.addEventListener("click", (event) => {
  const target = event.target;
  if (target.tagName === "IMG") {
    console.log(target.src);
  }
});

// Function to play audio in the active tab
function playAudio(audioUrl) {
  const audio = new Audio(audioUrl);
  audio.play();
}

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
  if (request.action === "getImages") {
    const imageUrls = getImageUrls();
    sendResponse({ urls: imageUrls });
    if (imageUrls.length > 0) {
      const audioUrl = await main(imageUrls[0]);
      playAudio(audioUrl);
    }
  }
});
