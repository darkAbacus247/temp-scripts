/// <summary>
/// SECTOR 7 GAME STUDIOS
/// DEVELOPER - darkAbacus247
/// CONTACT - seventhSectorAbacus@gmail.com
/// VERSION - 0.0.0c
/// </summary>

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

using WorldStreamer2;

public class S7GS_devAssist : EditorWindow {

	public class S7GS_Extension_VSP : EditorWindow {
        #region VARIABLES
        public static S7GS_Extension_VSP mainWindow;
        public bool menu_VSP = false;
        public bool menu_WS = false;
        public bool menu_ext = false;

        public float vegetationCellSize;
        public float billboardCellSize;
        #endregion VARIABLES

        #region CREATE MENU
        [MenuItem("S7GS/Advanced/Extensions/VSP Settings")]
        static void Main(){
            //CONFIGURE: Window Settings
            mainWindow = (S7GS_Extension_VSP)GetWindow(typeof(S7GS_Extension_VSP));

            mainWindow.minSize = new Vector2(64f, 64f);                                 // Min Window Size (Clamped to Max)
            mainWindow.maxSize = new Vector2(256f, 1024f);                               // Max Window Size

            mainWindow.titleContent = new GUIContent("S7GS: VSP Settings");
            mainWindow.autoRepaintOnSceneChange = true;

            //INSTANCIATE: Window
            mainWindow.Show();
        }
        #endregion CREATE MENU

        #region POPULATE MENU
        public void OnGUI(){
            #region AwesomeTechnologies / VSP Updates
            #region Toggle VSP Menu
            if (GUILayout.Button("VEGETATION STUDIO PRO SETTINGS")){
                // Toggle VSP Menu
                menu_VSP = !menu_VSP;
            }
            #endregion Toggle VSP Menu

            if(menu_VSP){
                #region VEGETATION DENSITIES
                if (GUILayout.Button("* UPDATE - Vegetation Densities")){
                    // Update Vegetation Settings
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.VegetationSettings.GrassDensity = 1;
                        target.VegetationSettings.PlantDensity = 1;
                        target.VegetationSettings.TreeDensity = 1;
                        target.VegetationSettings.ObjectDensity = 1;
                        target.VegetationSettings.LargeObjectDensity = 1;
                    }
                }
                #endregion VEGETATION DENSITIES

                #region VEGETATION DISTANCES
                if (GUILayout.Button("VSP - Set Vegetation Distance")){
                    // Set Vegetation Distances
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.VegetationSettings.PlantDistance = 72;
                        target.VegetationSettings.AdditionalTreeMeshDistance = 80;
                        target.VegetationSettings.AdditionalBillboardDistance = 8192;
                    }
                }
                #endregion VEGETATION DISTANCES

                #region VEGETATION CELL SIZE
                // VEGETATION CELL SIZE
                GUILayout.Label("Vegetation Cell Size = " + vegetationCellSize);
                vegetationCellSize = 152;  //GUILayout.HorizontalScrollbar(vegetationCellSize, 250, 32, 250);
                
                GUILayout.Label("Billboard Cell Size = " + billboardCellSize);
                billboardCellSize = 608; //GUILayout.HorizontalScrollbar(billboardCellSize, 8000, 500, 8000);

                if (GUILayout.Button("UPDATE - Cell Size")){
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.VegetationCellSize = vegetationCellSize;
                        target.BillboardCellSize = billboardCellSize;

                        target.RefreshVegetationSystem();
                    }
                }
                #endregion VEGETATION CELL SIZE

