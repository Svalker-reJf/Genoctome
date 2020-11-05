using System;
using System.Collections.Generic;
using Verse;
using static Genoctome.BaseGenoctome;

namespace Genoctome
{
    public class perks
    {
        public static Random rnd = new Random();

        public class Nutriculiosis_perk : perk
        {
            
            public override void run(ref genoctome owner)
            {
                --owner.Pawn.ageTracker.AgeBiologicalTicks;

                if (owner.Pawn.ageTracker.AgeBiologicalYears != owner.bestAge)
                {

                    owner.controlHediff(HediffDefOfLocal.InBestShape, true);

                    if (owner.Pawn.ageTracker.AgeBiologicalYears > owner.bestAge)
                    {
                        if (rnd.NextDouble() <= (owner.gen.mod + owner.specifications.recovery) / 10)
                        {
                            owner.Pawn.ageTracker.AgeBiologicalTicks -= 3600000L;
                            Log.Message(owner.Pawn.Name + " has became younger.");
                        }
                    }
                    else
                    {
                        if (rnd.NextDouble() <= (owner.gen.mod + owner.specifications.recovery) / 10)
                        {
                            owner.Pawn.ageTracker.AgeBiologicalTicks += 3600000L;
                            Log.Message(owner.Pawn.Name + " grew up.");
                        }
                    }
                }
                else owner.controlHediff(HediffDefOfLocal.InBestShape);
            }
        }
        public class PassiveRegeneration_perk : perk
        {
            public override void run(ref genoctome owner) 
            {
                for (short i = 0; i < owner.Pawn.health.hediffSet.hediffs.Count; i++)
                {
                    Hediff hediff = owner.Pawn.health.hediffSet.hediffs[i];
                    
                    if (!genoctome.ifTag("Biol", hediff.def.tags))  hediff.Heal( -(owner.gen.mod + owner.specifications.recovery) / 10); 
                };
            }
        }
        public class ConsusnessUp_perk : perk
        {
            public override void setMoment(ref genoctome owner)
            {
                
            }
            public override void run(ref genoctome owner)
            {
                
            }
        }
    }
}
