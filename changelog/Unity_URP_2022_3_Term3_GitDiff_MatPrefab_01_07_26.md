
# Unity URP 2022_3_62f3 Term 3 Git Diff Material/Prefab

## 01/07/26

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7772/7772), done.
On branch Unity_URP_2022_3_Term3

## Changes not staged for commit:

### modified:   Assets/Materials/pathElem.mat

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/Materials/pathElem.mat
diff --git a/Assets/Materials/pathElem.mat b/Assets/Materials/pathElem.mat
index 97fc46f..69a6c19 100644
--- a/Assets/Materials/pathElem.mat
+++ b/Assets/Materials/pathElem.mat
@@ -1,5 +1,18 @@
 %YAML 1.1
 %TAG !u! tag:unity3d.com,2011:
+--- !u!114 &-2990408095086758875
+MonoBehaviour:
+  m_ObjectHideFlags: 11
+  m_CorrespondingSourceObject: {fileID: 0}
+  m_PrefabInstance: {fileID: 0}
+  m_PrefabAsset: {fileID: 0}
+  m_GameObject: {fileID: 0}
+  m_Enabled: 1
+  m_EditorHideFlags: 0
+  m_Script: {fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}
+  m_Name:
+  m_EditorClassIdentifier:
+  version: 7
 --- !u!21 &2100000
 Material:
   serializedVersion: 8
@@ -8,7 +21,7 @@ Material:
   m_PrefabInstance: {fileID: 0}
   m_PrefabAsset: {fileID: 0}
   m_Name: pathElem
-  m_Shader: {fileID: 46, guid: 0000000000000000f000000000000000, type: 0}
+  m_Shader: {fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}
   m_Parent: {fileID: 0}
   m_ModifiedSerializedProperties: 0
   m_ValidKeywords:
@@ -18,12 +31,17 @@ Material:
   m_EnableInstancingVariants: 0
   m_DoubleSidedGI: 0
   m_CustomRenderQueue: -1
-  stringTagMap: {}
+  stringTagMap:
+    RenderType: Opaque
   disabledShaderPasses: []
   m_LockedProperties:
   m_SavedProperties:
     serializedVersion: 3
     m_TexEnvs:
+    - _BaseMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
     - _BumpMap:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
@@ -60,12 +78,38 @@ Material:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
+    - _SpecGlossMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - unity_Lightmaps:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - unity_LightmapsInd:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - unity_ShadowMasks:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
     m_Ints: []
     m_Floats:
+    - _AlphaClip: 0
+    - _AlphaToMask: 0
+    - _Blend: 0
+    - _BlendModePreserveSpecular: 1
     - _BumpScale: 1
+    - _ClearCoatMask: 0
+    - _ClearCoatSmoothness: 0
+    - _Cull: 2
     - _Cutoff: 0.5
+    - _DetailAlbedoMapScale: 1
     - _DetailNormalMapScale: 1
     - _DstBlend: 0
+    - _DstBlendAlpha: 0
+    - _EnvironmentReflections: 1
     - _GlossMapScale: 1
     - _Glossiness: 0.5
     - _GlossyReflections: 1
@@ -73,12 +117,20 @@ Material:
     - _Mode: 0
     - _OcclusionStrength: 1
     - _Parallax: 0.02
+    - _QueueOffset: 0
+    - _ReceiveShadows: 1
+    - _Smoothness: 0.5
     - _SmoothnessTextureChannel: 0
     - _SpecularHighlights: 1
     - _SrcBlend: 1
+    - _SrcBlendAlpha: 1
+    - _Surface: 0
     - _UVSec: 0
+    - _WorkflowMode: 1
     - _ZWrite: 1
     m_Colors:
-    - _Color: {r: 0.102941155, g: 1, b: 0.22048691, a: 1}
+    - _BaseColor: {r: 1, g: 1, b: 1, a: 1}
+    - _Color: {r: 1, g: 1, b: 1, a: 1}
     - _EmissionColor: {r: 0, g: 0, b: 0, a: 1}
+    - _SpecColor: {r: 0.19999996, g: 0.19999996, b: 0.19999996, a: 1}
   m_BuildTextureStacks: []

### modified:   Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat
diff --git a/Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat b/Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat
index 4c2e30b..7f38c99 100644
--- a/Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat
+++ b/Assets/RoadMarkers/Materials/Markers/GSDWhiteYellowDouble-6L.mat
@@ -2,186 +2,206 @@
 %TAG !u! tag:unity3d.com,2011:
 --- !u!21 &2100000
 Material:
-  serializedVersion: 6
+  serializedVersion: 8
   m_ObjectHideFlags: 0
