Here's the full `.bub` format reference:

---

# BubbleSpinner — .bub Format Reference

## File Structure

```
contact: CharacterName

title: NodeName
---
[content here]
===

title: NextNode
---
[content here]
===
```

---

## Commands

### `contact: Name`
Decorative header. Validated against `ConversationAsset.characterName` at parse time. Mismatch logs a warning but doesn't stop the game.

---

### `title: NodeName`
Starts a dialogue node. Must be unique within the file. First node is typically `Start`.

> Cross-chapter targets use `_Ch2`, `_Ch3` suffix — e.g. `title: Start_Ch2`

---

### `---` and `===`
Both treated identically. Use `---` after `title:` and `===` to end a node.
`===` also implicitly closes any open choice block.

---

### `[Speaker]: "Text"`
Text message bubble. Quotes are optional — parser strips them.

```
Sofia: "Hey!"
Player: "Hi!"
System: "9:42 AM"
```

**Player lines can appear anywhere** — no `-> ...` required before them unless you want a tap pause first.

---

### `Player: "..."`
Convention for a silent acknowledgement tap. Just a regular `Player:` line — `...` is not a special command, just a writing convention.

```
-> ...
Player: "..."
Sofia: "Anyway..."
```

---

### `System: "Text"`
Non-chat system message (timestamps, scene breaks). Case-insensitive. Can appear anywhere in a node.

```
System: "Later that Day."
```

---

### `>> media [Speaker] type:image path:[key]`
Image bubble. `path:` must be a valid Addressables key. Place `path:` last.

```
>> media npc type:image path:Sofia/happy
```

### `>> media [Speaker] type:image unlock:true path:[key]`
Same as above but also unlocks the CG to gallery and fires `OnCGUnlocked`. `unlock:true` must come before `path:`.

```
>> media npc type:image unlock:true path:CGs/Sofia_CG1
```

---

### `-> ...`
Tap-to-continue pause point.

**Pure pacing pause:**
```
Sofia: "I have something to tell you."
-> ...
Sofia: "I like you."
```

**Player-turn pause** — if next non-empty line is `Player:`, tapping sends that message first then resumes NPC:
```
Sofia: "What do you think?"
-> ...
Player: "I think it's great."
Sofia: "Really?"
```

**Pause before jump** — valid:
```
>> media npc type:image unlock:true path:Sofia/CG3
-> ...
<<jump EndNode>>
```

> Cannot be inside `>> choice`. Produces a warning and is ignored.

---

### `>> choice` / `>> endchoice`
Opens and closes a choice block.

```
>> choice
    -> "Option A"
        # Player: "I'll take A."
        <<jump NodeA>>

    -> "Option B"
        # Player: "I'll take B."
        <<jump NodeB>>
>> endchoice
```

- `>> endchoice` is recommended but `===` implicitly closes the block too
- Nested choice blocks not supported
- `-> ...` inside choice block is ignored with a warning
- Missing `<<jump>>` on a choice logs a warning

---

### `-> "Choice Text"`
One choice button. Must be inside `>> choice`. Text in double quotes.

```
-> "Let's go to the park"
```

---

### `# [Speaker]: "Text"`
Player message after selecting a choice. Must be inside a choice option after `-> "text"`.

```
-> "Let's go to the park"
    # Player: "Let's go to the park!"
    <<jump ParkNode>>
```

> Non-`#` lines inside choice options are ignored with a warning.

---

### `<<jump NodeName>>`
Jump to another node. If not found in current file, BubbleSpinner advances to the next chapter file.

```
<<jump EndNode>>
<<jump Node_Concern>>
<<jump Start_Ch2>>
```

---

### `//`
Comment. Inline or full line. Also used for section dividers between nodes.

```
Sofia: "Hi!" // greeting

//=====================================
// CONCERN NODE
//=====================================
title: Node_Concern
```

---

## Known Limitations

- `type:audio` not implemented — falls through to unrecognized line warning
- Variables (`<<set>>`) and conditionals (`<<if>>`) not yet implemented
- Cross-chapter warning suppression only covers Ch2–Ch5
- Timestamps assigned at parse time, not display time
- Message history grows with playtime — save file grows accordingly
- `contact:` mismatch is warning only, game continues
- Nested choice blocks not supported