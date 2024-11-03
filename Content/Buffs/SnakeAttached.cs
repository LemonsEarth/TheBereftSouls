using Terraria;
using Terraria.ModLoader;
using TheBereftSouls.Players;

namespace TheBereftSouls.Content.Buffs
{
    /// <summary>
    /// Used by Patch Leather Enchantment.
    /// </summary>
    [ExtendsFromMod("SOTS")]
    public class SnakeAttached : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BereftGlobalNPC>().snakeAttachedDebuff = true;
        }
    }
}
