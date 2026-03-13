# Project Structure

---

## Scripts

```
Assets/Scripts/
├── BubbleSpinner/                        # Standalone dialogue engine
│   ├── Core/
│   │   ├── IBubbleSpinnerCallbacks.cs
│   │   ├── BubbleSpinnerParser.cs
│   │   ├── DialogueExecutor.cs
│   │   └── ConversationManager.cs
│   └── Data/
│       ├── MessageData.cs
│       ├── ConversationAsset.cs
│       └── CharacterDatabase.cs
│
└── ChatSim/
    ├── Core/
    │   ├── AddressablesImageLoader.cs
    │   ├── BubbleSpinnerBridge.cs
    │   ├── GameBootstrap.cs
    │   ├── GameEvents.cs
    │   ├── SaveManager.cs
    │   ├── SceneFlowManager.cs
    │   └── SceneNames.cs
    │
    ├── Data/
    │   └── SaveData.cs
    │
    └── UI/
        ├── ChatApp/
        │   ├── Components/
        │   │   ├── TextMessageBubble.cs
        │   │   ├── ImageMessageBubble.cs
        │   │   └── ChoiceButton.cs
        │   ├── Controllers/
        │   │   ├── ChatAppController.cs       # Main controller
        │   │   ├── ChatAutoScroller.cs
        │   │   ├── ChatChoiceSpawner.cs
        │   │   ├── ChatMessageSpawner.cs
        │   │   └── ChatTimingController.cs
        │   ├── Panels/
        │   │   ├── ContactListPanel.cs
        │   │   └── ContactListItem.cs
        │   ├── ChatAppNavButtons.cs
        │   └── FullscreenCGViewer.cs
        │
        ├── Common/
        │   └── Components/
        │       ├── AutoResizeText.cs
        │       ├── PooledObject.cs
        │       └── PoolingManager.cs
        │
        ├── HomeScreen/
        │   ├── Contacts/
        │   │   ├── ContactsAppDetails.cs      # Future feature
        │   │   ├── ContactsAppItems.cs
        │   │   ├── ContactsAppPanels.cs
        │   │   └── ResetConfirmationDialog.cs
        │   ├── Gallery/
        │   │   ├── GalleryController.cs
        │   │   ├── GalleryFullscreenViewer.cs
        │   │   └── GalleryThumbnailItems.cs
        │   ├── HomeScreenController.cs
        │   └── HomeScreenNavButtons.cs
        │
        └── Screens/
            ├── DisclaimerScreen.cs
            └── LockScreen.cs
```

---

## Docs

```
Docs/
├── QuickStart.md
├── Addressables_Setup.md
├── Project_Structure.md
└── Scenes_Setup/
    ├── Scene_Overview.md
    ├── 00_Disclaimer_Setup.md
    ├── 01_Bootstrap_Setup.md
    ├── 02_Lockscreen_Setup.md
    ├── 03_PhoneScreen_Setup.md
    └── 04_ChatApp_Setup.md
```

---

## BubbleSpinner Docs

```
Assets/Scripts/BubbleSpinner/Docs/
├── BubbleSpinner.md    # Full script reference
└── FORMAT.md           # .bub syntax guide
```