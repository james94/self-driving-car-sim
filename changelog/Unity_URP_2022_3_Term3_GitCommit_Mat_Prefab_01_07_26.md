# chore(urp, term3): migrate road/path materials to URP Lit and stage Term 3 prefabs

## Summary
- Upgrade project assets for Unity 2022.3.62f3 URP.
- Convert legacy/standard materials to URP Lit with correct properties and metadata.
- Prepare Term 3 scene prefabs (car, UI, server, skid trail, socket/thread dispatcher) for standalone Highway Path Planner.

## Materials

Assets/Materials/pathElem.mat:

- Switch shader to URP Lit (guid 933532a4fcc9baf4fa0491de14d08ed7).
- Add URP properties: BaseMap, Smoothness/WorkflowMode, ReceiveShadows, SpecGlossMap, etc.
- Insert URP material version MonoBehaviour (version: 7).

Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat:

- Update to serializedVersion 8 with URP Lit shader (guid 933532a4fcc9baf4fa0491de14d08ed7).
- Add _EMISSION keyword, Opaque RenderType tag, URP floats/colors, and texture slots.
- Insert URP material version MonoBehaviour (version: 7).

## Untracked assets to add (Term 3 prefabs and upgrade artifacts)

Assets/1_SelfDrivingCar/Prefabs/* copy.prefab (+ .meta) including:

- Car 1, Car copy, CarUI copy, CarWaypointBased copy
- CommandServer copy, SimulatorUI copy
- SkidTrail copy, SocketIO copy, ThreadDispatcher copy

Terrain upgrade outputs:

- Assets/New Terrain 2.asset (+ .meta)
- Assets/_TerrainAutoUpgrade/ and .meta
- UpgradeLog.htm

Why:

- Align materials and scene assets with URP to ensure correct rendering in Unity 2022.3.
- Decouple Term 3 scene from fused Term 1/2 scripts while keeping a path to reintegration.

Follow-ups:

- git add the listed prefabs and terrain upgrade assets when ready.
- Verify URP renderer and pipeline settings across scenes.
- Revisit fused Term 1/2/3 approach after stabilizing Term 3 (share common shaders/materials where possible).



