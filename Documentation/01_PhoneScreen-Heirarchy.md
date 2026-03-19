03_PhoneScreen
└── Canvas (Screen Space - Overlay)
    ├── PhoneRoot
    │   ├── WallpaperContainer
    │   │   └── WallpaperImage
    │   │
    │   ├── Screens (ONLY ONE ACTIVE AT A TIME)
    │   │   ├── HomeScreenPanel
    │   │   │   ├── AppGrid
    │   │   │   │   ├── AppButton_Chat
    │   │   │   │   ├── AppButton_Contacts
    │   │   │   │   ├── AppButton_Gallery
    │   │   │   │   └── AppButton_Settings
    │   │   │   └── Dock
    │   │   │       ├── AppButton_Phone
    │   │   │       └── AppButton_Messages
    │   │   │
    │   │   ├── GalleryPanel                            ← ATTACH [GalleryController.cs] (Inactive in scene)
    │   │   │   ├── Header
    │   │   │   ├── ProgressText
    │   │   │   ├── ScrollView                          (active in scene)
    │   │   │   │   └── Viewport
    │   │   │   │       └── Content
    │   │   │   │           └── CGContainer
    │   │   │   │               ├── CharacterName
    │   │   │   │               └── CGGrid
    │   │   │   │                   └── CGThumbnail     ← ATTACH [GalleryThumbnailItem.cs] HERE (in prefab)
    │   │   │   │                       ├── Background
    │   │   │   │                       ├── ThumbnailImage
    │   │   │   │                       └── LockedOverlay
    │   │   │   └── GalleryFullscreenViewer             ← ATTACH [GalleryFullscreenViewer.cs] (Do not Put this panel Inactive) (active in scene)
    │   │   │       ├── BackgroundOverlay
    │   │   │       ├── ImageContainer
    │   │   │       │   └── CGImage
    │   │   │       └── TopBar
    │   │   │           ├── CloseButton
    │   │   │           └── CGNameText
    │   │   │
    │   │   ├── ContactsPanel                           ← ATTACH [ContactsAppPanel.cs] (Inactive in scene)
    │   │   │   ├── Header                              ← Empty GameObject (layout)
    │   │   │   │   ├── BackButton
    │   │   │   │   └── TitleText                       ← TextMeshProUGUI, text = "Contacts"
    │   │   │   │
    │   │   │   ├── ScrollView                          (active in scene)
    │   │   │   │   └── Viewport
    │   │   │   │       └── Content
    │   │   │   │           └── ContactsAppItem         ← ATTACH [ContactsAppItem.cs]
    │   │   │   │               ├── ProfileImage
    │   │   │   │               ├── InfoGroup   
    │   │   │   │               │   ├── NameText
    │   │   │   │               │   └── BioText 
    │   │   │   │               └── ResetButton 
    │   │   │   │                   └── Text 
    │   │   │   │
    │   │   │   ├── ContactsAppDetailPanel          ← ContactsAppDetailPanel.cs (active in scene) [FUTURE]
    │   │   │   │   ├── Overlay
    │   │   │   │   └── DetailCard
    │   │   │   │       ├── CloseButton
    │   │   │   │       ├── ProfileImage
    │   │   │   │       ├── NameText
    │   │   │   │       ├── InfoGroup
    │   │   │   │       │   ├── AgeText
    │   │   │   │       │   ├── BirthdateText
    │   │   │   │       │   ├── BioText
    │   │   │   │       │   └── DescriptionText
    │   │   │   │       └── ResetButton
    │   │   │   │           └── Text ("Reset Story")   
    │   │   │   │
    │   │   │   └── ResetConfirmationDialog             ← ATTACH [ResetConfirmationDialog.cs] (Do not Put this panel Inactive) (active in scene)
    │   │   │       └── ConfirmationDialog
    │   │   │           └── ContentPanel   
    │   │   │               ├── TitleText
    │   │   │               ├── MessageText 
    │   │   │               ├── CancelButton
    │   │   │               │   └── Text
    │   │   │               └── ResetButton 
    │   │   │                   └── Text    
    │   │   │
    │   │   └── SettingsPanel                       ← ATTACH THIS SCRIPT — INACTIVE by default
    │   │       └── ScrollView
    │   │           └── Viewport
    │   │               └── Content
    │   │                   ├── Section_Gameplay
    │   │                   │   ├── SectionHeader   (TMP — "Gameplay")
    │   │                   │   ├── MessageSpeed
    │   │                   │   │   ├── Label       (TMP — "Message Speed")
    │   │                   │   │   └── SpeedButton (Button)
    │   │                   │   │       ├── Icon    (Image)
    │   │                   │   │       └── StateText (TMP — "Normal" / "Fast")
    │   │                   │   └── TextSize
    │   │                   │       ├── Label       (TMP — "Text Size")
    │   │                   │       ├── SmallButton (Button)
    │   │                   │       ├── MediumButton(Button)
    │   │                   │       └── LargeButton (Button)
    │   │                   ├── Section_Data
    │   │                   │   ├── SectionHeader   (TMP — "Data")
    │   │                   │   └── ResetAllButton  (Button)
    │   │                   └── Section_About
    │   │                       ├── SectionHeader   (TMP — "About")
    │   │                       └── VersionText     (TMP)
    │   │
    │   ├── NavigationBar   [QuitConfirmationManager] ← Attach script here
    │   │   ├── QuitButton
    │   │   ├── HomeButton
    │   │   └── BackButton
    │   │
    │   ├── Overlays (CAN STACK)                ← new GameObject — ACTIVE
    │   │   ├── ResetConfirmationDialog         ← ResetConfirmationDialog.cs — ACTIVE
    │   │   │   └── ConfirmationDialog
    │   │   │       └── ContentPanel              ← INACTIVE
    │   │   │           └── Content
    │   │   │               ├── TitleText           (TMP)
    │   │   │               ├── MessageText         (TMP)
    │   │   │               ├── YesButton           (Button)
    │   │   │               │   └── Text            (TMP — "Yes")
    │   │   │               └── NoButton            (Button)
    │   │   │                   └── Text            (TMP — "No")
    │   │   │
    │   │   ├── NotificationPopup (FUTURE IMPLEMENTATION)
    │   │   └── Tooltip (FUTURE IMPLEMENTATION)
    │   │
    │   ├── Transitions (FUTURE IMPLEMENTATION)
    │   │   ├── FadeOverlay
    │   │   └── ScreenBlocker
    │   │
    │   └── QuitConfirmationPanel ← CREATE THIS
    │       ├── Overlay (Image - Black with 50% alpha)
    │       └── ConfirmPanel (Image - White background)
    │           ├── TitleText ("Quit Game?")
    │           ├── YesButton
    │           │   └── ButtonText ("Yes")
    │           └── NoButton
    │               └── ButtonText ("No")
    │
    └── EventSystem
