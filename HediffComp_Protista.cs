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
            gen.Insert(Pawn);

            gen.perkBook.add("Nutr", new perks.Nutriculiosis_perk(), true, true, true);
            gen.perkBook.add("PasReg", new perks.PassiveRegeneration_perk(), true, true, false);
            gen.perkBook.add("FatNeck", new perks.FatNeck_perk(), true, true, false);
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
