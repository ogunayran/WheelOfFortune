# Wheel of Fortune — Vertigo Games Developer Case

A risk/reward wheel spinning game inspired by the "Card Game" mode in Critical Strike.
Built with Unity 2021.3 LTS, TextMeshPro and DOTween.

## Gameplay

- Each zone the player spins a wheel of 8 slices: rewards + one bomb (normal zones).
- Hitting the bomb loses everything collected; the player may give up (restart) or pay currency to revive (bonus feature).
- Every 5th zone is a **safe zone** (silver wheel, no bomb), every 30th a **super zone** (golden wheel, special rewards, no bomb).
- While the wheel is idle the player can **cash out** and leave with everything collected.
- Rewards get better every zone (configurable growth factor).

## Architecture

```
Scripts/
  Configs/   ScriptableObjects: GameConfig (rules/tuning), WheelConfig (per-wheel slices & art),
             RewardDefinition. All slice contents are editable from the inspector.
  Core/      GameManager (state machine, single flow owner), RewardInventory & CurrencyWallet
             (plain C#, unit-testable), GameEvents (static event hub, Observer pattern).
  Wheel/     WheelController (builds wheel from data, spin tween), WheelView, WheelSliceView.
  UI/        AutoBoundView base (OnValidate reference binding) + passive views driven by events.
  Editor/    UiRuleValidator: menu tools that audit naming, RaycastTarget, TMP and sliced-sprite rules.
```

Design principles applied:

- **Single responsibility / SOLID** — rules live in configs, flow in GameManager, presentation in views.
- **Observer pattern** — UI never polls or reaches into logic; it reacts to `GameEvents`.
- **Data-driven** — wheels are built at runtime from `WheelConfig` assets; no hand-placed slices.
- **Editor automation** — all view references are bound in `OnValidate` (no drag & drop, no editor OnClick).

## Design decisions worth noting

- Cash out is allowed in any zone while the wheel is idle, matching the brief's
  "before spinning the wheel the player has a chance to walk away".
- Revive keeps the collected rewards and respins the same zone.
- Slice odds are uniform; a weighted variant would only change `GameManager.RequestSpin`.

## How to run

Open with Unity 2021.3 LTS, open `Scenes/Game`, press Play.
Android build: File > Build Settings > Android > Build (APK attached to the GitHub release).