                #region EXCLUDE SEA LEVEL CELLS
                // ADD Terrains to VSP packs
                if (GUILayout.Button("* UPDATE - Exclude Sea Level Cells")){
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.ExcludeSeaLevelCells = true;
                    }
                }
                #endregion EXCLUDE SEA LEVEL CELLS

                #region SUBDIVIDE VEGETATION SYSTEM
                // Subdivide Vegetation System
                if (GUILayout.Button("Subdivide Vegetation System"))
                {
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>())
                    {
                        var queue = new Queue<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>();
                        queue.Enqueue(target);

                        while (queue.Count > 0)
                        {
                            var currentTarget = queue.Dequeue();
                            var extents = currentTarget.VegetationSystemBounds.extents;

                            if (extents.x > 768 || extents.z > 768)
                            {
                                var offset = extents.x / 2;
                                AwesomeTechnologies.VegetationSystem.VegetationSystemPro[] vegSysPro = new AwesomeTechnologies.VegetationSystem.VegetationSystemPro[4];

                                for (int child = 0; child < 4; child++)
                                {
                                    var _vegSysPro = Instantiate(currentTarget);
                                    var _vspBoundsCenter = _vegSysPro.VegetationSystemBounds.center;
                                    vegSysPro[child] = _vegSysPro;

                                    switch (child)
                                    {
                                        case 0:
                                            {
                                                _vegSysPro.VegetationSystemBounds.center = new Vector3(_vspBoundsCenter.x - (extents.x / 2), 512, _vspBoundsCenter.z + (extents.z / 2));
                                                break;
                                            }

                                        case 1:
                                            {
                                                _vegSysPro.VegetationSystemBounds.center = new Vector3(_vspBoundsCenter.x + (extents.x / 2), 512, _vspBoundsCenter.z + (extents.z / 2));
                                                break;
                                            }

                                        case 2:
                                            {
                                                _vegSysPro.VegetationSystemBounds.center = new Vector3(_vspBoundsCenter.x - (extents.x / 2), 512, _vspBoundsCenter.z - (extents.z / 2));
                                                break;
                                            }

                                        case 3:
                                            {
                                                _vegSysPro.VegetationSystemBounds.center = new Vector3(_vspBoundsCenter.x + (extents.x / 2), 512, _vspBoundsCenter.z - (extents.z / 2));
                                                break;
                                            }
                                    }

                                    _vegSysPro.transform.position = _vegSysPro.VegetationSystemBounds.center;
                                    _vegSysPro.VegetationSystemBounds.extents = new Vector3(offset, 512, offset);
                                    queue.Enqueue(_vegSysPro);
                                }

                                // Remove Old Object
                                DestroyImmediate(currentTarget.gameObject);
                            }
                        }
                    }
                }
                #endregion SUBDIVIDE VEGETATION SYSTEM


                #region CREATE VEGETATION STORAGE
                if (GUILayout.Button("Create Vegetation Storage")){
                    // CREATE VEGETATION STORAGE
                    foreach (AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage target in GameObject.FindObjectsOfType<AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage>()){
                        AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStoragePackage newPackage = CreateInstance<AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStoragePackage>();

                        if (!AssetDatabase.IsValidFolder("Assets/PersistentVegetationStorageData")){
                            AssetDatabase.CreateFolder("Assets", "PersistentVegetationStorageData");
                        }

                        string filename = "VSP_Storage_" + Guid.NewGuid() + ".asset";
                        AssetDatabase.CreateAsset(newPackage, "Assets/PersistentVegetationStorageData/" + filename);

                        AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStoragePackage loadedPackage = AssetDatabase.LoadAssetAtPath<AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStoragePackage>("Assets/PersistentVegetationStorageData/" + filename);
                        target.PersistentVegetationStoragePackage = loadedPackage;
                        target.InitializePersistentStorage();
                    }
                }
                #endregion CREATE VEGETATION STORAGE

                #region INITIALIZE VEGETATION STORAGE
                if (GUILayout.Button("Initialize Vegetation Storage")){
                    // INITIALIZE VEGETATION STORAGE
                    foreach (AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage target in GameObject.FindObjectsOfType<AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage>()){
                        if (target.PersistentVegetationStoragePackage != null){
                            target.PersistentVegetationStoragePackage.ClearPersistentVegetationCells();

                            for (int i = 0; i <= target.GetComponent<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>().VegetationCellList.Count - 1; i++){
                                target.PersistentVegetationStoragePackage.AddVegetationCell();
                            }
                        }
                    }
                }
                #endregion INITIALIZE VEGETATION STORAGE

                #region REMOVE TERRAINS 
                // REMOVE Terrains to VSP packs
                if (GUILayout.Button("* UPDATE - Remove Terrains")){
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.RemoveAllTerrains();
                    }
                }
                #endregion REMOVE TERRAINS 

                #region ADD TERRAINS 
                // ADD Terrains to VSP packs
                if (GUILayout.Button("* UPDATE - Add Terrains")){
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.AddAllUnityTerrains();
                    }
                }
                #endregion ADD TERRAINS 

                #region BAKE VEGETATION
                // BAKE VEGETATION
                if (GUILayout.Button("BAKE VEGETATION")){
                    foreach (AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage target in GameObject.FindObjectsOfType<AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage>()){

                        for (int i = 0; i <= target.VegetationSystemPro.VegetationPackageProList.Count - 1; i++){
                            AwesomeTechnologies.VegetationSystem.VegetationPackagePro vegetationPackagePro =
                                target.VegetationSystemPro.VegetationPackageProList[i];

                            for (int j = 0; j <= vegetationPackagePro.VegetationInfoList.Count - 1; j++){
                                AwesomeTechnologies.VegetationSystem.VegetationItemInfoPro vegetationItemInfoPro = vegetationPackagePro.VegetationInfoList[j];
                                target.RemoveVegetationItemInstances(vegetationItemInfoPro.VegetationItemID, 0);

                                switch (vegetationItemInfoPro.VegetationItemID) {
                                    case "57b7a002-d0b8-447b-b647-5eb4622e8d99":
                                    case "512f21dc-fb39-45e5-aeb3-ad504e3c3ffd":
                                    case "bada00a8-211a-4841-8bff-8ba7b627555d":{
                                        // Do nothing, Grass Biomes not to be baked.
                                    }
                                    break;

                                    default: {
                                        target.BakeVegetationItem(vegetationItemInfoPro.VegetationItemID);
                                    }
                                    break;
                                }
                            }
                            EditorUtility.SetDirty(vegetationPackagePro);
                        }
                        EditorUtility.SetDirty(target.PersistentVegetationStoragePackage);
                        target.VegetationSystemPro.ClearCache();
                    }
                }
                #endregion BAKE VEGETATION

                #region GENERATE BIOME SPLATEMAPS
                // Generate Biome Splatmaps
                if (GUILayout.Button("* UPDATE - Generate Biome Splatmaps")){
                    foreach (AwesomeTechnologies.VegetationSystem.VegetationSystemPro target in GameObject.FindObjectsOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>()){
                        target.GetComponent<AwesomeTechnologies.TerrainSystem.TerrainSystemPro>().GenerateSplatMap(false);
                    }
                }
                #endregion GENERATE BIOME SPLATEMAPS
            }
            #endregion AwesomeTechnologies / VSP Updates

            #region NatureManufacture / WS Updates
            #region Toggle WS Menu
            if (GUILayout.Button("WORLD STREAMER SETTINGS")){
                // Toggle WS Menu
                menu_WS = !menu_WS;
            }
            #endregion Toggle WS Menu

            if(menu_WS){
                #region RESET RANGES
                if (GUILayout.Button("Reset Ranges")){
                    // Update Streamers
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        Debug.Log(sceneCollectionManager);

                        sceneCollectionManager.loadingRange = new Vector3Int(1, 1, 1);
                        //sceneCollectionManager.loadingRangeMin = new Vector3Int(0, 0, 0);

                        sceneCollectionManager.deloadingRange = new Vector3Int(1, 1, 1);
                    }
                }
                #endregion RESET RANGES

                #region Collapse All
                if (GUILayout.Button("Collapse All")){
                    // Collapse All
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        sceneCollectionManager.collapsed = true;
                    }
                }
                #endregion Collapse All

                #region Expand All
                if (GUILayout.Button("Expand All")){
                    // Expand All
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        sceneCollectionManager.collapsed = false;
                    }
                }
                #endregion Expand All

                #region Activate All
                if (GUILayout.Button("Activate All")){
                    // Activate All
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        sceneCollectionManager.active = true;
                    }
                }
                #endregion Activate All

                #region Deactivate All
                if (GUILayout.Button("Deactivate All")){
                    // Deactivate All
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        sceneCollectionManager.active = false;
                    }
                }
                #endregion Deactivate All

                #region Reset Priority
                if (GUILayout.Button("Reset Priority")){
                    // Reset Priority
                    foreach (var sceneCollectionManager in FindObjectOfType<Streamer>().sceneCollectionManagers){
                        sceneCollectionManager.priority = 1;
                    }
                }
            #endregion Reset Priority

                #region Debug Stuff
                if (GUILayout.Button("*** Debug Stuff ***")){
                    // Debug Stuff
                        
                }
                #endregion Debug Stuff
            }
            #endregion NatureManufacture / WS Updates
        }
        #endregion POPULATE MENU

        #region FUNCTIONS
        #endregion FUNCTIONS
    }
}