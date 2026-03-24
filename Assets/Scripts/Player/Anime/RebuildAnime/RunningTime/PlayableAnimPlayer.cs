using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace NoWireAnim
{
    [RequireComponent(typeof(Animator))]
    public sealed class PlayableAnimPlayer : MonoBehaviour {
        [Header("Visualizer")]
        [SerializeField] private bool registerToVisualizer = true;
        [SerializeField] private string graphName = "NoWireAnimGraph";
        private Animator animator;
        private PlayableGraph graph;
        private AnimationPlayableOutput output;
        private AnimationMixerPlayable mixer;

        private readonly AnimationClipPlayable[] slots = new AnimationClipPlayable[2];
        private int currentSlot = 0;
        private bool isFading;
        private float fadeDuration;
        private float fadeTimer;
        private int fromSlot;
        private int toSlot;

        public bool IsReady => graph.IsValid();
        public bool IsTransitioning => isFading;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            BuildGraph();
        }

        private void Update()
        {
            UpdateFade(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (registerToVisualizer)
            {
                PlayableGraphVisualizerBridge.Show(graph);
            }
        }

        private void OnDestroy()
        {
            if (!graph.IsValid())
                return;

            PlayableGraphVisualizerBridge.Hide(graph);
            graph.Destroy();
        }

        private void BuildGraph()
        {
            string finalName = string.IsNullOrWhiteSpace(graphName)
                ? $"{name}NoWireAnimGraph"
                : graphName;

            graph = PlayableGraph.Create(finalName);

            output = AnimationPlayableOutput.Create(graph, "AnimationOutput", animator);
            mixer = AnimationMixerPlayable.Create(graph, 2);

            output.SetSourcePlayable(mixer);

            mixer.SetInputWeight(0, 1f);
            mixer.SetInputWeight(1, 0f);

            graph.Play();

            if (registerToVisualizer)
            {
                PlayableGraphVisualizerBridge.Show(graph);
            }
        }

        public void PlayImmediate(AnimationClip clip, float speed = 1f)
        {
            if (clip == null)
                return;

            ReplaceSlot(currentSlot, clip, speed);

            mixer.SetInputWeight(currentSlot, 1f);
            mixer.SetInputWeight(1 - currentSlot, 0f);

            isFading = false;
            fadeTimer = 0f;
        }

        public void CrossFade(AnimationClip clip, float duration, float speed = 1f)
        {
            if (clip == null)
                return;

            fromSlot = currentSlot;
            toSlot = 1 - currentSlot;

            ReplaceSlot(toSlot, clip, speed);

            fadeDuration = Mathf.Max(0.0001f, duration);
            fadeTimer = 0f;
            isFading = true;
        }

        private void UpdateFade(float deltaTime)
        {
            if (!isFading)
                return;

            fadeTimer += deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);

            mixer.SetInputWeight(fromSlot, 1f - t);
            mixer.SetInputWeight(toSlot, t);

            if (t >= 1f)
            {
                currentSlot = toSlot;
                mixer.SetInputWeight(1 - currentSlot, 0f);
                isFading = false;
            }
        }

        private void ReplaceSlot(int slotIndex, AnimationClip clip, float speed)
        {
            if (slots[slotIndex].IsValid())
            {
                graph.Disconnect(mixer, slotIndex);
                graph.DestroyPlayable(slots[slotIndex]);
                slots[slotIndex] = default;
            }

            var playable = AnimationClipPlayable.Create(graph, clip);
            playable.SetTime(0);
            playable.SetSpeed(speed);

            graph.Connect(playable, 0, mixer, slotIndex);

            if (slotIndex == currentSlot)
                mixer.SetInputWeight(slotIndex, 1f);
            else
                mixer.SetInputWeight(slotIndex, 0f);

            slots[slotIndex] = playable;
        }
    }
}
