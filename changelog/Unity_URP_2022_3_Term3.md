# Unity URP 2022_3_62f3 Term 3 Changelog

## 01/10/26

### Git Status Iteration 1

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7858/7858), done.
On branch Unity_URP_2022_3_Term3
Changes not staged for commit:
  (use "git add/rm <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
        modified:   Assets/1_SelfDrivingCar/Scenes/JungleTrackAutonomous.unity
        modified:   Assets/1_SelfDrivingCar/Scenes/JungleTrackTraining.unity
        modified:   Assets/1_SelfDrivingCar/Scenes/LakeTrackAutonomous.unity
        modified:   Assets/1_SelfDrivingCar/Scenes/LakeTrackTraining.unity
        modified:   Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/UISystem.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs
        deleted:    Assets/1_SelfDrivingCar/Scripts/term3/UISystemFusedTerm2_3.cs
        deleted:    Assets/1_SelfDrivingCar/Scripts/term3/UISystemFusedTerm2_3.cs.meta
        modified:   Assets/1_SelfDrivingCar/Term2_Scenes/MenuSceneTerm2.unity
        modified:   Assets/1_SelfDrivingCar/Term2_Scenes/project_4/LakeTrackAutonomous_pid.unity
        modified:   Assets/1_SelfDrivingCar/Term2_Scenes/project_5/LakeTrackAutonomous_mpc.unity
        deleted:    Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenu.unity
        deleted:    Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenu.unity.meta
        modified:   Assets/1_SelfDrivingCar/Term3_Scenes/MenuScene.unity
        modified:   Assets/1_SelfDrivingCar/Term3_Scenes/MenuScene.unity.meta
        modified:   Assets/1_SelfDrivingCar/Term3_Scenes/PathPlanning.unity
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
        modified:   ProjectSettings/EditorBuildSettings.asset
        modified:   README.md

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        Assets/1_SelfDrivingCar/Scenes/JungleTrackAutonomousSettings.lighting
        Assets/1_SelfDrivingCar/Scenes/JungleTrackAutonomousSettings.lighting.meta
        Assets/1_SelfDrivingCar/Scenes/JungleTrackTrainingSettings.lighting
        Assets/1_SelfDrivingCar/Scenes/JungleTrackTrainingSettings.lighting.meta
        Assets/1_SelfDrivingCar/Scenes/LakeTrackAutonomousSettings.lighting
        Assets/1_SelfDrivingCar/Scenes/LakeTrackAutonomousSettings.lighting.meta
        Assets/1_SelfDrivingCar/Scripts/Shared.meta
        Assets/1_SelfDrivingCar/Scripts/Shared/
        Assets/1_SelfDrivingCar/Scripts/UI.meta
        Assets/1_SelfDrivingCar/Scripts/UI/
        Assets/1_SelfDrivingCar/Scripts/term3/UISystemTerm3.cs
        Assets/1_SelfDrivingCar/Scripts/term3/UISystemTerm3.cs.meta
        Assets/1_SelfDrivingCar/Sprites/term3.meta
        Assets/1_SelfDrivingCar/Sprites/term3/
        Assets/1_SelfDrivingCar/Term2_Scenes/project_4/LakeTrackAutonomous_pidSettings.lighting
        Assets/1_SelfDrivingCar/Term2_Scenes/project_4/LakeTrackAutonomous_pidSettings.lighting.meta
        Assets/1_SelfDrivingCar/Term2_Scenes/project_5/LakeTrackAutonomous_mpcSettings.lighting
        Assets/1_SelfDrivingCar/Term2_Scenes/project_5/LakeTrackAutonomous_mpcSettings.lighting.meta
        Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenuTerm3.unity
        Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenuTerm3.unity.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/Shared.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/Shared/
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/BrakeLightTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/BrakeLightTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAudioTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAudioTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarRemoteControlTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarRemoteControlTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarUserControlTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarUserControlTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/MudguardTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/MudguardTerm2.cs.meta
        changelog/Review_Udacitys_SDC_Sim_Term3_v1_2.md
        changelog/Unity_CarAIControlTerm2GitDiff.md
        changelog/Unity_CarControllerTermGitDiff.md
        docs/images/path_planner_highway_menu_view.png

## 01/07/26

### Git Status Iteration 1

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7772/7772), done.
On branch Unity_URP_2022_3_Term3
Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
        modified:   Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/UISystem.cs
        modified:   Assets/Materials/pathElem.mat
        modified:   Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        .vsconfig
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab.meta
        Assets/1_SelfDrivingCar/Scripts/term2.meta
        Assets/1_SelfDrivingCar/Scripts/term2/
        Assets/1_SelfDrivingCar/Scripts/term3.meta
        Assets/1_SelfDrivingCar/Scripts/term3/
        Assets/1_SelfDrivingCar/Term3_Scenes.meta
        Assets/1_SelfDrivingCar/Term3_Scenes/
        Assets/New Terrain 2.asset
        Assets/New Terrain 2.asset.meta
        Assets/RoadKit/MouseOrbitImproved.cs
        Assets/RoadKit/MouseOrbitImproved.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/CarTraffic.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/CarTraffic.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/perfect_controller.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/perfect_controller.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/
        Assets/Standard Assets/Vehicles/Car/Scripts/term3.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term3/
        Assets/_TerrainAutoUpgrade.meta
        Assets/_TerrainAutoUpgrade/
        UpgradeLog.htm
        changelog/

