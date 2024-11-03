using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SOTS.Items.Flails;
using SOTS.Items.Permafrost;
using SOTS.Items.SpiritStaves;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheBereftSouls.Players
{
    [ExtendsFromMod("SOTS")]
    public class BereftGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool snakeAttachedDebuff = false;

        public override void ResetEffects(NPC npc)
        {
            snakeAttachedDebuff = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (snakeAttachedDebuff)
            {
                if (!npc.boss && BereftSOTSPlayer.SnakeableEnemies.Contains(npc.aiStyle))
                {
                    int clampedDefense = (int)MathHelper.Clamp(npc.defense, 1, 20);
                    npc.velocity = new Vector2(npc.velocity.X * ((1 - (1 / clampedDefense)) * 0.7f), npc.velocity.Y + 0.04f);
                }
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (snakeAttachedDebuff)
            {
                modifiers.Defense.Flat -= 5;
            }
        }
    }
}