-  m_PrefabParentObject: {fileID: 0}
-  m_PrefabInternal: {fileID: 0}
+  m_CorrespondingSourceObject: {fileID: 0}
+  m_PrefabInstance: {fileID: 0}
+  m_PrefabAsset: {fileID: 0}
   m_Name: GSDWhiteYellowDouble-6L
-  m_Shader: {fileID: 4800000, guid: e94f0680ef6d2404dbc59e2ed5d88129, type: 3}
-  m_ShaderKeywords:
-  m_LightmapFlags: 5
+  m_Shader: {fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}
+  m_Parent: {fileID: 0}
+  m_ModifiedSerializedProperties: 0
+  m_ValidKeywords:
+  - _EMISSION
+  m_InvalidKeywords: []
+  m_LightmapFlags: 1
+  m_EnableInstancingVariants: 0
+  m_DoubleSidedGI: 0
   m_CustomRenderQueue: -1
-  stringTagMap: {}
+  stringTagMap:
+    RenderType: Opaque
+  disabledShaderPasses: []
+  m_LockedProperties:
   m_SavedProperties:
-    serializedVersion: 2
+    serializedVersion: 3
     m_TexEnvs:
-    - first:
-        name: _BackTex
-      second:
+    - _BackTex:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _BumpMap
-      second:
+    - _BaseMap:
+        m_Texture: {fileID: 2800000, guid: c1e7b792017215444a2f567213bdd486, type: 3}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _BumpMap:
         m_Texture: {fileID: 2800000, guid: de5bb1e0fee7e6a428dd7f8cfda0ecd1, type: 3}
         m_Scale: {x: 4, y: 4}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _Cube
-      second:
+    - _Cube:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _Decal
-      second:
+    - _Decal:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _Detail
-      second:
+    - _Detail:
         m_Texture: {fileID: 2800000, guid: 74abfab6bf577c64d863cca0004d26eb, type: 3}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _DownTex
-      second:
+    - _DetailAlbedoMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _DetailMask:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _DetailNormalMap:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _FrontTex
-      second:
+    - _DownTex:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _GlossMap
-      second:
+    - _EmissionMap:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _LeftTex
-      second:
+    - _FrontTex:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _MainTex
-      second:
+    - _GlossMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _LeftTex:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _MainTex:
         m_Texture: {fileID: 2800000, guid: c1e7b792017215444a2f567213bdd486, type: 3}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _ParallaxMap
-      second:
+    - _MetallicGlossMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _OcclusionMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _ParallaxMap:
         m_Texture: {fileID: 2800000, guid: 74abfab6bf577c64d863cca0004d26eb, type: 3}
         m_Scale: {x: 4, y: 4}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _ReflMask
-      second:
+    - _ReflMask:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _RightTex:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _SparkleTex:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _SpecGlossMap:
+        m_Texture: {fileID: 0}
+        m_Scale: {x: 1, y: 1}
+        m_Offset: {x: 0, y: 0}
+    - _ToonShade:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _RightTex
-      second:
+    - _UpTex:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _SparkleTex
-      second:
+    - unity_Lightmaps:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _ToonShade
-      second:
+    - unity_LightmapsInd:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
-    - first:
-        name: _UpTex
-      second:
+    - unity_ShadowMasks:
         m_Texture: {fileID: 0}
         m_Scale: {x: 1, y: 1}
         m_Offset: {x: 0, y: 0}
+    m_Ints: []
     m_Floats:
