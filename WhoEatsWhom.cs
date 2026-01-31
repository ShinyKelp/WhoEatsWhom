using BepInEx;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using Watcher;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace WhoEatsWhom
{

    [BepInPlugin("ShinyKelp.WhoEatsWhom", "Who Eats Whom", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        }

        private bool IsInit;
        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;
                On.StaticWorld.InitStaticWorld += CreateCustomRelationships;
                On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                IsInit = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void CreateCustomRelationships(On.StaticWorld.orig_InitStaticWorld orig)
        {
            orig();
            SetRelationshipsFromDirectory();
        }

        Dictionary<string, int> creatureIndexes;
        Dictionary<string, CreatureTemplate.Relationship.Type> relationshipTypes;

        public void SetRelationshipsFromDirectory()
        {
            if (!Directory.Exists(Custom.RootFolderDirectory() + "/WhoEatsWhom"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/WhoEatsWhom");

            if (!Directory.Exists(Custom.RootFolderDirectory() + "/WhoEatsWhom/Relationships"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/WhoEatsWhom/Relationships");
            if(creatureIndexes == null)
                creatureIndexes = new Dictionary<string, int>();
            creatureIndexes.Clear();
            foreach(CreatureTemplate ct in StaticWorld.creatureTemplates)
                creatureIndexes.Add(ct.type.ToString(), ct.type.index);


            if (relationshipTypes == null)
                relationshipTypes = new Dictionary<string, CreatureTemplate.Relationship.Type>();
            relationshipTypes.Clear();
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Afraid.ToString(), CreatureTemplate.Relationship.Type.Afraid);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Uncomfortable.ToString(), CreatureTemplate.Relationship.Type.Uncomfortable);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Eats.ToString(), CreatureTemplate.Relationship.Type.Eats);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Attacks.ToString(), CreatureTemplate.Relationship.Type.Attacks);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Antagonizes.ToString(), CreatureTemplate.Relationship.Type.Antagonizes);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Ignores.ToString(), CreatureTemplate.Relationship.Type.Ignores);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.DoesntTrack.ToString(), CreatureTemplate.Relationship.Type.DoesntTrack);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.AgressiveRival.ToString(), CreatureTemplate.Relationship.Type.AgressiveRival);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.Pack.ToString(), CreatureTemplate.Relationship.Type.Pack);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.PlaysWith.ToString(), CreatureTemplate.Relationship.Type.PlaysWith);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.SocialDependent.ToString(), CreatureTemplate.Relationship.Type.SocialDependent);
            relationshipTypes.Add(CreatureTemplate.Relationship.Type.StayOutOfWay.ToString(), CreatureTemplate.Relationship.Type.StayOutOfWay);

            Debug.Log("Set up dictionaries.");

            //Create readme
            string readmeContents = "Welcome to the relationship customizer! Here you can find a list of all valid creatures and valid relationships\n" +
                "NOTE: This file is overwritten every time the game loads. Do not touch this file!\n\n" +
                "FORMAT REMINDER:\n" +
                "Creature_A : Creature_B : Relationship_Type : Relationship_Intensity\n\n" +
                " === RELATIONSHIP TYPES ===\n";

            foreach(var relationshipType in relationshipTypes)
                readmeContents += relationshipType.Value + "\n";

            readmeContents += "\n === CREATURE NAMES ===\n";
            foreach (string critName in creatureIndexes.Keys)
                readmeContents += critName + "\n";

            string directoryPath = Custom.RootFolderDirectory() + "/WhoEatsWhom/Relationships";

            File.WriteAllText(
                Custom.RootFolderDirectory() + "/WhoEatsWhom/Readme.txt",
                readmeContents,
                System.Text.Encoding.UTF8
            );

            foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.txt"))
            {
                Debug.Log("\nSETTING RELATIONSHIPS FROM: " + Path.GetFileName(filePath));
                SetRelationshipsFromFile(filePath);
            }
        }

        private void SetRelationshipsFromFile(string path)
        {
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                    continue;

                int commentIndex = line.IndexOf("//");
                string withoutComments = commentIndex >= 0
                    ? line.Substring(0, commentIndex)
                    : line;

                string trimmedLine = withoutComments.Trim();

                string[] splitLine = trimmedLine.Split(':');

                if (splitLine.Length != 4)
                {
                    Debug.Log($"(INVALID) {line} <-- Wrong format.");
                    continue;
                }

                string creatureA = splitLine[0].Trim();
                string creatureB = splitLine[1].Trim();
                string type = splitLine[2].Trim();

                string errorLine = "";
                bool error = false;

                if (!float.TryParse(
                        splitLine[3].Trim(),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out float intensity))
                {
                    errorLine += $"Unreadable intensity value:  {splitLine[3]}. ";
                    error = true;
                }
                else if(intensity < 0f)
                {
                    error = true;
                    errorLine += $"Negative intensity value: {splitLine[3]}. ";
                }

                if (!creatureIndexes.ContainsKey(creatureA))
                {
                    error = true;
                    errorLine += $"Creature name not found: {creatureA}. ";
                }
                if (!creatureIndexes.ContainsKey(creatureB))
                {
                    error = true;
                    errorLine += $"Creature name not found: {creatureB}. ";
                }
                if (!relationshipTypes.ContainsKey(type))
                {
                    error = true;
                    errorLine += $"Relationship type not found: {type}. ";
                }
                if (!error)
                {
                    StaticWorld.creatureTemplates[creatureIndexes[creatureA]].relationships[creatureIndexes[creatureB]].type = relationshipTypes[type];
                    StaticWorld.creatureTemplates[creatureIndexes[creatureA]].relationships[creatureIndexes[creatureB]].intensity = intensity;
                    Debug.Log(withoutComments);
                }
                else
                {
                    Debug.Log($"(INVALID) {line} <-- {errorLine}");
                }
            }
        }


        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }


        private void ClearMemory()
        {
            if(creatureIndexes != null)
                creatureIndexes.Clear();
            if(relationshipTypes != null) 
                relationshipTypes.Clear();
        }

    }
}
