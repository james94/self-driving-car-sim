# Review Udacity's SDC Sim Term 3 v.12 Branch

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/open/udacity-sdc-sim$ git branch
  master
* term3_collection
ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/open/udacity-sdc-sim$ git checkout T3_v1.2
Updating files: 100% (134/134), done.
Note: switching to 'T3_v1.2'.

You are in 'detached HEAD' state. You can look around, make experimental
changes and commit them, and you can discard any commits you make in this
state without impacting any branches by switching back to a branch.

If you want to create a new branch to retain commits you create, you may
do so (now or later) by using -c with the switch command. Example:

  git switch -c <new-branch-name>

Or undo this operation with:

  git switch -

Turn off this advice by setting config variable advice.detachedHead to false

HEAD is now at deaf0777 Merge pull request #13 from udacity/extras
ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/open/udacity-sdc-sim$ git branch
* (HEAD detached at T3_v1.2)
  master
  term3_collection
ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/open/udacity-sdc-sim$ git lfs pull
ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/open/udacity-sdc-sim$ git lfs ls-files
36e4565319 * Assets/1_SelfDrivingCar/Sprites/Background.png
d34858a2ec * Assets/1_SelfDrivingCar/Sprites/LogoUdacity.png
27cb2e5d89 * Assets/1_SelfDrivingCar/Sprites/MenuBackground.png
7ec4084cef * Assets/1_SelfDrivingCar/Sprites/MenuButton.png
9568b8f750 * Assets/1_SelfDrivingCar/Sprites/RecordSprite.png
f4ac639d2d * Assets/1_SelfDrivingCar/Sprites/Speed.png
a2885f6a61 * Assets/1_SelfDrivingCar/Sprites/SplashSprite.png
66d2f1bada * Assets/1_SelfDrivingCar/UI/Speed180_93x186.png
9a42776a18 * Assets/1_SelfDrivingCar/UI/drivingUI_Base.png
4a9b23df6c * Assets/1_SelfDrivingCar/UI/drivingUI_Reference.png
9e56f16f06 * Assets/1_SelfDrivingCar/UI/record_disabled_80x85.png
e9e2657fae * Assets/1_SelfDrivingCar/UI/record_pause_80x84.png
5edd8f5289 * Assets/Plugins/SimpleFileBrowser/Sprites/DefaultFileIcon.png
f9d605cf77 * Assets/Plugins/SimpleFileBrowser/Sprites/DriveIcon.png
92c5bed0f6 * Assets/Plugins/SimpleFileBrowser/Sprites/FolderIcon.png
2844300721 * Assets/Plugins/SimpleFileBrowser/Sprites/ImageFileIcon.png
3d31768f4a * Assets/Plugins/SimpleFileBrowser/Sprites/TextFileIcon.png
1092c3473f * Assets/RoadKit/FBX/road.fbx
81df6effcb * Assets/RoadKit/Textures/asphalt_albedo.png
07c1d1639d * Assets/RoadKit/Textures/asphalt_ao.png
07c1d1639d * Assets/RoadKit/Textures/asphalt_normal.png
54ed38f4ed * Assets/RoadKit/Textures/asphalt_road_albedo.png
fc83971af9 * Assets/SampleScenes/Menu/Sprites/LogoUnitySprite.png
3f5cf94824 * Assets/SampleScenes/Menu/Sprites/MenuCornerTopRightSprite.png
897799c4cb * Assets/SampleScenes/Models/Brick.fbx
dd65b23e23 * Assets/SampleScenes/Models/Cube.fbx
9e9402e3a5 * Assets/SampleScenes/Models/GoalPosts.fbx
5bce2a9e74 * Assets/SampleScenes/Models/GroundObstacles.FBX
ed967cf043 * Assets/SampleScenes/Models/GroundRunway.FBX
b3662bf6e8 * Assets/SampleScenes/Models/GroundTrack.fbx
a068e9039e * Assets/SampleScenes/Models/Loop.fbx
3dc02c99aa * Assets/SampleScenes/Models/MiniRamps.fbx
ab5342a1b6 * Assets/SampleScenes/Models/Pickup.fbx
1808bb1493 * Assets/SampleScenes/Models/Pillar.fbx
acac5c7c0c * Assets/SampleScenes/Models/Platform.fbx
d788ba666a * Assets/SampleScenes/Models/Platforms.fbx
c436a65928 * Assets/SampleScenes/Models/PrimaryJumpRamp.fbx
5d50cbb1aa * Assets/SampleScenes/Models/RampElevated.fbx
d1bf4b50b9 * Assets/SampleScenes/Models/Ring.fbx
7718e7cf01 * Assets/SampleScenes/Textures/ChevronAlbedo.png
5cdf2b7f60 * Assets/SampleScenes/Textures/GUIButtonEmpty.png
5a19646d6b * Assets/SampleScenes/Textures/GUIButtonReset.png
69e3549e3a * Assets/SampleScenes/Textures/GUIButtonRun.png
3d9957ef97 * Assets/SampleScenes/Textures/GUIButtonSwitchLeft.png
581e981ab1 * Assets/SampleScenes/Textures/GUIButtonSwitchRight.png
62ac4afef3 * Assets/SampleScenes/Textures/GUICameraCycle.png
1f8dd8c73c * Assets/SampleScenes/Textures/GUICameraCycleDown.png
ef8bbfa77d * Assets/SampleScenes/Textures/GUIReticle.png
134fd296c4 * Assets/SampleScenes/Textures/GUITimescaleFull.png
c1465c863d * Assets/SampleScenes/Textures/GUITimescaleSlow.png
54b49ce8fa * Assets/SampleScenes/Textures/GridEmissive.png
8f2fc6dd26 * Assets/SampleScenes/Textures/SwatchMauveAlbedo.png
337e4f054e * Assets/SampleScenes/Textures/SwatchNavyAlbedo.png
7d270ada61 * Assets/SampleScenes/Textures/SwatchNavyDarkAlbedo.png
72603d3caa * Assets/SampleScenes/Textures/SwatchOrangeAlbedo.png
2f20f1e452 * Assets/SampleScenes/Textures/SwatchPinkAlbedo.png
0163d1210b * Assets/SampleScenes/Textures/SwatchTealAlbedo.png
602fbf9eaa * Assets/SampleScenes/Textures/SwatchTurquoiseAlbedo.png
edb249a51f * Assets/SampleScenes/Textures/SwatchWhiteAlbedo.png
66e38bb508 * Assets/SampleScenes/Textures/SwatchYellowAlbedo.png
731624c0ce * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonAcceleratorOverSprite.png
4faa35dad3 * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonAcceleratorUpSprite.png
cc83c3b905 * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonArrowOverSprite.png
b50e092ba0 * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonArrowUpSprite.png
507b9d17dd * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonBrakeOverSprite.png
c8ede8c5c1 * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonBrakeUpSprite.png
c7c29092ea * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonResetSprite.png
6d387b0c9e * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonSpacebarSprite.png
9c3176293b * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonThumbstickOverSprite.png
05b4d38f3d * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonThumbstickUpSprite.png
d4154a3a3d * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonTimescaleFullUpSprite.png
b4c6f2ed13 * Assets/Standard Assets/CrossPlatformInput/Sprites/ButtonTimescaleSlowUpSprite.png
7635349b22 * Assets/Standard Assets/CrossPlatformInput/Sprites/SliderBackgroundSprite.png
c5de86bff8 * Assets/Standard Assets/CrossPlatformInput/Sprites/SliderHandleSprite.png
f5be8858b9 * Assets/Standard Assets/CrossPlatformInput/Sprites/TouchpadSprite.png
f0527a4e65 * Assets/Standard Assets/Effects/ImageEffects/Textures/ContrastEnhanced3D16.png
d1d787faf5 * Assets/Standard Assets/Effects/ImageEffects/Textures/MotionBlurJitter.png
db2e3b8912 * Assets/Standard Assets/Effects/ImageEffects/Textures/Neutral3D16.png
d1d787faf5 * Assets/Standard Assets/Effects/ImageEffects/Textures/Noise.png
87df06b615 * Assets/Standard Assets/Effects/ImageEffects/Textures/NoiseAndGrain.png
77192f12da * Assets/Standard Assets/Effects/ImageEffects/Textures/NoiseEffectGrain.png
43961f707d * Assets/Standard Assets/Effects/ImageEffects/Textures/NoiseEffectScratch.png
d1d787faf5 * Assets/Standard Assets/Effects/ImageEffects/Textures/RandomVectors.png
490e77ada6 * Assets/Standard Assets/Effects/ImageEffects/Textures/VignetteMask.png
65c6c50153 * Assets/Standard Assets/Effects/ImageEffects/Textures/color correction ramp.png
65c6c50153 * Assets/Standard Assets/Effects/ImageEffects/Textures/grayscale ramp.png
0b36855092 * Assets/Standard Assets/Effects/TessellationShaders/Models/LowPolySphere.fbx
9fdaf42ac2 * Assets/Standard Assets/Effects/TessellationShaders/Textures/CliffHeight.png
3cc9379b66 * Assets/Standard Assets/Effects/TessellationShaders/Textures/CliffNormals.png
121cb5e428 * Assets/Standard Assets/Effects/ToonShading/Textures/UtilToonGradient.png
394cc267d4 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/BroadleafBark.tga
6ae700bc0f * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/BroadleafBark_Normal.tga
ba06cecda3 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Desktop_Atlas.tga
b70469faae * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Desktop_Atlas_Billboards.tga
a3e77f582a * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Desktop_Atlas_Billboards_Normal.tga
fa7233dc15 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Desktop_Atlas_Normal.tga
a2a4c5ed76 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Desktop_Atlas_Specular.tga
e660e420f0 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Mobile_Atlas.tga
52ebced0b8 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Mobile_Atlas_Billboards.tga
740f51a121 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Mobile_Atlas_Billboards_Normal.tga
b300d26b15 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Mobile_Atlas_Normal.tga
00d66d55b8 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaf_Mobile_Atlas_Specular.tga
7345d0fbed * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Card_Desktop.tga
b50db1be5e * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Card_Desktop_Normal.tga
512647379a * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Card_Desktop_Spec.tga
2babb86111 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Desktop.tga
ff1071cd19 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Desktop_Normal.tga
f6827dbe45 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Desktop_Spec.tga
e660e420f0 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Mobile.tga
b300d26b15 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Mobile_Normal.tga
00d66d55b8 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Broadleaves_Mobile_Spec.tga
3014796020 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Cap_01.tga
34d6d1b715 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Cap_01_Normal.tga
37270d1c0e * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Cap_02.tga
4e10df2ce6 * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/Cap_02_Normal.tga
71da934d8e * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/ClippedFrond.tga
b38d64112a * Assets/Standard Assets/Environment/SpeedTree/Broadleaf/ClippedFrond_Normal.tga
e40b471f03 * Assets/Standard Assets/Environment/SpeedTree/Conifer/ConiferBark.tga
a3f706179c * Assets/Standard Assets/Environment/SpeedTree/Conifer/ConiferBark_Normal.tga
84d4992c11 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Desktop_Atlas.tga
bc9e18c9a1 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Desktop_Atlas_Billboards.tga
1cf084ab73 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Desktop_Atlas_Billboards_Normal.tga
71453ee95c * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Desktop_Atlas_Normal.tga
bb37c5d8d7 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Desktop_Atlas_Specular.tga
e11d7076b3 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Needles_Dekstop_Spec.tga
62fe4593c2 * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Needles_Desktop.tga
351e09204b * Assets/Standard Assets/Environment/SpeedTree/Conifer/Conifer_Needles_Desktop_Normal.tga
79cab8f057 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmBark.tga
c930195112 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmBark_Detail.tga
118407ce06 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmBark_Detail_Normal.tga
2bf22515d9 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmBark_Normal.tga
39dd925e60 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmFrond.tga
0094373dd9 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmFrond_Normal.tga
f6353417d6 * Assets/Standard Assets/Environment/SpeedTree/Palm/PalmFrond_Spec.tga
9378354849 * Assets/Standard Assets/Environment/SpeedTree/Palm/Palm_Desktop_Atlas.tga
2d6b484f04 * Assets/Standard Assets/Environment/SpeedTree/Palm/Palm_Desktop_Atlas_Billboards.tga
5afe02f78e * Assets/Standard Assets/Environment/SpeedTree/Palm/Palm_Desktop_Atlas_Billboards_Normal.tga
18697241d3 * Assets/Standard Assets/Environment/SpeedTree/Palm/Palm_Desktop_Atlas_Normal.tga
96ce5a8265 * Assets/Standard Assets/Environment/SpeedTree/Palm/Palm_Desktop_Atlas_Specular.tga
7a4318f488 * Assets/Standard Assets/Environment/Water (Basic)/Models/WaterBasicPlane.fbx
16d1c5966d * Assets/Standard Assets/Environment/Water/Water/Models/WaterPlane.fbx
fcfd29c0b5 * Assets/Standard Assets/Environment/Water/Water4/Models/OceanPlane.FBX
f289aeced9 * Assets/Standard Assets/Environment/Water/Water4/Textures/SimpleFoam.png
af06ba7d1d * Assets/Standard Assets/Environment/Water/Water4/Textures/SmallWaves.png
4e54c1f440 * Assets/Standard Assets/Prototyping/Models/BlockPrototype04x04x04.fbx
d1e12abd42 * Assets/Standard Assets/Prototyping/Models/CubePrototype02x02x02.fbx
84c8b41266 * Assets/Standard Assets/Prototyping/Models/CubePrototype04x04x04.fbx
08b1b1bdab * Assets/Standard Assets/Prototyping/Models/CubePrototype08x08x08.fbx
a732b4a1df * Assets/Standard Assets/Prototyping/Models/FloorPrototype04x01x04.fbx
121b0d610e * Assets/Standard Assets/Prototyping/Models/FloorPrototype08x01x08.fbx
4ca2a524ab * Assets/Standard Assets/Prototyping/Models/FloorPrototype64x01x64.fbx
14e3e10df6 * Assets/Standard Assets/Prototyping/Models/HousePrototype16x16x24.fbx
ffdafd0d72 * Assets/Standard Assets/Prototyping/Models/JoinInnerPrototype01x06x01.fbx
776966a608 * Assets/Standard Assets/Prototyping/Models/JoinMidPrototype04x06x01.fbx
1b316448c5 * Assets/Standard Assets/Prototyping/Models/JoinOuterPrototype02x06x02.fbx
6b6e534a2c * Assets/Standard Assets/Prototyping/Models/PickupPrototype01x01x01.fbx
5255d56a28 * Assets/Standard Assets/Prototyping/Models/PillarPrototype01x02x01.fbx
13e4d5184b * Assets/Standard Assets/Prototyping/Models/PillarPrototype02x08x02.fbx
8d7b486849 * Assets/Standard Assets/Prototyping/Models/PlatformPrototype02x01x02.fbx
44c9b344bf * Assets/Standard Assets/Prototyping/Models/PlatformPrototype04x01x04.fbx
61a4c21173 * Assets/Standard Assets/Prototyping/Models/PlatformPrototype08x01x08.fbx
e80f8c709d * Assets/Standard Assets/Prototyping/Models/RampPrototype04x02x02.fbx
692609dff8 * Assets/Standard Assets/Prototyping/Models/StepsPrototype04x02x02.fbx
f39827a123 * Assets/Standard Assets/Prototyping/Models/WallPrototype08x08x01.fbx
54b49ce8fa * Assets/Standard Assets/Prototyping/Textures/GridEmissive.png
8f2fc6dd26 * Assets/Standard Assets/Prototyping/Textures/SwatchMauveAlbedo.png
337e4f054e * Assets/Standard Assets/Prototyping/Textures/SwatchNavyAlbedo.png
7d270ada61 * Assets/Standard Assets/Prototyping/Textures/SwatchNavyDarkAlbedo.png
72603d3caa * Assets/Standard Assets/Prototyping/Textures/SwatchOrangeAlbedo.png
2f20f1e452 * Assets/Standard Assets/Prototyping/Textures/SwatchPinkDAlbedo.png
0163d1210b * Assets/Standard Assets/Prototyping/Textures/SwatchTealAlbedo.png
602fbf9eaa * Assets/Standard Assets/Prototyping/Textures/SwatchTurquoiseAlbedo.png
edb249a51f * Assets/Standard Assets/Prototyping/Textures/SwatchWhiteAlbedo.png
66e38bb508 * Assets/Standard Assets/Prototyping/Textures/SwatchYellowAlbedo.png
bad5f32367 * Assets/Standard Assets/Vehicles/Car/Models/SkyCar.fbx
14dbb785c1 * Assets/Standard Assets/Vehicles/Car/Textures/ParticleCloudWhite.png
de52d6e60b * Assets/Standard Assets/Vehicles/Car/Textures/SkidTrailAlbedo.png
6e572567e5 * Assets/Standard Assets/Vehicles/Car/Textures/SkyCarBodyNormals.png
6801467dbe * Assets/Standard Assets/Vehicles/Car/Textures/SkyCarBodyOcclusion.png
7b3ed701ff * Assets/Standard Assets/Vehicles/Car/Textures/SkyCarLightsGlowAlpha.png
277c0eacb5 * Assets/Standard Assets/Vehicles/Car/Textures/SkyCarWheelNormals.png
fefcad2d55 * Assets/Standard Assets/Vehicles/Car/Textures/SkyCarWheelOcclusion.png
8bb0bae78d * Assets/race-track-lake/Models/electrical-towers.FBX
c371a3f6a7 * Assets/race-track-lake/Models/race-track.FBX
d5d17aaa57 * Assets/race-track-lake/Models/road-signs.FBX
5dd028e28e * Assets/race-track-lake/Models/rocks-mountain-1.FBX
e7951cc2e7 * Assets/race-track-lake/Models/rocks-mountain-2.FBX
45f32e1301 * Assets/race-track-lake/Models/tire.FBX
c84d691dfd * Assets/race-track-lake/Textures/arrow-green.png
67b3c188e8 * Assets/race-track-lake/Textures/arrow-red.png
387db29f45 * Assets/race-track-lake/Textures/asphalt-dirt.png
d04f72f659 * Assets/race-track-lake/Textures/asphalt-n.png
f65bb65185 * Assets/race-track-lake/Textures/asphalt.png
d089b1ac77 * Assets/race-track-lake/Textures/bridge_wall.png
6eee06ed12 * Assets/race-track-lake/Textures/bridge_wall_n.png
d88bf7135e * Assets/race-track-lake/Textures/concrete-floor-n.png
a3a5e956d7 * Assets/race-track-lake/Textures/concrete-floor.png
a857f4841d * Assets/race-track-lake/Textures/concrete-n.png
68ed52dad0 * Assets/race-track-lake/Textures/concrete.png
b10ba745bf * Assets/race-track-lake/Textures/dirt.png
161af67222 * Assets/race-track-lake/Textures/dirt_n.png
b8ae5b0f76 * Assets/race-track-lake/Textures/grass-n.png
35b37795fe * Assets/race-track-lake/Textures/grass.png
84752301da * Assets/race-track-lake/Textures/mountain-wall-n.png
a6bfc08fb7 * Assets/race-track-lake/Textures/mountain-wall.png
662e6a7b22 * Assets/race-track-lake/Textures/stone_wall.png
544ae16012 * Assets/race-track-lake/Textures/stone_wall_n.png
a6951d8762 * Assets/race-track-lake/Textures/stripes-n.png
f893829c82 * Assets/race-track-lake/Textures/stripes.png
5ac350675b * Assets/race-track-lake/Textures/tire-n.png
a667e9cb2d * Assets/race-track-lake/Textures/tire.png
58017f09ea * sim_image.png
