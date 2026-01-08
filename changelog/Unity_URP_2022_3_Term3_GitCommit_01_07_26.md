
# Unity URP 2022_3_62f3 Term 3 Git Commit

## 01/07/26: Git Commit Message

chore(term3): migrate to Unity 2022.3.62f3 and switch to standalone Term 3 Highway Path Planner

Summary
- Upgrade the project to Unity URP 2022.3.62f3 and refactor primary runtime scripts to target Term 3 (Highway Path Planning) as a standalone flow.
- Replace Term 1/2 telemetry and training behaviors with Term 3 control/telemetry, frenet frame, incident tracking, and HUD.

Details
- CommandServer.cs

- Replace Term 1/2 "steer" flow with Term 3 "control" event.
- Wire perfect_controller for trajectory ingestion and toggling server/simulator processing.
- Emit Term 3 telemetry: x, y, yaw (converted), speed, s, d, previous_path_x/y, end_path_s/d, and sensor_fusion.
- Add yaw conversion helper for Unity-to-math convention.

- UISystem.cs

- Switch HUD to Term 3 metrics: AccT, AccN, Acc, Jerk, Collision/Speeding/Lane incidents, best/current distance, and timer.
- Hook into CarAIControl for incident checks and distance/time evaluation.
- Add manual/auto toggle compatible with MouseOrbitImproved camera.

- CarAIControl.cs

- Add Term 3 highway features: waypoint tracking, frenet conversion (getThisFrenetFrame, getFrenetFrame), lane-change logic, spawn logic, and forward/reverse traffic support.
- Implement incident detection (collision, speeding, lane keeping) and expose DistanceEval/TimerEval for HUD.
- Provide helpers for NextWaypointDistance and lane clear checks via CarTraffic.

- CarController.cs

- Make WheelEffects optional/null-safe and remove hard dependency on tyre FX for Term 3 prefabs.
- Adjust braking to allow braking at any forward speed (>0 mph).
- Keep acceleration/jerk sensing; strip Term 2 data-recording/CSV/camera capture paths from the primary controller to avoid conflicts.

- WheelEffects.cs

- Make particle and skid trail logic null-safe to align with Term 3 prefabs that omit FX.

- MenuOptions.cs

- Simplify to Term 3: outline init, ControlMenu/MenuScene routing, and StartPathPlanning entry point.

- CarUserControl.cs
  
- Ensure manual driving path is available via simple Steering wrapper (for Term 3 manual/auto toggles).

Notes
- Backups of Term 2 scripts and fused Term 2/3 variants are kept under Assets/.../term2 and Assets/.../term3; they are staged in this commit.
- Follow-up: revisit fused approach to support Term 1/2/3 scenes concurrently within the same project.
- Breaking: Term 1/2 telemetry + recording paths removed from the primary scripts; use the term2 backups if needed.

Refs
- Git diffs captured in Unity_URP_2022_3_Term3_GitDiff_01_07_26.md
