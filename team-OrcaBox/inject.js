function getImageUrls() {
    const images = document.querySelectorAll('img');
    const backgroundImages = getBackgroundImages();
    const inlineStyleImages = getInlineStyleImages();
  
    const urls = Array.from(images).map((img) => img.src);
    urls.push(...backgroundImages);
    urls.push(...inlineStyleImages);
  
    return urls;
  }
  
  async function fetchImageURL() {
    try {   // Here we will receive the url from the endpoint, that url is unique for each audio uploaded at Azure blob
      const response = await fetch('http://localhost:7071/api/HttpTrigger_First');
      if (response.ok) {
        const data = await response.json();
        const imageURL = data.url;
        // handle the image URL
        console.log(imageURL);
        return imageURL;
      } else {
        // handle the error
        console.error(`An error occurred: ${response.status}`);
      }
    } catch (error) {
      // handle the error
      console.error(error);
    }
  }
  /*
  The response from the endpoint might look like:
    {
      "url": "https://example.com/image.jpg"
    }
  */

  function getBackgroundImages() {
    const elements = document.querySelectorAll('*');
    const urls = [];
  
    elements.forEach((element) => {
      const backgroundImage = getComputedStyle(element).getPropertyValue(
        'background-image'
      );
  
      if (
        backgroundImage &&
        backgroundImage !== 'none' &&
        backgroundImage.startsWith('url')
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
    const elements = document.querySelectorAll('*');
    const urls = [];
  
    elements.forEach((element) => {
      const style = element.getAttribute('style');
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
  
  document.addEventListener('click', (event) => {
    const target = event.target;
    if (target.tagName === 'IMG') {
      console.log(target.src);
    }
  });
  
  chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === 'getImages') {
      const imageUrls = getImageUrls();
      sendResponse({ urls: imageUrls });
    }
  });
  