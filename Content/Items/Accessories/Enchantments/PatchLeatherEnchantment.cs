using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Items.Earth;
using SOTS.Items.Pyramid;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheBereftSouls.Content.Projectiles.Friendly.Special;
using TheBereftSouls.Players;
using TheBereftSouls.Utils;

namespace TheBereftSouls.Content.Items.Accessories.Enchantments
{
    [ExtendsFromMod("SOTS")]
    public class PatchLeatherEnchantment : ModItem
    {
        int Timer = 0;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Venom] = true;
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }

            NPC target = null;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && player.Center.Distance(npc.Center) < 300)
                {
                    if (npc.GetGlobalNPC<BereftGlobalNPC>().snakeAttachedDebuff)
                    {
                        continue;
                    }
                    if (target == null)
                    {
                        target = npc;
                    }
                    float distanceToNPC = player.Center.Distance(npc.Center);
                    if (distanceToNPC < player.Center.Distance(target.Center))
                    {
                        target = npc;
                    }
                }
            }

            if (target != null && target.active && !target.GetGlobalNPC<BereftGlobalNPC>().snakeAttachedDebuff && Timer == 0)
            {
                Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, player.Center.DirectionTo(target.Center) * 10, ModContent.ProjectileType<SnakeHead>(), 7, 0);
                Timer = 30;
            }
            if (Timer > 0)
            {
                Timer--;
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<PatchLeatherHat>());
            recipe.AddIngredient(ModContent.ItemType<PatchLeatherTunic>());
            recipe.AddIngredient(ModContent.ItemType<PatchLeatherPants>());
            recipe.AddIngredient(ModContent.ItemType<SnakeBow>());
            recipe.AddIngredient(ItemID.PoisonDart, 300);
            recipe.AddIngredient(ModContent.ItemType<SunlightAmulet>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }


        public static NPC GetClosestNPC(Vector2 position, float baseDistance = 1000)
        {
            NPC closestEnemy = null;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && position.Distance(npc.Center) < baseDistance)
                {
                    if (closestEnemy == null)
                    {
                        closestEnemy = npc;
                    }
                    float distanceToNPC = position.Distance(npc.Center);
                    if (distanceToNPC < position.Distance(closestEnemy.Center))
                    {
                        closestEnemy = npc;
                    }
                }
            }
            return closestEnemy;
        }
    }
}

