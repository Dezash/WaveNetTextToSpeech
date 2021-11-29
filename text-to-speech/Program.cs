using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace text_to_speech
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.GetEncoding(1257);

            Console.Title = "Google Text-To-Speech File Reader";
            Console.WriteLine("Author: Gabrielius Laurinavičius");
            Console.WriteLine();

            GoogleCredential credentials;
            try
            {
                credentials = GoogleCredential.FromFile("keys.json");
            }
            catch
            {
                Console.Error.WriteLine("ERROR: File \"keys.json\" not found.");
                return;
            }

            Console.WriteLine("File name: ");
            string fileName = Console.ReadLine();
            if(!File.Exists(fileName))
            {
                Console.Error.WriteLine("ERROR: File \"{0}\" not found.", fileName);
                return;
            }

            Console.WriteLine("Generating file. Please wait...");

            TextToSpeechClient client = TextToSpeechClient.Create(credentials);


            SynthesizeSpeechResponse response;
            try
            {
                response = client.SynthesizeSpeech(
                    new SynthesisInput()
                    {
                        Text = File.ReadAllText(fileName)
                    },
                    new VoiceSelectionParams()
                    {
                        LanguageCode = "en-US",
                        Name = "en-US-Wavenet-C"
                    },
                    new AudioConfig()
                    {
                        AudioEncoding = AudioEncoding.Mp3
                    }
                );
            }
            catch(Google.GoogleApiException e)
            {
                foreach(DictionaryEntry entry in e.Data)
                {
                    Console.WriteLine("{0} => {1}", entry.Key, entry.Value);
                }

                return;
            }


            string speechFile = Path.Combine(Directory.GetCurrentDirectory(), "result.mp3");

            File.WriteAllBytes(speechFile, response.AudioContent);
            Console.WriteLine("Success!");
            Console.WriteLine("Output: {0}\\result.mp3", Directory.GetCurrentDirectory());
            Console.ReadKey();
        }
    }
}
