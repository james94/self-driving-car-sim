# Unity URP 2022_3_62f3 Term 3 Changelog

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
