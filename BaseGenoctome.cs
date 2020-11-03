using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

using Verse;
using static Verse.HediffComp;

namespace Genoctome
{
    public class BaseGenoctome
    {
        public class specifications
        {
            //! Способность геноктома выполнять задачи :
            public float resistance = 0;    //! сопротивления урону и средам
            public float power = 0;         //! поддерживать и развивать физическую силу
            public float agility = 0;       //! поддерживать и развивать мобильность

            public float recovery = 0;      //! эффективность регенерации и иммунизации
            public float cleverness = 0;    //! развитие навыков

            public float manupulation = 0;  //! контороль колонии вне организма
        }
        public class modifyer
        {
            public float mod { get; set; } = 0;

            public float percent(float total)
            {
                return total / 100 * mod;
            }

            public double percent(double total)
            {
                return (double)percent((float)total);
            }

            public int percent(int total)
            {
                return (int)percent((float)total);
            }

        };
        public enum generation
        {
            PROTO, CIVIL, WORKING, MILITARY, EXPERIMENTAL
        }
        public enum serial
        {
            PROTISTA, PUROFAG, HOBGOBLIN
        }
        public abstract class perk
        {
            public bool available, set;

            public virtual void run(ref genoctome owner)
            {
                
            }
        }
        public class perkBook
        {
            List<perk> _container = new List<perk>();
            public genoctome owner;


            public void add(perk perk, bool available, bool set)
            {
                _container.Add(perk);
                _container[_container.Count - 1].available = available;
                _container[_container.Count - 1].set = set;
            }


            public void run()
            {
                if (owner == null)
                {
                    Log.Message("perkBook don't know owner");
                }  
                else
                    for (short i = 0; i < _container.Count(); i++)
                    {
                        if (_container[i].available && _container[i].set) _container[i].run(ref owner);
                    }
            }


            public void report()
            {
                foreach ( var sic in _container)
                {
                    Log.Message( sic.GetType().ToString() );
                }
            }
        }
        public class genoctome
        {
            public static Random rnd;

            public int bestAge;

            public generation generation;          //! К какому поколению относится Геноктом  -2 2 6 10 14
             public modifyer gen;                  //! Модификатор влияния поколения
            public serial serial;                  //! К какой индивидуальной серии относится Геноктом

            public Pawn Pawn;                      //! Пешка-носитель Геноктома
            public perkBook perkBook;              //! Набор возможных навыков. Добавляйте навыки в HediffComp_Xxxx
            public specifications specifications;  //!
            

            public genoctome()
            {
                rnd = new Random();
                bestAge = rnd.Next(19, 24);

                //! Конкретизация модификатора поколения
                gen = new modifyer();
                switch (generation)
                {
                    case generation.PROTO:
                        gen.mod = -0.02f;
                        break;

                    case generation.CIVIL:
                        gen.mod = 0.02f;
                        break;

                    case generation.WORKING:
                        gen.mod = 0.06f;
                        break;

                    case generation.MILITARY:
                        gen.mod = 0.1f;
                        break;

                    case generation.EXPERIMENTAL:
                        gen.mod = 0.14f;
                        break;
                }

                specifications = new specifications();
            }


            private void Init(Pawn Pawn)
            {
                Log.Message("  pawn in gen");
                this.Pawn = Pawn;

                Log.Message("  gen in perkBook");
                perkBook = new perkBook();
                perkBook.owner = this;
            }


            public void Insert(Pawn Pawn) 
            {
                Log.Message(" genoctom.init");
                Init(Pawn);

                Pawn.health.AddHediff(HediffDefOfLocal.Splice);
            }
            public void inTick()
            {
                perkBook.run();
            }
            public void Remove()
            {
                if (ifSpliced()) Pawn.health.AddHediff(HediffDefOfLocal.PostAmputationInjury);
            }

            //*Добавляет-удаляет болезнь*//
            public void controlHediff(HediffDef Hediff, bool remove = false)
            {
                if (remove)
                {
                    Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(Hediff);
                    if (hediff != null) Pawn.health.RemoveHediff(hediff);

                }
                else Pawn.health.AddHediff(Hediff);
            }

            //*Проверяет завешён ли процесс сращивания*//
            public bool ifSpliced()
            {
                return !Pawn.health.hediffSet.HasHediff(HediffDefOfLocal.Splice);
            }

            //*Проверяет на естественность*//
            public bool ifTag(string tag, List<string> tags)
            {
                for (short i = 0; i < tags.Count; i++)
                {
                    if (tags[i] == tag) return true;
                }

                return false;
            }
        }

    }
}
