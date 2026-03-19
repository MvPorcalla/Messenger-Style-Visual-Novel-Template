# 03_PhoneScreen — Scene Setup Guide

---

## Overview

This guide covers the complete setup for the `03_PhoneScreen` scene: hierarchy, prefabs, script attachment, Inspector wiring, and final checks.

**Scripts involved:**

| Script | Namespace |
|---|---|
| `HomeScreenController.cs` | `ChatSim.UI.HomeScreen` |
| `HomeScreenNavButtons.cs` | `ChatSim.UI.HomeScreen` |
| `GalleryController.cs` | `ChatSim.UI.HomeScreen.Gallery` |
| `GalleryFullscreenViewer.cs` | `ChatSim.UI.HomeScreen.Gallery` |
| `GalleryThumbnailItem.cs` | `ChatSim.UI.HomeScreen.Gallery` |
| `ContactsAppPanel.cs` | `ChatSim.UI.HomeScreen.Contacts` |
| `ContactsAppItem.cs` | `ChatSim.UI.HomeScreen.Contacts` |
| `ContactsAppDetails.cs` | `ChatSim.UI.HomeScreen.Contacts` — **[FUTURE]** |
| `SettingsPanel.cs` | `ChatSim.UI.HomeScreen.Settings` |
| `ResetConfirmationDialog.cs` | `ChatSim.UI.Overlay.Dialogs` |

---

## Part 1 — Scene Hierarchy

