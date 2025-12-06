using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

namespace HonestJam8.Narration {

    public partial class NarrationManager : Node {
        private readonly List<NarrationItems> PreviousNarrationItems = [];
        private readonly List<NarrationItems> NextNarrationItems = [];
        private Label _label;
        private AudioStreamPlayer _audioStreamPlayer;
        private const string AudioPathBase = "res://Audio/Narration/";
        public override void _Ready() {
            _label = GetNode<Label>("Label");
            _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

            // Preload audio
            foreach (var value in Enum.GetValues(typeof(NarrationItems))) {
                ResourceLoader.Load(AudioPathBase + value.ToString() + ".wav");
            }

            _audioStreamPlayer.Finished += () => {
                PlayNextNarrationInQueue();
            };
        }

        [Signal]
        public delegate void SpawnCondemnedEventHandler();


        private void PlayNextNarrationInQueue() {
            if (NextNarrationItems.Count == 0 || _audioStreamPlayer.Playing) {
                return;
            }

            var currentItem = NextNarrationItems.First();
            NextNarrationItems.Remove(currentItem);

            var sound = (AudioStreamWav)ResourceLoader.Load(AudioPathBase + currentItem.ToString() + ".wav");

            _audioStreamPlayer.Stream = sound;
            _audioStreamPlayer.Play(0);

            _label.Text = narrationTexts[currentItem];

            PreviousNarrationItems.Add(currentItem);
        }

        private void AddNarrationItemIfNotAlreadyDone(NarrationItems narrationItem, List<NarrationItems> skipIfItemDone) {
            foreach (var item in skipIfItemDone) {
                if (NextNarrationItems.Contains(item)) {
                    return;
                }
                if (PreviousNarrationItems.Contains(item)) {
                    return;
                }
            }

            NextNarrationItems.Add(narrationItem);
        }

        public void OnAreaTriggered(AreaName areaName) {
            switch (areaName) {
                case AreaName.Spawn:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Spawn_Hello, [NarrationItems.Spawn_Hello]);
                    break;
                case AreaName.Corridor_SP_Z1:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Spawn_Zone1, [NarrationItems.Spawn_Zone1, NarrationItems.Spawn_Zone2]);
                    break;
                case AreaName.Corridor_SP_Z2:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Spawn_Zone2, [NarrationItems.Spawn_Zone2, NarrationItems.Spawn_Zone1]);
                    break;
                case AreaName.Zone1:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Zone1, [NarrationItems.Zone1]);
                    break;
                case AreaName.Zone2:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Zone2, [NarrationItems.Zone2]);
                    break;
                case AreaName.Vent:
                    if (PreviousNarrationItems.Contains(NarrationItems.Zone2)) {
                        AddNarrationItemIfNotAlreadyDone(NarrationItems.Zone2_Zone1, [NarrationItems.Zone2_Zone1]);
                    } else {
                        AddNarrationItemIfNotAlreadyDone(NarrationItems.Zone1_Zone2, [NarrationItems.Zone1_Zone2]);
                    }
                    break;
                case AreaName.Corridor_Z1_EP:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.Zone1_EvacPod, [NarrationItems.Zone1_EvacPod]);
                    break;
                case AreaName.EscapePod:
                    AddNarrationItemIfNotAlreadyDone(NarrationItems.EvacPod, [NarrationItems.EvacPod]);
                    break;
            }

            PlayNextNarrationInQueue();
        }

        public static readonly Dictionary<NarrationItems, string> narrationTexts = new() {
            { NarrationItems.Spawn_Hello, """
                Crew member, you’ve been unconscious for 9 9 9 9 9 9…
                [glitch]
                Minor system fault. Welcome to the post major-incident helper program. We hope you'll have a good experience with us.
                You have been out for 9  minutes. The ship suffered a major incident. Follow the left door for the primary recovery route.
                """ },
            { NarrationItems.Spawn_Zone1, """
                Left door selected. No brain damage detected. Proceed to the secondary control room.
                """ },
            { NarrationItems.Spawn_Zone2, """
                [explosion]
                Right door selected. That was the other left… Previous room depressurized. Proceed to storage room C for an alternative way.
                """ },
            { NarrationItems.Zone1, """
                You're in the auxilliary control room. In order to save the ship, you'll need to press the blue button to restart the communication array.
                """ },
            { NarrationItems.Zone2, """
                Please use the vent to join the right room. This indication should not be hard to follow.
                """ },
            { NarrationItems.Zone2_Zone1, """
                Crawling through vents is highly inefficient, but you enjoy wasting my processing cycles, don’t you?
                """ },
            { NarrationItems.Zone1_Zone2, """
                What ... What are you doing ? It's the wrong way!
                """ },
            { NarrationItems.Zone1_EvacPod, """
                Hurry up, the ship is not safe.
                """ },
            { NarrationItems.EvacPod, """
                You have reached the evacuation pod. Please enter to launch
                """ },
        };
    }
}
