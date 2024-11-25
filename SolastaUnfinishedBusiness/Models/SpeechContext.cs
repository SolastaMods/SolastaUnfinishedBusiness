using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NAudio.Wave;
using Random = System.Random;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpeechContext
{
    private const string PiperDownloadURL =
        "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_windows_amd64.zip";

    private static readonly string PiperFolder = Path.Combine(Main.ModFolder, "piper");
    private static readonly string VoicesFolder = Path.Combine(Main.ModFolder, "Voices");

    private static readonly string[] Voices =
    [
        "en/en_GB/alan/medium/en_GB-alan-medium",
        "en/en_GB/alba/medium/en_GB-alba-medium",
        "en/en_GB/northern_english_male/medium/en_GB-northern_english_male-medium",
        "en/en_US/hfc_female/medium/en_US-hfc_female-medium",
        "en/en_US/hfc_male/medium/en_US-hfc_male-medium",
        "en/en_US/joe/medium/en_US-joe-medium",
        "en/en_US/kristin/medium/en_US-kristin-medium",
        "en/en_US/lessac/medium/en_US-lessac-medium",
        "en/en_US/ryan/medium/en_US-ryan-medium"
    ];

    internal static readonly WaveOutEvent WaveOutEvent = new();

    private static readonly string[] Quotes =
    [
        "Chuck Norris does in fact use a stunt double, but only for crying scenes.",
        "Chuck Norris doesn't flush the toilet, he scares the crap out of it.",
        "Chuck Norris went skydiving and his parachute didn't open, he took it back for a refund.",
        "Chuck Norris was awarded the Nobel Peace Prize, for letting so many people live.",
        "Chuck Norris's computer doesn't have a backspace key.",
        "Chuck Norris once had a fight with Superman. The loser had to wear his underpants on the outside.",
        "Chuck Norris once won a game of Connect Four in three moves.",
        "Chuck Norris can make sticks by rubbing two fires together.",
        "Chuck Norris once took a lie detector test. The machine confessed everything.",
        "Chuck Norris can fold airplanes into paper.",
        "Chuck Norris has no chin, under his beard is just another fist with an equally powerful beard.",
        "Chuck Norris can gargle peanut butter.",
        "Chuck Norris picked an apple from an orange tree and made lemonade.",
        "Chuck Norris is so fast he can run around the world and punch himself in the back of the head.",
        "Chuck Norris can put a plane in reverse.",
        "Chuck Norris is able to build a snowman out of water.",
        "Chuck Norris didn't call the wrong number, you answered the wrong phone.",
        "Chuck Norris didn't cheat death, he won fairly and squarely.",
        "Chuck Norris walked into chemistry class and ripped the Periodic Table of Elements off of the wall. Why? Because the only element Chuck Norris needs is the element of surprise.",
        "Chuck Norris once wrestled a bear, an alligator, and a tiger all at once. He won by tying them together with an anaconda.",
        "Chuck Norris was once bitten by a poisonous snake. And after a week of excruciating pain, the snake died.",
        "There are no streets named after Chuck Norris because no one would ever cross Chuck Norris",
        "Chuck Norris's mother tried to have an abortion. The procedure resulted in the doctor being knocked unconscious by Chuck Norris.",
        "When alexander graham bell first invented the telephone he had three missed calls from Chuck Norris",
        "Chuck Norris doesn't worry about gas prices, his vehicles run on fear.",
        "Chuck Norris doesn't pay taxes, taxes pay Chuck Norris.",
        "Chuck Norris once had an arm wrestling contest with superman. I'm not going to say who won, but the loser had to wear his underwear on the outside for the rest of his life.",
        "When Chuck Norris was born the doctor asked him to name his parents.",
        "The laws of physics always bend the rules for Chuck Norris.",
        "Chuck Norris didn't get a Covid-19 vaccine. Covid-19 got a Chuck Norris vaccine.",
        "Chuck Norris eats his meat so rare that he only eats unicorns and dragons.",
        "Chuck Norris once played Russian Roulette with a fully loaded gun and won.",
        "Whenever Chuck Norris peels onions, the onions always cry.",
        "Chuck Norris can pull a wheelie when riding a unicycle.",
        "Chuck Norris was born with two umbilical cords, one red and one blue. The bomb squad cut the wrong cord.",
        "Chuck Norris makes a lot of money selling his urine, it is called Red Bull.",
        "Chuck Norris is able to slam a revolving door.",
        "The day after Chuck Norris was born he drove his mother home, he wanted her to get some rest.",
        "Chuck Norris built the hospital that he was born in.",
        "Chuck Norris knows exactly what to do with the drunken sailors early in the morning.",
        "Chuck Norris played a game of rock, paper scissors against his reflection, and won.",
        "When Chuck Norris went to Burger King and ordered a big mac, they made it for him, perfectly.",
        "The Swiss Army uses Chuck Norris Knives.",
        "A condom puts on protection to avoid becoming impregnated by Chuck Norris on date night.",
        "Chuck Norris is able to start a fire using an extinguisher.",
        "Chuck Norris doesn't need to throw out the trash, it always throws itself out.",
        "Chuck Norris has to carry a concealed weapons permit when he wears his regular clothes.",
        "When Chuck Norris once roundhouse kicked a coal mine and turned it into a diamond mine.",
        "Chuck Norris doesn't strike gold, gold is the byproduct of Chuck Norris roundhouse kicking rocks.",
        "When Chuck Norris lifts weights, the weights get in shape.",
        "Chuck Norris is able to strangle people using a cordless phone.",
        "Chuck Norris is the reason that Wally is always hiding.",
        "When Chuck Norris falls from a great height, the ground has it's life flash before it's eyes.",
        "When Chuck Norris enters a building that is on fire, the Chuck Norris alarm rings.",
        "When Thanos snapped his fingers he disappeared. Chuck Norris doesn't like snapping.",
        "The sun has to wear sunglasses when Chuck Norris glances at it.",
        "When Chuck Norris looked into the abyss, the abyss looked the other way.",
        "The Grand Canyon was formed when Chuck Norris was doing a triathlon.",
        "Bigfoot is still hiding because he once saw Chuck Norris walking in the mountains.",
        "When Chuck Norris drops the soap in prison, he picks it up successfully.",
        "The Loch Ness Monster claims to have seen Chuck Norris.",
        "Chuck Norris can drink a whole glass of beer. Yep, even the glass.",
        "When Chuck Norris uses the internet he can skip ads whenever he wants, ads are not able to skip Chuck Norris.",
        "Chuck Norris doesn't negotiate with terrorists.",
        "The terrorists negotiate with Chuck Norris.",
        "Chuck Norris won an arm wrestling tournament, with both arms tied behind his back.",
        "Chuck Norris got a divorce and was asked to give half his assets and property away. Chuck Norris proceeded to chop the entire universe in half with his bare hands.",
        "The Flash discovered how to run at the speed of light when he discovered Chuck Norris was looking for him.",
        "When Chuck Norris goes bowling he doesn't get every pin with a single bowl he gets every pin in the bowling alley.",
        "The reason why people say it's pointless for Trump to build a wall is because Chuck Norris walks to Mexico and back once a month.",
        "Ghosts tell Chuck Norris stories at the campfire.",
        "Chuck Norris mines bitcoin with a pen and paper.",
        "When Chuck Norris goes to a restaurant, the waiter tips him.",
        "Tornadoes don't exist, Chuck Norris just really doesn't like trailer parks.",
        "Chuck Norris was born May 6th 1945. The Nazis surrendered May 7th 1945, this is not a coincidence.",
        "Chuck Norris has counted to infinity more than once. Then he counted backward from infinity.",
        "Chuck Norris has a bear rug on his lounge floor. The bear is still alive, it is just afraid to move.",
        "Chuck Norris doesn't go to the gym, instead he goes shop lifting.",
        "If Chuck Norris was on The Titanic the iceberg would have dodged the ship.",
        "Chuck Norris is able to make other people walk in his sleep.",
        "Chuck Norris once raced the earth around the sun and won by three years.",
        "Chuck Norris was asked to fire someone once, that is how hell was invented.",
        "When Chuck Norris jumps on the Tempur-Pedic mattress, the wine glass falls over.",
        "When Chuck Norris was a child at school, his teachers would raise their hands in order to talk to him.",
        "When Chuck Norris's parents had nightmares, they would come to his bedroom.",
        "When Chuck Norris crosses the road, vehicles look both ways.",
        "Chuck Norris once missed two days of school. Those two days are now called the weekend.",
        "Chuck Norris doesn't pop his collar, his shirts are stimulated from touching his shoulders.",
        "Chuck Norris once threw a grenade and killed 100 men, after that the grenade exploded.",
        "Chuck Norris was able to smell a gas leak before they added the scent to gas.",
        "Chuck Norris has a diary, it is called the Guinness Book Of World Records.",
        "Hi there, I heard that you are a huge fan of when Chuck Norris does push ups the earth moves, we call this phenomenon an earthquakes.",
        "Chuck Norris uses pepper spray to season his meat.",
        "Chuck Norris is able to sketch your portrait using an eraser.",
        "The dinosaurs once looked at Chuck Norris the wrong way, that is why we no longer have dinosaurs.",
        "Chuck Norris had a staring competition with the sun and won.",
        "Chuck Norris once spun a ball on his finger, to this day planet earth continues to turn.",
        "Chuck Norris doesn't climb trees, he just pushed them over and walks over them.",
        "Chuck Norris can kill 2 stones with one bird.",
        "Chuck Norris doesn't need to wear a watch, he simply decides what time it is."
    ];

    internal static readonly string[] VoiceNames = Voices.Select(x => x.Split('/')[2].Replace("_", " ")).ToArray();

    internal static void Load()
    {
        DownloadPiper();
        DownloadVoices();
    }

    internal static void SpeakQuote()
    {
        var random = new Random();
        var quoteNumber = random.Next(0, Quotes.Length);

        Speak(Quotes[quoteNumber]);
    }

    //TODO: how to integrate an unity Mono Behavior with async / await?
    internal static async void Speak(string inputText)
    {
        try
        {
            // only custom campaigns
            if (Gui.GameCampaign)
            {
                if (Gui.GameCampaign.campaignDefinition.IsUserCampaign)
                {
                    return;
                }
            }

            // only if audio and feature enabled
            var audioSettingsService = ServiceRepository.GetService<IAudioSettingsService>();

            if (!Main.Settings.EnableSpeech ||
                !audioSettingsService.MasterEnabled)
            {
                return;
            }

            var task = Task.Run(async () =>
            {
                var audioStream = new MemoryStream();
                var buffer = new byte[16384];
                var modelName = Path.GetFileName(Voices[Main.Settings.SpeechVoice]);
                var modelFileName = Path.Combine(VoicesFolder, modelName + ".onnx");
                var piper = new Process();
                var scale = Main.Settings.SpeechScale;
                int bytesRead;

                piper.StartInfo.FileName = Path.Combine(PiperFolder, "piper.exe");
                piper.StartInfo.Arguments = $"--model \"{modelFileName}\" --length_scale {scale:F} --output-raw";
                piper.StartInfo.UseShellExecute = false;
                piper.StartInfo.CreateNoWindow = true;
                piper.StartInfo.RedirectStandardInput = true;
                piper.StartInfo.RedirectStandardOutput = true;
                piper.Start();

                using var writer = piper.StandardInput;

                await writer.WriteAsync(inputText);
                writer.Close();

                while ((bytesRead = await piper.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    audioStream.Write(buffer, 0, bytesRead);
                }

                return audioStream;
            });
            var audioStream = await task;

            audioStream.Position = 0;

            using var waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22050, 1));

            while (WaveOutEvent.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
            }

            WaveOutEvent.Init(waveStream);
            WaveOutEvent.Volume = audioSettingsService.MasterVolume * audioSettingsService.VoicesVolume;
            WaveOutEvent.Play();
        }
        catch (Exception e)
        {
            Main.Error(e);
        }
    }

    private static void DownloadPiper()
    {
        var message = "Piper successfully downloaded.";
        var fullZipFile = Path.Combine(Main.ModFolder, "piper_windows_amd64.zip");
        var wc = new WebClient();

        try
        {
            if (Directory.Exists(PiperFolder))
            {
                message = "Piper already exists.";
            }
            else
            {
                wc.DownloadFile(PiperDownloadURL, fullZipFile);
                ZipFile.ExtractToDirectory(fullZipFile, Main.ModFolder);
                File.Delete(fullZipFile);
            }
        }
        catch
        {
            message = "Cannot download Piper.";
        }

        Main.Info(message);
    }

    private static void DownloadVoices()
    {
        const string URL_PREFIX = "https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/";

        var wc = new WebClient();

        if (!Directory.Exists(VoicesFolder))
        {
            Directory.CreateDirectory(VoicesFolder);
        }

        foreach (var voice in Voices)
        {
            var message = $"Voice {voice} successfully downloaded";
            var model = $"{voice}.onnx";
            var modelFilename = Path.GetFileName(model);
            var fullModelFilename = Path.Combine(VoicesFolder, modelFilename);
            var modelUrl = $"{URL_PREFIX}{model}?download=true";

            try
            {
                if (!File.Exists(fullModelFilename))
                {
                    wc.DownloadFile(modelUrl, fullModelFilename);

                    var json = $"{voice}.onnx.json";
                    var jsonFilename = Path.GetFileName(json);
                    var fullJsonFilename = Path.Combine(VoicesFolder, jsonFilename);
                    var jsonUrl = $"{URL_PREFIX}{json}?download=true";

                    if (!File.Exists(fullJsonFilename))
                    {
                        wc.DownloadFile(jsonUrl, fullJsonFilename);
                    }
                }
                else
                {
                    message = $"Voice {voice} already exists.";
                }
            }
            catch
            {
                message = $"Cannot download voice {voice}.";
            }

            Main.Info(message);
        }
    }
}
