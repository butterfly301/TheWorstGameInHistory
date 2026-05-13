using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoBitMachines.FlareEngine
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] public Image icon;
        [SerializeField] public Image background;
        [SerializeField] public Image nextSignal;
        [SerializeField] public TextMeshProUGUI messenger;
        [SerializeField] public TextMeshProUGUI message;
        [SerializeField] public List<Button> choices = new();

        [SerializeField] public UnityEvent onBegin = new();
        [SerializeField] public UnityEvent onEnd = new();
        [SerializeField] public UnityEvent transitionIn = new();
        [SerializeField] public UnityEvent transitionOut = new();
        [SerializeField] public UnityEvent loadMessage = new();
        [SerializeField] public UnityEvent messageEvent;
        [NonSerialized] private bool beginConversation;
        [NonSerialized] private MessagePosition boxDir;
        [NonSerialized] public bool canAnimate;
        [NonSerialized] private string currentActor = "";
        [NonSerialized] public Sprite defaultBackground;
        [NonSerialized] private bool endConversation;
        [NonSerialized] public bool ignoreBoxFlip;

        [NonSerialized] public bool isLoading;
        [NonSerialized] public Sprite nextBackground;
        [NonSerialized] private List<Choice> nextChoice;

        [NonSerialized] public Sprite nextIcon;
        [NonSerialized] public string nextMessage;

        [NonSerialized] private MessageType nextMessageType;
        [NonSerialized] public string nextMessengerName;
        [NonSerialized] private MessagePosition oldBoxDir;

        private bool noLoadMessage =>
            loadMessage.GetPersistentEventCount() ==
            0; // this will fail if event exists but object is not set. All events must be properly setup, or deleted

        private bool noTransitionIn => transitionIn.GetPersistentEventCount() == 0;
        private bool noTransitionOut => transitionOut.GetPersistentEventCount() == 0;

        private void Awake()
        {
            if (background != null) defaultBackground = background.sprite;
        }

        public void StartMessage(MessageType type, Sprite icon, Sprite background, string name, string message,
            List<Choice> choice, MessagePosition boxDirection, UnityEvent messageEvent)
        {
            nextIcon = icon;
            nextMessengerName = name;
            nextMessage = message;
            nextMessageType = type;
            nextChoice = choice;
            boxDir = boxDirection;
            DisableChoices();
            isLoading = false;
            canAnimate = false;
            ignoreBoxFlip = boxDir == MessagePosition.Ignore;
            this.messageEvent = messageEvent;
            nextBackground = background != null ? background : defaultBackground;

            if (beginConversation)
            {
                beginConversation = false;
                onBegin.Invoke();

                if (noTransitionIn)
                    LoadMessage();
                else
                    TransitionIn();
            }
            else if (name != currentActor)
            {
                if (noTransitionOut)
                    LoadMessage();
                else
                    TransitionOut();
            }
            else
            {
                TransitionInComplete();
            }

            currentActor = name;
        }

        public void StartConversation()
        {
            gameObject.SetActive(true);
            beginConversation = true;
            endConversation = false;
            currentActor = "";
        }

        public void EndConversation(bool gameReset)
        {
            if (gameReset)
            {
                endConversation = true;
                gameObject.SetActive(false);
                return;
            }

            endConversation = true;
            onEnd.Invoke();
            if (noTransitionOut)
                gameObject.SetActive(false);
            else
                TransitionOut();
        }

        public void TransitionOutComplete()
        {
            if (endConversation)
                gameObject.SetActive(false);
            else
                TransitionIn();
        }

        public void TransitionInComplete()
        {
            LoadMessage();
            if (noLoadMessage)
            {
                EnableNextSignal(true, true);
            }
            else
            {
                EnableNextSignal(false);
                loadMessage.Invoke();
            }
        }

        public void MessageLoadingComplete()
        {
            EnableNextSignal(true, true);
        }

        private void TransitionIn()
        {
            LoadFrame();
            EnableMessage(false);
            EnableNextSignal(false);
            transitionIn.Invoke();
            FlipDialogueBox(boxDir);
        }

        private void TransitionOut()
        {
            EnableMessage(false);
            EnableNextSignal(false);
            transitionOut.Invoke();
            FlipDialogueBox(oldBoxDir);
        }

        private void EnableNextSignal(bool value, bool loadComplete = false)
        {
            if (nextSignal != null)
            {
                nextSignal.gameObject.SetActive(value);
                if (nextMessageType == MessageType.Choice)
                    nextSignal.gameObject.SetActive(false);
            }

            if (value)
                isLoading = false;
            if (!value)
                isLoading = true;
            if (loadComplete && messageEvent != null)
                messageEvent.Invoke();
        }

        private void EnableMessage(bool value)
        {
            if (message != null)
                message.enabled = value;
            if (messenger != null)
                messenger.enabled = value;
            if (message != null && nextMessageType == MessageType.Choice)
                message.enabled = false;
        }

        private void LoadFrame()
        {
            if (icon != null)
                icon.sprite = nextIcon;
            if (background != null)
                background.sprite = nextBackground;
        }

        private void LoadMessage()
        {
            canAnimate = true;
            EnableMessage(true);
            EnableNextSignal(true);
            if (icon != null)
                icon.sprite = nextIcon;
            if (background != null)
                background.sprite = nextBackground;
            if (messenger != null)
                messenger.SetText(nextMessengerName);
            if (message != null)
                message.SetText(nextMessageType == MessageType.Message ? nextMessage : "");
            EnableChoices(nextChoice);
        }

        private void DisableChoices()
        {
            for (var i = 0; i < choices.Count; i++)
                if (choices[i] != null)
                {
                    choices[i].onClick.RemoveAllListeners();
                    choices[i].gameObject.SetActive(false);
                }
        }

        private void EnableChoices(List<Choice> newChoice)
        {
            if (nextMessageType == MessageType.Message)
                return;

            for (var i = 0; i < choices.Count; i++)
                if (i < newChoice.Count && choices[i] != null)
                {
                    if (i == 0 && EventSystem.current != null)
                        EventSystem.current.SetSelectedGameObject(choices[i].gameObject);
                    choices[i].onClick.AddListener(newChoice[i].ChoiceSelected);
                    choices[i].gameObject.SetActive(true);
                    var text = choices[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                        text.SetText(newChoice[i].choice);
                }
        }

        private void FlipDialogueBox(MessagePosition type)
        {
            if (type == MessagePosition.Ignore || ignoreBoxFlip)
                return;
            oldBoxDir = type;
            var sign = type == MessagePosition.Left ? 1f : -1f;
            if (background != null)
                SetScaleX(background.transform, sign);
            if (message != null)
                SetScaleX(message.transform, sign);
            if (messenger != null)
                SetScaleX(messenger.transform, sign);
            for (var i = 0; i < choices.Count; i++)
                if (choices[i] != null)
                    SetScaleX(choices[i].transform, sign);
        }

        private void SetScaleX(Transform transform, float sign)
        {
            var v = transform.localScale;
            transform.localScale = new Vector3(Mathf.Abs(v.x) * sign, v.y, v.z);
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private bool active;
        [SerializeField] private bool addOption;
        [SerializeField] private bool choicesFoldOut;
        [SerializeField] private bool eventsFoldOut;
        [SerializeField] private bool referenceFoldOut;
        [SerializeField] private bool beginFoldOut;
        [SerializeField] private bool endFoldOut;
        [SerializeField] private bool inFoldOut;
        [SerializeField] private bool outFoldOut;
        [SerializeField] private bool loadFoldOut;
        [SerializeField] private int signalIndex = -1;
#pragma warning restore 0414
#endif

        #endregion
    }
}