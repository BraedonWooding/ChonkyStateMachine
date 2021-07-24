using BenchmarkDotNet.Attributes;
using ChonkyStateMachine;
using Stateless;
using System;
using System.Collections.Generic;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        string[] states = new string[] { "prevent", "fairies", "discover", "limping", "slope", "respect", "found", "lively", "teeny-tiny", "open", "travel", "promise", "draconian", "tendency", "long", "itch", "actor", "vacuous", "town", "abounding", "available", "credit", "copper", "park", "girl", "strap", "steadfast", "wave", "payment", "smile", "smash", "aberrant", "cemetery", "scary", "tomatoes", "snail", "pink", "annoying", "wasteful", "program", "crack", "trap", "wax", "fuel", "grandiose", "puncture", "race", "bless", "naive" };
        string[] transitions = new string[800] { "cultured", "damp", "divergent", "penitent", "healthy", "quiet", "wooden", "wild", "long-term", "husky", "high-pitched", "spicy", "second", "tremendous", "reflective", "dynamic", "incandescent", "ready", "fearful", "gusty", "groovy", "guarded", "sneaky", "ten", "cruel", "adorable", "frantic", "aboriginal", "stingy", "faint", "green", "ethereal", "fanatical", "mundane", "picayune", "thinkable", "woozy", "bright", "violet", "ubiquitous", "painful", "trashy", "guiltless", "youthful", "silly", "ugliest", "immense", "productive", "temporary", "needless", "bite-sized", "serious", "encouraging", "well-to-do", "obese", "irritating", "oafish", "lackadaisical", "ahead", "jumpy", "grey", "fumbling", "bad", "new", "whispering", "rambunctious", "busy", "cute", "daffy", "elfin", "blue", "rebel", "tasty", "humorous", "overconfident", "nauseating", "slim", "fresh", "idiotic", "distinct", "accidental", "untidy", "glamorous", "irate", "astonishing", "silky", "measly", "available", "colossal", "addicted", "handy", "coordinated", "cloudy", "flagrant", "heady", "exultant", "shivering", "trite", "naughty", "second-hand", "violent", "innocent", "utter", "majestic", "tacky", "lonely", "apathetic", "glorious", "boundless", "brash", "merciful", "tedious", "fertile", "numberless", "next", "better", "complex", "ludicrous", "spooky", "humdrum", "important", "frightening", "stiff", "willing", "one", "terrible", "observant", "unwritten", "knowing", "wistful", "uptight", "strange", "worried", "gaping", "parched", "brawny", "guttural", "craven", "overjoyed", "nasty", "vacuous", "thirsty", "ill-informed", "hushed", "sedate", "satisfying", "raspy", "cut", "deadpan", "dramatic", "chemical", "nervous", "like", "happy", "fixed", "skinny", "flawless", "faded", "far-flung", "utopian", "light", "warlike", "oceanic", "outstanding", "likeable", "conscious", "savory", "venomous", "two", "brave", "decorous", "foregoing", "volatile", "imported", "hissing", "uninterested", "thankful", "lewd", "tiny", "phobic", "empty", "clear", "adamant", "lacking", "fretful", "dry", "aquatic", "plausible", "level", "knotty", "ill", "obsequious", "delicious", "parallel", "cool", "tested", "eminent", "caring", "broken", "hateful", "physical", "secretive", "spurious", "exuberant", "windy", "swift", "frail", "female", "silent", "full", "little", "alleged", "square", "overrated", "deafening", "lucky", "amazing", "sour", "luxuriant", "familiar", "tawdry", "stupid", "thoughtless", "keen", "noxious", "efficient", "axiomatic", "responsible", "unaccountable", "elated", "befitting", "statuesque", "undesirable", "ripe", "clumsy", "dependent", "acoustic", "feigned", "jaded", "pointless", "old", "adventurous", "sore", "threatening", "good", "demonic", "assorted", "glistening", "smelly", "teeny", "rightful", "toothsome", "royal", "well-made", "mere", "filthy", "joyous", "closed", "impartial", "open", "nosy", "hungry", "known", "victorious", "shallow", "laughable", "alive", "jobless", "polite", "waggish", "fluttering", "defiant", "imperfect", "silent", "abnormal", "many", "thin", "wealthy", "cloistered", "terrific", "languid", "billowy", "shaggy", "steadfast", "four", "comfortable", "shaky", "grouchy", "halting", "pink", "paltry", "puny", "aberrant", "tricky", "belligerent", "obnoxious", "wide-eyed", "thirsty", "chief", "didactic", "sore", "clever", "outrageous", "pleasant", "rapid", "noiseless", "regular", "dashing", "learned", "best", "ajar", "torpid", "earsplitting", "jazzy", "snobbish", "kindly", "future", "endurable", "nifty", "overwrought", "tangible", "neighborly", "witty", "red", "foolish", "incompetent", "festive", "first", "offbeat", "nutty", "dreary", "bored", "towering", "fuzzy", "insidious", "lame", "moaning", "bitter", "mixed", "remarkable", "superficial", "gray", "jumbled", "arrogant", "short", "tacit", "private", "dysfunctional", "changeable", "successful", "wiggly", "messy", "educated", "hollow", "elastic", "seemly", "flippant", "abject", "sleepy", "panoramic", "barbarous", "labored", "incredible", "ablaze", "maddening", "chivalrous", "right", "berserk", "bright", "lazy", "unkempt", "plastic", "homely", "obsolete", "dear", "knowledgeable", "wrong", "giant", "shy", "greasy", "itchy", "helpful", "awful", "upbeat", "quarrelsome", "magical", "watery", "righteous", "wholesale", "deeply", "mammoth", "slippery", "wrathful", "feeble", "agonizing", "fine", "general", "resolute", "lowly", "nostalgic", "brainy", "dusty", "abhorrent", "handsomely", "thick", "smart", "cagey", "useful", "misty", "tired", "yielding", "null", "useless", "colorful", "ad hoc", "burly", "acid", "annoyed", "great", "gullible", "lying", "scary", "early", "mindless", "hot", "ruthless", "poised", "broad", "spotted", "extra-large", "automatic", "calculating", "detailed", "marked", "quick", "sincere", "organic", "literate", "grumpy", "nappy", "stale", "nine", "evasive", "petite", "industrious", "pushy", "workable", "taboo", "hideous", "mean", "warm", "natural", "nonchalant", "homeless", "chubby", "plant", "harmonious", "permissible", "salty", "scrawny", "abashed", "extra-small", "minor", "thoughtful", "hesitant", "aspiring", "tightfisted", "vivacious", "superb", "complete", "grateful", "macho", "evanescent", "naive", "disagreeable", "super", "courageous", "gabby", "curious", "truthful", "absorbed", "soft", "nutritious", "bewildered", "equable", "cowardly", "intelligent", "dangerous", "obtainable", "huge", "roasted", "sudden", "quaint", "rampant", "elderly", "exclusive", "lovely", "creepy", "excited", "disturbed", "cheerful", "sable", "aggressive", "mighty", "functional", "nimble", "lavish", "free", "annoying", "previous", "impolite", "voiceless", "tranquil", "ashamed", "spiteful", "half", "furry", "abundant", "average", "panicky", "kind", "amused", "disastrous", "stimulating", "near", "historical", "lean", "obeisant", "absurd", "shrill", "psychotic", "accessible", "depressed", "efficacious", "wandering", "greedy", "fair", "combative", "mountainous", "rhetorical", "rural", "bashful", "confused", "unnatural", "symptomatic", "dull", "cluttered", "last", "large", "lively", "internal", "enchanted", "kaput", "simplistic", "black-and-white", "eatable", "teeny-tiny", "flashy", "zippy", "alike", "ordinary", "impossible", "magenta", "mysterious", "late", "mute", "obscene", "imminent", "afraid", "bustling", "jittery", "gratis", "worthless", "robust", "highfalutin", "troubled", "cautious", "enchanting", "pastoral", "envious", "mellow", "infamous", "cold", "verdant", "loutish", "beneficial", "common", "malicious", "tough", "noisy", "juicy", "optimal", "daily", "awake", "delicate", "shut", "living", "straight", "rare", "melodic", "striped", "gifted", "succinct", "wide", "capable", "elite", "electric", "purring", "uncovered", "chilly", "heartbreaking", "classy", "loud", "true", "glib", "different", "hard", "third", "wiry", "eight", "married", "calm", "rotten", "hard-to-find", "acrid", "racial", "careful", "same", "vulgar", "gigantic", "sophisticated", "real", "nondescript", "yummy", "wet", "peaceful", "ill-fated", "talented", "wonderful", "judicious", "quack", "puzzling", "profuse", "unable", "bent", "massive", "splendid", "descriptive", "necessary", "uneven", "marvelous", "hanging", "tearful", "discreet", "redundant", "bawdy", "quickest", "deranged", "romantic", "steady", "draconian", "sassy", "versed", "gorgeous", "thundering", "damaged", "earthy", "futuristic", "anxious", "ignorant", "miscreant", "grubby", "normal", "diligent", "defective", "murky", "enthusiastic", "modern", "shiny", "purple", "rustic", "wry", "perpetual", "abortive", "nippy", "oval", "hapless", "placid", "innate", "cooperative", "understood", "loose", "elegant", "finicky", "unsuitable", "wretched", "upset", "adaptable", "deserted", "scintillating", "supreme", "faulty", "entertaining", "secret", "breakable", "abrasive", "pathetic", "flimsy", "fallacious", "staking", "synonymous", "neat", "nebulous", "public", "proud", "cumbersome", "unequaled", "milky", "stupendous", "jolly", "tasteless", "plain", "moldy", "excellent", "inexpensive", "stormy", "prickly", "snotty", "powerful", "energetic", "zonked", "aloof", "orange", "furtive", "stereotyped", "ruddy", "tense", "blue-eyed", "interesting", "brief", "actually", "blushing", "frightened", "fragile", "graceful", "damaging", "selfish", "forgetful", "political", "material", "holistic", "acceptable", "grieving", "imaginary", "expensive", "six", "lush", "illustrious", "meaty", "crazy", "wanting", "spotless", "smoggy", "lively", "receptive", "tasteful", "slimy", "friendly", "unadvised", "hellish", "fluffy", "cuddly", "melted", "devilish", "unarmed", "probable", "garrulous", "famous", "present", "quizzical", "whole", "sloppy", "pale", "concerned", "shocking", "weak", "valuable", "voracious", "lethal", "ceaseless", "well-off", "sweltering", "sulky", "sturdy", "steep", "grandiose", "jagged", "fearless", "domineering", "eager" };

        ChonkyStateMachine.StateMachine<string, string> sm;
        List<(string, string, string)> possibleTransitions = new List<(string, string, string)>();
        List<(string, string, string)> possibleTransitions2 = new List<(string, string, string)>();

        Stateless.StateMachine<string, string> stateless_sm;

        [GlobalSetup]
        public void SetupStateMachine()
        {
            var smBuilder = new StateMachineBuilder<string, string>(true);
            var rand = new Random();
            var prevState = "burst";

            for (int i = 0; i < transitions.Length; i++)
            {
                string from = prevState, to = states[rand.Next(0, states.Length)];
                if (possibleTransitions.Contains((transitions[i], from, to)) == false)
                {
                    prevState = to;
                    possibleTransitions.Add((transitions[i], from, to));
                    smBuilder.AddTransition(transitions[i], from, to);
                }
                else
                {
                    i--;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                string from = prevState, to = states[rand.Next(0, states.Length)];
                if (possibleTransitions2.Contains((transitions[i], from, to)) == false)
                {
                    prevState = to;
                    possibleTransitions2.Add((transitions[i], from, to));
                }
                else
                {
                    i--;
                }
            }

            sm = smBuilder.Construct();

            stateless_sm = new Stateless.StateMachine<string, string>("burst");

            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                var cfg = stateless_sm.Configure(from);
                if (to == from) cfg.PermitReentry(trigger);
                else cfg.Permit(trigger, to);
            }

            stateless_sm.Configure("burst")
                .PermitReentry("reset");
            stateless_sm.Configure(possibleTransitions[possibleTransitions.Count - 1].Item3)
                .Permit("reset", "burst");
        }

        [Benchmark]
        public void StatelessCreateNever()
        {
            stateless_sm.Fire("reset");
            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                stateless_sm.Fire(trigger);
            }
        }

        [Benchmark]
        public void StatelessCreateEveryTime()
        {
            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                var tmp = new Stateless.StateMachine<string, string>(from);

                foreach ((string inner_trigger, string inner_from, string inner_to) in possibleTransitions)
                {
                    var cfg = tmp.Configure(inner_from);
                    if (inner_to == inner_from) cfg.PermitReentry(inner_trigger);
                    else cfg.Permit(inner_trigger, inner_to);
                }

                tmp.Fire(trigger);
            }
        }

        [Benchmark]
        public void StatelessCreateOnce()
        {
            var tmp = new Stateless.StateMachine<string, string>("burst");

            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                var cfg = tmp.Configure(from);
                if (to == from) cfg.PermitReentry(trigger);
                else cfg.Permit(trigger, to);
            }

            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                tmp.Fire(trigger);
            }
        }

        [Benchmark]
        public void StatelessCostConstructor()
        {
            var tmp = new Stateless.StateMachine<string, string>("burst");
            foreach ((string inner_trigger, string inner_from, string inner_to) in possibleTransitions2)
            {
                var cfg = tmp.Configure(inner_from);
                if (inner_to == inner_from) cfg.PermitReentry(inner_trigger);
                else cfg.Permit(inner_trigger, inner_to);
            }
        }

        [Benchmark]
        public void ChonkyCreateEverytime()
        {
            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                var smBuilder = new ChonkyStateMachine.StateMachineBuilder<string, string>(true);
                foreach ((string inner_trigger, string inner_from, string inner_to) in possibleTransitions)
                {
                    smBuilder.AddTransition(inner_trigger, inner_from, inner_to);
                }
                var sm = smBuilder.Construct();

                sm.NextState(from, trigger);
            }
        }

        [Benchmark]
        public void ChonkyCreateOnce()
        {
            var smBuilder = new ChonkyStateMachine.StateMachineBuilder<string, string>(true);
            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                smBuilder.AddTransition(trigger, from, to);
            }
            var sm = smBuilder.Construct();

            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                sm.NextState(from, trigger);
            }
        }

        [Benchmark]
        public void ChonkyCreateNever()
        {
            foreach ((string trigger, string from, string to) in possibleTransitions)
            {
                sm.NextState(from, trigger);
            }
        }

        [Benchmark]
        public void ChonkyCostConstructor()
        {
            var tmp = new ChonkyStateMachine.StateMachineBuilder<string, string>(true);
            foreach ((string inner_trigger, string inner_from, string inner_to) in possibleTransitions2)
            {
                tmp.AddTransition(inner_trigger, inner_from, inner_to);
            }
        }
    }
}