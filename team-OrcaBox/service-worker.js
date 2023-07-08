chrome.action.onClicked.addListener(async (tab) => {
  const [tabId] = await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    function: () => chrome.runtime.id,
  });

  chrome.tabs.sendMessage(tabId.result, { action: 'getImages' }, (response) => {
    if (response && response.urls) {
      console.log(response.urls);
    }
  });
});
