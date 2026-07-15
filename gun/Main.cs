using BlueprintCore.Utils;
using gun.Classes.Gunslinger;
using gun.Firearms;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.JsonSystem.Converters;
using Kingmaker.Modding;
using Kingmaker.SharedTypes;
using Kingmaker.Utility;
using Newtonsoft.Json;
using Owlcat.Runtime.Core.Utils;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager.Param;

namespace gun;

public static class Main {
    internal static Harmony HarmonyInstance;
    internal static UnityModManager.ModEntry.ModLogger Log;
    public static string ModPath;
    public static readonly HashSet<string> Bundles = new HashSet<string>();

    public static IEnumerable<string> GetFilesFromDirectory(string directory)
    {
        return Directory.GetFiles(System.IO.Path.Combine(ModPath, directory), "*", SearchOption.AllDirectories);
    }
    public static bool Load(UnityModManager.ModEntry modEntry) {
        Log = modEntry.Logger;
        //LogWrapper.EnableInternalVerboseLogs();
        ModPath = modEntry.Path;
        HarmonyInstance = new Harmony(modEntry.Info.Id);
        try {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        } catch {
            HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
            throw;
        }
        return true;
    }

    [HarmonyPatch(typeof(BlueprintsCache))]
    public static class BlueprintsCaches_Patch {
        private static bool Initialized = false;
        
        [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        [HarmonyAfter("DragonLibrary")]
        public static void Init_Postfix() {
            try {
                if (Initialized) {
                    Log.Log("Already initialized blueprints cache.");
                    return;
                }
                Initialized = true;

                Log.Log("Patching blueprints.");
                // Insert your mod's patching methods here
                OwlcatModificationsManager OwlcatModManager = OwlcatModificationsManager.Instance;

                List<OwlcatModification> list = [.. OwlcatModManager.m_Modifications];
                //Log.Log(System.IO.Path.Combine(ModPath, "Bundles\\"));
                list.AddRange(OwlcatModificationsManager.LoadModifications(Path.Combine(ModPath, "Bundles\\")));
                OwlcatModManager.m_Modifications = list.ToArray();
                
                string[] enabledModifications = [ "GunAssets" ];
                foreach (string modificationName in enabledModifications)
                {

                    OwlcatModification owlcatModification = OwlcatModManager.m_Modifications.FirstItem((OwlcatModification d) => d.Manifest?.UniqueName == modificationName);
                    if (owlcatModification == null)
                    {
                        PFLog.Mods.Error("Missing modification: " + modificationName);
                        continue;
                    }
                    Log.Log("Found mod");
                    string path = owlcatModification.Path;
                    Log.Log("At path:" + path);
                    OwlcatModificationManifest manifest = owlcatModification.Manifest;
                    Log.Log("got manifest");
                    if (manifest == null)
                    {
                        PFLog.Mods.Error("Modification can't be loaded: " + modificationName + " (" + path + ")");
                    }
                    else 
                    {
                        Log.Log("getting ready to apply");
                        PFLog.Mods.Log("Apply modification: " + manifest.UniqueName + " (" + path + ")");
                        
                        owlcatModification.Apply();
                        Log.Log("applied");
                        owlcatModification.Reload();
                        Log.Log("reloading");
                        
                        if (!list.Contains(owlcatModification))
                            list.Add(owlcatModification);
                    }
                }
                OwlcatModManager.AppliedModifications = list.ToArray();

                BaseFirearm.Configure();
                Gunslinger.Configure();
                Musket.Configure();
                Pistol.Configure();
                Rifle.Configure();
                Revolver.Configure();

            } catch (Exception e) {
                Log.Log(string.Concat("Failed to initialize.", e));
            }
        }
    }

}
