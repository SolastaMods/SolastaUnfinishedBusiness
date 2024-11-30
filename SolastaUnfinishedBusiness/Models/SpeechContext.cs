using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using NAudio.Wave;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using Random = System.Random;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpeechContext
{
    internal const int MaxHeroes = 6;

    private const string DefaultVoice = "No Voice";
    private const float DefaultScale = 0.8f;

    private const string VoicesURLPrefix = "https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/";

    private const string PiperLinuxDownloadURL =
        "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_linux_x86_64.tar.gz";

    private const string PiperOSXDownloadURL =
        "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_macos_x64.tar.gz";

    private const string PiperWindowsDownloadURL =
        "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_windows_amd64.zip";

    private static readonly Regex RemoveNpcSpeechTags =
        new(@"<[bci/].*?>|\*.+?\*|\(.+?\)|\[.+?\]|\{.+?\}", RegexOptions.Compiled);

    private static readonly string PiperFolder =
        Path.Combine(
            Main.ModFolder,
            Path.Combine(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "piper_linux_x86_64" // linux unzips to piper_linux_x86_64/piper folder
                    : Path.Combine(
                        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                            ? "piper_macos_x64" // macos unzips to piper_macos_x64/piper folder
                            : "."), // windows unzips to ./piper folder
                "piper"));

    private static readonly string VoicesFolder = Path.Combine(Main.ModFolder, Path.Combine("..", "Voices"));

    private static readonly WaveOutEvent WaveOutEvent = new();

    private static readonly (string, Gender)[] SuggestedVoicesUrls =
    [
        ($"{VoicesURLPrefix}en/en_GB/alan/medium/en_GB-alan-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_GB/alba/medium/en_GB-alba-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_GB/aru/medium/en_GB-aru-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_GB/cori/medium/en_GB-cori-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_GB/jenny_dioco/medium/en_GB-jenny_dioco-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_GB/northern_english_male/medium/en_GB-northern_english_male-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_GB/semaine/medium/en_GB-semaine-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_GB/vctk/medium/en_GB-vctk-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/amy/medium/en_US-amy-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/arctic/medium/en_US-arctic-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/bryce/medium/en_US-bryce-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/hfc_female/medium/en_US-hfc_female-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/hfc_male/medium/en_US-hfc_male-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/joe/medium/en_US-joe-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/john/medium/en_US-john-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/kristin/medium/en_US-kristin-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/kusal/medium/en_US-kusal-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/lessac/medium/en_US-lessac-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/libritts_r/medium/en_US-libritts_r-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/ljspeech/medium/en_US-ljspeech-medium", Gender.Female),
        ($"{VoicesURLPrefix}en/en_US/norman/medium/en_US-norman-medium", Gender.Male),
        ($"{VoicesURLPrefix}en/en_US/ryan/medium/en_US-ryan-medium", Gender.Male)
        //("https://huggingface.co/quarterturn/kuroki_tomoko_en_piper/resolve/main/kuroki_tomoko", Gender.Female)
    ];

    private static readonly string[] FemaleNpcs =
    [
        "Aristocrat_Adria",
        "Aristocrat_Lyria",
        "Atima_Bladeburn",
        "Beryl_Stonebeard",
        "Bitterroot",
        "Caer_Cyflen_Guard",
        "CaerCyflenCityGuard_NewEmpire_Female",
        "Captain_Verissa_Ironshell",
        "Ceiwad_Silverflower",
        "Circe",
        "Council_Trooper",
        "CultistGuard",
        "Daliat_Sunbird",
        "DLC1_Complex_NPC_Guard_Recruit",
        "DLC1_Complex_NPC_Guard_Watcher",
        "DLC1_Complex_NPC_Trainer_01",
        "DLC1_NPC_2_Marches_Helia_Fairblade",
        "DLC1_NPC_3_Marches_Rogue_Leyrin_Catpaw",
        "DLC1_NPC_Armorer_Gail_Hunt",
        "DLC1_NPC_CityGuard_Captain_ThePeople04",
        "DLC1_NPC_Finaliel",
        "DLC1_NPC_Forge_Ravener",
        "DLC1_NPC_Malariel",
        "DLC1_NPC_Mask_CafrainShadow",
        "DLC1_NPC_Merchant_Mask_Yasmin",
        "DLC1_NPC_Rebelion_Ellaria_Anfarel",
        "DLC1_NPC_Rebellion_Blue",
        "DLC1_NPC_Rebellion_Red",
        "DLC1_NPC_Rebellion_Sima_Temple",
        "DLC1_NPC_ThePeople_Karelia",
        "DLC1_NPC_ThePeople_Reya",
        "DLC1_NPC_ThePeople_Rose",
        "DLC1_NPC_ThePeople_Tortured",
        "DLC1_NPC_Witch_Neutral",
        "DLC1_Orc_Shaman_Leader",
        "DLC1_Valley_NPC_Samko_Flint",
        "DLC3_Berghild_StrongSpine",
        "DLC3_Beryl_Stonebeard",
        "DLC3_Council_Trooper_1",
        "DLC3_ElvenClans_DragonbornIntermediate",
        "DLC3_ElvenClans_ElfAdvisor2",
        "DLC3_ElvenClans_Leralyn",
        "DLC3_Gallivan_Royals_TheCousin",
        "DLC3_Gallivan_Royals_TheQueen",
        "DLC3_Gallivan_Suspect01",
        "DLC3_Gallivan_Suspect02",
        "DLC3_GarradSoldier02",
        "DLC3_GarradSoldier03",
        "DLC3_GarradSoldier0C",
        "DLC3_Grimhild_DarkHead",
        "DLC3_Kara_WiseHead",
        "DLC3_Lena_Switfhand",
        "DLC3_Lisbath_Townsend",
        "DLC3_Misouk",
        "DLC3_NPC_Crowd8_DLC3_Ending",
        "DLC3_NPC_Einareum_Merchant_General",
        "DLC3_NPC_Einareum_Merchant_Weapons",
        "DLC3_NPC_ElvenClans_Greybear_Hunter",
        "DLC3_NPC_ElvenClans_Guard",
        "DLC3_NPC_ElvenClans_GuardCaptain",
        "DLC3_NPC_GenericScavengerScout",
        "DLC3_NPC_Helia_Fairblade",
        "DLC3_NPC_HumanClans_Guard",
        "DLC3_NPC_HumanClans1_DLC3_Ending",
        "DLC3_NPC_HumanClansLeader",
        "DLC3_NPC_Narrator_DLC3_Ending",
        "DLC3_NPC_NorthernClans_Merchant_Ingredients",
        "DLC3_NPC_SouthernClans_Caretaker",
        "DLC3_NPC_SouthernClans_Cousin_Kaikonnen",
        "DLC3_NPC_SouthernClans_Innkeeper",
        "DLC3_NPC_SouthernClans_Merchant_Clan",
        "DLC3_NPC_SouthernClans_Merchant_General",
        "DLC3_NPC_SouthernClans_Merchant_Ingredients",
        "DLC3_NPC_SouthernClans_Merchant_Scavenger",
        "DLC3_NPC_WhiteCity_Guard_Captain",
        "DLC3_NPC_WhiteCity_Trapper_Family_01",
        "DLC3_Undermountain_EttivenGuard",
        "DLC3_Undermountain_Investigation_Informant",
        "DLC3_Undermountain_PerlevinnGuard_Banter",
        "DLC3_Vigdis_Kaikonnen",
        "DLC3_Violet_Goodcheer",
        "DLC3_WhiteCity_MotherYoungDwarf",
        "DLC3_WhiteCity_YoungDwarf",
        "Heather_Merran",
        "Hertha_Gormsdottir",
        "Joriel_Foxeye",
        "Kebra",
        "Kythaela",
        "Leira_Kean",
        "Lena",
        "Lisbath_Townsend",
        "Maddy_Greenisle",
        "Maid_Coparann",
        "Mayor_Kiaradth_Bright-Spark",
        "Merchant_Annie_Bagmordah",
        "Merchant_Gorim_Ironsoot",
        "Milan",
        "Mildred_Warmhearth",
        "Philosopher_Illoreth",
        "Priestess_Of_Pakri_Elaine_Velasco"
    ];

    private static readonly string[] Quotes =
    [
        "{Subject} does in fact use a stunt double, but only for crying scenes.",
        "{Subject} doesn't flush the toilet, he scares the crap out of it.",
        "{Subject} went skydiving and his parachute didn't open, he took it back for a refund.",
        "{Subject} was awarded the Nobel Peace Prize, for letting so many people live.",
        "{Subject}'s computer doesn't have a backspace key.",
        "{Subject} once had a fight with Superman. The loser had to wear his underpants on the outside.",
        "{Subject} once won a game of Connect Four in three moves.",
        "{Subject} can make sticks by rubbing two fires together.",
        "{Subject} once took a lie detector test. The machine confessed everything.",
        "{Subject} can fold airplanes into paper.",
        "{Subject} has no chin, under his beard is just another fist with an equally powerful beard.",
        "{Subject} can gargle peanut butter.",
        "{Subject} picked an apple from an orange tree and made lemonade.",
        "{Subject} is so fast he can run around the world and punch himself in the back of the head.",
        "{Subject} can put a plane in reverse.",
        "{Subject} is able to build a snowman out of water.",
        "{Subject} didn't call the wrong number, you answered the wrong phone.",
        "{Subject} didn't cheat death, he won fairly and squarely.",
        "{Subject} walked into chemistry class and ripped the Periodic Table of Elements off of the wall. Why? Because the only element {Subject} needs is the element of surprise.",
        "{Subject} once wrestled a bear, an alligator, and a tiger all at once. He won by tying them together with an anaconda.",
        "{Subject} was once bitten by a poisonous snake. And after a week of excruciating pain, the snake died.",
        "There are no streets named after {Subject} because no one would ever cross {Subject}",
        "{Subject}'s mother tried to have an abortion. The procedure resulted in the doctor being knocked unconscious by {Subject}.",
        "When alexander graham bell first invented the telephone he had three missed calls from {Subject}",
        "{Subject} doesn't worry about gas prices, his vehicles run on fear.",
        "{Subject} doesn't pay taxes, taxes pay {Subject}.",
        "{Subject} once had an arm wrestling contest with superman. I'm not going to say who won, but the loser had to wear his underwear on the outside for the rest of his life.",
        "When {Subject} was born the doctor asked him to name his parents.",
        "The laws of physics always bend the rules for {Subject}.",
        "{Subject} didn't get a Covid-19 vaccine. Covid-19 got a {Subject} vaccine.",
        "{Subject} eats his meat so rare that he only eats unicorns and dragons.",
        "{Subject} once played Russian Roulette with a fully loaded gun and won.",
        "Whenever {Subject} peels onions, the onions always cry.",
        "{Subject} can pull a wheelie when riding a unicycle.",
        "{Subject} was born with two umbilical cords, one red and one blue. The bomb squad cut the wrong cord.",
        "{Subject} makes a lot of money selling his urine, it is called Red Bull.",
        "{Subject} is able to slam a revolving door.",
        "The day after {Subject} was born he drove his mother home, he wanted her to get some rest.",
        "{Subject} built the hospital that he was born in.",
        "{Subject} knows exactly what to do with the drunken sailors early in the morning.",
        "{Subject} played a game of rock, paper scissors against his reflection, and won.",
        "When {Subject} went to Burger King and ordered a big mac, they made it for him, perfectly.",
        "The Swiss Army uses {Subject} Knives.",
        "A condom puts on protection to avoid becoming impregnated by {Subject} on date night.",
        "{Subject} is able to start a fire using an extinguisher.",
        "{Subject} doesn't need to throw out the trash, it always throws itself out.",
        "{Subject} has to carry a concealed weapons permit when he wears his regular clothes.",
        "When {Subject} once roundhouse kicked a coal mine and turned it into a diamond mine.",
        "{Subject} doesn't strike gold, gold is the byproduct of {Subject} roundhouse kicking rocks.",
        "When {Subject} lifts weights, the weights get in shape.",
        "{Subject} is able to strangle people using a cordless phone.",
        "{Subject} is the reason that Wally is always hiding.",
        "When {Subject} falls from a great height, the ground has it's life flash before it's eyes.",
        "When {Subject} enters a building that is on fire, the {Subject} alarm rings.",
        "When Thanos snapped his fingers he disappeared. {Subject} doesn't like snapping.",
        "The sun has to wear sunglasses when {Subject} glances at it.",
        "When {Subject} looked into the abyss, the abyss looked the other way.",
        "The Grand Canyon was formed when {Subject} was doing a triathlon.",
        "Bigfoot is still hiding because he once saw {Subject} walking in the mountains.",
        "When {Subject} drops the soap in prison, he picks it up successfully.",
        "The Loch Ness Monster claims to have seen {Subject}.",
        "{Subject} can drink a whole glass of beer. Yep, even the glass.",
        "When {Subject} uses the internet he can skip ads whenever he wants, ads are not able to skip {Subject}.",
        "{Subject} doesn't negotiate with terrorists.",
        "The terrorists negotiate with {Subject}.",
        "{Subject} won an arm wrestling tournament, with both arms tied behind his back.",
        "{Subject} got a divorce and was asked to give half his assets and property away. {Subject} proceeded to chop the entire universe in half with his bare hands.",
        "The Flash discovered how to run at the speed of light when he discovered {Subject} was looking for him.",
        "When {Subject} goes bowling he doesn't get every pin with a single bowl he gets every pin in the bowling alley.",
        "The reason why people say it's pointless for Trump to build a wall is because {Subject} walks to Mexico and back once a month.",
        "Ghosts tell {Subject} stories at the campfire.",
        "{Subject} mines bitcoin with a pen and paper.",
        "When {Subject} goes to a restaurant, the waiter tips him.",
        "Tornadoes don't exist, {Subject} just really doesn't like trailer parks.",
        "{Subject} was born May 6th 1945. The Nazis surrendered May 7th 1945, this is not a coincidence.",
        "{Subject} has counted to infinity more than once. Then he counted backward from infinity.",
        "{Subject} has a bear rug on his lounge floor. The bear is still alive, it is just afraid to move.",
        "{Subject} doesn't go to the gym, instead he goes shop lifting.",
        "If {Subject} was on The Titanic the iceberg would have dodged the ship.",
        "{Subject} is able to make other people walk in his sleep.",
        "{Subject} once raced the earth around the sun and won by three years.",
        "{Subject} was asked to fire someone once, that is how hell was invented.",
        "When {Subject} jumps on the Tempur-Pedic mattress, the wine glass falls over.",
        "When {Subject} was a child at school, his teachers would raise their hands in order to talk to him.",
        "When {Subject}'s parents had nightmares, they would come to his bedroom.",
        "When {Subject} crosses the road, vehicles look both ways.",
        "{Subject} once missed two days of school. Those two days are now called the weekend.",
        "{Subject} doesn't pop his collar, his shirts are stimulated from touching his shoulders.",
        "{Subject} once threw a grenade and killed 100 men, after that the grenade exploded.",
        "{Subject} was able to smell a gas leak before they added the scent to gas.",
        "{Subject} has a diary, it is called the Guinness Book Of World Records.",
        "Hi there, I heard that you are a huge fan of when {Subject} does push ups the earth moves, we call this phenomenon an earthquakes.",
        "{Subject} uses pepper spray to season his meat.",
        "{Subject} is able to sketch your portrait using an eraser.",
        "The dinosaurs once looked at {Subject} the wrong way, that is why we no longer have dinosaurs.",
        "{Subject} had a staring competition with the sun and won.",
        "{Subject} once spun a ball on his finger, to this day planet earth continues to turn.",
        "{Subject} doesn't climb trees, he just pushed them over and walks over them.",
        "{Subject} can kill 2 stones with one bird.",
        "{Subject} doesn't need to wear a watch, he simply decides what time it is."
    ];

    private static readonly Random Quoteziner = new();

    private static readonly List<string> AvailableFemaleVoices = [];

    private static readonly List<string> AvailableMaleVoices = [];

    private static readonly Dictionary<string, (string, float)> CampaignVoices = [];

    internal static readonly string[] Choices = new List<string> { "Narrator" }
        .Union(Enumerable.Range(1, MaxHeroes).Select(n => $"Hero {n}")).ToArray();

    internal static string[] VoiceNames { get; private set; }

    internal static void Load()
    {
        InitPiper();
        RefreshAvailableVoices();
        InitVoiceAssignments();
        UpdateAvailableVoices();
    }

    private static void InitPiper()
    {
        if (!Directory.Exists(VoicesFolder))
        {
            Directory.CreateDirectory(VoicesFolder);
        }

        string url;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            url = PiperLinuxDownloadURL;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            url = PiperOSXDownloadURL;
        }
        else
        {
            url = PiperWindowsDownloadURL;
        }

        var message = "Piper successfully downloaded.";
        var filename = Path.GetFileName(url);
        var fullZipFile = Path.Combine(Main.ModFolder, filename);
        using var wc = new WebClient();

        try
        {
            if (Directory.Exists(PiperFolder))
            {
                message = "Piper already exists.";
            }
            else
            {
                wc.DownloadFile(url, fullZipFile);
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

    internal static void RefreshAvailableVoices()
    {
        var directoryInfo = new DirectoryInfo(VoicesFolder);
        var voices = directoryInfo.GetFiles("*.onnx").Select(x => x.Name.Replace(".onnx", string.Empty));

        VoiceNames = new List<string> { DefaultVoice }.Union(voices).ToArray();
    }

    private static void InitVoiceAssignments()
    {
        // remove any invalid key
        Main.Settings.SpeechVoices.Keys
            .Where(x => x is < 0 or > MaxHeroes)
            .Do(x => Main.Settings.SpeechVoices.Remove(x));

        for (var i = 0; i <= MaxHeroes; i++)
        {
            Main.Settings.SpeechVoices.TryAdd(i, (DefaultVoice, DefaultScale));

            if (!VoiceNames.Contains(Main.Settings.SpeechVoices[i].Item1))
            {
                Main.Settings.SpeechVoices[i] = (DefaultVoice, DefaultScale);
            }
        }
    }

    internal static void UpdateAvailableVoices()
    {
        var assignedVoices = Main.Settings.SpeechVoices.Values.Select(x => x.Item1).Distinct().ToArray();

        AvailableFemaleVoices.SetRange(
            VoiceNames
                .Where(x =>
                    x != DefaultVoice &&
                    !assignedVoices.Contains(x) &&
                    SuggestedVoicesUrls
                        .Any(y => y.Item1.Contains(x) && y.Item2 == Gender.Female)));

        AvailableMaleVoices.SetRange(
            VoiceNames
                .Where(x =>
                    x != DefaultVoice &&
                    !assignedVoices.Contains(x) &&
                    SuggestedVoicesUrls
                        .Any(y => y.Item1.Contains(x) && y.Item2 == Gender.Male)));
    }

    internal static void CollectCustomCampaignVoiceData()
    {
        const string NARRATOR = "NARRATOR";
        const string UB_VOICE_DATA = "UB_VOICE_DATA";

        if (!Gui.Game.CampaignDefinition.IsUserCampaign)
        {
            return;
        }

        CampaignVoices.Clear();

        var userCampaign = Gui.Session.UserCampaign;
        var voiceData = userCampaign.UserItems.FirstOrDefault(x =>
            x.ReferenceItemDefinition.IsDocument &&
            x.DocumentFragments is { Count: > 0 } &&
            x.InternalName == UB_VOICE_DATA);

        if (voiceData == null)
        {
            Main.Info($"Campaign {userCampaign.DisplayTitle} has no voice data.");

            return;
        }

        foreach (var fragment in voiceData.DocumentFragments)
        {
            var arr = fragment.Split(',');

            if (arr.Length is < 2 or > 3)
            {
                Main.Info($"Failed to parse voice data: [{fragment}]");
                continue;
            }

            var npc = arr[0];
            var voice = arr[1];
            var scale = DefaultScale;

            if (arr.Length == 3)
            {
                try
                {
                    scale = float.Parse(arr[2]);
                }
                catch (FormatException ex)
                {
                    Main.Info($"Failed to parse voice scale data: [{fragment}] {ex.Message}");
                    scale = DefaultScale;
                }
            }

            if (scale < 0.5 || scale > 2)
            {
                Main.Info($"Failed to validate scale range: [{fragment}]");
                scale = DefaultScale;
            }

            if (!VoiceNames.Contains(voice))
            {
                Main.Info(
                    $"voice definition on campaign {userCampaign.DisplayTitle}, fragment [{fragment}], was not found");

                continue;
            }

            if (npc == NARRATOR ||
                userCampaign.UserNpcs.Any(x => x.InternalName == npc))
            {
                CampaignVoices.AddOrReplace(npc, (voice, scale));
            }
            else
            {
                Main.Info(
                    $"NPC definition on campaign {userCampaign.DisplayTitle}, fragment [{fragment}], was not found");
            }
        }
    }

    internal static void ShutUp()
    {
        WaveOutEvent.Stop();
    }

    private static string StripXmlTagsAndNarration(string str)
    {
        return RemoveNpcSpeechTags.Replace(str, string.Empty);
    }

    private static bool TryGetExecutablePath(out string executablePath)
    {
        var executable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "piper.exe" : "piper";

        executablePath = Path.Combine(PiperFolder, executable);

        return File.Exists(executablePath);
    }

    internal static void SpeakQuote()
    {
        var quoteNumber = Quoteziner.Next(0, Quotes.Length);
        var subjects = new[] { "Chuck Norris", "Zappa" };
        var subject = subjects[Quoteziner.Next(0, subjects.Length)];
        var quote = Quotes[quoteNumber].Replace("{Subject}", subject);

        Speak(quote, Main.Settings.SpeechChoice, false);
    }

    internal static void Speak(string inputText, GameLocationCharacter character)
    {
        var index = Gui.Game.GameCampaign.Party.CharactersList
            .FindIndex(x => x.RulesetCharacter == character.RulesetCharacter);

        if (index < 0)
        {
            return;
        }

        Speak(inputText, index + 1);
    }

    // heroId zero is the Narrator and 1-6 map to heroes in party
    internal static async void Speak(string inputText, int heroId, bool forceUseCampaign = true)
    {
        try
        {
            ShutUp();

            // only if audio enabled
            var audioSettingsService = ServiceRepository.GetService<IAudioSettingsService>();

            if (!audioSettingsService.MasterEnabled)
            {
                return;
            }

            if (!Main.Settings.EnableSpeech || heroId < 0 || heroId > MaxHeroes)
            {
                return;
            }

            // only custom campaigns
            if (forceUseCampaign)
            {
                // unity life check...
                if (Gui.GameCampaign)
                {
                    if (!Gui.GameCampaign.campaignDefinition.IsUserCampaign)
                    {
                        return;
                    }
                }
            }

            if (!TryGetExecutablePath(out var executablePath))
            {
                return;
            }

            string voice;
            float scale;

            if (!Main.Settings.ForceModSpeechOnNpcs &&
                CampaignVoices.TryGetValue("NARRATOR", out var voiceData))
            {
                (voice, scale) = voiceData;
            }
            else
            {
                (voice, scale) = Main.Settings.SpeechVoices[heroId];
            }

            if (voice == DefaultVoice)
            {
                return;
            }

            var task = Task.Run(async () => await GetPiperTask(executablePath, voice, scale, inputText));
            var audioStream = await task;

            using var waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22050, 1));

            PlaySpeech(audioSettingsService, waveStream);
        }
        catch (Exception e)
        {
            Main.Error(e);
        }
    }

    internal static async void SpeakNpc(string inputText, GameLocationCharacter character)
    {
        try
        {
            ShutUp();

            // only if audio enabled
            var audioSettingsService = ServiceRepository.GetService<IAudioSettingsService>();

            if (!audioSettingsService.MasterEnabled)
            {
                return;
            }

            if (!Main.Settings.EnableSpeechOnNpcs)
            {
                return;
            }


            // only custom campaigns
            // unity life check...
            if (Gui.GameCampaign)
            {
                if (!Gui.GameCampaign.campaignDefinition.IsUserCampaign)
                {
                    return;
                }
            }

            if (!TryGetExecutablePath(out var executablePath))
            {
                return;
            }

            if (character.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
            {
                return;
            }

            var internalName = rulesetCharacterMonster.MonsterDefinition.Name;
            var scale = 1f;
            var voice = DefaultVoice;

            if (!Main.Settings.ForceModSpeechOnNpcs &&
                CampaignVoices.TryGetValue(internalName, out var voiceData))
            {
                (voice, scale) = voiceData;
            }

            if (Main.Settings.ForceModSpeechOnNpcs ||
                CampaignVoices.Count == 0)
            {
                // assign dub data on a round-robin basis for campaigns without it
                var userNpc = Gui.Session.UserCampaign.UserNpcs.FirstOrDefault(x => x.InternalName == internalName);

                if (userNpc == null)
                {
                    return;
                }

                var npcId = Gui.Session.UserCampaign.UserNpcs.IndexOf(userNpc);

                if (npcId < 0)
                {
                    return;
                }

                switch (FemaleNpcs.Contains(internalName))
                {
                    case true when AvailableFemaleVoices.Count > 0:
                    {
                        voice = AvailableFemaleVoices[npcId % AvailableFemaleVoices.Count];
                        break;
                    }
                    case false when AvailableMaleVoices.Count > 0:
                    {
                        voice = AvailableMaleVoices[npcId % AvailableMaleVoices.Count];
                        break;
                    }
                    default:
                        return;
                }

                scale = Main.Settings.SpeechVoices[0].Item2;
            }

            if (voice == DefaultVoice)
            {
                return;
            }

            var task = Task.Run(async () => await GetPiperTask(executablePath, voice, scale, inputText));
            var audioStream = await task;

            using var waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22050, 1));

            PlaySpeech(audioSettingsService, waveStream);
        }
        catch (Exception e)
        {
            Main.Error(e);
        }
    }

    private static void PlaySpeech(IAudioSettingsService audioSettingsService, WaveStream waveStream)
    {
        waveStream.Position = 0;

        WaveOutEvent.Init(waveStream);
        WaveOutEvent.Volume = audioSettingsService.MasterVolume * audioSettingsService.VoicesVolume;
        WaveOutEvent.Play();
    }

    private static async Task<MemoryStream> GetPiperTask(
        string executablePath, string voiceName, float scale, string inputText)
    {
        var audioStream = new MemoryStream();
        var buffer = new byte[16384];
        var modelFileName = Path.Combine(VoicesFolder, voiceName + ".onnx");
        var piper = new Process();
        int bytesRead;

        piper.StartInfo.FileName = executablePath;
        piper.StartInfo.Arguments = $"--model \"{modelFileName}\" --length_scale {scale:F} --output-raw";
        piper.StartInfo.UseShellExecute = false;
        piper.StartInfo.CreateNoWindow = true;
        piper.StartInfo.RedirectStandardInput = true;
        piper.StartInfo.RedirectStandardOutput = true;
        piper.Start();

        using var writer = piper.StandardInput;

        await writer.WriteAsync(StripXmlTagsAndNarration(inputText));
        writer.Close();

        while ((bytesRead = await piper.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            audioStream.Write(buffer, 0, bytesRead);
        }

        return audioStream;
    }

    private enum Gender { Male, Female }

    internal sealed class VoicesDownloader : MonoBehaviour
    {
        private static VoicesDownloader _shared;
        private IEnumerator _coroutine;

        private float _progress;

        [NotNull]
        internal static VoicesDownloader Shared
        {
            get
            {
                if (_shared)
                {
                    return _shared;
                }

                _shared = new GameObject().AddComponent<VoicesDownloader>();
                DontDestroyOnLoad(_shared.gameObject);

                _shared._coroutine = null;

                return _shared;
            }
        }

        internal string GetButtonLabel()
        {
            return _coroutine != null
                ? Gui.Format("ModUi/&DownloadVoiceOngoing", $"{_progress:00.0%}").Bold().Khaki()
                : Gui.Localize("ModUi/&DownloadVoice");
        }

        private void UpdateProgress(ref int loaded, int total)
        {
            if (total <= 0)
            {
                _progress = 0.0f;
                return;
            }

            _progress = loaded++ / (float)total;
        }

        private IEnumerator DownloadVoicesImpl()
        {
            if (!Directory.Exists(VoicesFolder))
            {
                Directory.CreateDirectory(VoicesFolder);
            }

            var current = 0;
            var total = SuggestedVoicesUrls.Length;

            foreach (var (voice, _) in SuggestedVoicesUrls)
            {
                yield return null;

                UpdateProgress(ref current, total);
                DownloadVoice(voice);
            }

            RefreshAvailableVoices();
            StopCoroutine(_coroutine);
            _coroutine = null;
            _progress = 0f;
        }

        internal void DownloadVoices()
        {
            if (_coroutine != null)
            {
                return;
            }

            _progress = 0f;
            _coroutine = DownloadVoicesImpl();
            StartCoroutine(_coroutine);
        }

        private static void DownloadVoice(string voice)
        {
            using var wc = new WebClient();

            var message = $"Voice {voice} successfully downloaded";
            var model = $"{voice}.onnx";
            var modelFilename = Path.GetFileName(model);
            var fullModelFilename = Path.Combine(VoicesFolder, modelFilename);
            var modelUrl = $"{model}?download=true";

            try
            {
                if (!File.Exists(fullModelFilename))
                {
                    wc.DownloadFile(modelUrl, fullModelFilename);

                    var json = $"{voice}.onnx.json";
                    var jsonFilename = Path.GetFileName(json);
                    var fullJsonFilename = Path.Combine(VoicesFolder, jsonFilename);
                    var jsonUrl = $"{json}?download=true";

                    var voiceNames = VoiceNames;

                    Array.Resize(ref voiceNames, VoiceNames.Length + 1);
                    VoiceNames = voiceNames;
                    VoiceNames[VoiceNames.Length - 1] = Path.GetFileName(voice);

                    if (!File.Exists(fullJsonFilename))
                    {
                        wc.DownloadFile(jsonUrl, fullJsonFilename);
                    }
                    else
                    {
                        message = $"Voice settings {voice} already exists.";
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
