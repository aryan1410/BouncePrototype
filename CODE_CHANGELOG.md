# Code Changelog

## 2026-09-21 - Fix enemy stomp detection

### Changed
- Updated `Assets/Scripts/Enemy.cs`, in `OnCollisionEnter2D`.
- Preserved the original height-and-velocity check inside a block comment.
- Inspect every collision contact and accept a stomp when `contact.normal.y * gSign < -0.7f`.
- Keep the existing bounce, enemy defeat notification, goal unlock, and side-hit respawn behavior.
- Keep reversed-gravity support through the existing `gSign` value.

### Reason
The physics solver may stop the ball before the collision callback reads its velocity, causing a valid landing to fail the old downward-velocity check. Contact normals identify impacts on the enemy's gravity-facing top surface without depending on its post-collision velocity.

### Code locations
File: `Assets/Scripts/Enemy.cs`  
Method: `OnCollisionEnter2D(Collision2D collision)`.

| Section | Line range in the updated file | Details |
| --- | --- | --- |
| Preserved old code, commented out | 52-59 | Block-comment delimiters are on lines 52 and 59; the original logic and its comments remain on lines 53-58. Line 51 labels the old implementation. |
| New stomp detection | 61-72 | Explanation on lines 61-62; executable contact-normal logic on lines 63-72. |
| Contact-direction threshold | 67 | Accept a stomp when `collision.GetContact(i).normal.y * gSign < -0.7f`. |
| Existing outcome handling | 74-85 | Bounce, enemy defeat, goal unlock, and side-hit respawn logic are preserved. |

Line numbers refer to the file immediately after this update and may shift with future edits.

### Updated code
```csharp
bool stomped = false;
for (int i = 0; i < collision.contactCount; i++)
{
    if (collision.GetContact(i).normal.y * gSign < -0.7f)
    {
        stomped = true;
        break;
    }
}
```

### Scope
- The normal is evaluated in the enemy's collision callback: a normal-gravity top hit points downward.
- The threshold accepts top contacts within approximately 45 degrees of vertical. Pure side and opposite-side hits still cause a respawn.
- This shared script affects enemies in any scene that uses it, including Level1.
- No scene layout, collider settings, controls, or remote GitHub branches were changed.

### Validation
- Compiled all nine gameplay scripts successfully against Unity 6000.3.22f1 assemblies, with no errors. The standalone compiler reported only CS0649 warnings for the two existing Inspector-assigned fields in `Level3Completion.cs`.
- Verified all scene files have the same SHA-256 hashes as before the edit.
- Unity Play Mode checks are pending: straight top landing, diagonal top landing, side collision, inverted-gravity stomp, bounce, and goal unlock.


## 2026-09-23 - Restore completion UI after renaming Level3 to Level2

### Changed
- Updated `Goal.OnTriggerEnter2D` to show the completion popup in `Level2`, the renamed final scene, when the exit requirements are met and `Level3Completion.Instance` is available.
- Updated `Level3Completion.PlayAgain` to reload the active scene by build index.
- Preserved both previous implementations as comments. The class name remains unchanged to preserve existing component and button bindings.

### Reason
The original goal condition required the exact scene name `Level3`, so renaming the scene to `Level2` bypassed the victory popup and advanced to the next scene. Checking the new name restores the popup, while reloading the active scene prevents the replay button from returning to the old scene name.

### Code locations
| File | Preserved old code, commented out | New code | Explanation |
| --- | --- | --- | --- |
| `Assets/Scripts/Goal.cs` | Lines 30-38 (original code and comments: 31-37) | Lines 41-45 | Line 40 explains the renamed final scene. |
| `Assets/Scripts/Level3Completion.cs` | Line 37 | Line 40 | Line 39 explains replaying the active scene. |

Line numbers refer to the files immediately after this update and may shift with future edits.

### Updated code
`Assets/Scripts/Goal.cs`:
```csharp
if (SceneManager.GetActiveScene().name == "Level2"
    && Level3Completion.Instance != null)
    Level3Completion.Instance.ShowWin();
else
    LevelManager.Instance.LoadNextLevel();
```

`Assets/Scripts/Level3Completion.cs`:
```csharp
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
```

### Validation
- Compiled all 14 gameplay scripts successfully against Unity 6000.3.22f1 assemblies with no errors. The standalone compiler reported only the two existing CS0649 warnings for Inspector-assigned `winPopup` and `player` fields.
- Verified scene and scene meta file hashes were unchanged by this code update.
- Unity Play Mode verification is pending: reach the Level2 goal, confirm the victory popup, then use Play Again to restart Level2.

### Scope
- Level2 is now treated as the final scene for this popup; other scene names continue through `LoadNextLevel`.
- No scene layout, build order, Git commits, or remote branches were changed.
