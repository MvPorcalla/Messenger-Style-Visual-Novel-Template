
```
Assets/Scripts/
├── BubbleSpinner/                           # PURE STANDALONE MODULE
│   ├── Core/
│   │   ├── IBubbleSpinnerCallbacks.cs       # 
│   │   ├── BubbleSpinnerParser.cs           # 
│   │   ├── DialogueExecutor.cs              # 
│   │   └── ConversationManager.cs           #
│   │
│   └── Data/
│       ├── MessageData.cs                   #
│       ├── ConversationAsset.cs             #
│       └── CharacterDatabase.cs             #
│
├── ChatSim/ 
    ├── Core/
    │   ├── AddressablesImageLoader.cs          # NEW - Async image loader
    │   ├── BubbleSpinnerBridge.cs        
    │   ├── GameBootstrap.cs              
    │   ├── GameEvents.cs                 
    │   ├── SaveManager.cs                 
    │   ├── SceneFlowManager.cs
    │   └── Scenename.cs
    │
    ├── Data/
    │   ├── Config/
    │   │   └── 
    │   └── SaveData.cs
    │   
    └── UI/
        ├── ChatApp/                        # Chat application
        │   ├── Components/
        │   │   ├── MessageBubble.cs        # Individual bubble
        │   │   ├── ImageMessageBubble.cs   # NEW - CG bubble with click
        │   │   └── ChoiceButton.cs         # Individual choice button
        │   │
        │   ├── Controllers/
        │   │   ├── ChatAppController.cs    # ⭐ MAIN CONTROLLER (absorbs UIManager)
        │   │   ├── ChatAutoScroller.cs     # Auto-scroll logic
        │   │   ├── ChatChoiceSpawner.cs    # Spawns choice buttons
        │   │   ├── ChatMessageSpawner.cs   # Spawns message bubbles
        │   │   └── ChatTimingController.cs # Timing & animations
        │   │
        │   ├── Panels/
        │   │   ├── ContactListPanel.cs     # Contact list
        │   │   └── ContactListItem.cs      # Individual contact button
        │   │
        │   ├── ChatAppNavButtons.cs        # Contact list
        │   └── FullscreenCGViewer.cs       # 
        │
        ├── Common/                         # Shared UI utilities
        │   └── Components/
        │       ├── AutoResizeText.cs       # Used by MessageBubble
        │       ├── PooledObject.cs         # Pooling system
        │       └── PoolingManager.cs       # Pooling system    
        │
        ├── HomeScreen/
        │   ├── Contacts/
        │   │   ├── ContactsAppDetails.cs       # (future Feature)
        │   │   ├── ContactsAppItems.cs
        │   │   ├── ContactsAppPanels.cs
        │   │   └── ResetConfirmationDialog.cs
        │   │
        │   ├── Gallery/
        │   │   ├── GalleryController.cs
        │   │   ├── GalleryFullscreenVeiwer.cs
        │   │   └── GalleryThumbnailItems.cs
        │   │
        │   ├── Setting/
        │   │   └── SettingPanel.cs
        │   │
        │   ├── HomeScreenController.cs          # Home screen & app launcher
        │   └── HomeScreenNavButtons.cs          # 
        │
        ├── Overlay/                        # Phone operating system
        │   ├── Dialog/
        │       └── ResetConfirmationDialog.cs
        │
        └── Screens/                    # Scene Specific Controller
            ├── DisclaimerScreen.cs     # First-time disclaimer
            └── LockScreen.cs           # Lock screen
```