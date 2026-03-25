# ISSUE:

---

TODO:

take note: review the savemanager.cs and the messagedata.cs

        /// <summary>
        /// Initializes a blank conversation state at chapter 0 with empty history lists.
        /// </summary>
        public ConversationState()
        {
            version             = CURRENT_VERSION;
            conversationId      = "";
            characterName       = "";
            currentChapterIndex = 0;
            currentNodeName     = "";
            currentMessageIndex = 0;
            isInPauseState      = false;
            resumeTarget        = ResumeTarget.None;
            readMessageIds      = new List<string>();
            messageHistory      = new List<MessageData>();
            unlockedCGs         = new List<string>();
        }

        /// <summary>
        /// Initializes a blank conversation state with the given conversation ID.
        /// </summary>
        public ConversationState(string convId) : this()
        {
            conversationId = convId;
        }