### Git Status Iteration 2

This was after accounting for some of the C# scripts and adding them.

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7778/7778), done.
On branch Unity_URP_2022_3_Term3
Changes to be committed:
  (use "git restore --staged <file>..." to unstage)
        modified:   Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
        modified:   Assets/1_SelfDrivingCar/Scripts/UISystem.cs
        new file:   Assets/RoadKit/MouseOrbitImproved.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
        new file:   Assets/Standard Assets/Vehicles/Car/Scripts/CarTraffic.cs
        modified:   Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs
        new file:   Assets/Standard Assets/Vehicles/Car/Scripts/perfect_controller.cs
        new file:   Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs
        new file:   Assets/Standard Assets/Vehicles/Car/Scripts/term3/CarAIControlFusedTerm2_3.cs
        new file:   Assets/Standard Assets/Vehicles/Car/Scripts/term3/CarControllerFusedTerm2_3.cs

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
        modified:   Assets/Materials/pathElem.mat
        modified:   Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        .vsconfig
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab.meta
        Assets/1_SelfDrivingCar/Scripts/term2.meta
        Assets/1_SelfDrivingCar/Scripts/term2/
        Assets/1_SelfDrivingCar/Scripts/term3.meta
        Assets/1_SelfDrivingCar/Scripts/term3/
        Assets/1_SelfDrivingCar/Term3_Scenes.meta
        Assets/1_SelfDrivingCar/Term3_Scenes/
        Assets/New Terrain 2.asset
        Assets/New Terrain 2.asset.meta
        Assets/RoadKit/MouseOrbitImproved.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/CarTraffic.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/perfect_controller.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
        Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term3.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term3/CarAIControlFusedTerm2_3.cs.meta
        Assets/Standard Assets/Vehicles/Car/Scripts/term3/CarControllerFusedTerm2_3.cs.meta
        Assets/_TerrainAutoUpgrade.meta
        Assets/_TerrainAutoUpgrade/
        UpgradeLog.htm
        changelog/

### Git Status Iteration 3

This was after pushing the latest C# script updates to the repo;

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7827/7827), done.
On branch Unity_URP_2022_3_Term3
Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
        modified:   Assets/Materials/pathElem.mat
        modified:   Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab.meta
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab
        Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab.meta
        Assets/New Terrain 2.asset
        Assets/New Terrain 2.asset.meta
        Assets/_TerrainAutoUpgrade.meta
        Assets/_TerrainAutoUpgrade/
        UpgradeLog.htm

no changes added to commit (use "git add" and/or "git commit -a")
