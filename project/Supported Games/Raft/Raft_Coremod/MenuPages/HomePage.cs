using HarmonyLib;
using HMLLibrary;
using SocketIO;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftModLoader
{
    public class HomePage : MenuPage
    {
        public static HomePage instance;
        public static TextMeshProUGUI RandomText;
        public static TextMeshProUGUI achievementText;
        public static Transform featuredMods;

        public static string[] randomTexts = {
        "This text isn't to fill blank space.",
        "Got any mini shields?",
        "This text doesn't exist!",
        "I am out of Ideas.",
        "Wait are you actually reading this?!",
        "Out of messages ideas.",
        "Is this the Krusty Krab??",
        "Welcome to meme land!",
        "Is your mom a pickaxe?",
        "So sad! Alexa, launch raft.",
        "Publishing Half-Life 3...",
        "Do not turn the Power off or remove the Memory Card! ",
        "Leeroooooooooy Jenkins !",
        "Approaching the wild Wumpus.",
        "You do not currently own this DLC",
        "Eating cereal with a fork.",
        "Are You Ready Kids?",
        "Joining the battle.",
        "Shopping for more features.",
        "The cake is a lie.",
        "Deja vu, I’ve just been in this place before!",
        "Setting fire to the rain...",
        "Do you even read these?",
        "We need more power!",
        "Raft: Battle Royale is coming soon!",
        "Are you feeling it now?",
        "Feeding the Wumpus...",
        "Do you know da wae?",
        "Fighting the Fake News -Donald J. Trump",
        "It's prime time b*tch!",
        "Mining those diamonds!",
        "Hold my beer.",
        "Believing in the You that believes in me.",
        "Telling Ghost Pepper to Scream Passion!",
        "Entering The Oasis...",
        "Scoopty-Whooping...",
        "Calibrating the France!",
        "My name is your mom's name.",
        "Team Rocket is blasting off again!",
        "/ban @everyone",
        "Playing past bedtime.",
        "Preparing for the boss...",
        "Pressing buttons - BEEP BOOP!",
        "Eyeballing the clock!",
        "Take me homeeee...Country Roadddddd...",
        "Hmph, I'm the coolest!",
        "MarshMello is playing RocketLeague!",
        "Real People, Not Actors",
        "FAILING TO DISARM BOMB!!",
        "Finding a better Tek",
        "Did xTBolt Finish his castle",
        "Tek is a bad coder btw",
        "Deleting FalseHope",
        "Programming the flux capacitor",
        "would you like fries with that?",
        "The last time I tried this the monkey didn't survive. Let's hope it works better this time",
        "is that Minecraft?",
        "Bruce could be your daddy",
        "OOF",
        "Press Alt+F4 for a quick IQ test",
        "Loading completed. Press F13 to continue.",
        "Water detected on drive C:, please wait. Spin dry commencing.",
        "I'm quite drunk, loading might take a little more time than the usual! Please be patient!",
        "Loading the Loading message..",
        "Calling police.....",
        "Instaling windows 98",
        "Are you mad?",
        "Please, if you enjoy our mods then give us ur moms credit card",
        "Hey, i can fly!",
        "What's the matter? You got somethin' in your eye?",
        "We have to go. I'm almost happy here.",
        "Lemme tell you a story...",
        "Floor is lava!",
    };

        public async void Awake()
        {
            instance = this;
            RandomText = transform.Find("RandomText").Find("text").GetComponent<TextMeshProUGUI>();
            RandomText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Length)];
            featuredMods = transform.Find("FeaturedMods");
            achievementText = transform.Find("Achievements").Find("Background").Find("Stat").GetComponent<TextMeshProUGUI>();
            transform.Find("Links").Find("website").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.raftmodding.com"));
            transform.Find("Links").Find("discord").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.raftmodding.com/discord"));
            transform.Find("Links").Find("patreon").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.patreon.com/hytekgames"));
            transform.Find("Links").Find("documentation").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://api.raftmodding.com"));
            transform.Find("Links").Find("facebook").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.facebook.com/ModdingRaft"));
            await Task.Delay(1000);
            achievementText.text = "You've died " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_deaths").GetValue() + " times, used your hook " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_hookCount").GetValue() + " times, placed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_build_foundationCount").GetValue() + " foundations, killed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_sharkKills").GetValue() + " sharks, killed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_birdKills").GetValue() + " seagulls, and captured " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_capturedAnimals").GetValue() + " animals.";
        }

        public async override void OnPageOpen()
        {
            RandomText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Length)];
            achievementText.text = "You've died " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_deaths").GetValue() + " times, used your hook " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_hookCount").GetValue() + " times, placed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_build_foundationCount").GetValue() + " foundations, killed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_sharkKills").GetValue() + " sharks, killed " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_birdKills").GetValue() + " seagulls, and captured " + Traverse.Create(typeof(AchievementHandler)).Field("Stat_player_capturedAnimals").GetValue() + " animals.";
            while (RSocket.socket == null) { await Task.Delay(100); }
            if (RSocket.socket.IsConnected && RSocket.socket.IsWsConnected)
            {
                RSocket.socket.Emit("server:GetHomepageInfo");
            }
        }

        public void OnHomepageInfoFromServer(SocketIOEvent e)
        {
            for (int i = 1; i < 6; i++)
            {
                featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("Name").GetComponent<TextMeshProUGUI>().text = e.data["featuredMods"][i - 1]["title"].str;
                featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("Description").GetComponent<TextMeshProUGUI>().text = e.data["featuredMods"][i - 1]["description"].str;
                featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("viewmod").GetComponent<Button>().interactable = true;
                featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("viewmod").GetComponent<Button>().onClick.RemoveAllListeners();
                string url = e.data["featuredMods"][i - 1]["id"].str;
                featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("viewmod").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.raftmodding.com/mods/" + url));

                RawImage img = featuredMods.Find("modlist").Find("FeaturedMod" + i).Find("ModIcon").GetComponent<RawImage>();
                HUtils.DownloadUncachedTexture(e.data["featuredMods"][i - 1]["iconImageUrl"].str).ContinueWith((t) =>
                  {
                      img.texture = t.Result;
                  });
            }

            string readme = e.data["latestChangelog"]["readme"].str.Replace("\\r", "\r").Replace("\\n", "\n");
            transform.Find("News").Find("Description").GetComponent<TextMeshProUGUI>().text = readme;
            transform.Find("News").Find("Name").GetComponent<TextMeshProUGUI>().text = "RaftModLoader " + e.data["latestChangelog"]["rmlVersion"].str;
            transform.Find("News").Find("Time").GetComponent<TextMeshProUGUI>().text = HUtils.GetDateFormatted(e.data["latestChangelog"]["createdAt"].str);
            transform.Find("News").Find("view").GetComponent<Button>().onClick.RemoveAllListeners();
            transform.Find("News").Find("view").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.raftmodding.com/loader/" + e.data["latestChangelog"]["rmlVersion"].str));
        }
    }
}