using System;
using ClassicalSharp;
using OpenTK;
using OpenTK.Input;
using ClassicalSharp.Entities;
using ClassicalSharp.Events;
using System.Net;
using Newtonsoft.Json;
using Microsoft.CSharp;
using Newtonsoft.Json.Linq;

namespace JokePlugin {

    public class Core : Plugin {

        public string ClientVersion { get { return "0.99.9.96"; } }

        public void Dispose() {

        }

        public void Init(Game game) {
            game.CommandList.Register(new ChuckCommand());
            game.CommandList.Register(new AdviceCommand());
            game.CommandList.Register(new QuotesCommand());
            game.CommandList.Register(new InsultCommand());
            game.CommandList.Register(new JokeCommand());
            game.CommandList.Register(new RandomCommand());
            game.CommandList.Register(new FMLCommand());
            game.CommandList.Register(new PROGCommand());
        }

        public void Ready(Game game) {
        }

        public void Reset(Game game) { }

        public void OnNewMap(Game game) {
        }

        public void OnNewMapLoaded(Game game) {
        }


    }

    //Chuck Norris
    internal class Chuck
    {
        public string type;
        public ChuckValue value;

    }

    internal class ChuckValue
    {
        public int id;
        public string joke;
        public string[] categories;
    }
    //

    //Advice Norris
    internal class Advice
    {
        public AdviceValue slip;

    }

    internal class AdviceValue
    {
        public int slip_id;
        public string advice;
    }
    //

    //Joke
    internal class Joke
    {
        public JokeValue[] attachments;
        public string response_type;
        public string username;

    }

    internal class JokeValue
    {
        public string text;
        public string fallback;
        public string footer;
    }
    //


    public class ChuckCommand : ClassicalSharp.Commands.Command
    {

        public ChuckCommand()
        {
            Name = "chuck";
            Help = new string[] {
                "&a/client chuck",
                "&eSend a Chuck Norris joke in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var json = new WebClient().DownloadString("http://api.icndb.com/jokes/random");

            Chuck deserialized = JsonConvert.DeserializeObject<Chuck>(json);

            game.Server.SendChat(deserialized.value.joke);

        }
    }

    public class AdviceCommand : ClassicalSharp.Commands.Command
    {

        public AdviceCommand()
        {
            Name = "advice";
            Help = new string[] {
                "&a/client advice",
                "&eSend an advice in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var json = new WebClient().DownloadString("http://api.adviceslip.com/advice");

            Advice deserialized = JsonConvert.DeserializeObject<Advice>(json);

            deserialized.slip.advice = StringExtensions.StripIncompatableQuotes(deserialized.slip.advice);
            game.Server.SendChat(deserialized.slip.advice);

        }
    }

    public class QuotesCommand : ClassicalSharp.Commands.Command
    {

        public QuotesCommand()
        {
            Name = "quote";
            Help = new string[] {
                "&a/client quote",
                "&eSend a quote in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var json = new WebClient().DownloadString("https://ron-swanson-quotes.herokuapp.com/v2/quotes");

            string[] deserialized = JsonConvert.DeserializeObject<string[]>(json);

            deserialized[0] = StringExtensions.StripIncompatableQuotes(deserialized[0]);
            game.Server.SendChat(deserialized[0]);

        }
    }

    public class InsultCommand : ClassicalSharp.Commands.Command
    {

        public InsultCommand()
        {
            Name = "insult";
            Help = new string[] {
                "&a/client insult",
                "&eSend an insult in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var json = new WebClient().DownloadString("https://insult.mattbas.org/api/insult");

            game.Server.SendChat(json);

        }
    }

    public class JokeCommand : ClassicalSharp.Commands.Command
    {

        public JokeCommand()
        {
            Name = "joke";
            Help = new string[] {
                "&a/client joke",
                "&eSend a joke in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var json = new WebClient().DownloadString("https://icanhazdadjoke.com/slack");

            Joke deserialized = JsonConvert.DeserializeObject<Joke>(json);

            game.Server.SendChat(deserialized.attachments[0].text);

        }
    }

    public class RandomCommand : ClassicalSharp.Commands.Command
    {

        string[] RawArgs;
        int count = 0;

        public RandomCommand()
        {
            Name = "random";
            Help = new string[] {
                "&a/client random",
                "&eSend a completly random thing in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "MockClient/0.1 by Me");

            if (count > 15) { return; }

            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(exeEvent);
            client.DownloadStringAsync(new Uri("http://api.reddit.com/random"));

        }

        public void exeEvent(object s, DownloadStringCompletedEventArgs e) {
            JToken token = JArray.Parse(e.Result);
            string text = (string)token.SelectToken("[0].data.children[0].data.title");



            if (text.Length > 150) { count++; this.Execute(RawArgs); } else {

                count = 0;
                text = StringExtensions.StripIncompatableQuotes(text);
                game.Server.SendChat(text);
            }
        }
    }

    public class FMLCommand : ClassicalSharp.Commands.Command
    {



        public FMLCommand()
        {
            Name = "fml";
            Help = new string[] {
                "&a/client fml",
                "&eSend a FML quote in the chat.",
            };
        }

        public override void Execute(string[] args)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "MockClient/0.1 by Me");


            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(exeEvent);
            client.DownloadStringAsync(new Uri("http://api.reddit.com/r/FML/random"));

        }

        public void exeEvent(object s, DownloadStringCompletedEventArgs e)
        {
            JToken token = JArray.Parse(e.Result);
            string text = (string)token.SelectToken("[0].data.children[0].data.title");

            text = StringExtensions.StripIncompatableQuotes(text);
            game.Server.SendChat(text);
            
        }
    }

    public class PROGCommand : ClassicalSharp.Commands.Command
    {



        public PROGCommand()
        {
            Name = "progjerk";
            Help = new string[] {
                "&a/client progjerk",
                "&eSend a jerk quote about programming.",
            };
        }

        public override void Execute(string[] args)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "MockClient/0.1 by Me");


            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(exeEvent);
            client.DownloadStringAsync(new Uri("http://api.reddit.com/r/programmingcirclejerk/random"));

        }

        public void exeEvent(object s, DownloadStringCompletedEventArgs e)
        {
            JToken token = JArray.Parse(e.Result);
            string text = (string)token.SelectToken("[0].data.children[0].data.title");

            text = StringExtensions.StripIncompatableQuotes(text);
            game.Server.SendChat(text);

        }
    }


    public static class StringExtensions
        {
            public static string StripIncompatableQuotes(string s)
            {
                if (!string.IsNullOrEmpty(s))
                    return s.Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"');
                else
                    return s;
            }
        }
    

}