```
03_PhoneScreen
└── Canvas (Screen Space - Overlay)
    ├── PhoneRoot
    │   ├── WallpaperContainer
    │   │   └── WallpaperImage
    │   │
    │   ├── Screens                                     ← only one panel active at a time
    │   │   ├── HomeScreenPanel                         ← HomeScreenController — ACTIVE
    │   │   │   ├── AppGrid
    │   │   │   │   ├── AppButton_Chat
    │   │   │   │   ├── AppButton_Contacts
    │   │   │   │   ├── AppButton_Gallery
    │   │   │   │   └── AppButton_Settings
    │   │   │   └── Dock
    │   │   │       ├── AppButton_Phone
    │   │   │       └── AppButton_Messages
    │   │   │
    │   │   ├── GalleryPanel                            ← GalleryController — INACTIVE
    │   │   │   ├── Header
    │   │   │   ├── ProgressText
    │   │   │   ├── ScrollView
    │   │   │   │   └── Viewport
    │   │   │   │       └── Content                     ← populated at runtime
    │   │   │   └── GalleryFullscreenViewer             ← GalleryFullscreenViewer + CanvasGroup — ACTIVE
    │   │   │       ├── BackgroundOverlay
    │   │   │       ├── ImageContainer
    │   │   │       │   └── CGImage
    │   │   │       └── TopBar
    │   │   │           ├── CloseButton
    │   │   │           └── CGNameText
    │   │   │
    │   │   ├── ContactsPanel                           ← ContactsAppPanel — INACTIVE
    │   │   │   ├── Header
    │   │   │   │   ├── BackButton
    │   │   │   │   └── TitleText                       (TMP — "Contacts")
    │   │   │   ├── ScrollView
    │   │   │   │   └── Viewport
    │   │   │   │       └── Content                     ← populated at runtime
    │   │   │   └── ContactsAppDetailPanel              ← ContactsAppDetails — ACTIVE [FUTURE]
    │   │   │       ├── Overlay
    │   │   │       └── DetailCard
    │   │   │           ├── CloseButton
    │   │   │           ├── ProfileImage
    │   │   │           ├── NameText
    │   │   │           ├── InfoGroup
    │   │   │           │   ├── AgeText
    │   │   │           │   ├── BirthdateText
    │   │   │           │   ├── BioText
    │   │   │           │   └── DescriptionText
    │   │   │           └── ResetButton
    │   │   │               └── Text                    (TMP — "Reset Story")
    │   │   │
    │   │   └── SettingsPanel                           ← SettingsPanel — INACTIVE
    │   │       └── ScrollView
    │   │           └── Viewport
    │   │               └── Content
    │   │                   ├── Section_Gameplay
    │   │                   │   ├── SectionHeader       (TMP — "Gameplay")
    │   │                   │   ├── MessageSpeed
    │   │                   │   │   ├── Label           (TMP — "Message Speed")
    │   │                   │   │   └── SpeedButton     (Button)
    │   │                   │   │       ├── Icon        (Image)
    │   │                   │   │       └── StateText   (TMP — "Normal" / "Fast")
    │   │                   │   └── TextSize
    │   │                   │       ├── Label           (TMP — "Text Size")
    │   │                   │       ├── SmallButton     (Button)
    │   │                   │       ├── MediumButton    (Button)
    │   │                   │       └── LargeButton     (Button)
    │   │                   ├── Section_Data
    │   │                   │   ├── SectionHeader       (TMP — "Data")
    │   │                   │   └── ResetAllButton      (Button)
    │   │                   └── Section_About
    │   │                       ├── SectionHeader       (TMP — "About")
    │   │                       └── VersionText         (TMP)
    │   │
    │   ├── NavigationBar                               ← HomeScreenNavButtons
    │   │   ├── QuitButton
    │   │   ├── HomeButton
    │   │   └── BackButton
    │   │
    │   ├── Overlays                                    ← new GameObject — ACTIVE (can stack)
    │   │   ├── ResetConfirmationDialog                 ← ResetConfirmationDialog — ACTIVE
    │   │   │   └── ResetDialog                         ← INACTIVE (script manages visibility)
    │   │   │       └── ContentPanel
    │   │   │           └── Content
    │   │   │               ├── TitleText               (TMP)
    │   │   │               ├── MessageText             (TMP)
    │   │   │               ├── YesButton               (Button)
    │   │   │               │   └── Text                (TMP — "Yes")
    │   │   │               └── NoButton                (Button)
    │   │   │                   └── Text                (TMP — "No")
    │   │   │
    │   │   ├── QuitConfirmationPanel                   ← ACTIVE
    │   │   │   └── QuitDialog                          ← INACTIVE
    │   │   │       └── ContentPanel
    │   │   │           ├── TitleText                   (TMP)
    │   │   │           ├── YesButton                   (Button)
    │   │   │           │   └── Text                    (TMP — "Yes")
    │   │   │           └── NoButton                    (Button)
    │   │   │               └── Text                    (TMP — "No")
    │   │   │
    │   │   ├── NotificationPopup                       ← [FUTURE]
    │   │   └── Tooltip                                 ← [FUTURE]
    │   │
    │   └── Transitions                                 ← [FUTURE]
    │       ├── FadeOverlay
    │       └── ScreenBlocker
    │
    └── EventSystem
```

> **Active / Inactive rules:**
> - `HomeScreenPanel` — **active** (first screen shown)
> - `GalleryPanel` — **inactive** (opened by app button)
> - `ContactsPanel` — **inactive** (opened by app button)
> - `SettingsPanel` — **inactive** (opened by app button)
> - `GalleryFullscreenViewer` — **active** (script manages own visibility via `SetActive`)
> - `ContactsAppDetailPanel` — **active** (script manages own visibility) **[FUTURE]**
> - `Overlays` — **active** (container is always active; children manage their own visibility)
> - `ResetConfirmationDialog` — **active** (inner `ResetDialog` starts inactive — set by script in `Awake`)
> - `QuitConfirmationPanel` — **active** (inner `QuitDialog` starts inactive)

---

## Part 2 — Prefab Setup

Create prefabs in `Assets/Prefabs/PhoneScreen/`.

---

### 2.1 CGThumbnail Prefab

```
CGThumbnail (root)       ← Button + GalleryThumbnailItem
├── Background           (Image)
├── ThumbnailImage       (Image)
└── LockedOverlay        (GameObject)
```

