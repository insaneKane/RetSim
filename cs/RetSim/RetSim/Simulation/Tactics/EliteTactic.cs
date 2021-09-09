﻿using RetSim.Data;
using RetSim.Events;
using System;
using System.Collections.Generic;
using static RetSim.Data.Auras;

namespace RetSim.Tactics
{
    public class EliteTactic : Tactic
    {
        Spell trinket1 = null;
        Spell trinket2 = null;

        public EliteTactic()
        {
        }

        public override List<Event> PreFight(FightSimulation fight)
        {
            var onStart = new List<Event>()
            {
                new CastEvent(Spells.SealOfCommand, fight, 0),
                new CastEvent(Spells.AvengingWrath, fight, 1495),
                new CastEvent(Spells.SealOfBlood, fight, 1500),
                new AutoAttackEvent(fight, 1501)
            };

            if (fight.Player.Equipment.Trinket1.OnUse != null)
                trinket1 = Spells.ByID[fight.Player.Equipment.Trinket1.OnUse.ID];

            if (fight.Player.Equipment.Trinket2.OnUse != null)
                trinket2 = Spells.ByID[fight.Player.Equipment.Trinket2.OnUse.ID];

            if (trinket1 != null)
                onStart.Add(new CastEvent(trinket1, fight, 1495));

            else if (trinket2 != null)
                onStart.Add(new CastEvent(trinket2, fight, 1495));

            return onStart;
        }

        public override Event GetActionBetween(int start, int end, FightSimulation fight)
        {
            var swing = fight.Player.TimeOfNextSwing() - start;
            var gcd = fight.Player.GCD.GetDuration(start);
            var cs = fight.Player.Spellbook.IsOnCooldown(Spells.CrusaderStrike) ? fight.Player.Spellbook[Spells.CrusaderStrike.ID].CooldownEnd.Timestamp - start : 0;

            if (!fight.Player.Spellbook.IsOnCooldown(Spells.AvengingWrath) && start > 1500)
                return new CastEvent(Spells.AvengingWrath, fight, fight.Timestamp);

            if (trinket1 != null && !fight.Player.Spellbook.IsOnCooldown(trinket1) && start > 21495)
                return new CastEvent(trinket1, fight, fight.Timestamp);

            if (trinket2 != null && !fight.Player.Spellbook.IsOnCooldown(trinket2) && start > 21495)
                return new CastEvent(trinket2, fight, fight.Timestamp);

            if (gcd == 0 && !fight.Player.Auras[SealOfCommand].Active && swing - gcd > 1510 && end > start + gcd)
            {
                if (fight.Player.Auras.CurrentSeal == SealOfBlood && !fight.Player.Spellbook.IsOnCooldown(Spells.Judgement))
                    return new CastEvent(Spells.Judgement, fight, fight.Timestamp);

                return new CastEvent(Spells.SealOfCommand, fight, start + gcd);
            }

            if (gcd == 0 && fight.Player.Auras[SealOfCommand].Active && end > fight.Player.TimeOfNextSwing() - 390)
                return new CastEvent(Spells.SealOfBlood, fight, Math.Max(fight.Player.TimeOfNextSwing() - 390, start));



            //if (!player.IsOnGCD())
            //{
            //    if (player.Auras.GetRemainingDuration(Glossaries.Auras.SealOfCommand, start) < 5000)
            //        return new CastEvent(start + 0, player, Spells.SealOfCommand);

            //    else if (!player.Spellbook.IsOnCooldown(Spells.CrusaderStrike))
            //        return new CastEvent(start + 0, player, Spells.CrusaderStrike);
            //}

            //if (!fight.Player.GCD.Active && !fight.Player.Spellbook.IsOnCooldown(Spells.CrusaderStrike))
            //    return new CastEvent(Spells.CrusaderStrike, fight, fight.Timestamp);

            else
                return null;
        }
    }
}