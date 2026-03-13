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
| `ResetConfirmationDialog.cs` | `ChatSim.UI.HomeScreen.Contacts` |

---

## Part 1 — Scene Hierarchy

```
03_PhoneScreen
└── Canvas
    └── PhoneRoot
        ├── WallpaperContainer
        │   └── WallpaperImage
        │
        ├── Screens                         ← only one panel active at a time
        │   ├── HomeScreenPanel             ← HomeScreenController
        │   │   ├── AppGrid
        │   │   │   ├── AppButton_Chat
        │   │   │   ├── AppButton_Contacts
        │   │   │   ├── AppButton_Gallery
        │   │   │   └── AppButton_Settings
        │   │   └── Dock
        │   │       ├── AppButton_Phone
        │   │       └── AppButton_Messages
        │   │
        │   ├── GalleryPanel                ← GalleryController — INACTIVE by default
        │   │   ├── Header
        │   │   ├── ProgressText
        │   │   ├── ScrollView
        │   │   │   └── Viewport
        │   │   │       └── Content         ← populated at runtime
        │   │   └── GalleryFullscreenViewer ← GalleryFullscreenViewer, Canvas Group — ACTIVE
        │   │       ├── BackgroundOverlay
        │   │       ├── ImageContainer
        │   │       │   └── CGImage
        │   │       └── TopBar
        │   │           ├── CloseButton
        │   │           └── CGNameText
        │   │
        │   └── ContactsPanel               ← ContactsAppPanel — INACTIVE by default
        │       ├── Header
        │       │   ├── BackButton
        │       │   └── TitleText
        │       ├── ScrollView
        │       │   └── Viewport
        │       │       └── Content         ← populated at runtime
        │       ├── ContactsAppDetailPanel  ← future implementation — ACTIVE in scene
        │       │   ├── Overlay
        │       │   └── DetailCard
        │       │       ├── CloseButton
        │       │       ├── ProfileImage
        │       │       ├── NameText
        │       │       ├── InfoGroup
        │       │       └── ResetButton
        │       └── ResetConfirmationDialog ← ResetConfirmationDialog — ACTIVE in scene
        │           └── ConfirmationDialog
        │               └── ContentPanel
        │                   ├── TitleText
        │                   ├── MessageText
        │                   ├── CancelButton
        │                   └── ResetButton
        │
        ├── NavigationBar                   ← HomeScreenNavButtons
        │   ├── QuitButton
        │   ├── HomeButton
        │   └── BackButton
        │
        └── QuitConfirmationPanel           ← INACTIVE by default
            ├── Overlay
            └── ConfirmPanel
                ├── TitleText
                ├── YesButton
                └── NoButton
```

> **Active / Inactive rules:**
> - `HomeScreenPanel` — active (first screen shown)
> - `GalleryPanel` — **inactive** (opened by app button)
> - `ContactsPanel` — **inactive** (opened by app button)
> - `GalleryFullscreenViewer` — **active** (script manages own visibility via `SetActive`)
> - `ResetConfirmationDialog` — **active** (script sets itself inactive in `Awake`)
> - `QuitConfirmationPanel` — **inactive** (shown on quit button press)

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
├── NameText             (TMP)
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
   nameText      → NameText (TMP)
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
| `ResetConfirmationDialog` | `ResetConfirmationDialog` |

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
  appName       → "Chat" / "Contacts" / "Gallery" / etc.
  button        → AppButton_Chat / AppButton_Contacts / etc. (Button)
  targetScene   → scene name string — fill if app opens a scene (e.g. "04_ChatApp")
  targetPanel   → panel GameObject — fill if app opens a panel in this scene
```

> For each app: fill either `targetScene` OR `targetPanel`, not both. `Chat` uses `targetScene`. `Contacts` and `Gallery` use `targetPanel`.

---

### HomeScreenNavButtons

```
[Navigation Buttons]
homeButton            → HomeButton (Button)
backButton            → BackButton (Button)
quitButton            → QuitButton (Button)

[Quit Confirmation]
quitConfirmationPanel → QuitConfirmationPanel (GameObject)
yesQuitButton         → YesButton (Button)
noQuitButton          → NoButton (Button)

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
zoomSpeed         → 0.1
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
resetConfirmationDialog  → ResetConfirmationDialog (from Hierarchy)
```

---

### ResetConfirmationDialog

```
[UI Elements]
titleText   → ContentPanel/TitleText (TMP)
messageText → ContentPanel/MessageText (TMP)
yesButton   → ContentPanel/ResetButton (Button)
noButton    → ContentPanel/CancelButton (Button)
```

> This script calls `gameObject.SetActive(false)` in `Awake` — leave the GameObject **active** in the scene. Do not pre-deactivate it.

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
☐ HomeScreenPanel          — active (default start screen)
☐ GalleryPanel             — inactive
☐ ContactsPanel            — inactive
☐ GalleryFullscreenViewer  — active (script self-manages visibility)
☐ ResetConfirmationDialog  — active (script sets inactive in Awake)
☐ QuitConfirmationPanel    — inactive

HomeScreenController
☐ homeScreenPanel assigned
☐ apps list populated (one entry per app button)
☐ Each AppButton: button assigned + targetScene or targetPanel filled

HomeScreenNavButtons
☐ homeButton assigned
☐ backButton assigned
☐ quitButton assigned
☐ quitConfirmationPanel assigned
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
☐ resetConfirmationDialog assigned

ResetConfirmationDialog
☐ ResetConfirmationDialog.cs attached
☐ titleText assigned
☐ messageText assigned
☐ yesButton assigned
☐ noButton assigned

ContactsAppItem.prefab
☐ Button on root (itemButton)
☐ ContactsAppItem.cs attached
☐ itemButton assigned
☐ profileImage assigned
☐ nameText assigned
☐ resetButton assigned
☐ ResetButton.onClick — NOT wired in Inspector
☐ Prefab saved
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
`resetConfirmationDialog` on `ContactsAppPanel` is not assigned, or the GameObject was left inactive in the scene. The script sets itself inactive in `Awake` — leave it active in the scene editor.

**App buttons do nothing when clicked**
Each `AppButton` entry in `HomeScreenController.apps` must have either `targetScene` or `targetPanel` filled — not both empty. Check the apps list in the Inspector and confirm each entry is wired.

**Back button does nothing from a panel**
`homeScreenController` on `HomeScreenNavButtons` is not assigned. Drag `HomeScreenPanel` into the field — Unity finds `HomeScreenController` on it automatically.

**Items not spawning into scroll views**
`contactContainer` and `contentContainer` must point to the `Content` child inside `Viewport` — not `Viewport` itself and not the `ScrollView`.