**Setup:**
1. Add **Button** component to root.
2. Add **`GalleryThumbnailItem.cs`** to root.
3. Wire Inspector fields on `GalleryThumbnailItem`:
   ```
   thumbnailImage  → ThumbnailImage (Image)
   lockedOverlay   → LockedOverlay (GameObject)
   ```
4. Save as `CGThumbnail.prefab`.

> Configure inside prefab mode — not on a scene instance.

---

### 2.2 CGContainer Prefab (Character Section)

```
CGContainer (root)
├── CharacterName    (TMP)         ← first child — index 0, read by GalleryController
└── CGGrid           (GameObject)  ← second child — index 1, thumbnails spawn here
```

**Setup:**
1. Add a **Vertical Layout Group** or **Grid Layout Group** to `CGGrid` as needed.
2. No scripts required on this prefab — `GalleryController` reads children by index.
3. Save as `CGContainer.prefab`.

---

### 2.3 ContactsAppItem Prefab

```
ContactsAppItem (root)   ← Button (itemButton) + ContactsAppItem
├── ProfileImage         (Image)
├── InfoGroup            (GameObject)
│   ├── NameText         (TMP)
│   └── BioText          (TMP)
└── ResetButton          (Button)
    └── Text             (TMP — "Reset Story")
```

**Setup:**
1. Add **Button** to root (this is `itemButton`).
2. Add **`ContactsAppItem.cs`** to root.
3. Wire Inspector fields:
   ```
   itemButton    → Button on root
   profileImage  → ProfileImage (Image)
   nameText      → InfoGroup/NameText (TMP)
   resetButton   → ResetButton (Button)
   ```
4. Save as `ContactsAppItem.prefab`.

> Do **not** wire `ResetButton.onClick` in the Inspector — `ContactsAppItem.cs` wires it at runtime via `SetupResetButton()`.

---

## Part 3 — Script Attachment

| GameObject | Scripts to Attach |
|---|---|
| `HomeScreenPanel` | `HomeScreenController` |
| `NavigationBar` | `HomeScreenNavButtons` |
| `GalleryPanel` | `GalleryController` |
| `GalleryFullscreenViewer` | `GalleryFullscreenViewer`, `Canvas Group` |
| `ContactsPanel` | `ContactsAppPanel` |
| `SettingsPanel` | `SettingsPanel` |
| `ResetConfirmationDialog` | `ResetConfirmationDialog` |

> `ResetConfirmationDialog` is now a **shared overlay** under `Overlays` — it is no longer a child of `ContactsPanel`. It is referenced by both `ContactsAppPanel` and `SettingsPanel` via Inspector assignment.

> `Canvas Group` on `GalleryFullscreenViewer` is required for fade animations. Add it via **Add Component → Canvas Group**.

---

## Part 4 — Inspector Wiring

### HomeScreenController

```
[Home Screen Panel]
homeScreenPanel  → HomeScreenPanel (GameObject)

[App Buttons]
apps             → List of AppButton entries — one per app icon

  Each AppButton entry:
  enabled       → ☑ true (or ☐ to hide the button)
  appName       → "Chat" / "Contacts" / "Gallery" / "Settings" / etc.
  button        → AppButton_Chat / AppButton_Contacts / etc. (Button)
  targetScene   → scene name string — fill if app opens a scene (e.g. "04_ChatApp")
  targetPanel   → panel GameObject — fill if app opens a panel in this scene
```

> For each app: fill either `targetScene` OR `targetPanel`, not both. `Chat` uses `targetScene`. `Contacts`, `Gallery`, and `Settings` use `targetPanel`.

---

### HomeScreenNavButtons

```
[Navigation Buttons]
homeButton            → HomeButton (Button)
backButton            → BackButton (Button)
quitButton            → QuitButton (Button)

[Quit Confirmation]
quitConfirmationPanel → Overlays/QuitConfirmationPanel (GameObject)
yesQuitButton         → QuitDialog/ContentPanel/YesButton (Button)
noQuitButton          → QuitDialog/ContentPanel/NoButton (Button)

[Home Screen]
homeScreenController  → HomeScreenPanel (drag — Unity finds HomeScreenController on it)
```

