using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                float chanseToChange = 0.2f; 
                --owner.Pawn.ageTracker.AgeBiologicalTicks;

                if (owner.Pawn.ageTracker.AgeBiologicalYears != owner.bestAge && owner.ifSpliced())
                {

                    owner.controlHediff(HediffDefOfLocal.InBestShape, true);

                    if (owner.Pawn.ageTracker.AgeBiologicalYears > owner.bestAge)
                    {
                        if (rnd.NextDouble() <= chanseToChange + owner.gen.mod)
                        {
                            owner.Pawn.ageTracker.AgeBiologicalTicks -= 3600000L;
                            Log.Message(owner.Pawn.Name + " has became younger.");
                        }
                    }
                    else
                    {
                        if (rnd.NextDouble() <= chanseToChange + owner.gen.mod)
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
            public override void run(ref genoctome owner )
            {
                for ( short i = 0; i < owner.Pawn.health.hediffSet.hediffs.Count; i++ )
                {
                    Hediff hediff = owner.Pawn.health.hediffSet.hediffs[0];

                    Log.Message(hediff.Label);

                    if ( !owner.ifTag( "Biol", hediff.sourceHediffDef.tags ) )  hediff.Heal( owner.gen.mod );
                };
            }
        }
    }
}
