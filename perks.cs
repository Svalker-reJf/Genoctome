using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using static Genoctome.BaseGenoctome;
using static Iz14_07.reflection;

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
                for (short i = 0; i < owner.Pawn.health.hediffSet.hediffs.Count; ++i)
                {
                    Hediff hediff = owner.Pawn.health.hediffSet.hediffs[i];
                    
                    if (!genoctome.ifTag("Biol", hediff.def.tags))  hediff.Heal( -(owner.gen.mod + owner.specifications.recovery) / 10); 
                };
            }
        }
        public class ConsusnessUp_perk : perk
        {
            public override void run(ref genoctome owner)
            {
                
            }
        }
        public class FatNeck_perk : perk
        {
            public override void enter(ref genoctome owner)
            {
                /*
                Log.Message(genoctome.takeBodyPart(owner.Pawn, "Neck").def.hitPoints.ToString());
                BodyPartDef donor = BodyPartDefOf.Neck;
                BodyPartRecord parentRecipient = genoctome.takeBodyPart(owner.Pawn, "Neck");
                Log.Message($" Neck.hit={BodyPartDefOf.Neck.hitPoints}, def={parentRecipient.def.hitPoints}");
                parentRecipient.def = new BodyPartDef();
                Log.Message($" Neck.hit={BodyPartDefOf.Neck.hitPoints}, def={parentRecipient.def.hitPoints}");

                if (parentRecipient != null)
                {
                    genoctome.copyTo(donor, parentRecipient.def);
                    Log.Message($" Neck.hit={BodyPartDefOf.Neck.hitPoints}, def={parentRecipient.def.hitPoints}");
                    parentRecipient.def.hitPoints += 12;
                    Log.Message($" Neck.hit={BodyPartDefOf.Neck.hitPoints}, def={parentRecipient.def.hitPoints}");
                }*/

                speaker_withLog speaker = new speaker_withLog();

                Log.Message($"type def is {owner.Pawn.def.GetType().Name}");
                ThingDef buffer = null;
                
                copyTo(ref owner.Pawn.def, ref buffer, true, speaker);
                    Log.Message("set buffer");

                    Log.Message("new ThingDef");

                copyTo(ref buffer, ref owner.Pawn.def, true, speaker);
                    Log.Message("copy ThingDef");

                genoctome.takeBodyPart(owner.Pawn, "Neck").def.hitPoints += 12;
                    Log.Message("change hitHoints");


            }

        }

    }
}
