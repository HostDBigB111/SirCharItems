using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using System.Collections.Generic;

namespace SirCharItems
{
    public class LaserPistol : GunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Laser Pistol", "laserpistol");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:laser_pistol", "host:laser_pistol");
            gun.gameObject.AddComponent<LaserPistol>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Low-Tech");
            gun.SetLongDescription("An Old Laser Pistol. To some this might be considered an otherworldly weapon, but in reality this is very outdated technology.");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "laserpistol_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("ak-47", true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.1f;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(250);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.encounterTrackable.EncounterGuid = "laser pistol go pew pew, please kill me thank you :D";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 6f;
            projectile.baseData.speed = 24f;
            projectile.transform.parent = gun.barrelOffset;
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            projectile.shouldRotate = true;
            projectile.SetProjectileSpriteRight("laserpistol_projectile_001", 20, 8, null, null);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            Gun gunReference = (PickupObjectDatabase.GetById(32) as Gun);
            gun.muzzleFlashEffects = gunReference.muzzleFlashEffects;
            gun.barrelOffset.transform.localPosition = new Vector3(1.365f, 0.715f, 0);

        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_raidenlaser_shot_01", gameObject);
        }
        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        protected void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }
        //Now add the Tools class to your project.
        //All that's left now is sprite stuff. 
        //Your sprites should be organized, like how you see in the mod folder. 
        //Every gun requires that you have a .json to match the sprites or else the gun won't spawn at all
        //.Json determines the hand sprites for your character. You can make a gun two handed by having both "SecondaryHand" and "PrimaryHand" in the .json file, which can be edited through Notepad or Visual Studios
        //By default this gun is a one-handed weapon
        //If you need a basic two handed .json. Just use the jpxfrd2.json.
        //And finally, don't forget to add your Gun to your ETGModule class!
    }
    public static class Tools
    {
        public static tk2dSpriteDefinition CopyDefinitionFrom(this tk2dSpriteDefinition other)
        {
            tk2dSpriteDefinition result = new tk2dSpriteDefinition
            {
                boundsDataCenter = new Vector3
                {
                    x = other.boundsDataCenter.x,
                    y = other.boundsDataCenter.y,
                    z = other.boundsDataCenter.z
                },
                boundsDataExtents = new Vector3
                {
                    x = other.boundsDataExtents.x,
                    y = other.boundsDataExtents.y,
                    z = other.boundsDataExtents.z
                },
                colliderConvex = other.colliderConvex,
                colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions,
                colliderType = other.colliderType,
                colliderVertices = other.colliderVertices,
                collisionLayer = other.collisionLayer,
                complexGeometry = other.complexGeometry,
                extractRegion = other.extractRegion,
                flipped = other.flipped,
                indices = other.indices,
                material = new Material(other.material),
                materialId = other.materialId,
                materialInst = new Material(other.materialInst),
                metadata = other.metadata,
                name = other.name,
                normals = other.normals,
                physicsEngine = other.physicsEngine,
                position0 = new Vector3
                {
                    x = other.position0.x,
                    y = other.position0.y,
                    z = other.position0.z
                },
                position1 = new Vector3
                {
                    x = other.position1.x,
                    y = other.position1.y,
                    z = other.position1.z
                },
                position2 = new Vector3
                {
                    x = other.position2.x,
                    y = other.position2.y,
                    z = other.position2.z
                },
                position3 = new Vector3
                {
                    x = other.position3.x,
                    y = other.position3.y,
                    z = other.position3.z
                },
                regionH = other.regionH,
                regionW = other.regionW,
                regionX = other.regionX,
                regionY = other.regionY,
                tangents = other.tangents,
                texelSize = new Vector2
                {
                    x = other.texelSize.x,
                    y = other.texelSize.y
                },
                untrimmedBoundsDataCenter = new Vector3
                {
                    x = other.untrimmedBoundsDataCenter.x,
                    y = other.untrimmedBoundsDataCenter.y,
                    z = other.untrimmedBoundsDataCenter.z
                },
                untrimmedBoundsDataExtents = new Vector3
                {
                    x = other.untrimmedBoundsDataExtents.x,
                    y = other.untrimmedBoundsDataExtents.y,
                    z = other.untrimmedBoundsDataExtents.z
                }
            };
            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector2 vector in other.uvs)
            {
                uvs.Add(new Vector2
                {
                    x = vector.x,
                    y = vector.y
                });
            }
            result.uvs = uvs.ToArray();
            List<Vector3> colliderVertices = new List<Vector3>();
            foreach (Vector3 vector in other.colliderVertices)
            {
                colliderVertices.Add(new Vector3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                });
            }
            result.colliderVertices = colliderVertices.ToArray();
            return result;
        }
        public static void SetProjectileSpriteRight(this Projectile proj, string name, int pixelWidth, int pixelHeight, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null)
        {
            try
            {
                bool flag = overrideColliderPixelWidth == null;
                if (flag)
                {
                    overrideColliderPixelWidth = new int?(pixelWidth);
                }
                bool flag2 = overrideColliderPixelHeight == null;
                if (flag2)
                {
                    overrideColliderPixelHeight = new int?(pixelHeight);
                }
                float num = (float)pixelWidth / 16f;
                float num2 = (float)pixelHeight / 16f;
                float x = (float)overrideColliderPixelWidth.Value / 16f;
                float y = (float)overrideColliderPixelHeight.Value / 16f;
                proj.GetAnySprite().spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
                tk2dSpriteDefinition tk2dSpriteDefinition = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(12) as Gun).DefaultModule.projectiles[0].GetAnySprite().spriteId].CopyDefinitionFrom();
                tk2dSpriteDefinition.boundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
                tk2dSpriteDefinition.boundsDataExtents = new Vector3(num, num2, 0f);
                tk2dSpriteDefinition.untrimmedBoundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
                tk2dSpriteDefinition.untrimmedBoundsDataExtents = new Vector3(num, num2, 0f);
                tk2dSpriteDefinition.position0 = new Vector3(0f, 0f, 0f);
                tk2dSpriteDefinition.position1 = new Vector3(0f + num, 0f, 0f);
                tk2dSpriteDefinition.position2 = new Vector3(0f, 0f + num2, 0f);
                tk2dSpriteDefinition.position3 = new Vector3(0f + num, 0f + num2, 0f);
                tk2dSpriteDefinition.colliderVertices[1].x = x;
                tk2dSpriteDefinition.colliderVertices[1].y = y;
                tk2dSpriteDefinition.name = name;
                ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[proj.GetAnySprite().spriteId] = tk2dSpriteDefinition;
                proj.baseData.force = 0f;
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:", false);
                ETGModConsole.Log(ex.ToString(), false);
            }


        }



    }










    }

