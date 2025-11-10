## Quick repo-intel for AI coding agents

This file contains concise, actionable guidance so an AI coding assistant can be productive immediately in the IGNITES Unity project.

Keep it short — follow these repository-specific conventions, important hotspots, and developer workflows.

1. Project overview
   - Unity 2D/3D hybrid (art + gameplay prototype). Open with Unity Hub (recommended: 2022 LTS or newer). See `README.md` for the high-level description.
   - C# scripts live in `Assets/Scripts/`. Scenes are under `Assets/Scenes/`.

2. Important patterns & conventions (search these files for examples)
   - Movement & physics: scripts commonly use a Rigidbody2D assigned via inspector and set in `Start()` (e.g. `PlayerMovement.cs`, `Enemy_Movement.cs`). Look for `rb` and velocity manipulation.
   - Animator usage: animations are driven by boolean flags and triggers. Example parameters: `isIdle`, `isChasing`, `isAttacking`, `Attack`, `Attack-2`, `Hurt`, `Die`. See `Enemy_Movement.cs`, `Enemy.cs`, `PlayerMovement.cs` for naming conventions.
   - Tagging: collision/trigger code checks GameObject tags (e.g. `if (collision.gameObject.tag == "Player")`). Respect and preserve tag names when refactoring.
   - Facing/flip: character facing is implemented by flipping `transform.localScale.x` and a `facingDirection` integer (-1/1). Keep that pattern to preserve animation orientation.
   - Audio: AudioClips and AudioSources are assigned in inspector; code calls `PlayOneShot` or sets `clip` + `Play()` (see `PlayerMovement.cs`).

3. Hotspots to inspect when changing gameplay code
   - `Assets/Scripts/PlayerMovement.cs` — player input, attack triggers, footstep audio logic.
   - `Assets/Scripts/Enemy_Movement.cs` — enemy AI state machine (Idle/Chasing/Attacking), trigger-based player detection, animator flags.
   - `Assets/Scripts/Enemy_Combat.cs` and `Enemy.cs` — damage, health, death handling and animator triggers.
   - `InputSystem_Actions.inputactions` — if you change input bindings, update this asset or use Unity Input System.

4. Build / run / debug workflows
   - Primary dev loop: open project in Unity Editor via Unity Hub (2022 LTS+), press Play to test scene. No CI/build scripts present in repo.
   - Source control: commit .meta files (Unity expects them). Use standard Git + GitHub flow.
   - If you see compile errors in the Editor, open the Console window — Unity will report script compile errors and the failing C# file/line.

5. Project-specific gotchas an AI should watch for
   - Non-standard API usage: search for `rb.linearVelocity` in the codebase — the canonical Unity property is `rb.velocity`. If Editor reports an error, update accordingly and run the Editor again.
   - Animator parameter names are hard-coded strings. Changing names requires updating all callers and the Animator controllers in the Editor.
   - Many behaviours rely on component assignment in the Inspector (Animator, Rigidbody2D, AudioSource). When editing/adding scripts, ensure required components are assigned or add null checks and helpful log messages.
   - Tag and layer names (e.g., `Player`) are used as control flow points. Don't rename tags without updating code and Scenes.

6. Examples of safe edits
   - Small fix: replace `rb.linearVelocity = ...` with `rb.velocity = ...` (if Editor shows an undefined member). Keep the same vector math.
   - Improve robustness: wrap GetComponent lookups with null-checks and editor-time warnings (e.g., `if (!rb) rb = GetComponent<Rigidbody2D>();` and `Debug.LogError` when missing).

7. Files to reference when writing instructions or PR descriptions
   - `Assets/Scripts/PlayerMovement.cs` — player input & sounds
   - `Assets/Scripts/Enemy_Movement.cs` — enemy AI state logic and animator flags
   - `Assets/Scripts/Enemy.cs` / `Enemy_Combat.cs` — health, damage, death
   - `InputSystem_Actions.inputactions` — input bindings

If anything here is unclear or you want more detail on the Animator controllers, input bindings, or scene wiring, tell me which area and I will add a short focused section with example code references.
