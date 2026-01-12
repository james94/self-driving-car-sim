# Kept T3 Path Planner Working, Restored Behavioral Cloning, PID, MPC, EKF, UKF, Particle Filter Scenes

URP 2022.3 migration, cross-term inheritance refactor (T1/T2/T3), unified UI/servers/telemetry, audio NRE fixes, and scene routing

## Summary

- Migrated Udacity Self-Driving Car simulator to Unity 2022.3.62f3 (URP).
- Kept Term 3 Path Planner working while restoring Term 1 Behavioral Cloning and Term 2 PID/MPC/EKF/UKF/Particle Filter scenes.
- Reduced duplication via shared base classes with term-specific children.
- Standardized socket telemetry, UI interactions, and audio behavior across scenes.

## Highlights

Shared base layers introduced:

- CarControllerBase, CarAIControlBase, CarAudioBase, CarUserControlBase, CarRemoteControlBase
- BrakeLightBase, MudguardBase, UISystemBase
- CommandServerBase with InitSocket, RegisterHandler, Enqueue, IsManualInputActive, CaptureFrameBase64, TryGetFloat, ConvertUnityYawToMathAngle


Term 3 children updated to inherit bases:

- CarController, CarAIControl, CarAudio, CarUserControl, CarRemoteControl, Mudguard

Term 1/2 children added/updated:

- CarControllerTerm2, CarAIControlTerm2, CarAudioTerm2, CarUserControlTerm2, CarRemoteControlTerm2, MudguardTerm2, BrakeLightTerm2

Networking refactors:

- CommandServer (Path Planner), CommandServerTerm2 (PID), CommandServer_pid, CommandServer_mpc now use CommandServerBase
- Telemetry payloads use JSONObject.AddField, consistent typed fields, try/catch, and manual-mode suppression


Audio fixes:

- CarAudioBase adds ListenerTarget with fallback to Camera.main/AudioListener to prevent NullReferenceException
- Unified 1-channel and 4-channel mixing, doppler configuration applied consistently


UI consolidation:

- UISystem and UISystemTerm2 now inherit UISystemBase (MPH UI, Escape-key handling)
- UISystemTerm2 overrides OnEscapePressed to route back to Term 1 or Term 2 menus
- MenuOptions supports Term1/Term2/Term3 flags and routes to ControlMenu/MenuScene variants


Waypoint tracker changes:

- WaypointTracker_pid/mpc updated to operate on CarControllerTerm2


Scenes:

- Updated Term 1/2 (Lake/Jungle Training/Autonomous, PID, MPC) scenes
- Term 3 Menu/Path Planning updated; legacy ControlMenu removed; ControlMenuTerm3 added

## Technical changes


CarAIControlBase:

- Shared cautious speed, angle/distance braking, lateral wander, evasive avoidance, and helpers: InitAICommon, DesiredSpeedByDirection/Distance, ComputeAccel, ComputeSteer
- CarAIControl and CarAIControlTerm2 consume shared helpers

CarControllerBase:

- Shared sensors (ToggleSensorView, SenseDistance, getSensors), motion sensing (AccelerationT/N, Jerk), UpdateMainCarMotionSensing, and common properties (CurrentSpeed, CurrentSteerAngle, AccelInput, MaxSpeed via abstract GetTopSpeed)
- CarController/CarControllerTerm2 call InitControllerBase and use UpdateMainCarMotionSensing

CarAudioBase:

- Shared engine audio sources and per-frame mixing via UpdateAudio
- ListenerTarget fallback chain prevents NRE when Camera.main is absent
- CarAudio/CarAudioTerm2 provide GetRevs/GetAccelInput

CarUserControlBase and CarRemoteControlBase:

- Centralize input reading (Steering helper), provide abstract ApplyMove for child controllers

BrakeLightBase and MudguardBase:

- Centralize renderer toggle and steer-based local rotation; children return GetBrakeInput/GetSteerAngle from their bound controllers

CommandServerBase:

- Socket lifecycle helpers, main-thread queueing, robust float parsing, safe camera frame capture, yaw-angle conversion
- CommandServer_pid and CommandServer_mpc reworked to use base helpers; telemetry fields now typed and consistent with Term 2 practices

## Behavioral notes

- Assign appropriate child components per scene:
  - Term 1/2: CarControllerTerm2 with CarAudioTerm2, CarUserControlTerm2, CarRemoteControlTerm2, MudguardTerm2, BrakeLightTerm2
  - Term 3: CarController with CarAudio, CarUserControl, CarRemoteControl, Mudguard, BrakeLight
- Ensure FrontFacingCamera is set where telemetry images are required (PID/Term 2, Path Planner).
- For audio rolloff, optionally assign ListenerTarget to a specific camera or AudioListener.

## Scene/menu routing

- Term 1: ControlMenu/MenuScene
- Term 2: ControlMenuTerm2/MenuSceneTerm2
- Term 3: ControlMenuTerm3/MenuSceneTerm3
- PathPlanning scene available from Term 3 and Term 2 project carousel in MenuOptions

## Fixes
- Audio NRE in Behavioral Cloning resolved by ListenerTarget fallback.
- Telemetry payload stability via typed AddField and main-thread Enqueue.
- Robust steer/throttle parsing with TryGetFloat; safer manual-mode detection.

Co-authored-by: GitHub Copilot (using GPT-5)