-    - first:
-        name: _DecalColoring
-      second: 0.5
-    - first:
-        name: _EdgeLength
-      second: 10
-    - first:
-        name: _FrezFalloff
-      second: 4
-    - first:
-        name: _FrezPow
-      second: 0
-    - first:
-        name: _Gloss
-      second: 0
-    - first:
-        name: _LineWidth
-      second: 0.1
-    - first:
-        name: _Outline
-      second: 0.005
-    - first:
-        name: _Parallax
-      second: 0.01619403
-    - first:
-        name: _Reflection
-      second: 0
-    - first:
-        name: _ShadowIntensity
-      second: 0.6
-    - first:
-        name: _Shininess
-      second: 0.03020274
-    - first:
-        name: _Sparkle
-      second: 0.01
-    - first:
-        name: _SquashAmount
-      second: 1
+    - _AlphaClip: 0
+    - _AlphaToMask: 0
+    - _Blend: 0
+    - _BlendModePreserveSpecular: 1
+    - _BumpScale: 1
+    - _ClearCoatMask: 0
+    - _ClearCoatSmoothness: 0
+    - _Cull: 2
+    - _Cutoff: 0.5
+    - _DecalColoring: 0.5
+    - _DetailAlbedoMapScale: 1
+    - _DetailNormalMapScale: 1
+    - _DstBlend: 0
+    - _DstBlendAlpha: 0
+    - _EdgeLength: 10
+    - _EnvironmentReflections: 1
+    - _FrezFalloff: 4
+    - _FrezPow: 0
+    - _Gloss: 0
+    - _GlossMapScale: 0
+    - _Glossiness: 0
+    - _GlossyReflections: 0
+    - _LineWidth: 0.1
+    - _Metallic: 0
+    - _OcclusionStrength: 1
+    - _Outline: 0.005
+    - _Parallax: 0.01619403
+    - _QueueOffset: 0
+    - _ReceiveShadows: 1
+    - _Reflection: 0
+    - _ShadowIntensity: 0.6
+    - _Shininess: 0.03020274
+    - _Smoothness: 0.5
+    - _SmoothnessTextureChannel: 0
+    - _Sparkle: 0.01
+    - _SpecularHighlights: 1
+    - _SquashAmount: 1
+    - _SrcBlend: 1
+    - _SrcBlendAlpha: 1
+    - _Surface: 0
+    - _WorkflowMode: 1
+    - _ZWrite: 1
     m_Colors:
-    - first:
-        name: _Color
-      second: {r: 1, g: 1, b: 1, a: 1}
-    - first:
-        name: _Emission
-      second: {r: 0, g: 0, b: 0, a: 0}
-    - first:
-        name: _GridColor
-      second: {r: 0, g: 0, b: 0, a: 0}
-    - first:
-        name: _HighlightColor
-      second: {r: 1, g: 1, b: 1, a: 1}
-    - first:
-        name: _LineColor
-      second: {r: 1, g: 1, b: 1, a: 1}
-    - first:
-        name: _OutlineColor
-      second: {r: 0, g: 0, b: 0, a: 1}
-    - first:
-        name: _ReflectColor
-      second: {r: 1, g: 1, b: 1, a: 0.5}
-    - first:
-        name: _Scale
-      second: {r: 1, g: 1, b: 1, a: 1}
-    - first:
-        name: _SpecColor
-      second: {r: 0.5, g: 0.5, b: 0.5, a: 1}
-    - first:
-        name: _SpecularColor
-      second: {r: 0.5, g: 0.5, b: 0.5, a: 1}
-    - first:
-        name: _Tint
-      second: {r: 0.5, g: 0.5, b: 0.5, a: 0.5}
+    - _BaseColor: {r: 1, g: 1, b: 1, a: 1}
+    - _Color: {r: 1, g: 1, b: 1, a: 1}
+    - _Emission: {r: 0, g: 0, b: 0, a: 0}
+    - _EmissionColor: {r: 0, g: 0, b: 0, a: 1}
+    - _GridColor: {r: 0, g: 0, b: 0, a: 0}
+    - _HighlightColor: {r: 1, g: 1, b: 1, a: 1}
+    - _LineColor: {r: 1, g: 1, b: 1, a: 1}
+    - _OutlineColor: {r: 0, g: 0, b: 0, a: 1}
+    - _ReflectColor: {r: 1, g: 1, b: 1, a: 0.5}
+    - _Scale: {r: 1, g: 1, b: 1, a: 1}
+    - _SpecColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
+    - _SpecularColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
+    - _Tint: {r: 0.5, g: 0.5, b: 0.5, a: 0.5}
+  m_BuildTextureStacks: []
+--- !u!114 &5273167997837081231
+MonoBehaviour:
+  m_ObjectHideFlags: 11
+  m_CorrespondingSourceObject: {fileID: 0}
+  m_PrefabInstance: {fileID: 0}
+  m_PrefabAsset: {fileID: 0}
+  m_GameObject: {fileID: 0}
+  m_Enabled: 1
+  m_EditorHideFlags: 0
+  m_Script: {fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}
+  m_Name:
+  m_EditorClassIdentifier:
+  version: 7

## Untracked files:

### Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab
### Assets/1_SelfDrivingCar/Prefabs/Car 1.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/Car copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/CarUI copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/CarWaypointBased copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/CommandServer copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/SimulatorUI copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/SkidTrail copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/SocketIO copy.prefab.meta
### Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab
### Assets/1_SelfDrivingCar/Prefabs/ThreadDispatcher copy.prefab.meta
### Assets/New Terrain 2.asset
### Assets/New Terrain 2.asset.meta
### Assets/_TerrainAutoUpgrade.meta
### Assets/_TerrainAutoUpgrade/
### UpgradeLog.htm


