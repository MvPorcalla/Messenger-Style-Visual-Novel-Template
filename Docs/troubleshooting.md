# Troubleshooting

Common mistakes organized by system.

---

## Bootstrap

**"SaveManager not assigned in Inspector!"**
`SaveManager` must be dragged into the `GameBootstrap` Inspector field manually — it is not found automatically. Ensure it is a child of `GameBootstrap` and assigned in the Inspector.

**"ConversationManager not initialized!"**
Something is calling `GameBootstrap.Conversation` before Bootstrap has finished initializing. Do not call it from `Awake()` — use `Start()` or wait for a `GameEvents.OnSceneLoaded` callback.

**Bootstrap scene shows a black screen**
Expected. Bootstrap has no Camera or Canvas by design.

**"Scene 'XX' not found in Build Settings"**
Scene name constants in `SceneNames.cs` must match your actual scene file names exactly, including capitalisation.

---

## Disclaimer

**Disclaimer shows on every launch**
`HasSeenDisclaimer` is not being written correctly. Verify `MarkAccepted()` is called before `LoadBootstrap()` and that `PlayerPrefs.Save()` is called immediately after.

**ContinueButton never becomes interactable**
`checkBoxToggle` reference is missing or `RegisterListeners()` is not being reached. Verify the reference is assigned in the Inspector.

**Disclaimer skipped on first launch**
`skipForTesting` is set to `true` in the Inspector. Uncheck it before building.

**ExitButton does nothing**
`Application.Quit()` has no effect in the Unity Editor. Test exit behavior on a device build.

**TOSPanel visible on start**
Set `TOSPanel` inactive in the Inspector before entering Play Mode.

---

## Chat App

**`messageDisplay` shows None after dragging**
Drag the `ChatPanel` GameObject from the Hierarchy — not the script file from the Project window. Unity finds the `ChatMessageSpawner` component automatically.

**Messages spawn in the wrong place**
`chatContent` must point to the `Content` GameObject inside `Viewport` — not `Viewport` itself and not `ChatPanel`.

**`choiceContainer` left empty**
Drag `ChatChoices` itself — the same GameObject the `ChatChoiceSpawner` script is on.

**`timingController` or `autoScroll` shows None**
Both scripts live on `ChatAppController`. Drag the `ChatAppController` GameObject into both fields.

**Dialogue freezes after messages display**
`OnMessagesDisplayComplete()` is not being called after message animations finish. This must be called by the UI layer when display is done — forgetting it halts all dialogue flow.

**Bubbles not appearing at runtime**
All bubble container instances must be removed from the scene. `Content` should be empty except `TypingIndicator`. Prefabs are spawned at runtime by `ChatMessageSpawner`.

---

## Phone Screen

**Gallery shows no thumbnails**
`ConversationAsset` files must have their `cgAddressableKeys` list populated. Open each asset in the Inspector and verify the list has entries such as `Sofia/CG1`.

**Gallery fade animation doesn't work**
`Canvas Group` component is missing from `GalleryFullscreenViewer`. Add it via Add Component in the Inspector.

**GalleryFullscreenViewer invisible on open**
`GalleryFullscreenViewer` was set inactive in the scene. It must stay active — the script manages its own visibility.

**Contacts list shows empty**
`CharacterDatabase.asset` must have valid character entries. Open it in the Inspector and verify the list is populated.

**Reset button does nothing**
Do not wire `ResetButton.onClick` manually in the Inspector — `ContactsAppItem.cs` wires it at runtime. Manual connections may conflict.

**ResetConfirmationDialog invisible on open**
`ResetConfirmationDialog` was set inactive in the scene. It must stay active.

**Items not spawning into scroll views**
`contactContainer` and `contentContainer` must point to the `Content` GameObject inside `Viewport` — not `Viewport` itself.

---

## BubbleSpinner

**Dialogue produces no output**
Events were subscribed after `ContinueFromCurrentState()` was called. Always subscribe to executor events before calling `ContinueFromCurrentState()` — events fired before subscription are lost.

**`.bub` file not recognized in Unity**
`BubFileImporter.cs` is missing from an `Editor/` folder. Unity requires this importer to treat `.bub` files as `TextAsset`. Add it and reimport.

**"Node not found" warning in Console**
A `<<jump>>` target does not match any `title:` in the current file. Check spelling and capitalisation. Cross-chapter targets must follow the `_Ch2`, `_Ch3` pattern to suppress the warning.

**Choice buttons never appear**
`>> endchoice` is missing and `===` did not close the block. Add `>> endchoice` after the last choice option.

**CG not unlocking to gallery**
`unlock:true` must appear before `path:` in the `>> media` command. Also verify the Addressables key matches exactly.

**Conversation resumes from wrong position**
`OnMessagesDisplayComplete()` was not called after a message batch. BubbleSpinner uses this call to advance its internal state — skipping it causes resume position to be recorded incorrectly.

**Save file grows very large**
`messageHistory` in `ConversationState` grows indefinitely. This is expected — each displayed message is appended for UI reload on re-entry. Consider capping history length for very long conversations.

---

## Pooling

**"Cannot recycle — no PooledObject component" warning**
An object was instantiated manually (not via `PoolingManager.Get()`) and then passed to `Recycle()`. Only objects retrieved through `PoolingManager.Get()` can be recycled.

**Pooled objects show stale content**
`ResetForPool()` is not being called before recycling. Call it on the component before passing the object to `Recycle()`.