---

### GalleryController

```
[Gallery UI]
contentContainer        → Content (GalleryPanel > ScrollView > Viewport > Content)
progressText            → ProgressText (TMP)

[Prefabs]
characterSectionPrefab  → CGContainer.prefab (from Project)
thumbnailPrefab         → CGThumbnail.prefab (from Project)

[Character Data]
characterDatabase       → CharacterDatabase.asset (from Project)

[Display Options]
showLockedCGs           → ☑ true
showEmptySections       → ☐ false
lockedCGSprite          → optional placeholder sprite (from Project) or leave None

[Fullscreen Viewer]
fullscreenViewer        → GalleryFullscreenViewer (from Hierarchy)
```

> `contentContainer` must be the `Content` GameObject inside `Viewport` — not `Viewport` itself.

---

### GalleryFullscreenViewer

```
[UI Elements]
viewerPanel       → GalleryFullscreenViewer (this GameObject — itself)
cgImage           → CGImage (ImageContainer > CGImage)
closeButton       → CloseButton (TopBar > CloseButton)
cgNameText        → CGNameText (TopBar > CGNameText)
canvasGroup       → Canvas Group on this GameObject

[Background]
backgroundOverlay → BackgroundOverlay (Image)

[Zoom Settings]
minZoom           → 1
maxZoom           → 3
zoomSpeed         → 0.001
doubleTapZoom     → 2
doubleTapTime     → 0.3

[Pan Settings]
enablePanLimits   → ☑ true

[Animation]
fadeDuration      → 0.3
```

> `viewerPanel` points to the same GameObject this script is on. Drag `GalleryFullscreenViewer` from the Hierarchy into the field.

---

### ContactsAppPanel

```
[Database]
characterDatabase        → CharacterDatabase.asset (from Project)

[UI References]
contactContainer         → Content (ContactsPanel > ScrollView > Viewport > Content)
contactsAppItemPrefab    → ContactsAppItem.prefab (from Project)

[Dialog]
useConfirmationDialog    → ☑ true
resetConfirmationDialog  → Overlays/ResetConfirmationDialog (from Hierarchy)
```

> `resetConfirmationDialog` is now in `Overlays` — drag it from there, not from inside `ContactsPanel`.

---

### SettingsPanel

```
[Gameplay — Message Speed]
messageSpeedButton  → Section_Gameplay/MessageSpeed/SpeedButton (Button)
messageSpeedLabel   → Section_Gameplay/MessageSpeed/SpeedButton/StateText (TMP)
messageSpeedIcon    → Section_Gameplay/MessageSpeed/SpeedButton/Icon (Image)
normalModeSprite    → sprite asset for normal mode (from Project)
fastModeSprite      → sprite asset for fast mode (from Project)

[Gameplay — Text Size]
smallTextButton     → Section_Gameplay/TextSize/SmallButton (Button)
mediumTextButton    → Section_Gameplay/TextSize/MediumButton (Button)
largeTextButton     → Section_Gameplay/TextSize/LargeButton (Button)

[Data]
resetAllButton      → Section_Data/ResetAllButton (Button)
resetAllDialog      → Overlays/ResetConfirmationDialog (from Hierarchy)

[About]
versionText         → Section_About/VersionText (TMP)
```

> `resetAllDialog` points to the same shared `ResetConfirmationDialog` used by `ContactsAppPanel`. Drag it from `Overlays` in the Hierarchy.

> `normalModeSprite` and `fastModeSprite` are optional — if left empty, only the label text will update on toggle.

---

### ResetConfirmationDialog

```
[UI Elements]
confirmationDialog  → ResetDialog (child of ResetConfirmationDialog — the INACTIVE inner panel)
titleText           → ResetDialog/ContentPanel/Content/TitleText (TMP)
messageText         → ResetDialog/ContentPanel/Content/MessageText (TMP)
yesButton           → ResetDialog/ContentPanel/Content/YesButton (Button)
noButton            → ResetDialog/ContentPanel/Content/NoButton (Button)
```

