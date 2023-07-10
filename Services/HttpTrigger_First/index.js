const sdk = require("microsoft-cognitiveservices-speech-sdk");
const fs = require("fs");
var readline = require("readline");

function synthesizeSpeech(descriptionGenerated) {
  SpeechSynthesizer = sdk.SpeechSynthesizer
  const speechConfig = sdk.SpeechConfig.fromSubscription("bc8bd5d9563b422582c50f35dbf3e6c3","eastus");
  speechConfig.speechSynthesisLanguage = "en-US"; 
  speechConfig.speechSynthesisVoiceName = "en-US-JennyNeural";
  const audioConfig = sdk.AudioConfig.fromAudioFileOutput("C:\\Users\\t-fvzquez\\Documents\\CodingJam\\output.wav");
  
  var rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
  });

  rl.question("Input: ", function (descriptionGenerated) {
    rl.close();
    const speechSynthesizer = new SpeechSynthesizer(speechConfig, audioConfig);
    speechSynthesizer.speakTextAsync(
        descriptionGenerated,
        result => {
            speechSynthesizer.close();
            if (result) {
                // return result as stream
                return fs.createReadStream("C:\\Users\\t-fvzquez\\Music\\jello.wav");
            }
        },
        error => {
            console.log(error);
            speechSynthesizer.close();
        });
    });
};

module.exports = {
    synthesizeSpeech: synthesizeSpeech
};

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');
    // 
    const name = (req.query.name || (req.body && req.body.name));
    const responseMessage = name
        ? "Hello, " + name + ". This HTTP triggered function executed successfully."
        : "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: responseMessage
    };

    synthesizeSpeech(context.res);

}