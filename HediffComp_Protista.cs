using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using static Genoctome.BaseGenoctome;

namespace Genoctome
{
    public class HediffComp_Protista : HediffComp
    {
        public HediffCompProperties_Protista Props => (HediffCompProperties_Protista)props;
        public genoctome gen = new genoctome();


        //----------------------------------


        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Log.Message("Protista.Insert");
            gen.Insert(Pawn);

            Log.Message("Protista.perkBook.add & report");
            gen.perkBook.add(new perks.Nutriculiosis_perk(), true, true);
            gen.perkBook.report();
        }


        public override void CompPostTick(ref float severityAdjustment)
        {
            gen.inTick();
        }


        public override void CompPostPostRemoved()
        {
            gen.Remove();
        }
    }

}