> The script sets `ResetDialog` inactive in `Awake`. Leave `ResetConfirmationDialog` itself **active** in the scene — do not pre-deactivate it.

> This dialog is shared — both `ContactsAppPanel` and `SettingsPanel` reference it. Only one caller can show it at a time.

---

## Part 5 — Content Layout Setup

On the `Content` GameObject inside both `GalleryPanel` and `ContactsPanel` scroll views, add these two components:

**Vertical Layout Group**
```
Control Child Size (Width)   → ☑ true
Control Child Size (Height)  → ☐ false
Child Force Expand (Width)   → ☑ true
Spacing                      → set as needed
```

**Content Size Fitter**
```
Vertical Fit  → Preferred Size
```

---

## Part 6 — Final Checklist

```
SCENE OBJECTS — Active / Inactive
☐ HomeScreenPanel              — active (default start screen)
☐ GalleryPanel                 — inactive
☐ ContactsPanel                — inactive
☐ SettingsPanel                — inactive
☐ GalleryFullscreenViewer      — active (script self-manages visibility)
☐ ContactsAppDetailPanel       — active (script self-manages visibility) [FUTURE]
☐ Overlays                     — active (container always active)
☐ ResetConfirmationDialog      — active (inner ResetDialog starts inactive — set by script)
☐ QuitConfirmationPanel        — active (inner QuitDialog starts inactive)

HomeScreenController
☐ homeScreenPanel assigned
☐ apps list populated (one entry per app button, including Settings)
☐ Each AppButton: button assigned + targetScene or targetPanel filled

HomeScreenNavButtons
☐ homeButton assigned
☐ backButton assigned
☐ quitButton assigned
☐ quitConfirmationPanel assigned (from Overlays)
☐ yesQuitButton assigned
☐ noQuitButton assigned
☐ homeScreenController assigned

GalleryController
☐ GalleryController.cs attached to GalleryPanel
☐ contentContainer assigned
☐ progressText assigned
☐ characterSectionPrefab assigned
☐ thumbnailPrefab assigned
☐ characterDatabase assigned
☐ fullscreenViewer assigned

GalleryFullscreenViewer
☐ GalleryFullscreenViewer.cs attached
☐ Canvas Group component added
☐ viewerPanel assigned (itself)
☐ cgImage assigned
☐ closeButton assigned
☐ cgNameText assigned
☐ canvasGroup assigned
☐ backgroundOverlay assigned

CGThumbnail.prefab
☐ Button component on root
☐ GalleryThumbnailItem.cs attached
☐ thumbnailImage assigned
☐ lockedOverlay assigned
☐ Prefab saved

CGContainer.prefab
☐ First child is CharacterName (TMP)
☐ Second child is CGGrid (thumbnails parent)
☐ No scripts required
☐ Prefab saved

ContactsAppPanel
☐ ContactsAppPanel.cs attached to ContactsPanel
☐ characterDatabase assigned
☐ contactContainer assigned
☐ contactsAppItemPrefab assigned
☐ resetConfirmationDialog assigned (from Overlays — not from ContactsPanel)

ContactsAppItem.prefab
☐ Button on root (itemButton)
☐ ContactsAppItem.cs attached
☐ itemButton assigned
☐ profileImage assigned
☐ nameText assigned (InfoGroup/NameText)
☐ resetButton assigned
☐ ResetButton.onClick — NOT wired in Inspector
☐ Prefab saved

SettingsPanel
☐ SettingsPanel.cs attached to SettingsPanel
☐ messageSpeedButton assigned
☐ messageSpeedLabel assigned
☐ messageSpeedIcon assigned
☐ normalModeSprite assigned (or leave None)
☐ fastModeSprite assigned (or leave None)
☐ smallTextButton assigned
☐ mediumTextButton assigned
☐ largeTextButton assigned
☐ resetAllButton assigned
☐ resetAllDialog assigned (from Overlays — shared with ContactsAppPanel)
☐ versionText assigned

ResetConfirmationDialog
☐ ResetConfirmationDialog.cs attached
☐ confirmationDialog assigned (inner ResetDialog GameObject)
☐ titleText assigned
☐ messageText assigned
☐ yesButton assigned
☐ noButton assigned
☐ ResetConfirmationDialog GameObject left ACTIVE in scene
```

