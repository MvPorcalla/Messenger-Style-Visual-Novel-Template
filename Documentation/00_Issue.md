# ISSUE:

---

TODO:

another is when i enter a chatapppanel i see the flicker of the content from empty to populating it


---

TODO: Critical

I’m noticing some behavior: DialougeExecutioner.cs

## Why is it doing this, and should I leave it as-is?

**Scenario: conversation reaches a choice, player backs out**

| Re-entry | Expected |
|---|---|
| 1st re-entry | Choice buttons shown directly — no pause button |
| Every re-entry after | Same — choice buttons shown directly |

**Scenario: conversation is mid-messages when player backs out**

| Re-entry | Expected |
|---|---|
| 1st re-entry | Pause button shown (safe resume point) → press it → messages replay → choice/end shown |
| Every re-entry after | Same as above until player actually progresses |


i quest his is fine 

but my real issue is on fast mode
everytime i reach choice or end button it show the pause button first before the choice or end button
also the replaying are getting out of hand when i reach the end and re enter it keeps replaying from the last pause point

---

TODO:

simplify GalleryFullscreenViewer.cs

the backbutton in the PhoneHomescreen when i open full screen it directly send it back to homescreenpanel instead of back to gallerypanel like the closebutton
it should be when i open the fullscreen of image when iback from the phonehomescreen.cs 
fullscreen -> gallerypanel -> homescreen

consider making a GalleryController and put all the script there

---

TODO: 

Fix image fullscreen viewer there is 2 script maybve combine it and put in common folder for reusability
Fix confirmation dialogue currently its messy

---

TODO: 

Note: BubbleSpinner is a standalone script for parsing `.bub` files. It connects to the UI through a bridge.

---