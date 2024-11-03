using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS.Projectiles.Earth;
using Steamworks;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBereftSouls.Content.Buffs;
using TheBereftSouls.Content.Tiles.Special;
using TheBereftSouls.Players;
using TheBereftSouls.Utils;

namespace TheBereftSouls.Content.Projectiles.Friendly.Special
{
    [ExtendsFromMod("SOTS")]
    public class SnakeHead : ModProjectile
    {
        public const string BodyTexturePath = "TheBereftSouls/Content/Projectiles/Friendly/Special/SnakeBody";
        public static Asset<Texture2D> BodyTexture;

        bool Attached
        {
            get { return Projectile.ai[0] == 1; }
            set { Projectile.ai[0] = value ? 1 : 0; }
        }

        ref float AttachedEnemy => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void Load()
        {
            BodyTexture = ModContent.Request<Texture2D>(BodyTexturePath);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = owner.Center.DirectionTo(Projectile.Center).ToRotation();

            if (Attached)
            {
                if (Main.npc[(int)AttachedEnemy] != null && Main.npc[(int)AttachedEnemy].active)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Main.npc[(int)AttachedEnemy].Center;
                    Projectile.netUpdate = true;
                }
                if (Main.npc[(int)AttachedEnemy] == null || !Main.npc[(int)AttachedEnemy].active || Projectile.Center.Distance(owner.Center) > 250)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.Center.Distance(owner.Center) > 400)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Attached = true;
            AttachedEnemy = target.whoAmI;
            target.AddBuff(ModContent.BuffType<SnakeAttached>(), 30);
            target.AddBuff(BuffID.Venom, 30);
            Projectile.timeLeft -= target.defense;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];
            BereftSOTSPlayer bPlayer = owner.GetModPlayer<BereftSOTSPlayer>();

            Vector2 projToPlayer = (owner.Center - Projectile.Center);
            int segmentCount = (int)projToPlayer.Length() / 16;

            for (int i = 0; i < segmentCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var dust = Dust.NewDustDirect(Projectile.Center + projToPlayer.SafeNormalize(Vector2.Zero) * i * 16, 16, 16, DustID.IchorTorch, Scale: Main.rand.NextFloat(0.5f, 2f));
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.player[Projectile.owner] == null || Main.player[Projectile.owner].dead)
            {
                return true;
            }
            Player player = Main.player[Projectile.owner];
            Vector2 bodyDrawPos = player.Center;
            Vector2 playerToHead = Projectile.Center - player.Center;
            float rotation = playerToHead.SafeNormalize(Vector2.Zero).ToRotation();
            float segmentHeight = BodyTexture.Value.Height;
            int drawnSegments = 0;
            float distanceLeft = playerToHead.Length() + segmentHeight / 2;

            if (segmentHeight == 0)
            {
                segmentHeight = 16;
            }

            while (distanceLeft > 0f)
            {
                bodyDrawPos += playerToHead.SafeNormalize(Vector2.Zero) * segmentHeight;
                distanceLeft = bodyDrawPos.Distance(Projectile.Center);
                drawnSegments++;
                distanceLeft -= segmentHeight;
                Main.EntitySpriteDraw(BodyTexture.Value, bodyDrawPos - Main.screenPosition, null, lightColor, rotation, new Vector2(BodyTexture.Value.Width / 2f, BodyTexture.Value.Height / 2f), 1f, SpriteEffects.None);
            }

            return true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}