---

## Part 7 — Common Mistakes

**Gallery shows no thumbnails**
`ConversationAsset` files must have their `cgAddressableKeys` list populated (e.g. `Sofia/CG1`, `Sofia/CG2`). Open each asset in the Inspector and verify. The gallery skips characters with empty CG lists when `showEmptySections` is false.

**Gallery fade animation doesn't work**
`Canvas Group` component is missing from `GalleryFullscreenViewer`. Add it via Add Component, then assign it to the `canvasGroup` field.

**GalleryFullscreenViewer doesn't open on thumbnail click**
`fullscreenViewer` on `GalleryController` is not assigned. Drag `GalleryFullscreenViewer` from the Hierarchy into the field.

**Character sections render with wrong name or no thumbnails**
`CGContainer.prefab` child order is wrong. `GalleryController` reads child index 0 as the header text and index 1 as the grid. The header `CharacterName` TMP must be the first child, `CGGrid` must be the second.

**Contacts list shows empty**
`characterDatabase` on `ContactsAppPanel` is not assigned, or `CharacterDatabase.asset` has no entries. Open the asset in the Inspector and verify the characters list is populated.

**Reset button does nothing**
Do not wire `ResetButton.onClick` in the Inspector — `ContactsAppItem.cs` calls `resetButton.onClick.RemoveAllListeners()` and rewires it at runtime. Any Inspector listener will be cleared.

**ResetConfirmationDialog never appears**
`resetConfirmationDialog` on `ContactsAppPanel` or `resetAllDialog` on `SettingsPanel` is not assigned, or is pointing to the wrong GameObject. The field must reference `ResetConfirmationDialog` from the `Overlays` container — not a stale reference from inside `ContactsPanel`. Confirm the inner `ResetDialog` GameObject is left **inactive** and the outer `ResetConfirmationDialog` is **active**.

**ResetConfirmationDialog doesn't show up when triggered**
The script is attached to the outer `ResetConfirmationDialog` GameObject — that object must be **active** in the scene. If `ResetDialog` (the inner panel) is active in the scene instead, flip it — leave `ResetDialog` **inactive** and `ResetConfirmationDialog` **active**. The script sets `ResetDialog` inactive in `Awake` and re-activates it via `Show()`.

**Settings Reset All does nothing**
`resetAllDialog` on `SettingsPanel` is not assigned. Drag `ResetConfirmationDialog` from `Overlays` into the field. If `GameBootstrap.Save` or `GameBootstrap.Conversation` is null, check that `GameBootstrap` has fully initialized before the settings panel is opened.

**App buttons do nothing when clicked**
Each `AppButton` entry in `HomeScreenController.apps` must have either `targetScene` or `targetPanel` filled — not both empty. Check the apps list in the Inspector and confirm each entry is wired, including the Settings button.

**Back button does nothing from a panel**
`homeScreenController` on `HomeScreenNavButtons` is not assigned. Drag `HomeScreenPanel` into the field — Unity finds `HomeScreenController` on it automatically.

**Items not spawning into scroll views**
`contactContainer` and `contentContainer` must point to the `Content` child inside `Viewport` — not `Viewport` itself and not the `ScrollView`.

**QuitConfirmationPanel doesn't appear**
`quitConfirmationPanel` on `HomeScreenNavButtons` is pointing to the old location. The panel has moved — drag it from `Overlays/QuitConfirmationPanel` in the Hierarchy.