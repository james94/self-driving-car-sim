chore!: migrate to Unity 2022.3, unify Term 1/2/3 via shared base classes, and stabilize PID/MPC/Path Planning pipelines

Summary
- Upgraded Highway Driving Path Planner (Term 3) from Unity 2017.1.0f3 to 2022.3.62f3 (URP).
- Restored Term 1 (Behavioral Cloning) and Term 2 (PID, MPC, EKF/UKF/Particle Filter) by introducing shared base classes and term-specific children.
- Unified socket servers and telemetry across PID/MPC/Path Planning with safer parsing and image capture helpers.
- Fixed audio NREs and normalized engine audio across Term 1/2/3.

Why
- Enable building and launching Term 3 Path Planner while keeping all Term 1/2 scenes functional.
- Reduce code duplication and ease maintenance by leveraging inheritance and common utilities.

Key changes
- Shared base components
  - CarControllerBase, CarAIControlBase, CarAudioBase, CarUserControlBase, CarRemoteControlBase
  - BrakeLightBase, MudguardBase
  - CommandServerBase (InitSocket, RegisterHandler, Enqueue, IsManualInputActive, CaptureFrameBase64, TryGetFloat, ConvertUnityYawToMathAngle)
- Term 3 children updated to inherit:
  - CarAudio, CarUserControl, CarRemoteControl, Mudguard, CarAIControl, CarController
- Term 1/2 children added/updated:
  - CarControllerTerm2, CarAIControlTerm2, CarAudioTerm2, CarUserControlTerm2, CarRemoteControlTerm2, MudguardTerm2, BrakeLightTerm2
- Audio
  - CarAudioBase: ListenerTarget with fallbacks (Camera.main/AudioListener) to prevent NullReferenceException
  - Unified 1/4-channel mixing and doppler configuration
- Command servers
  - CommandServer_pid and CommandServer_mpc now inherit CommandServerBase
  - Use RegisterHandler/Enqueue, IsManualInputActive, CaptureFrameBase64, TryGetFloat
  - Telemetry uses JSONObject.AddField with typed values; improved error handling
- Waypoint trackers
  - WaypointTracker_pid/mpc now work with CarControllerTerm2 types
- UI
  - UISystem/UISystemTerm2 now inherit UISystemBase (shared MPH UI handling and Escape behavior)
  - MenuOptions supports Term1/Term2/Term3 flags, routes to correct control/menu scenes, and Term 2 project carousel
- Scenes
  - Updated Term 1/2 scenes (Lake/Jungle Training/Autonomous, PID, MPC)
  - Updated Term 3 menu/path planning; added ControlMenuTerm3; removed legacy ControlMenu

Behavioral changes
- BrakeLight/Mudguard/Audio/UserControl/RemoteControl/AIControl scripts now reference base classes; drag appropriate Term1/2 or Term3 controller in Inspector.
- PID/MPC telemetry payloads are consistent and safer (AddField, try/catch, manual-mode suppression).

Fixes
- Prevent CarAudio NRE when Camera.main is missing by adding ListenerTarget and fallbacks.
- Stabilized telemetry in PID/MPC with robust JSON parsing and queueing on main thread.

Migration notes
- Assign correct term-specific components on prefabs (e.g., CarAudioTerm2 with CarControllerTerm2).
- Ensure FrontFacingCamera set where telemetry images are required.
- For Term 3 control/menu, use ControlMenuTerm3/MenuSceneTerm3; Term 1 uses ControlMenu/MenuScene; Term 2 uses ControlMenuTerm2/MenuSceneTerm2.

Co-authored-by: GitHub Copilot (using GPT-5)
